using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Moley.Models;
using Moley.Models.MetaDesigner;
using Moley.Data;
using Moley.Data.Dto;
using Moley.MetaDataService;

namespace Moley.Controllers
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

        /// <summary>
        /// Loads meta data for all applications in this instance
        /// </summary>
        [HttpPost]
        public JsonResult GetListView([FromBody] ListRetrivalArgs model)
        {
            var t = Repository.GetApplicationMeta();
            var listdata = t.Select(p => p.Application).ToList();
            return new JsonResult(listdata);
        }

        [HttpPost]
        public JsonResult Save([FromBody] ApplicationDescriptionDto model)
        {
            var res = Repository.SaveApplication(model);
            return new JsonResult(res);
        }


        public IActionResult ValidateModel()
        {
            var apps = Repository.GetApplicationMeta();
            var views = Repository.GetMetaDataViews();
            var res = MetaServer.ValidateModel(apps, views);
            return View(res);
        }

      

        public IActionResult ConfigureDatabase()
        {
            var res = new List<OperationResult>();
            var l = Repository.GetApplicationMeta();
            foreach (var t in l)
            {
                res.Add(MetaServer.ConfigureDatabase(t));
            }

            return View(res);
        }

        public IActionResult ViewMetaModelDocumentation()
        {
            var l = Repository.GetApplicationMeta();
            return View(l);
        }

        public IActionResult GenerateTestData()
        {
            var l = Repository.GetApplicationMeta();
            var res = MetaServer.GenerateTestData(l, Repository);
            return View(res);
        }


        /*********************  API ***********************************************************/


        /// <summary>
        /// Get meta data for application with id
        /// </summary>
        [HttpGet("/ApplicationInfo/GetApplication/{applicationid}")]
        public JsonResult GetApplication(int applicationid)
        {
            var t = Repository.GetApplicationMeta().Find(p => p.Application.Id == applicationid);
            return new JsonResult(t.Application);

        }

        /// <summary>
        /// Get meta data for application with id
        /// </summary>
        [HttpGet("/ApplicationInfo/GetApplicationUI/{applicationid}")]
        public JsonResult GetApplicationUI(int applicationid)
        {
            var t = Repository.GetApplicationMeta().Find(p => p.Application.Id == applicationid);
            return new JsonResult(UIVmCreator.GetUIVm(t));

        }

        /// <summary>
        /// Get meta data for application tables for application with id
        /// </summary>
        [HttpGet("/ApplicationInfo/GetApplicationDB/{applicationid}")]
        public JsonResult GetApplicationDB(int applicationid)
        {

            try
            {
                var t = Repository.GetApplicationMeta().Find(p => p.Application.Id == applicationid);
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
            var t = Repository.GetApplicationMeta().Find(p => p.Application.Id == applicationid);
            return new JsonResult(ListViewVm.GetListView(t));

        }


        /// <summary>
        /// Get meta data for available datatypes
        /// </summary>
        [HttpGet("/ApplicationInfo/GetApplicationTableDataTypes")]
        public JsonResult GetApplicationTableDataTypes()
        {
            var t = new MetaDataItemDto();
            return new JsonResult(t.DataTypes);

        }


        /// <summary>
        /// Get meta data for application tables for application with id
        /// </summary>
        [HttpGet("/ApplicationInfo/GetApplicationListOfDatabaseTables/{applicationid}")]
        public JsonResult GetApplicationListOfDatabaseTables(int applicationid)
        {
            var t = Repository.GetApplicationMeta().Find(p => p.Application.Id == applicationid);
            return new JsonResult(UIDbTable.GetTables(t));

        }

        /// <summary>
        /// Get meta data for data views
        /// </summary>
        [HttpGet("/ApplicationInfo/GetDataViews")]
        public JsonResult GetDataViews()
        {
            var t = Repository.GetMetaDataViews();
            return new JsonResult(DataViewVm.GetDataViews(t));

        }

        /// <summary>
        /// Get meta data for application ui declarations for application with id
        /// </summary>
        [HttpGet("/ApplicationInfo/GetValueDomains")]
        public JsonResult GetValueDomains()
        {
            var t = Repository.GetValueDomains();
            return new JsonResult(t.Select(p=> p.DomainName).Distinct());

        }

      

        [HttpPost]
        public JsonResult SaveApplicationUI([FromBody] UIVm model)
        {
            if (model.Id < 1)
                throw new InvalidOperationException("ApplicationId missing in model");


            var views = Repository.GetMetaDataViews();
            var app = Repository.GetApplicationMeta().Find(p => p.Application.Id == model.Id);
            if (app == null)
                throw new InvalidOperationException("Could not find application");

            var dtolist = MetaUIItemCreator.GetMetaUI(model, app, views);

            Repository.SaveApplicationUI(dtolist);

            return GetApplicationUI(model.Id);
        }

        [HttpPost]
        public JsonResult RemoveApplicationUI([FromBody] UserInput model)
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

                var app = Repository.GetApplicationMeta().Find(p => p.Application.Id == model.ApplicationId);
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
