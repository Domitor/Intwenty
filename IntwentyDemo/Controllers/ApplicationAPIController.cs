
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Intwenty.Data.Dto;
using Microsoft.AspNetCore.Http;
using Intwenty;
using Intwenty.Interface;

namespace IntwentyDemo.Controllers
{
    /*
       -- This is an example of how to override the Intwenty ApplicationAPIController --
       1.   Name the controller ApplicationAPIController and put in the Controllers folder. 
       2.   Attribute routing must be used otherwise it will be conflicts with routes in the base controller.
    */

    
    [Authorize(Policy = "IntwentyAppAuthorizationPolicy")]
    public class ApplicationAPIController : Intwenty.Controllers.ApplicationAPIController
    {
        private IIntwentyDataService DataRepository { get; }
        private IIntwentyModelService ModelRepository { get; }

        public ApplicationAPIController(IIntwentyDataService dataservice, IIntwentyModelService modelservice) : base(dataservice, modelservice)
        {
            DataRepository = dataservice;
            ModelRepository = modelservice;
        }


        /// <summary>
        /// Get the latest version data by id for an application with applicationid 
        /// </summary>
        /// <param name="applicationid">The ID of the application in the meta model</param>
        /// <param name="id">The data id</param>
        [HttpGet("Application/API/GetLatestVersion/{applicationid}/{id}")]
        public override JsonResult GetLatestVersion(int applicationid, int id)
        {
            return base.GetLatestVersion(applicationid, id);
        }

        /// <summary>
        /// Get the latest version data for the owning user and with applicationid 
        /// </summary>
        /// <param name="applicationid">The ID of the application in the meta model</param>
        [HttpGet("Application/API/GetLatestByLoggedInUser/{applicationid}")]
        public override JsonResult GetLatestByLoggedInUser(int applicationid)
        {
            return base.GetLatestByLoggedInUser(applicationid);
        }

        /// <summary>
        /// Loads data for a listview for the application with supplied Id
        /// </summary>
        [HttpPost("Application/API/GetListView")]
        public override JsonResult GetListView([FromBody] ListRetrivalArgs model)
        {
            return base.GetListView(model);
        }

        /// <summary>
        /// Get Domain data based on the meta model for application with Id.
        /// </summary>
        [HttpGet("Application/API/GetValueDomains/{id}")]
        public override JsonResult GetValueDomains(int id)
        {
            return base.GetValueDomains(id);
        }

        /// <summary>
        /// Get a dataview record by a search value and a view name.
        /// Used from the LOOKUP Control
        /// </summary>
        [HttpPost("Application/API/GetDataViewValue")]
        public override JsonResult GetDataViewValue([FromBody] ListRetrivalArgs model)
        {
            return base.GetDataViewValue(model);
        }

        /// <summary>
        /// Get a dataview record by a search value and a view name.
        /// Used from the LOOKUP Control
        /// </summary>
        [HttpPost("Application/API/GetDataView")]
        public override JsonResult GetDataView([FromBody] ListRetrivalArgs model)
        {
            return base.GetDataView(model);
        }

        /// <summary>
        /// Get a json structure for a new application, including defaultvalues
        /// </summary>
        [HttpGet("Application/API/CreateNew/{id}")]
        public override JsonResult CreateNew(int id)
        {
            return base.CreateNew(id);
        }


        [HttpPost("Application/API/Save")]
        public override JsonResult Save([FromBody] System.Text.Json.JsonElement model)
        {
            return base.Save(model);
        }

        [HttpPost("Application/API/Delete")]
        public override JsonResult Delete([FromBody] System.Text.Json.JsonElement model)
        {
            return base.Delete(model);
        }

        [HttpPost("Application/API/UploadImage")]
        public override Task<JsonResult> UploadImage(IFormFile file)
        {
            return base.UploadImage(file);
        }




    }

    
}
