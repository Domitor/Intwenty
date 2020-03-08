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
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, String>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
          
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //RENAME IDENTITY TABLES
            builder.Entity<ApplicationUser>(entity => {entity.ToTable(name: "security_User");});
            builder.Entity<ApplicationRole>(entity =>{entity.ToTable(name: "security_Role");});
            builder.Entity<IdentityUserRole<string>>(entity =>{entity.ToTable("security_UserRoles");});
            builder.Entity<IdentityUserClaim<string>>(entity =>{entity.ToTable("security_UserClaims");});
            builder.Entity<IdentityUserLogin<string>>(entity =>{entity.ToTable("security_UserLogins");});
            builder.Entity<IdentityRoleClaim<string>>(entity =>{entity.ToTable("security_RoleClaims");});
            builder.Entity<IdentityUserToken<string>>(entity =>{entity.ToTable("security_UserTokens");});


            //SYSTEM MODEL
            new InformationStatusMap(builder.Entity<InformationStatus>());
            new SystemIDMap(builder.Entity<SystemID>());
            new NoSeriesMap(builder.Entity<NoSerie>());
            new ApplicationDescriptionMap(builder.Entity<ApplicationDescription>());
            new MetaDataItemMap(builder.Entity<MetaDataItem>());
            new MetaUIItemMap(builder.Entity<MetaUIItem>());
            new MetaDataViewMap(builder.Entity<MetaDataView>());
            new MetaMenuItemMap(builder.Entity<MetaMenuItem>());
            new ValueDomainMap(builder.Entity<ValueDomain>());


            //RENAME SYSTEM MODEL TABLES
            builder.Entity<ApplicationDescription>(entity => { entity.ToTable(name: "sysmodel_ApplicationDescription"); });
            builder.Entity<MetaDataItem>(entity => { entity.ToTable(name: "sysmodel_MetaDataItem"); });
            builder.Entity<MetaUIItem>(entity => { entity.ToTable("sysmodel_MetaUIItem"); });
            builder.Entity<MetaMenuItem>(entity => { entity.ToTable("sysmodel_MetaMenuItemm"); });
            builder.Entity<MetaDataView>(entity => { entity.ToTable("sysmodel_MetaDataView"); });
            builder.Entity<ValueDomain>(entity => { entity.ToTable("sysmodel_ValueDomain"); });
            builder.Entity<InformationStatus>(entity => { entity.ToTable("sysdata_InformationStatus"); });
            builder.Entity<SystemID>(entity => { entity.ToTable("sysdata_SystemID"); });
            builder.Entity<NoSerie>(entity => { entity.ToTable("sysdata_NoSeries"); });
          



        }

    }
}
