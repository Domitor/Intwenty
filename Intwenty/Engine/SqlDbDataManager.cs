using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Intwenty.Data.DBAccess;
using Intwenty.Data.DBAccess.Helpers;
using Intwenty.Data.Dto;
using Intwenty.Data.Entity;
using Intwenty.Engine.Custom;
using Intwenty.Model;




namespace Intwenty.Engine
{
   

   

    public class SqlDbDataManager : IDataManager
    {
        protected IIntwentyModelService ModelRepository { get; set; }

        protected IntwentySqlDbClient SqlClient { get; set; }

        protected ApplicationModel Model { get; set; }

        protected IntwentySettings Settings { get; set; }

        protected DateTime ApplicationSaveTimeStamp { get; set; }

        protected List<SqlDataTypeMap> DataTypes { get; set; }

        protected List<DBMSCommandMap> Commands { get; set; }

       


        protected SqlDbDataManager(ApplicationModel model, IIntwentyModelService modelservice, IntwentySettings settings, IntwentySqlDbClient sqlclient)
        {
            Settings = settings;
            Model = model;
            ModelRepository = modelservice;
            SqlClient = sqlclient;
            ApplicationSaveTimeStamp = DateTime.Now;
            DataTypes = DBHelpers.GetDataTypeMap();
            Commands = DBHelpers.GetDBMSCommandMap();
        }

        public static SqlDbDataManager GetDataManager(ApplicationModel model, IIntwentyModelService modelservice, IntwentySettings settings, IntwentySqlDbClient sqlclient)
        {
            

            if (model != null && model.Application.MetaCode == "XXXXX")
            {
                return new  CustomSqlDbDataManagerExample(model, modelservice, settings, sqlclient);
            }
            else
            {
                var t = new SqlDbDataManager(model,modelservice, settings, sqlclient);
                return t;
            }
        }


     




        #region Implementation

        public OperationResult ConfigureDatabase()
        {
            var res = new OperationResult(true, string.Format("Database configured for application {0}", this.Model.Application.Title));

            try
            {
                CreateMainTable(res);
                CreateApplicationVersioningTable(res);

                foreach (var t in Model.DataStructure)
                {
                    if (t.IsMetaTypeDataColumn && t.IsRoot)
                    {
                        CreateDBColumn(res, t, Model.Application.DbName);
                    }

                    if (t.IsMetaTypeDataTable)
                    {
                        CreateDBTable(res, t);
                    }
                }

                CreateIndexes(res);

            }
            catch (Exception ex)
            {
                res.SetError(ex.Message, "");
            }

            return res;
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

            var t = FetchLatestIdByOwnerUser(state);
            if (t.Id < 1)
                return new OperationResult(false, "Requested data could not be found.");

            state.Id = t.Id;
            state.Version = t.Version;

            return GetLatestVersion(state);
        }


      
        private OperationResult GetLatestVersion(ClientStateInfo state)
        {
            var jsonresult = new StringBuilder();
            var result = new OperationResult(true, string.Format("Fetched latest version for application {0}", this.Model.Application.Title), state.Id, state.Version);

            try
            {
                var columns = new List<IIntwentyDataColum>();
                columns.Add(new IntwentyDataColumn() { ColumnName = "Id", DataType = DatabaseModelItem.DataTypeInt });
                columns.Add(new IntwentyDataColumn() { ColumnName = "Version", DataType = DatabaseModelItem.DataTypeInt });
                columns.Add(new IntwentyDataColumn() { ColumnName = "ApplicationId", DataType = DatabaseModelItem.DataTypeInt });
                columns.Add(new IntwentyDataColumn() { ColumnName = "ChangedDate", DataType = DatabaseModelItem.DataTypeDateTime });
                columns.Add(new IntwentyDataColumn() { ColumnName = "CreatedBy", DataType = DatabaseModelItem.DataTypeString });
                columns.Add(new IntwentyDataColumn() { ColumnName = "ChangedBy", DataType = DatabaseModelItem.DataTypeString });
                columns.Add(new IntwentyDataColumn() { ColumnName = "OwnedBy", DataType = DatabaseModelItem.DataTypeString });


                var sql_stmt = new StringBuilder();
                sql_stmt.Append("SELECT t1.* ");
                foreach (var col in this.Model.DataStructure)
                {
                    if (col.IsMetaTypeDataColumn && col.IsRoot)
                    {
                        sql_stmt.Append(", t2." + col.DbName + " ");
                        columns.Add(col);

                    }
                }
                sql_stmt.Append("FROM sysdata_InformationStatus t1 ");
                sql_stmt.Append("JOIN " + this.Model.Application.DbName + " t2 on t1.Id=t2.Id and t1.Version = t2.Version ");
                sql_stmt.Append("WHERE t1.ApplicationId = " + this.Model.Application.Id + " ");
                sql_stmt.Append("AND t1.Id = " + state.Id);
               

                jsonresult.Append("{");

                var ds = new DataSet();
                SqlClient.Open();
                SqlClient.CreateCommand(sql_stmt.ToString());
                var appjson = SqlClient.GetJsonObject(columns);
                SqlClient.Close();
               
                if (appjson.Length < 5)
                {
                    jsonresult.Append("}");
                    result.Data = jsonresult.ToString();
                    return result;
                }

                jsonresult.Append("\"" + this.Model.Application.DbName + "\":" + appjson.ToString());

                //SUBTABLES
                foreach (var t in this.Model.DataStructure)
                {
                    if (t.IsMetaTypeDataTable && t.IsRoot)
                    {
                        columns = new List<IIntwentyDataColum>();
                        columns.Add(new IntwentyDataColumn() { ColumnName = "Id", DataType = DatabaseModelItem.DataTypeInt });
                        columns.Add(new IntwentyDataColumn() { ColumnName = "Version", DataType = DatabaseModelItem.DataTypeInt });
                        columns.Add(new IntwentyDataColumn() { ColumnName = "ApplicationId", DataType = DatabaseModelItem.DataTypeInt });
                        columns.Add(new IntwentyDataColumn() { ColumnName = "ChangedDate", DataType = DatabaseModelItem.DataTypeDateTime });
                        columns.Add(new IntwentyDataColumn() { ColumnName = "CreatedBy", DataType = DatabaseModelItem.DataTypeString });
                        columns.Add(new IntwentyDataColumn() { ColumnName = "ChangedBy", DataType = DatabaseModelItem.DataTypeString });
                        columns.Add(new IntwentyDataColumn() { ColumnName = "OwnedBy", DataType = DatabaseModelItem.DataTypeString });
                        columns.Add(new IntwentyDataColumn() { ColumnName = "ParentId", DataType = DatabaseModelItem.DataTypeInt });

                        sql_stmt = new StringBuilder();
                        sql_stmt.Append("SELECT t1.ApplicationId ");
                        sql_stmt.Append(", t2.Id ");
                        sql_stmt.Append(", t2.Version ");
                        sql_stmt.Append(", t2.ChangedDate ");
                        sql_stmt.Append(", t2.CreatedBy ");
                        sql_stmt.Append(", t2.ChangedBy ");
                        sql_stmt.Append(", t2.OwnedBy ");
                        sql_stmt.Append(", t2.ParentId ");
                     
                        foreach (var col in this.Model.DataStructure)
                        {
                            if (col.IsMetaTypeDataColumn && col.ParentMetaCode == t.MetaCode)
                            {
                                sql_stmt.Append(", t2." + col.DbName + " ");
                                columns.Add(col);
                            }
                        }
                        sql_stmt.Append("FROM sysdata_InformationStatus t1 ");
                        sql_stmt.Append("JOIN " + t.DbName + " t2 on t1.Id=t2.ParentId and t1.Version = t2.Version ");
                        sql_stmt.Append("WHERE t1.ApplicationId = " + this.Model.Application.Id + " ");
                        sql_stmt.Append("AND t1.Id = " + state.Id);
                        SqlClient.Open();
                        SqlClient.CreateCommand(sql_stmt.ToString());
                        var tablearray = SqlClient.GetJsonArray(columns);
                        SqlClient.Close();

                        jsonresult.Append(", \""+t.DbName+"\": " + tablearray.ToString());

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

        public virtual OperationResult GetVersion()
        {
            throw new NotImplementedException();
        }

        public virtual OperationResult Save(ClientStateInfo state)
        {

            if (state == null)
                return new OperationResult(false, "No client state found when performing save application.", 0, 0);

            var result = new OperationResult(true, string.Format("Saved application {0}", this.Model.Application.Title), state.Id, state.Version);

            try
            {
                //CONNECT MODEL TO DATA
                state.InferModel(Model);

                SqlClient.Open();

                BeforeSave(state);

                if (state.Id < 1)
                {
                    state.Id = GetNewSystemID("APPLICATION", this.Model.Application.MetaCode, state);
                    result.Status = LifecycleStatus.NEW_NOT_SAVED;
                    state.Version = CreateVersionRecord(state);
                    BeforeSaveNew(state);
                    InsertMainTable(state);
                    InsertInformationStatus(state);
                    HandleSubTables(state);
                    result.Status = LifecycleStatus.NEW_SAVED;
                }
                else if (state.Id > 0 && this.Model.Application.UseVersioning)
                {
                    result.Status = LifecycleStatus.EXISTING_NOT_SAVED;
                    state.Version = CreateVersionRecord(state);
                    BeforeSaveUpdate(state);
                    InsertMainTable(state);
                    UpdateInformationStatus(state);
                    HandleSubTables(state);
                    result.Status = LifecycleStatus.EXISTING_SAVED;
                }
                else if (state.Id > 0 && !this.Model.Application.UseVersioning)
                {
                    result.Status = LifecycleStatus.EXISTING_NOT_SAVED;
                    BeforeSaveUpdate(state);
                    UpdateMainTable(state);
                    UpdateInformationStatus(state);
                    HandleSubTables(state);
                    result.Status = LifecycleStatus.EXISTING_SAVED;
                }

                SqlClient.Close();

                result.Id = state.Id;
                result.Version = state.Version;

                AfterSave(state);

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

        public virtual OperationResult GetListByOwnerUser(ListRetrivalArgs args)
        {
            return GetListInternal(args, true, true, true);
        }

        public virtual OperationResult GetList(ListRetrivalArgs args)
        {
            return GetListInternal(args, false, true, true);
        }

        public virtual OperationResult GetListByOwnerUser(string owneruserid)
        {
            return GetListInternal(new ListRetrivalArgs() { ApplicationId = this.Model.Application.Id, OwnerUserId = owneruserid }, true, false, false);
        }

        public virtual OperationResult GetList()
        {
            return GetListInternal(new ListRetrivalArgs() { ApplicationId = this.Model.Application.Id }, false, false, false);
        }


        private OperationResult GetListInternal(ListRetrivalArgs args, bool getbyowneruser, bool selectcolumns, bool usepaging)
        {
            if (args == null)
                return new OperationResult(false, "Can't get list without ListRetrivalArgs",0,0);

            var result = new OperationResult(true, string.Format("Fetched list for application {0}", this.Model.Application.Title),0,0);
          
            if (args.MaxCount == 0 && usepaging)
            {
                SqlClient.Open();
                SqlClient.CreateCommand("select count(*) FROM sysdata_InformationStatus where ApplicationId = " + this.Model.Application.Id);
                var max = SqlClient.ExecuteScalarQuery();
                if (max == DBNull.Value)
                    args.MaxCount = 0;
                else
                    args.MaxCount = Convert.ToInt32(max);

                SqlClient.Close();
            }

            result.RetriveListArgs = new ListRetrivalArgs();
            result.RetriveListArgs = args;


            try
            {
                var columns = new List<IIntwentyDataColum>();
                if (selectcolumns)
                {
                    columns.Add(new IntwentyDataColumn() { ColumnName = "Id", DataType = DatabaseModelItem.DataTypeInt });
                    columns.Add(new IntwentyDataColumn() { ColumnName = "Version", DataType = DatabaseModelItem.DataTypeInt });
                    columns.Add(new IntwentyDataColumn() { ColumnName = "ApplicationId", DataType = DatabaseModelItem.DataTypeInt });
                    columns.Add(new IntwentyDataColumn() { ColumnName = "ChangedDate", DataType = DatabaseModelItem.DataTypeDateTime });
                    columns.Add(new IntwentyDataColumn() { ColumnName = "CreatedBy", DataType = DatabaseModelItem.DataTypeString });
                    columns.Add(new IntwentyDataColumn() { ColumnName = "ChangedBy", DataType = DatabaseModelItem.DataTypeString });
                    columns.Add(new IntwentyDataColumn() { ColumnName = "OwnedBy", DataType = DatabaseModelItem.DataTypeString });
                }

                var sql_list_stmt = new StringBuilder();
                sql_list_stmt.Append("SELECT t1.* ");

                if (this.Model.UIStructure.Exists(p => p.IsMetaTypeListViewField && p.IsDataColumnConnected) && selectcolumns)
                {

                    foreach (var t in this.Model.UIStructure)
                    {
                        if (t.IsMetaTypeListViewField && t.IsDataColumnConnected && t.DataColumnInfo.IsRoot)
                        {
                            sql_list_stmt.Append(", t2." + t.DataColumnInfo.DbName + " ");
                            columns.Add(t.DataColumnInfo);
                        }
                    }

                }
                else
                {
                    foreach (var t in this.Model.DataStructure)
                    {
                        if (t.IsMetaTypeDataColumn && t.IsRoot)
                        {
                            sql_list_stmt.Append(", t2." + t.DbName + " ");
                            if (selectcolumns)
                                columns.Add(t);
                        }
                    }


                }
               
                sql_list_stmt.Append("FROM sysdata_InformationStatus t1 ");
                sql_list_stmt.Append("JOIN " + this.Model.Application.DbName + " t2 on t1.Id=t2.Id and t1.Version = t2.Version ");
                sql_list_stmt.Append("WHERE t1.ApplicationId = @ApplicationId ");
                if (getbyowneruser)
                    sql_list_stmt.Append("AND t1.OwnedBy = @OwnedBy ");

                if (!string.IsNullOrEmpty(args.FilterField) && !string.IsNullOrEmpty(args.FilterValue))
                    sql_list_stmt.Append("AND t2."+ args.FilterField + " LIKE '%"+ args.FilterValue + "%'  ");

                sql_list_stmt.Append("ORDER BY t1.Id");


                string json = "[]";
                SqlClient.Open();
                SqlClient.CreateCommand(sql_list_stmt.ToString());
                SqlClient.AddParameter("@ApplicationId", this.Model.Application.Id);
                if (getbyowneruser)
                    SqlClient.AddParameter("@OwnedBy", args.OwnerUserId);

                if (selectcolumns && usepaging)
                    json = SqlClient.GetJsonArray(columns, result.RetriveListArgs.CurrentRowNum, (result.RetriveListArgs.CurrentRowNum + result.RetriveListArgs.BatchSize)).ToString();
                if (!selectcolumns && usepaging)
                    json = SqlClient.GetJsonArray(result.RetriveListArgs.CurrentRowNum, (result.RetriveListArgs.CurrentRowNum + result.RetriveListArgs.BatchSize)).ToString();
                if (!selectcolumns && !usepaging)
                    json = SqlClient.GetJsonArray().ToString();

                SqlClient.Close();

                result.Data = json;

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

        public OperationResult GetVersionListById(ClientStateInfo state)
        {
            throw new NotImplementedException();
        }

        public OperationResult GetVersion(ClientStateInfo state)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Creates a JSON object with all valuedomains (arrays) used in an application
        /// </summary>
        public virtual OperationResult GetApplicationValueDomains()
        {
           
            var sb = new StringBuilder();
            var result = new OperationResult(true, string.Format("Fetched domains used in ui for application {0}", this.Model.Application.Title), 0, 0);

            try
            {
                var domainindex = 0;
                var rowindex = 0;
                var valuedomains = new List<string>();
                var domaintables = new List<ApplicationTable>();

                //COLLECT DOMAINS AND VIEWS USED BY UI
                foreach (var t in this.Model.UIStructure)
                {
                    if (t.HasValueDomain)
                    {
                        var domainparts = t.Domain.Split(".".ToCharArray()).ToList();
                        if (domainparts.Count >= 2)
                        {
                            if (!valuedomains.Exists(p => p == domainparts[1]))
                                valuedomains.Add(domainparts[1]);
                        }
                    }
                }

                SqlClient.Open();

                foreach (var d in valuedomains)
                {

                    SqlClient.CreateCommand("SELECT Id, DomainName, Code, Value FROM sysmodel_ValueDomainItem WHERE DomainName = @P1");
                    SqlClient.AddParameter("@P1", d);
                    var domaindata = SqlClient.GetDataSet();
                    domaindata.DbName = d;
                    domaintables.Add(domaindata);
                }

                SqlClient.Close();


                sb.Append("{");


                foreach (ApplicationTable table in domaintables)
                {

                    if (domainindex == 0)
                        sb.Append("\"" + "VALUEDOMAIN_" + table.DbName + "\":");
                    else
                        sb.Append(",\"" + "VALUEDOMAIN_" + table.DbName + "\":");

                    domainindex += 1;
                    rowindex = 0;
                   

                    foreach (var row in table.Rows)
                    {
                        if (rowindex == 0)
                            sb.Append("{");
                        else
                            sb.Append(",{");

                        sb.Append(DBHelpers.GetJSONValue("Id", row.GetIntValue("Id")));
                        sb.Append("," + DBHelpers.GetJSONValue("DomainName", row.GetStringValue("DomainName")));
                        sb.Append("," + DBHelpers.GetJSONValue("Code", row.GetStringValue("Code")));
                        sb.Append("," + DBHelpers.GetJSONValue("Value", row.GetStringValue("Value")));

                        sb.Append("}");
                        rowindex += 1;
                    }
                    sb.Append("]");
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

        /// <summary>
        /// Creates a JSON object with all valuedomains
        /// </summary>
        public virtual OperationResult GetValueDomains()
        {
            var sb = new StringBuilder();
            var result = new OperationResult(true, "Fetched all value domins", 0, 0);

            try
            {

                var domainindex = 0;
                var rowindex = 0;
                var domaintables = new List<ApplicationTable>();

                SqlClient.Open();
                SqlClient.CreateCommand("SELECT distinct DomainName FROM sysmodel_ValueDomainItem");
                var domains = SqlClient.GetDataSet();
                foreach (var d in domains.Rows)
                {

                    SqlClient.CreateCommand("SELECT Id, DomainName, Code, Value FROM sysmodel_ValueDomainItem WHERE DomainName = @P1");
                    SqlClient.AddParameter("@P1", d.GetStringValue("DomainName"));
                    var domainitems = SqlClient.GetDataSet();
                    domainitems.DbName = d.GetStringValue("DomainName");
                    domaintables.Add(domainitems);
                }

                SqlClient.Close();

               

                sb.Append("{");


                foreach (ApplicationTable table in domaintables)
                {

                    if (domainindex == 0)
                        sb.Append("\"" + "VALUEDOMAIN_" + table.DbName + "\":");
                    else
                        sb.Append(",\"" + "VALUEDOMAIN_" + table.DbName + "\":");

                    domainindex += 1;
                    rowindex = 0;


                    foreach (var row in table.Rows)
                    {
                        if (rowindex == 0)
                            sb.Append("{");
                        else
                            sb.Append(",{");

                        sb.Append(DBHelpers.GetJSONValue("Id", row.GetIntValue("Id")));
                        sb.Append("," + DBHelpers.GetJSONValue("DomainName", row.GetStringValue("DomainName")));
                        sb.Append("," + DBHelpers.GetJSONValue("Code", row.GetStringValue("Code")));
                        sb.Append("," + DBHelpers.GetJSONValue("Value", row.GetStringValue("Value")));

                        sb.Append("}");
                        rowindex += 1;
                    }
                    sb.Append("]");
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
                if (dv== null)
                    throw new InvalidOperationException("Could not find dataview to fetch");
                if (dv.HasNonSelectSql)
                    throw new InvalidOperationException(string.Format("The sql query defined for dataview {0} has invalid statements.", dv.Title + " ("+dv.MetaCode+")"));


                var columns = new List<IIntwentyDataColum>();
                foreach (var viewcol in viewinfo)
                {
                    if ((viewcol.IsMetaTypeDataViewColumn || viewcol.IsMetaTypeDataViewKeyColumn) && viewcol.ParentMetaCode == dv.MetaCode)
                    {
                        columns.Add(new IntwentyDataColumn() { ColumnName = viewcol.SQLQueryFieldName, DataType = viewcol.DataType});
                    }
                }
              

                result.AddMessage("RESULT", string.Format("Fetched dataview {0}", dv.Title));


                var sql = string.Format(dv.SQLQuery, " ");
                if (!string.IsNullOrEmpty(args.FilterField) && !string.IsNullOrEmpty(args.FilterValue))
                {
                    //Infer where formatter
                    if (!dv.SQLQuery.Contains("{0}"))
                    {
                        var tmp = dv.SQLQuery.ToUpper();
                        var frmind = tmp.IndexOf("FROM");
                        if (frmind > 5)
                        {
                            frmind += 7;
                            var blankind = tmp.IndexOf(" ", frmind);
                            sql = tmp.Insert(blankind, "{0}");
                        }
                    }

                    sql = string.Format(dv.SQLQuery, " WHERE " + args.FilterField + " LIKE '%" + args.FilterValue + "%' ");
                }

                SqlClient.Open();
                SqlClient.CreateCommand(sql);
                StringBuilder json = null;
                json = SqlClient.GetJsonArray(columns, result.RetriveListArgs.CurrentRowNum, (result.RetriveListArgs.CurrentRowNum + result.RetriveListArgs.BatchSize));
                SqlClient.Close();

                result.Data = json.ToString();

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
            var sql = "";
            var result = new OperationResult(true, "Fetched dataview record", 0, 0);
            

            try
            {

                var viewinfo = ModelRepository.GetDataViewModels();

                if (args == null)
                    throw new InvalidOperationException("Call to GetDataViewValue without ListRetrivalArgs");


                result.RetriveListArgs = new ListRetrivalArgs();
                result.RetriveListArgs = args;
            

                foreach (var v in viewinfo)
                {
                    if (v.IsMetaTypeDataView && v.MetaCode == args.DataViewMetaCode)
                    {
                        var keyfield = viewinfo.Find(p => p.IsMetaTypeDataViewKeyColumn && p.ParentMetaCode == v.MetaCode);
                        if (keyfield == null)
                            continue;

                        sql = string.Format(v.SQLQuery, " WHERE " + keyfield.SQLQueryFieldName + " = @P1 ");
                        break;
                    }
                }

                if (string.IsNullOrEmpty(sql))
                    throw new InvalidOperationException("Could not find view and key value in GetDataViewValue(viewinfo, args).");

                var columns = new List<IIntwentyDataColum>();
                foreach (var viewcol in viewinfo)
                {
                    if ((viewcol.IsMetaTypeDataViewColumn || viewcol.IsMetaTypeDataViewKeyColumn) && viewcol.ParentMetaCode == args.DataViewMetaCode)
                    {
                        columns.Add(new IntwentyDataColumn() { ColumnName = viewcol.SQLQueryFieldName, DataType = viewcol.DataType });
                    }
                }

                SqlClient.Open();
                SqlClient.CreateCommand(sql);
                SqlClient.AddParameter("@P1", args.FilterValue);
                var data = SqlClient.GetJsonObject(columns);
                SqlClient.Close();

                result.Data = data.ToString();

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

        public OperationResult GenerateTestData(int gencount, ClientStateInfo state)
        {
            var rnd = new Random(9999999);


            foreach (var t in this.Model.DataStructure)
            {

                if (t.IsMetaTypeDataColumn && t.IsRoot)
                {
                    if (t.HasPropertyWithValue("DEFVAL","AUTO"))
                    {
                        //continue;
                    }

                    var lookup = this.Model.UIStructure.Find(p => p.IsMetaTypeLookUp && p.IsDataViewConnected && p.IsDataViewColumnConnected && p.IsDataColumnConnected && p.DataMetaCode == t.MetaCode);
                    if (lookup != null)
                    {

                        SqlClient.Open();
                        SqlClient.CreateCommand(lookup.DataViewInfo.SQLQuery);
                        var ds = SqlClient.GetDataSet();
                        SqlClient.Close();
                        if (ds.Rows.Count > 0)
                        {
                            var maxindex = ds.Rows.Count - 1;
                            var rowindex = new Random(1).Next(0, maxindex);
                            state.Values.Add(new ApplicationValue() { DbName = t.DbName, Value = ds.Rows[rowindex].GetStringValue(lookup.DataViewColumnInfo.SQLQueryFieldName) });
                            if (lookup.IsDataViewColumn2Connected && lookup.IsDataColumn2Connected)
                            {
                                state.Values.Add(new ApplicationValue() { DbName = lookup.DataColumnInfo2.MetaCode, Value = ds.Rows[rowindex].GetStringValue(lookup.DataViewColumnInfo2.SQLQueryFieldName) });
                            }
                        }

                    }
                    else
                    {

                        var value = TestDataHelper.GetSemanticValue(this.Model,t, rnd, gencount);
                        if (value != null)
                            state.Values.Add(new ApplicationValue() { DbName = t.DbName, Value = value });

                    }
                }

            }

            return this.Save(state);

        }

       

        #endregion

        #region Handlers

        protected virtual void BeforeSave(ClientStateInfo data)
        {

        }

        protected virtual void BeforeSaveNew(ClientStateInfo data)
        {

        }

        protected virtual void BeforeSaveUpdate( ClientStateInfo data)
        {

        }

        protected virtual void AfterSave(ClientStateInfo data)
        {

        }

        #endregion

        #region Helpers

        private void SetParameters(List<ApplicationValue> paramlist)
        {
            foreach (var p in paramlist)
            {
                if (p.Model.IsDataTypeText || p.Model.IsDataTypeString)
                {
                    var val = p.GetAsString();
                    if (!string.IsNullOrEmpty(val))
                        SqlClient.AddParameter("@" + p.DbName, val);
                    else
                        SqlClient.AddParameter("@" + p.DbName, DBNull.Value);
                }
                else if (p.Model.IsDataTypeInt)
                {
                    var val = p.GetAsInt();
                    if (val.HasValue)
                        SqlClient.AddParameter("@" + p.DbName, val.Value);
                    else
                        SqlClient.AddParameter("@" + p.DbName, DBNull.Value);
                }
                else if (p.Model.IsDataTypeBool)
                {
                    var val = p.GetAsBool();
                    if (val.HasValue)
                        SqlClient.AddParameter("@" + p.DbName, val.Value);
                    else
                        SqlClient.AddParameter("@" + p.DbName, DBNull.Value);
                }
                else if (p.Model.IsDataTypeDateTime)
                {
                    var val = p.GetAsDateTime();
                    if (val.HasValue)
                        SqlClient.AddParameter("@" + p.DbName, val.Value);
                    else
                        SqlClient.AddParameter("@" + p.DbName, DBNull.Value);
                }
                else if (p.Model.IsDataType1Decimal || p.Model.IsDataType2Decimal || p.Model.IsDataType3Decimal)
                {
                    var val = p.GetAsDecimal();
                    if (val.HasValue)
                        SqlClient.AddParameter("@" + p.DbName, val.Value);
                    else
                        SqlClient.AddParameter("@" + p.DbName, DBNull.Value);
                }
            }

        }


      

        #endregion

        #region Save

        private int GetNewSystemID(string metatype, string metacode, ClientStateInfo state)
        {

            var model = new SystemID() { ApplicationId=this.Model.Application.Id, GeneratedDate=DateTime.Now, MetaCode =metacode, MetaType = metatype, Properties= state.Properties };
            var result = SqlClient.Insert(model, true);
            return model.Id;
        }

        private void InsertMainTable(ClientStateInfo state)
        {
            var paramlist = new List<ApplicationValue>();

            if (state.Id < 1)
                throw new InvalidOperationException("Invalid systemid");

            var sql_insert = new StringBuilder();
            var sql_insert_value = new StringBuilder();
            sql_insert.Append("INSERT INTO " + this.Model.Application.DbName + " ");
            sql_insert.Append(" (ID, Version, ApplicationId,  CreatedBy, ChangedBy, OwnedBy, ChangedDate");

            sql_insert_value.Append(" VALUES (");
            sql_insert_value.Append(" @Id");
            sql_insert_value.Append(",@Version");
            sql_insert_value.Append(",@ApplicationId");
            sql_insert_value.Append(",@CreatedBy");
            sql_insert_value.Append(",@ChangedBy");
            sql_insert_value.Append(",@OwnedBy");
            sql_insert_value.Append(",@ChangedDate");


            foreach (var t in state.Values)
            {
                if (!t.HasModel)
                    continue;

                sql_insert.Append("," + t.DbName);

                if (!t.HasValue)
                {
                    sql_insert_value.Append(",null");
                }
                else
                {
                    sql_insert_value.Append(",@" + t.DbName);
                    paramlist.Add(t);
                }
                
            }

            sql_insert.Append(")");
            sql_insert_value.Append(")");
            sql_insert.Append(sql_insert_value.ToString());

            SqlClient.CreateCommand(sql_insert.ToString());
            SqlClient.AddParameter("@Id", state.Id);
            SqlClient.AddParameter("@Version", state.Version);
            SqlClient.AddParameter("@ApplicationId", this.Model.Application.Id);
            SqlClient.AddParameter("@CreatedBy", state.UserId);
            SqlClient.AddParameter("@ChangedBy", state.UserId);
            SqlClient.AddParameter("@OwnedBy", state.OwnerUserId);
            SqlClient.AddParameter("@ChangedDate", this.ApplicationSaveTimeStamp.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
            SetParameters(paramlist);
            SqlClient.ExecuteNonQuery();

        }

        private void HandleSubTables(ClientStateInfo state)
        {
            foreach (var table in state.SubTables)
            {
                if (!table.HasModel)
                    continue;

                foreach (var row in table.Rows)
                {
                    if (row.Id < 1 || this.Model.Application.UseVersioning)
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

  

        private void UpdateMainTable(ClientStateInfo state)
        {
            var paramlist = new List<ApplicationValue>();

            StringBuilder sql_update = new StringBuilder();
            sql_update.Append("UPDATE " + this.Model.Application.DbName);
            sql_update.Append(" set ChangedDate='" + this.ApplicationSaveTimeStamp.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "'");
            sql_update.Append(",ChangedBy=@ChangedBy");


            foreach (var t in state.Values)
            {
                if (!t.HasModel)
                    continue;

                if (!t.HasValue)
                {
                    sql_update.Append("," + t.DbName + "=null");
                }
                else
                {
                    sql_update.Append("," + t.DbName + "=@" + t.DbName);
                    paramlist.Add(t);
                }
                
            }

            sql_update.Append(" WHERE ID=@ID and Version = @Version");


            SqlClient.CreateCommand(sql_update.ToString());
            SqlClient.AddParameter("@ID", state.Id);
            SqlClient.AddParameter("@Version", state.Version);
            SqlClient.AddParameter("@ChangedBy", state.UserId);
            SetParameters(paramlist);
            SqlClient.ExecuteNonQuery();

        }

       

        private void InsertTableRow(ApplicationTableRow data, ClientStateInfo state)
        {
            var paramlist = new List<ApplicationValue>();

            var rowid = GetNewSystemID(DatabaseModelItem.MetaTypeDataTable, data.Table.Model.MetaCode, state);
            if (rowid < 1)
                throw new InvalidOperationException("Could not get a new row id for table " + data.Table.DbName);

            var sql_insert = new StringBuilder();
            var sql_insert_value = new StringBuilder();
            sql_insert.Append("INSERT INTO " + data.Table.DbName + " ");
            sql_insert.Append(" (Id, Version, ApplicationId, CreatedBy,  ChangedBy, OwnedBy, ChangedDate, ParentId");

            sql_insert_value.Append(" VALUES (");
            sql_insert_value.Append(" @Id");
            sql_insert_value.Append(",@Version");
            sql_insert_value.Append(",@ApplicationId");
            sql_insert_value.Append(",@CreatedBy");
            sql_insert_value.Append(",@ChangedBy");
            sql_insert_value.Append(",@OwnedBy");
            sql_insert_value.Append(",@ChangedDate");
            sql_insert_value.Append(",@ParentId");


            foreach (var t in data.Values)
            {
                if (!t.HasModel)
                    continue;

                sql_insert.Append("," + t.DbName);

                if (!t.HasValue)
                {
                    sql_insert_value.Append(",null");
                }
                else
                {
                    sql_insert_value.Append(",@" + t.DbName);
                    paramlist.Add(t);
               }
                
            }

            sql_insert.Append(")");
            sql_insert_value.Append(")");
            sql_insert.Append(sql_insert_value.ToString());

            SqlClient.CreateCommand(sql_insert.ToString());
            SqlClient.AddParameter("@Id", rowid);
            SqlClient.AddParameter("@Version", state.Version);
            SqlClient.AddParameter("@ApplicationId", this.Model.Application.Id);
            SqlClient.AddParameter("@CreatedBy", state.UserId);
            SqlClient.AddParameter("@ChangedBy", state.UserId);
            SqlClient.AddParameter("@OwnedBy", state.OwnerUserId);
            SqlClient.AddParameter("@ChangedDate", this.ApplicationSaveTimeStamp.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
            SqlClient.AddParameter("@ParentId", state.Id);
            SetParameters(paramlist);
            SqlClient.ExecuteNonQuery();

        }

      

        private void UpdateTableRow(ApplicationTableRow data, ClientStateInfo state)
        {
            var paramlist = new List<ApplicationValue>();

            int rowid = 0;
            StringBuilder sql_update = new StringBuilder();
            sql_update.Append("UPDATE " + data.Table.DbName);
            sql_update.Append(" set ChangedDate='" + this.ApplicationSaveTimeStamp.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "'");
            sql_update.Append(",ChangedBy=@ChangedBy");


            foreach (var t in data.Values)
            {
                if (t.DbName.ToLower() == "id")
                    rowid = t.GetAsInt().Value;

                if (!t.HasModel)
                    continue;
             
                if (!t.HasValue)
                {
                    sql_update.Append("," + t.DbName + "=null");
                }
                else
                {
                    sql_update.Append("," + t.DbName + "=@" + t.DbName);
                    paramlist.Add(t);
                }
                
            }

            sql_update.Append(" WHERE ID=@ID and Version = @Version");


            SqlClient.CreateCommand(sql_update.ToString());
            SqlClient.AddParameter("@ID", rowid);
            SqlClient.AddParameter("@Version", state.Version);
            SqlClient.AddParameter("@ChangedBy", state.UserId);
            SetParameters(paramlist);
            SqlClient.ExecuteNonQuery();

        }

        private int CreateVersionRecord(ClientStateInfo state)
        {
            int newversion = 0;
            String sql = String.Empty;
            sql = "select max(version) from " + this.Model.Application.VersioningTableName;
            sql += " where ID=" + Convert.ToString(state.Id);
            sql += " and MetaCode='" + this.Model.Application.MetaCode + "' and MetaType='APPLICATION'";

            SqlClient.CreateCommand(sql);
            object obj = SqlClient.ExecuteScalarQuery();
            if (obj != null && obj != DBNull.Value)
            {

                newversion = Convert.ToInt32(obj);
                newversion += 1;
            }
            else
            {
                newversion = 1;
            }

            var getdatecmd = Commands.Find(p => p.Key == "GETDATE" && p.DbEngine == SqlClient.DbEngine);

            //DefaultVersioningTableColumns
            sql = "insert into " + this.Model.Application.VersioningTableName;
            sql += " (ID, Version, ApplicationId, MetaCode, MetaType, ChangedDate, ParentId)";
            sql += " VALUES (@P1, @P2, @P3, @P4, @P5, {0}, @P6)";
            sql = string.Format(sql, getdatecmd.Command);

            SqlClient.CreateCommand(sql);
            SqlClient.AddParameter("@P1", state.Id);
            SqlClient.AddParameter("@P2", newversion);
            SqlClient.AddParameter("@P3", this.Model.Application.Id);
            SqlClient.AddParameter("@P4", this.Model.Application.MetaCode);
            SqlClient.AddParameter("@P5", "APPLICATION");
            SqlClient.AddParameter("@P6", 0);
            SqlClient.ExecuteNonQuery();

         
            return newversion;
        }

      

        private OperationResult FetchLatestIdByOwnerUser(ClientStateInfo state)
        {
            var result = new OperationResult(true, string.Format("Fetched latest id for application {0} for Owner {1}", this.Model.Application.Title, state.OwnerUserId), 0, 0);

            try
            {
                SqlClient.Open();
                SqlClient.CreateCommand("SELECT max(id) from sysdata_InformationStatus where ApplicationId=@ApplicationId and OwnedBy=@OwnedBy");
                SqlClient.AddParameter("@ApplicationId", state.ApplicationId);
                SqlClient.AddParameter("@OwnedBy", state.OwnerUserId);
                var maxid = SqlClient.ExecuteScalarQuery();
                if (maxid != null && maxid != DBNull.Value)
                {
                
                    SqlClient.CreateCommand("SELECT Id,Version from sysdata_InformationStatus where Id = @Id");
                    SqlClient.AddParameter("@Id", Convert.ToInt32(maxid));
                    var ds = SqlClient.GetDataSet();
                    if (ds.Rows.Count == 0)
                    {
                        var fail = new OperationResult(false, string.Format("Latest id for application {0} for Owner {1} could not be found", this.Model.Application.Title, state.OwnerUserId), 0, 0);
                        return fail;
                    }

                    result.Id = ds.GetFirstRowIntValue("Id");
                    result.Version = ds.GetFirstRowIntValue("Version");
                }


            }
            catch (Exception ex)
            {
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", string.Format("Fetched latest id for application {0} for Owner {1} failed", this.Model.Application.Title, state.OwnerUserId));
                result.AddMessage("SYSTEMERROR", ex.Message);
                result.Data = "{}";

            }

            return result;
        }


    

        private void InsertInformationStatus(ClientStateInfo state)
        {
            var model = new InformationStatus() { Id = state.Id, ApplicationId= this.Model.Application.Id, 
                                                  ChangedBy= state.UserId, ChangedDate = DateTime.Now, CreatedBy = state.UserId, 
                                                  MetaCode = this.Model.Application.MetaCode, OwnedBy = state.OwnerUserId, 
                                                  PerformDate = DateTime.Now, Version = state.Version };

            SqlClient.Insert(model, true);

        }

        private void UpdateInformationStatus(ClientStateInfo state)
        {

            var getdatecmd = Commands.Find(p => p.Key == "GETDATE" && p.DbEngine == SqlClient.DbEngine);
            SqlClient.CreateCommand("Update sysdata_InformationStatus set ChangedDate="+getdatecmd.Command+", ChangedBy = @ChangedBy, Version = @Version WHERE ID=@ID");
            SqlClient.AddParameter("@ChangedBy", state.UserId);
            SqlClient.AddParameter("@Version", state.Version);
            SqlClient.AddParameter("@ID", state.Id);
            SqlClient.ExecuteNonQuery();
        }


        #endregion

        #region ConfigureDB

        private void CreateMainTable(OperationResult o)
        {

            var table_exist = false;
            SqlClient.Open();
            table_exist = SqlClient.TableExist(this.Model.Application.DbName);
            SqlClient.Close();
            if (table_exist)
            {
                o.AddMessage("DBCONFIG", "Main table " + this.Model.Application.DbName + " for application: " + this.Model.Application.Title + " is already present");
            }
            else
            {
    
                string create_sql = GetCreateTableStmt(ModelRepository.GetDefaultMainTableColumns(), this.Model.Application.DbName);

                SqlClient.Open();
                SqlClient.CreateCommand(create_sql);
                SqlClient.ExecuteScalarQuery();
                SqlClient.Close();

                o.AddMessage("DBCONFIG", "Main table: " + this.Model.Application.DbName + " for application: " + this.Model.Application.Title + "  was created successfully");

            }
        }

        private void CreateApplicationVersioningTable(OperationResult o)
        {
            var table_exist = false;
            SqlClient.Open();
            table_exist = SqlClient.TableExist(this.Model.Application.VersioningTableName);
            SqlClient.Close();
            if (table_exist)
            {
                //o.AddMessage("DBCONFIG", "Found versioning table (" + this.Model.Application.VersioningTableName + ") for application:" + this.Model.Application.Title);
            }
            else
            {

                string create_sql = GetCreateTableStmt(ModelRepository.GetDefaultVersioningTableColumns(), this.Model.Application.VersioningTableName);

                SqlClient.Open();
                SqlClient.CreateCommand(create_sql);
                SqlClient.ExecuteScalarQuery();
                SqlClient.Close();

                //o.AddMessage("DBCONFIG", "Versioning table: " + this.Model.Application.VersioningTableName + " was created successfully");

            }
        }

        private void CreateDBColumn(OperationResult o, DatabaseModelItem column, string tablename)
        {
            if (!column.IsMetaTypeDataColumn)
            {
                o.AddMessage("DBCONFIG_ERROR", "Invalid MetaType when configuring column");
                return;
            }


            var colexist = false;
            SqlClient.Open();
            colexist = SqlClient.ColumnExist(tablename, column.DbName);
            SqlClient.Close();
       
            if (colexist)
            {
               o.AddMessage("DBCONFIG", "Column: " + column.DbName + " in table: " + tablename + " is already present.");
            }
            else
            {
                var coldt = DataTypes.Find(p => p.IntwentyType == column.DataType && p.DbEngine == SqlClient.DbEngine);
                string create_sql = "ALTER TABLE " + tablename + " ADD " + column.DbName + " " + coldt.DBMSDataType;

                SqlClient.Open();
                SqlClient.CreateCommand(create_sql);
                SqlClient.ExecuteScalarQuery();
                SqlClient.Close();

                o.AddMessage("DBCONFIG", "Column: " + column.DbName + " ("+coldt.DBMSDataType+") was created successfully in table: " + tablename);

            }


        }

        private void CreateDBTable(OperationResult o, DatabaseModelItem table)
        {

            if (!table.IsMetaTypeDataTable)
            {
                o.AddMessage("DBCONFIG_ERROR", "Invalid MetaType when configuring table");
                return;
            }


            var table_exist = false;
            SqlClient.Open();
            table_exist = SqlClient.TableExist(table.DbName);
            SqlClient.Close();
            if (table_exist)
            {
                o.AddMessage("DBCONFIG", "Table: " + table.DbName + " in application: " + this.Model.Application.Title + " is already present.");
            }
            else
            {

                string create_sql = GetCreateTableStmt(ModelRepository.GetDefaultSubTableColumns(), table.DbName);

                SqlClient.Open();
                SqlClient.CreateCommand(create_sql);
                SqlClient.ExecuteScalarQuery();
                SqlClient.Close();

                o.AddMessage("DBCONFIG", "Subtable: " + table.DbName + " in application: " + this.Model.Application.Title + "  was created successfully");

            }

            foreach (var t in Model.DataStructure)
            {
                if (t.IsMetaTypeDataColumn && t.ParentMetaCode == table.MetaCode)
                    CreateDBColumn(o, t, table.DbName);
            }

        }

        private void CreateIndexes(OperationResult o)
        {
            string sql = string.Empty;

            SqlClient.Open();

            try
            {

                //Ctreate index on main application table
                sql = string.Format("CREATE UNIQUE INDEX {0}_Idx1 ON {0} (Id, Version)", this.Model.Application.DbName);
                SqlClient.CreateCommand(sql);
                SqlClient.ExecuteScalarQuery();

              

                //Create index on versioning table
                sql = string.Format("CREATE UNIQUE INDEX {0}_Idx1 ON {0} (Id, Version, MetaCode, MetaType)", this.Model.Application.VersioningTableName);
                SqlClient.CreateCommand(sql);
                SqlClient.ExecuteScalarQuery();

               
                //Create index on subtables
                foreach (var t in this.Model.DataStructure)
                {
                    if (t.IsMetaTypeDataTable)
                    {
                        sql = string.Format("CREATE UNIQUE INDEX {0}_Idx1 ON {0} (Id, Version)", t.DbName);
                        SqlClient.CreateCommand(sql);
                        SqlClient.ExecuteScalarQuery();

                    

                        sql = string.Format("CREATE INDEX {0}_Idx3 ON {0} (ParentId)", t.DbName);
                        SqlClient.CreateCommand(sql);
                        SqlClient.ExecuteScalarQuery();

                    }
                }

                o.AddMessage("DBCONFIG", "Database Indexes was created successfully for application " + this.Model.Application.Title);

            }
            catch
            {
               
            }

            SqlClient.Close();




        }

        private string GetCreateTableStmt(List<IntwentyDataColumn> columns, string tablename)
        {
            var res = string.Format("CREATE TABLE {0}", tablename) + " (";
            var sep = "";
            foreach (var c in columns)
            {
                SqlDataTypeMap dt;
                if (c.DataType == DatabaseModelItem.DataTypeString)
                   dt = DataTypes.Find(p => p.IntwentyType == c.DataType && p.DbEngine == SqlClient.DbEngine && p.Length == StringLength.Short);
                else
                    dt = DataTypes.Find(p => p.IntwentyType == c.DataType && p.DbEngine == SqlClient.DbEngine);

                res += sep + string.Format("{0} {1} not null", c.ColumnName, dt.DBMSDataType);
                sep = ", ";
            }

            res += ")";

            return res;

        }

      


        #endregion






    }
}
    
