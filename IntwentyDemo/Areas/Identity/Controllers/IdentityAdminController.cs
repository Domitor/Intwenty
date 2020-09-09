using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Intwenty.Model.DesignerVM;
using Intwenty.Model;
using Microsoft.AspNetCore.Authorization;
using Intwenty.Data.Dto;
using Intwenty.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using Intwenty.Data.DBAccess.Helpers;
using Intwenty.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Intwenty;

namespace IntwentyDemo.Areas.Identity.Controllers
{

    [Area("Identity")]
    [Authorize(Policy = "IntwentyModelAuthorizationPolicy")]
    public class IdentityAdminController : Controller
    {
        private IIntwentyDataService DataRepository { get; }
        private IIntwentyModelService ModelRepository { get; }

        private UserManager<IntwentyUser> UserManager { get; }

        public IdentityAdminController(IIntwentyDataService ms, IIntwentyModelService sr, UserManager<IntwentyUser> umgr)
        {
            DataRepository = ms;
            ModelRepository = sr;
            UserManager = umgr;
        }

        [HttpGet("Identity/IdentityAdmin/UserAdministration")]
        public IActionResult UserAdministration()
        {
            return View();
        }

        [HttpGet("Identity/IdentityAdmin/GetUsers")]
        public JsonResult GetUsers()
        {
            var mapper = DataRepository.GetDbObjectMapper();
            var list = mapper.GetAll<IntwentyUser>().Select(p=> new IntwentyUserVm(p));
            return new JsonResult(list);
        }

        [HttpPost("/IdentityAdmin/BlockUser")]
        public JsonResult BlockUser([FromBody] IntwentyUserVm model)
        {

            //Requires SetLockoutEnabled in startup.cs
            var user = UserManager.FindByIdAsync(model.Id).Result;
            if (user != null)
            {
                UserManager.SetLockoutEndDateAsync(user, DateTime.Now.AddYears(100));
            }
            

            return GetUsers();
        }

        [HttpPost("/IdentityAdmin/UnblockUser")]
        public JsonResult UnblockUser([FromBody] IntwentyUserVm model)
        {

            //Requires SetLockoutEnabled in startup.cs
            var user = UserManager.FindByIdAsync(model.Id).Result;
            if (user != null)
            {
                UserManager.ResetAccessFailedCountAsync(user);
                UserManager.SetLockoutEndDateAsync(user, null);
            }


            return GetUsers();
        }

        [HttpPost("/IdentityAdmin/ResetMFA")]
        public JsonResult ResetMFA([FromBody] IntwentyUserVm model)
        {

            //Requires SetLockoutEnabled in startup.cs
            var user = UserManager.FindByIdAsync(model.Id).Result;
            if (user != null)
            {
                UserManager.SetTwoFactorEnabledAsync(user, false);
            }


            return GetUsers();
        }

        [HttpPost("/IdentityAdmin/DeleteUser")]
        public JsonResult DeleteUser([FromBody] IntwentyUserVm model)
        {
            var user = UserManager.FindByIdAsync(model.Id).Result;
            if (user != null)
                UserManager.DeleteAsync(user);


            return GetUsers();
        }




    }
}
