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

        /// <summary>
        /// Generate a presentation UI based on UIStructure for the application with the supplied application model
        /// </summary>
        public virtual IActionResult List(int id)
        {
            var path = this.Request.Path.Value;
            var apps = ModelRepository.GetLocalizedApplicationModels();
            ApplicationModel t = null;
            if (path.ToUpper().Contains("/APPLICATION/") && id > 0)
                t = apps.Find(p => p.Application.Id == id);
            else
                t = apps.Find(p => !string.IsNullOrEmpty(p.Application.ApplicationPath) && path.ToUpper().Contains(p.Application.ApplicationPath.ToUpper()));

            if (t == null)
                return NotFound();
            if (!t.UseListViewAuthorization)
                return View(t);
            if (UserManager.HasPermission(User, t, Areas.Identity.Models.IntwentyPermission.Read))
                return View(t);
            else
                return Forbid();

        }

        /// <summary>
        /// Generate create UI based on UIStructure for the application with the supplied application model id.
        /// </summary>
        public virtual IActionResult Create(int id)
        {

            var path = this.Request.Path.Value;
            var apps = ModelRepository.GetLocalizedApplicationModels();
            ApplicationModel current_model = null;
            if (path.ToUpper().Contains("/APPLICATION/") && id > 0)
                current_model = apps.Find(p => p.Application.Id == id);
            else
                current_model = apps.Find(p => !string.IsNullOrEmpty(p.Application.ApplicationPath) && path.ToUpper().Contains(p.Application.ApplicationPath.ToUpper()));

            if (current_model == null)
                return NotFound();
            if (!current_model.UseCreateViewAuthorization)
                return View(current_model);
            if (UserManager.HasPermission(User, current_model, Areas.Identity.Models.IntwentyPermission.Modify))
                return View(current_model);
            else
                return Forbid();
        }


        /// <summary>
        /// Generate edit UI based on UIStructure for the application with the supplied application model id and application data id.
        /// </summary>
        public virtual IActionResult Edit(int applicationid, int id)
        {
            if (id < 0)
                return NotFound();

            ViewBag.SystemId = Convert.ToString(id);

            var path = this.Request.Path.Value;
            var apps = ModelRepository.GetLocalizedApplicationModels();
            ApplicationModel t = null;
            if (path.ToUpper().Contains("/APPLICATION/") && applicationid > 0)
                t = apps.Find(p => p.Application.Id == applicationid);
            else
                t = apps.Find(p => !string.IsNullOrEmpty(p.Application.ApplicationPath) && path.ToUpper().Contains(p.Application.ApplicationPath.ToUpper()));

            if (t == null)
                return NotFound();
            if (!t.UseEditViewAuthorization)
                return View(t);
            if (UserManager.HasPermission(User, t, Areas.Identity.Models.IntwentyPermission.Read))
                return View(t);
            else
                return Forbid();

        }

        /// <summary>
        /// Generate a detail presentation UI based on UIStructure for the application with the supplied application model id and application data id.
        /// </summary>
        public virtual IActionResult Detail(int applicationid, int id)
        {
            if (id < 0)
                return NotFound();

            ViewBag.SystemId = Convert.ToString(id);

            var path = this.Request.Path.Value;
            var apps = ModelRepository.GetLocalizedApplicationModels();
            ApplicationModel t = null;
            if (path.ToUpper().Contains("/APPLICATION/") && applicationid > 0)
                t = apps.Find(p => p.Application.Id == applicationid);
            else
                t = apps.Find(p => !string.IsNullOrEmpty(p.Application.ApplicationPath) && path.ToUpper().Contains(p.Application.ApplicationPath.ToUpper()));

            if (t == null)
                return NotFound();
            if (!t.UseDetailViewAuthorization)
                return View(t);
            if (UserManager.HasPermission(User, t, Areas.Identity.Models.IntwentyPermission.Read))
                return View(t);
            else
                return Forbid();

        }


        /// <summary>
        /// Renders a list view for application with supplied application model id.
        /// </summary>
        public virtual IActionResult EditList(int id)
        {

            var path = this.Request.Path.Value;
            var apps = ModelRepository.GetLocalizedApplicationModels();
            ApplicationModel t = null;
            if (path.ToUpper().Contains("/APPLICATION/") && id > 0)
                t = apps.Find(p => p.Application.Id == id);
            else
                t = apps.Find(p => !string.IsNullOrEmpty(p.Application.ApplicationPath) && path.ToUpper().Contains(p.Application.ApplicationPath.ToUpper()));

            if (t == null)
                return NotFound();
            if (!t.UseEditViewAuthorization)
                return View(t);
            if (UserManager.HasPermission(User, t, Areas.Identity.Models.IntwentyPermission.Read))
                return View(t);
            else
                return Forbid();
        }



    }
}
