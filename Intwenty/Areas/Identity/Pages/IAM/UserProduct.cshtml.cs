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
    public class UserProductModel : PageModel
    {

        private IIntwentyDataService DataRepository { get; }

        private IntwentyUserManager UserManager { get; }

        private IIntwentyProductManager ProductManager { get; }

        public string UserId { get; set; }
        public int OrganizationId { get; set; }
        public string ProductId { get; set; }


        public UserProductModel(IIntwentyDataService ms, IntwentyUserManager usermanager, IIntwentyProductManager prodmanager)
        {
            DataRepository = ms;
            UserManager = usermanager;
            ProductManager = prodmanager;
        }

        public void OnGet(string userid, int organizationid, string productid)
        {
            UserId = userid;
            OrganizationId = organizationid;
            ProductId = productid;
        }

        public async Task<JsonResult> OnGetLoad(string userid, int organizationid, string productid)
        {
            var user = await UserManager.FindByIdAsync(userid);
            var products = await UserManager.GetOrganizationProductsAsync(user);
            var result = products.Find(p => p.UserId == userid && p.OrganizationId == organizationid && p.ProductId == productid);
            var authorizations = await UserManager.GetUserAuthorizationsAsync(user, productid);

            result.RoleAuthorizations = authorizations.Where(p => p.AuthorizationItemType == "ROLE").Select(p => new IntwentyAuthorizationVm(p)).ToList();
            result.ViewAuthorizations = authorizations.Where(p => p.AuthorizationItemType == "VIEW").Select(p => new IntwentyAuthorizationVm(p)).ToList();
            result.ApplicationAuthorizations = authorizations.Where(p => p.AuthorizationItemType == "APPLICATION").Select(p => new IntwentyAuthorizationVm(p)).ToList();
            result.SystemAuthorizations = authorizations.Where(p => p.AuthorizationItemType == "SYSTEM").Select(p => new IntwentyAuthorizationVm(p)).ToList();

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