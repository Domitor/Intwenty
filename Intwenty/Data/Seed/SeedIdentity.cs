using System;
using Intwenty.Data.DBAccess;
using Intwenty.Data.DBAccess.Helpers;
using Intwenty.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text;
using Intwenty.Model;

namespace Intwenty.Data.Seed
{
    public static class SeedIdentity
    {


        public static void Seed(IServiceProvider services)
        {

            var Settings = services.GetRequiredService<IOptions<IntwentySettings>>();

            if (!Settings.Value.SeedDatabaseOnStartUp ||
                !Settings.Value.UseDemoSettings ||
                !Settings.Value.ReCreateDatabaseOnStartup)
                return;

            if (string.IsNullOrEmpty(Settings.Value.DemoAdminUser) ||
                string.IsNullOrEmpty(Settings.Value.DemoUser) ||
                string.IsNullOrEmpty(Settings.Value.DemoAdminPassword) ||
                string.IsNullOrEmpty(Settings.Value.DemoUserPassword))
                return;

            var userManager = services.GetRequiredService<UserManager<IntwentyUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IntwentyRole>>();

        

            var u = userManager.FindByNameAsync(Settings.Value.DemoAdminUser);
            if (u.Result != null)
            {
                userManager.RemoveFromRoleAsync(u.Result, "ADMINISTRATOR");
                userManager.DeleteAsync(u.Result);
            }
            u = userManager.FindByNameAsync(Settings.Value.DemoUser);
            if (u.Result != null)
            {
                userManager.RemoveFromRoleAsync(u.Result, "USER");
                userManager.DeleteAsync(u.Result);
            }

            var r = roleManager.FindByNameAsync("ADMINISTRATOR");
            if (r.Result != null)
                roleManager.DeleteAsync(r.Result);

            r = roleManager.FindByNameAsync("USER");
            if (r.Result != null)
                roleManager.DeleteAsync(r.Result);


            var role = new IntwentyRole();
            role.Name = "ADMINISTRATOR";
            roleManager.CreateAsync(role);

            role = new IntwentyRole();
            role.Name = "USER";
            roleManager.CreateAsync(role);

            var user = new IntwentyUser();
            user.UserName = Settings.Value.DemoAdminUser;
            user.Email = Settings.Value.DemoAdminUser;
            user.FirstName = "Admin";
            user.LastName = "Adminsson";
            user.EmailConfirmed = true;
            user.Culture = Settings.Value.DefaultCulture;
            userManager.CreateAsync(user, Settings.Value.DemoAdminPassword);
            userManager.AddToRoleAsync(user, "ADMINISTRATOR");


            user = new IntwentyUser();
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
