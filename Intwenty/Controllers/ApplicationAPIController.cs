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
        /// Used by the EditView
        /// </summary>
        /// <param name="applicationid">The ID of the application in the meta model</param>
        /// <param name="id">The data id</param>
        [HttpGet]
        public virtual async Task<IActionResult> GetEditData(int applicationid, int id)
        {
            var model = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == applicationid);

            if (model == null || id < 1)
                return BadRequest();

            if (!model.UseEditViewAuthorization)
            {
                var state = new ClientStateInfo() { Id = id, ApplicationId = applicationid };
                var data = DataRepository.Get(state, model);
                return new JsonResult(data);
            } 
            else 
            {

                if (!User.Identity.IsAuthenticated)
                    return new JsonResult(new OperationResult(false, MessageCode.USERERROR, "You do not have access to this resource"));
                if (!await UserManager.HasPermission(User, model, IntwentyPermission.Read))
                    return new JsonResult(new OperationResult(false, MessageCode.USERERROR, string.Format("You do not have access to this resource, apply for read permission for application {0}", model.Application.Title)));

                var state = new ClientStateInfo() { Id = id, ApplicationId = applicationid };

                if (model.Application.EditViewRequirement == "OWNER")
                    state.FilterValues.Add(new FilterValue() { Name = "OwnedBy", Value = User.Identity.Name });
              
                var data = DataRepository.Get(state, model);
                return new JsonResult(data);
                
            }

        }

        /// <summary>
        /// Get the latest version data by id for an application with applicationid
        /// Used by the DetailView
        /// </summary>
        /// <param name="applicationid">The ID of the application in the meta model</param>
        /// <param name="id">The data id</param>
        [HttpGet]
        public virtual async Task<IActionResult> GetDetailData(int applicationid, int id)
        {
            var model = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == applicationid);

            if (model == null || id < 1)
                return BadRequest();

            if (!model.UseDetailViewAuthorization)
            {
                var state = new ClientStateInfo() { Id = id, ApplicationId = applicationid };
                var data = DataRepository.Get(state, model);
                return new JsonResult(data);
            }
            else
            {
                if (!User.Identity.IsAuthenticated)
                    return new JsonResult(new OperationResult(false, MessageCode.USERERROR, "You do not have access to this resource"));
                if (!await UserManager.HasPermission(User, model, IntwentyPermission.Read))
                    return new JsonResult(new OperationResult(false, MessageCode.USERERROR, string.Format("You do not have access to this resource, apply for read permission for application {0}", model.Application.Title)));

                var state = new ClientStateInfo() { Id = id, ApplicationId = applicationid };

                if (model.Application.DetailViewRequirement == "OWNER")
                    state.FilterValues.Add(new FilterValue() { Name = "OwnedBy", Value = User.Identity.Name });

                var data = DataRepository.Get(state, model);
                return new JsonResult(data);

            }

        }

        /// <summary>
        /// Get the latest version of the latest application owned by the logged in user 
        /// </summary>
        /// <param name="applicationid">The ID of the application in the model</param>
        [HttpGet]
        public virtual async Task<IActionResult> GetLatestByLoggedInUser(int applicationid)
        {

            if (applicationid < 1)
                return BadRequest();

            var model = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == applicationid);

            if (model == null)
                return BadRequest();

            if (!User.Identity.IsAuthenticated)
                return new JsonResult(new OperationResult(false, MessageCode.USERERROR, "You do not have access to this resource"));
            if (!await UserManager.HasPermission(User, model, IntwentyPermission.Read))
                return new JsonResult(new OperationResult(false, MessageCode.USERERROR, string.Format("You do not have access to this resource, apply for read permission for application {0}", model.Application.Title)));

          
            var state = new ClientStateInfo() { ApplicationId = applicationid };
            state.FilterValues.Add(new FilterValue() { Name = "OwnedBy", Value = User.Identity.Name });
            var data = DataRepository.GetLatestByOwnerUser(state);
            return new JsonResult(data);

        }

        /// <summary>
        /// Loads data for a listview for the application with supplied Id
        /// </summary>
        [HttpPost]
        public virtual async Task<IActionResult> GetEditListData([FromBody] ListFilter model)
        {
           

            if (model == null)
                return BadRequest();
            if (model.ApplicationId < 1)
                return BadRequest();

            var appmodel = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == model.ApplicationId);
            if (appmodel == null)
                return BadRequest();

            if (!appmodel.UseEditListViewAuthorization)
            {
                var listdata = DataRepository.GetPagedJsonArray(model, appmodel);
                return new JsonResult(listdata);
            }
            else
            {
                if (!User.Identity.IsAuthenticated)
                    return new JsonResult(new OperationResult(false, MessageCode.USERERROR, "You do not have access to this resource"));
                if (!await UserManager.HasPermission(User, appmodel, IntwentyPermission.Read))
                    return new JsonResult(new OperationResult(false, MessageCode.USERERROR, string.Format("You do not have access to this resource, apply for read permission for application {0}", appmodel.Application.Title)));

                if (appmodel.Application.EditListViewRequirement == "OWNER")
                    model.OwnerUserId = User.Identity.Name;
                  

                var listdata = DataRepository.GetPagedJsonArray(model, appmodel);
                return new JsonResult(listdata);

            }

           
        }

        /// <summary>
        /// Loads data for a listview for the application with supplied Id
        /// </summary>
        [HttpPost]
        public virtual async Task<IActionResult> GetListData([FromBody] ListFilter model)
        {
            if (model == null)
                return BadRequest();
            if (model.ApplicationId < 1)
                return BadRequest();

            var appmodel = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == model.ApplicationId);
            if (appmodel == null)
                return BadRequest();

            if (!appmodel.UseListViewAuthorization)
            {
                var listdata = DataRepository.GetPagedJsonArray(model, appmodel);
                return new JsonResult(listdata);
            }
            else
            {
                if (!User.Identity.IsAuthenticated)
                    return new JsonResult(new OperationResult(false, MessageCode.USERERROR, "You do not have access to this resource"));
                if (!await UserManager.HasPermission(User, appmodel, IntwentyPermission.Read))
                    return new JsonResult(new OperationResult(false, MessageCode.USERERROR, string.Format("You do not have access to this resource, apply for read permission for application {0}", appmodel.Application.Title)));

                if (appmodel.Application.EditListViewRequirement == "OWNER")
                    model.OwnerUserId = User.Identity.Name;

                var listdata = DataRepository.GetPagedJsonArray(model, appmodel);
                return new JsonResult(listdata);

            }
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
        /// Get a dataview record by a search value and a view name.
        /// Used from the LOOKUP Control
        /// </summary>
        [HttpPost]
        public virtual JsonResult GetDataViewValue([FromBody] ListFilter model)
        {
            var viewitem = DataRepository.GetDataViewRecord(model);
            return new JsonResult(viewitem);
        }

        /// <summary>
        /// Get a dataview record by a search value and a view name.
        /// Used from the LOOKUP Control
        /// </summary>
        [HttpPost]
        public virtual JsonResult GetDataView([FromBody] ListFilter model)
        {
            var dv = DataRepository.GetDataView(model);
            return new JsonResult(dv);
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
      
            var state = ClientStateInfo.CreateFromJSON(model);

            if (state == null)
                return BadRequest();
            if (state.ApplicationId < 1)
                return BadRequest();

            var appmodel = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == state.ApplicationId);
            if (appmodel == null)
                return BadRequest();

            if (!appmodel.UseCreateViewAuthorization && state.Id < 1)
            {
                var res = DataRepository.Save(state, appmodel);
                return new JsonResult(res);
            }
            else if (!appmodel.UseEditViewAuthorization && state.Id > 0)
            {
                var res = DataRepository.Save(state, appmodel);
                return new JsonResult(res);
            }
            else
            {
                if (!User.Identity.IsAuthenticated)
                    return new JsonResult(new OperationResult(false, MessageCode.USERERROR, "You must login to use this function"));
                if (!await UserManager.HasPermission(User, appmodel, IntwentyPermission.Modify))
                    return new JsonResult(new OperationResult(false, MessageCode.USERERROR, string.Format("You are not authorized to modify data in this application, apply for modify permission in application {0}", appmodel.Application.Title)));

                //if (appmodel.Application.EditViewRequirement == "OWNER")
                //    TODO: IF UPDATE, CHECK THAT THE UPDATER OWNS THE DATA

                state.UserId = User.Identity.Name;

                var res = DataRepository.Save(state, appmodel);
                return new JsonResult(res);

            }


        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete([FromBody] System.Text.Json.JsonElement model)
        {

            var state = ClientStateInfo.CreateFromJSON(model);

            if (state == null)
                return BadRequest();
            if (state.ApplicationId < 1)
                return BadRequest();

            var appmodel = ModelRepository.GetApplicationModels().Find(p => p.Application.Id == state.ApplicationId);
            if (appmodel == null)
                return BadRequest();

            if (!User.Identity.IsAuthenticated)
                return new JsonResult(new OperationResult(false, MessageCode.USERERROR, "You must log in to use this function"));
            if (!await UserManager.HasPermission(User, appmodel, IntwentyPermission.Delete))
                return new JsonResult(new OperationResult(false, MessageCode.USERERROR, string.Format("You are not authorized to delete data in this application, apply for delete permission in application {0}", appmodel.Application.Title)));

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
