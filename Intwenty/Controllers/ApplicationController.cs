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



        public virtual async Task<IActionResult> View(int? id)
        {

            ViewBag.Id = 0;
            if (id.HasValue && id.Value > 0)
                ViewBag.Id = id;

            var path = this.Request.Path.Value;
            var current_view = ModelRepository.GetLocalizedViewModelByPath(path);

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