using System;
using Intwenty.Areas.Identity.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text;
using Intwenty.Model;

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


            var userManager = services.GetRequiredService<UserManager<IntwentyUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IntwentyRole>>();

            var admrole = roleManager.FindByNameAsync("SUPERADMIN");
            if (admrole.Result == null)
            {
                var role = new IntwentyRole();
                role.Name = "SUPERADMIN";
                roleManager.CreateAsync(role);
            }

            admrole = roleManager.FindByNameAsync("USERADMIN");
            if (admrole.Result == null)
            {
                var role = new IntwentyRole();
                role.Name = "USERADMIN";
                roleManager.CreateAsync(role);
            }

            admrole = roleManager.FindByNameAsync("SYSTEMADMIN");
            if (admrole.Result == null)
            {
                var role = new IntwentyRole();
                role.Name = "SYSTEMADMIN";
                roleManager.CreateAsync(role);
            }

            var userrole = roleManager.FindByNameAsync("USER");
            if (userrole.Result == null)
            {
                var role = new IntwentyRole();
                role.Name = "USER";
                roleManager.CreateAsync(role);
            }

            userrole = roleManager.FindByNameAsync("APIUSER");
            if (userrole.Result == null)
            {
                var role = new IntwentyRole();
                role.Name = "APIUSER";
                roleManager.CreateAsync(role);
            }

            var currr_admin = userManager.FindByNameAsync(Settings.Value.DemoAdminUser);
            if (currr_admin.Result == null)
            {
                var user = new IntwentyUser();
                user.UserName = Settings.Value.DemoAdminUser;
                user.Email = Settings.Value.DemoAdminUser;
                user.FirstName = "Admin";
                user.LastName = "Adminsson";
                user.EmailConfirmed = true;
                user.Culture = Settings.Value.DefaultCulture;
                userManager.CreateAsync(user, Settings.Value.DemoAdminPassword);
                userManager.AddToRoleAsync(user, "SUPERADMIN");
            }

            var curr_user = userManager.FindByNameAsync(Settings.Value.DemoUser);
            if (curr_user.Result == null)
            {
                var user = new IntwentyUser();
                user.UserName = Settings.Value.DemoUser;
                user.Email = Settings.Value.DemoUser;
                user.FirstName = "User";
                user.LastName = "Usersson";
                user.EmailConfirmed = true;
                user.Culture = Settings.Value.DefaultCulture;
                userManager.CreateAsync(user, Settings.Value.DemoUserPassword);
                userManager.AddToRoleAsync(user, "USER");
            }

           


        }


    }
}
