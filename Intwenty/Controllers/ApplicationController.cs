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
using Intwenty.Helpers;

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



        public virtual async Task<IActionResult> View(int? id, string requestinfo)
        {
            ViewBag.ApplicationId=0;
            ViewBag.ApplicationViewId=0;
            ViewBag.Id = 0;

            if (id.HasValue && id.Value > 0)
                ViewBag.Id = id.Value;

            ViewBag.RequestInfo = "";
            if (!string.IsNullOrEmpty(requestinfo))
            {
                ViewBag.RequestInfo = requestinfo;

                var props = new HashTagPropertyObject();
                props.Properties = requestinfo.B64UrlDecode();

                var pid = props.GetAsInt("ID");
                if (pid > 0 && ViewBag.Id == 0)
                    ViewBag.Id = pid;

                var vid = props.GetAsInt("VIEWID");
                if (vid > 0)
                    ViewBag.ApplicationViewId = vid;

                var aid = props.GetAsInt("APPLICATIONID");
                if (aid > 0)
                    ViewBag.ApplicationId = aid;
            }
            
           
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
            if (ViewBag.ApplicationViewId > 0)
            {
                current_view = ModelRepository.GetLocalizedViewModelById(ViewBag.ApplicationViewId);
            }
            else
            {
                var path = this.Request.Path.Value;
                current_view = ModelRepository.GetLocalizedViewModelByPath(path);
            }

         

            if (current_view == null)
                return NotFound();

            ViewBag.ApplicationId = current_view.ApplicationInfo.Id;
            ViewBag.ApplicationViewId = current_view.Id;

            if (current_view.IsPublic)
                return View(current_view);
            if (await UserManager.HasAuthorization(User, current_view))
                return View(current_view);
            else
                return Forbid();


        }





    }
}