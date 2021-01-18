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
    }
}