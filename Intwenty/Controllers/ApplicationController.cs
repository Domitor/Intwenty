using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Intwenty.Model.Dto;
using Microsoft.AspNetCore.Http;
using Intwenty.Interface;
using Intwenty.Areas.Identity.Data;

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
            var t = ModelRepository.GetLocalizedApplicationModels().Find(p => p.Application.Id == id);
            if (t == null)
                return NotFound();
            if (!t.UseViewAuthorization)
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
           
            var t = ModelRepository.GetLocalizedApplicationModels().Find(p=> p.Application.Id == id);
            if (t == null)
                return NotFound();
            if (!t.UseCreateAuthorization)
                return View(t);
            if (UserManager.HasPermission(User, t, Areas.Identity.Models.IntwentyPermission.Read))
                return View(t);
            else
                return Forbid();
        }


        /// <summary>
        /// Generate edit UI based on UIStructure for the application with the supplied application model id and application data id.
        /// </summary>
        public virtual IActionResult Edit(int applicationid, int id)
        {
            
            ViewBag.SystemId = Convert.ToString(id);
            var t = ModelRepository.GetLocalizedApplicationModels().Find(p => p.Application.Id == applicationid);
            if (t == null)
                return NotFound();
            if (!t.UseModifyAuthorization)
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
            ViewBag.SystemId = Convert.ToString(id);
            var t = ModelRepository.GetLocalizedApplicationModels().Find(p => p.Application.Id == applicationid);
            if (t == null)
                return NotFound();
            if (!t.UseViewAuthorization)
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

            var t = ModelRepository.GetLocalizedApplicationModels().Find(p => p.Application.Id == id);
            if (t == null)
                return NotFound();
            if (!t.UseModifyAuthorization)
                return View(t);
            if (UserManager.HasPermission(User, t, Areas.Identity.Models.IntwentyPermission.Read))
                return View(t);
            else
                return Forbid();
        }



    }
}
