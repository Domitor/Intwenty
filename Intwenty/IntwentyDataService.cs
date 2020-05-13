using System;
using Microsoft.Extensions.Options;
using Intwenty.Data;
using Intwenty.Engine;
using Intwenty.Model;
using System.Collections.Generic;
using System.Linq;
using Intwenty.Data.Dto;
using Intwenty.Data.Entity;
using Intwenty.Data.DBAccess.Helpers;
using Intwenty.Data.DBAccess;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace Intwenty
{
    public interface IIntwentyDataService
    {
        /// <summary>
        /// Creates a new application JSON Document including defaultvalues.
        /// </summary>
        /// <returns>An OperationResult including a json object</returns>
        OperationResult CreateNew(ClientStateInfo state);

        /// <summary>
        /// Saves application data
        /// </summary>
        /// <returns>An OperationResult including Id and Version for the saved application</returns>
        OperationResult Save(ClientStateInfo state);

        /// <summary>
        /// Deletes all application data (maintable and subtables) by id.
        /// If the application uses versioning, all versions are deleted.
        /// </summary>
        /// <returns>An OperationResult including Id and Version for the deleted application</returns>
        OperationResult DeleteById(ClientStateInfo state);

        /// <summary>
        /// Deletes data by Id
        /// Parameter Id can be an Id of an application subtable row, or an application maintable Id
        /// Parameter dbname can be an application  subtable name or main tablename
        /// If the dbname represents a main application table, all application data (maintable and subtables) is deleted.
        /// If the dbname represents an application subtable, only the subtable row that matches the id parameter is deleted.
        /// If the application uses versioning, all versions are deleted.
        /// </summary>
        /// <returns>An OperationResult including Id and Version for the deleted application</returns>
        OperationResult DeleteById(int applicationid, int id, string dbname);


        /// <summary>
        /// Get the latest version data for and application based on Id
        /// </summary>
        /// <returns>An OperationResult including a json object</returns>
        OperationResult GetLatestVersionById(ClientStateInfo state);

        /// <summary>
        /// Get the latest version data for and application based on OwnerUserId
        /// </summary>
        /// <returns>An OperationResult including a json object</returns>
        OperationResult GetLatestVersionByOwnerUser(ClientStateInfo state);


        /// <summary>
        /// Get a list of (latest version) application data that matches the filter specified in args. 
        /// If there's a LISTVIEW defined for the application, the columns in the list view is returned otherwise all columns.
        /// This function supports paging. It returns the number of records specified in args.BatchSize
        /// </summary>
        /// <returns>An OperationResult including a json array and the current paging rownum</returns>
        OperationResult GetList(ListRetrivalArgs args);

        /// <summary>
        /// Get a list of (latest version) application data, based on OwnedBy and that matches the filter specified in args. 
        /// If there's a LISTVIEW defined for the application, the columns in the list view is returned otherwise all columns.
        /// This function supports paging. It returns the number of records specified in BatchSize
        /// </summary>
        /// <returns>An OperationResult including a json array and the current paging rownum</returns>
        OperationResult GetListByOwnerUser(ListRetrivalArgs args);

        /// <summary>
        /// Get a list of (latest version) application data. 
        /// All columns from the application's main table is returned.
        /// </summary>
        /// <returns>An OperationResult including a json array</returns>
        OperationResult GetList(int applicationid);

        /// <summary>
        /// Get a list of (latest version) application data based on OwnedBy. 
        /// All columns from the application's main table is returned.
        /// </summary>
        /// <returns>An OperationResult including a json array</returns>
        OperationResult GetListByOwnerUser(int applicationid, string owneruserid);

        /// <summary>
        /// Get a list of all versions for an application based on Id
        /// </summary>
        /// <returns>An OperationResult including a json array</returns>
        OperationResult GetVersionListById(ClientStateInfo state);

        /// <summary>
        /// Get the data for an application based on Id and Version
        /// </summary>
        /// <returns>An OperationResult including a json object</returns>
        OperationResult GetVersion(ClientStateInfo state);

        /// <summary>
        /// Get value domains used by UI in the application specified by application id
        /// </summary>
        OperationResult GetValueDomains(int ApplicationId);

        /// <summary>
        /// Get all value domains.
        /// </summary>
        OperationResult GetValueDomains();

        /// <summary>
        /// Gets a list of data based on the DataView defined by args.DataViewMetaCode and that matches the filter specified in args.
        /// </summary>
        /// <returns>An OperationResult including a json array</returns>
        OperationResult GetDataView(ListRetrivalArgs args);

        /// <summary>
        /// Gets  the first record of data based on the DataView defined by args.DataViewMetaCode and that matches the filter specified in args.
        /// </summary>
        /// <returns>An OperationResult including a json object</returns>
        OperationResult GetDataViewRecord(ListRetrivalArgs args);

        OperationResult Validate(ApplicationModel model, ClientStateInfo state);

      
        void LogError(string message, int applicationid=0, string appmetacode="NONE", string username="");

        void LogWarning(string message, int applicationid = 0, string appmetacode = "NONE", string username = "");

        void LogInfo(string message, int applicationid = 0, string appmetacode = "NONE", string username = "");

        IIntwentyDbORM GetDbObjectMapper();

    }

    public class IntwentyDataService : IIntwentyDataService
    {
        
        private IntwentySettings Settings { get; }

        private DBMS DBMSType { get; }

        private bool IsNoSql { get; }

        private IIntwentyModelService ModelRepository { get; }

        private IMemoryCache ApplicationCache { get; }

        public IntwentyDataService(IOptions<IntwentySettings> settings, IIntwentyModelService modelservice, IMemoryCache cache)
        {
            Settings = settings.Value;
            ModelRepository = modelservice;
            DBMSType = Settings.DefaultConnectionDBMS;
            IsNoSql = settings.Value.IsNoSQL;
            ApplicationCache = cache;
        }

        public IIntwentyDbORM GetDbObjectMapper()
        {
            if (IsNoSql)
            {
                var t = new IntwentyNoSqlDbClient(DBMSType, Settings.DefaultConnection);
                return t;
            }
            else
            {
                var t = new IntwentySqlDbClient(DBMSType, Settings.DefaultConnection);
                return t;
            }

        }

        public OperationResult CreateNew(ClientStateInfo state)
        {
            var result = new OperationResult();

            try
            {

                var model = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == state.ApplicationId);
                if (model == null)
                {
                    result = new OperationResult(false, string.Format("Could not find application model for applicationid {0} when creating a new empty app", state.ApplicationId));
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

                        IIntwentyDbORM client = null;
                        if (IsNoSql)
                            client = new IntwentyNoSqlDbClient(DBMSType, Settings.DefaultConnection);
                        else
                            client = new IntwentySqlDbClient(DBMSType, Settings.DefaultConnection);


                        var result = client.GetAll<DefaultValue>();
                        var current = result.Find(p => p.ApplicationId == model.Application.Id && p.ColumnName == dbcol.DbName);
                        if (current == null)
                        {

                            var firstval = string.Format("{0}{1}", prefix, (istart));
                            current = new DefaultValue() { ApplicationId = model.Application.Id, ColumnName = dbcol.DbName, GeneratedDate = DateTime.Now, TableName = model.Application.DbName, Count = istart, LatestValue = firstval };
                            client.Insert(current);
                            res.Add(current);

                        }
                        else
                        {
                            current.Count += iseed;
                            current.LatestValue = string.Format("{0}{1}", prefix, current.Count);
                            client.Update(current);
                            res.Add(current);
                        }
                    }

                }

            }

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

                var validation = Validate(model, state);
                if (validation.IsSuccess)
                {
                    RemoveFromApplicationCache(state.ApplicationId, state.Id);

                    if (IsNoSql)
                    {
                        var t = NoSqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentyNoSqlDbClient(DBMSType, Settings.DefaultConnection));
                        var result = t.Save(state);
                        return result;
                    }
                    else
                    {
                        var t = SqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentySqlDbClient(DBMSType, Settings.DefaultConnection));
                        var result = t.Save(state);
                        return result;
                    }


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
                result.AddMessage("USERERROR", string.Format("Save Intwenty application failed"));
                result.AddMessage("SYSTEMERROR", ex.Message);
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

                if (IsNoSql)
                {
                    var t = NoSqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentyNoSqlDbClient(DBMSType, Settings.DefaultConnection));
                    return t.DeleteById(state);
                }
                else
                {
                    var t = SqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentySqlDbClient(DBMSType, Settings.DefaultConnection));
                    return t.DeleteById(state);
                }


            }
            catch (Exception ex)
            {
                var result = new OperationResult();
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", string.Format("Delete Intwenty application by Id failed"));
                result.AddMessage("SYSTEMERROR", ex.Message);
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
                    var client = GetDbObjectMapper();
                    var t = client.GetOne<SystemID>(id);
                    if (t == null)
                        throw new InvalidOperationException(string.Format("Could not find parent id when deleting row in subtable {0}", dbname));
                    if (t.ParentId < 1)
                        throw new InvalidOperationException(string.Format("Could not find parent id when deleting row in subtable {0}", dbname));

                    RemoveFromApplicationCache(applicationid, t.ParentId);
                }
                else
                {
                    RemoveFromApplicationCache(applicationid, id);
                }
                if (IsNoSql)
                {
                    var t = NoSqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentyNoSqlDbClient(DBMSType, Settings.DefaultConnection));
                    return t.DeleteById(id, dbname);
                }
                else
                {
                    var t = SqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentySqlDbClient(DBMSType, Settings.DefaultConnection));
                    return t.DeleteById(id, dbname);
                }


            }
            catch (Exception ex)
            {
                var result = new OperationResult();
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", string.Format("DeleteById(applicationid,id,dbname) failed"));
                result.AddMessage("SYSTEMERROR", ex.Message);
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


                if (IsNoSql)
                {
                    var t = NoSqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentyNoSqlDbClient(DBMSType, Settings.DefaultConnection));
                    return t.GetList(args);
                }
                else
                {
                    var t = SqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentySqlDbClient(DBMSType, Settings.DefaultConnection));
                    return t.GetList(args);
                }
            }
            catch (Exception ex)
            {
                var result = new OperationResult();
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", string.Format("GetList(args) of Intwenty applications failed"));
                result.AddMessage("SYSTEMERROR", ex.Message);
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

                if (IsNoSql)
                {
                    var t = NoSqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentyNoSqlDbClient(DBMSType, Settings.DefaultConnection));
                    return t.GetListByOwnerUser(args);
                }
                else
                {
                    var t = SqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentySqlDbClient(DBMSType, Settings.DefaultConnection));
                    return t.GetListByOwnerUser(args);
                }

            }
            catch (Exception ex)
            {
                var result = new OperationResult();
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", string.Format("GetListByOwnerUser(args) of Intwenty applications failed"));
                result.AddMessage("SYSTEMERROR", ex.Message);
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

                if (IsNoSql)
                {
                    var t = NoSqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentyNoSqlDbClient(DBMSType, Settings.DefaultConnection));
                    result = t.GetList();

                    if (result.IsSuccess)
                        ApplicationCache.Set(string.Format("APPLIST_APPID_{0}", applicationid), result);

                    return result;
                }
                else
                {
                    var t = SqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentySqlDbClient(DBMSType, Settings.DefaultConnection));
                    result = t.GetList();

                    if (result.IsSuccess)
                        ApplicationCache.Set(string.Format("APPLIST_APPID_{0}", applicationid), result);

                    return result;
                }
            }
            catch (Exception ex)
            {
                var result = new OperationResult();
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", string.Format("GetList(applicationid) of Intwenty applications failed"));
                result.AddMessage("SYSTEMERROR", ex.Message);
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

                if (IsNoSql)
                {
                    var t = NoSqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentyNoSqlDbClient(DBMSType, Settings.DefaultConnection));
                    result = t.GetListByOwnerUser(owneruserid);

                    if (result.IsSuccess)
                    {
                        ApplicationCache.Set(string.Format("APPLIST_APPID_{0}_OWNER_{1}", applicationid, owneruserid), result);
                        AddCachedUser(owneruserid);

                    }
                    return result;

                }
                else
                {
                    var t = SqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentySqlDbClient(DBMSType, Settings.DefaultConnection));
                    result = t.GetListByOwnerUser(owneruserid);

                    if (result.IsSuccess)
                    {
                        ApplicationCache.Set(string.Format("APPLIST_APPID_{0}_OWNER_{1}", applicationid, owneruserid), result);
                        AddCachedUser(owneruserid);
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                var result = new OperationResult();
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", string.Format("GetListByOwnerUser(applicationid, owneruserid) of Intwenty applications failed"));
                result.AddMessage("SYSTEMERROR", ex.Message);
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

                if (IsNoSql)
                {
                    var t = NoSqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentyNoSqlDbClient(DBMSType, Settings.DefaultConnection));
                    result = t.GetLatestVersionById(state);
                    if (result.IsSuccess)
                    {
                        ApplicationCache.Set(string.Format("APP_APPID_{0}_ID_{1}", state.ApplicationId, state.Id), result);
                    }
                    return result;
                }
                else
                {
                    var t = SqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentySqlDbClient(DBMSType, Settings.DefaultConnection));
                    result = t.GetLatestVersionById(state);
                    if (result.IsSuccess)
                    {
                        ApplicationCache.Set(string.Format("APP_APPID_{0}_ID_{1}", state.ApplicationId, state.Id), result);
                    }
                    return result;
                }

            }
            catch (Exception ex)
            {
                var result = new OperationResult();
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", string.Format("GetLatestVersionById(state) of Intwenty application failed"));
                result.AddMessage("SYSTEMERROR", ex.Message);
                result.Data = "{}";
                return result;
            }
        }

        public OperationResult GetLatestVersionByOwnerUser(ClientStateInfo state)
        {
            try
            {
                if (state.ApplicationId < 1)
                    throw new InvalidOperationException("Parameter state must be a valid ApplicationId");

                var model = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == state.ApplicationId);
                if (model == null)
                    throw new InvalidOperationException(string.Format("state.ApplicationId {0} is not representing a valid application model", state.ApplicationId));

                if (IsNoSql)
                {
                    var t = NoSqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentyNoSqlDbClient(DBMSType, Settings.DefaultConnection));
                    return t.GetLatestVersionByOwnerUser(state);
                }
                else
                {
                    var t = SqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentySqlDbClient(DBMSType, Settings.DefaultConnection));
                    return t.GetLatestVersionByOwnerUser(state);
                }
            }
            catch (Exception ex)
            {
                var result = new OperationResult();
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", string.Format("GetLatestVersionByOwnerUser(state) of Intwenty application failed"));
                result.AddMessage("SYSTEMERROR", ex.Message);
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

                if (IsNoSql)
                {
                    var t = NoSqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentyNoSqlDbClient(DBMSType, Settings.DefaultConnection));
                    return t.GetApplicationValueDomains();
                }
                else
                {
                    var t = SqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentySqlDbClient(DBMSType, Settings.DefaultConnection));
                    return t.GetApplicationValueDomains();
                }

            }
            catch (Exception ex)
            {
                var result = new OperationResult();
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", string.Format("GetValueDomains(applicationid) used in an Intwenty application failed"));
                result.AddMessage("SYSTEMERROR", ex.Message);
                result.Data = "{}";
                return result;
            }

        }

        public OperationResult GetValueDomains()
        {
   

            if (IsNoSql)
            {
                var t = NoSqlDbDataManager.GetDataManager(null, ModelRepository, Settings, new IntwentyNoSqlDbClient(DBMSType, Settings.DefaultConnection));
                return t.GetValueDomains();
            }
            else
            {
                var t = SqlDbDataManager.GetDataManager(null, ModelRepository, Settings, new IntwentySqlDbClient(DBMSType, Settings.DefaultConnection));
                return t.GetValueDomains();
            }

        }

        public OperationResult Validate(ApplicationModel app, ClientStateInfo state)
        {

            foreach (var t in app.UIStructure)
            {
                if (t.IsDataColumnConnected && t.DataColumnInfo.Mandatory)
                {
                    var dv = state.Data.Values.FirstOrDefault(p => p.DbName == t.DataColumnInfo.DbName);
                    if (dv != null && !dv.HasValue)
                    {
                        return new OperationResult(false, string.Format("The field {0} is mandatory", t.Title),state.Id, state.Version);
                    }
                    foreach (var table in state.Data.SubTables)
                    {
                        foreach (var row in table.Rows)
                        {
                            dv = row.Values.Find(p => p.DbName == t.DataColumnInfo.DbName);
                            if (dv != null && !dv.HasValue)
                            {
                                return new OperationResult(false, string.Format("The field {0} is mandatory", t.Title), state.Id, state.Version);
                            }
                        }

                    }
                   
                }
            }

            return new OperationResult(true, "Successfully validated", state.Id, state.Version);
        }

       

        public OperationResult GetDataView(ListRetrivalArgs args)
        {
            if (IsNoSql)
            {
                var t = NoSqlDbDataManager.GetDataManager(null, ModelRepository, Settings, new IntwentyNoSqlDbClient(DBMSType, Settings.DefaultConnection));
                return t.GetDataView(args);
            }
            else
            {
                var t = SqlDbDataManager.GetDataManager(null, ModelRepository, Settings, new IntwentySqlDbClient(DBMSType, Settings.DefaultConnection));
                return t.GetDataView(args);
            }

          
        }

        public OperationResult GetDataViewRecord(ListRetrivalArgs args)
        {
            if (IsNoSql)
            {
                var t = NoSqlDbDataManager.GetDataManager(null, ModelRepository, Settings, new IntwentyNoSqlDbClient(DBMSType, Settings.DefaultConnection));
                return t.GetDataViewRecord(args);
            }
            else
            {
                var t = SqlDbDataManager.GetDataManager(null, ModelRepository, Settings, new IntwentySqlDbClient(DBMSType, Settings.DefaultConnection));
                return t.GetDataViewRecord(args);
            }

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

            try
            {
              

                if (IsNoSql)
                {
                    var t = new EventLog() { ApplicationId=applicationid, AppMetaCode=appmetacode, EventDate=DateTime.Now, Message=message, UserName=username, Verbosity=verbosity };
                    var client = new IntwentyNoSqlDbClient(DBMSType, Settings.DefaultConnection);
                    client.Insert(t);
                }
                else
                {
                    var getdatecmd = DBHelpers.GetDBMSCommandMap().Find(p => p.Key == "GETDATE" && p.DbEngine == Settings.DefaultConnectionDBMS);
                    var client = new IntwentySqlDbClient(DBMSType, Settings.DefaultConnection);
                    client.Open();
                    client.CreateCommand("INSERT INTO sysdata_EventLog (EventDate, Verbosity, Message, AppMetaCode, ApplicationId,UserName) VALUES ("+getdatecmd.Command+", @Verbosity, @Message, @AppMetaCode, @ApplicationId,@UserName)");
                    client.AddParameter("@Verbosity", verbosity);
                    client.AddParameter("@Message", message);
                    client.AddParameter("@AppMetaCode", appmetacode);
                    client.AddParameter("@ApplicationId", applicationid);
                    client.AddParameter("@UserName", username);
                    client.ExecuteNonQuery();
                    client.Close();
                }

            }
            catch (Exception ex)
            {
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
