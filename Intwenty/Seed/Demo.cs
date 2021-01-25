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
    public static class Demo
    {


        public static async Task SeedDemoUsersAndRoles(IServiceProvider services)
        {

            var Settings = services.GetRequiredService<IOptions<IntwentySettings>>();

            if (!Settings.Value.SeedDemoUserAccountsOnStartUp)
                return;

          
            var userManager = services.GetRequiredService<IntwentyUserManager>();
            var roleManager = services.GetRequiredService<RoleManager<IntwentyProductAuthorizationItem>>();
            var productManager = services.GetRequiredService<IIntwentyProductManager>();
            var organizationManager = services.GetRequiredService<IIntwentyOrganizationManager>();

            //ENSURE WE HAVE A PRODUCT AND ORG
            IntwentyProduct product = await productManager.FindByIdAsync(Settings.Value.ProductId);
            if (product == null)
            {
                product = new IntwentyProduct();
                product.Id = Settings.Value.ProductId;
                product.ProductName = Settings.Value.SiteTitle;
                await productManager.CreateAsync(product);
            }

            IntwentyOrganization org = await organizationManager.FindByNameAsync(Settings.Value.DefaultProductOrganization);
            if (org == null)
            {
                org = new IntwentyOrganization();
                org.Name = Settings.Value.DefaultProductOrganization;
                await organizationManager.CreateAsync(org);
            }

            var all_products = await organizationManager.GetProductsAsync(org.Id);
            var thisproduct = all_products.Find(p => p.ProductId == product.Id);
            if (thisproduct == null)
            {
                await organizationManager.AddProductAsync(new IntwentyOrganizationProduct() { OrganizationId = org.Id, ProductId = product.Id, ProductName = product.ProductName });
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

            if (!string.IsNullOrEmpty(Settings.Value.DemoAdminUser) && !string.IsNullOrEmpty(Settings.Value.DemoAdminPassword))
            {
                IntwentyUser admin_user = await userManager.FindByNameAsync(Settings.Value.DemoAdminUser);
                if (admin_user == null)
                {
                    admin_user = new IntwentyUser();
                    admin_user.UserName = Settings.Value.DemoAdminUser;
                    admin_user.Email = Settings.Value.DemoAdminUser;
                    admin_user.FirstName = "Admin";
                    admin_user.LastName = "Adminsson";
                    admin_user.EmailConfirmed = true;
                    admin_user.Culture = Settings.Value.DefaultCulture;
                    await userManager.CreateAsync(admin_user, Settings.Value.DemoAdminPassword);
                }

                var all_members = await organizationManager.GetMembersAsync(org.Id);
                var admin_member = all_members.Find(p => p.UserId == admin_user.Id);
                if (admin_member == null)
                {
                    await organizationManager.AddMemberAsync(new IntwentyOrganizationMember() { OrganizationId = org.Id, UserId = admin_user.Id, UserName = admin_user.UserName });
                    await userManager.AddUpdateUserRoleAuthorizationAsync("SUPERADMIN", admin_user.Id, org.Id, Settings.Value.ProductId);
                }
            }

            if (!string.IsNullOrEmpty(Settings.Value.DemoUser) && !string.IsNullOrEmpty(Settings.Value.DemoUserPassword))
            {
                IntwentyUser default_user = await userManager.FindByNameAsync(Settings.Value.DemoUser);
                if (default_user == null)
                {
                    default_user = new IntwentyUser();
                    default_user.UserName = Settings.Value.DemoUser;
                    default_user.Email = Settings.Value.DemoUser;
                    default_user.FirstName = "User";
                    default_user.LastName = "Usersson";
                    default_user.EmailConfirmed = true;
                    default_user.Culture = Settings.Value.DefaultCulture;
                    await userManager.CreateAsync(default_user, Settings.Value.DemoUserPassword);
                }

                var all_members = await organizationManager.GetMembersAsync(org.Id);
                var user_member = all_members.Find(p => p.UserId == default_user.Id);
                if (user_member == null)
                {
                    await organizationManager.AddMemberAsync(new IntwentyOrganizationMember() { OrganizationId = org.Id, UserId = default_user.Id, UserName = default_user.UserName });
                    await userManager.AddUpdateUserRoleAuthorizationAsync("SUPERADMIN", default_user.Id, org.Id, Settings.Value.ProductId);
                }
            }
            


           


        }


    }
}
