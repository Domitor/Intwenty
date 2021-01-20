using System;
using Intwenty.Areas.Identity.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text;
using Intwenty.Model;
using Intwenty.Areas.Identity.Data;
using System.Threading.Tasks;

namespace Intwenty.Seed
{
    public static class Identity
    {


        public static async Task SeedDemoUsersAndRoles(IServiceProvider services)
        {

            var Settings = services.GetRequiredService<IOptions<IntwentySettings>>();

            if (!Settings.Value.UseDemoSettings)
                return;

            if (string.IsNullOrEmpty(Settings.Value.DemoAdminUser) ||
                string.IsNullOrEmpty(Settings.Value.DemoUser) ||
                string.IsNullOrEmpty(Settings.Value.DemoAdminPassword) ||
                string.IsNullOrEmpty(Settings.Value.DemoUserPassword))
                return;


            var userManager = services.GetRequiredService<IntwentyUserManager>();
            var roleManager = services.GetRequiredService<RoleManager<IntwentyProductAuthorizationItem>>();
            var productManager = services.GetRequiredService<IIntwentyProductManager>();
            var organizationManager = services.GetRequiredService<IIntwentyOrganizationManager>();

            IntwentyProduct product = null;
            product = await productManager.FindByIdAsync(Settings.Value.ProductId);
            if (product == null)
            {
                product = new IntwentyProduct();
                product.Id = Settings.Value.ProductId;
                product.ProductName = Settings.Value.SiteTitle;
                await productManager.CreateAsync(product);
            }

            IntwentyOrganization org = null;
            org = await organizationManager.FindByNameAsync(Settings.Value.DefaultProductOrganization);
            if (org == null)
            {
                org = new IntwentyOrganization();
                org.Name = Settings.Value.DefaultProductOrganization;
                var t = await organizationManager.CreateAsync(org);
                if (t.Succeeded)
                {
                    org = await organizationManager.FindByNameAsync(Settings.Value.DefaultProductOrganization);
                    await organizationManager.AddProductAsync(new IntwentyOrganizationProduct() { OrganizationId = org.Id, ProductId = product.Id, ProductName = product.ProductName });
                }
            }

           

            var admrole = await roleManager.FindByNameAsync("SUPERADMIN");
            if (admrole == null)
            {
                var role = new IntwentyProductAuthorizationItem();
                role.ProductId = product.Id;
                role.Name = "SUPERADMIN";
                role.AuthorizationType = "ROLE";
                await roleManager.CreateAsync(role);
            }

            admrole = await roleManager.FindByNameAsync("USERADMIN");
            if (admrole == null)
            {
                var role = new IntwentyProductAuthorizationItem();
                role.ProductId = product.Id;
                role.Name = "USERADMIN";
                role.AuthorizationType = "ROLE";
                await roleManager.CreateAsync(role);
            }

            admrole = await roleManager.FindByNameAsync("SYSTEMADMIN");
            if (admrole == null)
            {
                var role = new IntwentyProductAuthorizationItem();
                role.ProductId = product.Id;
                role.Name = "SYSTEMADMIN";
                role.AuthorizationType = "ROLE";
                await roleManager.CreateAsync(role);
            }

            var userrole = await roleManager.FindByNameAsync("USER");
            if (userrole == null)
            {
                var role = new IntwentyProductAuthorizationItem();
                role.ProductId = product.Id;
                role.Name = "USER";
                role.AuthorizationType = "ROLE";
                await roleManager.CreateAsync(role);
            }

            userrole = await roleManager.FindByNameAsync("APIUSER");
            if (userrole == null)
            {
                var role = new IntwentyProductAuthorizationItem();
                role.ProductId = product.Id;
                role.Name = "APIUSER";
                role.AuthorizationType = "ROLE";
                await roleManager.CreateAsync(role);
            }

            var currr_admin = await userManager.FindByNameAsync(Settings.Value.DemoAdminUser);
            if (currr_admin == null)
            {
                var user = new IntwentyUser();
                user.UserName = Settings.Value.DemoAdminUser;
                user.Email = Settings.Value.DemoAdminUser;
                user.FirstName = "Admin";
                user.LastName = "Adminsson";
                user.EmailConfirmed = true;
                user.Culture = Settings.Value.DefaultCulture;
                await userManager.CreateAsync(user, Settings.Value.DemoAdminPassword);
                await organizationManager.AddMemberAsync(new IntwentyOrganizationMember() { OrganizationId = org.Id, UserId = user.Id, UserName = user.UserName });
                var result = await userManager.AddUpdateUserRoleAuthorizationAsync("SUPERADMIN", user.Id, org.Id, Settings.Value.ProductId);
            }

            var curr_user = await userManager.FindByNameAsync(Settings.Value.DemoUser);
            if (curr_user == null)
            {
                var user = new IntwentyUser();
                user.UserName = Settings.Value.DemoUser;
                user.Email = Settings.Value.DemoUser;
                user.FirstName = "User";
                user.LastName = "Usersson";
                user.EmailConfirmed = true;
                user.Culture = Settings.Value.DefaultCulture;
                await userManager.CreateAsync(user, Settings.Value.DemoUserPassword);
                await organizationManager.AddMemberAsync(new IntwentyOrganizationMember() { OrganizationId = org.Id, UserId = user.Id, UserName = user.UserName });
                var result = await userManager.AddUpdateUserRoleAuthorizationAsync("USER", user.Id, org.Id, Settings.Value.ProductId);
            }

           


        }


    }
}
