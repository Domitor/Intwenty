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
            context.Set<ApplicationItem>().Add(new ApplicationItem() { Id = 20, Description = "An app for managing products", MetaCode = "PRODUCT", Title = "Product", DbName = "Product", IsHierarchicalApplication = false, UseVersioning = false, TestDataAmount = 0 });


            //MENU
            context.Set<MenuItem>().Add(new MenuItem() { AppMetaCode = "", MetaType = "MAINMENU", MetaCode = "SYSMENU", ParentMetaCode = "ROOT", Title = "Menu", Order = 1, Action = "", Controller = "" });
            context.Set<MenuItem>().Add(new MenuItem() { AppMetaCode = "PRODUCER", MetaType = "MENUITEM", MetaCode = "MI_PRODUCER", ParentMetaCode = "SYSMENU", Title = "My Company", Order = 1, Action = "", Controller = "" });
            context.Set<MenuItem>().Add(new MenuItem() { AppMetaCode = "PRODUCT", MetaType = "MENUITEM", MetaCode = "MI_PRODUCT", ParentMetaCode = "SYSMENU", Title = "My Products", Order = 2, Action = "", Controller = "" });


            //VALUEDOMAIN (USED IN COMBOBOXES ETC)
            context.Set<ValueDomainItem>().Add(new ValueDomainItem() { DomainName = "MONTHS", Code = "1", Value = "January" });
            context.Set<ValueDomainItem>().Add(new ValueDomainItem() { DomainName = "MONTHS", Code = "2", Value = "February" });
            context.Set<ValueDomainItem>().Add(new ValueDomainItem() { DomainName = "MONTHS", Code = "3", Value = "March" });
            context.Set<ValueDomainItem>().Add(new ValueDomainItem() { DomainName = "MONTHS", Code = "4", Value = "April" });
            context.Set<ValueDomainItem>().Add(new ValueDomainItem() { DomainName = "MONTHS", Code = "5", Value = "May" });
            context.Set<ValueDomainItem>().Add(new ValueDomainItem() { DomainName = "MONTHS", Code = "6", Value = "June" });
            context.Set<ValueDomainItem>().Add(new ValueDomainItem() { DomainName = "MONTHS", Code = "7", Value = "July" });
            context.Set<ValueDomainItem>().Add(new ValueDomainItem() { DomainName = "MONTHS", Code = "8", Value = "August" });
            context.Set<ValueDomainItem>().Add(new ValueDomainItem() { DomainName = "MONTHS", Code = "9", Value = "September" });
            context.Set<ValueDomainItem>().Add(new ValueDomainItem() { DomainName = "MONTHS", Code = "10", Value = "October" });
            context.Set<ValueDomainItem>().Add(new ValueDomainItem() { DomainName = "MONTHS", Code = "11", Value = "November" });
            context.Set<ValueDomainItem>().Add(new ValueDomainItem() { DomainName = "MONTHS", Code = "12", Value = "December" });

            context.Set<ValueDomainItem>().Add(new ValueDomainItem() { DomainName = "YEARS", Code = "2019", Value = "2019" });
            context.Set<ValueDomainItem>().Add(new ValueDomainItem() { DomainName = "YEARS", Code = "2020", Value = "2020" });
            context.Set<ValueDomainItem>().Add(new ValueDomainItem() { DomainName = "YEARS", Code = "2021", Value = "2021" });
            context.Set<ValueDomainItem>().Add(new ValueDomainItem() { DomainName = "YEARS", Code = "2022", Value = "2022" });
            context.Set<ValueDomainItem>().Add(new ValueDomainItem() { DomainName = "YEARS", Code = "2023", Value = "2023" });





            //APPLICATION PRODUCER
            //--------------------
            //DATABASE - MAINTABLE
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "PRODUCER", MetaType = "DATACOLUMN", MetaCode = "COMPANYNAME", DbName = "CompanyName", ParentMetaCode = "ROOT", DataType = "STRING" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "PRODUCER", MetaType = "DATACOLUMN", MetaCode = "COMPANYPHONE", DbName = "CompanyPhone", ParentMetaCode = "ROOT", DataType = "STRING" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "PRODUCER", MetaType = "DATACOLUMN", MetaCode = "COMPANYADDRESS", DbName = "CompanyAddress", ParentMetaCode = "ROOT", DataType = "STRING" });




            //APPLICATION PRODUCT
            //--------------------
            //DATABASE - MAINTABLE
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "PRODUCT", MetaType = "DATACOLUMN", MetaCode = "PRODID", DbName = "ProductId", ParentMetaCode = "ROOT", DataType = "STRING" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "PRODUCT", MetaType = "DATACOLUMN", MetaCode = "PRODNAME", DbName = "Name", ParentMetaCode = "ROOT", DataType = "STRING" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "PRODUCT", MetaType = "DATACOLUMN", MetaCode = "PRODPIC", DbName = "Image", ParentMetaCode = "ROOT", DataType = "STRING" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "PRODUCT", MetaType = "DATACOLUMN", MetaCode = "PRODDESCRIPTION", DbName = "Description", ParentMetaCode = "ROOT", DataType = "STRING" });


            //UI
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "PRODUCT", MetaType = "SECTION", MetaCode = "MAINSECTION", DataMetaCode = "", Title = "Product", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "PRODUCT", MetaType = "PANEL", MetaCode = "ITMPNL_A", DataMetaCode = "", Title = "", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "PRODUCT", MetaType = "TEXTBOX", MetaCode = "TB_PRODNAME", DataMetaCode = "PRODNAME", Title = "Name", ParentMetaCode = "ITMPNL_A", RowOrder = 1, ColumnOrder = 1 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "PRODUCT", MetaType = "TEXTAREA", MetaCode = "TB_PRODDESCR", DataMetaCode = "PRODDESCRIPTION", Title = "Description", ParentMetaCode = "ITMPNL_A", RowOrder = 2, ColumnOrder = 1 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "PRODUCT", MetaType = "PANEL", MetaCode = "ITMPNL_B", DataMetaCode = "", Title = "", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 2 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "PRODUCT", MetaType = "IMAGEBOX", MetaCode = "IMGBOX", DataMetaCode = "PRODPIC", Title = "Product Picture", ParentMetaCode = "ITMPNL_B", RowOrder = 3, ColumnOrder = 1 });

            //LISTVIEW
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "PRODUCT", MetaType = "LISTVIEW", MetaCode = "MAIN_LISTVIEW", DataMetaCode = "", Title = "Products", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "PRODUCT", MetaType = "LISTVIEWFIELD", MetaCode = "LV_ID", DataMetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 1 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "PRODUCT", MetaType = "LISTVIEWFIELD", MetaCode = "LV_PRODPIC", DataMetaCode = "PRODPIC", Title = "Image", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 2, Properties="IMAGE=TRUE" });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "PRODUCT", MetaType = "LISTVIEWFIELD", MetaCode = "LV_PRODNAME", DataMetaCode = "PRODNAME", Title = "Name", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 3 });


            context.SaveChanges();
        }
    }
}
