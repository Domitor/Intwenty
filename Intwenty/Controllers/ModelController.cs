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
using Intwenty.Data.DBAccess.Helpers;

namespace Intwenty.Controllers
{

    [Authorize(Policy = "IntwentyModelAuthorizationPolicy")]
    public class ModelController : Controller
    {
        public IIntwentyDataService DataRepository { get; }
        public IIntwentyModelService ModelRepository { get; }

        public ModelController(IIntwentyDataService ms, IIntwentyModelService sr)
        {
            DataRepository = ms;
            ModelRepository = sr;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpGet("/Model/Edit/{applicationid}")]
        public IActionResult Edit(int applicationid)
        {
            ViewBag.SystemId = Convert.ToString(applicationid);
            return View();
        }

        [HttpGet("/Model/EditDB/{applicationid}")]
        public IActionResult EditDB(int applicationid)
        {
            ViewBag.SystemId = Convert.ToString(applicationid);
            return View();
        }

        [HttpGet("/Model/EditUI/{applicationid}")]
        public IActionResult EditUI(int applicationid)
        {
            ViewBag.SystemId = Convert.ToString(applicationid);
            return View();
        }

        public IActionResult EditDataviews()
        {
            return View();
        }

        public IActionResult EditValueDomains()
        {
            return View();
        }

        [Obsolete]
        public IActionResult EditNoSeries()
        {
            return View();
        }

        public IActionResult EditMainMenu()
        {
            return View();
        }



        public IActionResult GetList()
        {
            return View();
        }

      

        [HttpPost]
        public JsonResult Save([FromBody] ApplicationModelItem model)
        {
            var res = ModelRepository.SaveApplication(model);
            return new JsonResult(res);
        }


        public IActionResult ToolConfigureDatabase()
        {
            return View();
        }

        public IActionResult ToolValidateModel()
        {
            var res = ModelRepository.ValidateModel();
            return View(res);
        }

        public IActionResult ToolModelDocumentation()
        {
            var l = ModelRepository.GetApplicationModels();
            return View(l);
        }

        public IActionResult ToolGenerateTestData()
        {
            return View();
        }

        public IActionResult ImportModel()
        {
            return View();
        }

        public IActionResult ImportData()
        {
            return View();
        }


        /*********************  API ***********************************************************/

        /// <summary>
        /// Configure the database according to the model
        /// </summary>
        [HttpPost]
        public JsonResult RunDatabaseConfiguration()
        {
            var res = ModelRepository.ConfigureDatabase();
            return new JsonResult(res); 
        }

        /// <summary>
        /// Creates test data for all applications in the model
        /// </summary>
        [HttpPost]
        public JsonResult GenerateTestData()
        {
            var res = DataRepository.GenerateTestData();
            return new JsonResult(res);
        }

        /// <summary>
        /// Get a list of testdata batch names
        /// </summary>
        [HttpGet("/Model/GetTestDataBatches")]
        public JsonResult GetTestDataBatches()
        {
            var t = ModelRepository.GetTestDataBatches();
            return new JsonResult(t);

        }

        /// <summary>
        /// Delete testdata included in the batch
        /// </summary>
        [HttpPost]
        public JsonResult DeleteTestDataBatch([FromBody] string batchname)
        {
            
            try
            {
                ModelRepository.DeleteTestDataBatch(batchname);
            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError(ex.Message, "An error occured when deleting a testdata batch.");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }
         
            return GetTestDataBatches();

        }

        /// <summary>
        /// Get model data for applications
        /// </summary>
        [HttpGet("/Model/GetApplications")]
        public JsonResult GetApplications()
        {
            var t = ModelRepository.GetApplicationModelItems();
            return new JsonResult(t);

        }

        /// <summary>
        /// Get full model data for application with id
        /// </summary>
        [HttpGet("/Model/GetApplication/{applicationid}")]
        public JsonResult GetApplication(int applicationid)
        {
            var t = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == applicationid);
            return new JsonResult(t.Application);

        }

        /// <summary>
        /// Delete model data for application
        /// </summary>
        [HttpPost]
        public JsonResult DeleteApplicationModel([FromBody] ApplicationModelItem model)
        {
            try
            {
                ModelRepository.DeleteApplicationModel(model);
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

        /// <summary>
        /// Get UI model for application with id
        /// </summary>
        [HttpGet("/Model/GetApplicationUI/{applicationid}")]
        public JsonResult GetApplicationUI(int applicationid)
        {
            var t = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == applicationid);
            return new JsonResult(UIModelCreator.GetUIVm(t));

        }

        /// <summary>
        /// Get database model for application tables for application with id
        /// </summary>
        [HttpGet("/Model/GetApplicationDB/{applicationid}")]
        public JsonResult GetApplicationDB(int applicationid)
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
        /// Get meta data for application with id
        /// </summary>
        [HttpGet("/Model/GetApplicationListView/{applicationid}")]
        public JsonResult GetApplicationListView(int applicationid)
        {
            var t = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == applicationid);
            return new JsonResult(ListViewVm.GetListView(t));

        }


        /// <summary>
        /// Get meta data for available datatypes
        /// </summary>
        [HttpGet("/Model/GetApplicationTableDataTypes")]
        public JsonResult GetApplicationTableDataTypes()
        {
            var t = new DatabaseModelItem();
            return new JsonResult(DatabaseModelItem.DataTypes);

        }

        /// <summary>
        /// Get model data for all application tables
        /// </summary>
        [HttpGet("/Model/GetListOfDatabaseTables/")]
        public JsonResult GetListOfDatabaseTables()
        {
            var res = new List<DatabaseTableVm>();
            var apps = ModelRepository.GetApplicationModels();
            foreach (var t in apps)
            {
                res.AddRange(DatabaseModelCreator.GetDatabaseTableVm(t));
            }
            return new JsonResult(res);

        }


        /// <summary>
        /// Get model data for application tables for application with id
        /// </summary>
        [HttpGet("/Model/GetApplicationListOfDatabaseTables/{applicationid}")]
        public JsonResult GetApplicationListOfDatabaseTables(int applicationid)
        {
            var t = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == applicationid);
            return new JsonResult(DatabaseModelCreator.GetDatabaseTableVm(t));

        }

        /// <summary>
        /// Get meta data for data views
        /// </summary>
        [HttpGet("/Model/GetDataViews")]
        public JsonResult GetDataViews()
        {
            var t = ModelRepository.GetDataViewModels();
            var views = DataViewModelCreator.GetDataViewVm(t);
            var res = new JsonResult(views);
            return res;

        }

        /// <summary>
        /// Get meta data for application ui declarations for application with id
        /// </summary>
        [HttpGet("/Model/GetValueDomains")]
        public JsonResult GetValueDomains()
        {
            var t = ModelRepository.GetValueDomains();
            return new JsonResult(t);

        }

        /// <summary>
        /// Get meta data for application ui declarations for application with id
        /// </summary>
        [HttpGet("/Model/GetValueDomainNames")]
        public JsonResult GetValueDomainNames()
        {
            var t = ModelRepository.GetValueDomains();
            return new JsonResult(t.Select(p => p.DomainName).Distinct());

        }

        /// <summary>
        /// Create a json file containing the current model
        /// </summary>
        [HttpGet("/Model/ExportModel")]
        public IActionResult ExportModel()
        {
            var t = ModelRepository.GetSystemModel();
            var json = System.Text.Json.JsonSerializer.Serialize(t);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
            return File(bytes, "application/json", "intwentymodel.json");
        }

        [HttpPost]
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
        [HttpGet("/Application/ExportData")]
        public IActionResult ExportData()
        {
            var data = new StringBuilder("{\"IntwentyData\":[");
            var apps = ModelRepository.GetApplicationModels();
            var sep = "";
            foreach (var app in apps)
            {
                var client = DataRepository.GetDbObjectMapper();
                var infostatuslist = client.GetAll<InformationStatus>().Where(p => p.ApplicationId == app.Application.Id);

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


        [HttpPost]
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


        [HttpPost]
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


        [HttpPost]
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

        /// <summary>
        /// Get meta data for number series
        /// </summary>
        [Obsolete]
        [HttpGet("/Model/GetNoSeries")]
        public JsonResult GetNoSeries()
        {
            return new JsonResult(new List<NoSeriesVm>());

        }


        /// <summary>
        /// Get meta data for number series
        /// </summary>
        [HttpGet("/Model/GetMenuModelItems")]
        public JsonResult GetMenuModelItems()
        {
            var t = ModelRepository.GetMenuModelItems();
            return new JsonResult(t.Where(p=> p.IsMetaTypeMenuItem));

        }

        [HttpPost]
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

                ModelRepository.SaveUserInterfaceModel(dtolist);

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

      

        [HttpPost]
        public JsonResult SaveApplicationDB([FromBody] DBVm model)
        {
            try
            {
                if (model.Id < 1)
                    throw new InvalidOperationException("ApplicationId missing in model");

                var list = DatabaseModelCreator.GetDatabaseModel(model);

                ModelRepository.SaveApplicationDB(list, model.Id);

            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError(ex.Message, "An error occured when saving application database meta data.");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

            return GetApplicationDB(model.Id);
        }



        /// <summary>
        /// Removes one database object (column / table) from the UI meta data collection
        /// </summary>
        [HttpPost]
        public JsonResult RemoveFromApplicationDB([FromBody] DatabaseTableFieldVm model)
        {
            try
            {
                if (model.Id < 1)
                    throw new InvalidOperationException("Id is missing in model when removing db object");
                if (model.ApplicationId < 1)
                    throw new InvalidOperationException("ApplicationId is missing in model when removing db object");

                ModelRepository.DeleteApplicationDB(model.Id);

            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError(ex.Message, "An error occured when deleting application database meta data.");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

            return GetApplicationDB(model.ApplicationId);
        }

        [HttpPost]
        public JsonResult SaveDataView([FromBody] DataViewVm model)
        {
            try
            {

                var dtolist = DataViewModelCreator.GetDataViewModel(model);
                ModelRepository.SaveDataView(dtolist);

            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError(ex.Message, "An error occured when saving application dataview meta data.");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

            return GetDataViews();
        }

        [HttpPost]
        public JsonResult SaveApplicationListView([FromBody] ListViewVm model)
        {
            try
            {

                if (model.ApplicationId < 1)
                    throw new InvalidOperationException("ApplicationId is missing in model when saving listview");

                var app = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == model.ApplicationId);
                if (app == null)
                    throw new InvalidOperationException("Could not find application");

                var dtolist = MetaDataListViewCreator.GetMetaUIListView(model,app);
                ModelRepository.SaveUserInterfaceModel(dtolist);

            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError(ex.Message, "An error occured when saving application dataview meta data.");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

            return GetApplicationListView(model.ApplicationId);
        }




    }
}
