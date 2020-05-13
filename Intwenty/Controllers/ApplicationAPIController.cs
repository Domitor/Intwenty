using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Intwenty.Data.Dto;
using Microsoft.AspNetCore.Http;


namespace Intwenty.Controllers
{
    [Authorize(Policy = "IntwentyAppAuthorizationPolicy")]
    public class ApplicationAPIController : Controller
    {
        private IIntwentyDataService DataRepository { get; }
        private IIntwentyModelService ModelRepository { get; }

        public ApplicationAPIController(IIntwentyDataService ms, IIntwentyModelService sr)
        {
            DataRepository = ms;
            ModelRepository = sr;
        }


        /// <summary>
        /// Get the latest version data by id for an application with applicationid 
        /// </summary>
        /// <param name="applicationid">The ID of the application in the meta model</param>
        /// <param name="id">The data id</param>
        [HttpGet("/Application/API/GetLatestVersion/{applicationid}/{id}")]
        public JsonResult GetLatestVersion(int applicationid, int id)
        {
            var state = new ClientStateInfo() { Id = id, ApplicationId = applicationid };
            var data = DataRepository.GetLatestVersionById(state);
            return new JsonResult(data);

        }

        /// <summary>
        /// Loads data for a listview for the application with supplied Id
        /// </summary>
        [HttpPost("/Application/API/GetListView")]
        public JsonResult GetListView([FromBody] ListRetrivalArgs model)
        {
            var listdata = DataRepository.GetList(model);
            return new JsonResult(listdata);
        }

        /// <summary>
        /// Get Domain data based on the meta model for application with Id.
        /// </summary>
        [HttpGet("/Application/API/GetValueDomains/{id}")]
        public JsonResult GetValueDomains(int id)
        {
            var data = DataRepository.GetValueDomains(id);
            var res = new JsonResult(data);
            return res;

        }

        /// <summary>
        /// Get a dataview record by a search value and a view name.
        /// Used from the LOOKUP Control
        /// </summary>
        [HttpPost("/Application/API/GetDataViewValue")]
        public JsonResult GetDataViewValue([FromBody] ListRetrivalArgs model)
        {
            var viewitem = DataRepository.GetDataViewRecord(model);
            return new JsonResult(viewitem);
        }

        /// <summary>
        /// Get a dataview record by a search value and a view name.
        /// Used from the LOOKUP Control
        /// </summary>
        [HttpPost("/Application/API/GetDataView")]
        public JsonResult GetDataView([FromBody] ListRetrivalArgs model)
        {
            var dv = DataRepository.GetDataView(model);
            return new JsonResult(dv);
        }

        /// <summary>
        /// Get new NoSeries for fields in the application with Id.
        /// </summary>
        [HttpGet("/Application/API/CreateNew/{id}")]
        public JsonResult GetDefaultValues(int id)
        {
            var state = new ClientStateInfo() { ApplicationId = id };
            var t = DataRepository.CreateNew(state);
            return new JsonResult(t);

        }


        
        [HttpPost("/Application/API/Save")]
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

        [HttpPost("/Application/API/UploadImage")]
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
