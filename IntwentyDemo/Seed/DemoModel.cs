using System;
using System.Linq;
using Intwenty.Entity;
using Intwenty.Areas.Identity.Models;
using Intwenty.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Intwenty.DataClient;
using Intwenty.Interface;

namespace IntwentyDemo.Seed
{
    public static class DemoModel
    {
     

        public static void SeedModel(IServiceProvider services)
        {
            var Settings = services.GetRequiredService<IOptions<IntwentySettings>>();

            if (!Settings.Value.SeedModelOnStartUp)
                return;

            var systems = new List<SystemItem>();
            var applications = new List<ApplicationItem>();
            var valuedomains = new List<ValueDomainItem>();
            var views = new List<ViewItem>();
            var userinterfacestructure = new List<UserInterfaceStructureItem>();
            var userinterface = new List<UserInterfaceItem>();
            var functions = new List<FunctionItem>();
            var dbitems = new List<DatabaseItem>();
            var dataviews = new List<DataViewItem>();
            var endpoints = new List<EndpointItem>();

            //SYSTEMS
            systems.Add(new SystemItem() { MetaCode = "INTWENTYDEFAULTSYS", Title="Default", DbPrefix = "def" });
            systems.Add(new SystemItem() { MetaCode = "WAREHOUSE", Title = "WMS", DbPrefix ="wms" });
            systems.Add(new SystemItem() { MetaCode = "BLOG", Title = "The blog engine", DbPrefix = "blog" });

            //APPLICATIONS
            applications.Add(new ApplicationItem() { Id = 10, Description = "An app for managing customers", SystemMetaCode="WAREHOUSE", MetaCode = "CUSTOMER", Title = "Customer", TitleLocalizationKey="CUSTOMER", DbName = "wms_Customer", DataMode = 1, UseVersioning = false, TenantIsolationLevel = 0, TenantIsolationMethod = 0 });
            applications.Add(new ApplicationItem() { Id = 20, Description = "An app for managing items", SystemMetaCode = "WAREHOUSE", MetaCode = "ITEM", Title = "Item", TitleLocalizationKey = "ITEM", DbName = "wms_Item", DataMode = 0, UseVersioning = false, TenantIsolationLevel = 1, TenantIsolationMethod = 2 });
            applications.Add(new ApplicationItem() { Id = 30, Description = "An app for managing sales orders", SystemMetaCode = "WAREHOUSE", MetaCode = "SALESORDER", Title = "Sales Order", TitleLocalizationKey = "SALESORDER", DbName = "wms_SalesHeader", DataMode = 1, UseVersioning = false, TenantIsolationLevel = 0, TenantIsolationMethod = 0 });
            applications.Add(new ApplicationItem() { Id = 40, Description = "An app for managing vendors", SystemMetaCode = "WAREHOUSE", MetaCode = "VENDOR", Title = "Vendor", TitleLocalizationKey = "VENDOR", DbName = "wms_Vendor", DataMode = 0, UseVersioning = false, TenantIsolationLevel = 2, TenantIsolationMethod = 2 });
            applications.Add(new ApplicationItem() { Id = 50, Description = "An app for blogging", SystemMetaCode = "BLOG", MetaCode = "BLOGAPP", Title = "The blog", TitleLocalizationKey = "", DbName = "blog_Blog", DataMode = 1, UseVersioning = false, TenantIsolationLevel = 0, TenantIsolationMethod = 0 });



            //VALUEDOMAIN (USED IN COMBOBOXES ETC)
            valuedomains.Add(new ValueDomainItem() { DomainName = "ITEMCATEGORY", Code = "PROD", Value = "Products" });
            valuedomains.Add(new ValueDomainItem() { DomainName = "ITEMCATEGORY", Code = "SERV", Value = "Services" });
            valuedomains.Add(new ValueDomainItem() { DomainName = "CUSTOMERTAGS", Code = "1", Value = "Tag 1" });
            valuedomains.Add(new ValueDomainItem() { DomainName = "CUSTOMERTAGS", Code = "2", Value = "Tag 2" });
            valuedomains.Add(new ValueDomainItem() { DomainName = "CUSTOMERTAGS", Code = "3", Value = "Tag 3" });


            #region Customer

            //APPLICATION CUSTOMER
            //--------------------
            //DATABASE - MAINTABLE
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMERID", DbName = "CustomerId", ParentMetaCode = "ROOT", DataType = "STRING", Properties= "DEFVALUE=AUTO#DEFVALUE_START=1000#DEFVALUE_PREFIX=CUST#DEFVALUE_SEED=100#UNIQUE=TRUE#MANDATORY=TRUE" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMERNAME", DbName = "CustomerName", ParentMetaCode = "ROOT", DataType = "STRING" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMERPHONE", DbName = "CustomerPhone", ParentMetaCode = "ROOT", DataType = "STRING" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMEREMAIL", DbName = "CustomerEmail", ParentMetaCode = "ROOT", DataType = "STRING" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMERSTATUS", DbName = "CustomerStatus", ParentMetaCode = "ROOT", DataType = "STRING" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMERTAGS", DbName = "CustomerTags", ParentMetaCode = "ROOT", DataType = "STRING" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMERTAGSTEXT", DbName = "CustomerTagsText", ParentMetaCode = "ROOT", DataType = "TEXT" });

            //APPLICATION - VIEWS
            views.Add(new ViewItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER",  MetaCode = "LV_CUSTOMER1", MetaType = "UIVIEW", Title = "Customer List", TitleLocalizationKey = "CUSTOMERLIST", Path = "Customers/List", IsPrimary = true, IsPublic=false });
            views.Add(new ViewItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER",  MetaCode = "INPUT_CUSTOMER1", MetaType = "UIVIEW", Title = "Create Customer", TitleLocalizationKey = "VIEW_CUST_ADDEDITVIEW", Path = "Customers/Create", IsPrimary = false, IsPublic = false });
            views.Add(new ViewItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER",  MetaCode = "INPUT_CUSTOMER2", MetaType = "UIVIEW", Title = "Edit Customer", TitleLocalizationKey = "", Path = "Customers/Edit/{id}", IsPrimary= false, IsPublic = false });

            //UI
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", ViewMetaCode = "LV_CUSTOMER1", MetaCode = "CUST_LISTVIEW", MetaType = "LISTINTERFACE", DataTableMetaCode = "CUSTOMER" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", ViewMetaCode = "INPUT_CUSTOMER1", MetaCode = "CUST_INPUTVIEW", MetaType = "INPUTINTERFACE", DataTableMetaCode = "CUSTOMER" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", ViewMetaCode = "INPUT_CUSTOMER2", MetaCode = "CUST_INPUTVIEW", MetaType = "INPUTINTERFACE", DataTableMetaCode = "CUSTOMER" });

            //INPUT UI
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", UserInterfaceMetaCode= "CUST_INPUTVIEW", MetaType = "SECTION", MetaCode = "MAINSECTION", Title = "", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", UserInterfaceMetaCode = "CUST_INPUTVIEW", MetaType = "PANEL", MetaCode = "CUSTPNL1", Title = "Basics", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", UserInterfaceMetaCode = "CUST_INPUTVIEW", MetaType = "TEXTBOX", MetaCode = "TB_CUSTID", DataColumn1MetaCode = "CUSTOMERID", Title = "Customer ID", TitleLocalizationKey = "CUSTOMERID", ParentMetaCode = "CUSTPNL1", RowOrder = 1, ColumnOrder = 1, Properties="READONLY=TRUE" });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", UserInterfaceMetaCode = "CUST_INPUTVIEW", MetaType = "TEXTBOX", MetaCode = "TB_CUSTNAME", DataColumn1MetaCode = "CUSTOMERNAME", Title = "Customer Name", TitleLocalizationKey = "CUSTOMERNAME", ParentMetaCode = "CUSTPNL1", RowOrder = 2, ColumnOrder = 1 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", UserInterfaceMetaCode = "CUST_INPUTVIEW", MetaType = "MULTISELECT", MetaCode = "MS_TAGS", DataColumn1MetaCode = "CUSTOMERTAGS", DataColumn2MetaCode = "CUSTOMERTAGSTEXT", Domain = "VALUEDOMAIN.CUSTOMERTAGS", Title = "Tags", TitleLocalizationKey = "MS_TAGS", ParentMetaCode = "CUSTPNL1", RowOrder = 3, ColumnOrder = 1 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", UserInterfaceMetaCode = "CUST_INPUTVIEW", MetaType = "PANEL", MetaCode = "CUSTPNL2",  Title = "Contact", TitleLocalizationKey = "CUSTOMERCONTACT", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 2 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", UserInterfaceMetaCode = "CUST_INPUTVIEW", MetaType = "EMAILBOX", MetaCode = "TBCUSTMAIL", DataColumn1MetaCode = "CUSTOMEREMAIL", Title = "Email", TitleLocalizationKey = "CUSTOMEREMAIL", ParentMetaCode = "CUSTPNL2", RowOrder = 1, ColumnOrder = 2 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", UserInterfaceMetaCode = "CUST_INPUTVIEW", MetaType = "NUMBOX", MetaCode = "TBCUSTPHONE", DataColumn1MetaCode = "CUSTOMERPHONE", Title = "Phone", TitleLocalizationKey = "CUSTOMERPHONE", ParentMetaCode = "CUSTPNL2", RowOrder = 2, ColumnOrder = 2 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", UserInterfaceMetaCode = "CUST_INPUTVIEW", MetaType = "TEXTBOX", MetaCode = "TBCUSTSTAT", DataColumn1MetaCode = "CUSTOMERSTATUS", Title = "Status", TitleLocalizationKey = "", ParentMetaCode = "CUSTPNL2", RowOrder = 3, ColumnOrder = 2, Properties="READONLY=TRUE" });

            //LIST UI
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", UserInterfaceMetaCode = "CUST_LISTVIEW", MetaType = "TABLE", MetaCode = "CUST_LISTVIEW", Title = "Customer List", TitleLocalizationKey = "CUSTOMERLIST", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", UserInterfaceMetaCode = "CUST_LISTVIEW", MetaType = "TABLETEXTCOLUMN", MetaCode = "LV_ID", DataColumn1MetaCode = "ID", Title = "ID", ParentMetaCode = "CUST_LISTVIEW", RowOrder = 1, ColumnOrder = 1 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", UserInterfaceMetaCode = "CUST_LISTVIEW", MetaType = "TABLETEXTCOLUMN", MetaCode = "LV_CUSTID", DataColumn1MetaCode = "CUSTOMERID", Title = "Customer ID", TitleLocalizationKey = "CUSTOMERID", ParentMetaCode = "CUST_LISTVIEW", RowOrder = 1, ColumnOrder = 2 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", UserInterfaceMetaCode = "CUST_LISTVIEW", MetaType = "TABLETEXTCOLUMN", MetaCode = "LV_CUSTNAME", DataColumn1MetaCode = "CUSTOMERNAME", Title = "Customer Name", TitleLocalizationKey = "CUSTOMERNAME", ParentMetaCode = "CUST_LISTVIEW", RowOrder = 1, ColumnOrder = 3 });

          

            //UI FUNCTIONS
            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", ViewMetaCode = "LV_CUSTOMER1", MetaType = "CREATE", DataTableMetaCode= "CUSTOMER",  MetaCode = "CUST_FUNC_CREATE", Path = "Customers/Create", RequiredAuthorization = 0, Title = "Create" });
            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", ViewMetaCode = "LV_CUSTOMER1", MetaType = "EDIT",   DataTableMetaCode = "CUSTOMER", MetaCode = "CUST_FUNC_EDIT", Path = "Customers/Edit", RequiredAuthorization = 0, Title = "Edit" });
            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", ViewMetaCode = "LV_CUSTOMER1", MetaType = "DELETE", DataTableMetaCode = "CUSTOMER", MetaCode = "CUST_FUNC_DELETE", Path = "Customers/API/Delete", RequiredAuthorization = 0, Title = "Delete" });
            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", ViewMetaCode = "LV_CUSTOMER1", MetaType = "FILTER", DataTableMetaCode = "CUSTOMER", MetaCode = "CUST_FUNC_FILTER", Path = "", RequiredAuthorization = 0, Title = "Filter" });
            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", ViewMetaCode = "LV_CUSTOMER1", MetaType = "PAGING", DataTableMetaCode = "CUSTOMER", MetaCode = "CUST_FUNC_PAGING", Path = "", RequiredAuthorization = 0, Title = "Paging" });

            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", ViewMetaCode = "INPUT_CUSTOMER1", MetaType = "SAVE", DataTableMetaCode = "CUSTOMER", MetaCode = "CUST_FUNC_SAVE", Path = "Customers/API/Save", RequiredAuthorization = 0, Title = "Save", Properties= "AFTERSAVEACTION=GOTOLISTVIEW#GOTOLISTVIEWPATH=Customers/List" });
            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", ViewMetaCode = "INPUT_CUSTOMER1", MetaType = "NAVIGATE", DataTableMetaCode = "CUSTOMER", MetaCode = "CUST_FUNC_BACK", Path = "Customers/List", RequiredAuthorization = 0, Title = "Back to list" });

            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", ViewMetaCode = "INPUT_CUSTOMER2", MetaType = "SAVE", MetaCode = "CUST_FUNC_SAVE1", DataTableMetaCode = "CUSTOMER", Path = "Customers/API/Save", RequiredAuthorization = 0, Title = "Save", Properties = "AFTERSAVEACTION=REFRESH" });
            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", ViewMetaCode = "INPUT_CUSTOMER2", MetaType = "NAVIGATE", MetaCode = "CUST_FUNC_BACK1", DataTableMetaCode = "CUSTOMER", Path = "Customers/List", RequiredAuthorization = 0, Title = "Back to list" });


            #endregion
        
            #region item

            //APPLICATION ITEM
            //---------------------
            //DATABASE - MAINTABLE
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "ITEMID", DbName = "ItemId", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "DEFVALUE=AUTO#DEFVALUE_START=1000#DEFVALUE_PREFIX=ITEM#DEFVALUE_SEED=100#UNIQUE=TRUE#MANDATORY=TRUE" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "NAME", DbName = "ItemName", ParentMetaCode = "ROOT", DataType = "STRING" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "CATCODE", DbName = "ItemCategoryCode", ParentMetaCode = "ROOT", DataType = "STRING" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "CATVALUE", DbName = "ItemCategoryValue", ParentMetaCode = "ROOT", DataType = "STRING" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "MODIFIED", DbName = "Modified", ParentMetaCode = "ROOT", DataType = "DATETIME" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "ACTIVE", DbName = "Active", ParentMetaCode = "ROOT", DataType = "BOOLEAN" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "PURCHASEPRICE", DbName = "PurchasePrice", ParentMetaCode = "ROOT", DataType = "2DECIMAL" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "SALESPRICE", DbName = "SalesPrice", ParentMetaCode = "ROOT", DataType = "2DECIMAL" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "VENDORCODE", DbName = "VendorCode", ParentMetaCode = "ROOT", DataType = "STRING" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "VENDORTXT", DbName = "VendorName", ParentMetaCode = "ROOT", DataType = "STRING" });

            //ITEM - VIEWS
            views.Add(new ViewItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaCode = "ITEM_LISTVIEW", MetaType = "UIVIEW", Title = "Item List", TitleLocalizationKey = "ITEMLIST", Path = "Items/List", IsPrimary = true, IsPublic = false });
            views.Add(new ViewItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaCode = "ITEM_CREATE_VIEW", MetaType = "UIVIEW", Title = "Create Itemn", TitleLocalizationKey = "VIEW_CUST_ADDEDITVIEW", Path = "Items/Create", IsPrimary = false, IsPublic = false });
            views.Add(new ViewItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaCode = "ITEM_EDIT_VIEW", MetaType = "UIVIEW", Title = "Edit Edit", TitleLocalizationKey = "", Path = "Items/Edit/{id}", IsPrimary = false, IsPublic = false });

            //UI
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", ViewMetaCode = "ITEM_LISTVIEW", MetaCode = "ITEMLISTUI", MetaType = "LISTINTERFACE", DataTableMetaCode = "ITEM" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", ViewMetaCode = "ITEM_CREATE_VIEW", MetaCode = "ITEMINPUTUI", MetaType = "INPUTINTERFACE", DataTableMetaCode = "ITEM" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", ViewMetaCode = "ITEM_EDIT_VIEW", MetaCode = "ITEMINPUTUI", MetaType = "INPUTINTERFACE", DataTableMetaCode = "ITEM" });


            //UI INPUT STRUCTURE
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMINPUTUI", MetaType = "SECTION", MetaCode = "MAINSECTION", Title = "Basics", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMINPUTUI", MetaType = "PANEL", MetaCode = "ITMPNL_A", Title = "Basics", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMINPUTUI", MetaType = "TEXTBOX", MetaCode = "TB_ITID", DataColumn1MetaCode = "ITEMID", Title = "Item ID", ParentMetaCode = "ITMPNL_A", RowOrder = 1, ColumnOrder = 1, Properties = "READONLY=TRUE" });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMINPUTUI", MetaType = "TEXTBOX", MetaCode = "TB_ITNAME", DataColumn1MetaCode = "NAME", Title = "Item Name", ParentMetaCode = "ITMPNL_A", RowOrder = 2, ColumnOrder = 1 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMINPUTUI", MetaType = "DATEPICKER", MetaCode = "DP_MOD", DataColumn1MetaCode = "MODIFIED", Title = "Modified Date", ParentMetaCode = "ITMPNL_A", RowOrder = 3, ColumnOrder = 1 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMINPUTUI", MetaType = "CHECKBOX", MetaCode = "CB_ACTIVE", DataColumn1MetaCode = "ACTIVE", Title = "Is Active", ParentMetaCode = "ITMPNL_A", RowOrder = 4, ColumnOrder = 1 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMINPUTUI", MetaType = "PANEL", MetaCode = "ITMPNL_B", Title = "Basics", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 2 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMINPUTUI", MetaType = "COMBOBOX", MetaCode = "CB_CATEGORY", DataColumn1MetaCode = "CATCODE", Title = "Category", ParentMetaCode = "ITMPNL_B", RowOrder = 1, ColumnOrder = 2, Domain = "VALUEDOMAIN.ITEMCATEGORY" });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMINPUTUI", MetaType = "NUMBOX", MetaCode = "NUMBOX_PURCHPRICE", DataColumn1MetaCode = "PURCHASEPRICE", Title = "Purchase Price", ParentMetaCode = "ITMPNL_B", RowOrder = 2, ColumnOrder = 2 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMINPUTUI", MetaType = "NUMBOX", MetaCode = "NUMBOX_SALESPRICE", DataColumn1MetaCode = "SALESPRICE", Title = "Sales Pricee", ParentMetaCode = "ITMPNL_B", RowOrder = 3, ColumnOrder = 2 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMINPUTUI", MetaType = "LOOKUP", MetaCode = "LOOKUPVEND", DataColumn1MetaCode = "VENDORCODE", DataColumn2MetaCode = "VENDORTXT", DataViewMetaCode = "VENDORVIEW", DataViewColumn1MetaCode = "VF_VENDID", DataViewColumn2MetaCode = "VF_VENDNAME", Title = "Vendor", ParentMetaCode = "ITMPNL_B", RowOrder = 4, ColumnOrder = 2 });


            //UI LIST STRUCTURE
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMINPUTUI", MetaType = "TABLE", MetaCode = "MAIN_EDITLISTVIEW", Title = "Item List", TitleLocalizationKey = "ITEMLIST", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMINPUTUI", MetaType = "TABLETEXTCOLUMN", MetaCode = "LV_ID", DataColumn1MetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 1 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMINPUTUI", MetaType = "TABLETEXTCOLUMN", MetaCode = "LV_ITEMID", DataColumn1MetaCode = "ITEMID", Title = "Item ID", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 2 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMINPUTUI", MetaType = "TABLETEXTCOLUMN", MetaCode = "LV_ITEMNAME", DataColumn1MetaCode = "NAME", Title = "Item Name", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 3 });

            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", ViewMetaCode = "ITEM_LISTVIEW", MetaType = "CREATE", DataTableMetaCode = "ITEM", MetaCode = "ITEM_FUNC_CREATE", Path = "Items/Create", RequiredAuthorization = 0, Title = "Create" });
            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", ViewMetaCode = "ITEM_LISTVIEW", MetaType = "EDIT", DataTableMetaCode = "ITEM", MetaCode = "ITEM_FUNC_EDIT", Path = "Items/Edit", RequiredAuthorization = 0, Title = "Edit" });
            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", ViewMetaCode = "ITEM_LISTVIEW", MetaType = "DELETE", DataTableMetaCode = "ITEM", MetaCode = "ITEM_FUNC_DELETE", Path = "Items/API/Delete", RequiredAuthorization = 0, Title = "Delete" });
            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", ViewMetaCode = "ITEM_LISTVIEW", MetaType = "FILTER", DataTableMetaCode = "ITEM", MetaCode = "ITEM_FUNC_FILTER", Path = "", RequiredAuthorization = 0, Title = "Filter" });
            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", ViewMetaCode = "ITEM_LISTVIEW", MetaType = "PAGING", DataTableMetaCode = "ITEM", MetaCode = "ITEM_FUNC_PAGING", Path = "", RequiredAuthorization = 0, Title = "Paging" });

            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", ViewMetaCode = "ITEM_CREATE_VIEW", MetaType = "SAVE", DataTableMetaCode = "ITEM", MetaCode = "ITEM_FUNC_SAVE", Path = "Items/API/Save", RequiredAuthorization = 0, Title = "Save", Properties = "AFTERSAVEACTION=GOTOLISTVIEW#GOTOLISTVIEWPATH=Items/List" });
            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", ViewMetaCode = "ITEM_CREATE_VIEW", MetaType = "NAVIGATE", DataTableMetaCode = "ITEM", MetaCode = "ITEM_FUNC_BACK", Path = "Items/List", RequiredAuthorization = 0, Title = "Back to list" });

            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", ViewMetaCode = "ITEM_EDIT_VIEW", MetaType = "SAVE", MetaCode = "ITEM_FUNC_SAVE1", DataTableMetaCode = "ITEM", Path = "Items/API/Save", RequiredAuthorization = 0, Title = "Save", Properties = "AFTERSAVEACTION=REFRESH" });
            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", ViewMetaCode = "ITEM_EDIT_VIEW", MetaType = "NAVIGATE", MetaCode = "ITEM_FUNC_BACK1", DataTableMetaCode = "ITEM", Path = "Items/List", RequiredAuthorization = 0, Title = "Back to list" });


            #endregion


            #region sales order
     
            //APPLICATION SALESORDER
            //--------------------
            //DATABASE - MAINTABLE
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "ORDERNO", DbName = "OrderNo", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "DEFVALUE=AUTO#DEFVALUE_START=1000#DEFVALUE_PREFIX=SO#DEFVALUE_SEED=100#UNIQUE=TRUE#MANDATORY=TRUE" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "ORDERDATE", DbName = "OrderDate", ParentMetaCode = "ROOT", DataType = "DATETIME" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "CUSTID", DbName = "CustomerId", ParentMetaCode = "ROOT", DataType = "STRING" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "CUSTNAME", DbName = "CustomerName", ParentMetaCode = "ROOT", DataType = "STRING" });
            
            //DATABASE - SUBTABLE (SALESLINES)
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "DATATABLE", MetaCode = "DTORDLINE", DbName = "wms_SalesLine", ParentMetaCode = "ROOT", DataType = "" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "DTORDLINE_ITEMID", DbName = "ItemNo", ParentMetaCode = "DTORDLINE", DataType = "STRING" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "DTORDLINE_ITEMNAME", DbName = "ItemName", ParentMetaCode = "DTORDLINE", DataType = "STRING" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "DTORDLINE_QTY", DbName = "Qty", ParentMetaCode = "DTORDLINE", DataType = "INTEGER" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "DTORDLINE_ORDERNO", DbName = "OrderNo", ParentMetaCode = "DTORDLINE", DataType = "STRING" });


            //ITEM - VIEWS
            views.Add(new ViewItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaCode = "SO_LISTVIEW", MetaType = "UIVIEW", Title = "Item List", TitleLocalizationKey = "SALESORDERLIST", Path = "SalesOrders/List", IsPrimary = true, IsPublic = false });
            views.Add(new ViewItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaCode = "SO_CREATE_VIEW", MetaType = "UIVIEW", Title = "Create Sales Order", TitleLocalizationKey = "", Path = "SalesOrders/Create", IsPrimary = false, IsPublic = false });
            views.Add(new ViewItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaCode = "SO_EDIT_VIEW", MetaType = "UIVIEW", Title = "Edit Sales Order", TitleLocalizationKey = "", Path = "SalesOrders/Edit/{id}", IsPrimary = false, IsPublic = false });

            //UI
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", ViewMetaCode = "SO_LISTVIEW", MetaCode = "SO_LISTUI", MetaType = "LISTINTERFACE", DataTableMetaCode = "SALESORDER" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", ViewMetaCode = "SO_CREATE_VIEW", MetaCode = "SO_INPUTUI", MetaType = "INPUTINTERFACE", DataTableMetaCode = "SALESORDER" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", ViewMetaCode = "SO_EDIT_VIEW", MetaCode = "SO_INPUTUI", MetaType = "INPUTINTERFACE", DataTableMetaCode = "SALESORDER" });



            //INPUT UI
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", UserInterfaceMetaCode= "SO_INPUTUI", MetaType = "SECTION", MetaCode = "SECT_HDR", Title = "Sales Header", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", UserInterfaceMetaCode = "SO_INPUTUI", MetaType = "PANEL", MetaCode = "PANEL1", Title = "", ParentMetaCode = "SECT_HDR", RowOrder = 1, ColumnOrder = 1 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", UserInterfaceMetaCode = "SO_INPUTUI", MetaType = "TEXTBOX", MetaCode = "TB_ORDERNO", DataColumn1MetaCode = "ORDERNO", Title = "Order No", ParentMetaCode = "PANEL1", RowOrder = 1, ColumnOrder = 1, Properties = "READONLY=TRUE" });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", UserInterfaceMetaCode = "SO_INPUTUI", MetaType = "DATEPICKER", MetaCode = "DP_ORDERDATE", DataColumn1MetaCode = "ORDERDATE", Title = "Order Date", ParentMetaCode = "PANEL1", RowOrder = 2, ColumnOrder = 1 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", UserInterfaceMetaCode = "SO_INPUTUI", MetaType = "LOOKUP", MetaCode = "LOOKUP_CUSTOMER", DataColumn1MetaCode = "CUSTID", DataColumn2MetaCode = "CUSTNAME", DataViewMetaCode = "CUSTOMERVIEW", DataViewColumn1MetaCode = "VCUSTID", DataViewColumn2MetaCode = "VCUSTNAME", Title = "Customer", ParentMetaCode = "PANEL1", RowOrder = 3, ColumnOrder = 1 });


            //TABLE UI
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", UserInterfaceMetaCode = "SO_LISTUI", MetaType = "TABLE", MetaCode = "MAIN_EDITLISTVIEW", Title = "Sales Orders", TitleLocalizationKey = "SALESORDERLIST", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0, Properties = "" });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", UserInterfaceMetaCode = "SO_LISTUI", MetaType = "TABLETEXTCOLUMN", MetaCode = "LV_ID", DataColumn1MetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 1 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", UserInterfaceMetaCode = "SO_LISTUI", MetaType = "TABLETEXTCOLUMN", MetaCode = "LF_ORDERID", DataColumn1MetaCode = "ORDERNO", Title = "Order No", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 2 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", UserInterfaceMetaCode = "SO_LISTUI", MetaType = "TABLETEXTCOLUMN", MetaCode = "LF_CUSTNAME", DataColumn1MetaCode = "CUSTNAME", Title = "Customer", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 3 });

            //FUNCTIONS
            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", ViewMetaCode = "SO_LISTVIEW", MetaType = "CREATE", DataTableMetaCode = "SALESORDER", MetaCode = "SO_FUNC_CREATE", Path = "SalesOrders/Create", RequiredAuthorization = 0, Title = "Create" });
            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", ViewMetaCode = "SO_LISTVIEW", MetaType = "EDIT", DataTableMetaCode = "SALESORDER", MetaCode = "SO_FUNC_EDIT", Path = "SalesOrders/Edit", RequiredAuthorization = 0, Title = "Edit" });
            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", ViewMetaCode = "SO_LISTVIEW", MetaType = "DELETE", DataTableMetaCode = "SALESORDER", MetaCode = "SO_FUNC_DELETE", Path = "SalesOrders/API/Delete", RequiredAuthorization = 0, Title = "Delete" });
            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", ViewMetaCode = "SO_LISTVIEW", MetaType = "FILTER", DataTableMetaCode = "SALESORDER", MetaCode = "SO_FUNC_FILTER", Path = "", RequiredAuthorization = 0, Title = "Filter" });
            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", ViewMetaCode = "SO_LISTVIEW", MetaType = "PAGING", DataTableMetaCode = "SALESORDER", MetaCode = "SO_FUNC_PAGING", Path = "", RequiredAuthorization = 0, Title = "Paging" });

            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", ViewMetaCode = "SO_CREATE_VIEW", MetaType = "SAVE", DataTableMetaCode = "SALESORDER", MetaCode = "SO_FUNC_SAVE", Path = "SalesOrders/API/Save", RequiredAuthorization = 0, Title = "Save", Properties = "AFTERSAVEACTION=GOTOLISTVIEW#GOTOLISTVIEWPATH=SalesOrders/List" });
            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", ViewMetaCode = "SO_CREATE_VIEW", MetaType = "NAVIGATE", DataTableMetaCode = "SALESORDER", MetaCode = "SO_FUNC_BACK", Path = "SalesOrders/List", RequiredAuthorization = 0, Title = "Back to list" });

            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", ViewMetaCode = "SO_EDIT_VIEW", MetaType = "SAVE", MetaCode = "SO_FUNC_SAVE1", DataTableMetaCode = "SALESORDER", Path = "SalesOrders/API/Save", RequiredAuthorization = 0, Title = "Save", Properties = "AFTERSAVEACTION=REFRESH" });
            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", ViewMetaCode = "SO_EDIT_VIEW", MetaType = "NAVIGATE", MetaCode = "SO_FUNC_BACK1", DataTableMetaCode = "SALESORDER", Path = "SalesOrders/List", RequiredAuthorization = 0, Title = "Back to list" });



            #endregion



            #region Vendor

            //DATABASE - MAINTABLE
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", MetaType = "DATACOLUMN", MetaCode = "VENDORID", DbName = "VendorId", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "DEFVALUE=AUTO#DEFVALUE_START=1000#DEFVALUE_PREFIX=VEND#DEFVALUE_SEED=100#UNIQUE=TRUE#MANDATORY=TRUE" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", MetaType = "DATACOLUMN", MetaCode = "VENDORNAME", DbName = "VendorName", ParentMetaCode = "ROOT", DataType = "STRING" });

            //ITEM - VIEWS
            views.Add(new ViewItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", MetaCode = "VEND_LISTVIEW", MetaType = "UIVIEW", Title = "Vendor List", TitleLocalizationKey = "VENDORLIST", Path = "Vendors/List", IsPrimary = true, IsPublic = false });
            views.Add(new ViewItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", MetaCode = "VEND_CREATE_VIEW", MetaType = "UIVIEW", Title = "Create Sales Order", TitleLocalizationKey = "", Path = "Vendors/Create", IsPrimary = false, IsPublic = false });
            views.Add(new ViewItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", MetaCode = "VEND_EDIT_VIEW", MetaType = "UIVIEW", Title = "Edit Sales Order", TitleLocalizationKey = "", Path = "Vendors/Edit/{id}", IsPrimary = false, IsPublic = false });

            //UI
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", ViewMetaCode = "VEND_LISTVIEW", MetaCode = "VEND_LISTUI", MetaType = "LISTINTERFACE", DataTableMetaCode = "VENDOR" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", ViewMetaCode = "VEND_CREATE_VIEW", MetaCode = "VEND_INPUTUI", MetaType = "INPUTINTERFACE", DataTableMetaCode = "VENDOR" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", ViewMetaCode = "VEND_EDIT_VIEW", MetaCode = "VEND_INPUTUI", MetaType = "INPUTINTERFACE", DataTableMetaCode = "VENDOR" });


            //INPUT UI
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", UserInterfaceMetaCode = "VEND_INPUTUI", MetaType = "SECTION", MetaCode = "MAINSECTION", Title = "Sales Header", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", UserInterfaceMetaCode = "VEND_INPUTUI", MetaType = "PANEL", MetaCode = "PNL1",  Title = "Basics", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", UserInterfaceMetaCode = "VEND_INPUTUI", MetaType = "TEXTBOX", MetaCode = "TB_VENDID", DataColumn1MetaCode = "VENDORID", Title = "Vendor ID", ParentMetaCode = "PNL1", RowOrder = 1, ColumnOrder = 1, Properties = "READONLY=TRUE" });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", UserInterfaceMetaCode = "VEND_INPUTUI", MetaType = "TEXTBOX", MetaCode = "TB_VENDNAME", DataColumn1MetaCode = "VENDORNAME", Title = "Vendor Name", ParentMetaCode = "PNL1", RowOrder = 2, ColumnOrder = 1 });

            //TABLE UI
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", UserInterfaceMetaCode = "VEND_LISTUI", MetaType = "TABLE", MetaCode = "MAIN_EDITLISTVIEW", Title = "Vendor List", TitleLocalizationKey = "VENDORLIST", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", UserInterfaceMetaCode = "VEND_LISTUI", MetaType = "TABLETEXTCOLUMN", MetaCode = "LV_ID", DataColumn1MetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 1 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", UserInterfaceMetaCode = "VEND_LISTUI", MetaType = "TABLETEXTCOLUMN", MetaCode = "LV_VENDID", DataColumn1MetaCode = "VENDORID", Title = "Vendor ID", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 2 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", UserInterfaceMetaCode = "VEND_LISTUI", MetaType = "TABLETEXTCOLUMN", MetaCode = "LV_VENDNAME", DataColumn1MetaCode = "VENDORNAME", Title = "Vendor Name", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 3 });



            //FUNCTIONS
            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", ViewMetaCode = "VEND_LISTVIEW", MetaType = "CREATE", DataTableMetaCode = "VENDOR", MetaCode = "VEND_FUNC_CREATE", Path = "Vendors/Create", RequiredAuthorization = 0, Title = "Create" });
            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", ViewMetaCode = "VEND_LISTVIEW", MetaType = "EDIT", DataTableMetaCode = "VENDOR", MetaCode = "VEND_FUNC_EDIT", Path = "Vendors/Edit", RequiredAuthorization = 0, Title = "Edit" });
            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", ViewMetaCode = "VEND_LISTVIEW", MetaType = "DELETE", DataTableMetaCode = "VENDOR", MetaCode = "VEND_FUNC_DELETE", Path = "Vendors/API/Delete", RequiredAuthorization = 0, Title = "Delete" });
            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", ViewMetaCode = "VEND_LISTVIEW", MetaType = "FILTER", DataTableMetaCode = "VENDOR", MetaCode = "VEND_FUNC_FILTER", Path = "", RequiredAuthorization = 0, Title = "Filter" });
            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", ViewMetaCode = "VEND_LISTVIEW", MetaType = "PAGING", DataTableMetaCode = "VENDOR", MetaCode = "VEND_FUNC_PAGING", Path = "", RequiredAuthorization = 0, Title = "Paging" });

            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", ViewMetaCode = "VEND_CREATE_VIEW", MetaType = "SAVE", DataTableMetaCode = "VENDOR", MetaCode = "VEND_FUNC_SAVE", Path = "Vendors/API/Save", RequiredAuthorization = 0, Title = "Save", Properties = "AFTERSAVEACTION=GOTOLISTVIEW#GOTOLISTVIEWPATH=Vendors/List" });
            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", ViewMetaCode = "VEND_CREATE_VIEW", MetaType = "NAVIGATE", DataTableMetaCode = "VENDOR", MetaCode = "VEND_FUNC_BACK", Path = "Vendors/List", RequiredAuthorization = 0, Title = "Back to list" });

            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", ViewMetaCode = "VEND_EDIT_VIEW", MetaType = "SAVE", MetaCode = "VEND_FUNC_SAVE1", DataTableMetaCode = "VENDOR", Path = "Vendors/API/Save", RequiredAuthorization = 0, Title = "Save", Properties = "AFTERSAVEACTION=REFRESH" });
            functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", ViewMetaCode = "VEND_EDIT_VIEW", MetaType = "NAVIGATE", MetaCode = "VEND_FUNC_BACK1", DataTableMetaCode = "VENDOR", Path = "Vendors/List", RequiredAuthorization = 0, Title = "Back to list" });



            #endregion

            #region Blog

            //DATABASE - MAINTABLE
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "DATACOLUMN", MetaCode = "POSTHEADER", DbName = "ArticleHeader", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "DATACOLUMN", MetaCode = "POSTTEXT", DbName = "ArticleText", ParentMetaCode = "ROOT", DataType = "TEXT", Properties = "" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "DATACOLUMN", MetaCode = "POSTIMAGE", DbName = "ArticleImage", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "DATATABLE", MetaCode = "POSTCOMMENTS", DbName = "blog_ArticleComments", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "DATACOLUMN", MetaCode = "COMMENTHEADER", DbName = "CommentLabel", ParentMetaCode = "POSTCOMMENTS", DataType = "STRING", Properties = "" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "DATACOLUMN", MetaCode = "COMMENTTEXT", DbName = "CommentText", ParentMetaCode = "POSTCOMMENTS", DataType = "TEXT", Properties = "" });

            //ITEM - VIEWS
            views.Add(new ViewItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaCode = "BLOG_LISTVIEW", MetaType = "UIVIEW", Title = "Blog Posts", TitleLocalizationKey = "", Path = "Blog/List", IsPrimary = true, IsPublic = false });
            views.Add(new ViewItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaCode = "BLOG_CREATE_VIEW", MetaType = "UIVIEW", Title = "Create Blog Post", TitleLocalizationKey = "", Path = "Blog/Create", IsPrimary = false, IsPublic = false });
            views.Add(new ViewItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaCode = "BLOG_EDIT_VIEW", MetaType = "UIVIEW", Title = "Edit Blog Post", TitleLocalizationKey = "", Path = "Blog/Edit/{id}", IsPrimary = false, IsPublic = false });
            views.Add(new ViewItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaCode = "BLOG_PUBLIC_VIEW", MetaType = "UIVIEW", Title = "The Intwenty Blog", TitleLocalizationKey = "", Path = "Blog/Index", IsPrimary = false, IsPublic = true });

            //UI
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", ViewMetaCode = "BLOG_LISTVIEW", MetaCode = "BLOG_LISTUI", MetaType = "LISTINTERFACE", DataTableMetaCode = "BLOGAPP" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", ViewMetaCode = "BLOG_CREATE_VIEW", MetaCode = "BLOG_INPUTUI", MetaType = "INPUTINTERFACE", DataTableMetaCode = "BLOGAPP" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", ViewMetaCode = "BLOG_EDIT_VIEW", MetaCode = "BLOG_INPUTUI", MetaType = "INPUTINTERFACE", DataTableMetaCode = "BLOGAPP" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", ViewMetaCode = "BLOG_PUBLIC_VIEW", MetaCode = "BLOG_PUBUI", MetaType = "LISTINTERFACE", DataTableMetaCode = "BLOGAPP" });


            //UI
            //INPUT
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", UserInterfaceMetaCode = "BLOG_INPUTUI", MetaType = "CREATEVIEW", MetaCode = "ADDEDITVIEW", Title = "Add a new article", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "" });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", UserInterfaceMetaCode = "BLOG_INPUTUI", MetaType = "SECTION", MetaCode = "MAINSECTION", Title = "Article", ParentMetaCode = "ADDEDITVIEW", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", UserInterfaceMetaCode = "BLOG_INPUTUI", MetaType = "PANEL", MetaCode = "PNL1", Title = "", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", UserInterfaceMetaCode = "BLOG_INPUTUI", MetaType = "TEXTBOX", MetaCode = "TB_HEADER", DataColumn1MetaCode = "POSTHEADER", Title = "Article Header", ParentMetaCode = "PNL1", RowOrder = 1, ColumnOrder = 1, Properties = "" });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", UserInterfaceMetaCode = "BLOG_INPUTUI", MetaType = "TEXTAREA", MetaCode = "TB_TEXT", DataColumn1MetaCode = "POSTTEXT", Title = "Article Text", ParentMetaCode = "PNL1", RowOrder = 2, ColumnOrder = 1 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", UserInterfaceMetaCode = "BLOG_INPUTUI", MetaType = "PANEL", MetaCode = "PNL2", Title = "", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", UserInterfaceMetaCode = "BLOG_INPUTUI", MetaType = "IMAGEBOX", MetaCode = "IMGBOX_ARTIMAGE", DataColumn1MetaCode = "POSTIMAGE", Title = "Image", ParentMetaCode = "PNL2", RowOrder = 1, ColumnOrder = 1, Properties = "" });

            //LIST (Presentation)
            /*
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", UserInterfaceMetaCode = "BLOG_PUBUI", MetaType = "LISTVIEW", MetaCode = "ARTLISTVIEW", Title = "", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "" });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", UserInterfaceMetaCode = "BLOG_PUBUI", MetaType = "SECTION", MetaCode = "LVSECTION", Title = "", ParentMetaCode = "ARTLISTVIEW", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", UserInterfaceMetaCode = "BLOG_PUBUI", MetaType = "PANEL", MetaCode = "LVPANEL", Title = "", ParentMetaCode = "LVSECTION", RowOrder = 1, ColumnOrder = 1 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", UserInterfaceMetaCode = "BLOG_PUBUI", MetaType = "IMAGE", MetaCode = "LV_POSTIMAGE", DataColumn1MetaCode = "POSTIMAGE", Title = "", ParentMetaCode = "LVPANEL", RowOrder = 1, ColumnOrder = 1, Properties = "" });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", UserInterfaceMetaCode = "BLOG_PUBUI", MetaType = "LABEL", MetaCode = "LV_POSTLABEL", DataColumn1MetaCode = "POSTHEADER", Title = "", ParentMetaCode = "LVPANEL", RowOrder = 2, ColumnOrder = 1, Properties = "" });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", UserInterfaceMetaCode = "BLOG_PUBUI", MetaType = "TEXTBLOCK", MetaCode = "LV_POSTTEXT", DataColumn1MetaCode = "POSTTEXT", Title = "", ParentMetaCode = "LVPANEL", RowOrder = 3, ColumnOrder = 1, Properties = "" });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", UserInterfaceMetaCode = "BLOG_PUBUI", MetaType = "PANEL", MetaCode = "LVRIGHTPANEL", Title = "", ParentMetaCode = "LVSECTION", RowOrder = 1, ColumnOrder = 2 });
            */


            //LIST
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", UserInterfaceMetaCode = "BLOG_LISTUI", MetaType = "TABLE", MetaCode = "MAIN_EDITLISTVIEW", Title = "Articles", TitleLocalizationKey = "", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", UserInterfaceMetaCode = "BLOG_LISTUI", MetaType = "TABLETEXTCOLUMN", MetaCode = "ELV_ID", DataColumn1MetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 1 });
            userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", UserInterfaceMetaCode = "BLOG_LISTUI", MetaType = "TABLETEXTCOLUMN", MetaCode = "ELV_POSTHEADER", DataColumn1MetaCode = "POSTHEADER", Title = "Article Header", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 2 });


            //FUNCTIONS
            functions.Add(new FunctionItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", ViewMetaCode = "BLOG_LISTVIEW", MetaType = "CREATE", DataTableMetaCode = "BLOGAPP", MetaCode = "BLOG_FUNC_CREATE", Path = "Blog/Create", RequiredAuthorization = 0, Title = "Create" });
            functions.Add(new FunctionItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", ViewMetaCode = "BLOG_LISTVIEW", MetaType = "EDIT", DataTableMetaCode = "BLOGAPP", MetaCode = "BLOG_FUNC_EDIT", Path = "Blog/Edit", RequiredAuthorization = 0, Title = "Edit" });
            functions.Add(new FunctionItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", ViewMetaCode = "BLOG_LISTVIEW", MetaType = "DELETE", DataTableMetaCode = "BLOGAPP", MetaCode = "BLOG_FUNC_DELETE", Path = "Blog/API/Delete", RequiredAuthorization = 0, Title = "Delete" });
            functions.Add(new FunctionItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", ViewMetaCode = "BLOG_LISTVIEW", MetaType = "FILTER", DataTableMetaCode = "BLOGAPP", MetaCode = "BLOG_FUNC_FILTER", Path = "", RequiredAuthorization = 0, Title = "Filter" });
            functions.Add(new FunctionItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", ViewMetaCode = "BLOG_LISTVIEW", MetaType = "PAGING", DataTableMetaCode = "BLOGAPP", MetaCode = "BLOG_FUNC_PAGING", Path = "", RequiredAuthorization = 0, Title = "Paging" });

            functions.Add(new FunctionItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", ViewMetaCode = "BLOG_CREATE_VIEW", MetaType = "SAVE", DataTableMetaCode = "BLOGAPP", MetaCode = "BLOG_FUNC_SAVE", Path = "Blog/API/Save", RequiredAuthorization = 0, Title = "Save", Properties = "AFTERSAVEACTION=GOTOLISTVIEW#GOTOLISTVIEWPATH=Blog/List" });
            functions.Add(new FunctionItem() { SystemMetaCode = "BLOG", AppMetaCode = "VENBLOGAPPDOR", ViewMetaCode = "BLOG_CREATE_VIEW", MetaType = "NAVIGATE", DataTableMetaCode = "BLOGAPP", MetaCode = "BLOG_FUNC_BACK", Path = "Blog/List", RequiredAuthorization = 0, Title = "Back to list" });

            functions.Add(new FunctionItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", ViewMetaCode = "BLOG_EDIT_VIEW", MetaType = "SAVE", MetaCode = "BLOG_FUNC_SAVE1", DataTableMetaCode = "BLOGAPP", Path = "Blog/API/Save", RequiredAuthorization = 0, Title = "Save", Properties = "AFTERSAVEACTION=REFRESH" });
            functions.Add(new FunctionItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", ViewMetaCode = "BLOG_EDIT_VIEW", MetaType = "NAVIGATE", MetaCode = "BLOG_FUNC_BACK1", DataTableMetaCode = "BLOGAPP", Path = "Blog/List", RequiredAuthorization = 0, Title = "Back to list" });




            #endregion

            #region Dataviews

            //DATAVIEWS
            //---------
            //CUSTOMER
            dataviews.Add(new DataViewItem() { SystemMetaCode = "WAREHOUSE", MetaType = "DATAVIEW", MetaCode = "CUSTOMERVIEW", ParentMetaCode = "ROOT", SQLQuery = "select CustomerId, CustomerName from wms_Customer order by CustomerId asc", Title = "Customers", TitleLocalizationKey = "CUSTOMERS", SQLQueryFieldName = "" });
            dataviews.Add(new DataViewItem() { SystemMetaCode = "WAREHOUSE", MetaType = "DATAVIEWKEYCOLUMN", MetaCode = "VCUSTID", ParentMetaCode = "CUSTOMERVIEW", SQLQuery = "", Title = "Id", TitleLocalizationKey = "", SQLQueryFieldName = "CustomerId", SQLQueryFieldDataType = "STRING" });
            dataviews.Add(new DataViewItem() { SystemMetaCode = "WAREHOUSE", MetaType = "DATAVIEWCOLUMN", MetaCode = "VCUSTNAME", ParentMetaCode = "CUSTOMERVIEW", SQLQuery = "", Title = "Name", TitleLocalizationKey = "NAME", SQLQueryFieldName = "CustomerName", SQLQueryFieldDataType = "STRING" });


            //VENDOR
            dataviews.Add(new DataViewItem() { SystemMetaCode = "WAREHOUSE", MetaType = "DATAVIEW", MetaCode = "VENDORVIEW", ParentMetaCode = "ROOT", SQLQuery = "select VendorId, VendorName from wms_Vendor order by VendorId asc", Title = "Vendors", SQLQueryFieldName = "" });
            dataviews.Add(new DataViewItem() { SystemMetaCode = "WAREHOUSE", MetaType = "DATAVIEWKEYCOLUMN", MetaCode = "VF_VENDID", ParentMetaCode = "VENDORVIEW", SQLQuery = "", Title = "Id", SQLQueryFieldName = "VendorId", SQLQueryFieldDataType = "STRING" });
            dataviews.Add(new DataViewItem() { SystemMetaCode = "WAREHOUSE", MetaType = "DATAVIEWCOLUMN", MetaCode = "VF_VENDNAME", ParentMetaCode = "VENDORVIEW", SQLQuery = "", Title = "Name", SQLQueryFieldName = "VendorName", SQLQueryFieldDataType = "STRING" });

            //ITEM
            dataviews.Add(new DataViewItem() { SystemMetaCode = "WAREHOUSE", MetaType = "DATAVIEW", MetaCode = "ITEMLOOKUP", ParentMetaCode = "ROOT", SQLQuery = "select ItemId, ItemName from wms_Item order by ItemId asc", Title = "Items", SQLQueryFieldName = "" });
            dataviews.Add(new DataViewItem() { SystemMetaCode = "WAREHOUSE", MetaType = "DATAVIEWKEYCOLUMN", MetaCode = "ITEMID", ParentMetaCode = "ITEMLOOKUP", SQLQuery = "", Title = "Id", SQLQueryFieldName = "ItemId", SQLQueryFieldDataType = "STRING" });
            dataviews.Add(new DataViewItem() { SystemMetaCode = "WAREHOUSE", MetaType = "DATAVIEWCOLUMN", MetaCode = "ITEMNAME", ParentMetaCode = "ITEMLOOKUP", SQLQuery = "", Title = "Name", SQLQueryFieldName = "ItemName", SQLQueryFieldDataType = "STRING" });

            #endregion
        
            #region endpoints

            //Define som endpoints for the customer application 
            endpoints.Add(new EndpointItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "TABLEGET", MetaCode = "EP_CUSTOMER_GETLATEST", DataMetaCode = "CUSTOMER", Path = "MyAPI/Customer", Title = "Get customer", Description = "Get latest version of a customer by id", ParentMetaCode = "ROOT" });
            endpoints.Add(new EndpointItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "TABLELIST", MetaCode = "EP_CUSTOMER_GETALL", DataMetaCode = "CUSTOMER", Path = "MyAPI/Customers", Title = "Get all customers", Description = "Get all customers in the database", ParentMetaCode = "ROOT" });
            endpoints.Add(new EndpointItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "TABLESAVE", MetaCode = "EP_CUSTOMER_SAVE", DataMetaCode = "CUSTOMER", Path = "MyAPI/Customer/Save", Title = "Save customer", Description = "Create or update customer", ParentMetaCode = "ROOT" });

            //Define  endpoint for the vendor dataview
            endpoints.Add(new EndpointItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "", MetaType = "DATAVIEWLIST", MetaCode = "EP_DV_1", DataMetaCode = "VENDORVIEW", Path = "MyAPI/Vendors", Title = "Get Vendors", Description = "Get vendors by using an intwenty dataview", ParentMetaCode = "ROOT" });

            //Define free unbound endpoint
            endpoints.Add(new EndpointItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "", MetaType = "CUSTOMPOST", MetaCode = "EP_CUST_1", DataMetaCode = "", Path = "MyAPI/PostAnyThing", Title = "Post Anything", Description = "This endpoint must be implemented in a custom controller", ParentMetaCode = "ROOT" });

            //Authentication API
            endpoints.Add(new EndpointItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "", MetaType = "CUSTOMPOST", MetaCode = "EP_AUTH", DataMetaCode = "", Path = "MyAPI/Authenticate", Title = "Authenticate with a product", Description = "Authenticates a user", ParentMetaCode = "ROOT" });

            #endregion


            //Insert models if not existing
            var client = new Connection(Settings.Value.DefaultConnectionDBMS, Settings.Value.DefaultConnection);
            client.Open();

            var current_systems = client.GetEntities<SystemItem>();
            foreach (var t in systems)
            {
                if (!current_systems.Exists(p => p.MetaCode == t.MetaCode))
                    client.InsertEntity(t);
            }

            var current_apps = client.GetEntities<ApplicationItem>();
            foreach (var t in applications)
            {
                if (!current_apps.Exists(p => p.MetaCode == t.MetaCode && p.SystemMetaCode == t.SystemMetaCode))
                    client.InsertEntity(t);
            }

            var current_domains = client.GetEntities<ValueDomainItem>();
            foreach (var t in valuedomains)
            {
                if (!current_domains.Exists(p => p.DomainName ==  t.DomainName))
                    client.InsertEntity(t);
            }

            var current_views = client.GetEntities<ViewItem>();
            foreach (var t in views)
            {
                if (!current_views.Exists(p => p.MetaCode == t.MetaCode && p.AppMetaCode == t.AppMetaCode))
                    client.InsertEntity(t);
            }

            var current_userinterface = client.GetEntities<UserInterfaceItem>();
            foreach (var t in userinterface)
            {
                if (!current_userinterface.Exists(p => p.MetaCode == t.MetaCode && p.AppMetaCode == t.AppMetaCode))
                    client.InsertEntity(t);
            }

            var current_ui_structure = client.GetEntities<UserInterfaceStructureItem>();
            foreach (var t in userinterfacestructure)
            {
                if (!current_ui_structure.Exists(p => p.MetaCode == t.MetaCode && p.AppMetaCode == t.AppMetaCode))
                    client.InsertEntity(t);
            }

            var current_functions = client.GetEntities<FunctionItem>();
            foreach (var t in functions)
            {
                if (!current_functions.Exists(p => p.MetaCode == t.MetaCode && p.AppMetaCode == t.AppMetaCode))
                    client.InsertEntity(t);
            }

            var current_db = client.GetEntities<DatabaseItem>();
            foreach (var t in dbitems)
            {
                if (!current_db.Exists(p => p.MetaCode == t.MetaCode && p.AppMetaCode == t.AppMetaCode))
                    client.InsertEntity(t);
            }

          

            var current_dataview = client.GetEntities<DataViewItem>();
            foreach (var t in dataviews)
            {
                if (!current_dataview.Exists(p => p.MetaCode == t.MetaCode && p.MetaType == t.MetaType))
                    client.InsertEntity(t);
            }

            var current_endpoints = client.GetEntities<EndpointItem>();
            foreach (var t in endpoints)
            {
                if (!current_endpoints.Exists(p => p.MetaCode == t.MetaCode && p.AppMetaCode == t.AppMetaCode))
                    client.InsertEntity(t);
            }

            client.Close();
           
        }

        public static void SeedLocalizations(IServiceProvider services)
        {
            var Settings = services.GetRequiredService<IOptions<IntwentySettings>>();
            if (!Settings.Value.SeedLocalizationsOnStartUp)
                return;

            var translations = new List<TranslationItem>();

            translations.Add(new TranslationItem() { Culture = "en-US", TransKey = "VIEW_CUST_ADDEDITVIEW", Text = "New Customer" });
            translations.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "VIEW_CUST_ADDEDITVIEW", Text = "Ny Kund" });
            translations.Add(new TranslationItem() { Culture = "en-US", TransKey = "CUSTOMER", Text = "Customer" });
            translations.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "CUSTOMER", Text = "Kund" });
            translations.Add(new TranslationItem() { Culture = "en-US", TransKey = "CUSTOMERS", Text = "Customers" });
            translations.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "CUSTOMERS", Text = "Kunder" });
            translations.Add(new TranslationItem() { Culture = "en-US", TransKey = "CUSTOMERLIST", Text = "Customer list" });
            translations.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "CUSTOMERLIST", Text = "Kunder" });
            translations.Add(new TranslationItem() { Culture = "en-US", TransKey = "CUSTOMERID", Text = "Customer ID" });
            translations.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "CUSTOMERID", Text = "Kund ID" });
            translations.Add(new TranslationItem() { Culture = "en-US", TransKey = "CUSTOMERNAME", Text = "Customer Name" });
            translations.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "CUSTOMERNAME", Text = "Kund namn" });
            translations.Add(new TranslationItem() { Culture = "en-US", TransKey = "CUSTOMERCONTACT", Text = "Contact" });
            translations.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "CUSTOMERCONTACT", Text = "Kontakt" });
            translations.Add(new TranslationItem() { Culture = "en-US", TransKey = "CUSTOMERPHONE", Text = "Phone" });
            translations.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "CUSTOMERPHONE", Text = "Telefon" });
            translations.Add(new TranslationItem() { Culture = "en-US", TransKey = "CUSTOMEREMAIL", Text = "E-Mail" });
            translations.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "CUSTOMEREMAIL", Text = "E-Post" });
            translations.Add(new TranslationItem() { Culture = "en-US", TransKey = "NAME", Text = "Name" });
            translations.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "NAME", Text = "Namn" });
            translations.Add(new TranslationItem() { Culture = "en-US", TransKey = "MENU", Text = "Menu" });
            translations.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "MENU", Text = "Meny" });
            translations.Add(new TranslationItem() { Culture = "en-US", TransKey = "ITEM", Text = "Item" });
            translations.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "ITEM", Text = "Artikel" });
            translations.Add(new TranslationItem() { Culture = "en-US", TransKey = "ITEMLIST", Text = "Item list" });
            translations.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "ITEMLIST", Text = "Artiklar" });
            translations.Add(new TranslationItem() { Culture = "en-US", TransKey = "SALESORDER", Text = "Sales Order" });
            translations.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "SALESORDER", Text = "Säljorder" });
            translations.Add(new TranslationItem() { Culture = "en-US", TransKey = "SALESORDERLIST", Text = "Sales Orders" });
            translations.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "SALESORDERLIST", Text = "Säljordrar" });
            translations.Add(new TranslationItem() { Culture = "en-US", TransKey = "VENDOR", Text = "Vendor" });
            translations.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "VENDOR", Text = "Tillverkare" });
            translations.Add(new TranslationItem() { Culture = "en-US", TransKey = "VENDORLIST", Text = "Vendors" });
            translations.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "VENDORLIST", Text = "Tillverkare" });

            var client = new Connection(Settings.Value.DefaultConnectionDBMS, Settings.Value.DefaultConnection);
            client.Open();

            var current_trans = client.GetEntities<TranslationItem>();
            foreach (var t in translations)
            {
                if (!current_trans.Exists(p => p.Culture == t.Culture && p.TransKey == t.TransKey))
                    client.InsertEntity(t);
            }

            client.Close();
        }

        public static void ConfigureDatabase(IServiceProvider services)
        {
            var Settings = services.GetRequiredService<IOptions<IntwentySettings>>();
            if (!Settings.Value.ConfigureDatabaseOnStartUp)
                return;

            var modelservice = services.GetRequiredService<IIntwentyModelService>();
            modelservice.ConfigureDatabase();

        }

        public static void SeedData(IServiceProvider services)
        {
            var Settings = services.GetRequiredService<IOptions<IntwentySettings>>();
            if (!Settings.Value.SeedDataOnStartUp)
                return;

          

        }

      


    }
}
