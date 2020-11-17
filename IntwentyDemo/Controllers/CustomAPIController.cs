
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
   
    [Authorize(Policy = "IntwentyAppAuthorizationPolicy")]
    public class CustomAPIController : Controller
    {
        private IIntwentyDataService DataRepository { get; }
        private IIntwentyModelService ModelRepository { get; }

        public CustomAPIController(IIntwentyDataService dataservice, IIntwentyModelService modelservice) 
        {
            DataRepository = dataservice;
            ModelRepository = modelservice;
        }


        [HttpPost("MyAPI/PostAnyThing")]
        public IActionResult Implementation([FromBody] System.Text.Json.JsonElement model)
        {
            return new JsonResult("{}");
        }



    }
        

}
