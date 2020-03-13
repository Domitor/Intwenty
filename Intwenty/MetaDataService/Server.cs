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

        OperationResult Save(ApplicationModel app, ClientStateInfo state, Dictionary<string, object> data);

        OperationResult GetLatestVersion(ApplicationModel app, ClientStateInfo state);

        OperationResult GetListView(ApplicationModel app, ListRetrivalArgs args);

        OperationResult GetValueDomains(ApplicationModel app);

        OperationResult GetDataView(ApplicationModel app, List<DataViewModelItem> viewinfo, ListRetrivalArgs args);

        OperationResult GetDataViewValue(ApplicationModel app, List<DataViewModelItem> viewinfo, ListRetrivalArgs args);

        OperationResult Validate(ApplicationModel app, ClientStateInfo state, Dictionary<string, object> data);

        OperationResult ValidateModel(List<ApplicationModel> apps, List<DataViewModelItem> viewinfo);

        OperationResult GenerateTestData(List<ApplicationModel> apps, IModelRepository repository);

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

        public OperationResult Save(ApplicationModel app, ClientStateInfo state, Dictionary<string, object> data)
        {

            var validation = Validate(app, state, data);
            if (validation.IsSuccess)
            {
                var t = DataManager.GetDataManager(app);
                t.ClientState = state;
                var result = t.Save(data);
                return result;
            }
            else
            {
                return validation;
            }
        }


        public OperationResult GetListView(ApplicationModel app, ListRetrivalArgs args)
        {
            var t = DataManager.GetDataManager(app);
            t.DataRepository = DataRepository;
            t.ModelRepository = ModelRepository;
            return t.GetListView(args);
        }

        public OperationResult GetLatestVersion(ApplicationModel app, ClientStateInfo state)
        {
            var t = DataManager.GetDataManager(app);
            t.DataRepository = DataRepository;
            t.ModelRepository = ModelRepository;
            t.ClientState = state;
            return t.GetLatestVersion();
        }

        public OperationResult GetValueDomains(ApplicationModel app)
        {
            var t = DataManager.GetDataManager(app);
            return t.GetValueDomains();
        }

        public OperationResult Validate(ApplicationModel app, ClientStateInfo state, Dictionary<string, object> data)
        {

            foreach (var t in app.UIStructure)
            {
                if (t.IsDataConnected && t.DataInfo.Mandatory)
                {
                    var dv = data.FirstOrDefault(p => p.Key == t.DataMetaCode);
                    if (dv.Equals(default(KeyValuePair<string, object>)))
                    {
                        return new OperationResult(false, string.Format("The field {0} is mandatory", t.Title),state.Id, state.Version);
                    }
                    else
                    {
                        if (dv.Value == null)
                            return new OperationResult(false, string.Format("The field {0} is mandatory", t.Title), state.Id, state.Version);
                        else if (string.IsNullOrEmpty(Convert.ToString(dv.Value)))
                            return new OperationResult(false, string.Format("The field {0} is mandatory", t.Title), state.Id, state.Version);
                    }
                }
            }

            return new OperationResult(true, "Successfully validated", state.Id, state.Version);
        }

        public OperationResult ValidateModel(List<ApplicationModel> apps, List<DataViewModelItem> viewinfo)
        {
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
                    res.AddMessage("WARNING", string.Format("The application {0} has no Database objects (DATVALUE, DATAVALUETABLE, etc.). Or MetaDataItems has wrong [AppMetaCode]", a.Application.Title));

                if (a.UIStructure.Count == 0)
                    res.AddMessage("WARNING", string.Format("The application {0} has no UI objects. Or MetaUIItems has wrong [AppMetaCode]", a.Application.Title));
                
                    
                foreach (var ui in a.UIStructure)
                {
                    if (string.IsNullOrEmpty(ui.Title))
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

                    if (!string.IsNullOrEmpty(ui.MetaCode) && (ui.MetaCode.ToUpper() != ui.MetaCode))
                        res.AddMessage("ERROR", string.Format("The UI object {0} in application: {1} has a non uppercase [MetaCode].", ui.Title, a.Application.Title));

                    if (ui.IsMetaTypeListView && !a.UIStructure.Exists(p => p.ParentMetaCode == ui.MetaCode && p.IsMetaTypeListViewField))
                        res.AddMessage("ERROR", string.Format("The UI object {0} of type LISTVIEW in application {1} has no children with [MetaType]=LISTVIEWFIELD.", ui.Title, a.Application.Title));


                    if (ui.IsMetaTypeLookUp && !a.UIStructure.Exists(p => p.ParentMetaCode == ui.MetaCode && p.IsMetaTypeLookUpField))
                        res.AddMessage("ERROR", string.Format("The UI object {0} of type LOOKUP in application {1} has no children with [MetaType]=LOOKUPFIELD.", ui.Title, a.Application.Title));

                    if (ui.IsMetaTypeLookUp && !a.UIStructure.Exists(p => p.ParentMetaCode == ui.MetaCode && p.IsMetaTypeLookUpKeyField))
                        res.AddMessage("ERROR", string.Format("The UI object {0} of type LOOKUP in application {1} has no children with [MetaType]=LOOKUPKEYFIELD.", ui.Title, a.Application.Title));

                    if (ui.IsMetaTypeLookUp && !ui.IsDataViewConnected)
                        res.AddMessage("ERROR", string.Format("The UI object {0} of type LOOKUP in application {1} is not connected to a dataview, check domainname.", ui.Title, a.Application.Title));

                    if (ui.IsMetaTypeLookUpKeyField && ui.IsDataViewConnected && !ui.ViewInfo.IsMetaTypeDataViewKeyField)
                        res.AddMessage("ERROR", string.Format("The UI object {0} of type LOOKUPKEYFIELD in application {1} is not connected to a DATAVIEWKEYFIELD, check domainname.", ui.Title, a.Application.Title));

                    if (ui.IsMetaTypeLookUpField && ui.IsDataViewConnected && !ui.ViewInfo.IsMetaTypeDataViewField)
                        res.AddMessage("ERROR", string.Format("The UI object {0} of type LOOKUPFIELD in application {1} is not connected to a DATAVIEWFIELD, check domainname.", ui.Title, a.Application.Title));


                    if (!ui.IsDataConnected && !string.IsNullOrEmpty(ui.DataMetaCode) && ui.DataMetaCode.ToUpper() != "ID" && ui.DataMetaCode.ToUpper() != "VERSION")
                        res.AddMessage("ERROR", string.Format("The UI object: {0} in application {1} has a missconfigured connection to MetaDataItem [DataMetaCode].", new object[] { ui.Title, a.Application.Title } ));

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

                if (!string.IsNullOrEmpty(v.SQLQuery) && v.IsMetaTypeDataViewField)
                {
                    res.AddMessage("WARNING", string.Format("The view object: {0} has a sql query specified. (DATAVIEWFIELD can't have queries)", v.Title));
                }

                if (v.IsMetaTypeDataView && !viewinfo.Exists(p => p.ParentMetaCode == v.MetaCode && p.IsMetaTypeDataViewField))
                    res.AddMessage("ERROR", string.Format("The view object: {0} has no children with [MetaType]=DATAVIEWFIELD.", v.Title));

                if (v.IsMetaTypeDataView && !viewinfo.Exists(p => p.ParentMetaCode == v.MetaCode && p.IsMetaTypeDataViewKeyField))
                    res.AddMessage("ERROR", string.Format("The view object: {0} has no children with [MetaType]=DATAVIEWKEYFIELD.", v.Title));

                if (v.IsMetaTypeDataViewField || v.IsMetaTypeDataViewKeyField)
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

        public OperationResult GenerateTestData(List<ApplicationModel> apps, IModelRepository repository)
        {
            var counter = 0;
            foreach (var app in apps)
            {
                for (int i = 0; i < app.Application.TestDataAmount; i++)
                {
                    var t = DataManager.GetDataManager(app);
                    t.ClientState = new ClientStateInfo();
                    var result = t.GenerateTestData(repository, i);
                    if (result.IsSuccess)
                        counter += 1;
                }
            }

            return new OperationResult(true,string.Format("Generated {0} test data applications.",counter),0,0);
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
