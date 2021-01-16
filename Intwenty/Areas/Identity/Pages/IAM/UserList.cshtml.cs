using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intwenty.Areas.Identity.Entity;
using Intwenty.Interface;
using Intwenty.Areas.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Intwenty.Areas.Identity.Pages.IAM
{
    [Authorize(Policy = "IntwentyModelAuthorizationPolicy")]
    public class UserListModel : PageModel
    {

        private IIntwentyDataService DataRepository { get; }
        private IIntwentyModelService ModelRepository { get; }
        private UserManager<IntwentyUser> UserManager { get; }

        public UserListModel(IIntwentyDataService ms, IIntwentyModelService sr, UserManager<IntwentyUser> umgr)
        {
            DataRepository = ms;
            ModelRepository = sr;
            UserManager = umgr;
        }

        public void OnGet()
        {
           
        }

        public async Task<JsonResult> OnGetLoad()
        {
            var client = DataRepository.GetIAMDataClient();
            await client.OpenAsync();
            var result = await client.GetEntitiesAsync<IntwentyUser>();
            var list = result.Select(p => new IntwentyUserVm(p));
            await client.CloseAsync();
            return new JsonResult(list);
        }

        public async Task<JsonResult> Load()
        {
            var client = DataRepository.GetIAMDataClient();
            await client.OpenAsync();
            var result = await client.GetEntitiesAsync<IntwentyUser>();
            var list = result.Select(p => new IntwentyUserVm(p));
            await client.CloseAsync();
            return new JsonResult(list);
        }

        public async Task<JsonResult> OnPostAddUser([FromBody] IntwentyUserVm model)
        {
            var user = new IntwentyUser();
            user.UserName = model.Email;
            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.EmailConfirmed = true;

            await UserManager.CreateAsync(user);

            return await Load();
        }

        public JsonResult OnPostBlockUser([FromBody] IntwentyUserVm model)
        {

            //Requires SetLockoutEnabled in startup.cs
            var user = UserManager.FindByIdAsync(model.Id).Result;
            if (user != null)
            {
                UserManager.SetLockoutEndDateAsync(user, DateTime.Now.AddYears(100));
            }

            return Load().Result;
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


            return Load().Result;
        }

        public JsonResult OnPostResetMFA([FromBody] IntwentyUserVm model)
        {

            //Requires SetLockoutEnabled in startup.cs
            var user = UserManager.FindByIdAsync(model.Id).Result;
            if (user != null)
            {
                UserManager.SetTwoFactorEnabledAsync(user, false);
            }


            return Load().Result;
        }

        public JsonResult OnPostDeleteUser([FromBody] IntwentyUserVm model)
        {
            var user = UserManager.FindByIdAsync(model.Id).Result;
            if (user != null)
                UserManager.DeleteAsync(user);


            return Load().Result;
        }

    }
}
