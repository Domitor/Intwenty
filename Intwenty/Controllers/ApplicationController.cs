using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moley.Models;
using Moley.Data;
using Moley.MetaDataService;
using System.Globalization;

namespace Moley.Controllers
{
    public class ApplicationController : Controller
    {
        private IServiceEngine MetaServer { get; }
        private ISystemRepository Repository { get; }

        public ApplicationController(IServiceEngine ms, ISystemRepository sr)
        {
            MetaServer = ms;
            Repository = sr;
        }

        /// <summary>
        /// Generate UI based on UIStructure for the application with the supplied Id.
        /// </summary>
        public IActionResult Create(int id)
        {
            var t = Repository.GetApplicationMeta().Find(p=> p.Application.Id == id);
            return View(t);
        }

        /// <summary>
        /// Renders a list view for application with supplied Id
        /// </summary>
        public IActionResult GetList(int id)
        {
            var t = Repository.GetApplicationMeta().Find(p => p.Application.Id == id);
            return View(t);
        }

        /// <summary>
        /// Generate UI based on UIStructure for the application with the supplied Id.
        /// </summary>
        [HttpGet("/Application/Open/{applicationid}/{id}")]
        public IActionResult Open(int applicationid, int id)
        {
            ViewBag.SystemId = Convert.ToString(id);
            var t = Repository.GetApplicationMeta().Find(p => p.Application.Id == applicationid);
            return View(t);

        }

        /// <summary>
        /// Generate UI based on UIStructure for the application with the supplied Id.
        /// </summary>
        [HttpGet("/Application/GetLatestVersion/{applicationid}/{id}")]
        public JsonResult GetLatestVersion(int applicationid, int id)
        {
            var t = Repository.GetApplicationMeta().Find(p => p.Application.Id == applicationid);
            var state = new ClientStateInfo() { Id = id };
            var data = MetaServer.GetLatestVersion(t, state);
            //var settings = new Newtonsoft.Json.JsonSerializerSettings();
            //settings.Culture = new CultureInfo("sv-SE");
            //settings.Culture.NumberFormat.NumberDecimalSeparator = ".";
            return new JsonResult(data);

        }

        /// <summary>
        /// Get Domain data based on the meta model for application with Id.
        /// </summary>
        [HttpGet("/Application/GetDomains/{id}")]
        public JsonResult GetDomains(int id)
        {
            var viewinfo = Repository.GetMetaDataViews();
            var systemmeta = Repository.GetApplicationMeta();
            var t = systemmeta.Find(p => p.Application.Id == id);
            var data = MetaServer.GetDomains(t, viewinfo);
            var res = new JsonResult(data);
            return res;

        }

        /// <summary>
        /// Get new NoSeries for fields in the application with Id.
        /// </summary>
        [HttpGet("/Application/GetNoSerieValues/{id}")]
        public JsonResult GetNoSerieValues(int id)
        {
            var t = Repository.GetNewNoSeriesValues(id);
            return new JsonResult(t);

        }

        /// <summary>
        /// Loads data for a listview for the application with supplied Id
        /// </summary>
        [HttpPost]
        public JsonResult GetListData([FromBody] ListRetrivalArgs model)
        {
            var t = Repository.GetApplicationMeta().Find(p => p.Application.Id == model.ApplicationId);
            var listdata = MetaServer.GetList(t, model);
            return new JsonResult(listdata);
        }

        [HttpPost]
        public JsonResult Save([FromBody] dynamic model)
        {
            var appvalues = new Dictionary<string, object>();
            if (model.Count < 1)
                return new JsonResult("{}");

            foreach (var j in model)
            {
                appvalues.Add(j.Name, j.Value.Value);
            }

            if (!appvalues.ContainsKey("ApplicationId"))
                throw new InvalidOperationException("ApplicationId missing when saving...");

            var appid = Convert.ToInt32(appvalues.First(p => p.Key == "ApplicationId").Value);

            var app = Repository.GetApplicationMeta().Find(p => p.Application.Id == appid);

            var state = new ClientStateInfo();
            state.Application = app.Application;

            if (appvalues.ContainsKey("Id") && appvalues.ContainsKey("Version"))
            {
                state.Id = Convert.ToInt32(appvalues.First(p => p.Key == "Id").Value);
                state.Version = Convert.ToInt32(appvalues.First(p => p.Key == "Version").Value);
            }

            var res = MetaServer.Save(app, state, appvalues);

            return new JsonResult(res);

        }




    }
}
