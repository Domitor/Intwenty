using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Intwenty.Model.Dto;
using Microsoft.AspNetCore.Http;
using Intwenty.Interface;
using Intwenty.Areas.Identity.Data;
using Intwenty.Model;

namespace Intwenty.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Policy = "IntwentyAppAuthorizationPolicy")]
    public class ApplicationController : Controller
    {

        private IIntwentyDataService DataRepository { get; }
        private IIntwentyModelService ModelRepository { get; }
        private IntwentyUserManager UserManager { get; }

        public ApplicationController(IIntwentyDataService dataservice, IIntwentyModelService modelservice, IntwentyUserManager usermanager)
        {
            DataRepository = dataservice;
            ModelRepository = modelservice;
            UserManager = usermanager;
        }



        public virtual async Task<IActionResult> View(int? viewid, int? id, string requestinfo)
        {
            
            ViewBag.Id = 0;
            if (id.HasValue && id.Value > 0)
                ViewBag.Id = id.Value;

            ViewBag.RequestInfo = "";
            if (!string.IsNullOrEmpty(requestinfo))
                ViewBag.RequestInfo = requestinfo;
            
           
            ViewBag.BaseAPIPath = Url.Content("~/Application/API/");
            ViewBag.SaveAPIPath = Url.Content("~/Application/API/Save");
            ViewBag.SaveLineAPIPath = Url.Content("~/Application/API/SaveSubTableLine");
            ViewBag.GetApplicationAPIPath = Url.Content("~/Application/API/GetApplication");
            ViewBag.GetListAPIPath = Url.Content("~/Application/API/GetPagedList");
            ViewBag.GetDomainAPIPath = Url.Content("~/Application/API/GetDomain");
            ViewBag.DeleteApplicationAPIPath = Url.Content("~/Application/API/Delete");
            ViewBag.DeleteLineAPIPath = Url.Content("~/Application/API/DeleteSubTableLine");
            ViewBag.CreateNewAPIPath = Url.Content("~/Application/API/CreateNew");

            ViewModel current_view = null;
            if (viewid.HasValue && viewid.Value > 0)
            {
                current_view = ModelRepository.GetLocalizedViewModelById(viewid.Value);
            }
            else
            {
                var path = this.Request.Path.Value;
                current_view = ModelRepository.GetLocalizedViewModelByPath(path);
            }

            if (current_view == null)
                return NotFound();
            if (current_view.IsPublic)
                return View(current_view);
            if (await UserManager.HasAuthorization(User, current_view))
                return View(current_view);
            else
                return Forbid();


        }





    }
}