using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Intwenty.Models;
using Intwenty.Models.MetaDesigner;
using Intwenty.Data;
using Intwenty.MetaDataService;
using Intwenty.MetaDataService.Model;

namespace Intwenty.Controllers
{
    public class ApplicationInfoController : Controller
    {
        public IServiceEngine MetaServer { get; }
        public ISystemRepository Repository { get; }

        public ApplicationInfoController(IServiceEngine ms, ISystemRepository sr)
        {
            MetaServer = ms;
            Repository = sr;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpGet("/ApplicationInfo/Edit/{applicationid}")]
        public IActionResult Edit(int applicationid)
        {
            ViewBag.SystemId = Convert.ToString(applicationid);
            return View();
        }

        [HttpGet("/ApplicationInfo/EditDB/{applicationid}")]
        public IActionResult EditDB(int applicationid)
        {
            ViewBag.SystemId = Convert.ToString(applicationid);
            return View();
        }

        [HttpGet("/ApplicationInfo/EditUI/{applicationid}")]
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
            var res = Repository.SaveApplication(model);
            return new JsonResult(res);
        }


        public IActionResult ValidateModel()
        {
            var apps = Repository.GetApplicationModels();
            var views = Repository.GetMetaDataViews();
            var res = MetaServer.ValidateModel(apps, views);
            return View(res);
        }

      

        public IActionResult ConfigureDatabase()
        {
            var res = new List<OperationResult>();
            var l = Repository.GetApplicationModels();
            foreach (var t in l)
            {
                res.Add(MetaServer.ConfigureDatabase(t));
            }

            return View(res);
        }

        public IActionResult ViewMetaModelDocumentation()
        {
            var l = Repository.GetApplicationModels();
            return View(l);
        }

        public IActionResult GenerateTestData()
        {
            var l = Repository.GetApplicationModels();
            var res = MetaServer.GenerateTestData(l, Repository);
            return View(res);
        }


        /*********************  API ***********************************************************/

        /// <summary>
        /// Get model data for applications
        /// </summary>
        [HttpGet("/ApplicationInfo/GetApplications")]
        public JsonResult GetApplications()
        {
            var t = Repository.GetApplicationModelItems();
            return new JsonResult(t);

        }

        /// <summary>
        /// Get full model data for application with id
        /// </summary>
        [HttpGet("/ApplicationInfo/GetApplication/{applicationid}")]
        public JsonResult GetApplication(int applicationid)
        {
            var t = Repository.GetApplicationModels().Find(p => p.Application.Id == applicationid);
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
                Repository.DeleteApplicationModel(model);
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
        [HttpGet("/ApplicationInfo/GetApplicationUI/{applicationid}")]
        public JsonResult GetApplicationUI(int applicationid)
        {
            var t = Repository.GetApplicationModels().Find(p => p.Application.Id == applicationid);
            return new JsonResult(UIVmCreator.GetUIVm(t));

        }

        /// <summary>
        /// Get database model for application tables for application with id
        /// </summary>
        [HttpGet("/ApplicationInfo/GetApplicationDB/{applicationid}")]
        public JsonResult GetApplicationDB(int applicationid)
        {

            try
            {
                var t = Repository.GetApplicationModels().Find(p => p.Application.Id == applicationid);
                if (t == null)
                    throw new InvalidOperationException("ApplicationId missing when fetching application db meta");

                var dbvm = DBVmCreator.GetDBVm(t);
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
        [HttpGet("/ApplicationInfo/GetApplicationListView/{applicationid}")]
        public JsonResult GetApplicationListView(int applicationid)
        {
            var t = Repository.GetApplicationModels().Find(p => p.Application.Id == applicationid);
            return new JsonResult(ListViewVm.GetListView(t));

        }


        /// <summary>
        /// Get meta data for available datatypes
        /// </summary>
        [HttpGet("/ApplicationInfo/GetApplicationTableDataTypes")]
        public JsonResult GetApplicationTableDataTypes()
        {
            var t = new DatabaseModelItem();
            return new JsonResult(DatabaseModelItem.DataTypes);

        }

        /// <summary>
        /// Get meta data for all application tables
        /// </summary>
        [HttpGet("/ApplicationInfo/GetListOfDatabaseTables/")]
        public JsonResult GetListOfDatabaseTables()
        {
            var res = new List<UIDbTable>();
            var apps = Repository.GetApplicationModels();
            foreach (var t in apps)
            {
                res.AddRange(UIDbTable.GetTables(t));
            }
            return new JsonResult(res);

        }


        /// <summary>
        /// Get meta data for application tables for application with id
        /// </summary>
        [HttpGet("/ApplicationInfo/GetApplicationListOfDatabaseTables/{applicationid}")]
        public JsonResult GetApplicationListOfDatabaseTables(int applicationid)
        {
            var t = Repository.GetApplicationModels().Find(p => p.Application.Id == applicationid);
            return new JsonResult(UIDbTable.GetTables(t));

        }

        /// <summary>
        /// Get meta data for data views
        /// </summary>
        [HttpGet("/ApplicationInfo/GetDataViews")]
        public JsonResult GetDataViews()
        {
            var t = Repository.GetMetaDataViews();
            var views = DataViewVm.GetDataViews(t);
            var res = new JsonResult(views);
            return res;

        }

        /// <summary>
        /// Get meta data for application ui declarations for application with id
        /// </summary>
        [HttpGet("/ApplicationInfo/GetValueDomains")]
        public JsonResult GetValueDomains()
        {
            var t = Repository.GetValueDomains();
            return new JsonResult(t);

        }

        /// <summary>
        /// Get meta data for application ui declarations for application with id
        /// </summary>
        [HttpGet("/ApplicationInfo/GetValueDomainNames")]
        public JsonResult GetValueDomainNames()
        {
            var t = Repository.GetValueDomains();
            return new JsonResult(t.Select(p => p.DomainName).Distinct());

        }

        [HttpPost]
        public JsonResult SaveValueDomains([FromBody] List<ValueDomainModelItem> model)
        {
            try
            {
                Repository.SaveValueDomains(model);
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

                Repository.DeleteValueDomain(model.Id);
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
        [HttpGet("/ApplicationInfo/GetNoSeries")]
        public JsonResult GetNoSeries()
        {
            var t = Repository.GetNoSeries();
            return new JsonResult(NoSeriesVmCreator.GetNoSeriesVm(t));

        }


        /// <summary>
        /// Get meta data for number series
        /// </summary>
        [HttpGet("/ApplicationInfo/GetMenuModelItems")]
        public JsonResult GetMenuModelItems()
        {
            var t = Repository.GetMenuModelItems();
            return new JsonResult(t.Where(p=> p.IsMetaTypeMenuItem));

        }



        [HttpPost]
        public JsonResult SaveApplicationUI([FromBody] UIVm model)
        {
            if (model.Id < 1)
                throw new InvalidOperationException("ApplicationId missing in model");


            var views = Repository.GetMetaDataViews();
            var app = Repository.GetApplicationModels().Find(p => p.Application.Id == model.Id);
            if (app == null)
                throw new InvalidOperationException("Could not find application");

            var dtolist = MetaUIItemCreator.GetMetaUI(model, app, views);

            Repository.SaveApplicationUI(dtolist);

            return GetApplicationUI(model.Id);
        }

        /// <summary>
        /// Removes one input control from the UI meta data collection
        /// </summary>
        [HttpPost]
        public JsonResult RemoveFromApplicationUI([FromBody] UserInput model)
        {
            if (model.Id < 1)
                throw new InvalidOperationException("Id is missing in model when removing UI");
            if (model.ApplicationId < 1)
                throw new InvalidOperationException("ApplicationId is missing in model when removing UI");

            Repository.DeleteApplicationUI(model.Id);

            return GetApplicationUI(model.ApplicationId);
        }

        [HttpPost]
        public JsonResult SaveApplicationDB([FromBody] DBVm model)
        {
            try
            {
                if (model.Id < 1)
                    throw new InvalidOperationException("ApplicationId missing in model");

                var list = MetaDataItemCreator.GetMetaDataItems(model);

                Repository.SaveApplicationDB(list, model.Id);

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
        public JsonResult RemoveFromApplicationDB([FromBody] UIDbTableField model)
        {
            try
            {
                if (model.Id < 1)
                    throw new InvalidOperationException("Id is missing in model when removing db object");
                if (model.ApplicationId < 1)
                    throw new InvalidOperationException("ApplicationId is missing in model when removing db object");

                Repository.DeleteApplicationDB(model.Id);

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

                var dtolist = MataDataViewCreator.GetMetaDataView(model);
                Repository.SaveDataView(dtolist);

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

                var app = Repository.GetApplicationModels().Find(p => p.Application.Id == model.ApplicationId);
                if (app == null)
                    throw new InvalidOperationException("Could not find application");

                var dtolist = MetaDataListViewCreator.GetMetaUIListView(model,app);
                Repository.SaveApplicationUI(dtolist);

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
