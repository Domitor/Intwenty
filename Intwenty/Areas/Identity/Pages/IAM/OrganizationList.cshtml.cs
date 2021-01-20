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
    public class OrganizationListModel : PageModel
    {

        private IIntwentyDataService DataRepository { get; }
        private IIntwentyModelService ModelRepository { get; }
        private IIntwentyOrganizationManager OrganizationManager { get; }

        public OrganizationListModel(IIntwentyDataService ms, IIntwentyModelService sr, IIntwentyOrganizationManager orgmanager)
        {
            DataRepository = ms;
            ModelRepository = sr;
            OrganizationManager = orgmanager;
        }

        public void OnGet()
        {
           
        }

        public async Task<IActionResult> OnGetLoad()
        {
            var list = await OrganizationManager.GetAllAsync();
            return new JsonResult(list.Select(p=> new IntwentyOrganizationVm(p)).ToList());
        }


        public async Task<IActionResult> OnPostAddEntity([FromBody] IntwentyOrganization model)
        {
             await OrganizationManager.CreateAsync(model);
             return await OnGetLoad();
        }
        public async Task<IActionResult> OnPostDeleteEntity([FromBody] IntwentyOrganization model)
        {
            var user = await OrganizationManager.FindByIdAsync(model.Id);
            if (user != null)
                await OrganizationManager.DeleteAsync(model);

            return await OnGetLoad();
        }



    }
}
