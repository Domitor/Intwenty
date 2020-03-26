using System;
using Microsoft.AspNetCore.Mvc;
using Intwenty.Models;
using Intwenty.Data;
using Intwenty.MetaDataService;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using Intwenty.MetaDataService.Model;

namespace Intwenty.Controllers.Custom
{
    public class CustomController : Controller
    {
        private IServiceEngine MetaServer { get; }
        private IModelRepository Repository { get; }

        public CustomController(IServiceEngine ms, IModelRepository sr)
        {
            MetaServer = ms;
            Repository = sr;
        }

      
        public IActionResult ShipmentFiles()
        {
            var t = Repository.GetApplicationModels().Find(p => p.Application.Id == 70);
            return View(t);
        }


        /*********************  API ***********************************************************/


        [HttpPost]
        public JsonResult GetShipmentFiles([FromBody] ListRetrivalArgs model)
        {
            var t = Repository.GetApplicationModels().Find(p => p.Application.Id == 70);
            var listdata = MetaServer.GetListView(t, model);
            return new JsonResult(listdata);
        }

        [HttpPost]
        public async Task<JsonResult> UploadDocument(IFormFile file)
        {
            var t = Repository.GetApplicationModels().Find(p => p.Application.Id == 70);
            var uniquefilename = $"{DateTime.Now.Ticks}_{file.FileName}";

            var fileandpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\USERDOC", uniquefilename);
            using (var fs = new FileStream(fileandpath, FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }

            var state = new ClientStateInfo() { Application = t.Application };

            var data = new ApplicationData();
            data.MainTable.Add(new ApplicationDataValue() { Code = "FILENAME", Value = uniquefilename });
            data.MainTable.Add(new ApplicationDataValue() { Code = "IMPORTDATE", Value = DateTime.Now });

            var res = MetaServer.Save(t, state, data);

            return new JsonResult(res);
        }


    }
}
