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
        [HttpGet("Application/GetList/{id}")]
        public override IActionResult GetList(int id)
        {
            return base.GetList(id);
        }

        /// <summary>
        /// Generate UI based on UIStructure for the application with the supplied Id.
        /// </summary>
        [HttpGet("Application/Edit/{applicationid}/{id}")]
        public override IActionResult Edit(int applicationid, int id)
        {
            return base.Edit(applicationid,id);
        }


    }
}
