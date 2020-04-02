using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Intwenty.Model.DesignerVM;
using Intwenty.Data;
using Intwenty;
using Intwenty.Model;
using Microsoft.AspNetCore.Authorization;
using Intwenty.Data.Dto;

namespace Intwenty.Controllers
{
    
    [Authorize(Roles="Administrator")]
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
            var res = DataRepository.ValidateModel();
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


        /*********************  API ***********************************************************/

        /// <summary>
        /// Configure the database according to the model
        /// </summary>
        [HttpPost]
        public JsonResult RunDatabaseConfiguration()
        {
            var res = DataRepository.ConfigureDatabase();
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
        [HttpGet("/Model/GetNoSeries")]
        public JsonResult GetNoSeries()
        {
            var t = ModelRepository.GetNoSeries();
            return new JsonResult(NoSeriesVmCreator.GetNoSeriesVm(t));

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
