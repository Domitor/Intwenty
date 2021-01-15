using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Intwenty.Model.UIDesign;
using Intwenty.Model;
using Microsoft.AspNetCore.Authorization;
using Intwenty.Model.Dto;
using Intwenty.Entity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using Intwenty.Helpers;
using Intwenty.Interface;
using Intwenty.Areas.Identity.Models;
using Intwenty.Areas.Identity.Entity;
using Intwenty.Areas.Identity.Data;

namespace Intwenty.Controllers
{

    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Policy = "IntwentyModelAuthorizationPolicy")]
    public class ModelAPIController : Controller
    {
        public IIntwentyDataService DataRepository { get; }
        public IIntwentyModelService ModelRepository { get; }
        private IntwentyUserManager UserManager { get; }

        public ModelAPIController(IIntwentyDataService ms, IIntwentyModelService sr, IntwentyUserManager usermanager)
        {
            DataRepository = ms;
            ModelRepository = sr;
            UserManager = usermanager;
        }

        #region Systems

        /// <summary>
        /// Get endpoints
        /// </summary>
        [HttpGet("/Model/API/GetSystems")]
        public IActionResult GetSystems()
        {
            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

            var res = ModelRepository.GetAuthorizedSystemModels(User);
            return new JsonResult(res);

        }


        [HttpPost("/Model/API/SaveSystem")]
        public IActionResult SaveSystem([FromBody] SystemModelItem model)
        {
            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

            try
            {
                ModelRepository.SaveSystemModel(model);
            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError(ex.Message, "An error occured when saving a system.");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

            return GetSystems();
        }


        [HttpPost("/Model/API/DeleteSystem")]
        public IActionResult DeleteSystem([FromBody] SystemModelItem model)
        {
            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

            try
            {
               

            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError(ex.Message, "An error occured when deleting a system.");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

            return GetSystems();
        }

        #endregion

        #region Applicaion models

        /// <summary>
        /// Get full model data for application with id
        /// </summary>
        [HttpGet("/Model/API/GetApplication/{applicationid}")]
        public IActionResult GetApplication(int applicationid)
        {
            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

            var t = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == applicationid);
            return new JsonResult(t.Application);

        }

        /// <summary>
        /// Get model data for applications
        /// </summary>
        [HttpGet("/Model/API/GetApplications")]
        public IActionResult GetApplications()
        {
            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

            var t = ModelRepository.GetAuthorizedApplicationModels(User);
            return new JsonResult(t);

        }


        [HttpPost("/Model/API/Save")]
        public IActionResult Save([FromBody] ApplicationModelItem model)
        {
            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

            var res = ModelRepository.SaveAppModel(model);
            if (res.IsSuccess)
            {
                //var t = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == res.Id);
                //if (t!=null)
                //     UserManager.AddUpdateUserPermissionAsync(User, new IntwentyUserPermissionItem() { Read = true, MetaCode = t.Application.MetaCode, PermissionType = ApplicationModelItem.MetaTypeApplication, Title = t.Application.Title  });
            }

            return new JsonResult(res);
        }

        /// <summary>
        /// Delete model data for application
        /// </summary>
        [HttpPost("/Model/API/DeleteApplicationModel")]
        public IActionResult DeleteApplicationModel([FromBody] ApplicationModelItem model)
        {

            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

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
        public IActionResult SaveDatabaseModels([FromBody] DBVm model)
        {
            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

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
        public IActionResult DeleteDatabaseModel([FromBody] DatabaseTableColumnVm model)
        {
            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

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
        public IActionResult GetDatabaseModels(int applicationid)
        {
            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

            try
            {
                var t = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == applicationid);
                if (t == null)
                    throw new InvalidOperationException("ApplicationId missing when fetching application db meta");

                var dbvm = DatabaseModelCreator.GetDatabaseVm(t);
                dbvm.PropertyCollection = IntwentyRegistry.IntwentyProperties;
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
        public IActionResult GetIntwentyDataTypes()
        {
            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

            var t = new DatabaseModelItem();
            return new JsonResult(DatabaseModelItem.GetAvailableDataTypes());

        }

        /// <summary>
        /// Get model data for application tables for application with id
        /// </summary>
        [HttpGet("/Model/API/GetDatabaseTables/{applicationid}")]
        public IActionResult GetDatabaseTables(int applicationid)
        {
            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

            var t = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == applicationid);
            return new JsonResult(DatabaseModelCreator.GetDatabaseTableVm(t));

        }

        /// <summary>
        /// Get model data for application tables for application with id
        /// </summary>
        [HttpGet("/Model/API/GetListviewTable/{applicationid}")]
        public IActionResult GetListviewTable(int applicationid)
        {
            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

            var t = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == applicationid);
            return new JsonResult(DatabaseModelCreator.GetListViewTableVm(t));

        }


        #endregion

        #region Dataview Model

        /// <summary>
        /// Get meta data for data views
        /// </summary>
        [HttpGet("/Model/API/GetDataViewModels")]
        public IActionResult GetDataViewModels()
        {
            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

            var t = ModelRepository.GetDataViewModels();
            var model = DataViewModelCreator.GetDataViewModelVm(t);
            model.PropertyCollection = IntwentyRegistry.IntwentyProperties;
            var res = new JsonResult(model);
            return res;

        }


        [HttpPost("/Model/API/SaveDataViewModels")]
        public IActionResult SaveDataViewModels([FromBody] DataViewVm model)
        {

            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

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
        public IActionResult DeleteDataViewModel([FromBody] DataViewVm model)
        {

            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

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
        /// Get UI view model for application with id and the viewtype
        /// </summary>
        [HttpGet("/Model/API/GetApplicationUI/{applicationid}/{viewtype}")]
        public IActionResult GetApplicationUI(int applicationid, string viewtype)
        {
            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

            var t = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == applicationid);
            var model = UIModelCreator.GetUIVm(t, viewtype);
            return new JsonResult(model);

        }

        [HttpPost("/Model/API/SaveUserInterfaceModel")]
        public IActionResult SaveUserInterfaceModel([FromBody] ViewVm model)
        {
            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

            try
            {
                if (model.ApplicationId < 1)
                    throw new InvalidOperationException("ApplicationId missing in model");


                var views = ModelRepository.GetDataViewModels();
                var app = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == model.ApplicationId);
                if (app == null)
                    throw new InvalidOperationException("Could not find application");

                var dtolist = UIModelCreator.GetUIModel(model, app, views);

                ModelRepository.SaveUserInterfaceModels(dtolist);

                var t = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == app.Application.Id);
                var vm = UIModelCreator.GetUIVm(t, model.ViewType);
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

        [HttpPost("/Model/API/SaveListViewModel")]
        public IActionResult SaveListViewModel([FromBody] ViewVm model)
        {
            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

            try
            {

                if (model.ApplicationId < 1)
                    throw new InvalidOperationException("ApplicationId is missing in model when saving listview");

                var app = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == model.ApplicationId);
                if (app == null)
                    throw new InvalidOperationException("Could not find application");

                var dtolist = UIModelCreator.GetListViewUIModel(model, app);
                ModelRepository.SaveUserInterfaceModels(dtolist);

                var t = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == app.Application.Id);
                var vm = UIModelCreator.GetUIVm(t, model.ViewType);
                return new JsonResult(vm);

            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError(ex.Message, "An error occured when saving application dataview meta data.");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

            
        }

        #endregion

        #region Value Domains


        /// <summary>
        /// Get meta data for application ui declarations for application with id
        /// </summary>
        [HttpGet("/Model/API/GetValueDomains")]
        public IActionResult GetValueDomains()
        {
            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

            var t = ModelRepository.GetValueDomains();
            return new JsonResult(t);

        }

        /// <summary>
        /// Get meta data for application ui declarations for application with id
        /// </summary>
        [HttpGet("/Model/API/GetValueDomainNames")]
        public IActionResult GetValueDomainNames()
        {
            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

            var t = ModelRepository.GetValueDomains();
            return new JsonResult(t.Select(p => p.DomainName).Distinct());

        }

        [HttpPost("/Model/API/SaveValueDomains")]
        public IActionResult SaveValueDomains([FromBody] List<ValueDomainModelItem> model)
        {
            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

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
        public IActionResult RemoveValueDomain([FromBody] ValueDomainModelItem model)
        {
            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

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
            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SUPERADMIN"))
                return Forbid();

            var t = ModelRepository.GetExportModel();
            var json = System.Text.Json.JsonSerializer.Serialize(t);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
            return File(bytes, "application/json", "intwentymodel.json");
        }

        [HttpPost("/Model/API/UploadModel")]
        public async Task<IActionResult> UploadModel(IFormFile file, bool delete)
        {
            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SUPERADMIN"))
                return Forbid();

            var result = new OperationResult();

            try
            {
                string fileContents;
                using (var stream = file.OpenReadStream())
                using (var reader = new StreamReader(stream))
                {
                    fileContents = await reader.ReadToEndAsync();
                }
                var model = System.Text.Json.JsonSerializer.Deserialize<ExportModel>(fileContents);
                model.DeleteCurrentModel = delete;
                result = ModelRepository.ImportModel(model);
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
            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SUPERADMIN"))
                return Forbid();

            var client = DataRepository.GetDataClient();
            var data = new StringBuilder("{\"IntwentyData\":[");

            try
            {
                var apps = ModelRepository.GetAuthorizedApplicationModels(User);
                var sep = "";
                foreach (var app in apps)
                {
                    client.Open();
                    var infostatuslist = client.GetEntities<Entity.InformationStatus>().Where(p => p.ApplicationId == app.Id);
                    client.Close();

                    foreach (var istat in infostatuslist)
                    {
                        data.Append(sep + "{");
                        data.Append(DBHelpers.GetJSONValue("ApplicationId", app.Id));
                        data.Append("," + DBHelpers.GetJSONValue("AppMetaCode", app.MetaCode));
                        data.Append("," + DBHelpers.GetJSONValue("DbName", app.DbName));
                        data.Append("," + DBHelpers.GetJSONValue("Id", istat.Id));
                        data.Append("," + DBHelpers.GetJSONValue("Version", istat.Version));
                        data.Append(",\"ApplicationData\":");

                        var state = new ClientStateInfo() { ApplicationId = app.Id, Id = istat.Id, Version = istat.Version };
                        var appversiondata = DataRepository.Get(state);
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
            catch 
            { 
            
            }
            finally 
            {
                client.Close();
            }

            return new JsonResult("");
            
        }


        [HttpPost("/Model/API/UploadData")]
        public async Task<IActionResult> UploadData(IFormFile file, bool delete)
        {

            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SUPERADMIN"))
                return Forbid();

            var result = new OperationResult();

            try
            {
                var authapps = ModelRepository.GetAuthorizedApplicationModels(User);
                int savefail = 0;
                int savesuccess= 0;
                string fileContents;
                using (var stream = file.OpenReadStream())
                using (var reader = new StreamReader(stream))
                {
                    fileContents = await reader.ReadToEndAsync();
                }

                var json = System.Text.Json.JsonDocument.Parse(fileContents).RootElement;
                var rootobject = json.EnumerateObject();
                foreach (var attr in rootobject)
                {
                    if (attr.Value.ValueKind == System.Text.Json.JsonValueKind.Array)
                    {
                       
                        var jsonarr = attr.Value.EnumerateArray();
                        foreach (var rec in jsonarr)
                        {
                            ApplicationModelItem app = null;
                            var istatobject = rec.EnumerateObject();
                            foreach (var istat in istatobject)
                            {
                                if (istat.Name == "ApplicationId")
                                    app = authapps.Find(p => p.Id == istat.Value.GetInt32());

                                if (istat.Name == "ApplicationData" && app!=null)
                                {
                                    var state = ClientStateInfo.CreateFromJSON(istat.Value);
                                    state.Id = 0;
                                    state.Version = 0;
                                    state.ApplicationId = app.Id;
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
        /// Get translations for the Application and UI model
        /// </summary>
        [HttpGet("/Model/API/GetModelTranslations")]
        public IActionResult GetModelTranslations()
        {

            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

            var res = new List<TranslationVm>();
            var translations = ModelRepository.GetTranslations();
            var apps = ModelRepository.GetApplicationModels();
            var langs = ModelRepository.Settings.SupportedLanguages;
            var metatypes = IntwentyRegistry.IntwentyMetaTypes;

            foreach (var a in apps)
            {
                if (string.IsNullOrEmpty(a.Application.TitleLocalizationKey))
                {
                    var key = "APP_LOC_" + BaseModelItem.GetQuiteUniqueString();
                    foreach (var l in langs)
                    {
                        res.Add(new TranslationVm() { ApplicationModelId = a.Application.Id, Culture = l.Culture, Key = key, ModelTitle = "Application: " + a.Application.Title, Text = "" });
                    }
                }
                else
                {
                    var trans = translations.FindAll(p => p.Key == a.Application.TitleLocalizationKey);
                    foreach (var l in langs)
                    {
                        var ct = trans.Find(p => p.Culture == l.Culture);
                        if (ct != null)
                            res.Add(new TranslationVm() { Culture = ct.Culture, Key = a.Application.TitleLocalizationKey, ModelTitle = "Application: " + a.Application.Title, Text = ct.Text, Id = ct.Id });
                        else
                            res.Add(new TranslationVm() { Culture = l.Culture, Key = a.Application.TitleLocalizationKey, ModelTitle = "Application: " + a.Application.Title, Text = "" });
                    }

                    foreach (var ct in trans.Where(p=> !langs.Exists(x=> x.Culture == p.Culture)))
                    {
                        res.Add(new TranslationVm() { Culture = ct.Culture, Key = ct.Key, ModelTitle = "Application: " + a.Application.Title, Text = ct.Text, Id = ct.Id });
                    }

                }

                foreach (var ui in a.UIStructure)
                {
                    var title = "";
                    var type = metatypes.Find(p => p.ModelCode == "UIMODEL" && p.Code == ui.MetaType);
                    title = "Application: " + a.Application.Title;
                    title += ", " + ui.Title;
                    if (type != null)
                        title += " ("+type.Title+")";

                    if (string.IsNullOrEmpty(ui.TitleLocalizationKey))
                    {
                        var uikey = "UI_LOC_" + BaseModelItem.GetQuiteUniqueString();
                        foreach (var l in langs)
                        {
                            res.Add(new TranslationVm() { UserInterfaceModelId = ui.Id, Culture = l.Culture, Key = uikey, ModelTitle = title, Text = "" });
                        }
                    }
                    else
                    {
                        var trans = translations.FindAll(p => p.Key == ui.TitleLocalizationKey);
                        foreach (var l in langs)
                        {
                            var ct = trans.Find(p => p.Culture == l.Culture);
                            if (ct != null)
                                res.Add(new TranslationVm() { Culture = ct.Culture, Key = ui.TitleLocalizationKey, ModelTitle = title, Text = ct.Text, Id = ct.Id });
                            else
                                res.Add(new TranslationVm() { Culture = l.Culture, Key = ui.TitleLocalizationKey, ModelTitle = title, Text = "" });
                        }

                        foreach (var ct in trans.Where(p => !langs.Exists(x => x.Culture == p.Culture)))
                        {
                            res.Add(new TranslationVm() { Culture = ct.Culture, Key = ct.Key, ModelTitle = title, Text = ct.Text, Id = ct.Id });
                        }

                    }

                }

            }

            var model = new TranslationManagementVm();
            model.Translations = res;
            return new JsonResult(model);


        }

            /// <summary>
            /// Get translations, that is not used by a model
            /// </summary>
            [HttpGet("/Model/API/GetNonModelTranslations")]
        public IActionResult GetNonModelTranslations()
        {

            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();


            var res = new List<TranslationVm>();
            var translations = ModelRepository.GetTranslations();
            var apps = ModelRepository.GetApplicationModels();

            foreach (var t in translations)
            {
                var ismodeltrans = false;
                foreach (var a in apps)
                {
                    if (!string.IsNullOrEmpty(a.Application.TitleLocalizationKey))
                    {
                        if (a.Application.TitleLocalizationKey == t.Key)
                            ismodeltrans = true;
                    }

                    if (a.UIStructure.Exists(p => !string.IsNullOrEmpty(p.TitleLocalizationKey) && p.TitleLocalizationKey == t.Key))
                    {
                        ismodeltrans = true;
                    }

                }

                if (!ismodeltrans)
                    res.Add(new TranslationVm() { Culture = t.Culture, Key = t.Key, ModelTitle = t.Key, Text = t.Text, Id = t.Id });



            }


            var model = new TranslationManagementVm();
            model.Translations = res;
            return new JsonResult(model);

        }


        [HttpPost("/Model/API/SaveModelTranslations")]
        public IActionResult SaveModelTranslations([FromBody] TranslationManagementVm model)
        {

            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

            try
            {
                foreach (var t in model.Translations)
                {
                    if (t.ApplicationModelId > 0 && !string.IsNullOrEmpty(t.Text))
                    {
                        ModelRepository.SetAppModelLocalizationKey(t.ApplicationModelId, t.Key);
                    }
                    else if (t.UserInterfaceModelId > 0 && !string.IsNullOrEmpty(t.Text))
                    {
                        ModelRepository.SetUserInterfaceModelLocalizationKey(t.UserInterfaceModelId, t.Key);
                    }
                }
                
                ModelRepository.SaveTranslations(model.Translations.Where(p=> p.Changed).Select(p=> TranslationVm.CreateTranslationModelItem(p)).ToList());
            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError(ex.Message, "An error occured when saving model translations.");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

            return GetModelTranslations();
        }

        [HttpPost("/Model/API/SaveNonModelTranslations")]
        public IActionResult SaveNonModelTranslations([FromBody] TranslationManagementVm model)
        {

            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

            try
            {

                ModelRepository.SaveTranslations(model.Translations.Where(p => p.Changed).Select(p => TranslationVm.CreateTranslationModelItem(p)).ToList());
            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError(ex.Message, "An error occured when saving model translations.");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

            return GetNonModelTranslations();
        }


        [HttpPost("/Model/API/DeleteModelTranslation")]
        public IActionResult DeleteModelTranslation([FromBody] TranslationVm model)
        {

            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

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

            return GetModelTranslations();
        }

        [HttpPost("/Model/API/DeleteNonModelTranslation")]
        public IActionResult DeleteNonModelTranslation([FromBody] TranslationVm model)
        {

            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

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

            return GetNonModelTranslations();
        }

        #endregion

        #region Endpoints

        /// <summary>
        /// Get endpoints
        /// </summary>
        [HttpGet("/Model/API/GetEndpoints")]
        public IActionResult GetEndpoints()
        {

            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

            var res = new EndpointManagementVm();
            res.Systems = ModelRepository.GetSystemModels();
            res.Endpoints = new List<EndpointVm>();

            var endpoints = ModelRepository.GetEndpointModels();
            foreach (var ep in endpoints)
            {
                res.Endpoints.Add(EndpointVm.CreateEndpointVm(ep));
            }

           

            res.EndpointDataSources = new List<EndpointDataSource>();
            var apps = ModelRepository.GetApplicationModels();
            foreach (var a in apps)
            {
                res.EndpointDataSources.Add(new EndpointDataSource() { id = a.Application.MetaCode + "|" + a.Application.MetaCode, title = a.Application.DbName, type = "TABLE" });
                foreach (var subtable in a.DataStructure.Where(p=> p.IsMetaTypeDataTable))
                {
                    res.EndpointDataSources.Add(new EndpointDataSource() { id = subtable.AppMetaCode + "|" + subtable.MetaCode, title = subtable.DbName, type = "TABLE" });

                }
            }
            var dataviews = ModelRepository.GetDataViewModels();
            foreach (var dv in dataviews.Where(p=> p.IsMetaTypeDataView))
            {
                res.EndpointDataSources.Add(new EndpointDataSource() { id = dv.MetaCode, title = dv.Title, type = "DATAVIEW" });
            }

            

            return new JsonResult(res);

        }

      


        [HttpPost("/Model/API/SaveEndpoints")]
        public IActionResult SaveEndpoints([FromBody] List<EndpointVm> model)
        {

            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

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
        public IActionResult DeleteEndpoint([FromBody] EndpointModelItem model)
        {

            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

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

        #region User Permission

        [HttpGet("Model/API/GetUserPermissions/{id}")]
        public IActionResult GetUserPermissions(string id)
        {
            return new JsonResult(new IntwentyUserPermissionVm());

            /*
            if (!User.Identity.IsAuthenticated)
                new JsonResult(new IntwentyUserPermissionVm());

            var adminuser = UserManager.GetUserAsync(User).Result;
            if (adminuser == null)
                new JsonResult(new IntwentyUserPermissionVm());

            if (!User.IsInRole("SUPERADMIN") && !User.IsInRole("USERADMIN"))
            {
                new JsonResult(new IntwentyUserPermissionVm());
            }

            var admin_permissions = UserManager.GetUserPermissions(adminuser).Result;

            var client = DataRepository.GetDataClient();
            client.Open();
            var user = client.GetEntity<IntwentyUser>(id);
            client.Close();

            if (user == null)
                new JsonResult(new IntwentyUserPermissionVm());

            var user_permissions = UserManager.GetUserPermissions(user).Result;
            var user_roles = UserManager.GetRolesAsync(user).Result;

            var res = new IntwentyUserPermissionVm();
            res.Id = user.Id;
            res.UserName = user.UserName;

            res.UserSystemPermissions.AddRange(user_permissions.Where(p => p.IsSystemPermission));
            res.UserApplicationPermissions.AddRange(user_permissions.Where(p => p.IsApplicationPermission));

            foreach (var role in user_roles)
            {
                res.UserRoles.Add(new IntwentyUserRoleItem() { RoleName = role });
            }

            res.Roles.AddRange(new IntwentyUserRoleItem[]
            {
                new IntwentyUserRoleItem() { RoleName="APIUSER" }
               ,new IntwentyUserRoleItem() { RoleName="USER" }
            });

         
            if (!User.IsInRole("SUPERADMIN"))
            {
                var apps = ModelRepository.GetAuthorizedApplicationModels(User);
                foreach (var a in apps)
                {
                    if (a.SystemInfo == null)
                        continue;

                    res.ApplicationPermissions.Add(IntwentyUserPermissionItem.CreateApplicationPermission(a.MetaCode, a.Title));
                    if (!res.SystemPermissions.Exists(p => p.IsSystemPermission && p.MetaCode == a.SystemInfo.MetaCode))
                        res.SystemPermissions.Add(IntwentyUserPermissionItem.CreateSystemPermission(a.SystemInfo.MetaCode, a.SystemInfo.Title));

                }
            }
            else
            {
                res.Roles.Add(new IntwentyUserRoleItem() { RoleName = "USERADMIN" });
                res.Roles.Add(new IntwentyUserRoleItem() { RoleName = "SYSTEMADMIN" });
                res.Roles.Add(new IntwentyUserRoleItem() { RoleName = "SUPERADMIN" });

                var apps = ModelRepository.GetAppModels();
                foreach (var a in apps)
                {
                    if (a.SystemInfo == null)
                        continue;

                    res.ApplicationPermissions.Add(IntwentyUserPermissionItem.CreateApplicationPermission(a.MetaCode, a.Title));
                    if (!res.SystemPermissions.Exists(p => p.IsSystemPermission && p.MetaCode == a.SystemInfo.MetaCode))
                         res.SystemPermissions.Add(IntwentyUserPermissionItem.CreateSystemPermission(a.SystemInfo.MetaCode, a.SystemInfo.Title));

                }

            }

            return new JsonResult(res);

            */
        }

        [HttpPost("Model/API/SaveUserPermissions")]
        public IActionResult SaveUserPermissions([FromBody] IntwentyUserPermissionVm model)
        {
            return new JsonResult("{}");

            /*
            if (!User.Identity.IsAuthenticated)
                new JsonResult(new IntwentyUserPermissionVm());

            if (!User.IsInRole("SUPERADMIN") && !User.IsInRole("USERADMIN"))
            {
                new JsonResult(new IntwentyUserPermissionVm());
            }

            var client = DataRepository.GetDataClient();
            client.Open();
            var user = client.GetEntity<IntwentyUser>(model.Id);
            client.Close();

            if (user == null)
                return BadRequest();

            var permissions = UserManager.GetUserPermissions(user).Result;
            var roles = UserManager.GetRolesAsync(user).Result.ToList();

            foreach (var r in model.UserRoles)
            {
                UserManager.AddToRoleAsync(user, r.RoleName);
            }



            foreach (var r in model.UserSystemPermissions)
            {
                UserManager.AddUpdateUserPermissionAsync(user, r);
            }


            foreach (var r in model.UserApplicationPermissions)
            {
                UserManager.AddUpdateUserPermissionAsync(user, r);
            }


            return GetUserPermissions(model.Id);
            */
        }

        [HttpPost("Model/API/DeleteUserPermission")]
        public IActionResult DeleteUserPermission([FromBody] IntwentyUserPermissionItem model)
        {
            return new JsonResult("{}");

            /*
            if (!User.Identity.IsAuthenticated)
                new JsonResult(new IntwentyUserPermissionVm());

            if (!User.IsInRole("SUPERADMIN") && !User.IsInRole("USERADMIN"))
            {
                new JsonResult(new IntwentyUserPermissionVm());
            }

            var client = DataRepository.GetDataClient();
            client.Open();
            var user = client.GetEntity<IntwentyUser>(model.UserId);
            client.Close();

            if (user == null)
                return BadRequest();

            if (model.PermissionType == "ROLE")
            {
                UserManager.RemoveFromRoleAsync(user, model.MetaCode);
            }
            else
            {
                UserManager.RemoveUserPermissionAsync(user, model);

            }

            return GetUserPermissions(user.Id);

            */
        }


        #endregion

        #region Tools
        /// <summary>
        /// Configure the database according to the model
        /// </summary>
        [HttpPost("/Model/API/RunDatabaseConfiguration")]
        public IActionResult RunDatabaseConfiguration()
        {
            if (!User.Identity.IsAuthenticated)
                return Forbid();
            if (!User.IsInRole("SYSTEMADMIN") && !User.IsInRole("SUPERADMIN"))
                return Forbid();

            var res = ModelRepository.ConfigureDatabase();
            return new JsonResult(res);
        }

        #endregion


    }
}
