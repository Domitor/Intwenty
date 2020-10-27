using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Intwenty.Model.DesignerVM;
using Intwenty.Model;
using Microsoft.AspNetCore.Authorization;
using Intwenty.Data.Dto;
using Intwenty.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using Intwenty.Data.Helpers;
using Intwenty.Interface;

namespace Intwenty.Controllers
{

    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Policy = "IntwentyModelAuthorizationPolicy")]
    public class ModelAPIController : Controller
    {
        public IIntwentyDataService DataRepository { get; }
        public IIntwentyModelService ModelRepository { get; }

        public ModelAPIController(IIntwentyDataService ms, IIntwentyModelService sr)
        {
            DataRepository = ms;
            ModelRepository = sr;
        }

        #region Applicaion models

        /// <summary>
        /// Get full model data for application with id
        /// </summary>
        [HttpGet("/Model/API/GetApplication/{applicationid}")]
        public JsonResult GetApplication(int applicationid)
        {
            var t = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == applicationid);
            return new JsonResult(t.Application);

        }

        /// <summary>
        /// Get model data for applications
        /// </summary>
        [HttpGet("/Model/API/GetApplications")]
        public JsonResult GetApplications()
        {
            var t = ModelRepository.GetAppModels();
            return new JsonResult(t);

        }


        [HttpPost("/Model/API/Save")]
        public JsonResult Save([FromBody] ApplicationModelItem model)
        {
            var res = ModelRepository.SaveAppModel(model);
            return new JsonResult(res);
        }

        /// <summary>
        /// Delete model data for application
        /// </summary>
        [HttpPost("/Model/API/DeleteApplicationModel")]
        public JsonResult DeleteApplicationModel([FromBody] ApplicationModelItem model)
        {
            try
            {
                ModelRepository.DeleteAppModel(model);
            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError(ex.Message, "An error occured when deleting application model data.");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

            return GetApplications();
        }

        #endregion

        #region Database Model



        [HttpPost("/Model/API/SaveDatabaseModels")]
        public JsonResult SaveDatabaseModels([FromBody] DBVm model)
        {
            try
            {
                if (model.Id < 1)
                    throw new InvalidOperationException("ApplicationId missing in model");

                var list = DatabaseModelCreator.GetDatabaseModel(model);

                ModelRepository.SaveDatabaseModels(list, model.Id);

            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError(ex.Message, "An error occured when saving database model.");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

            return GetDatabaseModels(model.Id);
        }



        /// <summary>
        /// Removes one database object (column / table) from the UI meta data collection
        /// </summary>
        [HttpPost("/Model/API/DeleteDatabaseModel")]
        public JsonResult DeleteDatabaseModel([FromBody] DatabaseTableColumnVm model)
        {
            try
            {
                if (model.Id < 1)
                    throw new InvalidOperationException("Id is missing in model when removing db model");
                if (model.ApplicationId < 1)
                    throw new InvalidOperationException("ApplicationId is missing when removing db model");

                ModelRepository.DeleteDatabaseModel(model.Id);

            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError(ex.Message, "An error occured when deleting database model.");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

            return GetDatabaseModels(model.ApplicationId);
        }

        /// <summary>
        /// Get database model for application tables for application with id
        /// </summary>
        [HttpGet("/Model/API/GetDatabaseModels/{applicationid}")]
        public JsonResult GetDatabaseModels(int applicationid)
        {

            try
            {
                var t = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == applicationid);
                if (t == null)
                    throw new InvalidOperationException("ApplicationId missing when fetching application db meta");

                var dbvm = DatabaseModelCreator.GetDatabaseVm(t);
                return new JsonResult(dbvm);
            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError(ex.Message, "An error occured when fetching application database meta data.");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

        }

        /// <summary>
        /// Get meta data for available datatypes
        /// </summary>
        [HttpGet("/Model/API/GetIntwentyDataTypes")]
        public JsonResult GetIntwentyDataTypes()
        {
            var t = new DatabaseModelItem();
            return new JsonResult(DatabaseModelItem.GetAvailableDataTypes());

        }

        /// <summary>
        /// Get model data for application tables for application with id
        /// </summary>
        [HttpGet("/Model/API/GetDatabaseTables/{applicationid}")]
        public JsonResult GetDatabaseTables(int applicationid)
        {
            var t = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == applicationid);
            return new JsonResult(DatabaseModelCreator.GetDatabaseTableVm(t));

        }

        /// <summary>
        /// Get model data for application tables for application with id
        /// </summary>
        [HttpGet("/Model/API/GetListviewTable/{applicationid}")]
        public JsonResult GetListviewTable(int applicationid)
        {
            var t = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == applicationid);
            return new JsonResult(DatabaseModelCreator.GetListViewTableVm(t));

        }


        #endregion

        #region Dataview Model

        /// <summary>
        /// Get meta data for data views
        /// </summary>
        [HttpGet("/Model/API/GetDataViewModels")]
        public JsonResult GetDataViewModels()
        {
            var t = ModelRepository.GetDataViewModels();
            var views = DataViewModelCreator.GetDataViewVm(t);
            var res = new JsonResult(views);
            return res;

        }


        [HttpPost("/Model/API/SaveDataViewModels")]
        public JsonResult SaveDataViewModels([FromBody] DataViewVm model)
        {
            try
            {

                var dtolist = DataViewModelCreator.GetDataViewModel(model);
                ModelRepository.SaveDataViewModels(dtolist);

            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError(ex.Message, "An error occured when saving dataview model.");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

            return GetDataViewModels();
        }


        [HttpPost("/Model/API/DeleteDataViewModel")]
        public JsonResult DeleteDataViewModel([FromBody] DataViewVm model)
        {
            try
            {
                ModelRepository.DeleteDataViewModel(model.Id);

            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError(ex.Message, "An error occured when deleting application dataview model.");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

            return GetDataViewModels();
        }

        #endregion

        #region UI Model


        /// <summary>
        /// Get UI model for application with id
        /// </summary>
        [HttpGet("/Model/API/GetApplicationUI/{applicationid}")]
        public JsonResult GetApplicationUI(int applicationid)
        {
            var t = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == applicationid);
            return new JsonResult(UIModelCreator.GetUIVm(t));

        }

        [HttpPost("/Model/API/SaveUserInterfaceModel")]
        public JsonResult SaveUserInterfaceModel([FromBody] UIVm model)
        {
            try
            {
                if (model.Id < 1)
                    throw new InvalidOperationException("ApplicationId missing in model");


                var views = ModelRepository.GetDataViewModels();
                var app = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == model.Id);
                if (app == null)
                    throw new InvalidOperationException("Could not find application");

                var dtolist = UIModelCreator.GetUIModel(model, app, views);

                ModelRepository.SaveUserInterfaceModels(dtolist);

                var t = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == app.Application.Id);
                var vm = UIModelCreator.GetUIVm(t);
                return new JsonResult(vm);

            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError(ex.Message, "An error occured when saving user interface model.");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

        }


        /// <summary>
        /// Get meta data for application with id
        /// </summary>
        [HttpGet("/Model/API/GetListViewModel/{applicationid}")]
        public JsonResult GetListViewModel(int applicationid)
        {
            var t = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == applicationid);
            return new JsonResult(ListViewVm.GetListView(t));

        }

        [HttpPost("/Model/API/SaveListViewModel")]
        public JsonResult SaveListViewModel([FromBody] ListViewVm model)
        {
            try
            {

                if (model.ApplicationId < 1)
                    throw new InvalidOperationException("ApplicationId is missing in model when saving listview");

                var app = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == model.ApplicationId);
                if (app == null)
                    throw new InvalidOperationException("Could not find application");

                var dtolist = MetaDataListViewCreator.GetMetaUIListView(model, app);
                ModelRepository.SaveUserInterfaceModels(dtolist);

            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError(ex.Message, "An error occured when saving application dataview meta data.");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

            return GetListViewModel(model.ApplicationId);
        }

        #endregion

        #region Value Domains


        /// <summary>
        /// Get meta data for application ui declarations for application with id
        /// </summary>
        [HttpGet("/Model/API/GetValueDomains")]
        public JsonResult GetValueDomains()
        {
            var t = ModelRepository.GetValueDomains();
            return new JsonResult(t);

        }

        /// <summary>
        /// Get meta data for application ui declarations for application with id
        /// </summary>
        [HttpGet("/Model/API/GetValueDomainNames")]
        public JsonResult GetValueDomainNames()
        {
            var t = ModelRepository.GetValueDomains();
            return new JsonResult(t.Select(p => p.DomainName).Distinct());

        }

        [HttpPost("/Model/API/SaveValueDomains")]
        public JsonResult SaveValueDomains([FromBody] List<ValueDomainModelItem> model)
        {
            try
            {
                ModelRepository.SaveValueDomains(model);
            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError(ex.Message, "An error occured when saving value domain meta data.");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

            return GetValueDomains();
        }


        [HttpPost("/Model/API/RemoveValueDomain")]
        public JsonResult RemoveValueDomain([FromBody] ValueDomainModelItem model)
        {
            try
            {
                if (model.Id < 1)
                    throw new InvalidOperationException("Id is missing in model when removing value domain value");

                ModelRepository.DeleteValueDomain(model.Id);
            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError(ex.Message, "An error occured when deleting value domain meta data.");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

            return GetValueDomains();
        }



        #endregion

        #region Import/Export

        /// <summary>
        /// Create a json file containing the current model
        /// </summary>
        [HttpGet("/Model/API/ExportModel")]
        public IActionResult ExportModel()
        {
            var t = ModelRepository.GetSystemModel();
            var json = System.Text.Json.JsonSerializer.Serialize(t);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
            return File(bytes, "application/json", "intwentymodel.json");
        }

        [HttpPost("/Model/API/UploadModel")]
        public async Task<JsonResult> UploadModel(IFormFile file, bool delete)
        {
            var result = new OperationResult();

            try
            {
                string fileContents;
                using (var stream = file.OpenReadStream())
                using (var reader = new StreamReader(stream))
                {
                    fileContents = await reader.ReadToEndAsync();
                }
                var model = System.Text.Json.JsonSerializer.Deserialize<SystemModel>(fileContents);
                model.DeleteCurrentModel = delete;
                result = ModelRepository.InsertSystemModel(model);
            }
            catch(Exception ex) 
            {
                result.SetError(ex.Message, "An error occured when uploading a new model file.");
                var jres = new JsonResult(result);
                jres.StatusCode = 500;
                return jres;
            }

            return new JsonResult(result);
        }

        /// <summary>
        /// Create a json file containing all data registered with the current model
        /// </summary>
        [HttpGet("/Model/API/ExportData")]
        public IActionResult ExportData()
        {
            var data = new StringBuilder("{\"IntwentyData\":[");
            var apps = ModelRepository.GetApplicationModels();
            var sep = "";
            foreach (var app in apps)
            {
                var client = DataRepository.GetDataClient();
                var infostatuslist = client.GetEntities<InformationStatus>().Where(p => p.ApplicationId == app.Application.Id);

                foreach (var istat in infostatuslist)
                {
                    data.Append(sep + "{");
                    data.Append(DBHelpers.GetJSONValue("ApplicationId", app.Application.Id));
                    data.Append("," + DBHelpers.GetJSONValue("AppMetaCode", app.Application.MetaCode));
                    data.Append("," + DBHelpers.GetJSONValue("DbName", app.Application.DbName));
                    data.Append("," + DBHelpers.GetJSONValue("Id", istat.Id));
                    data.Append("," + DBHelpers.GetJSONValue("Version", istat.Version));
                    data.Append(",\"ApplicationData\":");

                    var state = new ClientStateInfo() { ApplicationId = app.Application.Id, Id = istat.Id, Version = istat.Version };
                    var appversiondata = DataRepository.GetLatestVersionById(state);
                    data.Append(appversiondata.Data);
                    data.Append("}");
                    sep = ",";

                }
            }
            data.Append("]");
            data.Append("}");


            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data.ToString());
            return File(bytes, "application/json", "intwentydata.json");
        }


        [HttpPost("/Model/API/UploadData")]
        public async Task<JsonResult> UploadData(IFormFile file, bool delete)
        {
            var result = new OperationResult();

            try
            {
                int savefail = 0;
                int savesuccess= 0;
                string fileContents;
                using (var stream = file.OpenReadStream())
                using (var reader = new StreamReader(stream))
                {
                    fileContents = await reader.ReadToEndAsync();
                }

                var apps = ModelRepository.GetApplicationModels();
                var json = System.Text.Json.JsonDocument.Parse(fileContents).RootElement;
                var rootobject = json.EnumerateObject();
                foreach (var attr in rootobject)
                {
                    if (attr.Value.ValueKind == System.Text.Json.JsonValueKind.Array)
                    {
                       
                        var jsonarr = attr.Value.EnumerateArray();
                        foreach (var rec in jsonarr)
                        {
                            ApplicationModel app = null;
                            var istatobject = rec.EnumerateObject();
                            foreach (var istat in istatobject)
                            {
                                if (istat.Name == "ApplicationId")
                                    app = apps.Find(p => p.Application.Id == istat.Value.GetInt32());

                                if (istat.Name == "ApplicationData" && app!=null)
                                {
                                    var state = ClientStateInfo.CreateFromJSON(istat.Value);
                                    state.Id = 0;
                                    state.Version = 0;
                                    state.ApplicationId = app.Application.Id;
                                    state.AddUpdateProperty("ISIMPORT", "TRUE");
                                    if (state.HasData)
                                        state.Data.RemoveKeyValues();
                                    var saveresult = DataRepository.Save(state);
                                    if (saveresult.IsSuccess)
                                        savesuccess += 1;
                                    else
                                        savefail += 1;


                                }
                            }
                        }

                    }
                }

                if (savefail== 0)
                    result.SetSuccess(string.Format("Successfully imported {0} applications.", savesuccess));
                else
                    result.SetSuccess(string.Format("Successfully imported {0} applications. Failed to import {1} applications.", savesuccess, savefail));
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "An error occured when uploading a data file.");
                var jres = new JsonResult(result);
                jres.StatusCode = 500;
                return jres;
            }

            return new JsonResult(result);
        }


        #endregion

        #region Translations

      

        /// <summary>
        /// Get translations
        /// </summary>
        [HttpGet("/Model/API/GetTranslations")]
        public JsonResult GetTranslations()
        {
            var t = ModelRepository.GetTranslations();
            var res = new JsonResult(t);
            return res;

        }


        [HttpPost("/Model/API/SaveTranslations")]
        public JsonResult SaveTranslations([FromBody] List<TranslationModelItem> model)
        {
            try
            {
                ModelRepository.SaveTranslations(model);
            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError(ex.Message, "An error occured when saving translations.");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

            return GetTranslations();
        }


        [HttpPost("/Model/API/DeleteTranslation")]
        public JsonResult DeleteTranslation([FromBody] TranslationModelItem model)
        {
            try
            {
                ModelRepository.DeleteTranslation(model.Id);

            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError(ex.Message, "An error occured when deleting a translation.");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

            return GetTranslations();
        }

        #endregion

        #region Endpoints



        /// <summary>
        /// Get endpoints
        /// </summary>
        [HttpGet("/Model/API/GetEndpoints")]
        public JsonResult GetEndpoints()
        {
            var res = new EndpointManagementVm();

            res.Endpoints = new List<EndpointVm>();
            var endpoints = ModelRepository.GetEndpointModels();
            foreach (var ep in endpoints)
            {
                res.Endpoints.Add(EndpointVm.CreateEndpointVm(ep));
            }

            res.EndpointDataSourceTypes = new List<EndpointDataSourceType>();
            res.EndpointDataSourceTypes.Add(new EndpointDataSourceType() { id = "TABLEOPERATION", title = "Database table" });
            res.EndpointDataSourceTypes.Add(new EndpointDataSourceType() { id = "DATAVIEWOPERATION", title = "Intwenty Data View" });
            res.EndpointActions = ModelRepository.GetValueDomains().Where(p => p.DomainName == "ENDPOINT_TABLE_ACTION" || 
                                                                               p.DomainName == "ENDPOINT_DATAVIEW_ACTION").ToList();

            res.EndpointDataSources = new List<EndpointDataSource>();
            var apps = ModelRepository.GetApplicationModels();
            foreach (var a in apps)
            {
                res.EndpointDataSources.Add(new EndpointDataSource() { id = a.Application.MetaCode + "|" + a.Application.MetaCode, title = a.Application.DbName, type = "TABLEOPERATION" });
                foreach (var subtable in a.DataStructure.Where(p=> p.IsMetaTypeDataTable))
                {
                    res.EndpointDataSources.Add(new EndpointDataSource() { id = subtable.AppMetaCode + "|" + subtable.MetaCode, title = subtable.DbName, type = "TABLEOPERATION" });

                }
            }
            var dataviews = ModelRepository.GetDataViewModels();
            foreach (var dv in dataviews.Where(p=> p.IsMetaTypeDataView))
            {
                res.EndpointDataSources.Add(new EndpointDataSource() { id = dv.MetaCode, title = dv.Title, type = "DATAVIEWOPERATION" });
            }

            return new JsonResult(res);

        }

      


        [HttpPost("/Model/API/SaveEndpoints")]
        public JsonResult SaveEndpoints([FromBody] List<EndpointVm> model)
        {
            try
            {
                var list = new List<EndpointModelItem>();
                foreach (var m in model)
                    list.Add(EndpointVm.CreateEndpointModelItem(m));

                ModelRepository.SaveEndpointModels(list);
            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError(ex.Message, "An error occured when saving endpoints.");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

            return GetEndpoints();
        }


        [HttpPost("/Model/API/DeleteEndpoint")]
        public JsonResult DeleteEndpoint([FromBody] EndpointModelItem model)
        {
            try
            {
                ModelRepository.DeleteEndpointModel(model.Id);

            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError(ex.Message, "An error occured when deleting an endpoint.");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

            return GetEndpoints();
        }

        #endregion

        /// <summary>
        /// Get menu items
        /// </summary>
        [HttpGet("/Model/API/GetMenuModelItems")]
        public JsonResult GetMenuModelItems()
        {
            var t = ModelRepository.GetMenuModels();
            return new JsonResult(t.Where(p=> p.IsMetaTypeMenuItem));

        }

      

        /// <summary>
        /// Configure the database according to the model
        /// </summary>
        [HttpPost("/Model/API/RunDatabaseConfiguration")]
        public JsonResult RunDatabaseConfiguration()
        {
            var res = ModelRepository.ConfigureDatabase();
            return new JsonResult(res);
        }


    }
}
