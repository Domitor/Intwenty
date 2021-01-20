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
using Intwenty.Areas.Identity.Data;

namespace Intwenty.Areas.Identity.Pages.IAM
{
    [Authorize(Policy = "IntwentyUserAdminAuthorizationPolicy")]
    public class UserModel : PageModel
    {

        private IIntwentyDataService DataRepository { get; }
        private IIntwentyModelService ModelRepository { get; }
        private IntwentyUserManager UserManager { get; }

        public string Id { get; set; }

        public UserModel(IIntwentyDataService ms, IIntwentyModelService sr, IntwentyUserManager usermanager)
        {
            DataRepository = ms;
            ModelRepository = sr;
            UserManager = usermanager;
        }

        public void OnGet(string id)
        {
            Id = id;   
        }

        public async Task<IActionResult> OnGetLoad(string id)
        {
            var result = await UserManager.FindByIdAsync(id);
            var model = new IntwentyUserVm(result);
            model.UserProducts = await UserManager.GetOrganizationProductsAsync(result);
            return new JsonResult(model);
        }


        
        public async Task<IActionResult> OnPostUpdateEntity([FromBody] IntwentyUserVm model)
        {

            var user = await UserManager.FindByNameAsync(model.UserName);
            if (user != null)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                await UserManager.UpdateAsync(user);
                return await OnGetLoad(user.Id);
            }

            return new JsonResult("{}");
          
        }

        public async Task<IActionResult> OnPostCreateAPIKey([FromBody] IntwentyUserVm model)
        {
            var user = await UserManager.FindByNameAsync(model.UserName);
            if (user != null)
            {
                user.APIKey = Intwenty.Model.BaseModelItem.GetQuiteUniqueString();
                await UserManager.UpdateAsync(user);
                return await OnGetLoad(user.Id);
            }

            return new JsonResult("{}");
         
        }



    }
}
