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
        private IntwentyUserManager UserManager { get; }
        private IIntwentyOrganizationManager OrganizationManager  { get; }
        private IIntwentyProductManager ProductManager { get; }

        public int Id { get; set; }

        public OrganizationModel(IIntwentyDataService ms, IIntwentyModelService sr, IIntwentyOrganizationManager orgmanager, IntwentyUserManager usermanager, IIntwentyProductManager productmanager)
        {
            DataRepository = ms;
            ModelRepository = sr;
            OrganizationManager = orgmanager;
            UserManager = usermanager;
            ProductManager = productmanager;
        }

        public void OnGet(int id)
        {
            Id = id;   
        }

        public async Task<JsonResult> OnGetLoad(int id)
        {
            var org = await OrganizationManager.FindByIdAsync(id);
            var members = await OrganizationManager.GetMembersAsync(id);
            var products = await OrganizationManager.GetProductsAsync(id);
            var users = await UserManager.GetUsersAsync();

            var model = new IntwentyOrganizationVm(org);

            foreach (var m in members)
            {
                var orgmembervm = new IntwentyOrganizationMemberVm(m);
                var user = users.Find(p => p.Id == m.UserId);
                if (user != null)
                {
                    orgmembervm.FirstName = user.FirstName;
                    orgmembervm.LastName = user.LastName;
                }

                model.Members.Add(orgmembervm);

            }

            foreach (var m in products)
            {
                var orgproductvm = new IntwentyOrganizationProductVm(m);
                model.Products.Add(orgproductvm);
            }

            return new JsonResult(model);
        }

        public async Task<JsonResult> OnGetLoadUsers(int id)
        {
            var t = await UserManager.GetUsersAsync();
            return new JsonResult(t);
        }

        public async Task<JsonResult> OnGetLoadProducts(int id)
        {
            var t = await ProductManager.GetAll();
            return new JsonResult(t);
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

        public async Task<IActionResult> OnPostAddMember([FromBody] IntwentyOrganizationMemberVm model)
        {
            var user = await UserManager.FindByIdAsync(model.UserId);
            if (user==null)
                return await OnGetLoad(model.OrganizationId);

            await OrganizationManager.AddMemberAsync(new IntwentyOrganizationMember() { OrganizationId = model.OrganizationId,  UserId = model.UserId, UserName = user.UserName });
            return await OnGetLoad(model.OrganizationId);

        }

        public async Task<IActionResult> OnPostRemoveMember([FromBody] IntwentyOrganizationMemberVm model)
        {
            await OrganizationManager.RemoveMemberAsync(new IntwentyOrganizationMember() { Id = model.Id, OrganizationId = model.OrganizationId, UserId = model.UserId });
            return await OnGetLoad(model.OrganizationId);

        }

        public async Task<IActionResult> OnPostAddProduct([FromBody] IntwentyOrganizationProduct model)
        {
            await OrganizationManager.AddProductAsync(model);
            return await OnGetLoad(model.OrganizationId);
        }

        public async Task<IActionResult> OnPostRemoveProduct([FromBody] IntwentyOrganizationProduct model)
        {
            await OrganizationManager.RemoveProductAsync(model);
            return await OnGetLoad(model.OrganizationId);

        }


    }
}
