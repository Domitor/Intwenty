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

        /// <summary>
        /// Renders a list view for application with supplied Id
        /// </summary>
        [HttpGet("Application/List/{id}")]
        public override IActionResult List(int id)
        {
            return base.List(id);
        }

        /// <summary>
        /// Generate UI based on UIStructure for the application with the supplied Id.
        /// </summary>
        [HttpGet("Application/Edit/{applicationid}/{id}")]
        public override IActionResult Edit(int applicationid, int id)
        {
            return base.Edit(applicationid,id);
        }

        /// <summary>
        /// Generate a detail presentation UI based on UIStructure for the application with the supplied application model id and application data id.
        /// </summary>
        [HttpGet("Application/Detail/{applicationid}/{id}")]
        public override IActionResult Detail(int applicationid, int id)
        {
            return base.Detail(applicationid, id);

        }


        /// <summary>
        /// Renders a list view for application with supplied application model id.
        /// </summary>
        [HttpGet("Application/EditList/{id}")]
        public override IActionResult EditList(int id)
        {
            return base.EditList(id);
        }


    }

    */
}
