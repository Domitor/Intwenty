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

namespace Intwenty.Areas.Identity.Pages
{
    [Authorize(Policy = "IntwentyModelAuthorizationPolicy")]
    public class OrganizationProductModel : PageModel
    {

        private IIntwentyDataService DataRepository { get; }

        private IIntwentyOrganizationManager OrganizationManager { get; }

        private IIntwentyProductManager ProductManager { get; }

        public int OrganizationId { get; set; }
        public string ProductId { get; set; }


        public OrganizationProductModel(IIntwentyDataService ms, IIntwentyOrganizationManager orgmanager, IIntwentyProductManager prodmanager)
        {
            DataRepository = ms;
            OrganizationManager = orgmanager;
            ProductManager = prodmanager;
        }

        public void OnGet(int organizationid, string productid)
        {
            OrganizationId = organizationid;
            ProductId = productid;
        }

        public async Task<JsonResult> OnGetLoad(int organizationid, string productid)
        {
            var result = await OrganizationManager.GetOrganizationProductAsync(organizationid, productid);
            return new JsonResult(result);
        }

        public async Task<JsonResult> OnGetLoadAuthItems(int organizationid, string productid)
        {
            var t = await ProductManager.GetAthorizationItemsAsync(productid);
            var authitems = new
            {
                roleItems = t.Where(p => p.AuthorizationType == "ROLE")
                ,systemItems = t.Where(p => p.AuthorizationType == "SYSTEM")
                ,applicationItems = t.Where(p => p.AuthorizationType == "APPLICATION")
                ,viewItems = t.Where(p => p.AuthorizationType == "VIEW")
            };

            return new JsonResult(authitems);
        }

        public async Task<IActionResult> OnPostUpdateEntity([FromBody] IntwentyProductVm model)
        {
            await Task.Delay(1);
            //FIND OBJECT IN DB, UPDATE FROM MODEL AND SAVE
            /*
            var product = await Manager.FindByIdAsync(model.Id);
            if (product != null)
            {
                product.ProductName = model.ProductName;
                await ProductManager.UpdateAsync(product);
                return await OnGetLoad(product.Id);
            }
            */

            return new JsonResult("{}");



        }

        public async Task<IActionResult> OnPostAddRoleAuthorization([FromBody] IntwentyAuthorizationVm model)
        {
            var allauths = await ProductManager.GetAthorizationItemsAsync(model.ProductId);
            var authitem = allauths.Find(p => p.Id == model.AuthorizationItemId);
            //await OrganizationManager.AddUpdateRoleAuthorizationAsync(authitem.NormalizedName,  model.OrganizationId, model.ProductId);
            return await OnGetLoad( model.OrganizationId, model.ProductId);

        }

        public async Task<IActionResult> OnPostAddSystemAuthorization([FromBody] IntwentyAuthorizationVm model)
        {
            var allauths = await ProductManager.GetAthorizationItemsAsync(model.ProductId);
            var authitem = allauths.Find(p => p.Id == model.AuthorizationItemId);
            //await OrganizationManager.AddUpdateSystemAuthorizationAsync(authitem.NormalizedName,  model.OrganizationId, model.ProductId, model.Read, model.Modify, model.Delete);
            return await OnGetLoad(model.OrganizationId, model.ProductId);

        }

        public async Task<IActionResult> OnPostAddApplicationAuthorization([FromBody] IntwentyAuthorizationVm model)
        {
            var allauths = await ProductManager.GetAthorizationItemsAsync(model.ProductId);
            var authitem = allauths.Find(p => p.Id == model.AuthorizationItemId);
            //await OrganizationManager.AddUpdateApplicationAuthorizationAsync(authitem.NormalizedName, model.OrganizationId, model.ProductId, model.Read, model.Modify, model.Delete);
            return await OnGetLoad(model.OrganizationId, model.ProductId);

        }

        public async Task<IActionResult> OnPostAddViewAuthorization([FromBody] IntwentyAuthorizationVm model)
        {
            var allauths = await ProductManager.GetAthorizationItemsAsync(model.ProductId);
            var authitem = allauths.Find(p => p.Id == model.AuthorizationItemId);
            //await OrganizationManager.AddUpdateViewAuthorizationAsync(authitem.NormalizedName, model.OrganizationId, model.ProductId, model.Read, model.Modify, model.Delete);
            return await OnGetLoad(model.OrganizationId, model.ProductId);

        }

        public async Task<IActionResult> OnPostRemoveAuthorization([FromBody] IntwentyAuthorizationVm model)
        {
            //await OrganizationManager.RemoveUserAuthorizationAsync(model.OrganizationId, model.Id);
            return await OnGetLoad(model.OrganizationId, model.ProductId);

        }

    }
}