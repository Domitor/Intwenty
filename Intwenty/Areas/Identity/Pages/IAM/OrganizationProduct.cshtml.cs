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

        public async Task<IActionResult> OnPostUpdateEntity([FromBody] IntwentyOrganizationProductVm model)
        {

            //FIND OBJECT IN DB, UPDATE FROM MODEL AND SAVE
            var client = DataRepository.GetIAMDataClient();
            await client.OpenAsync();
            var orgproduct = await client.GetEntityAsync<IntwentyOrganizationProduct>(model.Id);
            await client.CloseAsync();
            if (orgproduct != null)
            {
                orgproduct.ProductURI = model.ProductURI;
                orgproduct.APIPath = model.APIPath;
                await client.OpenAsync();
                await client.UpdateEntityAsync(orgproduct);
                await client.CloseAsync();

            }

            return await OnGetLoad(model.OrganizationId, model.ProductId);

        }

        public async Task<IActionResult> OnPostAddRoleAuthorization([FromBody] IntwentyAuthorizationVm model)
        {
            var allauths = await ProductManager.GetAthorizationItemsAsync(model.ProductId);
            var authitem = allauths.Find(p => p.Id == model.AuthorizationId);
            await OrganizationManager.AddUpdateRoleAuthorizationAsync(authitem.NormalizedName,  model.OrganizationId, model.ProductId);
            return await OnGetLoad( model.OrganizationId, model.ProductId);

        }

        public async Task<IActionResult> OnPostAddSystemAuthorization([FromBody] IntwentyAuthorizationVm model)
        {
            var allauths = await ProductManager.GetAthorizationItemsAsync(model.ProductId);
            var authitem = allauths.Find(p => p.Id == model.AuthorizationId);
            await OrganizationManager.AddUpdateSystemAuthorizationAsync(authitem.NormalizedName,  model.OrganizationId, model.ProductId, model.Read, model.Modify, model.Delete);
            return await OnGetLoad(model.OrganizationId, model.ProductId);

        }

        public async Task<IActionResult> OnPostAddApplicationAuthorization([FromBody] IntwentyAuthorizationVm model)
        {
            var allauths = await ProductManager.GetAthorizationItemsAsync(model.ProductId);
            var authitem = allauths.Find(p => p.Id == model.AuthorizationId);
            await OrganizationManager.AddUpdateApplicationAuthorizationAsync(authitem.NormalizedName, model.OrganizationId, model.ProductId, model.Read, model.Modify, model.Delete);
            return await OnGetLoad(model.OrganizationId, model.ProductId);

        }

        public async Task<IActionResult> OnPostAddViewAuthorization([FromBody] IntwentyAuthorizationVm model)
        {
            var allauths = await ProductManager.GetAthorizationItemsAsync(model.ProductId);
            var authitem = allauths.Find(p => p.Id == model.AuthorizationId);
            await OrganizationManager.AddUpdateViewAuthorizationAsync(authitem.NormalizedName, model.OrganizationId, model.ProductId, model.Read, model.Modify, model.Delete);
            return await OnGetLoad(model.OrganizationId, model.ProductId);

        }

        public async Task<IActionResult> OnPostRemoveAuthorization([FromBody] IntwentyAuthorizationVm model)
        {
            await OrganizationManager.RemoveAuthorizationAsync(model.OrganizationId, model.Id);
            return await OnGetLoad(model.OrganizationId, model.ProductId);

        }

    }
}