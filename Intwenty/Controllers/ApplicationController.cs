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
            ViewBag.RequestInfo = "";
            ViewBag.Id = 0;

            if (id.HasValue && id.Value > 0)
                ViewBag.Id = id.Value;

            int viewid = 0;
            int applicationid = 0;

            if (!string.IsNullOrEmpty(requestinfo))
            {
                var props = new HashTagPropertyObject();
                props.Properties = requestinfo.B64UrlDecode();

                var pid = props.GetAsInt("ID");
                if (pid > 0 && ViewBag.Id == 0)
                    ViewBag.Id = pid;

                var vid = props.GetAsInt("VIEWID");
                if (vid > 0)
                    viewid = vid;

                var aid = props.GetAsInt("APPLICATIONID");
                if (aid > 0)
                    applicationid = aid;
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
            if (viewid > 0)
            {
                current_view = ModelRepository.GetLocalizedViewModelById(viewid);
            }
            else
            {
                var path = this.Request.Path.Value;
                current_view = ModelRepository.GetLocalizedViewModelByPath(path);
            }

            if (current_view == null)
                return NotFound();

            current_view.RuntimeRequestInfo = requestinfo;


            var viewpath = current_view.GetPropertyValue("RAZORVIEWPATH");
            if (string.IsNullOrEmpty(viewpath))
                viewpath = "View";

            if (current_view.IsPublic)
                return View(viewpath,current_view);
            if (await UserManager.HasAuthorization(User, current_view))
                return View(viewpath,current_view);
            else
                return Forbid();


        }





    }
}