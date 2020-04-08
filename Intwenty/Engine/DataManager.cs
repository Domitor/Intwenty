using Intwenty.Data.DBAccess;
using Intwenty.Data.DBAccess.Helpers;
using Intwenty.Data.Dto;
using Intwenty.Data.Entity;
using Intwenty.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Intwenty.Engine
{
    public enum LifecycleStatus
    {
        NONE = 0
        , NEW_NOT_SAVED = 1
        , NEW_SAVED = 2
        , EXISTING_NOT_SAVED = 3
        , EXISTING_SAVED = 4
        , DELETED_NOT_SAVED = 5
        , DELETED_SAVED = 6
    }

    public interface IDataManager
    {
        OperationResult ConfigureDatabase();

        OperationResult GetLatestIdByOwnerUser(ClientStateInfo data);

        OperationResult GetLatestVersion(ClientStateInfo data);

        OperationResult GetVersion();

        OperationResult GetListView(ListRetrivalArgs args);

        OperationResult Save(ClientStateInfo data);

        OperationResult GetValueDomains();

        OperationResult GetDataView(List<DataViewModelItem> viewinfo, ListRetrivalArgs args);

        OperationResult GetDataViewValue(List<DataViewModelItem> viewinfo, ListRetrivalArgs args);

        OperationResult GenerateTestData(int gencount);

    }

   

    public class DataManager : IDataManager
    {
        public IIntwentyModelService ModelRepository { get; set; }

        public IIntwentyDbAccessService DataRepository { get; set; }

        protected ApplicationModel Meta { get; set; }

        public ClientStateInfo ClientState { get; set; }

        protected LifecycleStatus Status { get; set; }

        protected bool CanRollbackVersion { get; set; }

        protected DateTime ApplicationSaveTimeStamp { get; set; }

        protected List<SqlDataTypeMap> DataTypes { get; set; }

        protected List<DBMSCommandMap> Commands { get; set; }

        protected DataManager(ApplicationModel application)
        {
            Meta = application;
            ApplicationSaveTimeStamp = DateTime.Now;
            DataTypes = DBHelpers.GetDataTypeMap();
            Commands = DBHelpers.GetDBMSCommandMap();
        }

        public static DataManager GetDataManager(ApplicationModel application)
        {

            if (application.Application.MetaCode == "XXXXX")
            {
                return new  Custom.AppDataManagerExample(application);
            }
            else
            {
                var t = new DataManager(application);
                return t;
            }
        }

      
        

        #region Implementation

        public OperationResult ConfigureDatabase()
        {
            var res = new OperationResult();

            try
            {
                CreateMainTable(res);
                CreateApplicationVersioningTable(res);

                foreach (var t in Meta.DataStructure)
                {
                    if (t.IsMetaTypeDataColumn && t.IsRoot)
                    {
                        CreateDBColumn(res, t, Meta.Application.DbName);
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
                throw ex;
            }

            return res;
        }

        public OperationResult GetLatestIdByOwnerUser(ClientStateInfo data)
        {
            ClientState = data;
            var result = new OperationResult(true, string.Format("Fetched latest id for application {0} for Owner {1}", this.Meta.Application.Title, data.OwnerUserId), 0, 0);

            try
            {
                DataRepository.Open();
                DataRepository.CreateCommand("SELECT max(id) from sysdata_InformationStatus where ApplicationId=@ApplicationId and OwnedBy=@OwnedBy");
                DataRepository.AddParameter("@ApplicationId", data.ApplicationId);
                DataRepository.AddParameter("@OwnedBy", data.OwnerUserId);
                var maxid = DataRepository.ExecuteScalarQuery();
                if (maxid != null && maxid != DBNull.Value)
                {
                    var ds = new DataSet();
                    DataRepository.CreateCommand("SELECT id,version from sysdata_InformationStatus where Id = @Id");
                    DataRepository.AddParameter("@Id", (int)maxid);
                    DataRepository.FillDataset(ds, "Result");
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        var fail = new OperationResult(false, string.Format("Latest id for application {0} for Owner {1} could not be found", this.Meta.Application.Title, data.OwnerUserId), 0, 0);
                        return fail;
                    }

                    result.Id = DBHelpers.GetRowIntValue(ds.Tables[0], "id");
                    result.Version = DBHelpers.GetRowIntValue(ds.Tables[0], "version");
                }

               
            }
            catch (Exception ex)
            {
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", string.Format("Fetched latest id for application {0} for Owner {1} failed", this.Meta.Application.Title, data.OwnerUserId));
                result.AddMessage("SYSTEMERROR", ex.Message);
                result.Data = "{}";

            }

            return result;
        }

        public virtual OperationResult GetLatestVersion(ClientStateInfo data)
        {
            ClientState = data;
            var jsonresult = new StringBuilder();
            var result = new OperationResult(true, string.Format("Fetched latest version for application {0}", this.Meta.Application.Title), ClientState.Id, ClientState.Version);

            try
            {
                var columns = new List<IIntwentyDataColum>();
                columns.Add(new IntwentyDataColum() { ColumnName = "Id", IsDateTime = false, IsNumeric = true });
                columns.Add(new IntwentyDataColum() { ColumnName = "Version", IsDateTime = false, IsNumeric = true });
                columns.Add(new IntwentyDataColum() { ColumnName = "ApplicationId", IsDateTime = false, IsNumeric = true });
                columns.Add(new IntwentyDataColum() { ColumnName = "OwnerId", IsDateTime = false, IsNumeric = true });
                columns.Add(new IntwentyDataColum() { ColumnName = "RowChangeDate", IsDateTime = true, IsNumeric = false });
                columns.Add(new IntwentyDataColum() { ColumnName = "UserID", IsDateTime = false, IsNumeric = false });
                columns.Add(new IntwentyDataColum() { ColumnName = "CreatedBy", IsDateTime = false, IsNumeric = false });
                columns.Add(new IntwentyDataColum() { ColumnName = "ChangedBy", IsDateTime = false, IsNumeric = false });
                columns.Add(new IntwentyDataColum() { ColumnName = "OwnedBy", IsDateTime = false, IsNumeric = false });

                var sql_stmt = new StringBuilder();
                sql_stmt.Append("SELECT t1.* ");
                foreach (var t in this.Meta.DataStructure)
                {
                    if (t.IsMetaTypeDataColumn && t.IsRoot)
                    {
                        sql_stmt.Append(", t2." + t.DbName + " ");
                        columns.Add(new IntwentyDataColum() { ColumnName = t.DbName, IsDateTime = t.IsDateTime, IsNumeric=t.IsNumeric });

                    }
                }
                sql_stmt.Append("FROM sysdata_InformationStatus t1 ");
                sql_stmt.Append("JOIN " + this.Meta.Application.DbName + " t2 on t1.Id=t2.Id and t1.Version = t2.Version ");
                sql_stmt.Append("WHERE t1.ApplicationId = " + this.Meta.Application.Id + " ");
                sql_stmt.Append("AND t1.Id = " + this.ClientState.Id);
               

                jsonresult.Append("{");

                var ds = new DataSet();
                DataRepository.Open();
                DataRepository.CreateCommand(sql_stmt.ToString());
                var appjson = DataRepository.GetAsJSONObject(columns);
                DataRepository.Close();
               
                if (appjson.Length < 5)
                {
                    jsonresult.Append("}");
                    result.Data = jsonresult.ToString();
                    return result;
                }

                jsonresult.Append("\"" + this.Meta.Application.DbName + "\":" + appjson.ToString());

                //SUBTABLES
                foreach (var t in this.Meta.DataStructure)
                {
                    if (t.IsMetaTypeDataTable && t.IsRoot)
                    {
                        columns = new List<IIntwentyDataColum>();
                        columns.Add(new IntwentyDataColum() { ColumnName = "ApplicationId", IsDateTime = false, IsNumeric = true });
                        columns.Add(new IntwentyDataColum() { ColumnName = "Id", IsDateTime = false, IsNumeric = true });
                        columns.Add(new IntwentyDataColum() { ColumnName = "RowChangeDate", IsDateTime = true, IsNumeric = false });
                        columns.Add(new IntwentyDataColum() { ColumnName = "UserID", IsDateTime = false, IsNumeric = false });
                        columns.Add(new IntwentyDataColum() { ColumnName = "ParentId", IsDateTime = false, IsNumeric = true });
                        columns.Add(new IntwentyDataColum() { ColumnName = "Version", IsDateTime = false, IsNumeric = true });
                        columns.Add(new IntwentyDataColum() { ColumnName = "OwnerId", IsDateTime = false, IsNumeric = true });

                        sql_stmt = new StringBuilder();
                        sql_stmt.Append("SELECT t1.ApplicationId ");
                        sql_stmt.Append(", t2.Id ");
                        sql_stmt.Append(", t2.RowChangeDate ");
                        sql_stmt.Append(", t2.UserId ");
                        sql_stmt.Append(", t2.ParentId ");
                        sql_stmt.Append(", t2.Version ");
                        sql_stmt.Append(", t2.OwnerId ");
                        foreach (var v in this.Meta.DataStructure)
                        {
                            if (v.IsMetaTypeDataColumn && v.ParentMetaCode == t.MetaCode)
                            {
                                sql_stmt.Append(", t2." + v.DbName + " ");
                                columns.Add(new IntwentyDataColum() { ColumnName = v.DbName, IsDateTime = v.IsDateTime, IsNumeric = v.IsNumeric });
                            }
                        }
                        sql_stmt.Append("FROM sysdata_InformationStatus t1 ");
                        sql_stmt.Append("JOIN " + t.DbName + " t2 on t1.Id=t2.ParentId and t1.Version = t2.Version ");
                        sql_stmt.Append("WHERE t1.ApplicationId = " + this.Meta.Application.Id + " ");
                        sql_stmt.Append("AND t1.Id = " + this.ClientState.Id);
                        DataRepository.Open();
                        DataRepository.CreateCommand(sql_stmt.ToString());
                        var tablearray = DataRepository.GetAsJSONArray(columns);
                        DataRepository.Close();

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
                result.AddMessage("USERERROR", string.Format("Get latest version for application {0} failed", this.Meta.Application.Title));
                result.AddMessage("SYSTEMERROR", ex.Message);
                result.Data = "{}";

            }

            return result;
        }

        public virtual OperationResult GetVersion()
        {
            throw new NotImplementedException();
        }

        public virtual OperationResult Save(ClientStateInfo data)
        {
            ClientState = data;
            if (ClientState == null)
                return new OperationResult(false, "No client state found when performing save application.", 0, 0);

            var result = new OperationResult(true, string.Format("Saved application {0}", this.Meta.Application.Title), ClientState.Id, ClientState.Version);

            try
            {
                //CONNECT MODEL TO DATA
                data.InferModel(Meta);

                DataRepository.Open();

                BeforeSave(data);

                if (ClientState.Id < 1)
                {
                    this.ClientState.Id = GetNewSystemID("APPLICATION", this.Meta.Application.MetaCode);
                    this.Status = LifecycleStatus.NEW_NOT_SAVED;
                    this.ClientState.Version = CreateVersionRecord();
                    BeforeSaveNew(data);
                    InsertMainTable(data);
                    InsertInformationStatus();
                    HandleSubTables(data);
                    this.Status = LifecycleStatus.NEW_SAVED;
                }
                else if (ClientState.Id > 0 && this.Meta.Application.UseVersioning)
                {
                    this.Status = LifecycleStatus.EXISTING_NOT_SAVED;
                    this.ClientState.Version = CreateVersionRecord();
                    BeforeSaveUpdate(data);
                    InsertMainTable(data);
                    UpdateInformationStatus();
                    HandleSubTables(data);
                    this.Status = LifecycleStatus.EXISTING_SAVED;
                }
                else if (ClientState.Id > 0 && !this.Meta.Application.UseVersioning)
                {
                    this.Status = LifecycleStatus.EXISTING_NOT_SAVED;
                    BeforeSaveUpdate(data);
                    UpdateMainTable(data);
                    UpdateInformationStatus();
                    HandleSubTables(data);
                    this.Status = LifecycleStatus.EXISTING_SAVED;
                }

                DataRepository.Close();

                result.Id = ClientState.Id;
                result.Version = ClientState.Version;

                AfterSave(data);

            }
            catch (Exception ex)
            {
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", string.Format("Save application {0} failed", this.Meta.Application.Title));
                result.AddMessage("SYSTEMERROR", ex.Message);
            }

            return result;

        }

        public virtual OperationResult GetListView(ListRetrivalArgs args)
        {
            if (args == null)
                return new OperationResult(false, "Can't get list without ListRetrivalArgs",0,0);

            var result = new OperationResult(true, string.Format("Fetched list for application {0}", this.Meta.Application.Title),0,0);
          
            if (args.MaxCount == 0)
            {
                DataRepository.Open();
                DataRepository.CreateCommand("select count(*) FROM sysdata_InformationStatus where ApplicationId = " + this.Meta.Application.Id);
                var max = DataRepository.ExecuteScalarQuery();
                if (max == DBNull.Value)
                    args.MaxCount = 0;
                else
                    args.MaxCount = Convert.ToInt32(max);

                DataRepository.Close();
            }

            result.RetriveListArgs = new ListRetrivalArgs();
            result.RetriveListArgs = args;


            try
            {
                var columns = new List<IIntwentyDataColum>();
                columns.Add(new IntwentyDataColum() { ColumnName = "Id", IsDateTime = false, IsNumeric = true });
                columns.Add(new IntwentyDataColum() { ColumnName = "Version", IsDateTime = false, IsNumeric = true });
                columns.Add(new IntwentyDataColum() { ColumnName = "ApplicationId", IsDateTime = false, IsNumeric = true });
                columns.Add(new IntwentyDataColum() { ColumnName = "OwnerId", IsDateTime = false, IsNumeric = true });
                columns.Add(new IntwentyDataColum() { ColumnName = "RowChangeDate", IsDateTime = true, IsNumeric = false });
                columns.Add(new IntwentyDataColum() { ColumnName = "UserID", IsDateTime = false, IsNumeric = false });
                columns.Add(new IntwentyDataColum() { ColumnName = "CreatedBy", IsDateTime = false, IsNumeric = false });
                columns.Add(new IntwentyDataColum() { ColumnName = "ChangedBy", IsDateTime = false, IsNumeric = false });
                columns.Add(new IntwentyDataColum() { ColumnName = "OwnedBy", IsDateTime = false, IsNumeric = false });

                var sql_list_stmt = new StringBuilder();
                sql_list_stmt.Append("SELECT t1.* ");
                foreach (var t in this.Meta.UIStructure)
                {
                    if (t.IsMetaTypeListViewField && t.IsDataColumnConnected)
                    {
                        sql_list_stmt.Append(", t2." + t.DataColumnInfo.DbName + " ");
                        columns.Add(new IntwentyDataColum() { ColumnName = t.DataColumnInfo.DbName, IsDateTime=t.DataColumnInfo.IsDateTime, IsNumeric = t.DataColumnInfo.IsNumeric });
                    }
                }
                sql_list_stmt.Append("FROM sysdata_InformationStatus t1 ");
                sql_list_stmt.Append("JOIN " + this.Meta.Application.DbName + " t2 on t1.Id=t2.Id and t1.Version = t2.Version ");
                sql_list_stmt.Append("WHERE t1.ApplicationId = " + this.Meta.Application.Id + " ");


                if (!string.IsNullOrEmpty(args.FilterField) && !string.IsNullOrEmpty(args.FilterValue))
                    sql_list_stmt.Append("AND t2."+ args.FilterField + " LIKE '%"+ args.FilterValue + "%'  ");

                sql_list_stmt.Append("ORDER BY t1.Id");


                var ds = new DataSet();
                DataRepository.Open();
                DataRepository.CreateCommand(sql_list_stmt.ToString());
                var json = DataRepository.GetAsJSONArray(columns, result.RetriveListArgs.CurrentRowNum, (result.RetriveListArgs.CurrentRowNum + result.RetriveListArgs.BatchSize));
                DataRepository.Close();

                result.Data = json.ToString();

            }
            catch (Exception ex)
            {
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", string.Format("Fetch list for application {0} failed", this.Meta.Application.Title));
                result.AddMessage("SYSTEMERROR", ex.Message);
                result.Data = "[]";

            }

            return result;


        }

        /// <summary>
        /// Creates a JSON object with all valuedomains (arrays) used in an application
        /// </summary>
        public virtual OperationResult GetValueDomains()
        {
            var columns = new List<IIntwentyDataColum>();
            columns.Add(new IntwentyDataColum() { ColumnName = "Id",  IsDateTime=false, IsNumeric=true });
            columns.Add(new IntwentyDataColum() { ColumnName = "DomainName",  IsDateTime = false, IsNumeric = false });
            columns.Add(new IntwentyDataColum() { ColumnName = "Code",  IsDateTime = false, IsNumeric = false });
            columns.Add(new IntwentyDataColum() { ColumnName = "Value",  IsDateTime = false, IsNumeric = false });

            var sb = new StringBuilder();
            var result = new OperationResult(true, string.Format("Fetched doamins used in ui for application {0}", this.Meta.Application.Title), 0, 0);

            try
            {
                var valuedomains = new List<string>();


                //COLLECT DOMAINS AND VIEWS USED BY UI
                foreach (var t in this.Meta.UIStructure)
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

                var ds = new DataSet();
                DataRepository.Open();

                foreach (var d in valuedomains)
                {

                    DataRepository.CreateCommand("SELECT Id, DomainName, Code, Value FROM sysmodel_ValueDomainItem WHERE DomainName = @P1");
                    DataRepository.AddParameter("@P1", d);
                    DataRepository.FillDataset(ds, "VALUEDOMAIN_" + d);
                }

                DataRepository.Close();

                var domainindex = 0;
                var rowindex = 0;
                var columnindex = 0;

                sb.Append("{");


                foreach (DataTable table in ds.Tables)
                {
                    foreach (var ic in columns)
                    {
                        ic.QueryResultColumn = null;
                        foreach (DataColumn dc in table.Columns)
                        {
                            if (dc.ColumnName.ToLower() == ic.ColumnName.ToLower())
                            {
                                ic.QueryResultColumn = dc;
                                break;
                            }
                        }
                    }

                    if (domainindex == 0)
                        sb.Append("\"" + table.TableName + "\":[");
                    else
                        sb.Append(",\"" + table.TableName + "\":[");

                    domainindex += 1;
                    rowindex = 0;
                   

                    foreach (DataRow row in table.Rows)
                    {
                        if (rowindex == 0)
                            sb.Append("{");
                        else
                            sb.Append(",{");

                        rowindex += 1;
                        columnindex = 0;
                        foreach (var dc in columns)
                        {
                            var val = DBHelpers.GetJSONValue(row, dc, DataRepository.GetDBMS());
                            if (string.IsNullOrEmpty(val))
                                continue;

                            if (columnindex == 0)
                                sb.Append(val);
                            else
                                sb.Append("," + val);

                            columnindex += 1;
                        }
                        sb.Append("}");
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
                result.AddMessage("USERERROR", string.Format("Fetch valuedomains for application {0} failed", this.Meta.Application.Title));
                result.AddMessage("SYSTEMERROR", ex.Message);
                result.Data = "[]";
 
            }

            return result;

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
                if (dv== null)
                    throw new InvalidOperationException("Could not find dataview to fetch");
                if (dv.HasNonSelectSql)
                    throw new InvalidOperationException(string.Format("The sql query defined for dataview {0} has invalid statements.", dv.Title + " ("+dv.MetaCode+")"));


                var columns = new List<IIntwentyDataColum>();
                foreach (var viewcol in viewinfo)
                {
                    if ((viewcol.IsMetaTypeDataViewColumn || viewcol.IsMetaTypeDataViewKeyColumn) && viewcol.ParentMetaCode == dv.MetaCode)
                    {
                        columns.Add(new IntwentyDataColum() { ColumnName = viewcol.SQLQueryFieldName, IsDateTime = false, IsNumeric = false });
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

                DataRepository.Open();
                DataRepository.CreateCommand(sql);
                StringBuilder json = null;
                json = DataRepository.GetAsJSONArray(columns, result.RetriveListArgs.CurrentRowNum, (result.RetriveListArgs.CurrentRowNum + result.RetriveListArgs.BatchSize));
                DataRepository.Close();

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

        public OperationResult GetDataViewValue(List<DataViewModelItem> viewinfo, ListRetrivalArgs args)
        {
            var sql = "";
            var result = new OperationResult(true, "Fetched dataview value", 0, 0);
            

            try
            {
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
                        columns.Add(new IntwentyDataColum() { ColumnName = viewcol.SQLQueryFieldName, IsDateTime = false, IsNumeric = false });
                    }
                }

                DataRepository.Open();
                DataRepository.CreateCommand(sql);
                DataRepository.AddParameter("@P1", args.FilterValue);
                var data = DataRepository.GetAsJSONObject(columns);
                DataRepository.Close();

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

        public OperationResult GenerateTestData(int gencount)
        {
            var rnd = new Random(9999999);
            var data = new ClientStateInfo();

            foreach (var t in this.Meta.DataStructure)
            {

                if (t.IsMetaTypeDataColumn)
                {
                    if (t.IsDataTypeString)
                    {
                        var noserie = this.Meta.NoSeries.Find(p => p.DataMetaCode == t.MetaCode);
                        if (noserie != null)
                        {
                            var nolist = ModelRepository.GetNewNoSeriesValues(this.Meta.Application.Id);
                            var noseriesval = nolist.FirstOrDefault(p => p.DataMetaCode == t.MetaCode).NewValue;
                            data.Values.Add(new ApplicationValue() { DbName = t.DbName, Value = noseriesval });
                            continue;
                        }
                    }

                    var lookup = this.Meta.UIStructure.Find(p => p.IsMetaTypeLookUp && p.IsDataViewConnected && p.IsDataViewColumnConnected && p.IsDataColumnConnected);
                    if (lookup != null)
                    {

                        var ds = new DataSet();
                        DataRepository.Open();
                        DataRepository.CreateCommand(lookup.DataViewInfo.SQLQuery);
                        DataRepository.FillDataset(ds, "VIEW");
                        DataRepository.Close();
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            var maxindex = ds.Tables[0].Rows.Count - 1;
                            var rowindex = new Random(1).Next(0, maxindex);
                            data.Values.Add(new ApplicationValue() { DbName = t.DbName, Value = ds.Tables[0].Rows[rowindex][lookup.DataViewColumnInfo.SQLQueryFieldName] });
                            if (lookup.IsDataViewColumn2Connected && lookup.IsDataColumn2Connected)
                            {
                                data.Values.Add(new ApplicationValue() { DbName = lookup.DataColumnInfo2.MetaCode, Value = ds.Tables[0].Rows[rowindex][lookup.DataViewColumnInfo2.SQLQueryFieldName] });
                            }
                        }

                    }
                    else
                    {

                        var value = GetSemanticValue(t, rnd, gencount);
                        if (value != null)
                            data.Values.Add(new ApplicationValue() { DbName = t.MetaCode, Value = value });

                    }
                }

            }

            return this.Save(data);

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
                        DataRepository.AddParameter("@" + p.DbName, val);
                    else
                        DataRepository.AddParameter("@" + p.DbName, DBNull.Value);
                }
                else if (p.Model.IsDataTypeInt)
                {
                    var val = p.GetAsInt();
                    if (val.HasValue)
                        DataRepository.AddParameter("@" + p.DbName, val.Value);
                    else
                        DataRepository.AddParameter("@" + p.DbName, DBNull.Value);
                }
                else if (p.Model.IsDataTypeBool)
                {
                    var val = p.GetAsBool();
                    if (val.HasValue)
                        DataRepository.AddParameter("@" + p.DbName, val.Value);
                    else
                        DataRepository.AddParameter("@" + p.DbName, DBNull.Value);
                }
                else if (p.Model.IsDataTypeDateTime)
                {
                    var val = p.GetAsDateTime();
                    if (val.HasValue)
                        DataRepository.AddParameter("@" + p.DbName, val.Value);
                    else
                        DataRepository.AddParameter("@" + p.DbName, DBNull.Value);
                }
                else if (p.Model.IsDataType1Decimal || p.Model.IsDataType2Decimal || p.Model.IsDataType3Decimal)
                {
                    var val = p.GetAsDecimal();
                    if (val.HasValue)
                        DataRepository.AddParameter("@" + p.DbName, val.Value);
                    else
                        DataRepository.AddParameter("@" + p.DbName, DBNull.Value);
                }
            }

        }


      

        #endregion

        #region Save

        private int GetNewSystemID(string metatype, string metacode)
        {

            var model = new SystemID() { ApplicationId=this.Meta.Application.Id, GeneratedDate=DateTime.Now, MetaCode =metacode, MetaType = metatype, Properties= this.ClientState.Properties };
            var result = DataRepository.Insert(model, true);
            return model.Id;
        }

        private void InsertMainTable(ClientStateInfo data)
        {
            var paramlist = new List<ApplicationValue>();

            if (this.ClientState.Id < 1)
                throw new InvalidOperationException("Invalid systemid");

            var sql_insert = new StringBuilder();
            var sql_insert_value = new StringBuilder();
            sql_insert.Append("INSERT INTO " + this.Meta.Application.DbName + " ");
            sql_insert.Append(" (ID, RowChangeDate, UserID,  Version, OwnerId");
            sql_insert_value.Append(" VALUES (" + Convert.ToString(this.ClientState.Id));
            sql_insert_value.Append(",'" + this.ApplicationSaveTimeStamp.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "'");
            sql_insert_value.Append(",'" + this.ClientState.UserId + "'");
            sql_insert_value.Append("," + Convert.ToString(this.ClientState.Version));
            sql_insert_value.Append("," + Convert.ToString(this.ClientState.OwnerId));

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

            DataRepository.CreateCommand(sql_insert.ToString());
            SetParameters(paramlist);
            DataRepository.ExecuteNonQuery();

        }

        private void HandleSubTables(ClientStateInfo data)
        {
            foreach (var table in data.SubTables)
            {
                if (!table.HasModel)
                    continue;

                foreach (var row in table.Rows)
                {
                    if (row.Id < 1 || this.Meta.Application.UseVersioning)
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


        private void UpdateMainTable(ClientStateInfo data)
        {
            var paramlist = new List<ApplicationValue>();

            StringBuilder sql_update = new StringBuilder();
            sql_update.Append("UPDATE " + this.Meta.Application.DbName);
            sql_update.Append(" set RowChangeDate='" + this.ApplicationSaveTimeStamp.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "'");
            sql_update.Append(",UserID='"+this.ClientState.UserId+"'");
            sql_update.Append(",OwnerId=" + Convert.ToString(this.ClientState.OwnerId));

            foreach (var t in data.Values)
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


            DataRepository.CreateCommand(sql_update.ToString());
            DataRepository.AddParameter("@ID", ClientState.Id);
            DataRepository.AddParameter("@Version", ClientState.Version);
            SetParameters(paramlist);
            DataRepository.ExecuteNonQuery();

        }

        private void InsertTableRow(ApplicationTableRow data)
        {
            var paramlist = new List<ApplicationValue>();

            var rowid = GetNewSystemID(DatabaseModelItem.MetaTypeDataTable, data.Table.Model.MetaCode);
            if (rowid < 1)
                throw new InvalidOperationException("Could not get a new row id for table " + data.Table.DbName);

            var sql_insert = new StringBuilder();
            var sql_insert_value = new StringBuilder();
            sql_insert.Append("INSERT INTO " + data.Table.DbName + " ");
            sql_insert.Append(" (Id, ParentId, RowChangeDate, UserID,  Version, OwnerId");
            sql_insert_value.Append(" VALUES (" + Convert.ToString(rowid));
            sql_insert_value.Append(", " + Convert.ToString(this.ClientState.Id));
            sql_insert_value.Append(",'" + this.ApplicationSaveTimeStamp.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "'");
            sql_insert_value.Append(",'SYSTEM'");
            sql_insert_value.Append("," + Convert.ToString(this.ClientState.Version));
            sql_insert_value.Append("," + Convert.ToString(this.ClientState.OwnerId));

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

            DataRepository.CreateCommand(sql_insert.ToString());
            SetParameters(paramlist);
            DataRepository.ExecuteNonQuery();

        }

        private void UpdateTableRow(ApplicationTableRow data)
        {
            var paramlist = new List<ApplicationValue>();

            StringBuilder sql_update = new StringBuilder();
            sql_update.Append("UPDATE " + data.Table.DbName);
            sql_update.Append(" set RowChangeDate='" + this.ApplicationSaveTimeStamp.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "'");
            sql_update.Append(",UserID='SYSTEM'");
            sql_update.Append(",OwnerId=" + Convert.ToString(this.ClientState.OwnerId));

            foreach (var t in data.Values)
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


            DataRepository.CreateCommand(sql_update.ToString());
            DataRepository.AddParameter("@ID", ClientState.Id);
            DataRepository.AddParameter("@Version", ClientState.Version);
            SetParameters(paramlist);
            DataRepository.ExecuteNonQuery();

        }

        private int CreateVersionRecord()
        {
            int newversion = 0;
            String sql = String.Empty;
            sql = "select max(version) from " + this.Meta.Application.VersioningTableName;
            sql += " where ID=" + Convert.ToString(this.ClientState.Id);
            sql += " and MetaCode='" + this.Meta.Application.MetaCode + "' and MetaType='APPLICATION'";

            DataRepository.CreateCommand(sql);
            object obj = DataRepository.ExecuteScalarQuery();
            if (obj != null && obj != DBNull.Value)
            {

                newversion = Convert.ToInt32(obj);
                newversion += 1;
            }
            else
            {
                newversion = 1;
            }

            var getdatecmd = Commands.Find(p => p.Key == "GETDATE" && p.DBMSType == DataRepository.GetDBMS());

            sql = "insert into " + this.Meta.Application.VersioningTableName;
            sql += " (ID, ParentID, MetaCode, MetaType, Version, OwnerId, RowChangeDate, UserID)";
            sql += " VALUES (@P1, @P2, @P3, @P4, @P5, @P6, {0}, @P8)";
            sql = string.Format(sql, getdatecmd.Command);

            DataRepository.CreateCommand(sql);
            DataRepository.AddParameter("@P1", this.ClientState.Id);
            DataRepository.AddParameter("@P2", 0);
            DataRepository.AddParameter("@P3", this.Meta.Application.MetaCode);
            DataRepository.AddParameter("@P4", "APPLICATION");
            DataRepository.AddParameter("@P5", newversion);
            DataRepository.AddParameter("@P6", this.ClientState.OwnerId);
            DataRepository.AddParameter("@P8", this.ClientState.UserId);
            DataRepository.ExecuteNonQuery();

            if (this.ClientState.Version < newversion)
                CanRollbackVersion = true;

            return newversion;
        }

       

        private DataSet GetNewDataSet()
        {
            DataSet ds = new DataSet(this.Meta.Application.MetaCode);

            ds.Tables.Add(this.Meta.Application.DbName);
            ds.Tables[this.Meta.Application.DbName].Columns.Add("ID", typeof(int));
            ds.Tables[this.Meta.Application.DbName].Columns.Add("RowChangeDate", typeof(DateTime));
            ds.Tables[this.Meta.Application.DbName].Columns.Add("UserID", typeof(string));
            ds.Tables[this.Meta.Application.DbName].Columns.Add("Version", typeof(int));
            ds.Tables[this.Meta.Application.DbName].Columns.Add("OwnerId", typeof(int));

            foreach (var t in this.Meta.DataStructure)
            {


                if (t.IsMetaTypeDataColumn && t.IsRoot)
                {

                    if (t.IsDataTypeInt)
                        ds.Tables[this.Meta.Application.DbName].Columns.Add(t.DbName, typeof(int));
                    else if (t.IsDataTypeDateTime)
                        ds.Tables[this.Meta.Application.DbName].Columns.Add(t.DbName, typeof(DateTime));
                    else if (t.IsDataTypeText || t.IsDataTypeString)
                        ds.Tables[this.Meta.Application.DbName].Columns.Add(t.DbName, typeof(string));
                    else
                        ds.Tables[this.Meta.Application.DbName].Columns.Add(t.DbName, typeof(double));
                }

                if (t.IsMetaTypeDataTable)
                {
                    ds.Tables.Add(t.DbName);
                    ds.Tables[t.DbName].Columns.Add("Id", typeof(int));
                    ds.Tables[t.DbName].Columns.Add("RowChangeDate", typeof(DateTime));
                    ds.Tables[t.DbName].Columns.Add("UserId", typeof(string));
                    ds.Tables[t.DbName].Columns.Add("ParentId", typeof(int));
                    ds.Tables[t.DbName].Columns.Add("Version", typeof(int));
                    ds.Tables[t.DbName].Columns.Add("OwnerId", typeof(int));


                    foreach (var dv in this.Meta.DataStructure)
                    {
                        if (dv.ParentMetaCode == t.MetaCode && t.MetaType == "DATACOLUMN")
                        {
                            if (dv.IsDataTypeInt)
                                ds.Tables[t.DbName].Columns.Add(dv.DbName, typeof(int));
                            else if (dv.IsDataTypeDateTime)
                                ds.Tables[t.DbName].Columns.Add(dv.DbName, typeof(DateTime));
                            else if (dv.IsDataTypeString || dv.IsDataTypeText)
                                ds.Tables[t.DbName].Columns.Add(dv.DbName, typeof(string));
                            else
                                ds.Tables[t.DbName].Columns.Add(dv.DbName, typeof(double));
                        }
                    }

                }
            }

            DataRow dr = ds.Tables[this.Meta.Application.DbName].NewRow();
            dr["Id"] = 0;
            dr["RowChangeDate"] = DateTime.Now;
            dr["UserId"] = "SYSTEM";
            dr["Version"] = 0;
            dr["OwnerId"] = 0;
            ds.Tables[this.Meta.Application.DbName].Rows.Add(dr);


            return ds;

        }

        private void InsertInformationStatus()
        {
            var model = new InformationStatus() { Id = this.ClientState.Id, ApplicationId= this.Meta.Application.Id, 
                                                  ChangedBy=this.ClientState.UserId, ChangedDate = DateTime.Now, CreatedBy = this.ClientState.UserId, 
                                                  MetaCode = this.Meta.Application.MetaCode, OwnedBy = this.ClientState.OwnerUserId, OwnerId = this.ClientState.OwnerId, 
                                                  PerformDate = DateTime.Now, Version = this.ClientState.Version };

            DataRepository.Insert(model, true);

        }

        private void UpdateInformationStatus()
        {

            var getdatecmd = Commands.Find(p => p.Key == "GETDATE" && p.DBMSType == DataRepository.GetDBMS());
            DataRepository.CreateCommand("Update sysdata_InformationStatus set ChangedDate="+getdatecmd.Command+", ChangedBy = @ChangedBy, Version = @Version WHERE ID=@ID");
            DataRepository.AddParameter("@ChangedBy", this.ClientState.UserId);
            DataRepository.AddParameter("@Version", this.ClientState.Version);
            DataRepository.AddParameter("@ID", this.ClientState.Id);
            DataRepository.ExecuteNonQuery();
        }


        #endregion

        #region ConfigureDB

        private void CreateMainTable(OperationResult o)
        {

            var table_exist = false;
            DataRepository.Open();
            table_exist = DataRepository.TableExist(this.Meta.Application.DbName);
            DataRepository.Close();
            if (table_exist)
            {
                o.AddMessage("DBCONFIG", "Main table " + this.Meta.Application.DbName + " for application: " + this.Meta.Application.Title + " is already present");
            }
            else
            {
                var intdt = DataTypes.Find(p => p.IntwentyType == "INTEGER" && p.DBMSType == DataRepository.GetDBMS());
                var datedt = DataTypes.Find(p => p.IntwentyType == "DATETIME" && p.DBMSType == DataRepository.GetDBMS());
                var stringdt = DataTypes.Find(p => p.IntwentyType == "STRING" && p.DBMSType == DataRepository.GetDBMS() && p.Length == StringLength.Short);

                var create_sql = "CREATE TABLE " + this.Meta.Application.DbName + " (Id {0} not null, RowChangeDate {1} not null, UserId {2} not null, Version {0}  not null, OwnerId {0}  not null)";
                create_sql = string.Format(create_sql,new object[] { intdt.DBMSDataType, datedt.DBMSDataType, stringdt.DBMSDataType });

                DataRepository.Open();
                DataRepository.CreateCommand(create_sql);
                DataRepository.ExecuteScalarQuery();
                DataRepository.Close();

                o.AddMessage("DBCONFIG", "Main table: " + this.Meta.Application.DbName + " for application: " + this.Meta.Application.Title + "  was created successfully");

            }
        }

        private void CreateApplicationVersioningTable(OperationResult o)
        {
            var table_exist = false;
            DataRepository.Open();
            table_exist = DataRepository.TableExist(this.Meta.Application.VersioningTableName);
            DataRepository.Close();
            if (table_exist)
            {
                //o.AddMessage("DBCONFIG", "Found versioning table (" + this.Meta.Application.VersioningTableName + ") for application:" + this.Meta.Application.Title);
            }
            else
            {

                var intdt = DataTypes.Find(p => p.IntwentyType == "INTEGER" && p.DBMSType == DataRepository.GetDBMS());
                var datedt = DataTypes.Find(p => p.IntwentyType == "DATETIME" && p.DBMSType == DataRepository.GetDBMS());
                var stringdt = DataTypes.Find(p => p.IntwentyType == "STRING" && p.DBMSType == DataRepository.GetDBMS() && p.Length == StringLength.Short);
                string create_sql = "CREATE TABLE " + this.Meta.Application.VersioningTableName + " (Id {0} not null, ParentId {0} not null, MetaCode {2} not null, MetaType {2} not null, Version {0} not null, OwnerId {0} not null, RowChangeDate {1} not null, UserId {2} not null)";
                create_sql = string.Format(create_sql, new object[] { intdt.DBMSDataType, datedt.DBMSDataType, stringdt.DBMSDataType });

                DataRepository.Open();
                DataRepository.CreateCommand(create_sql);
                DataRepository.ExecuteScalarQuery();
                DataRepository.Close();

                //o.AddMessage("DBCONFIG", "Versioning table: " + this.Meta.Application.VersioningTableName + " was created successfully");

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
            DataRepository.Open();
            colexist = DataRepository.ColumnExist(tablename, column.DbName);
            DataRepository.Close();
       
            if (colexist)
            {
               o.AddMessage("DBCONFIG", "Column: " + column.DbName + " in table: " + tablename + " is already present.");
            }
            else
            {
                var coldt = DataTypes.Find(p => p.IntwentyType == column.DataType && p.DBMSType == DataRepository.GetDBMS());
                string create_sql = "ALTER TABLE " + tablename + " ADD " + column.DbName + " " + coldt.DBMSDataType;

                DataRepository.Open();
                DataRepository.CreateCommand(create_sql);
                DataRepository.ExecuteScalarQuery();
                DataRepository.Close();

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
            DataRepository.Open();
            table_exist = DataRepository.TableExist(table.DbName);
            DataRepository.Close();
            if (table_exist)
            {
                o.AddMessage("DBCONFIG", "Table: " + table.DbName + " in application: " + this.Meta.Application.Title + " is already present.");
            }
            else
            {

                var intdt = DataTypes.Find(p => p.IntwentyType == "INTEGER" && p.DBMSType == DataRepository.GetDBMS());
                var datedt = DataTypes.Find(p => p.IntwentyType == "DATETIME" && p.DBMSType == DataRepository.GetDBMS());
                var stringdt = DataTypes.Find(p => p.IntwentyType == "STRING" && p.DBMSType == DataRepository.GetDBMS() && p.Length == StringLength.Short);

                string create_sql = "CREATE TABLE " + table.DbName + " (Id {0} not null, RowChangeDate {1} not null, UserId {2} not null, ParentId {0} not null, Version {0} not null, OwnerId {0} not null)";
                create_sql = string.Format(create_sql, new object[] { intdt.DBMSDataType, datedt.DBMSDataType, stringdt.DBMSDataType });

                DataRepository.Open();
                DataRepository.CreateCommand(create_sql);
                DataRepository.ExecuteScalarQuery();
                DataRepository.Close();

                o.AddMessage("DBCONFIG", "Subtable: " + table.DbName + " in application: " + this.Meta.Application.Title + "  was created successfully");

            }

            foreach (var t in Meta.DataStructure)
            {
                if (t.IsMetaTypeDataColumn && t.ParentMetaCode == table.MetaCode)
                    CreateDBColumn(o, t, table.DbName);
            }

        }

        private void CreateIndexes(OperationResult o)
        {
            string sql = string.Empty;

            DataRepository.Open();

            try
            {

                //Ctreate index on main application table
                sql = string.Format("CREATE UNIQUE INDEX {0}_Idx1 ON {0} (Id, Version)", this.Meta.Application.DbName);
                DataRepository.CreateCommand(sql);
                DataRepository.ExecuteScalarQuery();

                sql = string.Format("CREATE INDEX {0}_Idx2 ON {0} (RowChangeDate)", this.Meta.Application.DbName);
                DataRepository.CreateCommand(sql);
                DataRepository.ExecuteScalarQuery();

                //Create index on versioning table
                sql = string.Format("CREATE UNIQUE INDEX {0}_Idx1 ON {0} (Id, Version, MetaCode, MetaType)", this.Meta.Application.VersioningTableName);
                DataRepository.CreateCommand(sql);
                DataRepository.ExecuteScalarQuery();

                sql = string.Format("CREATE INDEX {0}_Idx2 ON {0} (RowChangeDate)", this.Meta.Application.VersioningTableName);
                DataRepository.CreateCommand(sql);
                DataRepository.ExecuteScalarQuery();




                //Create index on subtables
                foreach (var t in this.Meta.DataStructure)
                {
                    if (t.IsMetaTypeDataTable)
                    {
                        sql = string.Format("CREATE UNIQUE INDEX {0}_Idx1 ON {0} (Id, Version)", t.DbName);
                        DataRepository.CreateCommand(sql);
                        DataRepository.ExecuteScalarQuery();

                        sql = string.Format("CREATE INDEX {0}_Idx2 ON {0} (RowChangeDate)", t.DbName);
                        DataRepository.CreateCommand(sql);
                        DataRepository.ExecuteScalarQuery();


                        sql = string.Format("CREATE INDEX {0}_Idx3 ON {0} (ParentId)", t.DbName);
                        DataRepository.CreateCommand(sql);
                        DataRepository.ExecuteScalarQuery();

                    }
                }

                o.AddMessage("DBCONFIG", "Database Indexes was created successfully for application " + this.Meta.Application.Title);

            }
            catch(Exception ex)
            {
                throw ex;
            }

            DataRepository.Close();




        }



        #endregion

        #region Test Data

        private object GetSemanticValue(DatabaseModelItem t, Random rnd, int gencount)
        {
            var val = GetCustomSemanticValue(t, rnd, gencount);
            if (val == null)
                val = GetStandardSemanticValue(t, rnd, gencount);

            return val;
        }

        private object GetCustomSemanticValue(DatabaseModelItem t, Random rnd, int gencount)
        {
            if (this.Meta.Application.MetaCode == "VENDOR" && t.MetaCode == "VENDORNAME" && gencount == 1)
                return "Anderssons AB";
            if (this.Meta.Application.MetaCode == "VENDOR" && t.MetaCode == "VENDORNAME" && gencount == 2)
                return "Håkanssons AB";
            if (this.Meta.Application.MetaCode == "VENDOR" && t.MetaCode == "VENDORNAME" && gencount == 3)
                return "Nilssons AB";
            if (this.Meta.Application.MetaCode == "VENDOR" && t.MetaCode == "VENDORNAME" && gencount == 4)
                return "Svenssons AB";
            if (this.Meta.Application.MetaCode == "VENDOR" && t.MetaCode == "VENDORNAME" && gencount == 5)
                return "Filipssons AB";
            if (this.Meta.Application.MetaCode == "VENDOR" && t.MetaCode == "VENDORNAME" && gencount == 6)
                return "Jägmarks AB";
            if (this.Meta.Application.MetaCode == "VENDOR" && t.MetaCode == "VENDORNAME" && gencount == 7)
                return "Björklunds AB";
            if (this.Meta.Application.MetaCode == "VENDOR" && t.MetaCode == "VENDORNAME" && gencount == 8)
                return "Stensson AB";
            if (this.Meta.Application.MetaCode == "VENDOR" && t.MetaCode == "VENDORNAME" && gencount == 9)
                return "Mega varor AB";
            if (this.Meta.Application.MetaCode == "VENDOR" && t.MetaCode == "VENDORNAME" && gencount == 10)
                return "Superduper varor AB";

            if (this.Meta.Application.MetaCode == "LOCATION" && t.MetaCode == "LOCATIONNAME" && gencount == 1)
                return "Centrallager";
            if (this.Meta.Application.MetaCode == "LOCATION" && t.MetaCode == "LOCATIONNAME" && gencount == 2)
                return "Lager 2 (Alingsås)";
            if (this.Meta.Application.MetaCode == "LOCATION" && t.MetaCode == "LOCATIONNAME" && gencount == 3)
                return "Lager 3 (Alingsås)";

            return null;

        }

        private object GetStandardSemanticValue(DatabaseModelItem t, Random rnd, int gencount)
        {

            if (t.IsDataType1Decimal)
                return 200.1 + gencount;
            if (t.IsDataType2Decimal)
                return 400.55 + gencount;
            if (t.IsDataType3Decimal)
                return 70.855 + gencount;
            if (t.IsDataTypeText)
                return "This is the first sentence in a sample text generated automaticly for the datatype TEXT. This is the second sentence in a sample text generated automaticly. This is the third sentence in a sample text generated automaticly.";
            if (t.IsDataTypeString && t.MetaCode.Contains("ID"))
                return this.Meta.Application.MetaCode.Substring(0, 3) + "-" + rnd.Next();
            if (t.IsDataTypeString && t.MetaCode.Contains("NAME"))
                return "Test " + this.Meta.Application.MetaCode + " name";
            if (t.IsDataTypeString)
                return "A test string";
            if (t.IsDataTypeBool)
                return true;
            if (t.IsDataTypeDateTime)
                return DateTime.Now;
            if (t.IsDataTypeInt)
                return 20 + gencount;

            return null;

        }

        #endregion





    }
}
    
