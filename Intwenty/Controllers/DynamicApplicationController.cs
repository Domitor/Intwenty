using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Intwenty.Data.Dto;
using Microsoft.AspNetCore.Http;
using Intwenty.Interface;
using Microsoft.Extensions.Primitives;
using Intwenty.Areas.Identity.Models;

namespace Intwenty.Controllers
{
    [Authorize(Policy = "IntwentyAppAuthorizationPolicy")]
    public class DynamicApplicationController : Controller
    {
        private IIntwentyDataService DataRepository { get; }
        private IIntwentyModelService ModelRepository { get; }

        public DynamicApplicationController(IIntwentyDataService dataservice, IIntwentyModelService modelservice)
        {
            DataRepository = dataservice;
            ModelRepository = modelservice;
        }


        [HttpGet]
        public IActionResult GetLatestVersion(int? id)
        {
            if (IsAuthenticated())
            {
              
                var model = ModelRepository.GetApplicationModels().Find(p => p.Application.DbName.ToUpper() == GetApplicationFromPath());
                if (model == null)
                    return new BadRequestResult();

                if (!id.HasValue)
                    return new BadRequestResult();

                var state = new ClientStateInfo() { Id = id.Value, ApplicationId = model.Application.Id };
                var data = DataRepository.GetLatestVersionById(state);
                if (!data.IsSuccess)
                {
                    var res = new JsonResult(data.SystemError);
                    res.StatusCode = 400;
                    return res;
                }
                 

                return new JsonResult(data);
            }
            else
            {
                return Unauthorized();
            }


        }

        [HttpGet]
        public IActionResult GetAll()
        {
            if (IsAuthenticated())
            {
                var dbname = GetApplicationFromPath();
                var model = ModelRepository.GetApplicationModels().Find(p => p.Application.DbName.ToUpper() == dbname);
                if (model == null)
                    return new BadRequestResult();

                /*
                var client = DataRepository.GetDataClient();
                client.Open();
                var res = client.GetJSONArray(string.Format("select * from {0} order by id", dbname.ToLower()));
                client.Close();
                */

                var res = DataRepository.GetList(model.Application.Id);

                return new JsonResult(res);
            }
            else
            {
                return Unauthorized();
            }
          

        }

        [HttpPost]
        public IActionResult Save([FromBody] System.Text.Json.JsonElement data)
        {
            if (IsAuthenticated())
            {
                var dbname = GetApplicationFromPath();
                var model = ModelRepository.GetApplicationModels().Find(p => p.Application.DbName.ToUpper() == dbname);
                if (model == null)
                    return new BadRequestResult();

                var state = ClientStateInfo.CreateFromJSON(data);
                var res = DataRepository.Save(state);
                return new JsonResult(res);
            }
            else
            {
                return Unauthorized();
            }


        }

        private bool IsAuthenticated()
        {
            try
            {
                StringValues key;
                if (Request.Headers.TryGetValue("Authorization", out key))
                {
                    var client = DataRepository.GetDataClient();
                    client.Open();
                    var users = client.GetEntities<IntwentyUser>();
                    client.Close();

                    if (users.Exists(p => p.APIKey == key[0]))
                        return true;

                }
            }
            catch { }

            return false;
           
      }

        private string GetApplicationFromPath() 
        {
            var path = this.Request.Path.Value;
            var endindex = path.IndexOf("/API");
            if (endindex > 0)
            {
                var res =  path.Substring(0, endindex);
                endindex = res.LastIndexOf("/");
                if (endindex >= 0 && res.Length > (endindex + 1))
                    return res.Substring(endindex+1).ToUpper();
            }
           
           return string.Empty;
            
        }

       


    }
}
