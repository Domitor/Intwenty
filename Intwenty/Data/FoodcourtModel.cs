using Intwenty.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Intwenty.Data
{
    public static class FoodcourtModel
    {
        public static void Seed(IntwentyDbContext context, bool isupdate)
        {

            if (isupdate)
            {
                context.Set<ApplicationItem>().RemoveRange(context.Set<ApplicationItem>());
                context.Set<MenuItem>().RemoveRange(context.Set<MenuItem>());
                context.Set<DatabaseItem>().RemoveRange(context.Set<DatabaseItem>());
                context.Set<DataViewItem>().RemoveRange(context.Set<DataViewItem>());
                context.Set<UserInterfaceItem>().RemoveRange(context.Set<UserInterfaceItem>());
                context.Set<ValueDomainItem>().RemoveRange(context.Set<ValueDomainItem>());
                context.SaveChanges();
            }


            //APPLICATIONS
            context.Set<ApplicationItem>().Add(new ApplicationItem() { Id = 10, Description = "An app for managing producers", MetaCode = "PRODUCER", Title = "Producer", DbName = "Producer", IsHierarchicalApplication = false, UseVersioning = false, TestDataAmount = 0 });
          

            //MENU
            context.Set<MenuItem>().Add(new MenuItem() { AppMetaCode = "", MetaType = "MAINMENU", MetaCode = "SYSMENU", ParentMetaCode = "ROOT", Title = "Menu", Order = 1, Action = "", Controller = "" });
            //context.Set<MenuItem>().Add(new MenuItem() { AppMetaCode = "PRODUCER", MetaType = "MENUITEM", MetaCode = "M_PROD", ParentMetaCode = "SYSMENU", Title = "Producer", Order = 1, Action = "", Controller = "" });


            //VALUEDOMAIN (USED IN COMBOBOXES ETC)
            //context.Set<ValueDomainItem>().Add(new ValueDomainItem() { DomainName = "ITEMCATEGORY", Code = "A1", Value = "Primary" });
            //context.Set<ValueDomainItem>().Add(new ValueDomainItem() { DomainName = "ITEMCATEGORY", Code = "A2", Value = "Secondary" });


  
            //APPLICATION PRODUCER
            //--------------------
            //DATABASE - MAINTABLE
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "PRODUCER", MetaType = "DATACOLUMN", MetaCode = "COMPANYNAME", DbName = "CompanyName", ParentMetaCode = "ROOT", DataType = "STRING" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "PRODUCER", MetaType = "DATACOLUMN", MetaCode = "COMPANYPHONE", DbName = "CompanyPhone", ParentMetaCode = "ROOT", DataType = "STRING" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "PRODUCER", MetaType = "DATACOLUMN", MetaCode = "COMPANYADDRESS", DbName = "CompanyAddress", ParentMetaCode = "ROOT", DataType = "STRING" });


            context.SaveChanges();
        }
    }
}
