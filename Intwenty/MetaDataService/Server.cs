using Microsoft.Extensions.Options;
using Moley.Data;
using Moley.Data.Dto;
using Moley.MetaDataService.Custom;
using Moley.MetaDataService.Engine;
using Moley.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moley.MetaDataService
{
    public interface IServiceEngine
    {
        OperationResult ConfigureDatabase(ApplicationDto app);

        OperationResult Save(ApplicationDto app, ClientStateInfo state, Dictionary<string, object> data);

        OperationResult GetLatestVersion(ApplicationDto app, ClientStateInfo state);

        OperationResult GetListView(ApplicationDto app, ListRetrivalArgs args);

        OperationResult GetValueDomains(ApplicationDto app);

        OperationResult GetDataView(ApplicationDto app, List<MetaDataViewDto> viewinfo, ListRetrivalArgs args);

        OperationResult GetDataViewValue(ApplicationDto app, List<MetaDataViewDto> viewinfo, ListRetrivalArgs args);

        OperationResult Validate(ApplicationDto app, ClientStateInfo state, Dictionary<string, object> data);

        OperationResult ValidateModel(List<ApplicationDto> apps, List<MetaDataViewDto> viewinfo);

        OperationResult GenerateTestData(List<ApplicationDto> apps, ISystemRepository repository);

    }

    public class Server : IServiceEngine
    {
        
        private IOptions<SystemSettings> SysSettings { get; }

        public Server(IOptions<SystemSettings> sysconfig)
        {
            SysSettings = sysconfig;
        }
        

        public OperationResult ConfigureDatabase(ApplicationDto app)
        {
            var t = DataManager.GetDataManager(app);
            return t.ConfigureDatabase();
        }

        public OperationResult Save(ApplicationDto app, ClientStateInfo state, Dictionary<string, object> data)
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


        public OperationResult GetListView(ApplicationDto app, ListRetrivalArgs args)
        {
            var t = DataManager.GetDataManager(app);
            return t.GetListView(args);
        }

        public OperationResult GetLatestVersion(ApplicationDto app, ClientStateInfo state)
        {
            var t = DataManager.GetDataManager(app);
            t.ClientState = state;
            return t.GetLatestVersion();
        }

        public OperationResult GetValueDomains(ApplicationDto app)
        {
            var t = DataManager.GetDataManager(app);
            return t.GetValueDomains();
        }

        public OperationResult Validate(ApplicationDto app, ClientStateInfo state, Dictionary<string, object> data)
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

        public OperationResult ValidateModel(List<ApplicationDto> apps, List<MetaDataViewDto> viewinfo)
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

                res.AddMessage("RESULT", string.Format("Validating the model for application {0}", a.Application.Title));

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

                    if (ui.IsUITypeListView && !a.UIStructure.Exists(p => p.ParentMetaCode == ui.MetaCode && p.IsUITypeListViewField))
                        res.AddMessage("ERROR", string.Format("The UI object {0} of type LISTVIEW in application {1} has no children with [MetaType]=LISTVIEWFIELD.", ui.Title, a.Application.Title));


                    if (ui.IsUITypeLookUp && !a.UIStructure.Exists(p => p.ParentMetaCode == ui.MetaCode && p.IsUITypeLookUpField))
                        res.AddMessage("ERROR", string.Format("The UI object {0} of type LOOKUP in application {1} has no children with [MetaType]=LOOKUPFIELD.", ui.Title, a.Application.Title));

                    if (ui.IsUITypeLookUp && !a.UIStructure.Exists(p => p.ParentMetaCode == ui.MetaCode && p.IsUITypeLookUpKeyField))
                        res.AddMessage("ERROR", string.Format("The UI object {0} of type LOOKUP in application {1} has no children with [MetaType]=LOOKUPKEYFIELD.", ui.Title, a.Application.Title));

                    if (ui.IsUITypeLookUp && !ui.IsDataViewConnected)
                        res.AddMessage("ERROR", string.Format("The UI object {0} of type LOOKUP in application {1} is not connected to a dataview, check domainname.", ui.Title, a.Application.Title));

                    if (ui.IsUITypeLookUpKeyField && ui.IsDataViewConnected && !ui.ViewInfo.IsMetaTypeDataViewKeyField)
                        res.AddMessage("ERROR", string.Format("The UI object {0} of type LOOKUPKEYFIELD in application {1} is not connected to a DATAVIEWKEYFIELD, check domainname.", ui.Title, a.Application.Title));

                    if (ui.IsUITypeLookUpField && ui.IsDataViewConnected && !ui.ViewInfo.IsMetaTypeDataViewField)
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

                foreach (var v in viewinfo)
                {
                    if (string.IsNullOrEmpty(v.Title))
                    {
                        res.AddMessage("ERROR", string.Format("The view with Id: {0} has no [Title].", v.Id));
                        return res;
                    }

                    if (!v.HasValidMetaType)
                    {
                        res.AddMessage("ERROR", string.Format("The view object: {0} in application: {1} has no [MetaType].", v.Title, a.Application.Title));
                        return res;
                    }

                    if (!string.IsNullOrEmpty(v.SQLQueryFieldName) && v.IsMetaTypeDataView)
                    {
                        res.AddMessage("WARNING", string.Format("The view object: {0} in application {1} has a sqlquery field on the ROOT level.", v.Title, a.Application.Title));
                    }

                    if (!string.IsNullOrEmpty(v.SQLQuery) && v.IsMetaTypeDataViewField)
                    {
                        res.AddMessage("WARNING", string.Format("The view object: {0} in application {1} has a sqlquery specified. (DATAVIEWFIELD can't have queries)", v.Title, a.Application.Title));
                    }

                    if (v.IsMetaTypeDataView && !viewinfo.Exists(p => p.ParentMetaCode == v.MetaCode && p.IsMetaTypeDataViewField))
                        res.AddMessage("ERROR", string.Format("The view object: {0} in application {1} has no children with [MetaType]=DATAVIEWFIELD.", v.Title, a.Application.Title));

                    if (v.IsMetaTypeDataView && !viewinfo.Exists(p => p.ParentMetaCode == v.MetaCode && p.IsMetaTypeDataViewKeyField))
                        res.AddMessage("ERROR", string.Format("The view object: {0} in application {1} has no children with [MetaType]=DATAVIEWKEYFIELD.", v.Title, a.Application.Title));

                    if (v.IsMetaTypeDataViewField || v.IsMetaTypeDataViewKeyField)
                    {
                        var view = viewinfo.Find(p => p.IsMetaTypeDataView && p.MetaCode == v.ParentMetaCode);
                        if (view != null)
                        {
                            if (!view.SQLQuery.Contains(v.SQLQueryFieldName))
                                res.AddMessage("ERROR", string.Format("The viewfield {0} in application {1} has not a SQLQueryFieldName that is included in the SQLQuery of the parent view.", v.Title, a.Application.Title));
                        }
                        else
                        {
                            res.AddMessage("ERROR", string.Format("The view field {0} in application {1} has no parent with [MetaType]=DATAVIEW.", v.Title, a.Application.Title));
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

        public OperationResult GenerateTestData(List<ApplicationDto> apps, ISystemRepository repository)
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

       

        public OperationResult GetDataView(ApplicationDto app, List<MetaDataViewDto> viewinfo, ListRetrivalArgs args)
        {
            var t = DataManager.GetDataManager(app);
            return t.GetDataView(viewinfo, args);
        }

        public OperationResult GetDataViewValue(ApplicationDto app, List<MetaDataViewDto> viewinfo, ListRetrivalArgs args)
        {
            var t = DataManager.GetDataManager(app);
            return t.GetDataViewValue(viewinfo, args);
        }
    }
}
