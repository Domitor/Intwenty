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
using Intwenty.Model;
using Intwenty.DataClient.Model;
using Intwenty.DataClient;

namespace Intwenty.Controllers
{
   
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
        public IActionResult GetById(int? id)
        {
            if (!IsAuthenticated())
                return Unauthorized();
            
            var ep = GetEndpointModelFromPath();
            if (ep == null)
                return new BadRequestResult();

            if (!ep.IsDataTableConnected)
                return new BadRequestResult();

            if (!id.HasValue)
                 return new BadRequestResult();


            if (ModelRepository.GetApplicationModels().Exists(p => p.Application.DbName.ToLower() == ep.DataTableInfo.DbName.ToLower() &&
                                                                   p.Application.MetaCode == ep.AppMetaCode))
            {
                var model = ModelRepository.GetApplicationModels().Find(p => p.Application.DbName.ToUpper() == ep.DataTableInfo.DbName);
                if (model == null)
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
                var prms = new IIntwentySqlParameter[] { new IntwentySqlParameter("@P1", id.Value) };
                var client = DataRepository.GetDataClient();
                client.Open();
                var res = client.GetJSONObject(string.Format("select * from {0} where id = @P1", ep.DataTableInfo.DbName), parameters: prms);
                client.Close();
                return new JsonResult(res);

            }



        }

       

        [HttpGet]
        public IActionResult GetAll()
        {
            if (!IsAuthenticated())
                return Unauthorized();

             var ep = GetEndpointModelFromPath();
             if (ep == null)
                return new BadRequestResult();

            if (!ep.IsDataTableConnected)
                return new BadRequestResult();

          
            //Is application basequery
            if (ModelRepository.GetApplicationModels().Exists(p => p.Application.DbName.ToLower() == ep.DataTableInfo.DbName.ToLower() &&
                                                                    p.Application.MetaCode == ep.AppMetaCode))
            {
                var model = ModelRepository.GetApplicationModels().Find(p => p.Application.DbName.ToUpper() == ep.DataTableInfo.DbName);
                if (model == null)
                    return new BadRequestResult();

                var res = DataRepository.GetList(model.Application.Id);
                return new JsonResult(res);

            }
            else
            {
                var client = DataRepository.GetDataClient();
                client.Open();
                var res = client.GetJSONArray(string.Format("select * from {0} order by id", ep.DataTableInfo.DbName));
                client.Close();
                return new JsonResult(res);

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
                return new BadRequestResult();

            //Is application basequery
            if (ModelRepository.GetApplicationModels().Exists(p => p.Application.DbName.ToLower() == ep.DataTableInfo.DbName.ToLower() &&
                                                                    p.Application.MetaCode == ep.AppMetaCode))
            {
                var model = ModelRepository.GetApplicationModels().Find(p => p.Application.DbName.ToUpper() == ep.DataTableInfo.DbName);
                if (model == null)
                    return new BadRequestResult();

                var state = ClientStateInfo.CreateFromJSON(data);
                state.ApplicationId = model.Application.Id;
                state.Data.ApplicationId = model.Application.Id;
                var res = DataRepository.Save(state);
                if (!res.IsSuccess)
                {
                    var error = new JsonResult(res.SystemError);
                    error.StatusCode = 400;
                    return error;
                }
                else
                {
                    return new JsonResult(res);
                }


            }
            else
            {
                return new BadRequestResult();
            }


           
        }

        [HttpGet]
        public IActionResult GetData()
        {
            if (!IsAuthenticated())
                return Unauthorized();

            var ep = GetEndpointModelFromPath();
            if (ep == null)
                return new BadRequestResult();

            if (ep.IsDataViewConnected)
            {
                var args = new ListRetrivalArgs();
                args.BatchSize = 10000;
                args.DataViewMetaCode = ep.DataViewInfo.MetaCode;
                var res = DataRepository.GetDataView(args);
                return new JsonResult(res);
            }


            return new BadRequestResult();

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

        private EndpointModelItem GetEndpointModelFromPath() 
        {
            var path = this.Request.Path.Value;
            var ep = ModelRepository.GetEndpointModels().Find(p => (p.Path + p.Action).ToUpper() == path.ToUpper());
            return ep;

        }

       


    }
}
