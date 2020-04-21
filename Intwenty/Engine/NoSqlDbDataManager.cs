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

        public ClientStateInfo ClientState { get; set; }

        protected LifecycleStatus Status { get; set; }

        protected bool CanRollbackVersion { get; set; }

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
            var rnd = new Random(9999999);

            foreach (var t in this.Model.DataStructure)
            {

                if (t.IsMetaTypeDataColumn && t.IsRoot)
                {
                    if (t.HasPropertyWithValue("DEFVAL", "AUTO"))
                    {
                        //continue;
                    }

                    var lookup = this.Model.UIStructure.Find(p => p.IsMetaTypeLookUp && p.IsDataViewConnected && p.IsDataViewColumnConnected && p.IsDataColumnConnected && p.DataMetaCode == t.MetaCode);
                    if (lookup != null)
                    {
                        //continue;

                        var view = NoSqlClient.GetJSONArray(lookup.DataViewInfo.QueryTableDbName);
                        var doc = System.Text.Json.JsonDocument.Parse(view.ToString());
                        var array = doc.RootElement.EnumerateArray();
                        if (array.Count() > 0)
                        {
                            var val = array.Last();
                            this.ClientState.Values.Add(new ApplicationValue() { DbName = lookup.DataColumnInfo.DbName, Value = val.GetProperty(lookup.DataViewColumnInfo.SQLQueryFieldName).GetString() });
                            if (lookup.IsDataViewColumn2Connected && lookup.IsDataColumn2Connected)
                            {
                                this.ClientState.Values.Add(new ApplicationValue() { DbName = lookup.DataColumnInfo2.DbName, Value = val.GetProperty(lookup.DataViewColumnInfo2.SQLQueryFieldName).GetString() });
                            }

                        }


                    }
                    else
                    {

                        var value = TestDataHelper.GetSemanticValue(this.Model, t, rnd, gencount);
                        if (value != null)
                            this.ClientState.Values.Add(new ApplicationValue() { DbName = t.DbName, Value = value });

                    }
                }

            }

            return this.Save(this.ClientState);
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


                var columns = new List<IIntwentyDataColum>();
                foreach (var viewcol in viewinfo)
                {
                    if ((viewcol.IsMetaTypeDataViewColumn || viewcol.IsMetaTypeDataViewKeyColumn) && viewcol.ParentMetaCode == dv.MetaCode)
                    {
                        columns.Add(new IntwentyDataColum() { ColumnName = viewcol.SQLQueryFieldName });
                    }
                }


                var expression = "";
                if (!string.IsNullOrEmpty(args.FilterField) && !string.IsNullOrEmpty(args.FilterValue))
                {
                    expression = "(";
                    expression += string.Format("'[{0}]' LIKE '%{1}%'", args.FilterField, args.FilterValue);
                    expression += ")";
                }

                result.AddMessage("RESULT", string.Format("Fetched dataview {0}", dv.Title));

                var jsonresult = NoSqlClient.GetJSONArray(tablename, expression, columns, result.RetriveListArgs.CurrentRowNum, (result.RetriveListArgs.CurrentRowNum + result.RetriveListArgs.BatchSize));

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

                string expression = "()";
                var keyfield = viewinfo.Find(p => p.IsMetaTypeDataViewKeyColumn && p.ParentMetaCode == args.DataViewMetaCode);
                if (keyfield != null)
                {
                    expression = "(";
                    expression += string.Format("'[{0}]' = '{1}'", keyfield.SQLQueryFieldName, args.FilterValue);
                    expression += ")";
                }
                  

                var columns = new List<IIntwentyDataColum>();
                foreach (var viewcol in viewinfo)
                {
                    if ((viewcol.IsMetaTypeDataViewColumn || viewcol.IsMetaTypeDataViewKeyColumn) && viewcol.ParentMetaCode == args.DataViewMetaCode)
                    {
                        columns.Add(new IntwentyDataColum() { ColumnName = viewcol.SQLQueryFieldName });
                    }
                }


                var jsonresult = NoSqlClient.GetJSONObject(tablename, expression, columns);
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
                var appjson = NoSqlClient.GetJSONObject(this.Model.Application.DbName, this.ClientState.Id, this.ClientState.Version);
                jsonresult.Append("\"" + this.Model.Application.DbName + "\":" + appjson.ToString());
               

                //SUBTABLES
                foreach (var t in this.Model.DataStructure)
                {
                    if (t.IsMetaTypeDataTable && t.IsRoot)
                    {
                        var expression = "(";
                        expression += string.Format("'[{0}]' = '{1}'", "ParentId", this.ClientState.Id);
                        expression += ")";
                        var tablejson = NoSqlClient.GetJSONArray(t.DbName, expression, new List<IIntwentyDataColum>());
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
                args.MaxCount = NoSqlClient.GetDocumentCount(this.Model.Application.DbName);

            result.RetriveListArgs = new ListRetrivalArgs();
            result.RetriveListArgs = args;

            try
            {
                var json = NoSqlClient.GetJSONArray(this.Model.Application.DbName, result.RetriveListArgs.CurrentRowNum, (result.RetriveListArgs.CurrentRowNum + result.RetriveListArgs.BatchSize));
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
                    var expression = "(";
                    expression += string.Format("'[{0}]' = '{1}'", "DomainName", d.Key);
                    expression += ")";
                    var jsonresult = NoSqlClient.GetJSONArray("sysmodel_ValueDomainItem", expression, new List<IIntwentyDataColum>());

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
                    this.ClientState.Version = CreateVersionRecord();
                    BeforeSaveNew();
                    var informationstatus = InsertInformationStatus();
                    InsertMainTable(informationstatus);
                    HandleSubTables();
                    this.Status = LifecycleStatus.NEW_SAVED;
                }
                else if (ClientState.Id > 0 && this.Model.Application.UseVersioning)
                {
                    this.Status = LifecycleStatus.EXISTING_NOT_SAVED;
                    this.ClientState.Version = CreateVersionRecord();
                    BeforeSaveUpdate();
                    var informationstatus = UpdateInformationStatus();
                    InsertMainTable(informationstatus);
                    HandleSubTables();
                    this.Status = LifecycleStatus.EXISTING_SAVED;
                }
                else if (ClientState.Id > 0 && !this.Model.Application.UseVersioning)
                {
                    this.Status = LifecycleStatus.EXISTING_NOT_SAVED;
                    BeforeSaveUpdate();
                    var informationstatus = UpdateInformationStatus();
                    UpdateMainTable(informationstatus);
                    HandleSubTables();
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

        private int CreateVersionRecord()
        {

            var filter = "(";
            filter += string.Format("'[{0}]'='{1}'", "MetaCode", this.Model.Application.MetaCode);
            filter += string.Format(" AND '[{0}]'='{1}'", "MetaType", "APPLICATION");
            filter += string.Format(" AND '[{0}]'='{1}'", "Id", this.ClientState.Id);
            filter += ")";

            var newversion = NoSqlClient.GetMaxIntValue(this.Model.Application.VersioningTableName, filter, "Version");
            if (newversion < 1)
                newversion = 1;
            else
                newversion += 1;

            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append(string.Format("\"{0}\":{1}", "Id", this.ClientState.Id));
            sb.Append(string.Format(",\"{0}\":{1}", "ParentId", 0));
            sb.Append(string.Format(",\"{0}\":\"{1}\"", "MetaCode", this.Model.Application.MetaCode));
            sb.Append(string.Format(",\"{0}\":\"{1}\"", "MetaType", "APPLICATION"));
            sb.Append(string.Format(",\"{0}\":{1}", "Version", newversion));
            sb.Append(string.Format(",\"{0}\":{1}", "OwnerId", 0));
            sb.Append(string.Format(",\"{0}\":\"{1}\"", "RowChangeDate", DateTime.Now));
            sb.Append(string.Format(",\"{0}\":\"{1}\"", "UserID", this.ClientState.UserId));
            sb.Append("}");

            NoSqlClient.InsertJsonDocument(sb.ToString(), this.Model.Application.VersioningTableName, this.ClientState.Id, newversion);

            if (this.ClientState.Version < newversion)
                CanRollbackVersion = true;

            return newversion;
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
            json.Append(DBHelpers.GetJSONValue("Id", this.ClientState.Id ));
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

        private void HandleSubTables()
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
            json.Append(DBHelpers.GetJSONValue("Id", rowid));
            json.Append("," + DBHelpers.GetJSONValue("ParentId", this.ClientState.Id));
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

            NoSqlClient.InsertJsonDocument(json.ToString(), data.Table.DbName, rowid, this.ClientState.Version);

        }

        private void UpdateTableRow(ApplicationTableRow data)
        {
            
            var json = new StringBuilder();
            json.Append("{");
            json.Append(DBHelpers.GetJSONValue("Id", data.Id));
            json.Append("," + DBHelpers.GetJSONValue("ParentId", this.ClientState.Id));
            json.Append("," + DBHelpers.GetJSONValue("RowChangeDate", this.ClientState.Id));
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

            NoSqlClient.UpdateJsonDocument(json.ToString(), data.Table.DbName, data.Id, this.ClientState.Version);
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
