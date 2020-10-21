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
    public class DynamicApplicationController : Controller
    {
        private IIntwentyDataService DataRepository { get; }
        private IIntwentyModelService ModelRepository { get; }

        public DynamicApplicationController(IIntwentyDataService dataservice, IIntwentyModelService modelservice)
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
        public virtual JsonResult GetLatestVersion(int id)
        {
            return new JsonResult("");

        }

     




    }
}
