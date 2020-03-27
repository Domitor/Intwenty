using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Intwenty.Data.Entity;

namespace Intwenty.Data
{
    public class IntwentyDbContext : DbContext
    {
        public IntwentyDbContext(DbContextOptions<IntwentyDbContext> options)
            : base(options)
        {
          
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

           

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
