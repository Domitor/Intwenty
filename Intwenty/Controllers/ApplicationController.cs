using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Intwenty.Models;
using Intwenty.Data;
using Intwenty.MetaDataService;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace Intwenty.Controllers
{
    [Authorize(Roles = "User,Administrator")]
    public class ApplicationController : Controller
    {
        private IServiceEngine MetaServer { get; }
        private IModelRepository Repository { get; }

        public ApplicationController(IServiceEngine ms, IModelRepository sr)
        {
            MetaServer = ms;
            Repository = sr;
        }

        /// <summary>
        /// Generate UI based on UIStructure for the application with the supplied Id.
        /// </summary>
        public IActionResult Create(int id)
        {
            var t = Repository.GetApplicationModels().Find(p=> p.Application.Id == id);
            return View(t);
        }

        /// <summary>
        /// Renders a list view for application with supplied Id
        /// </summary>
        public IActionResult GetList(int id)
        {
            var t = Repository.GetApplicationModels().Find(p => p.Application.Id == id);
            return View(t);
        }

        /// <summary>
        /// Generate UI based on UIStructure for the application with the supplied Id.
        /// </summary>
        [HttpGet("/Application/Open/{applicationid}/{id}")]
        public IActionResult Open(int applicationid, int id)
        {
            ViewBag.SystemId = Convert.ToString(id);
            var t = Repository.GetApplicationModels().Find(p => p.Application.Id == applicationid);
            return View(t);

        }

        /*********************  API ***********************************************************/


        /// <summary>
        /// Get the latest version data by id for an application with applicationid 
        /// </summary>
        /// <param name="applicationid">The ID of the application in the meta model</param>
        /// <param name="id">The data id</param>
        [HttpGet("/Application/GetLatestVersion/{applicationid}/{id}")]
        public JsonResult GetLatestVersion(int applicationid, int id)
        {
            var t = Repository.GetApplicationModels().Find(p => p.Application.Id == applicationid);
            var state = new ClientStateInfo() { Id = id };
            var data = MetaServer.GetLatestVersion(t, state);
            //var settings = new Newtonsoft.Json.JsonSerializerSettings();
            //settings.Culture = new CultureInfo("sv-SE");
            //settings.Culture.NumberFormat.NumberDecimalSeparator = ".";
            return new JsonResult(data);

        }

        /// <summary>
        /// Loads data for a listview for the application with supplied Id
        /// </summary>
        [HttpPost]
        public JsonResult GetListView([FromBody] ListRetrivalArgs model)
        {
            var t = Repository.GetApplicationModels().Find(p => p.Application.Id == model.ApplicationId);
            var listdata = MetaServer.GetListView(t, model);
            return new JsonResult(listdata);
        }

        /// <summary>
        /// Get Domain data based on the meta model for application with Id.
        /// </summary>
        [HttpGet("/Application/GetValueDomains/{id}")]
        public JsonResult GetValueDomains(int id)
        {
            var t = Repository.GetApplicationModels().Find(p => p.Application.Id == id);
            var data = MetaServer.GetValueDomains(t);
            var res = new JsonResult(data);
            return res;

        }

        /// <summary>
        /// Get a dataview record by a search value and a view name.
        /// Used from the LOOKUP Control
        /// </summary>
        [HttpPost]
        public JsonResult GetDataViewValue([FromBody] ListRetrivalArgs model)
        {
            var viewinfo = Repository.GetDataViewModels();
            var t = Repository.GetApplicationModels().Find(p => p.Application.Id == model.ApplicationId);
            var viewitem = MetaServer.GetDataViewValue(t, viewinfo, model);
            return new JsonResult(viewitem);
        }

        /// <summary>
        /// Get a dataview record by a search value and a view name.
        /// Used from the LOOKUP Control
        /// </summary>
        [HttpPost]
        public JsonResult GetDataView([FromBody] ListRetrivalArgs model)
        {
            var viewinfo = Repository.GetDataViewModels();
            var t = Repository.GetApplicationModels().Find(p => p.Application.Id == model.ApplicationId);
            var dv = MetaServer.GetDataView(t, viewinfo, model);
            return new JsonResult(dv);
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


        
        [HttpPost]
        public JsonResult Save([FromBody] System.Text.Json.JsonElement model)
        {


            var jsonarr = model.EnumerateObject();
            var appvalues = new Dictionary<string, object>();
            if (jsonarr.Count() < 1)
                return new JsonResult("{}");

            foreach (var j in jsonarr)
            {
                if (j.Value.ValueKind == System.Text.Json.JsonValueKind.Object)
                {
                    var jsonobjarr = j.Value.EnumerateObject();
                    foreach (var av in jsonobjarr)
                    {
                        if (av.Value.ValueKind == System.Text.Json.JsonValueKind.Number)
                            appvalues.Add(av.Name, av.Value.GetDecimal());
                        if (av.Value.ValueKind == System.Text.Json.JsonValueKind.String || av.Value.ValueKind == System.Text.Json.JsonValueKind.Undefined)
                            appvalues.Add(av.Name, av.Value.GetString());
                    }
                }
                if (j.Value.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    appvalues.Add(j.Name, j.Value);
                }
            }

            if (!appvalues.ContainsKey("ApplicationId"))
                throw new InvalidOperationException("ApplicationId missing when saving...");

            var appid = Convert.ToInt32(appvalues.First(p => p.Key == "ApplicationId").Value);

            var app = Repository.GetApplicationModels().Find(p => p.Application.Id == appid);

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
