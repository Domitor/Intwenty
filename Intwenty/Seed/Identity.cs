using System;
using Intwenty.Areas.Identity.Models;
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


        public static void Seed(IServiceProvider services)
        {

            var Settings = services.GetRequiredService<IOptions<IntwentySettings>>();

            if (!Settings.Value.SeedModelOnStartUp ||
                !Settings.Value.UseDemoSettings)
                return;

            if (string.IsNullOrEmpty(Settings.Value.DemoAdminUser) ||
                string.IsNullOrEmpty(Settings.Value.DemoUser) ||
                string.IsNullOrEmpty(Settings.Value.DemoAdminPassword) ||
                string.IsNullOrEmpty(Settings.Value.DemoUserPassword))
                return;


            var userManager = services.GetRequiredService<UserManager<IntwentyUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IntwentyRole>>();

            var admrole = roleManager.FindByNameAsync("ADMINISTRATOR");
            if (admrole.Result == null)
            {
                var role = new IntwentyRole();
                role.Name = "ADMINISTRATOR";
                roleManager.CreateAsync(role);
            }

            var userrole = roleManager.FindByNameAsync("USER");
            if (userrole.Result == null)
            {
                var role = new IntwentyRole();
                role.Name = "USER";
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
                userManager.AddToRoleAsync(user, "ADMINISTRATOR");
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
