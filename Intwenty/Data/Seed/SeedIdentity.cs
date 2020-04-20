using Intwenty.Data.DBAccess;
using Intwenty.Data.DBAccess.Helpers;
using Intwenty.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Data.Seed
{
    public static class SeedIdentity
    {


        public static void Seed(IServiceProvider provider)
        {

            var Settings = provider.GetRequiredService<IOptions<SystemSettings>>();

            if (!Settings.Value.IsDevelopment)
                return;

            var Connections = provider.GetRequiredService<IOptions<ConnectionStrings>>();
            IIntwentyDbORM DataRepository = null;
            if (Settings.Value.IsNoSQL)
                DataRepository = new IntwentyNoSqlDbClient((DBMS)Settings.Value.IntwentyDBMS, Connections.Value.IntwentyConnection, "IntwentyDb");
            else
                DataRepository = new IntwentySqlDbClient((DBMS)Settings.Value.IntwentyDBMS, Connections.Value.IntwentyConnection);

            /*
         builder.Entity<SystemUser>(entity => { entity.ToTable(name: "security_User"); });
         builder.Entity<SystemRole>(entity => { entity.ToTable(name: "security_Role"); });
         builder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable("security_UserRoles"); });
         builder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable("security_UserClaims"); });
         builder.Entity<IdentityUserLogin<string>>(entity => { entity.ToTable("security_UserLogins"); });
         builder.Entity<IdentityRoleClaim<string>>(entity => { entity.ToTable("security_RoleClaims"); });
         builder.Entity<IdentityUserToken<string>>(entity => { entity.ToTable("security_UserTokens"); });
         */

            DataRepository.CreateTable<SystemUser>(true);
            DataRepository.CreateTable<SystemRole>(true);
            DataRepository.CreateTable<SystemUserRole>(true);
            //DataRepository.CreateTable<IdentityUserClaim<string>>(true);
            //DataRepository.CreateTable<IdentityUserLogin<string>>(true);
            //DataRepository.CreateTable<IdentityRoleClaim<string>>(true);
            //DataRepository.CreateTable<IdentityUserToken<string>>(true);


            var userManager = provider.GetRequiredService<UserManager<SystemUser>>();
            var roleManager = provider.GetRequiredService<RoleManager<SystemRole>>();


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


            var role = new SystemRole();
            role.Name = "Administrator";
            roleManager.CreateAsync(role);

            role = new SystemRole();
            role.Name = "User";
            roleManager.CreateAsync(role);

            var user = new SystemUser();
            user.UserName = "admin@intwenty.com";
            user.Email = "admin@intwenty.com";
            user.FirstName = "Admin";
            user.LastName = "Adminsson";
            user.EmailConfirmed = true;
            userManager.CreateAsync(user, "thriller");
            userManager.AddToRoleAsync(user, "Administrator");


            user = new SystemUser();
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
