using System;
using Microsoft.Extensions.DependencyInjection;
using Intwenty.Data.Entity;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Intwenty.Data
{
    public static class DBInitializer
    {
        public static void Initialize(IServiceProvider provider)
        {
            var context = provider.GetRequiredService<ApplicationDbContext>();


            if (context.Database.EnsureCreated())
            {
                SalesOrderDemoModel.Seed(context, false);
                SeedRolesAndUsers(context, provider, true).Wait();
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
                var u = await userManager.FindByNameAsync("admin@intwenty.se");
                if (u!=null)
                    await userManager.DeleteAsync(u);

                u = await userManager.FindByNameAsync("user@intwenty.se");
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
            user.UserName = "admin@intwenty.se";
            user.Email = "admin@intwenty.se";
            user.FirstName = "Admin";
            user.LastName = "Adminsson";
            user.EmailConfirmed = true;
            await userManager.CreateAsync(user, "Thriller#2020");
            await userManager.AddToRoleAsync(user, "Administrator");


            user = new SystemUser();
            user.UserName = "user@intwenty.se";
            user.Email = "user@intwenty.se";
            user.FirstName = "User";
            user.LastName = "Usersson";
            user.EmailConfirmed = true;
            await userManager.CreateAsync(user, "Thriller#2020");
            await userManager.AddToRoleAsync(user, "User");


            await context.SaveChangesAsync();
        }


    }
 }
