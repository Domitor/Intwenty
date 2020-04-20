using Intwenty.Data.DBAccess;
using Intwenty.Data.DBAccess.Helpers;
using Intwenty.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared;
using System;

namespace Intwenty.Data.Seed
{
    public static class SeedFoodcourtModel
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

        
            DataRepository.CreateTable<ApplicationItem>(true);
            DataRepository.CreateTable<DatabaseItem>(true);
            DataRepository.CreateTable<DataViewItem>(true);
            DataRepository.CreateTable<EventLog>(true);
            DataRepository.CreateTable<InformationStatus>(true);
            DataRepository.CreateTable<MenuItem>(true);
            DataRepository.CreateTable<SystemID>(true);
            DataRepository.CreateTable<UserInterfaceItem>(true);
            DataRepository.CreateTable<ValueDomainItem>(true);

            if (Settings.Value.ReCreateModelOnStartUp)
            {
                DataRepository.DeleteRange(DataRepository.GetAll<ApplicationItem>());
                DataRepository.DeleteRange(DataRepository.GetAll<DatabaseItem>());
                DataRepository.DeleteRange(DataRepository.GetAll<DataViewItem>());
                DataRepository.DeleteRange(DataRepository.GetAll<MenuItem>());
                DataRepository.DeleteRange(DataRepository.GetAll<UserInterfaceItem>());
                DataRepository.DeleteRange(DataRepository.GetAll<ValueDomainItem>());
            }


            //APPLICATIONS
            DataRepository.Insert(new ApplicationItem() { Id = 10, Description = "An app for managing producers", MetaCode = "PRODUCER", Title = "Producer", DbName = "Producer", IsHierarchicalApplication = false, UseVersioning = false });
             DataRepository.Insert(new ApplicationItem() { Id = 20, Description = "An app for managing products", MetaCode = "PRODUCT", Title = "Product", DbName = "Product", IsHierarchicalApplication = false, UseVersioning = false });


            //MENU
             DataRepository.Insert(new MenuItem() { AppMetaCode = "", MetaType = "MAINMENU", MetaCode = "SYSMENU", ParentMetaCode = "ROOT", Title = "Menu", OrderNo = 1, Action = "", Controller = "" });
             DataRepository.Insert(new MenuItem() { AppMetaCode = "PRODUCER", MetaType = "MENUITEM", MetaCode = "MI_PRODUCER", ParentMetaCode = "SYSMENU", Title = "My Company", OrderNo = 1, Action = "", Controller = "" });
             DataRepository.Insert(new MenuItem() { AppMetaCode = "PRODUCT", MetaType = "MENUITEM", MetaCode = "MI_PRODUCT", ParentMetaCode = "SYSMENU", Title = "My Products", OrderNo = 2, Action = "", Controller = "" });


            //VALUEDOMAIN (USED IN COMBOBOXES ETC)
             DataRepository.Insert(new ValueDomainItem() { DomainName = "MONTHS", Code = "1", Value = "January" });
             DataRepository.Insert(new ValueDomainItem() { DomainName = "MONTHS", Code = "2", Value = "February" });
             DataRepository.Insert(new ValueDomainItem() { DomainName = "MONTHS", Code = "3", Value = "March" });
             DataRepository.Insert(new ValueDomainItem() { DomainName = "MONTHS", Code = "4", Value = "April" });
             DataRepository.Insert(new ValueDomainItem() { DomainName = "MONTHS", Code = "5", Value = "May" });
             DataRepository.Insert(new ValueDomainItem() { DomainName = "MONTHS", Code = "6", Value = "June" });
             DataRepository.Insert(new ValueDomainItem() { DomainName = "MONTHS", Code = "7", Value = "July" });
             DataRepository.Insert(new ValueDomainItem() { DomainName = "MONTHS", Code = "8", Value = "August" });
             DataRepository.Insert(new ValueDomainItem() { DomainName = "MONTHS", Code = "9", Value = "September" });
             DataRepository.Insert(new ValueDomainItem() { DomainName = "MONTHS", Code = "10", Value = "October" });
             DataRepository.Insert(new ValueDomainItem() { DomainName = "MONTHS", Code = "11", Value = "November" });
             DataRepository.Insert(new ValueDomainItem() { DomainName = "MONTHS", Code = "12", Value = "December" });

             DataRepository.Insert(new ValueDomainItem() { DomainName = "YEARS", Code = "2019", Value = "2019" });
             DataRepository.Insert(new ValueDomainItem() { DomainName = "YEARS", Code = "2020", Value = "2020" });
             DataRepository.Insert(new ValueDomainItem() { DomainName = "YEARS", Code = "2021", Value = "2021" });
             DataRepository.Insert(new ValueDomainItem() { DomainName = "YEARS", Code = "2022", Value = "2022" });
             DataRepository.Insert(new ValueDomainItem() { DomainName = "YEARS", Code = "2023", Value = "2023" });


            //APPLICATION PRODUCER
            //--------------------
            //DATABASE - MAINTABLE
             DataRepository.Insert(new DatabaseItem() { AppMetaCode = "PRODUCER", MetaType = "DATACOLUMN", MetaCode = "COMPANYNAME", DbName = "CompanyName", ParentMetaCode = "ROOT", DataType = "STRING" });
             DataRepository.Insert(new DatabaseItem() { AppMetaCode = "PRODUCER", MetaType = "DATACOLUMN", MetaCode = "COMPANYPHONE", DbName = "CompanyPhone", ParentMetaCode = "ROOT", DataType = "STRING" });
             DataRepository.Insert(new DatabaseItem() { AppMetaCode = "PRODUCER", MetaType = "DATACOLUMN", MetaCode = "COMPANYADDRESS", DbName = "CompanyAddress", ParentMetaCode = "ROOT", DataType = "STRING" });


            //APPLICATION PRODUCT
            //--------------------
            //DATABASE - MAINTABLE
             DataRepository.Insert(new DatabaseItem() { AppMetaCode = "PRODUCT", MetaType = "DATACOLUMN", MetaCode = "PRODID", DbName = "ProductId", ParentMetaCode = "ROOT", DataType = "STRING" });
             DataRepository.Insert(new DatabaseItem() { AppMetaCode = "PRODUCT", MetaType = "DATACOLUMN", MetaCode = "PRODNAME", DbName = "Name", ParentMetaCode = "ROOT", DataType = "STRING" });
             DataRepository.Insert(new DatabaseItem() { AppMetaCode = "PRODUCT", MetaType = "DATACOLUMN", MetaCode = "PRODPIC", DbName = "Image", ParentMetaCode = "ROOT", DataType = "STRING" });
             DataRepository.Insert(new DatabaseItem() { AppMetaCode = "PRODUCT", MetaType = "DATACOLUMN", MetaCode = "PRODDESCRIPTION", DbName = "Description", ParentMetaCode = "ROOT", DataType = "STRING" });


            //UI
             DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "PRODUCT", MetaType = "SECTION", MetaCode = "MAINSECTION", DataMetaCode = "", Title = "Product", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
             DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "PRODUCT", MetaType = "PANEL", MetaCode = "ITMPNL_A", DataMetaCode = "", Title = "", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1 });
             DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "PRODUCT", MetaType = "TEXTBOX", MetaCode = "TB_PRODNAME", DataMetaCode = "PRODNAME", Title = "Name", ParentMetaCode = "ITMPNL_A", RowOrder = 1, ColumnOrder = 1 });
             DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "PRODUCT", MetaType = "TEXTAREA", MetaCode = "TB_PRODDESCR", DataMetaCode = "PRODDESCRIPTION", Title = "Description", ParentMetaCode = "ITMPNL_A", RowOrder = 2, ColumnOrder = 1 });
             DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "PRODUCT", MetaType = "PANEL", MetaCode = "ITMPNL_B", DataMetaCode = "", Title = "", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 2 });
             DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "PRODUCT", MetaType = "IMAGEBOX", MetaCode = "IMGBOX", DataMetaCode = "PRODPIC", Title = "Product Picture", ParentMetaCode = "ITMPNL_B", RowOrder = 3, ColumnOrder = 1 });

            //LISTVIEW
             DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "PRODUCT", MetaType = "LISTVIEW", MetaCode = "MAIN_LISTVIEW", DataMetaCode = "", Title = "Products", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
             DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "PRODUCT", MetaType = "LISTVIEWFIELD", MetaCode = "LV_ID", DataMetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 1 });
             DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "PRODUCT", MetaType = "LISTVIEWFIELD", MetaCode = "LV_PRODPIC", DataMetaCode = "PRODPIC", Title = "Image", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 2, Properties="IMAGE=TRUE" });
             DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "PRODUCT", MetaType = "LISTVIEWFIELD", MetaCode = "LV_PRODNAME", DataMetaCode = "PRODNAME", Title = "Name", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 3 });


        }
    }
}
