using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intwenty.Areas.Identity.Models;
using Intwenty.Interface;
using Intwenty.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Intwenty.Areas.Identity.Pages.Account.Admin
{
    [Authorize(Policy = "IntwentyModelAuthorizationPolicy")]
    public class UserAdministrationModel : PageModel
    {

        private IIntwentyDataService DataRepository { get; }
        private IIntwentyModelService ModelRepository { get; }

        private UserManager<IntwentyUser> UserManager { get; }

        public UserAdministrationModel(IIntwentyDataService ms, IIntwentyModelService sr, UserManager<IntwentyUser> umgr)
        {
            DataRepository = ms;
            ModelRepository = sr;
            UserManager = umgr;
        }

        public void OnGet()
        {
           
        }

        public JsonResult OnGetLoadUsers()
        {
            var mapper = DataRepository.GetDataClient();
            var list = mapper.GetEntities<IntwentyUser>().Select(p => new IntwentyUserVm(p));
            return new JsonResult(list);
        }

        public JsonResult LoadUsers()
        {
            var mapper = DataRepository.GetDataClient();
            var list = mapper.GetEntities<IntwentyUser>().Select(p => new IntwentyUserVm(p));
            return new JsonResult(list);
        }

        public JsonResult OnPostBlockUser([FromBody] IntwentyUserVm model)
        {

            //Requires SetLockoutEnabled in startup.cs
            var user = UserManager.FindByIdAsync(model.Id).Result;
            if (user != null)
            {
                UserManager.SetLockoutEndDateAsync(user, DateTime.Now.AddYears(100));
            }


            return LoadUsers();
        }


        public JsonResult OnPostUnblockUser([FromBody] IntwentyUserVm model)
        {

            //Requires SetLockoutEnabled in startup.cs
            var user = UserManager.FindByIdAsync(model.Id).Result;
            if (user != null)
            {
                UserManager.ResetAccessFailedCountAsync(user);
                UserManager.SetLockoutEndDateAsync(user, null);
            }


            return LoadUsers();
        }

        public JsonResult OnPostResetMFA([FromBody] IntwentyUserVm model)
        {

            //Requires SetLockoutEnabled in startup.cs
            var user = UserManager.FindByIdAsync(model.Id).Result;
            if (user != null)
            {
                UserManager.SetTwoFactorEnabledAsync(user, false);
            }


            return LoadUsers();
        }

        public JsonResult OnPostDeleteUser([FromBody] IntwentyUserVm model)
        {
            var user = UserManager.FindByIdAsync(model.Id).Result;
            if (user != null)
                UserManager.DeleteAsync(user);


            return LoadUsers();
        }

    }
}
