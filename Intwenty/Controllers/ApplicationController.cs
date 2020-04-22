using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Intwenty.Data.Dto;
using Microsoft.AspNetCore.Http;

namespace Intwenty.Controllers
{
    //[Authorize(Roles = "User,Administrator,Producer")]
    public class ApplicationController : Controller
    {
        private IIntwentyDataService DataRepository { get; }
        private IIntwentyModelService ModelRepository { get; }

        public ApplicationController(IIntwentyDataService ms, IIntwentyModelService sr)
        {
            DataRepository = ms;
            ModelRepository = sr;
        }

        /// <summary>
        /// Generate UI based on UIStructure for the application with the supplied Id.
        /// </summary>
        public IActionResult Create(int id)
        {
            var t = ModelRepository.GetApplicationModels().Find(p=> p.Application.Id == id);
            return View(t);
        }

        /// <summary>
        /// Renders a list view for application with supplied Id
        /// </summary>
        public IActionResult GetList(int id)
        {
            var t = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == id);
            return View(t);
        }

        /// <summary>
        /// Generate UI based on UIStructure for the application with the supplied Id.
        /// </summary>
        [HttpGet("/Application/Open/{applicationid}/{id}")]
        public IActionResult Edit(int applicationid, int id)
        {
            ViewBag.SystemId = Convert.ToString(id);
            var t = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == applicationid);
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
            var data = DataRepository.GetLatestVersion(state);
            return new JsonResult(data);

        }

        /// <summary>
        /// Loads data for a listview for the application with supplied Id
        /// </summary>
        [HttpPost]
        public JsonResult GetListView([FromBody] ListRetrivalArgs model)
        {
            var listdata = DataRepository.GetList(model);
            return new JsonResult(listdata);
        }

        /// <summary>
        /// Get Domain data based on the meta model for application with Id.
        /// </summary>
        [HttpGet("/Application/GetValueDomains/{id}")]
        public JsonResult GetValueDomains(int id)
        {
            var t = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == id);
            var data = DataRepository.GetValueDomains(t);
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
            var viewinfo = ModelRepository.GetDataViewModels();
            var t = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == model.ApplicationId);
            var viewitem = DataRepository.GetDataViewValue(t, viewinfo, model);
            return new JsonResult(viewitem);
        }

        /// <summary>
        /// Get a dataview record by a search value and a view name.
        /// Used from the LOOKUP Control
        /// </summary>
        [HttpPost]
        public JsonResult GetDataView([FromBody] ListRetrivalArgs model)
        {
            var viewinfo = ModelRepository.GetDataViewModels();
            var t = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == model.ApplicationId);
            var dv = DataRepository.GetDataView(t, viewinfo, model);
            return new JsonResult(dv);
        }

        /// <summary>
        /// Get new NoSeries for fields in the application with Id.
        /// </summary>
        [HttpGet("/Application/GetDefaultValues/{id}")]
        public JsonResult GetDefaultValues(int id)
        {
            var state = new ClientStateInfo() { ApplicationId = id };
            var t = DataRepository.GetDefaultValues(state);
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


            var res = DataRepository.Save(state);

            return new JsonResult(res);

        }

        [HttpPost]
        public async Task<JsonResult> UploadImage(IFormFile file)
        {
            var uniquefilename = $"{DateTime.Now.Ticks}_{file.FileName}";

            var fileandpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\USERDOC", uniquefilename);
            using (var fs = new FileStream(fileandpath, FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }

            return new JsonResult(new { fileName= uniquefilename });
        }




    }
}
