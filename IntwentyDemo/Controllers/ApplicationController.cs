using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Intwenty.Model.Dto;
using Microsoft.AspNetCore.Http;
using Intwenty;
using Intwenty.Interface;

namespace IntwentyDemo.Controllers
{
    /*
     
       -- This is an example of how to override the Intwenty ApplicationController --
       1.   Name the controller ApplicationController and put in the Controllers folder. 
       2.   Attribute routing must be used otherwise it will be conflicts with routes in the base controller.

    [Authorize(Policy = "IntwentyAppAuthorizationPolicy")]
    public class ApplicationController : Intwenty.Controllers.ApplicationController
    {

        private IIntwentyDataService DataRepository { get; }
        private IIntwentyModelService ModelRepository { get; }

      
        public ApplicationController(IIntwentyDataService dataservice, IIntwentyModelService modelservice) : base(dataservice, modelservice)
        {
            DataRepository = dataservice;
            ModelRepository = modelservice;
        }

        /// <summary>
        /// Generate UI based on UIStructure for the application with the supplied Id.
        /// </summary>
        [HttpGet("Application/Create/{id}")]
        public override IActionResult Create(int id)
        {
            return base.Create(id);
        }

      


    }

    */
}
