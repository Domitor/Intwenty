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
using Intwenty.MetaDataService.Model;

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
            var state = new ClientStateInfo() { Id = id, ApplicationId = applicationid };
            var data = MetaServer.GetLatestVersion(state);
            return new JsonResult(data);

        }

        /// <summary>
        /// Loads data for a listview for the application with supplied Id
        /// </summary>
        [HttpPost]
        public JsonResult GetListView([FromBody] ListRetrivalArgs model)
        {
            var listdata = MetaServer.GetListView(model);
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
      
            var state = ClientStateInfo.CreateFromJSON(model);
            if (!state.HasData)
                return new JsonResult("{}");

            if (state.ApplicationId < 1)
                throw new InvalidOperationException("ApplicationId missing when saving...");


            var res = MetaServer.Save(state);

            return new JsonResult(res);

        }




    }
}
