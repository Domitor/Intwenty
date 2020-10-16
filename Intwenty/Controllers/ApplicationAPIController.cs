using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Intwenty.Data.Dto;
using Microsoft.AspNetCore.Http;
using Intwenty.Interface;

namespace Intwenty.Controllers
{
    [Authorize(Policy = "IntwentyAppAuthorizationPolicy")]
    public class ApplicationAPIController : Controller
    {
        private IIntwentyDataService DataRepository { get; }
        private IIntwentyModelService ModelRepository { get; }

        public ApplicationAPIController(IIntwentyDataService dataservice, IIntwentyModelService modelservice)
        {
            DataRepository = dataservice;
            ModelRepository = modelservice;
        }


        /// <summary>
        /// Get the latest version data by id for an application with applicationid 
        /// </summary>
        /// <param name="applicationid">The ID of the application in the meta model</param>
        /// <param name="id">The data id</param>
        [HttpGet]
        public virtual JsonResult GetLatestVersion(int applicationid, int id)
        {
            var state = new ClientStateInfo() { Id = id, ApplicationId = applicationid };
            var data = DataRepository.GetLatestVersionById(state);
            return new JsonResult(data);

        }

        /// <summary>
        /// Get the latest version data for the owning user and with applicationid 
        /// </summary>
        /// <param name="applicationid">The ID of the application in the meta model</param>
        [HttpGet]
        public virtual JsonResult GetLatestByLoggedInUser(int applicationid)
        {
            if (!User.Identity.IsAuthenticated)
            {
                var r = new OperationResult();
                r.SetError("Cannot get GetLatestByLoggedInUser since the user is not logged in", "User is not logged in");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

            var state = new ClientStateInfo() { ApplicationId = applicationid, OwnerUserId = User.Identity.Name };
            var data = DataRepository.GetLatestVersionByOwnerUser(state);
            return new JsonResult(data);

        }

        /// <summary>
        /// Loads data for a listview for the application with supplied Id
        /// </summary>
        [HttpPost]
        public virtual JsonResult GetListView([FromBody] ListRetrivalArgs model)
        {
            var listdata = DataRepository.GetList(model);
            return new JsonResult(listdata);
        }

        /// <summary>
        /// Get Domain data based on the meta model for application with Id.
        /// </summary>
        [HttpGet]
        public virtual JsonResult GetValueDomains(int id)
        {
            var data = DataRepository.GetValueDomains(id);
            var res = new JsonResult(data);
            return res;

        }

        /// <summary>
        /// Get a dataview record by a search value and a view name.
        /// Used from the LOOKUP Control
        /// </summary>
        [HttpPost]
        public virtual JsonResult GetDataViewValue([FromBody] ListRetrivalArgs model)
        {
            var viewitem = DataRepository.GetDataViewRecord(model);
            return new JsonResult(viewitem);
        }

        /// <summary>
        /// Get a dataview record by a search value and a view name.
        /// Used from the LOOKUP Control
        /// </summary>
        [HttpPost]
        public virtual JsonResult GetDataView([FromBody] ListRetrivalArgs model)
        {
            var dv = DataRepository.GetDataView(model);
            return new JsonResult(dv);
        }

        /// <summary>
        /// Get a json structure for a new application, including defaultvalues
        /// </summary>
        [HttpGet]
        public virtual JsonResult CreateNew(int id)
        {
            var state = new ClientStateInfo() { ApplicationId = id };
            var t = DataRepository.CreateNew(state);
            return new JsonResult(t);

        }


        [HttpPost]
        public virtual JsonResult Save([FromBody] System.Text.Json.JsonElement model)
        {
      
            var state = ClientStateInfo.CreateFromJSON(model);
            var res = DataRepository.Save(state);
            return new JsonResult(res);

        }

        [HttpPost]
        public virtual JsonResult Delete([FromBody] System.Text.Json.JsonElement model)
        {

            var state = ClientStateInfo.CreateFromJSON(model);
            var res = DataRepository.DeleteById(state);
            return new JsonResult(res);

        }

        [HttpPost]
        public virtual async Task<JsonResult> UploadImage(IFormFile file)
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
