using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Intwenty.Data;
using IntwentyDemo.Data.Entity;

namespace IntwentyDemo.Data
{
    public static class DBInitializer
    {
        public static void Initialize(IServiceProvider provider)
        {
            var intwentycontext = provider.GetRequiredService<IntwentyDbContext>();
            var sitecontext = provider.GetRequiredService<ApplicationDbContext>();


            if (sitecontext.Database.EnsureCreated())
            {
                Intwenty.Data.SalesOrderDemoModel.Seed(intwentycontext, false);
                SeedRolesAndUsers(sitecontext, provider, true).Wait();
            }
            else
            {
                //SalesOrderDemoModel.Seed(context, true);
                //SeedRolesAndUsers(context, provider, true).Wait();
            }

           
        }


        private async static Task SeedRolesAndUsers(ApplicationDbContext context, IServiceProvider provider, bool isupdate)
        {
            var userManager = provider.GetRequiredService<UserManager<SystemUser>>();
            var roleManager = provider.GetRequiredService<RoleManager<SystemRole>>();

            if (isupdate)
            {
                var u = await userManager.FindByNameAsync("admin@intwenty.com");
                if (u!=null)
                    await userManager.DeleteAsync(u);

                u = await userManager.FindByNameAsync("user@intwenty.com");
                if (u != null)
                    await userManager.DeleteAsync(u);

                var r = await roleManager.FindByNameAsync("Administrator");
                if (r != null)
                    await roleManager.DeleteAsync(r);

                r = await roleManager.FindByNameAsync("User");
                if (r != null)
                    await roleManager.DeleteAsync(r);
              

            }

            

            var role = new SystemRole();
            role.Name = "Administrator";
            await roleManager.CreateAsync(role);

            role = new SystemRole();
            role.Name = "User";
            await roleManager.CreateAsync(role);

            var user = new SystemUser();
            user.UserName = "admin@intwenty.com";
            user.Email = "admin@intwenty.com";
            user.FirstName = "Admin";
            user.LastName = "Adminsson";
            user.EmailConfirmed = true;
            await userManager.CreateAsync(user, "thriller");
            await userManager.AddToRoleAsync(user, "Administrator");


            user = new SystemUser();
            user.UserName = "user@intwenty.com";
            user.Email = "user@intwenty.com";
            user.FirstName = "User";
            user.LastName = "Usersson";
            user.EmailConfirmed = true;
            await userManager.CreateAsync(user, "thriller");
            await userManager.AddToRoleAsync(user, "User");


            await context.SaveChangesAsync();
        }


    }
 }
