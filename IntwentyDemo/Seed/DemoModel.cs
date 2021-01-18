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
using Intwenty.Areas.Identity.Entity;

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
            var userinterface = new List<UserInterfaceItem>();
            var dbitems = new List<DatabaseItem>();
            var dataviews = new List<DataViewItem>();
            var endpoints = new List<EndpointItem>();

            //SYSTEMS
            systems.Add(new SystemItem() { MetaCode = "INTWENTYDEFAULTSYS", Title="Default", DbPrefix = "def" });
            systems.Add(new SystemItem() { MetaCode = "WAREHOUSE", Title = "WMS", DbPrefix ="wms" });
            systems.Add(new SystemItem() { MetaCode = "BLOG", Title = "The blog engine", DbPrefix = "blog" });

            //APPLICATIONS
            applications.Add(new ApplicationItem() { Id = 10, Description = "An app for managing customers", SystemMetaCode="WAREHOUSE", MetaCode = "CUSTOMER", Title = "Customer", TitleLocalizationKey="CUSTOMER", DbName = "wms_Customer", IsHierarchicalApplication = false, UseVersioning = false, ApplicationPath="Customers", CreateViewRequirement="AUTH", EditViewRequirement="AUTH", EditListViewRequirement="AUTH", DetailViewRequirement="AUTH", ListViewRequirement = "AUTH" });
            applications.Add(new ApplicationItem() { Id = 20, Description = "An app for managing items", SystemMetaCode = "WAREHOUSE", MetaCode = "ITEM", Title = "Item", TitleLocalizationKey = "ITEM", DbName = "wms_Item", IsHierarchicalApplication = false, UseVersioning = false, ApplicationPath = "Items", CreateViewRequirement = "AUTH", EditViewRequirement = "AUTH", EditListViewRequirement = "AUTH", DetailViewRequirement = "AUTH", ListViewRequirement = "AUTH" });
            applications.Add(new ApplicationItem() { Id = 30, Description = "An app for managing sales orders", SystemMetaCode = "WAREHOUSE", MetaCode = "SALESORDER", Title = "Sales Order", TitleLocalizationKey = "SALESORDER", DbName = "wms_SalesHeader", IsHierarchicalApplication = false, UseVersioning = false, ApplicationPath = "SalesOrder", CreateViewRequirement = "AUTH", EditViewRequirement = "AUTH", EditListViewRequirement = "AUTH", DetailViewRequirement = "AUTH", ListViewRequirement = "AUTH" });
            applications.Add(new ApplicationItem() { Id = 40, Description = "An app for managing vendors", SystemMetaCode = "WAREHOUSE", MetaCode = "VENDOR", Title = "Vendor", TitleLocalizationKey = "VENDOR", DbName = "wms_Vendor", IsHierarchicalApplication = false, UseVersioning = false, ApplicationPath = "Vendor", CreateViewRequirement = "AUTH", EditViewRequirement = "AUTH", EditListViewRequirement = "AUTH", DetailViewRequirement = "AUTH", ListViewRequirement = "AUTH" });
            applications.Add(new ApplicationItem() { Id = 50, Description = "An app for blogging", SystemMetaCode = "BLOG", MetaCode = "BLOGAPP", Title = "The blog", TitleLocalizationKey = "", DbName = "blog_Blog", IsHierarchicalApplication = false, UseVersioning = false, ApplicationPath = "", CreateViewRequirement = "AUTH", EditViewRequirement = "OWNER", EditListViewRequirement = "OWNER", DetailViewRequirement = "PUB", ListViewRequirement = "PUB" });

            //VALUEDOMAIN (USED IN COMBOBOXES ETC)
            valuedomains.Add(new ValueDomainItem() { DomainName = "ITEMCATEGORY", Code = "PROD", Value = "Products" });
            valuedomains.Add(new ValueDomainItem() { DomainName = "ITEMCATEGORY", Code = "SERV", Value = "Services" });
            valuedomains.Add(new ValueDomainItem() { DomainName = "CUSTOMERTAGS", Code = "1", Value = "Tag 1" });
            valuedomains.Add(new ValueDomainItem() { DomainName = "CUSTOMERTAGS", Code = "2", Value = "Tag 2" });
            valuedomains.Add(new ValueDomainItem() { DomainName = "CUSTOMERTAGS", Code = "3", Value = "Tag 3" });


            #region customer
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

            //UI
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "CREATEVIEW", MetaCode = "ADDEDITVIEW", TitleLocalizationKey= "VIEW_CUST_ADDEDITVIEW", Title = "Create Customer", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "SECTION", MetaCode = "MAINSECTION", Title = "", ParentMetaCode = "ADDEDITVIEW", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "PANEL", MetaCode = "CUSTPNL1", Title = "Basics", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "TEXTBOX", MetaCode = "TB_CUSTID", DataColumn1MetaCode = "CUSTOMERID", Title = "Customer ID", TitleLocalizationKey = "CUSTOMERID", ParentMetaCode = "CUSTPNL1", RowOrder = 1, ColumnOrder = 1, Properties="READONLY=TRUE" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "TEXTBOX", MetaCode = "TB_CUSTNAME", DataColumn1MetaCode = "CUSTOMERNAME", Title = "Customer Name", TitleLocalizationKey = "CUSTOMERNAME", ParentMetaCode = "CUSTPNL1", RowOrder = 2, ColumnOrder = 1 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "MULTISELECT", MetaCode = "MS_TAGS", DataColumn1MetaCode = "CUSTOMERTAGS", DataColumn2MetaCode = "CUSTOMERTAGSTEXT", Domain = "VALUEDOMAIN.CUSTOMERTAGS", Title = "Tags", TitleLocalizationKey = "MS_TAGS", ParentMetaCode = "CUSTPNL1", RowOrder = 3, ColumnOrder = 1 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "PANEL", MetaCode = "CUSTPNL2",  Title = "Contact", TitleLocalizationKey = "CUSTOMERCONTACT", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 2 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "EMAILBOX", MetaCode = "TBCUSTMAIL", DataColumn1MetaCode = "CUSTOMEREMAIL", Title = "Email", TitleLocalizationKey = "CUSTOMEREMAIL", ParentMetaCode = "CUSTPNL2", RowOrder = 1, ColumnOrder = 2 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "NUMBOX", MetaCode = "TBCUSTPHONE", DataColumn1MetaCode = "CUSTOMERPHONE", Title = "Phone", TitleLocalizationKey = "CUSTOMERPHONE", ParentMetaCode = "CUSTPNL2", RowOrder = 2, ColumnOrder = 2 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "TEXTBOX", MetaCode = "TBCUSTSTAT", DataColumn1MetaCode = "CUSTOMERSTATUS", Title = "Status", TitleLocalizationKey = "", ParentMetaCode = "CUSTPNL2", RowOrder = 3, ColumnOrder = 2, Properties="READONLY=TRUE" });

            //EDIT LISTVIEW
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "EDITLISTVIEW", MetaCode = "MAIN_EDITLISTVIEW", Title = "Customer List", TitleLocalizationKey = "CUSTOMERLIST", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "EDITLISTVIEWCOLUMN", MetaCode = "LV_ID", DataColumn1MetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 1 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "EDITLISTVIEWCOLUMN", MetaCode = "LV_CUSTID", DataColumn1MetaCode = "CUSTOMERID", Title = "Customer ID", TitleLocalizationKey = "CUSTOMERID", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 2 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "EDITLISTVIEWCOLUMN", MetaCode = "LV_CUSTNAME", DataColumn1MetaCode = "CUSTOMERNAME", Title = "Customer Name", TitleLocalizationKey = "CUSTOMERNAME", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 3 });
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

            //UI
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "CREATEVIEW", MetaCode = "ADDEDITVIEW", Title = "Add a new Item", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "SECTION", MetaCode = "MAINSECTION", Title = "Basics", ParentMetaCode = "ADDEDITVIEW", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "PANEL", MetaCode = "ITMPNL_A", Title = "Basics", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "TEXTBOX", MetaCode = "TB_ITID", DataColumn1MetaCode = "ITEMID", Title = "Item ID", ParentMetaCode = "ITMPNL_A", RowOrder = 1, ColumnOrder = 1, Properties = "READONLY=TRUE" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "TEXTBOX", MetaCode = "TB_ITNAME", DataColumn1MetaCode = "NAME", Title = "Item Name", ParentMetaCode = "ITMPNL_A", RowOrder = 2, ColumnOrder = 1 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "DATEPICKER", MetaCode = "DP_MOD", DataColumn1MetaCode = "MODIFIED", Title = "Modified Date", ParentMetaCode = "ITMPNL_A", RowOrder = 3, ColumnOrder = 1 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "CHECKBOX", MetaCode = "CB_ACTIVE", DataColumn1MetaCode = "ACTIVE", Title = "Is Active", ParentMetaCode = "ITMPNL_A", RowOrder = 4, ColumnOrder = 1 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "PANEL", MetaCode = "ITMPNL_B", Title = "Basics", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 2 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "COMBOBOX", MetaCode = "CB_CATEGORY", DataColumn1MetaCode = "CATCODE", Title = "Category", ParentMetaCode = "ITMPNL_B", RowOrder = 1, ColumnOrder = 2, Domain = "VALUEDOMAIN.ITEMCATEGORY" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "NUMBOX", MetaCode = "NUMBOX_PURCHPRICE", DataColumn1MetaCode = "PURCHASEPRICE", Title = "Purchase Price", ParentMetaCode = "ITMPNL_B", RowOrder = 2, ColumnOrder = 2 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "NUMBOX", MetaCode = "NUMBOX_SALESPRICE", DataColumn1MetaCode = "SALESPRICE", Title = "Sales Pricee", ParentMetaCode = "ITMPNL_B", RowOrder = 3, ColumnOrder = 2 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "LOOKUP", MetaCode = "LOOKUPVEND", DataColumn1MetaCode = "VENDORCODE", DataColumn2MetaCode = "VENDORTXT", DataViewMetaCode = "VENDORVIEW", DataViewColumn1MetaCode = "VF_VENDID", DataViewColumn2MetaCode = "VF_VENDNAME", Title = "Vendor", ParentMetaCode = "ITMPNL_B", RowOrder = 4, ColumnOrder = 2 });

            //EDITLISTVIEW
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "EDITLISTVIEW", MetaCode = "MAIN_EDITLISTVIEW", Title = "Item List", TitleLocalizationKey = "ITEMLIST", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "EDITLISTVIEWCOLUMN", MetaCode = "LV_ID", DataColumn1MetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 1 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "EDITLISTVIEWCOLUMN", MetaCode = "LV_ITEMID", DataColumn1MetaCode = "ITEMID", Title = "Item ID", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 2 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "EDITLISTVIEWCOLUMN", MetaCode = "LV_ITEMNAME", DataColumn1MetaCode = "NAME", Title = "Item Name", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 3 });
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


            //EDITLISTVIEW
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "EDITLISTVIEW", MetaCode = "MAIN_EDITLISTVIEW", Title = "Sales Orders", TitleLocalizationKey = "SALESORDERLIST", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0, Properties = "" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "EDITLISTVIEWCOLUMN", MetaCode = "LV_ID", DataColumn1MetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 1 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "EDITLISTVIEWCOLUMN", MetaCode = "LF_ORDERID", DataColumn1MetaCode = "ORDERNO", Title = "Order No", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 2 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "EDITLISTVIEWCOLUMN", MetaCode = "LF_CUSTNAME", DataColumn1MetaCode = "CUSTNAME", Title = "Customer", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 3 });

            //UI
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "CREATEVIEW", MetaCode = "ADDEDITVIEW", Title = "Create a new sales order", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "SECTION", MetaCode = "SECT_HDR", Title = "Sales Header", ParentMetaCode = "ADDEDITVIEW", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "PANEL", MetaCode = "PANEL1", Title = "", ParentMetaCode = "SECT_HDR", RowOrder = 1, ColumnOrder = 1 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "TEXTBOX", MetaCode = "TB_ORDERNO", DataColumn1MetaCode = "ORDERNO", Title = "Order No", ParentMetaCode = "PANEL1", RowOrder = 1, ColumnOrder = 1, Properties = "READONLY=TRUE" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "DATEPICKER", MetaCode = "DP_ORDERDATE", DataColumn1MetaCode = "ORDERDATE", Title = "Order Date", ParentMetaCode = "PANEL1", RowOrder = 2, ColumnOrder = 1 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "LOOKUP", MetaCode = "LOOKUP_CUSTOMER", DataColumn1MetaCode = "CUSTID", DataColumn2MetaCode = "CUSTNAME", DataViewMetaCode = "CUSTOMERVIEW", DataViewColumn1MetaCode = "VCUSTID", DataViewColumn2MetaCode = "VCUSTNAME", Title = "Customer", ParentMetaCode = "PANEL1", RowOrder = 3, ColumnOrder = 1 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "SECTION", MetaCode = "SECT_LINES", Title = "Sales Lines", ParentMetaCode = "ADDEDITVIEW", RowOrder = 2, ColumnOrder = 1, Properties = "COLLAPSIBLE=TRUE#STARTEXPANDED=TRUE" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "PANEL", MetaCode = "PANEL2", Title = "", ParentMetaCode = "SECT_LINES", RowOrder = 1, ColumnOrder = 1 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "EDITGRID", MetaCode = "LINETABLE", DataTableMetaCode = "DTORDLINE", Title = "Sales Lines", ParentMetaCode = "PANEL2", RowOrder = 4, ColumnOrder = 1 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "EDITGRID_LOOKUP", MetaCode = "LINE_ITEMID", DataColumn1MetaCode = "DTORDLINE_ITEMID", DataColumn2MetaCode = "DTORDLINE_ITEMNAME", DataViewMetaCode = "ITEMLOOKUP", DataViewColumn1MetaCode = "ITEMID", DataViewColumn2MetaCode= "ITEMNAME", Title = "Item", ParentMetaCode = "LINETABLE", RowOrder = 1, ColumnOrder = 1 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "EDITGRID_NUMBOX", MetaCode = "LINE_QTY", DataColumn1MetaCode = "DTORDLINE_QTY", Title = "Qty", ParentMetaCode = "LINETABLE", RowOrder = 1, ColumnOrder = 2 });
            #endregion

            #region Vendor

            //DATABASE - MAINTABLE
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", MetaType = "DATACOLUMN", MetaCode = "VENDORID", DbName = "VendorId", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "DEFVALUE=AUTO#DEFVALUE_START=1000#DEFVALUE_PREFIX=VEND#DEFVALUE_SEED=100#UNIQUE=TRUE#MANDATORY=TRUE" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", MetaType = "DATACOLUMN", MetaCode = "VENDORNAME", DbName = "VendorName", ParentMetaCode = "ROOT", DataType = "STRING" });


            //UI
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", MetaType = "CREATEVIEW", MetaCode = "ADDEDITVIEW", Title = "Add a vendor", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", MetaType = "SECTION", MetaCode = "MAINSECTION", Title = "Sales Header", ParentMetaCode = "ADDEDITVIEW", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", MetaType = "PANEL", MetaCode = "PNL1",  Title = "Basics", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", MetaType = "TEXTBOX", MetaCode = "TB_VENDID", DataColumn1MetaCode = "VENDORID", Title = "Vendor ID", ParentMetaCode = "PNL1", RowOrder = 1, ColumnOrder = 1, Properties = "READONLY=TRUE" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", MetaType = "TEXTBOX", MetaCode = "TB_VENDNAME", DataColumn1MetaCode = "VENDORNAME", Title = "Vendor Name", ParentMetaCode = "PNL1", RowOrder = 2, ColumnOrder = 1 });
            
            //EDITLISTVIEW
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", MetaType = "EDITLISTVIEW", MetaCode = "MAIN_EDITLISTVIEW", Title = "Vendor List", TitleLocalizationKey = "VENDORLIST", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", MetaType = "EDITLISTVIEWCOLUMN", MetaCode = "LV_ID", DataColumn1MetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 1 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", MetaType = "EDITLISTVIEWCOLUMN", MetaCode = "LV_VENDID", DataColumn1MetaCode = "VENDORID", Title = "Vendor ID", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 2 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", MetaType = "EDITLISTVIEWCOLUMN", MetaCode = "LV_VENDNAME", DataColumn1MetaCode = "VENDORNAME", Title = "Vendor Name", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 3 });

            #endregion

            #region Blog

            //DATABASE - MAINTABLE
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "DATACOLUMN", MetaCode = "POSTHEADER", DbName = "ArticleHeader", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "DATACOLUMN", MetaCode = "POSTTEXT", DbName = "ArticleText", ParentMetaCode = "ROOT", DataType = "TEXT", Properties = "" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "DATACOLUMN", MetaCode = "POSTIMAGE", DbName = "ArticleImage", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "DATATABLE", MetaCode = "POSTCOMMENTS", DbName = "blog_ArticleComments", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "DATACOLUMN", MetaCode = "COMMENTHEADER", DbName = "CommentLabel", ParentMetaCode = "POSTCOMMENTS", DataType = "STRING", Properties = "" });
            dbitems.Add(new DatabaseItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "DATACOLUMN", MetaCode = "COMMENTTEXT", DbName = "CommentText", ParentMetaCode = "POSTCOMMENTS", DataType = "TEXT", Properties = "" });


            //UI
            //CREATE  - UPDATE
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "CREATEVIEW", MetaCode = "ADDEDITVIEW", Title = "Add a new article", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "SECTION", MetaCode = "MAINSECTION", Title = "Article", ParentMetaCode = "ADDEDITVIEW", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "PANEL", MetaCode = "PNL1", Title = "", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "TEXTBOX", MetaCode = "TB_HEADER", DataColumn1MetaCode = "POSTHEADER", Title = "Article Header", ParentMetaCode = "PNL1", RowOrder = 1, ColumnOrder = 1, Properties = "" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "TEXTAREA", MetaCode = "TB_TEXT", DataColumn1MetaCode = "POSTTEXT", Title = "Article Text", ParentMetaCode = "PNL1", RowOrder = 2, ColumnOrder = 1 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "PANEL", MetaCode = "PNL2", Title = "", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "IMAGEBOX", MetaCode = "IMGBOX_ARTIMAGE", DataColumn1MetaCode = "POSTIMAGE", Title = "Image", ParentMetaCode = "PNL2", RowOrder = 1, ColumnOrder = 1, Properties = "" });

            //LIST (Presentation)
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "LISTVIEW", MetaCode = "ARTLISTVIEW", Title = "", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "SECTION", MetaCode = "LVSECTION", Title = "", ParentMetaCode = "ARTLISTVIEW", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "PANEL", MetaCode = "LVPANEL", Title = "", ParentMetaCode = "LVSECTION", RowOrder = 1, ColumnOrder = 1 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "IMAGE", MetaCode = "LV_POSTIMAGE", DataColumn1MetaCode = "POSTIMAGE", Title = "", ParentMetaCode = "LVPANEL", RowOrder = 1, ColumnOrder = 1, Properties = "" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "LABEL", MetaCode = "LV_POSTLABEL", DataColumn1MetaCode = "POSTHEADER", Title = "", ParentMetaCode = "LVPANEL", RowOrder = 2, ColumnOrder = 1, Properties = "" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "TEXTBLOCK", MetaCode = "LV_POSTTEXT", DataColumn1MetaCode = "POSTTEXT", Title = "", ParentMetaCode = "LVPANEL", RowOrder = 3, ColumnOrder = 1, Properties = "" });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "PANEL", MetaCode = "LVRIGHTPANEL", Title = "", ParentMetaCode = "LVSECTION", RowOrder = 1, ColumnOrder = 2 });



            //EDITLISTVIEW
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "EDITLISTVIEW", MetaCode = "MAIN_EDITLISTVIEW", Title = "Articles", TitleLocalizationKey = "", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "EDITLISTVIEWCOLUMN", MetaCode = "ELV_ID", DataColumn1MetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 1 });
            userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "EDITLISTVIEWCOLUMN", MetaCode = "ELV_POSTHEADER", DataColumn1MetaCode = "POSTHEADER", Title = "Article Header", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 2 });

            #endregion

            #region dataviews
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
            endpoints.Add(new EndpointItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "", MetaType = "CUSTOMPOST", MetaCode = "EP_AUTH", DataMetaCode = "", Path = "API/Authenticate", Title = "Authenticate with a product", Description = "Authenticates a user", ParentMetaCode = "ROOT" });

            #endregion


            //Insert models if not existing
            var iamclient = new Connection(Settings.Value.IAMConnectionDBMS, Settings.Value.IAMConnection);
            iamclient.Open();

            var client = new Connection(Settings.Value.DefaultConnectionDBMS, Settings.Value.DefaultConnection);
            client.Open();

            var current_permissions = iamclient.GetEntities<IntwentyProductAuthorizationItem>();

            var current_systems = client.GetEntities<SystemItem>();
            foreach (var t in systems)
            {
                if (!current_systems.Exists(p => p.MetaCode == t.MetaCode))
                    client.InsertEntity(t);

                if (!current_permissions.Exists(p => p.MetaCode == t.MetaCode && p.ProductId == Settings.Value.ProductId && p.AuthorizationType == SystemModelItem.MetaTypeSystem))
                {
                    var sysperm = new IntwentyProductAuthorizationItem() { Id = Guid.NewGuid().ToString(), Name = t.MetaCode, NormalizedName=t.MetaCode.ToUpper(), AuthorizationType = SystemModelItem.MetaTypeSystem, ProductId = Settings.Value.ProductId };
                    iamclient.InsertEntity(sysperm);
                }
            }

            var current_apps = client.GetEntities<ApplicationItem>();
            foreach (var t in applications)
            {
                if (!current_apps.Exists(p => p.MetaCode == t.MetaCode && p.SystemMetaCode == t.SystemMetaCode))
                    client.InsertEntity(t);

                if (!current_permissions.Exists(p => p.MetaCode == t.MetaCode && p.ProductId == Settings.Value.ProductId && p.AuthorizationType == ApplicationModelItem.MetaTypeApplication))
                {
                    var apperm = new IntwentyProductAuthorizationItem() { Id = Guid.NewGuid().ToString(), Name = t.MetaCode, NormalizedName = t.MetaCode.ToUpper(), AuthorizationType = ApplicationModelItem.MetaTypeApplication, ProductId = Settings.Value.ProductId };
                    iamclient.InsertEntity(apperm);
                }
            }

            var current_domains = client.GetEntities<ValueDomainItem>();
            foreach (var t in valuedomains)
            {
                if (!current_domains.Exists(p => p.DomainName ==  t.DomainName))
                    client.InsertEntity(t);
            }

            var current_ui = client.GetEntities<UserInterfaceItem>();
            foreach (var t in userinterface)
            {
                if (!current_ui.Exists(p => p.MetaCode == t.MetaCode && p.AppMetaCode == t.AppMetaCode))
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

            iamclient.Close();
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
