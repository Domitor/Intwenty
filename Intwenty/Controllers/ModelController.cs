using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Intwenty.Model.UIDesign;
using Intwenty.Model;
using Microsoft.AspNetCore.Authorization;
using Intwenty.Model.Dto;
using Intwenty.Entity;
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

        [HttpGet("/Model/EditUI/{applicationid}/{viewtype}")]
        public IActionResult EditUI(int applicationid, string viewtype)
        {
            ViewBag.SystemId = Convert.ToString(applicationid);
            ViewBag.ViewType = viewtype;
            return View();
        }

        public IActionResult EditTranslations()
        {
            return View();
        }

        public IActionResult EditEndpoints()
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
            var client = DataRepository.GetDataClient();
            var dbtypemap = client.GetDbTypeMap();
            var res = new List<ApplicationModel>();
            var appmodels = ModelRepository.GetApplicationModels();
            foreach (var app in appmodels)
            {
                foreach (var col in app.DataStructure.Where(p => p.IsMetaTypeDataColumn))
                {
                    var dbtype = dbtypemap.Find(p => p.IntwentyType == col.DataType && p.DbEngine == client.Database);
                    if (dbtype!=null)
                        col.AddUpdateProperty("DBDATATYPE", dbtype.DBMSDataType);
                }
            }
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

        public IActionResult UserPermission(string id)
        {
            if (!User.Identity.IsAuthenticated)
                return Forbid();

            if (!User.IsInRole("SUPERADMIN") && !User.IsInRole("USERADMIN"))
            {
                return Forbid();
            }

            ViewBag.SystemId = id;
            return View();
        }





    }
}
