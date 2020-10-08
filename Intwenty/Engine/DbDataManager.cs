using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Intwenty.Data.Dto;
using Intwenty.Data.Entity;
using Intwenty.Data.Helpers;
using Intwenty.DataClient;
using Intwenty.DataClient.Model;
using Intwenty.Engine.Custom;
using Intwenty.Model;




namespace Intwenty.Engine
{
   

   

    public class DbDataManager : IDataManager
    {
        protected IIntwentyModelService ModelRepository { get; set; }

        protected IDataClient Client { get; set; }

        protected ApplicationModel Model { get; set; }

        protected IntwentySettings Settings { get; set; }

        protected DateTime ApplicationSaveTimeStamp { get; set; }

        protected List<TypeMapItem> DataTypes { get; set; }

        protected List<CommandMapItem> Commands { get; set; }


        protected DbDataManager(ApplicationModel model, IIntwentyModelService modelservice, IntwentySettings settings, IDataClient client)
        {
            Settings = settings;
            Model = model;
            ModelRepository = modelservice;
            Client = client;
            ApplicationSaveTimeStamp = DateTime.Now;
            DataTypes = Client.GetDbTypeMap();
            Commands = Client.GetDbCommandMap();
        }

        public static DbDataManager GetDataManager(ApplicationModel model, IIntwentyModelService modelservice, IntwentySettings settings, IDataClient client)
        {
            

            if (model != null && model.Application.MetaCode == "XXXXX")
            {
                return new  CustomDataManagerExample(model, modelservice, settings, client);
            }
            else
            {
                var t = new DbDataManager(model,modelservice, settings, client);
                return t;
            }
        }


     




        #region Implementation

        public OperationResult ConfigureDatabase()
        {
            var res = new OperationResult(true, string.Format("Database configured for application {0}", this.Model.Application.Title));

            try
            {
                Client.Open();
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
            finally
            {
                Client.Close();
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

                Client.Open();

                var columns = new List<IIntwentyResultColumn>();
                columns.Add(new IntwentyDataColumn() { Name = "Id", DataType = DatabaseModelItem.DataTypeInt });
                columns.Add(new IntwentyDataColumn() { Name = "Version", DataType = DatabaseModelItem.DataTypeInt });
                columns.Add(new IntwentyDataColumn() { Name = "ApplicationId", DataType = DatabaseModelItem.DataTypeInt });
                columns.Add(new IntwentyDataColumn() { Name = "ChangedDate", DataType = DatabaseModelItem.DataTypeDateTime });
                columns.Add(new IntwentyDataColumn() { Name = "CreatedBy", DataType = DatabaseModelItem.DataTypeString });
                columns.Add(new IntwentyDataColumn() { Name = "ChangedBy", DataType = DatabaseModelItem.DataTypeString });
                columns.Add(new IntwentyDataColumn() { Name = "OwnedBy", DataType = DatabaseModelItem.DataTypeString });


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
              
                var appjson = Client.GetJSONObject(sql_stmt.ToString(), resultcolumns: columns.ToArray());
               
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
                        columns = new List<IIntwentyResultColumn>();
                        columns.Add(new IntwentyDataColumn() { Name = "Id", DataType = DatabaseModelItem.DataTypeInt });
                        columns.Add(new IntwentyDataColumn() { Name = "Version", DataType = DatabaseModelItem.DataTypeInt });
                        columns.Add(new IntwentyDataColumn() { Name = "ApplicationId", DataType = DatabaseModelItem.DataTypeInt });
                        columns.Add(new IntwentyDataColumn() { Name = "ChangedDate", DataType = DatabaseModelItem.DataTypeDateTime });
                        columns.Add(new IntwentyDataColumn() { Name = "CreatedBy", DataType = DatabaseModelItem.DataTypeString });
                        columns.Add(new IntwentyDataColumn() { Name = "ChangedBy", DataType = DatabaseModelItem.DataTypeString });
                        columns.Add(new IntwentyDataColumn() { Name = "OwnedBy", DataType = DatabaseModelItem.DataTypeString });
                        columns.Add(new IntwentyDataColumn() { Name = "ParentId", DataType = DatabaseModelItem.DataTypeInt });

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

                        var tablearray = Client.GetJSONArray(sql_stmt.ToString(), resultcolumns: columns.ToArray());


                        jsonresult.Append(", \""+t.DbName+"\": " + tablearray.ToString());

                    }
                }

                jsonresult.Append("}");

                result.Data = jsonresult.ToString();

            }
            catch (Exception ex)
            {
                Client.Close();
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", string.Format("Get latest version for application {0} failed", this.Model.Application.Title));
                result.AddMessage("SYSTEMERROR", ex.Message);
                result.Data = "{}";

            }
            finally
            {
                Client.Close();
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
                state.Data.InferModel(Model);

                Client.Open();

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
            finally
            {
                Client.Close();
            }

            return result;

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

                Client.Open();


                Client.RunCommand("DELETE FROM " + this.Model.Application.DbName + " WHERE Id=@Id", parameters: new IntwentySqlParameter[] { new IntwentySqlParameter() { Name = "@Id", Value = state.Id } });
                Client.RunCommand("DELETE FROM " + this.Model.Application.VersioningTableName + " WHERE Id=@Id", parameters: new IntwentySqlParameter[] { new IntwentySqlParameter() { Name = "@Id", Value = state.Id } });
 
                foreach (var table in Model.DataStructure)
                {
                    if (table.IsMetaTypeDataTable)
                    {
                        Client.RunCommand("DELETE FROM " + table.DbName + " WHERE ParentId=@Id", parameters: new IntwentySqlParameter[] { new IntwentySqlParameter() { Name = "@Id", Value = state.Id } });
                        Client.RunCommand("DELETE FROM sysdata_SystemId WHERE ParentId=@Id", parameters: new IntwentySqlParameter[] { new IntwentySqlParameter() { Name = "@Id", Value = state.Id } });

                    }
                }

               

                Client.RunCommand("DELETE FROM sysdata_SystemId WHERE Id=@Id", parameters: new IntwentySqlParameter[] { new IntwentySqlParameter() { Name = "@Id", Value = state.Id } });
                Client.RunCommand("DELETE FROM sysdata_InformationStatus WHERE Id=@Id", parameters: new IntwentySqlParameter[] { new IntwentySqlParameter() { Name = "@Id", Value = state.Id } });


            }
            catch (Exception ex)
            {
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", string.Format("Delete application {0} failed", this.Model.Application.Title));
                result.AddMessage("SYSTEMERROR", ex.Message);
            }
            finally
            {
                Client.Close();
            }

            return result;
        }

        public virtual OperationResult DeleteById(int id, string dbname)
        {

            if (id < 1)
                return new OperationResult(false, "No id found when performing DeleteById(id, dbname).", 0, 0);
            if (string.IsNullOrEmpty(dbname))
                return new OperationResult(false, "No dbname found when performing DeleteById(id, dbname).", 0, 0);

            OperationResult result=null;

            try
            {

                Client.Open();

                if (dbname.ToLower() == this.Model.Application.DbName.ToLower())
                {
                    result = new OperationResult(true, string.Format("Deleted application {0}", this.Model.Application.Title), id);

                    Client.RunCommand("DELETE FROM " + this.Model.Application.DbName + " WHERE Id=@Id", parameters: new IntwentySqlParameter[] { new IntwentySqlParameter() { Name="@Id", Value=id } });
                    Client.RunCommand("DELETE FROM " + this.Model.Application.VersioningTableName + " WHERE Id=@Id", parameters: new IntwentySqlParameter[] { new IntwentySqlParameter() { Name = "@Id", Value = id } });

                    foreach (var table in Model.DataStructure)
                    {
                        if (table.IsMetaTypeDataTable)
                        {
                            Client.RunCommand("DELETE FROM " + table.DbName + " WHERE ParentId=@Id", parameters: new IntwentySqlParameter[] { new IntwentySqlParameter() { Name = "@Id", Value = id } });
                            Client.RunCommand("DELETE FROM sysdata_SystemId WHERE ParentId=@Id", parameters: new IntwentySqlParameter[] { new IntwentySqlParameter() { Name = "@Id", Value = id } });
                        }
                    }

                    Client.RunCommand("DELETE FROM sysdata_SystemId WHERE Id=@Id", parameters: new IntwentySqlParameter[] { new IntwentySqlParameter() { Name = "@Id", Value = id } });
                    Client.RunCommand("DELETE FROM sysdata_InformationStatus WHERE Id=@Id", parameters: new IntwentySqlParameter[] { new IntwentySqlParameter() { Name = "@Id", Value = id } });


                }
                else
                {
                    foreach (var table in Model.DataStructure)
                    {
                        if (table.IsMetaTypeDataTable && table.DbName.ToLower() == dbname.ToLower())
                        {
                            result = new OperationResult(true, string.Format("Deleted sub table row {0}", table.DbName), id);

                            Client.RunCommand("DELETE FROM " + table.DbName + " WHERE Id=@Id", parameters: new IntwentySqlParameter[] { new IntwentySqlParameter() { Name = "@Id", Value = id } });
                            Client.RunCommand("DELETE FROM sysdata_SystemId WHERE Id=@Id", parameters: new IntwentySqlParameter[] { new IntwentySqlParameter() { Name = "@Id", Value = id } });

                        }
                    }
                }


                if (result == null)
                    throw new InvalidOperationException("Found nothing to delete");
            }
            catch (Exception ex)
            {
                result = new OperationResult();
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", string.Format("DeleteById(id, dbaname) in application {0} failed", this.Model.Application.Title));
                result.AddMessage("SYSTEMERROR", ex.Message);
            }
            finally
            {
                Client.Close();
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
            //Postgresql must alwaus use columns in order to set correct case in jsonnames
            if (Settings.DefaultConnectionDBMS == DBMS.PostgreSQL)
                return GetListInternal(new ListRetrivalArgs() { ApplicationId = this.Model.Application.Id, OwnerUserId = owneruserid }, true, true, false);
            else
                return GetListInternal(new ListRetrivalArgs() { ApplicationId = this.Model.Application.Id, OwnerUserId = owneruserid }, true, false, false);


        }

        public virtual OperationResult GetList()
        {
    
            //Postgresql must always use columns in order to set correct case in jsonnames
            if (Settings.DefaultConnectionDBMS == DBMS.PostgreSQL)
                return GetListInternal(new ListRetrivalArgs() { ApplicationId = this.Model.Application.Id }, false, true, false);
            else
                return GetListInternal(new ListRetrivalArgs() { ApplicationId = this.Model.Application.Id }, false, false, false);
        }
        


        private OperationResult GetListInternal(ListRetrivalArgs args, bool getbyowneruser, bool selectcolumns, bool usepaging)
        {
            if (args == null)
                return new OperationResult(false, "Can't get list without ListRetrivalArgs",0,0);

            var result = new OperationResult(true, string.Format("Fetched list for application {0}", this.Model.Application.Title),0,0);

            Client.Open();

            if (args.MaxCount == 0 && usepaging)
            {

                var max = Client.GetScalarValue("select count(*) FROM sysdata_InformationStatus where ApplicationId = " + this.Model.Application.Id);
                if (max == DBNull.Value)
                    args.MaxCount = 0;
                else
                    args.MaxCount = Convert.ToInt32(max);

            }

            result.RetriveListArgs = new ListRetrivalArgs();
            result.RetriveListArgs = args;


            try
            {
                var columns = new List<IIntwentyResultColumn>();
                if (selectcolumns)
                {
                    columns.Add(new IntwentyDataColumn() { Name = "Id", DataType = DatabaseModelItem.DataTypeInt });
                    columns.Add(new IntwentyDataColumn() { Name = "Version", DataType = DatabaseModelItem.DataTypeInt });
                    columns.Add(new IntwentyDataColumn() { Name = "ApplicationId", DataType = DatabaseModelItem.DataTypeInt });
                    columns.Add(new IntwentyDataColumn() { Name = "ChangedDate", DataType = DatabaseModelItem.DataTypeDateTime });
                    columns.Add(new IntwentyDataColumn() { Name = "CreatedBy", DataType = DatabaseModelItem.DataTypeString });
                    columns.Add(new IntwentyDataColumn() { Name = "ChangedBy", DataType = DatabaseModelItem.DataTypeString });
                    columns.Add(new IntwentyDataColumn() { Name = "OwnedBy", DataType = DatabaseModelItem.DataTypeString });
                }

                var sql_list_stmt = new StringBuilder();
                sql_list_stmt.Append("SELECT t1.* ");

                if (this.Model.UIStructure.Exists(p => p.IsMetaTypeListViewColumn && p.IsDataColumnConnected) && selectcolumns)
                {

                    foreach (var t in this.Model.UIStructure)
                    {
                        if (t.IsMetaTypeListViewColumn && t.IsDataColumnConnected && t.DataColumnInfo.IsRoot)
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

                var parameters = new List<IIntwentySqlParameter>();
                parameters.Add(new IntwentySqlParameter() { Name= "@ApplicationId", Value= this.Model.Application.Id });
                string json = "[]";

                if (getbyowneruser)
                    parameters.Add(new IntwentySqlParameter() { Name = "@OwnedBy", Value = args.OwnerUserId });

             

                if (selectcolumns && usepaging)
                    json = Client.GetJSONArray(sql_list_stmt.ToString(),result.RetriveListArgs.CurrentRowNum, (result.RetriveListArgs.CurrentRowNum + result.RetriveListArgs.BatchSize), false, parameters.ToArray(), columns.ToArray()).ToString();
                if (!selectcolumns && usepaging)
                    json = Client.GetJSONArray(sql_list_stmt.ToString(),result.RetriveListArgs.CurrentRowNum, (result.RetriveListArgs.CurrentRowNum + result.RetriveListArgs.BatchSize), false, parameters.ToArray()).ToString();
                if (!selectcolumns && !usepaging)
                    json = Client.GetJSONArray(sql_list_stmt.ToString(), parameters: parameters.ToArray()).ToString();
                if (selectcolumns && !usepaging)
                    json = Client.GetJSONArray(sql_list_stmt.ToString(), parameters: parameters.ToArray(), resultcolumns: columns.ToArray()).ToString();


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
            finally
            {
                Client.Close();
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
                var domains = new List<IResultSet>();

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

                Client.Open();

                foreach (var d in valuedomains)
                {
                    var parameters = new IntwentySqlParameter[] { new IntwentySqlParameter() { Name = "@P1", Value = d } };
                    var domainset = Client.GetResultSet("SELECT Id, DomainName, Code, Value FROM sysmodel_ValueDomainItem WHERE DomainName = @P1", parameters: parameters.ToArray());
                    domainset.Name = d;
                    domains.Add(domainset);
                }



                sb.Append("{");


                foreach (IResultSet set in domains)
                {

                    if (domainindex == 0)
                        sb.Append("\"" + "VALUEDOMAIN_" + set.Name + "\":[");
                    else
                        sb.Append(",\"" + "VALUEDOMAIN_" + set.Name + "\":[");

                    domainindex += 1;
                    rowindex = 0;
                   

                    foreach (var row in set.Rows)
                    {
                        if (rowindex == 0)
                            sb.Append("{");
                        else
                            sb.Append(",{");

                        sb.Append(DBHelpers.GetJSONValue("Id", row.GetAsInt("Id").Value));
                        sb.Append("," + DBHelpers.GetJSONValue("DomainName", row.GetAsString("DomainName")));
                        sb.Append("," + DBHelpers.GetJSONValue("Code", row.GetAsString("Code")));
                        sb.Append("," + DBHelpers.GetJSONValue("Value", row.GetAsString("Value")));

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
            finally
            {
                Client.Close();
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
                var domains = new List<IResultSet>();

                Client.Open();

                var names = Client.GetResultSet("SELECT distinct DomainName FROM sysmodel_ValueDomainItem");
                foreach (var d in names.Rows)
                {
                    var domainname = d.GetAsString("DomainName");
                    var parameters = new IntwentySqlParameter[] { new IntwentySqlParameter() { Name = "@P1", Value = domainname } };
                    var domainset =  Client.GetResultSet("SELECT Id, DomainName, Code, Value FROM sysmodel_ValueDomainItem WHERE DomainName = @P1", parameters: parameters.ToArray());
                    domainset.Name = domainname;
                    domains.Add(domainset);
                }

                sb.Append("{");

                foreach (IResultSet set in domains)
                {

                    if (domainindex == 0)
                        sb.Append("\"" + "VALUEDOMAIN_" + set.Name + "\":[");
                    else
                        sb.Append(",\"" + "VALUEDOMAIN_" + set.Name + "\":[");

                    domainindex += 1;
                    rowindex = 0;


                    foreach (var row in set.Rows)
                    {
                        if (rowindex == 0)
                            sb.Append("{");
                        else
                            sb.Append(",{");

                        sb.Append(DBHelpers.GetJSONValue("Id", row.GetAsInt("Id").Value));
                        sb.Append("," + DBHelpers.GetJSONValue("DomainName", row.GetAsString("DomainName")));
                        sb.Append("," + DBHelpers.GetJSONValue("Code", row.GetAsString("Code")));
                        sb.Append("," + DBHelpers.GetJSONValue("Value", row.GetAsString("Value")));

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
            finally
            {
                Client.Close();
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


                var columns = new List<IIntwentyResultColumn>();
                foreach (var viewcol in viewinfo)
                {
                    if ((viewcol.IsMetaTypeDataViewColumn || viewcol.IsMetaTypeDataViewKeyColumn) && viewcol.ParentMetaCode == dv.MetaCode)
                    {
                        columns.Add(new IntwentyDataColumn() { Name = viewcol.SQLQueryFieldName, DataType = viewcol.DataType});
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

                Client.Open();

                result.Data = Client.GetJSONArray(sql, result.RetriveListArgs.CurrentRowNum, (result.RetriveListArgs.CurrentRowNum + result.RetriveListArgs.BatchSize), resultcolumns:columns.ToArray());


            }
            catch (Exception ex)
            {
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", "Fetch dataview failed");
                result.AddMessage("SYSTEMERROR", ex.Message);
                result.Data = "{}";
            }
            finally
            {
                Client.Close();
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

                var columns = new List<IIntwentyResultColumn>();
                foreach (var viewcol in viewinfo)
                {
                    if ((viewcol.IsMetaTypeDataViewColumn || viewcol.IsMetaTypeDataViewKeyColumn) && viewcol.ParentMetaCode == args.DataViewMetaCode)
                    {
                        columns.Add(new IntwentyDataColumn() { Name = viewcol.SQLQueryFieldName, DataType = viewcol.DataType });
                    }
                }

                Client.Open();

                result.Data = Client.GetJSONObject(sql, parameters: new IIntwentySqlParameter[] { new IntwentySqlParameter("@P1", args.FilterValue) }, resultcolumns: columns.ToArray());


            }
            catch (Exception ex)
            {
                result.Messages.Clear();
                result.IsSuccess = false;
                result.AddMessage("USERERROR", "Fetch dataview failed");
                result.AddMessage("SYSTEMERROR", ex.Message);
                result.Data = "{}";
            }
            finally
            {
                Client.Close();
            }

            return result;

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

        private void SetParameters(List<ApplicationValue> paramlist, List<IIntwentySqlParameter> parameters)
        {
            foreach (var p in paramlist)
            {
                if (p.Model.IsDataTypeText || p.Model.IsDataTypeString)
                {
                    var val = p.GetAsString();
                    if (!string.IsNullOrEmpty(val))
                        parameters.Add(new IntwentySqlParameter("@" + p.DbName, val));
                    else
                        parameters.Add(new IntwentySqlParameter("@" + p.DbName, DBNull.Value));
                }
                else if (p.Model.IsDataTypeInt)
                {
                    var val = p.GetAsInt();
                    if (val.HasValue)
                        parameters.Add(new IntwentySqlParameter("@" + p.DbName, val.Value));
                    else
                        parameters.Add(new IntwentySqlParameter("@" + p.DbName, DBNull.Value));
                }
                else if (p.Model.IsDataTypeBool)
                {
                    var val = p.GetAsBool();
                    if (val.HasValue)
                        parameters.Add(new IntwentySqlParameter("@" + p.DbName, val.Value));
                    else
                        parameters.Add(new IntwentySqlParameter("@" + p.DbName, DBNull.Value));
                }
                else if (p.Model.IsDataTypeDateTime)
                {
                    var val = p.GetAsDateTime();
                    if (val.HasValue)
                        parameters.Add(new IntwentySqlParameter("@" + p.DbName, val.Value));
                    else
                        parameters.Add(new IntwentySqlParameter("@" + p.DbName, DBNull.Value));
                }
                else if (p.Model.IsDataType1Decimal || p.Model.IsDataType2Decimal || p.Model.IsDataType3Decimal)
                {
                    var val = p.GetAsDecimal();
                    if (val.HasValue)
                        parameters.Add(new IntwentySqlParameter("@" + p.DbName, val.Value));
                    else
                        parameters.Add(new IntwentySqlParameter("@" + p.DbName, DBNull.Value));
                }
            }

        }


      

        #endregion

        #region Save

        private int GetNewSystemID(string metatype, string metacode, ClientStateInfo state)
        {
            var model = new SystemID() { ApplicationId = this.Model.Application.Id, GeneratedDate = DateTime.Now, MetaCode = metacode, MetaType = metatype, Properties = state.Properties, ParentId=0 };
            if (metatype == DatabaseModelItem.MetaTypeDataTable)
            {
                model.ParentId = state.Id;
            }
              
            Client.InsertEntity(model);

            return model.Id;
        }

        private DateTime GetApplicationTimeStamp()
        {
            return this.ApplicationSaveTimeStamp;

            /*
            if (Settings.DefaultConnectionDBMS == DBMS.PostgreSQL)
                return this.ApplicationSaveTimeStamp.ToString("yyyy-MM-dd HH:mm:ss");
            else
                return this.ApplicationSaveTimeStamp.ToString("yyyy-MM-ddTHH:mm:ss.fff");
                */

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


            foreach (var t in state.Data.Values)
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
            sql_insert.Append(sql_insert_value);

            var parameters = new List<IIntwentySqlParameter>();
            parameters.Add(new IntwentySqlParameter("@Id", state.Id));
            parameters.Add(new IntwentySqlParameter("@Version", state.Version));
            parameters.Add(new IntwentySqlParameter("@ApplicationId", this.Model.Application.Id));
            parameters.Add(new IntwentySqlParameter("@CreatedBy", state.UserId));
            parameters.Add(new IntwentySqlParameter("@ChangedBy", state.UserId));
            parameters.Add(new IntwentySqlParameter("@OwnedBy", state.OwnerUserId));
            parameters.Add(new IntwentySqlParameter("@ChangedDate", GetApplicationTimeStamp()));
            SetParameters(paramlist, parameters);

            Client.RunCommand(sql_insert.ToString(), parameters:parameters.ToArray());


        }

        private void HandleSubTables(ClientStateInfo state)
        {
            foreach (var table in state.Data.SubTables)
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
            sql_update.Append(" set ChangedDate=@ChangedDate");
            sql_update.Append(",ChangedBy=@ChangedBy");


            foreach (var t in state.Data.Values)
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

            var parameters = new List<IIntwentySqlParameter>();
            parameters.Add(new IntwentySqlParameter("@ID", state.Id));
            parameters.Add(new IntwentySqlParameter("@Version", state.Version));
            parameters.Add(new IntwentySqlParameter("@ChangedBy", state.UserId));
            parameters.Add(new IntwentySqlParameter("@ChangedDate", GetApplicationTimeStamp()));
            SetParameters(paramlist, parameters);

            Client.RunCommand(sql_update.ToString(), parameters: parameters.ToArray());

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

            var parameters = new List<IIntwentySqlParameter>();
            parameters.Add(new IntwentySqlParameter("@Id", rowid));
            parameters.Add(new IntwentySqlParameter("@Version", state.Version));
            parameters.Add(new IntwentySqlParameter("@ApplicationId", this.Model.Application.Id));
            parameters.Add(new IntwentySqlParameter("@CreatedBy", state.UserId));
            parameters.Add(new IntwentySqlParameter("@ChangedBy", state.UserId));
            parameters.Add(new IntwentySqlParameter("@OwnedBy", state.OwnerUserId));
            parameters.Add(new IntwentySqlParameter("@ChangedDate", GetApplicationTimeStamp()));
            parameters.Add(new IntwentySqlParameter("@ParentId", state.Id));
            SetParameters(paramlist, parameters);

            Client.RunCommand(sql_insert.ToString(), parameters: parameters.ToArray());
           

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

            var parameters = new List<IIntwentySqlParameter>();
            parameters.Add(new IntwentySqlParameter("@ID", rowid));
            parameters.Add(new IntwentySqlParameter("@Version", state.Version));
            parameters.Add(new IntwentySqlParameter("@ChangedBy", state.UserId));
            SetParameters(paramlist, parameters);

            Client.RunCommand(sql_update.ToString(), parameters: parameters.ToArray());

        }

        private int CreateVersionRecord(ClientStateInfo state)
        {
            int newversion = 0;
            string sql = String.Empty;
            sql = "select max(version) from " + this.Model.Application.VersioningTableName;
            sql += " where ID=" + Convert.ToString(state.Id);
            sql += " and MetaCode='" + this.Model.Application.MetaCode + "' and MetaType='APPLICATION'";

            object obj = Client.GetScalarValue(sql);
            if (obj != null && obj != DBNull.Value)
            {
                newversion = Convert.ToInt32(obj);
                newversion += 1;
            }
            else
            {
                newversion = 1;
            }

            var getdatecmd = Commands.Find(p => p.Key == "GETDATE" && p.DbEngine == Client.Database);

            //DefaultVersioningTableColumns
            sql = "insert into " + this.Model.Application.VersioningTableName;
            sql += " (ID, Version, ApplicationId, MetaCode, MetaType, ChangedDate, ParentId)";
            sql += " VALUES (@P1, @P2, @P3, @P4, @P5, {0}, @P6)";
            sql = string.Format(sql, getdatecmd.Command);

            var parameters = new List<IIntwentySqlParameter>();
            parameters.Add(new IntwentySqlParameter("@P1",state.Id));
            parameters.Add(new IntwentySqlParameter("@P2", newversion));
            parameters.Add(new IntwentySqlParameter("@P3", this.Model.Application.Id));
            parameters.Add(new IntwentySqlParameter("@P4", this.Model.Application.MetaCode));
            parameters.Add(new IntwentySqlParameter("@P5", "APPLICATION"));
            parameters.Add(new IntwentySqlParameter("@P6", 0));

            Client.RunCommand(sql, parameters: parameters.ToArray());

         
            return newversion;
        }

      

        private OperationResult FetchLatestIdByOwnerUser(ClientStateInfo state)
        {
            var result = new OperationResult(true, string.Format("Fetched latest id for application {0} for Owner {1}", this.Model.Application.Title, state.OwnerUserId), 0, 0);

            try
            {
                var parameters = new List<IIntwentySqlParameter>();
                parameters.Add(new IntwentySqlParameter() { Name= "@ApplicationId", Value= state.ApplicationId });
                parameters.Add(new IntwentySqlParameter() { Name = "@OwnedBy", Value = state.OwnerUserId });

                Client.Open();
                var maxid = Client.GetScalarValue("SELECT max(id) from sysdata_InformationStatus where ApplicationId=@ApplicationId and OwnedBy=@OwnedBy", parameters: parameters.ToArray());
                if (maxid != null && maxid != DBNull.Value)
                {
      
                    var resultset  = Client.GetResultSet("SELECT Id,Version from sysdata_InformationStatus where Id = @Id", parameters: new IntwentySqlParameter[] { new IntwentySqlParameter() { Name = "@Id", Value = maxid} });
                    if (resultset.Rows.Count == 0)
                    {
                        var fail = new OperationResult(false, string.Format("Latest id for application {0} for Owner {1} could not be found", this.Model.Application.Title, state.OwnerUserId), 0, 0);
                        return fail;
                    }

                    result.Id = resultset.FirstRowGetAsInt("Id").Value;
                    result.Version = resultset.FirstRowGetAsInt("Version").Value;
                }
                Client.Close();

            }
            catch (Exception ex)
            {
                Client.Close();
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
                                                  PerformDate = DateTime.Now, Version = state.Version, EndDate = DateTime.Now, StartDate=DateTime.Now };

            Client.InsertEntity(model);

        }

        private void UpdateInformationStatus(ClientStateInfo state)
        {

            var getdatecmd = Commands.Find(p => p.Key == "GETDATE" && p.DbEngine == Client.Database);

            var parameters = new List<IIntwentySqlParameter>();
            parameters.Add(new IntwentySqlParameter() { Name = "@ChangedBy", Value = state.UserId });
            parameters.Add(new IntwentySqlParameter() { Name = "@Version", Value = state.Version });
            parameters.Add(new IntwentySqlParameter() { Name = "@ID", Value = state.Id });

            Client.RunCommand("Update sysdata_InformationStatus set ChangedDate="+getdatecmd.Command+", ChangedBy = @ChangedBy, Version = @Version WHERE ID=@ID", parameters: parameters.ToArray());

        }


        #endregion

        #region ConfigureDB

        private void CreateMainTable(OperationResult o)
        {

            var table_exist = false;
            table_exist = Client.TableExists(this.Model.Application.DbName);
            if (table_exist)
            {
                o.AddMessage("DBCONFIG", "Main table " + this.Model.Application.DbName + " for application: " + this.Model.Application.Title + " is already present");
            }
            else
            {
    
                string create_sql = GetCreateTableStmt(ModelRepository.GetDefaultMainTableColumns(), this.Model.Application.DbName);
                Client.RunCommand(create_sql);
                o.AddMessage("DBCONFIG", "Main table: " + this.Model.Application.DbName + " for application: " + this.Model.Application.Title + "  was created successfully");

            }
        }

        private void CreateApplicationVersioningTable(OperationResult o)
        {
            var table_exist = false;
            table_exist = Client.TableExists(this.Model.Application.VersioningTableName);
            if (table_exist)
            {
                //o.AddMessage("DBCONFIG", "Found versioning table (" + this.Model.Application.VersioningTableName + ") for application:" + this.Model.Application.Title);
            }
            else
            {

                string create_sql = GetCreateTableStmt(ModelRepository.GetDefaultVersioningTableColumns(), this.Model.Application.VersioningTableName);
                Client.RunCommand(create_sql);

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
            colexist = Client.ColumnExists(tablename, column.DbName);

            if (colexist)
            {
               o.AddMessage("DBCONFIG", "Column: " + column.DbName + " in table: " + tablename + " is already present.");
            }
            else
            {
                var coldt = DataTypes.Find(p => p.IntwentyType == column.DataType && p.DbEngine == Client.Database);
                string create_sql = "ALTER TABLE " + tablename + " ADD " + column.DbName + " " + coldt.DBMSDataType;
                Client.RunCommand(create_sql);
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
            table_exist = Client.TableExists(table.DbName);
            if (table_exist)
            {
                o.AddMessage("DBCONFIG", "Table: " + table.DbName + " in application: " + this.Model.Application.Title + " is already present.");
            }
            else
            {

                string create_sql = GetCreateTableStmt(ModelRepository.GetDefaultSubTableColumns(), table.DbName);
                Client.RunCommand(create_sql);
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

           
                try
                {
                    //Ctreate index on main application table
                    sql = string.Format("CREATE UNIQUE INDEX {0}_Idx1 ON {0} (Id, Version)", this.Model.Application.DbName);
                    Client.RunCommand(sql);
                }
                catch { }

                try
                {
                    //Create index on versioning table
                    sql = string.Format("CREATE UNIQUE INDEX {0}_Idx1 ON {0} (Id, Version, MetaCode, MetaType)", this.Model.Application.VersioningTableName);
                    Client.RunCommand(sql);
                }
                catch { }

            //Create index on subtables
            foreach (var t in this.Model.DataStructure)
            {
                if (t.IsMetaTypeDataTable)
                {
                    try
                    {
                        sql = string.Format("CREATE UNIQUE INDEX {0}_Idx1 ON {0} (Id, Version)", t.DbName);
                        Client.RunCommand(sql);
                    }
                    catch { }

                    try
                    {
                        sql = string.Format("CREATE INDEX {0}_Idx3 ON {0} (ParentId)", t.DbName);
                        Client.RunCommand(sql);
                    }
                    catch { }

                }
            }

           o.AddMessage("DBCONFIG", "Database Indexes was created successfully for application " + this.Model.Application.Title);

          

        }

        private string GetCreateTableStmt(List<IntwentyDataColumn> columns, string tablename)
        {
            var res = string.Format("CREATE TABLE {0}", tablename) + " (";
            var sep = "";
            foreach (var c in columns)
            {
                TypeMapItem dt;
                if (c.DataType == DatabaseModelItem.DataTypeString)
                   dt = DataTypes.Find(p => p.IntwentyType == c.DataType && p.DbEngine == Client.Database && p.Length == StringLength.Short);
                else if (c.DataType == DatabaseModelItem.DataTypeText)
                    dt = DataTypes.Find(p => p.IntwentyType == c.DataType && p.DbEngine == Client.Database && p.Length == StringLength.Long);
                else
                    dt = DataTypes.Find(p => p.IntwentyType == c.DataType && p.DbEngine == Client.Database);

                res += sep + string.Format("{0} {1} not null", c.Name, dt.DBMSDataType);
                sep = ", ";
            }

            res += ")";

            return res;

        }

      


        #endregion






    }
}
    
