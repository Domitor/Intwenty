using System;
using Microsoft.Extensions.Options;
using Intwenty.Data;
using Intwenty.MetaDataService.Engine;
using Intwenty.MetaDataService.Model;
using Intwenty.Models;
using System.Collections.Generic;
using System.Linq;

namespace Intwenty.MetaDataService
{
    public interface IServiceEngine
    {
        List<OperationResult> ConfigureDatabase();

        OperationResult Save(ClientStateInfo state);

        OperationResult GetLatestVersion(ClientStateInfo state);

        OperationResult GetListView(ListRetrivalArgs args);

        OperationResult GetValueDomains(ApplicationModel app);

        OperationResult GetDataView(ApplicationModel app, List<DataViewModelItem> viewinfo, ListRetrivalArgs args);

        OperationResult GetDataViewValue(ApplicationModel app, List<DataViewModelItem> viewinfo, ListRetrivalArgs args);

        OperationResult Validate(ApplicationModel app, ClientStateInfo state);

        OperationResult ValidateModel();

        OperationResult GenerateTestData();

    }

    public class Server : IServiceEngine
    {
        
        private IOptions<SystemSettings> SysSettings { get; }
        private IModelRepository ModelRepository { get; }
        private IDataAccessService DataRepository { get; }

        public Server(IOptions<SystemSettings> sysconfig, IModelRepository mr, IDataAccessService dr)
        {
            SysSettings = sysconfig;
            ModelRepository = mr;
            DataRepository = dr;
        }
        

        public List<OperationResult> ConfigureDatabase()
        {
            var res = new List<OperationResult>();
            var l = ModelRepository.GetApplicationModels();
            foreach (var app in l)
            {
                var t = DataManager.GetDataManager(app);
                res.Add(t.ConfigureDatabase());
            }

            return res;
        }

        public OperationResult Save(ClientStateInfo state)
        {
            var app = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == state.ApplicationId);

            var validation = Validate(app, state);
            if (validation.IsSuccess)
            {
                var t = DataManager.GetDataManager(app);
                var result = t.Save(state);
                return result;
            }
            else
            {
                return validation;
            }
        }


        public OperationResult GetListView(ListRetrivalArgs args)
        {
            var app = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == args.ApplicationId);
            var t = DataManager.GetDataManager(app);
            t.DataRepository = DataRepository;
            t.ModelRepository = ModelRepository;
            return t.GetListView(args);
        }

        public OperationResult GetLatestVersion(ClientStateInfo state)
        {
            var app = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == state.ApplicationId);
            var t = DataManager.GetDataManager(app);
            t.DataRepository = DataRepository;
            t.ModelRepository = ModelRepository;
            return t.GetLatestVersion(state);
        }

        public OperationResult GetValueDomains(ApplicationModel app)
        {
            var t = DataManager.GetDataManager(app);
            return t.GetValueDomains();
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
                            DataRepository.Open();
                            DataRepository.CreateCommand(v.SQLQuery);
                            var t = DataRepository.ExecuteScalarQuery();

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

            try
            {
                var apps = ModelRepository.GetApplicationModels();

                var counter = 0;

                var properties = "ISTESTDATA=TRUE#TESTDATABATCH=Test Batch - " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

                //RUN APPS WITHOUT A LOOKUP CONTROL AS THEY MIGHT BE USED BY DATAVIEWS
                foreach (var app in apps)
                {
                    if (app.UIStructure.Exists(p => p.IsMetaTypeLookUp))
                        continue;

                    var amount = app.Application.TestDataAmount;
                    if (amount < 100 || amount > 100)
                        amount = 100;

                    for (int i = 0; i < amount; i++)
                    {
                        var t = DataManager.GetDataManager(app);
                        var clientstate = new ClientStateInfo();
                        clientstate.Properties = properties;
                        t.ClientState = clientstate;
                        t.DataRepository = DataRepository;
                        t.ModelRepository = ModelRepository;
                        var result = t.GenerateTestData(i);
                        if (result.IsSuccess)
                            counter += 1;

                    }
                }

                //RUN APPS WITH A LOOKUP CONTROL AS THEY MIGHT BE USING DATAVIEWS
                foreach (var app in apps)
                {
                    if (!app.UIStructure.Exists(p => p.IsMetaTypeLookUp))
                        continue;

                    var amount = app.Application.TestDataAmount;
                    if (amount < 100 || amount > 100)
                        amount = 100;

                    for (int i = 0; i < amount; i++)
                    {
                        var t = DataManager.GetDataManager(app);
                        var clientstate = new ClientStateInfo();
                        clientstate.Properties = properties;
                        t.ClientState = clientstate;
                        t.DataRepository = DataRepository;
                        t.ModelRepository = ModelRepository;
                        var result = t.GenerateTestData(i);
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

       

        public OperationResult GetDataView(ApplicationModel app, List<DataViewModelItem> viewinfo, ListRetrivalArgs args)
        {
            var t = DataManager.GetDataManager(app);
            return t.GetDataView(viewinfo, args);
        }

        public OperationResult GetDataViewValue(ApplicationModel app, List<DataViewModelItem> viewinfo, ListRetrivalArgs args)
        {
            var t = DataManager.GetDataManager(app);
            return t.GetDataViewValue(viewinfo, args);
        }
    }
}
