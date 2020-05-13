using System;
using Intwenty.Data.DBAccess;
using Intwenty.Data.DBAccess.Helpers;
using Intwenty.Data.Dto;
using Intwenty.Data.Entity;
using Intwenty.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intwenty.Engine
{
    public class NoSqlDbDataManager : IDataManager
    {
        protected IIntwentyModelService ModelRepository { get; set; }

        protected ApplicationModel Model { get; set; }

        protected IntwentyNoSqlDbClient NoSqlClient { get; set; }

        protected IntwentySettings Settings { get; set; }

        protected DateTime ApplicationSaveTimeStamp { get; set; }


        protected NoSqlDbDataManager(ApplicationModel model, IIntwentyModelService modelservice, IntwentySettings settings, IntwentyNoSqlDbClient nosqlclient)
        {
            Settings = settings;
            Model = model;
            NoSqlClient = nosqlclient;
            ModelRepository = modelservice;
            ApplicationSaveTimeStamp = DateTime.Now;
        }

        public static NoSqlDbDataManager GetDataManager(ApplicationModel model, IIntwentyModelService modelservice, IntwentySettings settings, IntwentyNoSqlDbClient nosqlclient)
        {


            if (model != null && model.Application.MetaCode == "XXXXX")
            {
                return null; //Return custom datamanager
            }
            else
            {
                var t = new NoSqlDbDataManager(model, modelservice, settings, nosqlclient);
                return t;
            }
        }

        public OperationResult ConfigureDatabase()
        {
            return new OperationResult(true, string.Format("{0} configured for use with NoSql", Model.Application.Title));
        }

       
        public OperationResult GetVersionListById(ClientStateInfo state)
        {
            throw new NotImplementedException();
        }

        public OperationResult GetVersion(ClientStateInfo state)
        {
            throw new NotImplementedException();
        }

        public virtual OperationResult DeleteById(ClientStateInfo state)
        {
            if (state == null)
                return new OperationResult(false, "No client state found when performing DeleteById(state).", 0, 0);

            if (state.Id < 1)
                return new OperationResult(false, "No state.Id found when performing DeleteById(state).", 0, 0);

            var result = new OperationResult(true, string.Format("Deleted application {0}", this.Model.Application.Title), state.Id, 0);

            try
            {

                NoSqlClient.DeleteIntwentyJsonObject(this.Model.Application.DbName, state.Id);
                NoSqlClient.DeleteIntwentyJsonObject(this.Model.Application.VersioningTableName, state.Id);

                foreach (var table in Model.DataStructure)
                {
                    if (table.IsMetaTypeDataTable)
                    {
                        
                        var expression = new IntwentyExpression(string.Format("{0} = {1}", "ParentId", "@ParentId"));
                        expression.AddParameter(new IntwentyParameter() { ParameterName = "@ParentId", Value = state.Id, DataType = System.Data.DbType.Int32 });
                        var t = NoSqlClient.GetDataSet(table.DbName, expression);
                        if (t == null)
                            continue;

                        foreach (var row in t.Rows)
                        {
                            NoSqlClient.DeleteIntwentyJsonObject(table.DbName, row.Id);
                            NoSqlClient.DeleteJsonDocument("sysdata_SystemID", row.Id);
                        }
                    }
                }

              
                NoSqlClient.DeleteJsonDocument("sysdata_InformationStatus", state.Id);
                NoSqlClient.DeleteJsonDocument("sysdata_SystemID", state.Id);


            }
            catch (Exception ex)
            {
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", string.Format("Delete application {0} failed", this.Model.Application.Title));
                result.AddMessage("SYSTEMERROR", ex.Message);
            }

            return result;
        }

        public virtual OperationResult DeleteById(int id, string dbname)
        {
            if (id < 1)
                return new OperationResult(false, "No id found when performing DeleteById(id, dbname).", 0, 0);
            if (string.IsNullOrEmpty(dbname))
                return new OperationResult(false, "No dbname found when performing DeleteById(id, dbname).", 0, 0);

            var result = new OperationResult(true, string.Format("Deleted application {0}", this.Model.Application.Title), id, 0);

            try
            {

    

                if (dbname.ToLower() == this.Model.Application.DbName)
                {
                   
                    NoSqlClient.DeleteIntwentyJsonObject(this.Model.Application.DbName, id);
                    NoSqlClient.DeleteIntwentyJsonObject(this.Model.Application.VersioningTableName, id);

                    foreach (var table in Model.DataStructure)
                    {
                        if (table.IsMetaTypeDataTable)
                        {

                            var expression = new IntwentyExpression(string.Format("{0} = {1}", "ParentId", "@ParentId"));
                            expression.AddParameter(new IntwentyParameter() { ParameterName = "@ParentId", Value = id, DataType= System.Data.DbType.Int32 });
                            var t = NoSqlClient.GetDataSet(table.DbName, expression);
                            if (t == null)
                                continue;

                            foreach (var row in t.Rows)
                            {
                                NoSqlClient.DeleteIntwentyJsonObject(table.DbName, row.Id);
                                NoSqlClient.DeleteJsonDocument("sysdata_SystemID", row.Id);
                            }
                        }
                    }
                    NoSqlClient.DeleteJsonDocument("sysdata_InformationStatus", id);
                    NoSqlClient.DeleteJsonDocument("sysdata_SystemID", id);

                }
                else
                {
                    foreach (var table in Model.DataStructure)
                    {
                        if (table.IsMetaTypeDataTable && table.DbName.ToLower() == dbname)
                        {
                           
                            NoSqlClient.DeleteIntwentyJsonObject(table.DbName, id);
                            NoSqlClient.DeleteJsonDocument("sysdata_SystemID", id);

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", string.Format("DeleteById(id, dbaname) in application {0} failed", this.Model.Application.Title));
                result.AddMessage("SYSTEMERROR", ex.Message);
            }

            return result;
        }


        public OperationResult GetDataView(ListRetrivalArgs args)
        {
            var result = new OperationResult();

            try
            {

                var viewinfo = ModelRepository.GetDataViewModels();

                if (args == null)
                    throw new InvalidOperationException("Call to GetDataView without ListRetrivalArgs");

                result.IsSuccess = true;
                result.RetriveListArgs = new ListRetrivalArgs();
                result.RetriveListArgs = args;


                var dv = viewinfo.Find(p => p.MetaCode == args.DataViewMetaCode && p.IsMetaTypeDataView);
                if (dv == null)
                    throw new InvalidOperationException("Could not find dataview to fetch");
                if (dv.HasNonSelectSql)
                    throw new InvalidOperationException(string.Format("The sql query defined for dataview {0} has invalid statements.", dv.Title + " (" + dv.MetaCode + ")"));

                var tablename = dv.QueryTableDbName;
                if (string.IsNullOrEmpty(tablename))
                    throw new InvalidOperationException(string.Format("Could not find tablename/collectionname for the {0} dataview query", dv.Title));


                var columns = new List<IIntwentyDataColum>();
                foreach (var viewcol in viewinfo)
                {
                    if ((viewcol.IsMetaTypeDataViewColumn || viewcol.IsMetaTypeDataViewKeyColumn) && viewcol.ParentMetaCode == dv.MetaCode)
                    {
                        columns.Add(new IntwentyDataColumn() { ColumnName = viewcol.SQLQueryFieldName });
                    }
                }


                IntwentyExpression expression = null;
                if (!string.IsNullOrEmpty(args.FilterField) && !string.IsNullOrEmpty(args.FilterValue))
                {
                    expression = new IntwentyExpression(string.Format("{0} LIKE '%{1}%'", args.FilterField, "@" + args.FilterField));
                    expression.AddParameter(new IntwentyParameter() { ParameterName = "@" + args.FilterField, Value = args.FilterValue });
                }

                result.AddMessage("RESULT", string.Format("Fetched dataview {0}", dv.Title));

                StringBuilder jsonresult = null;
                if (expression==null)
                    jsonresult = NoSqlClient.GetJsonArray(tablename, columns, result.RetriveListArgs.CurrentRowNum, (result.RetriveListArgs.CurrentRowNum + result.RetriveListArgs.BatchSize));
                else
                    jsonresult = NoSqlClient.GetJsonArray(tablename, expression, columns, result.RetriveListArgs.CurrentRowNum, (result.RetriveListArgs.CurrentRowNum + result.RetriveListArgs.BatchSize));

                result.Data = jsonresult.ToString();

            }
            catch (Exception ex)
            {
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", "Fetch dataview failed");
                result.AddMessage("SYSTEMERROR", ex.Message);
                result.Data = "{}";
            }

            return result;
        }

        public OperationResult GetDataViewRecord(ListRetrivalArgs args)
        {

            var result = new OperationResult(true, "Fetched dataview value", 0, 0);


            try
            {
                var viewinfo = ModelRepository.GetDataViewModels();

                if (args == null)
                    throw new InvalidOperationException("Call to GetDataViewValue without ListRetrivalArgs");

                var dv = viewinfo.Find(p => p.MetaCode == args.DataViewMetaCode && p.IsMetaTypeDataView);
                if (dv == null)
                    throw new InvalidOperationException("Could not find dataview to fetch value from");

                if (dv.HasNonSelectSql)
                    throw new InvalidOperationException(string.Format("The sql query defined for dataview {0} has invalid statements.", dv.Title + " (" + dv.MetaCode + ")"));

                var tablename = dv.QueryTableDbName;
                if (string.IsNullOrEmpty(tablename))
                    throw new InvalidOperationException(string.Format("Could not find tablename/collectionname for the {0} dataview query", dv.Title));


                result.RetriveListArgs = new ListRetrivalArgs();
                result.RetriveListArgs = args;

              
                IntwentyExpression expression = null;
                var keyfield = viewinfo.Find(p => p.IsMetaTypeDataViewKeyColumn && p.ParentMetaCode == args.DataViewMetaCode);
                if (keyfield != null)
                {
                    expression = new IntwentyExpression(string.Format("{0} = {1}", keyfield.SQLQueryFieldName, "@" + keyfield.SQLQueryFieldName));
                    expression.AddParameter(new IntwentyParameter() { ParameterName = "@" + keyfield.SQLQueryFieldName, Value = args.FilterValue });
                }
                else
                {
                    throw new InvalidOperationException("Could not create expression beacause of no DataViewKeyColumn");
                }

    
                var columns = new List<IIntwentyDataColum>();
                foreach (var viewcol in viewinfo)
                {
                    if ((viewcol.IsMetaTypeDataViewColumn || viewcol.IsMetaTypeDataViewKeyColumn) && viewcol.ParentMetaCode == args.DataViewMetaCode)
                    {
                        columns.Add(new IntwentyDataColumn() { ColumnName = viewcol.SQLQueryFieldName, DataType = viewcol.DataType });
                    }
                }


                var jsonresult = NoSqlClient.GetJsonObject(tablename, expression, columns);
                result.Data = jsonresult.ToString();

            }
            catch (Exception ex)
            {
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", "Fetch dataview failed");
                result.AddMessage("SYSTEMERROR", ex.Message);
                result.Data = "{}";
            }

            return result;
        }


        public virtual OperationResult GetLatestVersionById(ClientStateInfo state)
        {
            if (state == null)
                return new OperationResult(false, "state was null when executing SqlDbManager.GetLatestVersionById.", 0, 0);
            if (state.Id < 0)
                return new OperationResult(false, "Id is required when executing SqlDbManager.GetLatestVersionById.", 0, 0);
            if (state.ApplicationId < 0)
                return new OperationResult(false, "ApplicationId is required when executing SqlDbManager.GetLatestVersionById.", 0, 0);


            return GetLatestVersion(state);
        }

        public virtual OperationResult GetLatestVersionByOwnerUser(ClientStateInfo state)
        {
            if (state == null)
                return new OperationResult(false, "state was null when executing SqlDbManager.GetLatestVersionByOwnerUser.", 0, 0);
            if (string.IsNullOrEmpty(state.OwnerUserId))
                return new OperationResult(false, "OwnerUserId is required when executing SqlDbManager.GetLatestVersionByOwnerUser.", 0, 0);
            if (state.ApplicationId < 0)
                return new OperationResult(false, "ApplicationId is required when executing SqlDbManager.GetLatestVersionByOwnerUser.", 0, 0);


            var istatlist = NoSqlClient.GetAll<InformationStatus>().Where(p=> p.ApplicationId== state.ApplicationId && p.OwnedBy == state.OwnerUserId);
            var t = istatlist.Max(p => p.Id);
            if (t < 1)
                return new OperationResult(false, "Requested data could not be found.");

            var istat = istatlist.First(p => p.Id == t);


            state.Id = istat.Id;
            state.Version = istat.Version;

            return GetLatestVersion(state);
        }

        private OperationResult GetLatestVersion(ClientStateInfo state)
        {
            var jsonresult = new StringBuilder();
            var result = new OperationResult(true, string.Format("Fetched latest version for application {0}", this.Model.Application.Title), state.Id, state.Version);
            
            try
            {

                jsonresult.Append("{");

                //MAIN TABLE
                var appjson = NoSqlClient.GetIntwentyJsonObject(this.Model.Application.DbName, state.Id);

                if (appjson.Length < 5)
                {
                    jsonresult.Append("}");
                    result.Messages.Clear();
                    result.Data = jsonresult.ToString();
                    result.IsSuccess = false;
                    result.AddMessage("USERERROR", string.Format("Get latest version for application {0} returned no data", this.Model.Application.Title));
                    result.AddMessage("SYSTEMERROR", string.Format("Get latest version for application {0} returned no data", this.Model.Application.Title));
                    return result;
                }

                jsonresult.Append("\"" + this.Model.Application.DbName + "\":" + appjson.ToString());
               

                //SUBTABLES
                foreach (var t in this.Model.DataStructure)
                {
                    if (t.IsMetaTypeDataTable && t.IsRoot)
                    {
                        var expression = new IntwentyExpression(string.Format("{0} = {1}", "ParentId", "@ParentId"));
                        expression.AddParameter(new IntwentyParameter() { ParameterName = "@ParentId", Value = state.Id, DataType = System.Data.DbType.Int32 });

                        var tablejson = NoSqlClient.GetJsonArray(t.DbName);
                        jsonresult.Append(",\"" + t.DbName + "\":" + tablejson.ToString());
                    }
                }

                jsonresult.Append("}");
                result.Data = jsonresult.ToString();
            }
            catch (Exception ex)
            {
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", string.Format("Get latest version for application {0} failed", this.Model.Application.Title));
                result.AddMessage("SYSTEMERROR", ex.Message);
                result.Data = "{}";

            }

            return result;
        }

        public OperationResult GetList(ListRetrivalArgs args)
        {
            if (args == null)
                return new OperationResult(false, "Can't get list without ListRetrivalArgs", 0, 0);

            var result = new OperationResult(true, string.Format("Fetched list for application {0}", this.Model.Application.Title), 0, 0);

            if (args.MaxCount == 0)
                args.MaxCount = NoSqlClient.GetJsonDocumentCount(this.Model.Application.DbName);

            result.RetriveListArgs = new ListRetrivalArgs();
            result.RetriveListArgs = args;

            try
            {
                var json = NoSqlClient.GetJsonArray(this.Model.Application.DbName, null, result.RetriveListArgs.CurrentRowNum, (result.RetriveListArgs.CurrentRowNum + result.RetriveListArgs.BatchSize));
                result.Data = json.ToString();

            }
            catch (Exception ex)
            {
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", string.Format("Fetch list for application {0} failed", this.Model.Application.Title));
                result.AddMessage("SYSTEMERROR", ex.Message);
                result.Data = "[]";

            }

            return result;
        }

        public OperationResult GetList()
        {

            var result = new OperationResult(true, string.Format("Fetched list for application {0}", this.Model.Application.Title), 0, 0);

            try
            {
                var json = NoSqlClient.GetJsonArray(this.Model.Application.DbName);
                result.Data = json.ToString();

            }
            catch (Exception ex)
            {
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", string.Format("Fetch list for application {0} failed", this.Model.Application.Title));
                result.AddMessage("SYSTEMERROR", ex.Message);
                result.Data = "[]";

            }

            return result;
        }

        public OperationResult GetListByOwnerUser(ListRetrivalArgs args)
        {
            if (args == null)
                return new OperationResult(false, "Can't get list without ListRetrivalArgs", 0, 0);

            var result = new OperationResult(true, string.Format("Fetched list for application {0}", this.Model.Application.Title), 0, 0);

            if (args.MaxCount == 0)
                args.MaxCount = NoSqlClient.GetJsonDocumentCount(this.Model.Application.DbName);

            result.RetriveListArgs = new ListRetrivalArgs();
            result.RetriveListArgs = args;

            try
            {
                var expression = new IntwentyExpression(string.Format("{0} = {1}", "OwnedBy", "@OwnedBy"));
                expression.AddParameter(new IntwentyParameter() { ParameterName = "@OwnedBy", Value = args.OwnerUserId });
                var json = NoSqlClient.GetJsonArray(this.Model.Application.DbName, expression, null, result.RetriveListArgs.CurrentRowNum, (result.RetriveListArgs.CurrentRowNum + result.RetriveListArgs.BatchSize));
                result.Data = json.ToString();

            }
            catch (Exception ex)
            {
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", string.Format("Fetch list for application {0} failed", this.Model.Application.Title));
                result.AddMessage("SYSTEMERROR", ex.Message);
                result.Data = "[]";

            }

            return result;
        }

        public OperationResult GetListByOwnerUser(string owneruserid)
        {

            var result = new OperationResult(true, string.Format("Fetched list for application {0}", this.Model.Application.Title), 0, 0);

            try
            {

                var expression = new IntwentyExpression(string.Format("{0} = {1}", "OwnedBy", "@OwnedBy"));
                expression.AddParameter(new IntwentyParameter() { ParameterName = "@OwnedBy", Value = owneruserid });
                var json = NoSqlClient.GetJsonArray(this.Model.Application.DbName, expression, null, 0, 0);
                result.Data = json.ToString();

            }
            catch (Exception ex)
            {
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", string.Format("Fetch list for application {0} failed", this.Model.Application.Title));
                result.AddMessage("SYSTEMERROR", ex.Message);
                result.Data = "[]";

            }

            return result;
        }

        public OperationResult GetApplicationValueDomains()
        {

            var sb = new StringBuilder();
            var result = new OperationResult(true, string.Format("Fetched doamins used in ui for application {0}", this.Model.Application.Title), 0, 0);

            try
            {
                var valuedomains = new List<string>();


                //COLLECT DOMAINS AND VIEWS USED BY UI
                foreach (var t in this.Model.UIStructure)
                {
                    if (t.HasValueDomain)
                    {
                        var domainparts = t.Domain.Split(".".ToCharArray()).ToList();
                        if (domainparts.Count >= 2)
                        {
                            if (!valuedomains.Contains(domainparts[1]))
                                valuedomains.Add(domainparts[1]);
                        }
                    }
                }

                sb.Append("{");
                var domainindex = 0;
                foreach (var d in valuedomains)
                {

                    var expression = new IntwentyExpression(string.Format("{0} = {1}", "DomainName", "@DomainName"));
                    expression.AddParameter(new IntwentyParameter() { ParameterName = "@DomainName", Value = d });
                    var jsonresult = NoSqlClient.GetJsonArray("sysmodel_ValueDomainItem", expression);

                    if (domainindex == 0)
                        sb.Append("\"" + "VALUEDOMAIN_" + d + "\":");
                    else
                        sb.Append(",\"" + "VALUEDOMAIN_" + d + "\":");

                    sb.Append(jsonresult.ToString());
                    domainindex += 1;
                }
                sb.Append("}");

                result.Data = sb.ToString();

            }
            catch (Exception ex)
            {
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", string.Format("Fetch valuedomains for application {0} failed", this.Model.Application.Title));
                result.AddMessage("SYSTEMERROR", ex.Message);
                result.Data = "[]";

            }

            return result;
        }

        public OperationResult GetValueDomains()
        {
           
            var sb = new StringBuilder();
            var result = new OperationResult(true, "Fetched all value domins", 0, 0);

            try
            {
                var allitems = NoSqlClient.GetAll<ValueDomainItem>();
                var valuedomains = new List<string>();

                foreach (var t in allitems)
                {
                    if (!valuedomains.Contains(t.DomainName))
                        valuedomains.Add(t.DomainName);

                }


                sb.Append("{");
                var domainindex = 0;
                foreach (var d in valuedomains)
                {
                    var expression = new IntwentyExpression(string.Format("{0} = {1}", "DomainName", "@DomainName"));
                    expression.AddParameter(new IntwentyParameter() { ParameterName = "@DomainName", Value = d });
                    var jsonresult = NoSqlClient.GetJsonArray("sysmodel_ValueDomainItem", expression);

                    if (domainindex == 0)
                        sb.Append("\"" + "VALUEDOMAIN_" + d + "\":");
                    else
                        sb.Append(",\"" + "VALUEDOMAIN_" + d + "\":");

                    sb.Append(jsonresult.ToString());
                    domainindex += 1;
                }
                sb.Append("}");

                result.Data = sb.ToString();

            }
            catch (Exception ex)
            {
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", "Fetch all valuedomains failed");
                result.AddMessage("SYSTEMERROR", ex.Message);
                result.Data = "[]";

            }

            return result;
        }

        public OperationResult GetVersion()
        {
            throw new NotImplementedException();
        }

        public OperationResult Save(ClientStateInfo state)
        {

            if (state == null)
                return new OperationResult(false, "No client state found when performing save application.", 0, 0);

            var result = new OperationResult(true, string.Format("Saved application {0}", this.Model.Application.Title), state.Id, state.Version);

            try
            {
                //CONNECT MODEL TO DATA
                state.Data.InferModel(Model);

                BeforeSave();

                if (state.Id < 1)
                {
                    state.Id = GetNewSystemID("APPLICATION", this.Model.Application.MetaCode, state);
                    result.Status = LifecycleStatus.NEW_NOT_SAVED;
                    state.Version = CreateVersionRecord(state);
                    BeforeSaveNew();
                    var informationstatus = InsertInformationStatus(state);
                    InsertMainTable(state);
                    HandleSubTables(state);
                    result.Status = LifecycleStatus.NEW_SAVED;
                }
                else if (state.Id > 0 && this.Model.Application.UseVersioning)
                {
                    result.Status = LifecycleStatus.EXISTING_NOT_SAVED;
                    state.Version = CreateVersionRecord(state);
                    BeforeSaveUpdate();
                    var informationstatus = UpdateInformationStatus(state);
                    UpdateMainTable(state);
                    HandleSubTables(state);
                    result.Status = LifecycleStatus.EXISTING_SAVED;
                }
                else if (state.Id > 0 && !this.Model.Application.UseVersioning)
                {
                    result.Status = LifecycleStatus.EXISTING_NOT_SAVED;
                    BeforeSaveUpdate();
                    var informationstatus = UpdateInformationStatus(state);
                    UpdateMainTable(state);
                    HandleSubTables(state);
                    result.Status = LifecycleStatus.EXISTING_SAVED;
                }


                result.Id = state.Id;
                result.Version = state.Version;

                AfterSave();

            }
            catch (Exception ex)
            {
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", string.Format("Save application {0} failed", this.Model.Application.Title));
                result.AddMessage("SYSTEMERROR", ex.Message);
            }

            return result;
        }

        private int CreateVersionRecord(ClientStateInfo state)
        {

            var expression = new IntwentyExpression(string.Format("{0} = {1} AND {2} = {3} AND {4} = {5}", new string[] { "MetaCode", "@MetaCode", "MetaType", "@MetaType", "Id","@Id" }));
            expression.AddParameter(new IntwentyParameter() { ParameterName = "@MetaCode", Value = this.Model.Application.MetaCode });
            expression.AddParameter(new IntwentyParameter() { ParameterName = "@MetaType", Value = "APPLICATION" });
            expression.AddParameter(new IntwentyParameter() { ParameterName = "@Id", Value = state.Id });


            int newversion = 1;
            var t = NoSqlClient.GetDataSet(this.Model.Application.VersioningTableName, expression);
            if (t != null && t.Rows.Count>0)
            {
                var max = t.Rows.Max(p => p.Version);
                if (max > 0)
                    newversion = max + 1;

            }


            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append(string.Format("\"{0}\":{1}", "Id", state.Id));
            sb.Append(string.Format(",\"{0}\":{1}", "Version", newversion));
            sb.Append(string.Format(",\"{0}\":{1}", "ApplicationId", this.Model.Application.Id));
            sb.Append(string.Format(",\"{0}\":\"{1}\"", "MetaCode", this.Model.Application.MetaCode));
            sb.Append(string.Format(",\"{0}\":\"{1}\"", "MetaType", "APPLICATION"));
            sb.Append(string.Format(",\"{0}\":\"{1}\"", "ChangedDate", DateTime.Now));
            sb.Append(string.Format(",\"{0}\":{1}", "ParentId", 0));
            sb.Append("}");


            NoSqlClient.InsertIntwentyJsonObject(sb.ToString(), this.Model.Application.VersioningTableName, state.Id, newversion);


            return newversion;
        }

        private int GetNewSystemID(string metatype, string metacode, ClientStateInfo state)
        {
            if (metatype == DatabaseModelItem.MetaTypeDataTable)
                return NoSqlClient.GetAutoIncrementalId(this.Model.Application.Id, state.Id, metatype, metacode, state.Properties);
            else
                return NoSqlClient.GetAutoIncrementalId(this.Model.Application.Id, 0, metatype, metacode, state.Properties);
        }

        private InformationStatus InsertInformationStatus(ClientStateInfo state)
        {
            var model = new InformationStatus()
            {
                Id = state.Id,
                ApplicationId = this.Model.Application.Id,
                ChangedBy = state.UserId,
                ChangedDate = DateTime.Now,
                CreatedBy = state.UserId,
                MetaCode = this.Model.Application.MetaCode,
                OwnedBy = state.OwnerUserId,
                PerformDate = DateTime.Now,
                Version = state.Version
            };

            NoSqlClient.Insert(model);

            return model;

        }

        private InformationStatus UpdateInformationStatus(ClientStateInfo state)
        {
            var model = new InformationStatus()
            {
                Id = state.Id,
                ApplicationId = this.Model.Application.Id,
                ChangedBy = state.UserId,
                ChangedDate = DateTime.Now,
                CreatedBy = state.UserId,
                MetaCode = this.Model.Application.MetaCode,
                OwnedBy = state.OwnerUserId,
                PerformDate = DateTime.Now,
                Version = state.Version
            };

            NoSqlClient.Update(model);

            return model;

        }

        private void InsertMainTable(ClientStateInfo state)
        {
            if (state.Id < 1)
                throw new InvalidOperationException("Invalid systemid");

            var json = new StringBuilder();
            json.Append("{");
            json.Append(DBHelpers.GetJSONValue("Id", state.Id));
            json.Append("," + DBHelpers.GetJSONValue("Version", state.Version));
            json.Append("," + DBHelpers.GetJSONValue("ApplicationId", this.Model.Application.Id));
            json.Append("," + DBHelpers.GetJSONValue("CreatedBy", state.UserId));
            json.Append("," + DBHelpers.GetJSONValue("ChangedBy", state.UserId));
            json.Append("," + DBHelpers.GetJSONValue("OwnedBy", state.OwnerUserId));
            json.Append("," + DBHelpers.GetJSONValue("ChangedDate", DateTime.Now));

            foreach (var t in state.Data.Values)
            {
                if (!t.HasModel)
                    continue;

                json.Append("," + DBHelpers.GetJSONValue(t.Value, t.Model));

            }
            json.Append("}");

            NoSqlClient.InsertIntwentyJsonObject(json.ToString(), this.Model.Application.DbName, state.Id);

            if (this.Model.Application.UseVersioning)
            {
                NoSqlClient.InsertIntwentyJsonObject(json.ToString(), this.Model.Application.DbName+"_Revision", state.Id, state.Version);
            }

        }

        private void UpdateMainTable(ClientStateInfo state)
        {
            if (state.Id < 1)
                throw new InvalidOperationException("Invalid systemid");

            var json = new StringBuilder();
            json.Append("{");
            json.Append(DBHelpers.GetJSONValue("Id", state.Id ));
            json.Append("," + DBHelpers.GetJSONValue("Version", state.Version));
            json.Append("," + DBHelpers.GetJSONValue("ApplicationId", this.Model.Application.Id));
            json.Append("," + DBHelpers.GetJSONValue("CreatedBy", state.UserId));
            json.Append("," + DBHelpers.GetJSONValue("ChangedBy", state.UserId));
            json.Append("," + DBHelpers.GetJSONValue("OwnedBy", state.OwnerUserId));
            json.Append("," + DBHelpers.GetJSONValue("ChangedDate", DateTime.Now));


            foreach (var t in state.Data.Values)
            {
                if (!t.HasModel)
                    continue;

                json.Append("," + DBHelpers.GetJSONValue(t.Value, t.Model));

            }
            json.Append("}");

            NoSqlClient.UpdateIntwentyJsonObject(json.ToString(), this.Model.Application.DbName, state.Id);

            if (this.Model.Application.UseVersioning)
            {
                NoSqlClient.InsertIntwentyJsonObject(json.ToString(), this.Model.Application.DbName + "_Revision", state.Id, state.Version);
            }
        }

        private void HandleSubTables(ClientStateInfo state)
        {
            foreach (var table in state.Data.SubTables)
            {
                if (!table.HasModel)
                    continue;

                foreach (var row in table.Rows)
                {
                    if (row.Id < 1)
                    {
                        InsertTableRow(row, state);

                    }
                    else
                    {
                        UpdateTableRow(row, state);

                    }

                }

            }

        }

        private void InsertTableRow(ApplicationTableRow data, ClientStateInfo state)
        {
            var rowid = GetNewSystemID(DatabaseModelItem.MetaTypeDataTable, data.Table.Model.MetaCode, state);
            if (rowid < 1)
                throw new InvalidOperationException("Could not get a new row id for table " + data.Table.DbName);


            var json = new StringBuilder();
            json.Append("{");
            json.Append(DBHelpers.GetJSONValue("Id", rowid));
            json.Append("," + DBHelpers.GetJSONValue("Version", state.Version));
            json.Append("," + DBHelpers.GetJSONValue("ApplicationId", this.Model.Application.Id));
            json.Append("," + DBHelpers.GetJSONValue("CreatedBy", state.UserId));
            json.Append("," + DBHelpers.GetJSONValue("ChangedBy", state.UserId));
            json.Append("," + DBHelpers.GetJSONValue("OwnedBy", state.OwnerUserId));
            json.Append("," + DBHelpers.GetJSONValue("ChangedDate", DateTime.Now));
            json.Append("," + DBHelpers.GetJSONValue("ParentId", state.Id));

     

            foreach (var t in data.Values)
            {
                if (!t.HasModel)
                    continue;

                json.Append("," + DBHelpers.GetJSONValue(t.Value, t.Model));

            }
            json.Append("}");

            NoSqlClient.InsertIntwentyJsonObject(json.ToString(), data.Table.DbName, rowid);

            if (this.Model.Application.UseVersioning)
            {
                NoSqlClient.InsertIntwentyJsonObject(json.ToString(), data.Table.DbName + "_Revision", rowid, state.Version);
            }

        }

        private void UpdateTableRow(ApplicationTableRow data, ClientStateInfo state)
        {
            
            var json = new StringBuilder();
            json.Append("{");
            json.Append(DBHelpers.GetJSONValue("Id", data.Id));
            json.Append("," + DBHelpers.GetJSONValue("Version", state.Version));
            json.Append("," + DBHelpers.GetJSONValue("ApplicationId", this.Model.Application.Id));
            json.Append("," + DBHelpers.GetJSONValue("CreatedBy", state.UserId));
            json.Append("," + DBHelpers.GetJSONValue("ChangedBy", state.UserId));
            json.Append("," + DBHelpers.GetJSONValue("OwnedBy", state.OwnerUserId));
            json.Append("," + DBHelpers.GetJSONValue("ChangedDate", DateTime.Now));
            json.Append("," + DBHelpers.GetJSONValue("ParentId", state.Id));


            foreach (var t in data.Values)
            {
               
                if (!t.HasModel)
                    continue;

                json.Append("," + DBHelpers.GetJSONValue(t.Value, t.Model));

            }
            json.Append("}");

            NoSqlClient.UpdateIntwentyJsonObject(json.ToString(), data.Table.DbName, data.Id);

            if (this.Model.Application.UseVersioning)
            {
                NoSqlClient.InsertIntwentyJsonObject(json.ToString(), data.Table.DbName + "_Revision", data.Id, state.Version);
            }

        }

        protected virtual void BeforeSave()
        {

        }

        protected virtual void BeforeSaveNew()
        {

        }

        protected virtual void BeforeSaveUpdate()
        {

        }

        protected virtual void AfterSave()
        {

        }

      
    }
}
