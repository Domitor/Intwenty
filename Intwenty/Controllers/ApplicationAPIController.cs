using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Intwenty.Model.Dto;
using Microsoft.AspNetCore.Http;
using Intwenty.Interface;
using Intwenty.Areas.Identity.Data;
using Intwenty.Areas.Identity.Models;
using Intwenty.Model;
using System.Collections.Generic;
using System.Linq;

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
            if (viewmodel != null)
            {
               
                ClientStateInfo state = null;
                if (User.Identity.IsAuthenticated)
                    state = new ClientStateInfo(User) { Id = id, ApplicationId = applicationid, ApplicationViewId = viewid, RetrievalMode = ApplicationRetrievalMode.MainTable };
                else
                    state = new ClientStateInfo() { Id = id, ApplicationId = applicationid, ApplicationViewId = viewid, RetrievalMode = ApplicationRetrievalMode.MainTable };

                if (viewmodel.IsPublic)
                {
                    var data = DataRepository.Get(state, model);
                    foreach (var listui in viewmodel.UserInterface)
                    {
                        if (!listui.IsSubTableUserInterface)
                            continue;

                        var filter = new ListFilter(User) { ApplicationId = model.Application.Id, ApplicationViewId = viewmodel.Id, DataTableDbName = listui.DataTableDbName };
                        var subtablearray = DataRepository.GetJsonArray(filter);
                        data.AddApplicationJSONArray(listui.DataTableDbName, subtablearray.Data);

                    }

                    return new JsonResult(data);
                }
                else
                {

                    if (!User.Identity.IsAuthenticated)
                        return new JsonResult(new OperationResult(false, MessageCode.USERERROR, "You do not have access to this resource"));
                    if (!await UserManager.HasAuthorization(User, viewmodel))
                        return new JsonResult(new OperationResult(false, MessageCode.USERERROR, string.Format("You do not have access to this resource, apply for read permission for application {0}", model.Application.Title)));

                    var data = DataRepository.Get(state, model);
                    foreach (var listui in viewmodel.UserInterface)
                    {
                        if (!listui.IsSubTableUserInterface)
                            continue;

                        var filter = new ListFilter(User) { ApplicationId = model.Application.Id, ApplicationViewId = viewmodel.Id, DataTableDbName = listui.DataTableDbName };
                        var subtablearray = DataRepository.GetJsonArray(filter);
                        data.AddApplicationJSONArray(listui.DataTableDbName, subtablearray.Data);

                    }

                    return new JsonResult(data);

                }
            }
            else
            {
                ClientStateInfo state = null;
                if (User.Identity.IsAuthenticated)
                    state = new ClientStateInfo(User) { Id = id, ApplicationId = applicationid, RetrievalMode = ApplicationRetrievalMode.MainTable };
                else
                    state = new ClientStateInfo() { Id = id, ApplicationId = applicationid, RetrievalMode = ApplicationRetrievalMode.MainTable };

                if (!User.Identity.IsAuthenticated)
                    return new JsonResult(new OperationResult(false, MessageCode.USERERROR, "You do not have access to this resource"));
                if (!await UserManager.HasAuthorization(User, model))
                    return new JsonResult(new OperationResult(false, MessageCode.USERERROR, string.Format("You do not have access to this resource, apply for read permission for application {0}", model.Application.Title)));

                var data = DataRepository.Get(state, model);

                foreach (var dt in model.DataStructure)
                {
                    if (!dt.IsMetaTypeDataTable)
                        continue;

                    var filter = new ListFilter(User) { ApplicationId = model.Application.Id, DataTableDbName = dt.DbName };
                    var subtablearray = DataRepository.GetJsonArray(filter);
                    data.AddApplicationJSONArray(dt.DbName, subtablearray.Data);

                }

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

        [HttpGet("/Application/API/GetDomain/{domainname}/{query}")]
        public virtual List<ValueDomainVm> GetDomain(string domainname, string query)
        {
            if (string.IsNullOrEmpty(domainname))
                return new List<ValueDomainVm>();

            if (!domainname.Contains("."))
                return new List<ValueDomainVm>();

            ClientStateInfo state = null;
            if (User.Identity.IsAuthenticated)
                state = new ClientStateInfo(User);
            else
                state = new ClientStateInfo();

            var arr = domainname.Split(".");
            var dtype = arr[0];
            var dname = arr[1];

            var retlist = new List<ValueDomainVm>();


            List<ValueDomainModelItem> domaindata = null;

            if (dtype.ToUpper() == "VALUEDOMAIN")
            {
                domaindata = DataRepository.GetValueDomain(dname);
            }
            if (dtype.ToUpper() == "APPDOMAIN")
            {
                domaindata = DataRepository.GetApplicationDomain(dname, state);
            }

            if (domaindata != null)
            {
                if (query.ToUpper() == "ALL")
                {
                    retlist = domaindata.Select(p => new ValueDomainVm() { Id = p.Id, Code = p.Code, DomainName = dname, Value = p.Value, Display=p.LocalizedTitle }).ToList();
                }
                else if (query.ToUpper() == "PRELOAD")
                {
                    var result = new List<ValueDomainVm>();
                    for (int i = 0; i < domaindata.Count; i++)
                    {
                        var p = domaindata[i];
                        if (i < 50)
                            result.Add(new ValueDomainVm() { Id = p.Id, Code = p.Code, DomainName = dname, Value = p.Value, Display = p.LocalizedTitle });
                        else
                            break;
                    }
                    retlist = result;
                }
                else
                {
                    retlist = domaindata.Select(p => new ValueDomainVm() { Id = p.Id, Code = p.Code, DomainName = dname, Value = p.Value, Display = p.LocalizedTitle }).Where(p=> p.Display.ToLower().Contains(query.ToLower())).ToList();
                }
            }

          
            return retlist;

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

            try
            {


                if (User.Identity.IsAuthenticated)
                    state = ClientStateInfo.CreateFromJSON(model, User);
                else
                    state = ClientStateInfo.CreateFromJSON(model);

                if (state == null)
                    return BadRequest();
                if (state.ApplicationId < 1)
                    return BadRequest();

                var appmodel = ModelRepository.GetApplicationModel(state.ApplicationId);
                if (appmodel == null)
                    return BadRequest();


                if (state.ApplicationViewId > 0)
                {
                    var viewmodel = appmodel.Views.Find(p => p.Id == state.ApplicationViewId);
                    if (viewmodel == null)
                        return BadRequest();

                    if (viewmodel.IsPublic)
                    {
                        var pub_save_res = DataRepository.Save(state, appmodel);
                        if (!pub_save_res.IsSuccess)
                            throw new InvalidOperationException(pub_save_res.UserError);

                        return new JsonResult(pub_save_res);
                    }
                    else
                    {
                        if (!User.Identity.IsAuthenticated)
                            throw new InvalidOperationException("You must login to use this function");

                        if (!await UserManager.HasAuthorization(User, viewmodel))
                            throw new InvalidOperationException(string.Format("You are not authorized to modify data in this view"));

                    }
                    
                }
                else
                {
                    if (!User.Identity.IsAuthenticated)
                        throw new InvalidOperationException("You must login to use this function");

                    if (!await UserManager.HasAuthorization(User, appmodel.Application))
                        throw new InvalidOperationException(string.Format("You are not authorized to modify data in this application"));

                }


                var res = DataRepository.Save(state, appmodel);
                if (!res.IsSuccess)
                    throw new InvalidOperationException(res.UserError);

                return new JsonResult(res);

            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError(ex.Message, "An error occured when saving an application.");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete([FromBody] System.Text.Json.JsonElement model)
        {

            ClientStateInfo state = null;

            try
            {
                if (User.Identity.IsAuthenticated)
                    state = ClientStateInfo.CreateFromJSON(model, User);
                else
                    state = ClientStateInfo.CreateFromJSON(model);

                if (state == null)
                    return BadRequest();
                if (state.ApplicationId < 1)
                    return BadRequest();
              
                var appmodel = ModelRepository.GetApplicationModel(state.ApplicationId);
                if (appmodel == null)
                    return BadRequest();

                if (state.ApplicationViewId > 0)
                {
                    var viewmodel = appmodel.Views.Find(p => p.Id == state.ApplicationViewId);
                    if (viewmodel == null)
                        return BadRequest();

                    if (viewmodel.IsPublic)
                    {
                        var pub_del_res = DataRepository.Delete(state, appmodel);
                        if (!pub_del_res.IsSuccess)
                            throw new InvalidOperationException(pub_del_res.UserError);

                        return new JsonResult(pub_del_res);
                    }
                    else
                    {
                        if (!User.Identity.IsAuthenticated)
                            throw new InvalidOperationException("You must login to use this function");

                        if (!await UserManager.HasAuthorization(User, viewmodel))
                            throw new InvalidOperationException(string.Format("You are not authorized to delete data in this view"));

                    }
                    
                }
                else
                {
                    if (!User.Identity.IsAuthenticated)
                        throw new InvalidOperationException("You must login to use this function");

                    if (!await UserManager.HasAuthorization(User, appmodel.Application))
                        throw new InvalidOperationException(string.Format("You are not authorized to delete data in this application"));

                }

                var res = DataRepository.Delete(state, appmodel);
                if (!res.IsSuccess)
                    throw new InvalidOperationException(res.UserError);

                return new JsonResult(res);

            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError(ex.Message, "An error occured when deleting an application.");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteTableLine([FromBody] System.Text.Json.JsonElement model)
        {

            ClientStateInfo state = null;

            try
            {
                ApplicationData lineinfo = ApplicationData.CreateFromJSON(model);
                var lineid = lineinfo.GetAsInt("Id").Value;
                var appid = lineinfo.GetAsInt("ApplicationId").Value;
                var viewid = lineinfo.GetAsInt("ApplicationViewId").Value;
                var tablename = lineinfo.GetAsString("TableName");
                var parentid = lineinfo.GetAsInt("ParentId").Value;

                if (appid < 1)
                    return BadRequest();

                if (lineid < 1)
                    return BadRequest();

                if (string.IsNullOrEmpty(tablename))
                    return BadRequest();

                if (User.Identity.IsAuthenticated)
                    state = new ClientStateInfo(User) { Id=parentid, ApplicationId = appid, ApplicationViewId = viewid };
                else
                    state = new ClientStateInfo() { Id=parentid, ApplicationId = appid, ApplicationViewId = viewid };


                var appmodel = ModelRepository.GetApplicationModel(state.ApplicationId);
                if (appmodel == null)
                    return BadRequest();


                if (state.ApplicationViewId > 0)
                {
                    var viewmodel = appmodel.Views.Find(p => p.Id == state.ApplicationViewId);
                    if (viewmodel == null)
                        return BadRequest();

                    if (viewmodel.IsPublic)
                    {
                        var pub_del_res = DataRepository.DeleteTableLine(state, appmodel, lineid, tablename);
                        if (!pub_del_res.IsSuccess)
                            throw new InvalidOperationException(pub_del_res.UserError);

                        return new JsonResult(pub_del_res);
                    }
                    else
                    {
                        if (!await UserManager.HasAuthorization(User, viewmodel))
                            throw new InvalidOperationException(string.Format("You are not authorized to delete data in this view"));

                     }
                    
                }
                else
                {
                    if (!User.Identity.IsAuthenticated)
                        throw new InvalidOperationException("You must login to use this function");

                    if (!await UserManager.HasAuthorization(User, appmodel.Application))
                        throw new InvalidOperationException(string.Format("You are not authorized to delete data in this application"));

                }


                var res = DataRepository.DeleteTableLine(state, appmodel, lineid, tablename);
                if (!res.IsSuccess)
                    throw new InvalidOperationException(res.UserError);

                return new JsonResult(res);

            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError(ex.Message, "An error occured when deleting a sub table line.");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

        }

        [HttpPost]
        public virtual async Task<IActionResult> SaveSubTableLine([FromBody] System.Text.Json.JsonElement model)
        {

            ClientStateInfo state = null;

            try
            {
                ApplicationData lineinfo = ApplicationData.CreateFromJSON(model);
                var lineid = lineinfo.GetAsInt("Id").Value;
                var appid = lineinfo.GetAsInt("ApplicationId").Value;
                var viewid = lineinfo.GetAsInt("ApplicationViewId").Value;
                var tablename = lineinfo.GetAsString("TableName");
                var parentid = lineinfo.GetAsInt("ParentId").Value;

                if (appid < 1)
                    return BadRequest();


                if (string.IsNullOrEmpty(tablename))
                    return BadRequest();

                if (User.Identity.IsAuthenticated)
                    state = new ClientStateInfo(User) { Id = parentid, ApplicationId = appid, ApplicationViewId = viewid };
                else
                    state = new ClientStateInfo() { Id = parentid, ApplicationId = appid, ApplicationViewId = viewid };


                var appmodel = ModelRepository.GetApplicationModel(state.ApplicationId);
                if (appmodel == null)
                    return BadRequest();


                if (state.ApplicationViewId > 0)
                {
                    var viewmodel = appmodel.Views.Find(p => p.Id == state.ApplicationViewId);
                    if (viewmodel == null)
                        return BadRequest();

                    if (viewmodel.IsPublic)
                    {
                        var pub_del_res = DataRepository.SaveSubTableLine(state, appmodel, lineid, tablename);
                        if (!pub_del_res.IsSuccess)
                            throw new InvalidOperationException(pub_del_res.UserError);

                        return new JsonResult(pub_del_res);
                    }
                    else
                    {
                        if (!await UserManager.HasAuthorization(User, viewmodel))
                            throw new InvalidOperationException(string.Format("You are not authorized to modify data in this view"));

                    }

                }
                else
                {
                    if (!User.Identity.IsAuthenticated)
                        throw new InvalidOperationException("You must login to use this function");

                    if (!await UserManager.HasAuthorization(User, appmodel.Application))
                        throw new InvalidOperationException(string.Format("You are not authorized to modify data in this application"));

                }


                var res = DataRepository.SaveSubTableLine(state, appmodel, lineid, tablename);
                if (!res.IsSuccess)
                    throw new InvalidOperationException(res.UserError);

                return new JsonResult(res);

            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError(ex.Message, "An error occured when saving a sub table line.");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

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
