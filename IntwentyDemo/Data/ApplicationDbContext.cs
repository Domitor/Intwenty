using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using IntwentyDemo.Data.Entity;

namespace IntwentyDemo.Data
{
    public class ApplicationDbContext : IdentityDbContext<SystemUser, SystemRole, String>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
          
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //RENAME IDENTITY TABLES
            builder.Entity<SystemUser>(entity => {entity.ToTable(name: "security_User");});
            builder.Entity<SystemRole>(entity =>{entity.ToTable(name: "security_Role");});
            builder.Entity<IdentityUserRole<string>>(entity =>{entity.ToTable("security_UserRoles");});
            builder.Entity<IdentityUserClaim<string>>(entity =>{entity.ToTable("security_UserClaims");});
            builder.Entity<IdentityUserLogin<string>>(entity =>{entity.ToTable("security_UserLogins");});
            builder.Entity<IdentityRoleClaim<string>>(entity =>{entity.ToTable("security_RoleClaims");});
            builder.Entity<IdentityUserToken<string>>(entity =>{entity.ToTable("security_UserTokens");});


        }

    }
}
