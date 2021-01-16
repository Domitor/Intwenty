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
    public class UserModel : PageModel
    {

        private IIntwentyDataService DataRepository { get; }
        private IIntwentyModelService ModelRepository { get; }
        private UserManager<IntwentyUser> UserManager { get; }

        public string Id { get; set; }

        public UserModel(IIntwentyDataService ms, IIntwentyModelService sr, UserManager<IntwentyUser> umgr)
        {
            DataRepository = ms;
            ModelRepository = sr;
            UserManager = umgr;
        }

        public void OnGet(string id)
        {
            Id = id;   
        }

        public async Task<JsonResult> OnGetLoad(string id)
        {
            var client = DataRepository.GetIAMDataClient();
            await client.OpenAsync();
            var result = await client.GetEntityAsync<IntwentyUser>(id);
            await client.CloseAsync();
            return new JsonResult(new IntwentyUserVm(result));
        }


        /*
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
        */


    }
}
