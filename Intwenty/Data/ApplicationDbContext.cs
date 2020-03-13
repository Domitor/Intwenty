using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Intwenty.Data.Entity;

namespace Intwenty.Data
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


            //SYSTEM MODEL
            new InformationStatusMap(builder.Entity<InformationStatus>());
            new SystemIDMap(builder.Entity<SystemID>());
            new NoSeriesMap(builder.Entity<NoSerie>());
            new ApplicationItemMap(builder.Entity<ApplicationItem>());
            new DatabaseItemMap(builder.Entity<DatabaseItem>());
            new UserInterfaceItemMap(builder.Entity<UserInterfaceItem>());
            new DataViewItemMap(builder.Entity<DataViewItem>());
            new MenuItemMap(builder.Entity<MenuItem>());
            new ValueDomainItemMap(builder.Entity<ValueDomainItem>());


            //RENAME SYSTEM MODEL TABLES
            builder.Entity<ApplicationItem>(entity => { entity.ToTable(name: "sysmodel_ApplicationItem"); });
            builder.Entity<DatabaseItem>(entity => { entity.ToTable(name: "sysmodel_DatabaseItem"); });
            builder.Entity<UserInterfaceItem>(entity => { entity.ToTable("sysmodel_UserInterfaceItem"); });
            builder.Entity<MenuItem>(entity => { entity.ToTable("sysmodel_MenuItem"); });
            builder.Entity<DataViewItem>(entity => { entity.ToTable("sysmodel_DataViewItem"); });
            builder.Entity<ValueDomainItem>(entity => { entity.ToTable("sysmodel_ValueDomainItem"); });
            builder.Entity<InformationStatus>(entity => { entity.ToTable("sysdata_InformationStatus"); });
            builder.Entity<SystemID>(entity => { entity.ToTable("sysdata_SystemID"); });
            builder.Entity<NoSerie>(entity => { entity.ToTable("sysdata_NoSeries"); });
          



        }

    }
}
