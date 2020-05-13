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
    public class ApplicationController : Controller
    {

        private IIntwentyDataService DataRepository { get; }
        private IIntwentyModelService ModelRepository { get; }

        public ApplicationController(IIntwentyDataService ms, IIntwentyModelService sr)
        {
            DataRepository = ms;
            ModelRepository = sr;
        }

        /// <summary>
        /// Generate UI based on UIStructure for the application with the supplied Id.
        /// </summary>
        public IActionResult Create(int id)
        {
            var t = ModelRepository.GetApplicationModels().Find(p=> p.Application.Id == id);
            return View(t);
        }

        /// <summary>
        /// Renders a list view for application with supplied Id
        /// </summary>
        public IActionResult GetList(int id)
        {
            var t = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == id);
            return View(t);
        }

        /// <summary>
        /// Generate UI based on UIStructure for the application with the supplied Id.
        /// </summary>
        [HttpGet("/Application/Edit/{applicationid}/{id}")]
        public IActionResult Edit(int applicationid, int id)
        {
            ViewBag.SystemId = Convert.ToString(id);
            var t = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == applicationid);
            return View(t);

        }


    }
}
