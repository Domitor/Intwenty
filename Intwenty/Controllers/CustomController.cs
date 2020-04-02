using System;
using Microsoft.AspNetCore.Mvc;
using Intwenty.Model;
using Intwenty.Data;
using Intwenty;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using Intwenty.Data.Dto;

namespace Intwenty.Controllers.Custom
{
    public class CustomController : Controller
    {
        private IIntwentyDataService DataRepository { get; }
        private IIntwentyModelService ModelRepository { get; }

        public CustomController(IIntwentyDataService ms, IIntwentyModelService sr)
        {
            DataRepository = ms;
            ModelRepository = sr;
        }

      
        public IActionResult ShipmentFiles()
        {
            var t = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == 70);
            return View(t);
        }


        /*********************  API ***********************************************************/


        [HttpPost]
        public JsonResult GetShipmentFiles([FromBody] ListRetrivalArgs model)
        {
            model.ApplicationId = 70;
            var listdata = DataRepository.GetListView(model);
            return new JsonResult(listdata);
        }

        [HttpPost]
        public async Task<JsonResult> UploadDocument(IFormFile file)
        {
            var uniquefilename = $"{DateTime.Now.Ticks}_{file.FileName}";

            var fileandpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\USERDOC", uniquefilename);
            using (var fs = new FileStream(fileandpath, FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }

            var state = new ClientStateInfo() { ApplicationId = 70 };
            state.Values.Add(new ApplicationValue() { DbName = "FileName", Value = uniquefilename });
            state.Values.Add(new ApplicationValue() { DbName = "ImportDate", Value = DateTime.Now.ToString() });

            var res = DataRepository.Save(state);

            return new JsonResult(res);
        }


    }
}
