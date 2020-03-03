using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Moley.Models;
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
        [HttpGet("/ApplicationInfo/GetApplicationTables/{applicationid}")]
        public JsonResult GetApplicationTables(int applicationid)
        {
            var t = Repository.GetApplicationMeta().Find(p => p.Application.Id == applicationid);
            var l = new List<MetaDataItemDto>();
            l.Add(new MetaDataItemDto("DATAVALUETABLE") {Id = 0, DbName = t.Application.MainTableName, Description = "Main table for " + t.Application.Title, Properties = "DEFAULTTABLE=TRUE" });
            l.AddRange(t.DataStructure.Where(p => p.IsMetaTypeDataValueTable));
            return new JsonResult(l);

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
        [HttpGet("/ApplicationInfo/GetApplicationTableColumns/{applicationid}")]
        public JsonResult GetApplicationTableColumns(int applicationid)
        {
            var t = Repository.GetApplicationMeta().Find(p => p.Application.Id == applicationid);
            var l = new List<MetaDataItemDto>();
            l.AddRange(t.DataStructure.Where(p => p.IsMetaTypeDataValue));
            return new JsonResult(l);

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

        /// <summary>
        /// Get meta data for application ui declarations for application with id
        /// </summary>
        /*
        [HttpGet("/ApplicationInfo/GetApplicationUIComponents/{applicationid}")]
        public JsonResult GetApplicationUIComponents(int applicationid)
        {
            var t = Repository.GetApplicationMeta().Find(p => p.Application.Id == applicationid);
            return new JsonResult(UIVm.GetInput(t));

        }*/

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
            
            return new JsonResult("");
        }
        
       


    }
}
