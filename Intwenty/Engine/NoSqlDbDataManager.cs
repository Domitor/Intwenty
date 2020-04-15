using Intwenty.Data.DBAccess;
using Intwenty.Data.DBAccess.Helpers;
using Intwenty.Data.Dto;
using Intwenty.Data.Entity;
using Intwenty.Model;
using Shared;
using System;
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

        protected SystemSettings Settings { get; set; }

        public ClientStateInfo ClientState { get; set; }

        protected LifecycleStatus Status { get; set; }

        protected bool CanRollbackVersion { get; set; }

        protected DateTime ApplicationSaveTimeStamp { get; set; }


        protected NoSqlDbDataManager(ApplicationModel model, IIntwentyModelService modelservice, SystemSettings settings, IntwentyNoSqlDbClient nosqlclient)
        {
            Settings = settings;
            Model = model;
            NoSqlClient = nosqlclient;
            ModelRepository = modelservice;
            ApplicationSaveTimeStamp = DateTime.Now;
        }

        public static NoSqlDbDataManager GetDataManager(ApplicationModel model, IIntwentyModelService modelservice, SystemSettings settings, IntwentyNoSqlDbClient nosqlclient)
        {


            if (model.Application.MetaCode == "XXXXX")
            {
                return null;
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

        public OperationResult GenerateTestData(int gencount)
        {
            throw new NotImplementedException();
        }

        public OperationResult GetDataView(List<DataViewModelItem> viewinfo, ListRetrivalArgs args)
        {
            var result = new OperationResult();

            try
            {
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

                var separator = "";
                var returnfields = "{";
                var columns = new List<IIntwentyDataColum>();
                foreach (var viewcol in viewinfo)
                {
                    if ((viewcol.IsMetaTypeDataViewColumn || viewcol.IsMetaTypeDataViewKeyColumn) && viewcol.ParentMetaCode == dv.MetaCode)
                    {
                        returnfields += separator + DBHelpers.GetJSONValue(viewcol.SQLQueryFieldName, 1);
                        separator = ", ";
                    }
                }
                returnfields += "}";

                var filterfields = "{}";
                if (!string.IsNullOrEmpty(args.FilterField) && !string.IsNullOrEmpty(args.FilterValue))
                {
                    filterfields = string.Format("\"{0}\": /{1}/", args.FilterField, args.FilterValue);
                    filterfields = "{" + filterfields + "}";
                }

                result.AddMessage("RESULT", string.Format("Fetched dataview {0}", dv.Title));

                var jsonresult = NoSqlClient.GetAsJSONArray(tablename, filterfields, returnfields, result.RetriveListArgs.CurrentRowNum, (result.RetriveListArgs.CurrentRowNum + result.RetriveListArgs.BatchSize));

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

        public OperationResult GetDataViewValue(List<DataViewModelItem> viewinfo, ListRetrivalArgs args)
        {

            var result = new OperationResult(true, "Fetched dataview value", 0, 0);


            try
            {
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

                string filterfields = string.Empty;
                foreach (var v in viewinfo)
                {
                    if (v.IsMetaTypeDataView && v.MetaCode == args.DataViewMetaCode)
                    {
                        var keyfield = viewinfo.Find(p => p.IsMetaTypeDataViewKeyColumn && p.ParentMetaCode == v.MetaCode);
                        if (keyfield == null)
                            continue;

                        filterfields = string.Format("\"{0}\":\"{1}\"", args.FilterField, args.FilterValue);
                        filterfields = "{" + filterfields + "}";
                        break;
                    }
                }

                if (string.IsNullOrEmpty(filterfields))
                    throw new InvalidOperationException("Could not set filter in GetDataViewValue(viewinfo, args).");

                var separator = "";
                var returnfields = "{";
                var columns = new List<IIntwentyDataColum>();
                foreach (var viewcol in viewinfo)
                {
                    if ((viewcol.IsMetaTypeDataViewColumn || viewcol.IsMetaTypeDataViewKeyColumn) && viewcol.ParentMetaCode == args.DataViewMetaCode)
                    {
                        returnfields += separator + DBHelpers.GetJSONValue(viewcol.SQLQueryFieldName, 1);
                        separator = ", ";
                    }
                }
                returnfields += "}";

                var jsonresult = NoSqlClient.GetAsJSONObject(tablename, filterfields, returnfields);
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

        public OperationResult GetLatestIdByOwnerUser(ClientStateInfo data)
        {
            throw new NotImplementedException();
        }

        public OperationResult GetLatestVersion(ClientStateInfo data)
        {
            ClientState = data;
            var jsonresult = new StringBuilder();
            var result = new OperationResult(true, string.Format("Fetched latest version for application {0}", this.Model.Application.Title), ClientState.Id, ClientState.Version);
            
            try
            {
                jsonresult.Append("{");

                //MAIN TABLE
                var appjson = NoSqlClient.GetAsJSONObject(this.Model.Application.DbName, this.ClientState.Id, this.ClientState.Version);
                jsonresult.Append("\"" + this.Model.Application.DbName + "\":" + appjson.ToString());
               

                //SUBTABLES
                foreach (var t in this.Model.DataStructure)
                {
                    if (t.IsMetaTypeDataTable && t.IsRoot)
                    {
                        var filter = "{" + string.Format("\"ParentId\":{0}", this.ClientState) + "}";
                        var tablejson = NoSqlClient.GetAsJSONArray(t.DbName, filter, string.Empty);
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
                args.MaxCount = NoSqlClient.GetCollectionCount(this.Model.Application.DbName);

            result.RetriveListArgs = new ListRetrivalArgs();
            result.RetriveListArgs = args;

            try
            {
                var json = NoSqlClient.GetAsJSONArray(this.Model.Application.DbName, result.RetriveListArgs.CurrentRowNum, (result.RetriveListArgs.CurrentRowNum + result.RetriveListArgs.BatchSize));
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

        public OperationResult GetValueDomains()
        {
           
            var sb = new StringBuilder();
            var result = new OperationResult(true, string.Format("Fetched doamins used in ui for application {0}", this.Model.Application.Title), 0, 0);

            try
            {
                var valuedomains = new Dictionary<string,string>();


                //COLLECT DOMAINS AND VIEWS USED BY UI
                foreach (var t in this.Model.UIStructure)
                {
                    if (t.HasValueDomain)
                    {
                        var domainparts = t.Domain.Split(".".ToCharArray()).ToList();
                        if (domainparts.Count >= 2)
                        {
                            if (!valuedomains.ContainsKey(domainparts[1]))
                                valuedomains.Add(domainparts[1], "");
                        }
                    }
                }

                sb.Append("{");
                var domainindex = 0;
                foreach (var d in valuedomains)
                {
                    var filter = "{" + string.Format("\"DomainName\":\"{0}\"", d.Key) + "}";
                    var jsonresult = NoSqlClient.GetAsJSONArray("sysmodel_ValueDomainItem", filter, "");

                    if (domainindex == 0)
                        sb.Append("\"" + "VALUEDOMAIN_" + d.Key + "\":");
                    else
                        sb.Append(",\"" + "VALUEDOMAIN_" + d.Key + "\":");

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

        public OperationResult GetVersion()
        {
            throw new NotImplementedException();
        }

        public OperationResult Save(ClientStateInfo data)
        {
            ClientState = data;
            if (ClientState == null)
                return new OperationResult(false, "No client state found when performing save application.", 0, 0);

            var result = new OperationResult(true, string.Format("Saved application {0}", this.Model.Application.Title), ClientState.Id, ClientState.Version);

            try
            {
                //CONNECT MODEL TO DATA
                data.InferModel(Model);

                BeforeSave();

                if (ClientState.Id < 1)
                {
                    this.ClientState.Id = GetNewSystemID("APPLICATION", this.Model.Application.MetaCode);
                    this.Status = LifecycleStatus.NEW_NOT_SAVED;
                    //this.ClientState.Version = CreateVersionRecord();
                    BeforeSaveNew();
                    var informationstatus = InsertInformationStatus();
                    InsertMainTable(informationstatus);
                    HandleSubTables(informationstatus);
                    this.Status = LifecycleStatus.NEW_SAVED;
                }
                else if (ClientState.Id > 0 && this.Model.Application.UseVersioning)
                {
                    this.Status = LifecycleStatus.EXISTING_NOT_SAVED;
                    //this.ClientState.Version = CreateVersionRecord();
                    BeforeSaveUpdate();
                    //InsertMainTable(data);
                    //UpdateInformationStatus();
                    //HandleSubTables(data);
                    this.Status = LifecycleStatus.EXISTING_SAVED;
                }
                else if (ClientState.Id > 0 && !this.Model.Application.UseVersioning)
                {
                    this.Status = LifecycleStatus.EXISTING_NOT_SAVED;
                    BeforeSaveUpdate();
                    var informationstatus = UpdateInformationStatus();
                    UpdateMainTable(informationstatus);
                    
                    //HandleSubTables(data);
                    this.Status = LifecycleStatus.EXISTING_SAVED;
                }


                result.Id = ClientState.Id;
                result.Version = ClientState.Version;

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

        private int GetNewSystemID(string metatype, string metacode)
        {
            var model = new SystemID() { ApplicationId = this.Model.Application.Id, GeneratedDate = DateTime.Now, MetaCode = metacode, MetaType = metatype, Properties = this.ClientState.Properties };
            NoSqlClient.GetNewSystemId(model);
            return model.Id;
        }

        private InformationStatus InsertInformationStatus()
        {
            var model = new InformationStatus()
            {
                Id = this.ClientState.Id,
                ApplicationId = this.Model.Application.Id,
                ChangedBy = this.ClientState.UserId,
                ChangedDate = DateTime.Now,
                CreatedBy = this.ClientState.UserId,
                MetaCode = this.Model.Application.MetaCode,
                OwnedBy = this.ClientState.OwnerUserId,
                OwnerId = this.ClientState.OwnerId,
                PerformDate = DateTime.Now,
                Version = this.ClientState.Version
            };

            NoSqlClient.Insert(model);

            return model;

        }

        private InformationStatus UpdateInformationStatus()
        {
            var model = new InformationStatus()
            {
                Id = this.ClientState.Id,
                ApplicationId = this.Model.Application.Id,
                ChangedBy = this.ClientState.UserId,
                ChangedDate = DateTime.Now,
                CreatedBy = this.ClientState.UserId,
                MetaCode = this.Model.Application.MetaCode,
                OwnedBy = this.ClientState.OwnerUserId,
                OwnerId = this.ClientState.OwnerId,
                PerformDate = DateTime.Now,
                Version = this.ClientState.Version
            };

            NoSqlClient.Update(model);

            return model;

        }

        private void InsertMainTable(InformationStatus informationstatus)
        {
            if (this.ClientState.Id < 1)
                throw new InvalidOperationException("Invalid systemid");

            var json = new StringBuilder();
            json.Append("{");
            json.Append(DBHelpers.GetJSONValue("RowChangeDate", DateTime.Now));
            json.Append("," + DBHelpers.GetJSONValue("UserID", this.ClientState.UserId));
            json.Append("," + DBHelpers.GetJSONValue("Version", 1));
            json.Append("," + DBHelpers.GetJSONValue("OwnerId", this.ClientState.OwnerId));
            json.Append("," + DBHelpers.GetJSONValue("ApplicationId", informationstatus.ApplicationId));
            json.Append("," + DBHelpers.GetJSONValue("CreatedBy", informationstatus.CreatedBy));
            json.Append("," + DBHelpers.GetJSONValue("ChangedBy", informationstatus.ChangedBy));
            json.Append("," + DBHelpers.GetJSONValue("OwnedBy", informationstatus.OwnedBy));


            foreach (var t in this.ClientState.Values)
            {
                if (!t.HasModel)
                    continue;

                json.Append("," + DBHelpers.GetJSONValue(t.Value, t.Model));

            }
            json.Append("}");

            NoSqlClient.InsertJsonDocument(json.ToString(), this.Model.Application.DbName, this.ClientState.Id, this.ClientState.Version);
        }

        private void UpdateMainTable(InformationStatus informationstatus)
        {
            if (this.ClientState.Id < 1)
                throw new InvalidOperationException("Invalid systemid");

            var json = new StringBuilder();
            json.Append("{");
            json.Append(DBHelpers.GetJSONValue("_id", this.ClientState.Id ));
            json.Append("," + DBHelpers.GetJSONValue("RowChangeDate", DateTime.Now));
            json.Append("," + DBHelpers.GetJSONValue("UserID", this.ClientState.UserId));
            json.Append("," + DBHelpers.GetJSONValue("Version", 1));
            json.Append("," + DBHelpers.GetJSONValue("OwnerId", this.ClientState.OwnerId));
            json.Append("," + DBHelpers.GetJSONValue("ApplicationId", informationstatus.ApplicationId));
            json.Append("," + DBHelpers.GetJSONValue("CreatedBy", informationstatus.CreatedBy));
            json.Append("," + DBHelpers.GetJSONValue("ChangedBy", informationstatus.ChangedBy));
            json.Append("," + DBHelpers.GetJSONValue("OwnedBy", informationstatus.OwnedBy));


            foreach (var t in this.ClientState.Values)
            {
                if (!t.HasModel)
                    continue;

                json.Append("," + DBHelpers.GetJSONValue(t.Value, t.Model));

            }
            json.Append("}");

            NoSqlClient.UpdateJsonDocument(json.ToString(), this.Model.Application.DbName, this.ClientState.Id, this.ClientState.Version);
        }

        private void HandleSubTables(InformationStatus informationstatus)
        {
            foreach (var table in this.ClientState.SubTables)
            {
                if (!table.HasModel)
                    continue;

                foreach (var row in table.Rows)
                {
                    if (row.Id < 1 || this.Model.Application.UseVersioning)
                    {
                        InsertTableRow(row);

                    }
                    else
                    {
                        UpdateTableRow(row);

                    }

                }

            }

        }

        private void InsertTableRow(ApplicationTableRow data)
        {
            var rowid = GetNewSystemID(DatabaseModelItem.MetaTypeDataTable, data.Table.Model.MetaCode);
            if (rowid < 1)
                throw new InvalidOperationException("Could not get a new row id for table " + data.Table.DbName);


            var json = new StringBuilder();
            json.Append("{");
            json.Append(DBHelpers.GetJSONValue("ParentId", this.ClientState.Id));
            json.Append("," + DBHelpers.GetJSONValue("RowChangeDate", DateTime.Now));
            json.Append("," + DBHelpers.GetJSONValue("UserId", this.ClientState.UserId));
            json.Append("," + DBHelpers.GetJSONValue("Version", 1));
            json.Append("," + DBHelpers.GetJSONValue("OwnerId", this.ClientState.OwnerId));

            foreach (var t in data.Values)
            {
                if (!t.HasModel)
                    continue;

                json.Append("," + DBHelpers.GetJSONValue(t.Value, t.Model));

            }
            json.Append("}");

            NoSqlClient.InsertJsonDocument(json.ToString(), this.Model.Application.DbName, rowid, this.ClientState.Version);

        }

        private void UpdateTableRow(ApplicationTableRow data)
        {
            var rowid = 0;
            var json = new StringBuilder();
            json.Append("{");
            json.Append(DBHelpers.GetJSONValue("ParentId", this.ClientState.Id));
            json.Append("," + DBHelpers.GetJSONValue("RowChangeDate", this.ClientState.Id));
            json.Append("," + DBHelpers.GetJSONValue("UserId", this.ClientState.UserId));
            json.Append("," + DBHelpers.GetJSONValue("Version", 1));
            json.Append("," + DBHelpers.GetJSONValue("OwnerId", this.ClientState.OwnerId));

            foreach (var t in data.Values)
            {
                if (t.DbName.ToLower() == "id")
                {
                    rowid = (int)t.Value;
                    json.Append(DBHelpers.GetJSONValue("_id", rowid));
                  
                }
                if (!t.HasModel)
                    continue;

                json.Append("," + DBHelpers.GetJSONValue(t.Value, t.Model));

            }
            json.Append("}");

            NoSqlClient.UpdateJsonDocument(json.ToString(), data.Table.DbName, rowid, this.ClientState.Version);
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
