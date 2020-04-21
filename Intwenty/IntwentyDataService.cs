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

namespace Intwenty
{
    public interface IIntwentyDataService
    {
        List<DefaultValue> GetDefaultValues(ClientStateInfo state);

        void CreateDatabase();

        List<OperationResult> ConfigureDatabase();

        void ConfigureDatabaseIfNeeded();

        OperationResult Save(ClientStateInfo state);

        OperationResult GetLatestVersion(ClientStateInfo state);

        OperationResult GetLatestIdByOwnerUser(ClientStateInfo state);

        OperationResult GetList(ListRetrivalArgs args);

        OperationResult GetValueDomains(ApplicationModel model);

        OperationResult GetDataView(ApplicationModel model, List<DataViewModelItem> viewinfo, ListRetrivalArgs args);

        OperationResult GetDataViewValue(ApplicationModel model, List<DataViewModelItem> viewinfo, ListRetrivalArgs args);

        OperationResult Validate(ApplicationModel model, ClientStateInfo state);

        OperationResult ValidateModel();

        OperationResult GenerateTestData();

        void LogError(string message, int applicationid=0, string appmetacode="NONE", string username="");

        void LogWarning(string message, int applicationid = 0, string appmetacode = "NONE", string username = "");

        void LogInfo(string message, int applicationid = 0, string appmetacode = "NONE", string username = "");

    }

    public class IntwentyDataService : IIntwentyDataService
    {
        
        private IntwentySettings Settings { get; }

        private DBMS DBMSType { get; }

        private bool IsNoSql { get; }

        private IIntwentyModelService ModelRepository { get; }

        public IntwentyDataService(IOptions<IntwentySettings> settings, IIntwentyModelService modelservice)
        {
            Settings = settings.Value;
            ModelRepository = modelservice;
            DBMSType = Settings.DefaultConnectionDBMS;
            IsNoSql = settings.Value.IsNoSQL;
        }

        public List<DefaultValue> GetDefaultValues(ClientStateInfo state)
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
                            client = new IntwentyNoSqlDbClient(DBMSType, Settings.DefaultConnection, "IntwentyDb");
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


        public List<OperationResult> ConfigureDatabase()
        {
            var res = new List<OperationResult>();
            var l = ModelRepository.GetApplicationModels();
            foreach (var model in l)
            {
                if (IsNoSql)
                {
                    var t = NoSqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentyNoSqlDbClient(DBMSType, Settings.DefaultConnection, "IntwentyDb"));
                    res.Add(t.ConfigureDatabase());

                    
                }
                else
                {
                    var t = SqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentySqlDbClient(DBMSType, Settings.DefaultConnection));
                    res.Add(t.ConfigureDatabase());
                }
            }

            return res;
        }

    

        public void ConfigureDatabaseIfNeeded()
        {
            if (IsNoSql)
                return;

            var l = ModelRepository.GetApplicationModels();
            if (l.Count > 0)
            {
                try
                {
                    var client = new IntwentySqlDbClient(DBMSType, Settings.DefaultConnection);
                    client.Open();
                    client.CreateCommand("SELECT 1 FROM " + l[0].Application.DbName);
                    client.ExecuteScalarQuery();
                    client.Close();
                }
                catch 
                {
                    ConfigureDatabase();
                }

            }
        }

        public OperationResult Save(ClientStateInfo state)
        {
            var model = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == state.ApplicationId);

            var validation = Validate(model, state);
            if (validation.IsSuccess)
            {
                if (IsNoSql)
                {
                    var t = NoSqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentyNoSqlDbClient(DBMSType, Settings.DefaultConnection, "IntwentyDb"));
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


        public OperationResult GetList(ListRetrivalArgs args)
        {
            var model = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == args.ApplicationId);

            if (IsNoSql)
            {
                var t = NoSqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentyNoSqlDbClient(DBMSType, Settings.DefaultConnection, "IntwentyDb"));
                return t.GetList(args);
            }
            else
            {
                var t = SqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentySqlDbClient(DBMSType, Settings.DefaultConnection));
                return t.GetList(args);
            }
           
        }

        public OperationResult GetLatestIdByOwnerUser(ClientStateInfo state)
        {
            if (state==null)
                return new OperationResult(false, "ClientStateInfo was null when using GetLatestIdByOwnerUser", 0, 0);
            if (state.ApplicationId < 1)
                return new OperationResult(false, "No ApplicationId was supplied when using GetLatestIdByOwnerUser", state.Id, state.Version);
            if (string.IsNullOrEmpty(state.OwnerUserId))
                return new OperationResult(false, "No OwnerUserId was supplied when using GetLatestIdByOwnerUser", state.Id, state.Version);
            if (state.UserId == ClientStateInfo.DEFAULT_USERID)
                return new OperationResult(false, "No UserId was supplied when using GetLatestIdByOwnerUser", state.Id, state.Version);

            var model = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == state.ApplicationId);


            if (IsNoSql)
            {
                var t = NoSqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentyNoSqlDbClient(DBMSType, Settings.DefaultConnection, "IntwentyDb"));
                return t.GetLatestIdByOwnerUser(state);
            }
            else
            {
                var t = SqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentySqlDbClient(DBMSType, Settings.DefaultConnection));
                return t.GetLatestIdByOwnerUser(state);
            }

          
        }

        public OperationResult GetLatestVersion(ClientStateInfo state)
        {
            var model = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == state.ApplicationId);

            if (IsNoSql)
            {
                var t = NoSqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentyNoSqlDbClient(DBMSType, Settings.DefaultConnection, "IntwentyDb"));
                return t.GetLatestVersion(state);
            }
            else
            {
                var t = SqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentySqlDbClient(DBMSType, Settings.DefaultConnection));
                return t.GetLatestVersion(state);
            }
          
        }

       

        public OperationResult GetValueDomains(ApplicationModel model)
        {
            if (IsNoSql)
            {
                var t = NoSqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentyNoSqlDbClient(DBMSType, Settings.DefaultConnection, "IntwentyDb"));
                return t.GetValueDomains();
            }
            else
            {
                var t = SqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentySqlDbClient(DBMSType, Settings.DefaultConnection));
                return t.GetValueDomains();
            }

        }

        public OperationResult Validate(ApplicationModel app, ClientStateInfo state)
        {

            foreach (var t in app.UIStructure)
            {
                if (t.IsDataColumnConnected && t.DataColumnInfo.Mandatory)
                {
                    var dv = state.Values.FirstOrDefault(p => p.DbName == t.DataColumnInfo.DbName);
                    if (dv != null && !dv.HasValue)
                    {
                        return new OperationResult(false, string.Format("The field {0} is mandatory", t.Title),state.Id, state.Version);
                    }
                    foreach (var table in state.SubTables)
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

        public OperationResult ValidateModel()
        {
            var apps = ModelRepository.GetApplicationModels();
            var viewinfo = ModelRepository.GetDataViewModels();
            var res = new OperationResult();

            if (apps.Count == 0)
            {
                res.IsSuccess = false;
                res.AddMessage("ERROR", "The model doesn't seem to exist");
            }

            foreach (var a in apps)
            {

                if (string.IsNullOrEmpty(a.Application.Title))
                {
                    res.AddMessage("ERROR", string.Format("The application with Id: {0} has no [Title].", a.Application.Id));
                    return res;
                }

                if (string.IsNullOrEmpty(a.Application.MetaCode))
                    res.AddMessage("ERROR", string.Format("The application: {0} has no [MetaCode].", a.Application.Title));

                if (string.IsNullOrEmpty(a.Application.DbName))
                    res.AddMessage("ERROR", string.Format("The application: {0} has no [DbName].", a.Application.Title));

                if (!string.IsNullOrEmpty(a.Application.MetaCode) && (a.Application.MetaCode.ToUpper() != a.Application.MetaCode))
                    res.AddMessage("ERROR", string.Format("The application: {0} has a non uppercase [MetaCode].", a.Application.Title));

                if (a.DataStructure.Count == 0)
                    res.AddMessage("WARNING", string.Format("The application {0} has no Database objects (DATVALUE, DATATABLE, etc.). Or MetaDataItems has wrong [AppMetaCode]", a.Application.Title));

                if (a.UIStructure.Count == 0)
                    res.AddMessage("WARNING", string.Format("The application {0} has no UI objects.", a.Application.Title));
                
                    
                foreach (var ui in a.UIStructure)
                {
                    if (string.IsNullOrEmpty(ui.Title) && !ui.IsMetaTypePanel)
                    {
                        res.AddMessage("ERROR", string.Format("The UI object with Id {0} in application {1} has no [Title].", ui.Id, a.Application.Title));
                        return res;
                    }

                    if (string.IsNullOrEmpty(ui.MetaCode))
                    {
                        res.AddMessage("ERROR", string.Format("The UI object {0} in application: {1} has no [MetaCode].", ui.Title, a.Application.Title));
                        return res;
                    }

                    if (string.IsNullOrEmpty(ui.ParentMetaCode))
                    {
                        res.AddMessage("ERROR", string.Format("The UI object {0} in application: {1} has no [ParentMetaCode].", ui.Title, a.Application.Title));
                        return res;
                    }

                    if (!ui.HasValidMetaType)
                    {
                        res.AddMessage("ERROR", string.Format("The UI object {0} in application: {1} has not a valid [MetaType].", ui.Title, a.Application.Title));
                        return res;
                    }

                    //if (!string.IsNullOrEmpty(ui.MetaCode) && (ui.MetaCode.ToUpper() != ui.MetaCode))
                    //    res.AddMessage("ERROR", string.Format("The UI object {0} in application: {1} has a non uppercase [MetaCode].", ui.Title, a.Application.Title));

                    if (ui.IsMetaTypeListView && !a.UIStructure.Exists(p => p.ParentMetaCode == ui.MetaCode && p.IsMetaTypeListViewField))
                        res.AddMessage("ERROR", string.Format("The UI object {0} of type LISTVIEW in application {1} has no children with [MetaType]=LISTVIEWFIELD.", ui.Title, a.Application.Title));


                    if (ui.IsMetaTypeLookUp && !ui.IsDataViewColumnConnected)
                        res.AddMessage("ERROR", string.Format("The UI object {0} of type LOOKUP in application {1} has no connection to a DATAVIEWKEYCOLUMN", ui.Title, a.Application.Title));

                    if (ui.IsMetaTypeLookUp && ui.IsDataViewColumnConnected && ui.DataViewColumnInfo.IsMetaTypeDataViewColumn)
                    {
                        res.AddMessage("ERROR", string.Format("The UI object {0} of type LOOKUP in application {1} has a connection (ViewMetaCode) to a DATAVIEWCOLUMN, it should be a DATAVIEWKEYCOLUMN", ui.Title, a.Application.Title));
                    }

                    if (ui.IsMetaTypeLookUp && !ui.IsDataViewConnected)
                        res.AddMessage("ERROR", string.Format("The UI object {0} of type LOOKUP in application {1} is not connected to a dataview, check domainname.", ui.Title, a.Application.Title));

                    if (!ui.IsMetaTypeEditGrid)
                    {
                        if (!ui.IsDataColumnConnected && !string.IsNullOrEmpty(ui.DataMetaCode) && ui.DataMetaCode.ToUpper() != "ID" && ui.DataMetaCode.ToUpper() != "VERSION")
                            res.AddMessage("ERROR", string.Format("The UI object: {0} in application {1} has a missconfigured connection to a database column [DataMetaCode].", new object[] { ui.Title, a.Application.Title }));
                    }
                    else
                    {
                        if (!ui.IsDataTableConnected)
                            res.AddMessage("ERROR", string.Format("The UI object: {0} in application {1} has a missconfigured connection to a database table [DataMetaCode].", new object[] { ui.Title, a.Application.Title }));
                    }

                    if (ui.IsMetaTypeEditGridLookUp && !ui.IsDataViewColumnConnected)
                        res.AddMessage("ERROR", string.Format("The UI object {0} of type EDITGRID_LOOKUP in application {1} has no connection to a DATAVIEWKEYCOLUMN", ui.Title, a.Application.Title));

                    if (!ui.HasValidProperties)
                    {
                        res.AddMessage("WARNING", string.Format("One or more properties on the UI object: {0} of type {1} in application {2} is not valid and may not be implemented.", new object[] { ui.Title, ui.MetaType, a.Application.Title }));
                    }

                }

                foreach (var db in a.DataStructure)
                {
                    if (string.IsNullOrEmpty(db.MetaCode))
                    {
                        res.AddMessage("ERROR", string.Format("The data object with Id: {0} in application: {1} has no [MetaCode].", db.Id, a.Application.Title));
                        return res;
                    }

                    if (string.IsNullOrEmpty(db.ParentMetaCode))
                    {
                        res.AddMessage("ERROR", string.Format("The data object: {0} in application: {1} has no [ParentMetaCode]. (ROOT ?)", db.MetaCode, a.Application.Title));
                        return res;
                    }

                    if (string.IsNullOrEmpty(db.MetaType))
                    {
                        res.AddMessage("ERROR", string.Format("The data object: {0} in application: {1} has no [MetaType].", db.MetaCode, a.Application.Title));
                        return res;
                    }

                    if (string.IsNullOrEmpty(db.DbName))
                    {
                        res.AddMessage("ERROR", string.Format("The data object: {0} in application {1} has no [DbName].", db.MetaCode, a.Application.Title));
                    }

                    if (!string.IsNullOrEmpty(db.DbName) && db.DbName.ToUpper() == db.DbName)
                    {
                        res.AddMessage("WARNING", string.Format("The data object: {0} in application {1} has its [DbName] in uppercase, not very nice.", db.MetaCode, a.Application.Title));
                    }

                }

            }


            foreach (var v in viewinfo)
            {
                if (string.IsNullOrEmpty(v.Title))
                {
                    res.AddMessage("ERROR", string.Format("The view with Id: {0} has no [Title].", v.Id));
                    return res;
                }

                if (!v.HasValidMetaType)
                {
                    res.AddMessage("ERROR", string.Format("The view object: {0} has no [MetaType].", v.Title));
                    return res;
                }

                if (!string.IsNullOrEmpty(v.SQLQueryFieldName) && v.IsMetaTypeDataView)
                {
                    res.AddMessage("WARNING", string.Format("The view object: {0} has a sql query field on the ROOT level.", v.Title));
                }

                if (!string.IsNullOrEmpty(v.SQLQuery) && v.IsMetaTypeDataViewColumn)
                {
                    res.AddMessage("WARNING", string.Format("The view object: {0} has a sql query specified. (DATAVIEWFIELD can't have queries)", v.Title));
                }

                if (v.IsMetaTypeDataView && !viewinfo.Exists(p => p.ParentMetaCode == v.MetaCode && p.IsMetaTypeDataViewColumn))
                    res.AddMessage("ERROR", string.Format("The view object: {0} has no children with [MetaType]=DATAVIEWCOLUMN.", v.Title));

                if (v.IsMetaTypeDataView && !viewinfo.Exists(p => p.ParentMetaCode == v.MetaCode && p.IsMetaTypeDataViewKeyColumn))
                    res.AddMessage("ERROR", string.Format("The view object: {0} has no children with [MetaType]=DATAVIEWKEYCOLUMN.", v.Title));

                if (v.IsMetaTypeDataViewColumn || v.IsMetaTypeDataViewKeyColumn)
                {
                    var view = viewinfo.Find(p => p.IsMetaTypeDataView && p.MetaCode == v.ParentMetaCode);
                    if (view != null)
                    {
                        if (!view.SQLQuery.Contains(v.SQLQueryFieldName))
                            res.AddMessage("ERROR", string.Format("The viewfield {0} has no SQLQueryFieldName that is included in the SQL Query of the parent view.", v.Title));
                    }
                    else
                    {
                        res.AddMessage("ERROR", string.Format("The view field {0} has no parent with [MetaType]=DATAVIEW.", v.Title));
                    }
                }

                if (v.IsMetaTypeDataView)
                {
                    if (v.HasNonSelectSql)
                    {
                        res.AddMessage("ERROR", string.Format("The sql query defined for dataview {0} contains invalid commands.", v.Title));
                    }
                    else
                    {
                        try
                        {
                            if (!IsNoSql)
                            {
                                var client = new IntwentySqlDbClient(DBMSType, Settings.DefaultConnection);
                                client.Open();
                                client.CreateCommand(v.SQLQuery);
                                var t = client.ExecuteScalarQuery();

                            }

                        }
                        catch
                        {
                            res.AddMessage("WARNING", string.Format("The sql query defined for dataview {0} returned an error.", v.Title));
                        }
                    }
                }


            }


            if (res.Messages.Exists(p => p.Code == "ERROR"))
            {
                res.IsSuccess = false;
            }
            else
            {
                res.IsSuccess = true;
                res.AddMessage("SUCCESS", "Model validated successfully");
            }

            return res;
        }

        public OperationResult GenerateTestData()
        {


            var res = new OperationResult();
            var amount = 100;

            try
            {
                IntwentyNoSqlDbClient nosqlclient=null;
                IntwentySqlDbClient sqlclient=null;
                if (IsNoSql)
                {
                    nosqlclient = new IntwentyNoSqlDbClient(DBMSType, Settings.DefaultConnection, "IntwentyDb");
                }
                else
                {
                    sqlclient = new IntwentySqlDbClient(DBMSType, Settings.DefaultConnection);
                }

                var apps = ModelRepository.GetApplicationModels();

                var counter = 0;

                var properties = "ISTESTDATA=TRUE#TESTDATABATCH=Test Batch - " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

                //RUN APPS WITHOUT A LOOKUP CONTROL AS THEY MIGHT BE USED BY DATAVIEWS
                foreach (var app in apps)
                {
                    if (app.UIStructure.Exists(p => p.IsMetaTypeLookUp))
                        continue;

   

                    for (int i = 0; i < amount; i++)
                    {
                        IDataManager manager;
                        if (IsNoSql)
                        {
                            manager = NoSqlDbDataManager.GetDataManager(app, ModelRepository, Settings, nosqlclient);
                        }
                        else
                        {

                            manager = SqlDbDataManager.GetDataManager(app, ModelRepository, Settings, sqlclient);
                        }

                        var clientstate = new ClientStateInfo();
                        clientstate.Properties = properties;
                        manager.ClientState = clientstate;
                        var result = manager.GenerateTestData(i);
                        if (result.IsSuccess)
                            counter += 1;


                    }
                }

                //RUN APPS WITH A LOOKUP CONTROL AS THEY MIGHT BE USING DATAVIEWS
                foreach (var app in apps)
                {
                    if (!app.UIStructure.Exists(p => p.IsMetaTypeLookUp))
                        continue;

                    for (int i = 0; i < amount; i++)
                    {
                        IDataManager manager;
                        if (IsNoSql)
                        {
                            manager = NoSqlDbDataManager.GetDataManager(app, ModelRepository, Settings, nosqlclient);
                        }
                        else
                        {

                            manager = SqlDbDataManager.GetDataManager(app, ModelRepository, Settings, sqlclient);
                        }

                        var clientstate = new ClientStateInfo();
                        clientstate.Properties = properties;
                        manager.ClientState = clientstate;
                        var result = manager.GenerateTestData(i);
                        if (result.IsSuccess)
                            counter += 1;

                    }
                }

                res = new OperationResult(true, string.Format("Generated {0} test data applications.", counter), 0, 0);
            }
            catch (Exception ex)
            {
                res.SetError(ex.Message, "An error occured when generating testdata");
            }


            return res;
        }

       

        public OperationResult GetDataView(ApplicationModel model, List<DataViewModelItem> viewinfo, ListRetrivalArgs args)
        {
            if (IsNoSql)
            {
                var t = NoSqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentyNoSqlDbClient(DBMSType, Settings.DefaultConnection, "IntwentyDb"));
                return t.GetDataView(viewinfo, args);
            }
            else
            {
                var t = SqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentySqlDbClient(DBMSType, Settings.DefaultConnection));
                return t.GetDataView(viewinfo, args);
            }

          
        }

        public OperationResult GetDataViewValue(ApplicationModel model, List<DataViewModelItem> viewinfo, ListRetrivalArgs args)
        {
            if (IsNoSql)
            {
                var t = NoSqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentyNoSqlDbClient(DBMSType, Settings.DefaultConnection, "IntwentyDb"));
                return t.GetDataViewValue(viewinfo, args);
            }
            else
            {
                var t = SqlDbDataManager.GetDataManager(model, ModelRepository, Settings, new IntwentySqlDbClient(DBMSType, Settings.DefaultConnection));
                return t.GetDataViewValue(viewinfo, args);
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

                    throw new NotImplementedException("LogEvent is not implemented for nosql");
                }
                else
                {
                    var client = new IntwentySqlDbClient(DBMSType, Settings.DefaultConnection);
                    client.Open();
                    client.CreateCommand("INSERT INTO [sysdata_EventLog] (EventDate, Verbosity, Message, AppMetaCode, ApplicationId,UserName) VALUES (getDate(), @Verbosity, @Message, @AppMetaCode, @ApplicationId,@UserName)");
                    client.AddParameter("@Verbosity", verbosity);
                    client.AddParameter("@Message", message);
                    client.AddParameter("@AppMetaCode", appmetacode);
                    client.AddParameter("@ApplicationId", applicationid);
                    client.AddParameter("@UserName", username);
                    client.ExecuteNonQuery();
                    client.Close();
                }

            }
            catch { }
        }

        public void CreateDatabase()
        {
            if (IsNoSql)
                return;

            if (!Settings.IsDevelopment)
                return;

            var client = new IntwentySqlDbClient(DBMSType, Settings.DefaultConnection);
            client.Open();
            client.CreateTable<ApplicationItem>(true);
            client.CreateTable<DatabaseItem>(true);
            client.CreateTable<DataViewItem>(true);
            client.CreateTable<EventLog>(true);
            client.CreateTable<InformationStatus>(true);
            client.CreateTable<MenuItem>(true);
            client.CreateTable<SystemID>(true);
            client.CreateTable<UserInterfaceItem>(true);
            client.CreateTable<ValueDomainItem>(true);

        }
    }
}
