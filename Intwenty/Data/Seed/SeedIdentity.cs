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


        public static void Seed(IServiceProvider provider)
        {

            var Settings = provider.GetRequiredService<IOptions<IntwentySettings>>();

            if (!Settings.Value.IsDevelopment)
                return;

            var userManager = provider.GetRequiredService<UserManager<IntwentyUser>>();
            var roleManager = provider.GetRequiredService<RoleManager<IntwentyRole>>();


            var u = userManager.FindByNameAsync("admin@intwenty.com");
            if (u.Result != null)
                userManager.DeleteAsync(u.Result);

            u = userManager.FindByNameAsync("user@intwenty.com");
            if (u.Result != null)
                userManager.DeleteAsync(u.Result);

            var r = roleManager.FindByNameAsync("Administrator");
            if (r.Result != null)
                roleManager.DeleteAsync(r.Result);

            r = roleManager.FindByNameAsync("User");
            if (r.Result != null)
                roleManager.DeleteAsync(r.Result);


            var role = new IntwentyRole();
            role.Name = "Administrator";
            roleManager.CreateAsync(role);

            role = new IntwentyRole();
            role.Name = "User";
            roleManager.CreateAsync(role);

            var user = new IntwentyUser();
            user.UserName = "admin@intwenty.com";
            user.Email = "admin@intwenty.com";
            user.FirstName = "Admin";
            user.LastName = "Adminsson";
            user.EmailConfirmed = true;
            userManager.CreateAsync(user, "thriller");
            userManager.AddToRoleAsync(user, "Administrator");


            user = new IntwentyUser();
            user.UserName = "user@intwenty.com";
            user.Email = "user@intwenty.com";
            user.FirstName = "User";
            user.LastName = "Usersson";
            user.EmailConfirmed = true;
            userManager.CreateAsync(user, "thriller");
            userManager.AddToRoleAsync(user, "User");

        }


    }
}
