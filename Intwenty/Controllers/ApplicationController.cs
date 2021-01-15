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
        public virtual async Task<IActionResult> List(int id)
        {
            var path = this.Request.Path.Value;
          
            ApplicationModel current_model = null;
            if (path.ToUpper().Contains("/APPLICATION/") && id > 0)
            {
                current_model = ModelRepository.GetLocalizedApplicationModel(id);
            }
            else
            {
                current_model = ModelRepository.GetLocalizedApplicationModelByPath(path);
            }

            if (current_model == null)
                return NotFound();
            if (!current_model.UseListViewAuthorization)
                return View(current_model);
            if (await UserManager.HasPermission(User, current_model, Areas.Identity.Models.IntwentyPermission.Read))
                return View(current_model);
            else
                return Forbid();

        }

        /// <summary>
        /// Generate create UI based on UIStructure for the application with the supplied application model id.
        /// </summary>
        public virtual async Task<IActionResult> Create(int id)
        {

            var path = this.Request.Path.Value;
            ApplicationModel current_model = null;
            if (path.ToUpper().Contains("/APPLICATION/") && id > 0)
            {
                current_model = ModelRepository.GetLocalizedApplicationModel(id);
            }
            else
            {
                current_model = ModelRepository.GetLocalizedApplicationModelByPath(path);
            }

            if (current_model == null)
                return NotFound();
            if (!current_model.UseCreateViewAuthorization)
                return View(current_model);
            if (await UserManager.HasPermission(User, current_model, Areas.Identity.Models.IntwentyPermission.Modify))
                return View(current_model);
            else
                return Forbid();
        }


        /// <summary>
        /// Generate edit UI based on UIStructure for the application with the supplied application model id and application data id.
        /// </summary>
        public virtual async Task<IActionResult> Edit(int applicationid, int id)
        {
            if (id < 0)
                return NotFound();

            ViewBag.SystemId = Convert.ToString(id);

            var path = this.Request.Path.Value;
            ApplicationModel current_model = null;
            if (path.ToUpper().Contains("/APPLICATION/") && applicationid > 0)
            {
                current_model = ModelRepository.GetLocalizedApplicationModel(applicationid);
            }
            else
            {
                current_model = ModelRepository.GetLocalizedApplicationModelByPath(path);
            }

            if (current_model == null)
                return NotFound();
            if (!current_model.UseEditViewAuthorization)
                return View(current_model);
            if (await UserManager.HasPermission(User, current_model, Areas.Identity.Models.IntwentyPermission.Read))
                return View(current_model);
            else
                return Forbid();

        }

        /// <summary>
        /// Generate a detail presentation UI based on UIStructure for the application with the supplied application model id and application data id.
        /// </summary>
        public virtual async Task<IActionResult> Detail(int applicationid, int id)
        {
            if (id < 0)
                return NotFound();

            ViewBag.SystemId = Convert.ToString(id);

            var path = this.Request.Path.Value;
            ApplicationModel current_model = null;
            if (path.ToUpper().Contains("/APPLICATION/") && applicationid > 0)
            {
                current_model = ModelRepository.GetLocalizedApplicationModel(applicationid);
            }
            else
            {
                current_model = ModelRepository.GetLocalizedApplicationModelByPath(path);
            }

            if (current_model == null)
                return NotFound();
            if (!current_model.UseDetailViewAuthorization)
                return View(current_model);
            if (await UserManager.HasPermission(User, current_model, Areas.Identity.Models.IntwentyPermission.Read))
                return View(current_model);
            else
                return Forbid();

        }


        /// <summary>
        /// Renders a list view for application with supplied application model id.
        /// </summary>
        public virtual async Task<IActionResult> EditList(int id)
        {

            var path = this.Request.Path.Value;
            ApplicationModel current_model = null;
            if (path.ToUpper().Contains("/APPLICATION/") && id > 0)
            {
                current_model = ModelRepository.GetLocalizedApplicationModel(id);
            }
            else
            {
                current_model = ModelRepository.GetLocalizedApplicationModelByPath(path);
            }

            if (current_model == null)
                return NotFound();
            if (!current_model.UseEditViewAuthorization)
                return View(current_model);
            if (await UserManager.HasPermission(User, current_model, Areas.Identity.Models.IntwentyPermission.Read))
                return View(current_model);
            else
                return Forbid();
        }



    }
}
