using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Intwenty.Model.Dto;
using Microsoft.AspNetCore.Http;
using Intwenty.Interface;
using Intwenty.Areas.Identity.Data;
using Microsoft.AspNetCore.Razor.Language;
using Intwenty.Areas.Identity.Models;
using Intwenty.Model;
using System.Collections.Generic;

namespace Intwenty.Controllers
{
    [Authorize(Policy = "IntwentyAppAuthorizationPolicy")]
    public class ApplicationAPIController : Controller
    {
        private IIntwentyDataService DataRepository { get; }
        private IIntwentyModelService ModelRepository { get; }
        private IntwentyUserManager UserManager { get; }
        public ApplicationAPIController(IIntwentyDataService dataservice, IIntwentyModelService modelservice, IntwentyUserManager usermanager)
        {
            DataRepository = dataservice;
            ModelRepository = modelservice;
            UserManager = usermanager;
        }




        /// <summary>
        /// Get the latest version data by id for an application with applicationid
        /// </summary>
        /// <param name="applicationid">The ID of the application in the meta model</param>
        /// <param name="id">The data id</param>
        [HttpGet]
        public virtual async Task<IActionResult> GetApplication(int applicationid, int viewid, int id)
        {

            var model = ModelRepository.GetApplicationModel(applicationid);
            if (model == null || id < 1)
                return BadRequest();

            var viewmodel = model.Views.Find(p => p.Id == viewid);
            if (viewmodel==null)
                return BadRequest();

            ClientStateInfo state = null;
            if (User.Identity.IsAuthenticated)
                state = new ClientStateInfo(User) { Id = id, ApplicationId = applicationid, ApplicationViewId = viewid };
            else
                state = new ClientStateInfo() { Id = id, ApplicationId = applicationid, ApplicationViewId = viewid };

            if (viewmodel.IsPublic)
            {
                var data = DataRepository.Get(state, model);
                return new JsonResult(data);
            } 
            else 
            {

                if (!User.Identity.IsAuthenticated)
                    return new JsonResult(new OperationResult(false, MessageCode.USERERROR, "You do not have access to this resource"));
                if (!await UserManager.HasAuthorization(User, viewmodel))
                    return new JsonResult(new OperationResult(false, MessageCode.USERERROR, string.Format("You do not have access to this resource, apply for read permission for application {0}", model.Application.Title)));

                var data = DataRepository.Get(state, model);
                return new JsonResult(data);
                
            }

        }


        /// <summary>
        /// Loads data for a listview for the application with supplied Id
        /// </summary>
        [HttpPost]
        public virtual async Task<IActionResult> GetPagedList([FromBody] ListFilter model)
        {
           

            if (model == null)
                return BadRequest();
            if (model.ApplicationId < 1)
                return BadRequest();

            var appmodel = ModelRepository.GetApplicationModel(model.ApplicationId);
            if (appmodel == null)
                return BadRequest();

            var viewmodel = appmodel.Views.Find(p => p.Id == model.ApplicationViewId);
            if (viewmodel == null)
                return BadRequest();

            if (User.Identity.IsAuthenticated)
                model.SetUser(User);
         

            if (viewmodel.IsPublic)
            {
                var listdata = DataRepository.GetJsonArray(model, appmodel);
                return new JsonResult(listdata);
            }
            else
            {
                if (!User.Identity.IsAuthenticated)
                    return new JsonResult(new OperationResult(false, MessageCode.USERERROR, "You do not have access to this resource"));
                if (!await UserManager.HasAuthorization(User, viewmodel))
                   return new JsonResult(new OperationResult(false, MessageCode.USERERROR, string.Format("You do not have access to this resource, apply for read permission for application {0}", appmodel.Application.Title)));


                var listdata = DataRepository.GetJsonArray(model, appmodel);
                return new JsonResult(listdata);

            }

           
        }

        [HttpGet("/Application/API/GetValueDomain/{domainname}/{query}")]
        public virtual List<ValueDomainVm> GetValueDomain(string domainname, string query)
        {

            var result = new List<ValueDomainVm>();
            result.Add(new ValueDomainVm() { Id = 1, Code = "1", Value = "Val 1" });
            result.Add(new ValueDomainVm() { Id = 2, Code = "2", Value = "Val 2" });
            result.Add(new ValueDomainVm() { Id = 3, Code = "3", Value = "Val 3" });

            return result;
        }

        /// <summary>
        /// Get Domain data based on the meta model for application with Id.
        /// </summary>
        [HttpGet]
        public virtual JsonResult GetValueDomains(int id)
        {
            var data = DataRepository.GetValueDomains(id);
            var res = new JsonResult(data);
            return res;

        }

      

        /// <summary>
        /// Get a json structure for a new application, including defaultvalues
        /// </summary>
        [HttpGet]
        public virtual JsonResult CreateNew(int id)
        {
            var state = new ClientStateInfo() { ApplicationId = id };
            var t = DataRepository.New(state);
            return new JsonResult(t);

        }


        [HttpPost]
        public virtual async Task<IActionResult> Save([FromBody] System.Text.Json.JsonElement model)
        {

            ClientStateInfo state = null;
            if (User.Identity.IsAuthenticated)
                state = ClientStateInfo.CreateFromJSON(model, User);
            else
                state = ClientStateInfo.CreateFromJSON(model);

            if (state == null)
                return BadRequest();
            if (state.ApplicationId < 1)
                return BadRequest();
            if (state.ApplicationViewId < 1)
                return BadRequest();
            var appmodel = ModelRepository.GetApplicationModel(state.ApplicationId);
            if (appmodel == null)
                return BadRequest();

            var viewmodel = appmodel.Views.Find(p => p.Id == state.ApplicationViewId);
            if (viewmodel == null)
                return BadRequest();


            if (viewmodel.IsPublic)
            {
                var res = DataRepository.Save(state, appmodel);
                return new JsonResult(res);
            }
            else
            {
                if (!User.Identity.IsAuthenticated)
                    return new JsonResult(new OperationResult(false, MessageCode.USERERROR, "You must login to use this function"));
                if (!await UserManager.HasAuthorization(User, viewmodel))
                   return new JsonResult(new OperationResult(false, MessageCode.USERERROR, string.Format("You are not authorized to modify data in this view or application")));


                var res = DataRepository.Save(state, appmodel);
                return new JsonResult(res);

            }


        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete([FromBody] System.Text.Json.JsonElement model)
        {

            ClientStateInfo state = null;
            if (User.Identity.IsAuthenticated)
                state = ClientStateInfo.CreateFromJSON(model, User);
            else
                state = ClientStateInfo.CreateFromJSON(model);

            if (state == null)
                return BadRequest();
            if (state.ApplicationId < 1)
                return BadRequest();
            if (state.ApplicationViewId < 1)
                return BadRequest();
            var appmodel = ModelRepository.GetApplicationModel(state.ApplicationId);
            if (appmodel == null)
                return BadRequest();

            var viewmodel = appmodel.Views.Find(p => p.Id == state.ApplicationViewId);
            if (viewmodel == null)
                return BadRequest();

            if (!User.Identity.IsAuthenticated)
                return new JsonResult(new OperationResult(false, MessageCode.USERERROR, "You must log in to use this function"));
            if (!await UserManager.HasAuthorization(User, viewmodel))
                return new JsonResult(new OperationResult(false, MessageCode.USERERROR, string.Format("You are not authorized to delete data in application {0}", appmodel.Application.Title)));

            var res = DataRepository.Delete(state, appmodel);
            return new JsonResult(res);

        }

        [HttpPost]
        public virtual async Task<JsonResult> UploadImage(IFormFile file)
        {
            var uniquefilename = $"{DateTime.Now.Ticks}_{file.FileName}";

            var fileandpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\USERDOC", uniquefilename);
            using (var fs = new FileStream(fileandpath, FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }

            return new JsonResult(new { fileName= uniquefilename });
        }




    }
}
