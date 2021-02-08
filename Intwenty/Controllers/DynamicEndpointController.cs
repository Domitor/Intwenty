﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Intwenty.Model.Dto;
using Microsoft.AspNetCore.Http;
using Intwenty.Interface;
using Microsoft.Extensions.Primitives;
using Intwenty.Areas.Identity.Entity;
using Intwenty.Model;
using Intwenty.DataClient.Model;
using Intwenty.DataClient;

namespace Intwenty.Controllers
{
   
    [AllowAnonymous]
    public class DynamicEndpointController : Controller
    {
        private IIntwentyDataService DataRepository { get; }
        private IIntwentyModelService ModelRepository { get; }

        public DynamicEndpointController(IIntwentyDataService dataservice, IIntwentyModelService modelservice)
        {
            DataRepository = dataservice;
            ModelRepository = modelservice;
        }


        [HttpGet]
        public IActionResult Get(int? id)
        {
            if (!IsAuthenticated())
                return Unauthorized();
            
            var ep = GetEndpointModelFromPath();
            if (ep == null)
                return new BadRequestResult();

            if (!ep.IsDataTableConnected)
                return new JsonResult("Failure due to endpoint model configuration") { StatusCode = 400 };

            if (!id.HasValue)
                 return new JsonResult("Parameter Id must be an integer value") { StatusCode = 400 };

            var model = ModelRepository.GetApplicationModels().Find(p => p.Application.DbName.ToLower() == ep.DataTableInfo.DbName.ToLower() &&
                                                                   p.Application.MetaCode == ep.AppMetaCode);

            if (model!= null)
            {
              
                var state = new ClientStateInfo() { Id = id.Value, ApplicationId = model.Application.Id };
                var data = DataRepository.Get(state);
                if (!data.IsSuccess)
                    return new JsonResult(data.UserError) { StatusCode = 400 };
                
                return new JsonResult(data);

            }
            else
            {
                var prms = new IIntwentySqlParameter[] { new IntwentySqlParameter("@P1", id.Value) };
                var client = DataRepository.GetDataClient();
                client.Open();
                var res = client.GetJsonObject(string.Format("select * from {0} where id = @P1", ep.DataTableInfo.DbName), parameters: prms);
                client.Close();
                return new JsonResult(res.GetJsonString());

            }



        }

       

        [HttpPost]
        public IActionResult List([FromBody] ListFilter model)
        {
            if (!IsAuthenticated())
                return Unauthorized();

             var ep = GetEndpointModelFromPath();
             if (ep == null)
                return new BadRequestResult();

            if (!ep.IsDataTableConnected)
                return new JsonResult("Failure due to endpoint model configuration") { StatusCode = 400 };

            if (model == null)
                return new JsonResult("Invalid request body") { StatusCode = 400 };


            var m = ModelRepository.GetApplicationModels().Find(p => p.Application.DbName.ToLower() == ep.DataTableInfo.DbName.ToLower() &&
                                                                     p.Application.MetaCode == ep.AppMetaCode);
            //Is application basequery
            if (m != null)
            {
                model.ApplicationId = m.Application.Id;
                var res = DataRepository.GetJsonArray(model);
                if (!res.IsSuccess)
                    return new JsonResult(res.UserError) { StatusCode = 400 };
                
                return new JsonResult(res);

            }
            else
            {
                var sql = string.Format("select * from {0} order by id", ep.DataTableInfo.DbName.ToLower());
                var client = DataRepository.GetDataClient();
                client.Open();
                var res = client.GetJsonArray(sql);
                client.Close();
                return new JsonResult(res.GetJsonString());

            }   
        }

        [HttpPost]
        public IActionResult Save([FromBody] System.Text.Json.JsonElement data)
        {
            if (!IsAuthenticated())
                return Unauthorized();


            var ep = GetEndpointModelFromPath();
            if (ep == null)
                return new BadRequestResult();

            if (!ep.IsDataTableConnected)
                return new JsonResult("Failure due to endpoint model configuration") { StatusCode = 400 };

         

            var model = ModelRepository.GetApplicationModels().Find(p => p.Application.DbName.ToLower() == ep.DataTableInfo.DbName.ToLower() &&
                                                                 p.Application.MetaCode == ep.AppMetaCode);

            if (model == null)
                return new JsonResult("Invalid request body") { StatusCode = 400 };

           
            var state = ClientStateInfo.CreateFromJSON(data);
            state.ApplicationId = model.Application.Id;
            state.Data.ApplicationId = model.Application.Id;
            var res = DataRepository.Save(state);
            if (!res.IsSuccess)
            {
                return new JsonResult(res.SystemError) { StatusCode = 400 };
            }
            else
            {
                return new JsonResult(res);
            }


           

           
        }

        [HttpPost]
        public IActionResult View([FromBody] ListFilter model)
        {
            if (!IsAuthenticated())
                return Unauthorized();

            var ep = GetEndpointModelFromPath();
            if (ep == null)
                return new BadRequestResult();

            if (!ep.IsDataViewConnected)
                return new JsonResult("Failure due to endpoint model configuration") { StatusCode = 400 };

            if (model == null)
                return new JsonResult("Invalid request body") { StatusCode = 400 };

            model.DataViewMetaCode = ep.DataViewInfo.MetaCode;
            var res = DataRepository.GetDataView(model);
            if (!res.IsSuccess)
                return new JsonResult(res.UserError) { StatusCode = 400 };

            return new JsonResult(res);

        }

        private bool IsAuthenticated()
        {
            try
            {
                StringValues key;
                if (Request.Headers.TryGetValue("Authorization", out key))
                {
                    var client = DataRepository.GetIAMDataClient();
                    client.Open();
                    var users = client.GetEntities<IntwentyUser>();
                    client.Close();

                    if (users.Exists(p => p.APIKey == key[0]))
                    {
                        return true;
                    }

                }
            }
            catch { }

            return false;
           
      }

        private EndpointModelItem GetEndpointModelFromPath() 
        {
            var path = this.Request.Path.Value;

            //var epmodels = ModelRepository.GetEndpointModels();

            var ep = ModelRepository.GetEndpointModels().Find(p => path.ToUpper().Contains((p.Path + p.Action).ToUpper()));
            return ep;

        }

       


    }
}
