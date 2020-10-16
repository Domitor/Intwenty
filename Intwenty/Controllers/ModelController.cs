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
            var main = ModelRepository.GetDefaultMainTableColumns();
            var subs = ModelRepository.GetDefaultSubTableColumns();

            //NEW LIST
            foreach (var m in appmodels)
                res.Add(new ApplicationModel() { Application = m.Application, DataStructure = new List<DatabaseModelItem>(), UIStructure = m.UIStructure });

            //ADD DEFAULT COLUMNS
            foreach (var m in res)
            {
                var app = appmodels.Find(p => p.Application.Id == m.Application.Id);
                if (app == null)
                    continue;

                foreach (var defcol in main)
                {
                    m.DataStructure.Add(new DatabaseModelItem(DatabaseModelItem.MetaTypeDataColumn) { AppMetaCode=app.Application.MetaCode, DbName = defcol.Name, DataType = defcol.DataType, MetaCode = defcol.Name.ToUpper(), ParentMetaCode = DatabaseModelItem.MetaTypeRoot, Title = defcol.Name, TableName = app.Application.DbName, Properties= "INTWENTYDEFAULTCOLUMN=TRUE" });
                }
                foreach (var ds in app.DataStructure.Where(p=> p.IsMetaTypeDataColumn && p.IsRoot))
                {
                    m.DataStructure.Add(ds);
                }
                foreach (var ds in app.DataStructure.Where(p => p.IsMetaTypeDataTable && p.IsRoot))
                {
                    m.DataStructure.Add(ds);
                    foreach (var defcol in subs)
                    {
                        m.DataStructure.Add(new DatabaseModelItem(DatabaseModelItem.MetaTypeDataColumn) { AppMetaCode = app.Application.MetaCode, DbName = defcol.Name, DataType = defcol.DataType, MetaCode = defcol.Name.ToUpper(), ParentMetaCode = ds.MetaCode, Title = defcol.Name, TableName = app.Application.DbName, Properties = "INTWENTYDEFAULTCOLUMN=TRUE" });
                    }
                    foreach (var subtblcol in app.DataStructure.Where(p => p.IsMetaTypeDataColumn && p.ParentMetaCode == ds.MetaCode))
                    {
                        m.DataStructure.Add(subtblcol);
                    }
                }
            }

            return View(res);
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
