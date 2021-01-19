﻿using System;
using Intwenty.Areas.Identity.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text;
using Intwenty.Model;
using Intwenty.Areas.Identity.Data;

namespace Intwenty.Seed
{
    public static class Identity
    {


        public static void SeedDemoUsersAndRoles(IServiceProvider services)
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
            product = productManager.FindByIdAsync(Settings.Value.ProductId).Result;
            if (product == null)
            {
                product = new IntwentyProduct();
                product.Id = Settings.Value.ProductId;
                product.ProductName = Settings.Value.SiteTitle;
                productManager.CreateAsync(product);
            }

            IntwentyOrganization org = null;
            org = organizationManager.FindByNameAsync(Settings.Value.DefaultProductOrganization).Result;
            if (org == null)
            {
                org = new IntwentyOrganization();
                org.Name = Settings.Value.DefaultProductOrganization;
                organizationManager.CreateAsync(org);
                org = organizationManager.FindByNameAsync(Settings.Value.DefaultProductOrganization).Result;
                organizationManager.AddProductAsync(new IntwentyOrganizationProduct() { OrganizationId = org.Id, ProductId = product.Id, ProductName = product.ProductName  });
            }

           

            var admrole = roleManager.FindByNameAsync("SUPERADMIN").Result;
            if (admrole == null)
            {
                var role = new IntwentyProductAuthorizationItem();
                role.ProductId = product.Id;
                role.Name = "SUPERADMIN";
                role.AuthorizationType = "ROLE";
                roleManager.CreateAsync(role);
            }

            admrole = roleManager.FindByNameAsync("USERADMIN").Result;
            if (admrole == null)
            {
                var role = new IntwentyProductAuthorizationItem();
                role.ProductId = product.Id;
                role.Name = "USERADMIN";
                role.AuthorizationType = "ROLE";
                roleManager.CreateAsync(role);
            }

            admrole = roleManager.FindByNameAsync("SYSTEMADMIN").Result;
            if (admrole == null)
            {
                var role = new IntwentyProductAuthorizationItem();
                role.ProductId = product.Id;
                role.Name = "SYSTEMADMIN";
                role.AuthorizationType = "ROLE";
                roleManager.CreateAsync(role);
            }

            var userrole = roleManager.FindByNameAsync("USER").Result;
            if (userrole == null)
            {
                var role = new IntwentyProductAuthorizationItem();
                role.ProductId = product.Id;
                role.Name = "USER";
                role.AuthorizationType = "ROLE";
                roleManager.CreateAsync(role);
            }

            userrole = roleManager.FindByNameAsync("APIUSER").Result;
            if (userrole == null)
            {
                var role = new IntwentyProductAuthorizationItem();
                role.ProductId = product.Id;
                role.Name = "APIUSER";
                role.AuthorizationType = "ROLE";
                roleManager.CreateAsync(role);
            }

            var currr_admin = userManager.FindByNameAsync(Settings.Value.DemoAdminUser).Result;
            if (currr_admin == null)
            {
                var user = new IntwentyUser();
                user.UserName = Settings.Value.DemoAdminUser;
                user.Email = Settings.Value.DemoAdminUser;
                user.FirstName = "Admin";
                user.LastName = "Adminsson";
                user.EmailConfirmed = true;
                user.Culture = Settings.Value.DefaultCulture;
                userManager.CreateAsync(user, Settings.Value.DemoAdminPassword);
                organizationManager.AddMemberAsync(new IntwentyOrganizationMember() { OrganizationId = org.Id, UserId = user.Id, UserName = user.UserName });
                var result = userManager.AddUserRoleAuthorizationAsync("SUPERADMIN", user.Id, org.Id, Settings.Value.ProductId);
            }

            var curr_user = userManager.FindByNameAsync(Settings.Value.DemoUser).Result;
            if (curr_user == null)
            {
                var user = new IntwentyUser();
                user.UserName = Settings.Value.DemoUser;
                user.Email = Settings.Value.DemoUser;
                user.FirstName = "User";
                user.LastName = "Usersson";
                user.EmailConfirmed = true;
                user.Culture = Settings.Value.DefaultCulture;
                userManager.CreateAsync(user, Settings.Value.DemoUserPassword);
                organizationManager.AddMemberAsync(new IntwentyOrganizationMember() { OrganizationId = org.Id, UserId = user.Id, UserName = user.UserName });
                var result = userManager.AddUserRoleAuthorizationAsync("USER", user.Id, org.Id, Settings.Value.ProductId);
            }

           


        }


    }
}
