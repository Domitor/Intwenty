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
    [Authorize(Policy = "IntwentyModelAuthorizationPolicy")]
    public class OrganizationModel : PageModel
    {

        private IIntwentyDataService DataRepository { get; }
        private IIntwentyModelService ModelRepository { get; }
        private UserManager<IntwentyUser> UserManager { get; }
        private IIntwentyOrganizationManager OrganizationManager  { get; }

        public int Id { get; set; }

        public OrganizationModel(IIntwentyDataService ms, IIntwentyModelService sr, IIntwentyOrganizationManager orgmanager, UserManager<IntwentyUser> usermanager)
        {
            DataRepository = ms;
            ModelRepository = sr;
            OrganizationManager = orgmanager;
            UserManager = usermanager;
        }

        public void OnGet(int id)
        {
            Id = id;   
        }

        public async Task<JsonResult> OnGetLoad(int id)
        {
            var result = await OrganizationManager.FindByIdAsync(id);
            return new JsonResult(new IntwentyOrganizationVm(result));
        }

        public async Task<IActionResult> OnPostUpdateEntity([FromBody] IntwentyOrganizationVm model)
        {

            var org = await OrganizationManager.FindByIdAsync(model.Id);
            if (org != null)
            {
                org.Name = model.Name;
                await OrganizationManager.UpdateAsync(org);
                return await OnGetLoad(org.Id);
            }

            return new JsonResult("{}");

        }


    }
}
