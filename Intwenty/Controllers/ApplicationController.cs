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
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Policy = "IntwentyAppAuthorizationPolicy")]
    public class ApplicationController : Controller
    {

        private IIntwentyDataService DataRepository { get; }
        private IIntwentyModelService ModelRepository { get; }

        public ApplicationController(IIntwentyDataService dataservice, IIntwentyModelService modelservice)
        {
            DataRepository = dataservice;
            ModelRepository = modelservice;
        }

        /// <summary>
        /// Generate UI based on UIStructure for the application with the supplied application model id.
        /// </summary>
        public virtual IActionResult Create(int id)
        {
            var t = ModelRepository.GetApplicationModels().Find(p=> p.Application.Id == id);
            return View(t);
        }

        /// <summary>
        /// Renders a list view for application with supplied application model id.
        /// </summary>
        public virtual IActionResult GetList(int id)
        {
            var t = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == id);
            return View(t);
        }

        /// <summary>
        /// Generate UI based on UIStructure for the application with the supplied application model id and application data id.
        /// </summary>
        public virtual IActionResult Edit(int applicationid, int id)
        {
            ViewBag.SystemId = Convert.ToString(id);
            var t = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == applicationid);
            return View(t);

        }


    }
}
