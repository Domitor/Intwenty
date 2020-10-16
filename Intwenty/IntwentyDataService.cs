using System;
using Microsoft.Extensions.Options;
using Intwenty.Data;
using Intwenty.Model;
using System.Collections.Generic;
using System.Linq;
using Intwenty.Data.Dto;
using Intwenty.Data.Entity;
using Intwenty.Data.Helpers;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Intwenty.DataClient;
using System.Runtime.InteropServices;
using Intwenty.DataClient.Model;
using Intwenty.Interface;

namespace Intwenty
{
   

    public class IntwentyDataService : IIntwentyDataService
    {
        
        private IntwentySettings Settings { get; }

        private DBMS DBMSType { get; }

        private IIntwentyModelService ModelRepository { get; }

        private IMemoryCache ApplicationCache { get; }

        private IDataManager DataManager { get; }

        public IntwentyDataService(IOptions<IntwentySettings> settings, IIntwentyModelService modelservice, IMemoryCache cache, IDataManager datamanager)
        {
            Settings = settings.Value;
            ModelRepository = modelservice;
            DBMSType = Settings.DefaultConnectionDBMS;
            ApplicationCache = cache;
            DataManager = datamanager;
        }

        public IDataClient GetDataClient()
        {
           return new Connection(DBMSType, Settings.DefaultConnection);
        }

        public OperationResult CreateNew(ClientStateInfo state)
        {
            var result = new OperationResult();

            try
            {

                var model = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == state.ApplicationId);
                if (model == null)
                {
                    result = new OperationResult(false, MessageCode.SYSTEMERROR, string.Format("Could not find application model for applicationid {0} when creating a new empty app", state.ApplicationId));
                    result.Data = "{}";
                    return result;
                }

                var sb = new StringBuilder();
                sb.Append("{");
                sb.Append("\"" + model.Application.DbName + "\":{");

                var sep = "";
                var defval = GetDefaultValues(state);
                if (defval.Count > 0)
                {
                    foreach (var df in defval)
                    {
                        sb.Append(sep + DBHelpers.GetJSONValue(df.ColumnName, df.LatestValue));
                        sep = ",";
                    }
                }
                sb.Append("}");

                foreach (var dbtbl in model.DataStructure)
                {
                    if (dbtbl.IsMetaTypeDataTable && dbtbl.IsRoot)
                    {
                        sb.Append(",\""+ dbtbl .DbName+ "\":[]");
                    }
                }
               
                sb.Append("}");

                result.Data = sb.ToString();
                result.SetSuccess("Generated a new empty application.");

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Error when creating a new application");
            }

            return result;
        }

        private List<DefaultValue> GetDefaultValues(ClientStateInfo state)
        {
            var res = new List<DefaultValue>();
            var model = ModelRepository.GetApplicationModels().Find(p=> p.Application.Id == state.ApplicationId);
            if (model == null)
                return new List<DefaultValue>();

            var client = new Connection(DBMSType, Settings.DefaultConnection);
            client.Open();

            foreach (var dbcol in model.DataStructure)
            {
                if (dbcol.IsMetaTypeDataColumn && dbcol.IsRoot)
                {
                    if (dbcol.HasPropertyWithValue("DEFVALUE", "AUTO"))
                    {
                        var start = dbcol.GetPropertyValue("DEFVALUE_START");
                        var seed = dbcol.GetPropertyValue("DEFVALUE_SEED");
                        var prefix = dbcol.GetPropertyValue("DEFVALUE_PREFIX");
                        int istart = Convert.ToInt32(start);
                        int iseed = Convert.ToInt32(seed);

                        var result = client.GetEntities<DefaultValue>();
                        var current = result.Find(p => p.ApplicationId == model.Application.Id && p.ColumnName == dbcol.DbName);
                        if (current == null)
                        {

                            var firstval = string.Format("{0}{1}", prefix, (istart));
                            current = new DefaultValue() { ApplicationId = model.Application.Id, ColumnName = dbcol.DbName, GeneratedDate = DateTime.Now, TableName = model.Application.DbName, Count = istart, LatestValue = firstval };
                            client.InsertEntity(current);
                            res.Add(current);

                        }
                        else
                        {
                            current.Count += iseed;
                            current.LatestValue = string.Format("{0}{1}", prefix, current.Count);
                            client.UpdateEntity(current);
                            res.Add(current);
                        }
                    }

                }

            }

            client.Close();

            return res;

        }

       

        public OperationResult Save(ClientStateInfo state)
        {
            try
            {
                if (state.ApplicationId < 1)
                    throw new InvalidOperationException("Parameter state must contain a valid ApplicationId");

                var model = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == state.ApplicationId);
                if (model == null)
                    throw new InvalidOperationException(string.Format("state.ApplicationId {0} is not representing a valid application model", state.ApplicationId));

                var validation = DataManager.Validate(model, state);
                if (validation.IsSuccess)
                {
                    RemoveFromApplicationCache(state.ApplicationId, state.Id);

                    var result = DataManager.Save(model,state);
                    return result;

                }
                else
                {
                    return validation;
                }
            }
            catch (Exception ex)
            {
                var result = new OperationResult();
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage(MessageCode.USERERROR, string.Format("Save Intwenty application failed"));
                result.AddMessage(MessageCode.SYSTEMERROR, ex.Message);
                result.Data = "{}";
                return result;
            }
        }

        public OperationResult DeleteById(ClientStateInfo state)
        {
            try
            {
                if (state.ApplicationId < 1)
                    throw new InvalidOperationException("Parameter state must contain a valid ApplicationId");

                var model = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == state.ApplicationId);
                if (model == null)
                    throw new InvalidOperationException(string.Format("state.ApplicationId {0} is not representing a valid application model", state.ApplicationId));

                RemoveFromApplicationCache(state.ApplicationId, state.Id);

                return DataManager.DeleteById(model, state);
                

            }
            catch (Exception ex)
            {
                var result = new OperationResult();
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage(MessageCode.USERERROR, string.Format("Delete Intwenty application by Id failed"));
                result.AddMessage(MessageCode.SYSTEMERROR, ex.Message);
                result.Data = "{}";
                return result;
            }
        }

        public OperationResult DeleteById(int applicationid, int id, string dbname)
        {
            try
            {
                if (applicationid < 1)
                    throw new InvalidOperationException("Parameter applicationid must contain a valid ApplicationId");

                if (id < 1)
                    throw new InvalidOperationException("Parameter id can not be zero");

                var model = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == applicationid);
                if (model == null)
                    throw new InvalidOperationException(string.Format("state.ApplicationId {0} is not representing a valid application model", applicationid));

                var modelitem = model.DataStructure.Find(p => p.DbName.ToLower() == dbname.ToLower());
                if (modelitem == null)
                    throw new InvalidOperationException("The dbname did not match the application {0} dbname or any of it's subtables");
                if (modelitem.IsMetaTypeDataTable)
                {
                    var client = GetDataClient();
                    client.Open();
                    var sysid = client.GetEntity<SystemID>(id);
                    client.Close();
                    if (sysid == null)
                        throw new InvalidOperationException(string.Format("Could not find parent id when deleting row in subtable {0}", dbname));
                    if (sysid.ParentId < 1)
                        throw new InvalidOperationException(string.Format("Could not find parent id when deleting row in subtable {0}", dbname));

                    RemoveFromApplicationCache(applicationid, sysid.ParentId);
                }
                else
                {
                    RemoveFromApplicationCache(applicationid, id);
                }
              
                return DataManager.DeleteById(model, id, dbname);
                
            }
            catch (Exception ex)
            {
                var result = new OperationResult();
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage(MessageCode.USERERROR, string.Format("DeleteById(applicationid,id,dbname) failed"));
                result.AddMessage(MessageCode.SYSTEMERROR, ex.Message);
                result.Data = "{}";
                return result;
            }
        }


        public OperationResult GetList(ListRetrivalArgs args)
        {
  
            try
            {

                if (args.ApplicationId < 1)
                    throw new InvalidOperationException("Parameter args must contain a valid ApplicationId");

                var model = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == args.ApplicationId);
                if (model == null)
                    throw new InvalidOperationException(string.Format("args.ApplicationId {0} is not representing a valid application model", args.ApplicationId));

                 return DataManager.GetList(model, args);
                
            }
            catch (Exception ex)
            {
                var result = new OperationResult();
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage(MessageCode.USERERROR, string.Format("GetList(args) of Intwenty applications failed"));
                result.AddMessage(MessageCode.SYSTEMERROR, ex.Message);
                result.Data = "[]";
                return result;
            }
        }

        public OperationResult GetListByOwnerUser(ListRetrivalArgs args)
        {


            try
            {
                if (args.ApplicationId < 1)
                    throw new InvalidOperationException("Parameter args must contain a valid ApplicationId");

                var model = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == args.ApplicationId);
                if (model == null)
                    throw new InvalidOperationException(string.Format("args.ApplicationId {0} is not representing a valid application model", args.ApplicationId));

              
                return DataManager.GetListByOwnerUser(model, args);
                

            }
            catch (Exception ex)
            {
                var result = new OperationResult();
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage(MessageCode.USERERROR, string.Format("GetListByOwnerUser(args) of Intwenty applications failed"));
                result.AddMessage(MessageCode.SYSTEMERROR, ex.Message);
                result.Data = "[]";
                return result;
            }

}

        public OperationResult GetList(int applicationid)
        {
            try
            {
                OperationResult result = null;

                if (applicationid < 1)
                    throw new InvalidOperationException("Parameter applicationid must be a valid ApplicationId");

                var model = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == applicationid);
                if (model == null)
                    throw new InvalidOperationException(string.Format("applicationid {0} is not representing a valid application model", applicationid));

               
                if (ApplicationCache.TryGetValue(string.Format("APPLIST_APPID_{0}", applicationid), out result))
                {
                    return result;
                }

            
                result = DataManager.GetList(model);

                if (result.IsSuccess)
                    ApplicationCache.Set(string.Format("APPLIST_APPID_{0}", applicationid), result);

                return result;
                
            }
            catch (Exception ex)
            {
                var result = new OperationResult();
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage(MessageCode.USERERROR, string.Format("GetList(applicationid) of Intwenty applications failed"));
                result.AddMessage(MessageCode.SYSTEMERROR, ex.Message);
                result.Data = "[]";
                return result;
            }

        }

        public OperationResult GetListByOwnerUser(int applicationid, string owneruserid)
        {
            try
            {
                OperationResult result = null;

                if (applicationid < 1)
                    throw new InvalidOperationException("Parameter applicationid must be a valid ApplicationId");

                if (string.IsNullOrEmpty(owneruserid))
                    throw new InvalidOperationException("Parameter owneruserid must not be empty");

                var model = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == applicationid);
                if (model == null)
                    throw new InvalidOperationException(string.Format("applicationid {0} is not representing a valid application model", applicationid));

                if (ApplicationCache.TryGetValue(string.Format("APPLIST_APPID_{0}_OWNER_{1}", applicationid, owneruserid), out result))
                {
                    return result;
                }

               
                result = DataManager.GetListByOwnerUser(model, owneruserid);

                if (result.IsSuccess)
                {
                    ApplicationCache.Set(string.Format("APPLIST_APPID_{0}_OWNER_{1}", applicationid, owneruserid), result);
                    AddCachedUser(owneruserid);
                }

                return result;
                
            }
            catch (Exception ex)
            {
                var result = new OperationResult();
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage(MessageCode.USERERROR, string.Format("GetListByOwnerUser(applicationid, owneruserid) of Intwenty applications failed"));
                result.AddMessage(MessageCode.SYSTEMERROR, ex.Message);
                result.Data = "[]";
                return result;
            }

        }



        public OperationResult GetLatestVersionById(ClientStateInfo state)
        {
            try
            {
                OperationResult result = null;

                if (state.ApplicationId < 1)
                    throw new InvalidOperationException("Parameter state must be a valid ApplicationId");

                var model = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == state.ApplicationId);
                if (model == null)
                    throw new InvalidOperationException(string.Format("state.ApplicationId {0} is not representing a valid application model", state.ApplicationId));

                if (ApplicationCache.TryGetValue(string.Format("APP_APPID_{0}_ID_{1}", state.ApplicationId, state.Id), out result))
                {
                    return result;
                }

                result = DataManager.GetLatestVersionById(model, state);
                if (result.IsSuccess)
                {
                    ApplicationCache.Set(string.Format("APP_APPID_{0}_ID_{1}", state.ApplicationId, state.Id), result);
                }
                return result;
                

            }
            catch (Exception ex)
            {
                var result = new OperationResult();
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage(MessageCode.USERERROR, string.Format("GetLatestVersionById(state) of Intwenty application failed"));
                result.AddMessage(MessageCode.SYSTEMERROR, ex.Message);
                result.Data = "{}";
                return result;
            }
        }

        public OperationResult GetLatestVersionByOwnerUser(ClientStateInfo state)
        {
            try
            {
                if (state.ApplicationId < 1)
                    throw new InvalidOperationException("Parameter state must contain a valid ApplicationId");

                var model = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == state.ApplicationId);
                if (model == null)
                    throw new InvalidOperationException(string.Format("state.ApplicationId {0} is not representing a valid application model", state.ApplicationId));

               
                return DataManager.GetLatestVersionByOwnerUser(model,state);
                
            }
            catch (Exception ex)
            {
                var result = new OperationResult();
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage(MessageCode.USERERROR, string.Format("GetLatestVersionByOwnerUser(state) of Intwenty application failed"));
                result.AddMessage(MessageCode.SYSTEMERROR, ex.Message);
                result.Data = "{}";
                return result;
            }

        }

       

        public OperationResult GetVersionListById(ClientStateInfo state)
        {
            throw new NotImplementedException();
        }

        public OperationResult GetVersion(ClientStateInfo state)
        {
            throw new NotImplementedException();
        }



        public OperationResult GetValueDomains(int applicationid)
        {
            try
            {
                if (applicationid < 1)
                    throw new InvalidOperationException("Parameter applicationid must be a valid ApplicationId");

                var model = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == applicationid);
                if (model == null)
                    throw new InvalidOperationException(string.Format("applicationid {0} is not representing a valid application model", applicationid));

                return DataManager.GetApplicationValueDomains(model);
                

            }
            catch (Exception ex)
            {
                var result = new OperationResult();
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage(MessageCode.USERERROR, string.Format("GetValueDomains(applicationid) used in an Intwenty application failed"));
                result.AddMessage(MessageCode.SYSTEMERROR, ex.Message);
                result.Data = "{}";
                return result;
            }

        }

        public OperationResult GetValueDomains()
        {
            return DataManager.GetValueDomains();
        }

        public List<ValueDomainModelItem> GetValueDomainItems()
        {
            return ModelRepository.GetValueDomains();
        }

        public List<ValueDomainModelItem> GetValueDomainItems(string domainname)
        {
            return ModelRepository.GetValueDomains().Where(p => p.DomainName.ToUpper() == domainname.ToUpper()).ToList();
        }

        public OperationResult Validate(ClientStateInfo state)
        {
            if (state==null)
                throw new InvalidOperationException("Parameter state cannot be null");
            if (state.ApplicationId < 1)
                throw new InvalidOperationException("Parameter state must contain a valid ApplicationId");

            var model = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == state.ApplicationId);

            return DataManager.Validate(model, state);
        }

       

        public OperationResult GetDataView(ListRetrivalArgs args)
        {
            return DataManager.GetDataView(args); 
        }

        public OperationResult GetDataViewRecord(ListRetrivalArgs args)
        {
          
            return DataManager.GetDataViewRecord(args);
            
        }

        public void LogError(string message, int applicationid = 0, string appmetacode = "NONE", string username = "")
        {
            LogEvent("ERROR",message,applicationid,appmetacode,username);
        }

        public void LogInfo(string message, int applicationid = 0, string appmetacode = "NONE", string username = "")
        {
            LogEvent("INFO",message, applicationid, appmetacode, username);
        }

        public void LogWarning(string message, int applicationid = 0, string appmetacode = "NONE", string username = "")
        {
            LogEvent("WARNING", message, applicationid, appmetacode, username);
        }

        private void LogEvent(string verbosity, string message, int applicationid = 0, string appmetacode = "NONE", string username = "")
        {

            var client = new Connection(DBMSType, Settings.DefaultConnection);
            client.Open();

            try
            {

                var parameters = new List<IIntwentySqlParameter>();
                parameters.Add(new IntwentySqlParameter("@Verbosity", verbosity));
                parameters.Add(new IntwentySqlParameter("@Message", message));
                parameters.Add(new IntwentySqlParameter("@AppMetaCode", appmetacode));
                parameters.Add(new IntwentySqlParameter("@ApplicationId", applicationid));
                parameters.Add(new IntwentySqlParameter("@UserName", username));

               

                var getdatecmd = client.GetDbCommandMap().Find(p => p.Key == "GETDATE" && p.DbEngine == Settings.DefaultConnectionDBMS);
               
                client.RunCommand("INSERT INTO sysdata_EventLog (EventDate, Verbosity, Message, AppMetaCode, ApplicationId,UserName) VALUES ("+getdatecmd.Command+", @Verbosity, @Message, @AppMetaCode, @ApplicationId,@UserName)", parameters:parameters.ToArray());

                client.Close();
                
            }
            catch (Exception ex)
            {
                client.Close();
                throw ex;
            }
        }

        private void AddCachedUser(string userid)
        {

            List<string> users;
            if (ApplicationCache.TryGetValue("CACHEDUSERS", out users))
            {
                if (!users.Contains(userid))
                    users.Add(userid);
            }
            else
            {
                users = new List<string>();
                users.Add(userid);
            }

            ApplicationCache.Set("CACHEDUSERS", users);
        }

        private void RemoveFromApplicationCache(int applicationid, int id)
        {
            ApplicationCache.Remove(string.Format("APP_APPID_{0}_ID_{1}", applicationid, id));
            ApplicationCache.Remove(string.Format("APPLIST_APPID_{0}", applicationid));

            List<string> users;
            if (ApplicationCache.TryGetValue("CACHEDUSERS", out users))
            {
                foreach (var u in users)
                {
                    ApplicationCache.Remove(string.Format("APPLIST_APPID_{0}_OWNER_{1}",applicationid,u));
                }
            }
           
        }


    }
}
