
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Intwenty.Model.Dto;
using Microsoft.AspNetCore.Http;
using Intwenty;
using Intwenty.Interface;
using Intwenty.Areas.Identity.Data;
using IntwentyDemo.Models;

namespace IntwentyDemo.Controllers
{
    /*
       -- This is an example of how to override the Intwenty ApplicationAPIController --
       1.   Name the controller ApplicationAPIController and put in the Controllers folder. 
       2.   Attribute routing must be used otherwise it will be conflicts with routes in the base controller.
       3.   If this controller overides Intwenty.Controllers.ApplicationAPIController, all methods must be overridden with route attributes, otherwist there will be a route confilict



    [Authorize(Policy = "IntwentyAppAuthorizationPolicy")]
    public class ApplicationAPIController : Intwenty.Controllers.ApplicationAPIController
    {
        private IIntwentyDataService DataRepository { get; }
        private IIntwentyModelService ModelRepository { get; }

        public ApplicationAPIController(IIntwentyDataService dataservice, IIntwentyModelService modelservice, IntwentyUserManager usermanager) : base(dataservice, modelservice, usermanager)
        {
            DataRepository = dataservice;
            ModelRepository = modelservice;
        }


      
        /// <summary>
        /// Loads data for a listview for the application with supplied Id
        /// </summary>
        [HttpPost("Application/API/GetEditListData")]
        public override IActionResult GetEditListData([FromBody] ListFilter model)
        {
            return base.GetEditListData(model);
        }

      



    }
           */

}
