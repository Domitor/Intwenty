using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Intwenty.Model.DesignerVM;
using Intwenty.Model;
using Microsoft.AspNetCore.Authorization;
using Intwenty.Data.Dto;
using Intwenty.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using Intwenty.Interface;

namespace Intwenty.Controllers
{

    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Policy = "IntwentyModelAuthorizationPolicy")]
    public class ModelController : Controller
    {
        public IIntwentyDataService DataRepository { get; }
        public IIntwentyModelService ModelRepository { get; }

        public ModelController(IIntwentyDataService ms, IIntwentyModelService sr)
        {
            DataRepository = ms;
            ModelRepository = sr;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpGet("/Model/Edit/{applicationid}")]
        public IActionResult Edit(int applicationid)
        {
            ViewBag.SystemId = Convert.ToString(applicationid);
            return View();
        }

        [HttpGet("/Model/EditDB/{applicationid}")]
        public IActionResult EditDB(int applicationid)
        {
            ViewBag.SystemId = Convert.ToString(applicationid);
            return View();
        }

        [HttpGet("/Model/EditUI/{applicationid}")]
        public IActionResult EditUI(int applicationid)
        {
            ViewBag.SystemId = Convert.ToString(applicationid);
            return View();
        }

        public IActionResult EditTranslations()
        {
            return View();
        }

        public IActionResult EditDataviews()
        {
            return View();
        }

        public IActionResult EditValueDomains()
        {
            return View();
        }

        [Obsolete]
        public IActionResult EditNoSeries()
        {
            return View();
        }

        public IActionResult EditMainMenu()
        {
            return View();
        }



        public IActionResult GetList()
        {
            return View();
        }


        public IActionResult ToolConfigureDatabase()
        {
            return View();
        }

        public IActionResult ToolValidateModel()
        {
            var res = ModelRepository.ValidateModel();
            return View(res);
        }

        public IActionResult ToolModelDocumentation()
        {
            var res = new List<ApplicationModel>();
            var appmodels = ModelRepository.GetApplicationModels();
            return View(appmodels);
        }

        public IActionResult ToolGenerateTestData()
        {
            return View();
        }

        public IActionResult ImportModel()
        {
            return View();
        }

        public IActionResult ImportData()
        {
            return View();
        }





    }
}
