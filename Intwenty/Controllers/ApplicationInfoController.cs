using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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

        public IActionResult Open()
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
        public JsonResult Save([FromBody] dynamic model)
        {
            var s = "";
            return new JsonResult("");
        }


        public IActionResult Get(int id)
        {
            return Ok();
        }

        public IActionResult ValidateModel()
        {
            var apps = Repository.GetApplicationMeta();
            var views = Repository.GetMetaDataViews();
            var res = MetaServer.ValidateModel(apps, views);
            return View(res);
        }

        [HttpPost]
        public IActionResult Save()
        {
            return Ok();
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





    }
}
