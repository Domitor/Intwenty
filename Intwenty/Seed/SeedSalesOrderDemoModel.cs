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

namespace Intwenty.Seed
{
    public static class SeedSalesOrderDemoModel
    {
     

        public static void Seed(IServiceProvider services)
        {
            var Settings = services.GetRequiredService<IOptions<IntwentySettings>>();

            if (!Settings.Value.SeedDatabaseOnStartUp ||
                !Settings.Value.ReCreateDatabaseOnStartup)
                return;

            var client = new Connection(Settings.Value.DefaultConnectionDBMS, Settings.Value.DefaultConnection);
            client.Open();

            client.DeleteEntities(client.GetEntities<ApplicationItem>());
            client.DeleteEntities(client.GetEntities<DatabaseItem>());
            client.DeleteEntities(client.GetEntities<DataViewItem>());
            client.DeleteEntities(client.GetEntities<MenuItem>());
            client.DeleteEntities(client.GetEntities<UserInterfaceItem>());
            client.DeleteEntities(client.GetEntities<EndpointItem>());


            //APPLICATIONS
            client.InsertEntity(new ApplicationItem() { Id = 10, Description = "An app for managing customers", MetaCode = "CUSTOMER", Title = "Customer", TitleLocalizationKey="CUSTOMER", DbName = "Customer", IsHierarchicalApplication = false, UseVersioning = false });
            client.InsertEntity(new ApplicationItem() { Id = 20, Description = "An app for managing items", MetaCode = "ITEM", Title = "Item", TitleLocalizationKey = "ITEM", DbName = "Item", IsHierarchicalApplication = false, UseVersioning = false });
            client.InsertEntity(new ApplicationItem() { Id = 30, Description = "An app for managing sales orders", MetaCode = "SALESORDER", Title = "Sales Order", TitleLocalizationKey = "SALESORDER", DbName = "SalesHeader", IsHierarchicalApplication = false, UseVersioning = false });
            client.InsertEntity(new ApplicationItem() { Id = 40, Description = "An app for managing vendors", MetaCode = "VENDOR", Title = "Vendor", TitleLocalizationKey = "VENDOR", DbName = "Vendor", IsHierarchicalApplication = false, UseVersioning = false });
            client.InsertEntity(new ApplicationItem() { Id = 50, Description = "An app for blogging", MetaCode = "BLOGAPP", Title = "The blog", TitleLocalizationKey = "", DbName = "Blog", IsHierarchicalApplication = false, UseVersioning = false });

            //MENU
            client.InsertEntity(new MenuItem() { AppMetaCode = "", MetaType = "MAINMENU", MetaCode = "SYSMENU", ParentMetaCode = "ROOT", Title = "Menu", TitleLocalizationKey = "MENU", OrderNo = 1, Action = "", Controller = "" });
            client.InsertEntity(new MenuItem() { AppMetaCode = "VENDOR", MetaType = "MENUITEM", MetaCode = "M_VEND", ParentMetaCode = "SYSMENU", Title = "Vendor", TitleLocalizationKey = "VENDOR", OrderNo = 40, Action = "", Controller = "" });
            client.InsertEntity(new MenuItem() { AppMetaCode = "CUSTOMER", MetaType = "MENUITEM", MetaCode = "M_CUST", ParentMetaCode = "SYSMENU", Title = "Customer", TitleLocalizationKey = "CUSTOMER", OrderNo = 10, Action = "", Controller = "" });
            client.InsertEntity(new MenuItem() { AppMetaCode = "ITEM", MetaType = "MENUITEM", MetaCode = "M_ITEM", ParentMetaCode = "SYSMENU", Title = "Item", TitleLocalizationKey = "ITEM", OrderNo = 20, Action = "", Controller = "" });
            client.InsertEntity(new MenuItem() { AppMetaCode = "SALESORDER", MetaType = "MENUITEM", MetaCode = "M_SORD", ParentMetaCode = "SYSMENU", Title = "Sales Order", TitleLocalizationKey = "SALESORDER", OrderNo = 30, Action = "", Controller = "" });
            client.InsertEntity(new MenuItem() { AppMetaCode = "BLOGAPP", MetaType = "MENUITEM", MetaCode = "M_BLOG", ParentMetaCode = "SYSMENU", Title = "Example blog", TitleLocalizationKey = "", OrderNo = 60, Action = "", Controller = "" });

            //VALUEDOMAIN (USED IN COMBOBOXES ETC)
            client.DeleteEntities(client.GetEntities<ValueDomainItem>().Where(p=> p.DomainName == "ITEMCATEGORY"));
            client.InsertEntity(new ValueDomainItem() { DomainName = "ITEMCATEGORY", Code = "PROD", Value = "Products" });
            client.InsertEntity(new ValueDomainItem() { DomainName = "ITEMCATEGORY", Code = "SERV", Value = "Services" });


            #region customer
            //APPLICATION CUSTOMER
            //--------------------
            //DATABASE - MAINTABLE
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMERID", DbName = "CustomerId", ParentMetaCode = "ROOT", DataType = "STRING", Properties= "DEFVALUE=AUTO#DEFVALUE_START=1000#DEFVALUE_PREFIX=CUST#DEFVALUE_SEED=100#UNIQUE=TRUE#MANDATORY=TRUE" });
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMERNAME", DbName = "CustomerName", ParentMetaCode = "ROOT", DataType = "STRING" });
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMERPHONE", DbName = "CustomerPhone", ParentMetaCode = "ROOT", DataType = "STRING" });
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMEREMAIL", DbName = "CustomerEmail", ParentMetaCode = "ROOT", DataType = "STRING" });
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMERSTATUS", DbName = "CustomerStatus", ParentMetaCode = "ROOT", DataType = "STRING" });


            //UI
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "CREATEVIEW", MetaCode = "ADDEDITVIEW", Title = "Create Customer", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "" });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "SECTION", MetaCode = "MAINSECTION", Title = "", ParentMetaCode = "ADDEDITVIEW", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "PANEL", MetaCode = "CUSTPNL1", Title = "Basics", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "TEXTBOX", MetaCode = "TB_CUSTID", DataColumn1MetaCode = "CUSTOMERID", Title = "Customer ID", TitleLocalizationKey = "CUSTOMERID", ParentMetaCode = "CUSTPNL1", RowOrder = 1, ColumnOrder = 1, Properties="READONLY=TRUE" });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "TEXTBOX", MetaCode = "TB_CUSTNAME", DataColumn1MetaCode = "CUSTOMERNAME", Title = "Customer Name", TitleLocalizationKey = "CUSTOMERNAME", ParentMetaCode = "CUSTPNL1", RowOrder = 2, ColumnOrder = 1 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "PANEL", MetaCode = "CUSTPNL2",  Title = "Contact", TitleLocalizationKey = "CUSTOMERCONTACT", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 2 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "EMAILBOX", MetaCode = "TBCUSTMAIL", DataColumn1MetaCode = "CUSTOMEREMAIL", Title = "Email", TitleLocalizationKey = "CUSTOMEREMAIL", ParentMetaCode = "CUSTPNL2", RowOrder = 1, ColumnOrder = 2 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "NUMBOX", MetaCode = "TBCUSTPHONE", DataColumn1MetaCode = "CUSTOMERPHONE", Title = "Phone", TitleLocalizationKey = "CUSTOMERPHONE", ParentMetaCode = "CUSTPNL2", RowOrder = 2, ColumnOrder = 2 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "TEXTBOX", MetaCode = "TBCUSTSTAT", DataColumn1MetaCode = "CUSTOMERSTATUS", Title = "Status", TitleLocalizationKey = "", ParentMetaCode = "CUSTPNL2", RowOrder = 3, ColumnOrder = 2, Properties="READONLY=TRUE" });

            //EDIT LISTVIEW
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "EDITLISTVIEW", MetaCode = "MAIN_EDITLISTVIEW", Title = "Customer List", TitleLocalizationKey = "CUSTOMERLIST", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "EDITLISTVIEWCOLUMN", MetaCode = "LV_ID", DataColumn1MetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 1 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "EDITLISTVIEWCOLUMN", MetaCode = "LV_CUSTID", DataColumn1MetaCode = "CUSTOMERID", Title = "Customer ID", TitleLocalizationKey = "CUSTOMERID", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 2 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "EDITLISTVIEWCOLUMN", MetaCode = "LV_CUSTNAME", DataColumn1MetaCode = "CUSTOMERNAME", Title = "Customer Name", TitleLocalizationKey = "CUSTOMERNAME", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 3 });
            #endregion

            #region item
            //APPLICATION ITEM
            //---------------------
            //DATABASE - MAINTABLE
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "ITEMID", DbName = "ItemId", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "DEFVALUE=AUTO#DEFVALUE_START=1000#DEFVALUE_PREFIX=ITEM#DEFVALUE_SEED=100#UNIQUE=TRUE#MANDATORY=TRUE" });
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "NAME", DbName = "ItemName", ParentMetaCode = "ROOT", DataType = "STRING" });
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "CATCODE", DbName = "ItemCategoryCode", ParentMetaCode = "ROOT", DataType = "STRING" });
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "CATVALUE", DbName = "ItemCategoryValue", ParentMetaCode = "ROOT", DataType = "STRING" });
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "MODIFIED", DbName = "Modified", ParentMetaCode = "ROOT", DataType = "DATETIME" });
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "ACTIVE", DbName = "Active", ParentMetaCode = "ROOT", DataType = "BOOLEAN" });
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "PURCHASEPRICE", DbName = "PurchasePrice", ParentMetaCode = "ROOT", DataType = "2DECIMAL" });
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "SALESPRICE", DbName = "SalesPrice", ParentMetaCode = "ROOT", DataType = "2DECIMAL" });
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "VENDORCODE", DbName = "VendorCode", ParentMetaCode = "ROOT", DataType = "STRING" });
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "VENDORTXT", DbName = "VendorName", ParentMetaCode = "ROOT", DataType = "STRING" });

            //UI
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "CREATEVIEW", MetaCode = "ADDEDITVIEW", Title = "Add a new Item", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "" });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "SECTION", MetaCode = "MAINSECTION", Title = "Basics", ParentMetaCode = "ADDEDITVIEW", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "PANEL", MetaCode = "ITMPNL_A", Title = "Basics", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "TEXTBOX", MetaCode = "TB_ITID", DataColumn1MetaCode = "ITEMID", Title = "Item ID", ParentMetaCode = "ITMPNL_A", RowOrder = 1, ColumnOrder = 1, Properties = "READONLY=TRUE" });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "TEXTBOX", MetaCode = "TB_ITNAME", DataColumn1MetaCode = "NAME", Title = "Item Name", ParentMetaCode = "ITMPNL_A", RowOrder = 2, ColumnOrder = 1 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "DATEPICKER", MetaCode = "DP_MOD", DataColumn1MetaCode = "MODIFIED", Title = "Modified Date", ParentMetaCode = "ITMPNL_A", RowOrder = 3, ColumnOrder = 1 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "CHECKBOX", MetaCode = "CB_ACTIVE", DataColumn1MetaCode = "ACTIVE", Title = "Is Active", ParentMetaCode = "ITMPNL_A", RowOrder = 4, ColumnOrder = 1 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "PANEL", MetaCode = "ITMPNL_B", Title = "Basics", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 2 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "COMBOBOX", MetaCode = "CB_CATEGORY", DataColumn1MetaCode = "CATCODE", Title = "Category", ParentMetaCode = "ITMPNL_B", RowOrder = 1, ColumnOrder = 2, Domain = "VALUEDOMAIN.ITEMCATEGORY" });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "NUMBOX", MetaCode = "NUMBOX_PURCHPRICE", DataColumn1MetaCode = "PURCHASEPRICE", Title = "Purchase Price", ParentMetaCode = "ITMPNL_B", RowOrder = 2, ColumnOrder = 2 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "NUMBOX", MetaCode = "NUMBOX_SALESPRICE", DataColumn1MetaCode = "SALESPRICE", Title = "Sales Pricee", ParentMetaCode = "ITMPNL_B", RowOrder = 3, ColumnOrder = 2 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "LOOKUP", MetaCode = "LOOKUPVEND", DataColumn1MetaCode = "VENDORCODE", DataColumn2MetaCode = "VENDORTXT", DataViewMetaCode = "VENDORVIEW", DataViewColumn1MetaCode = "VF_VENDID", DataViewColumn2MetaCode = "VF_VENDNAME", Title = "Vendor", ParentMetaCode = "ITMPNL_B", RowOrder = 4, ColumnOrder = 2 });

            //EDITLISTVIEW
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "EDITLISTVIEW", MetaCode = "MAIN_EDITLISTVIEW", Title = "Item List", TitleLocalizationKey = "ITEMLIST", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "EDITLISTVIEWCOLUMN", MetaCode = "LV_ID", DataColumn1MetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 1 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "EDITLISTVIEWCOLUMN", MetaCode = "LV_ITEMID", DataColumn1MetaCode = "ITEMID", Title = "Item ID", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 2 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "EDITLISTVIEWCOLUMN", MetaCode = "LV_ITEMNAME", DataColumn1MetaCode = "NAME", Title = "Item Name", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 3 });
            #endregion

            #region sales order
            //APPLICATION SALESORDER
            //--------------------
            //DATABASE - MAINTABLE
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "ORDERNO", DbName = "OrderNo", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "DEFVALUE=AUTO#DEFVALUE_START=1000#DEFVALUE_PREFIX=SO#DEFVALUE_SEED=100#UNIQUE=TRUE#MANDATORY=TRUE" });
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "ORDERDATE", DbName = "OrderDate", ParentMetaCode = "ROOT", DataType = "DATETIME" });
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "CUSTID", DbName = "CustomerId", ParentMetaCode = "ROOT", DataType = "STRING" });
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "CUSTNAME", DbName = "CustomerName", ParentMetaCode = "ROOT", DataType = "STRING" });
            //DATABASE - SUBTABLE (SALESLINES)
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "SALESORDER", MetaType = "DATATABLE", MetaCode = "DTORDLINE", DbName = "SalesLine", ParentMetaCode = "ROOT", DataType = "" });
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "ITEMID", DbName = "ItemNo", ParentMetaCode = "DTORDLINE", DataType = "STRING" });
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "ITEMNAME", DbName = "ItemName", ParentMetaCode = "DTORDLINE", DataType = "STRING" });
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "QTY", DbName = "Qty", ParentMetaCode = "DTORDLINE", DataType = "INTEGER" });

            //EDITLISTVIEW
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "EDITLISTVIEW", MetaCode = "MAIN_EDITLISTVIEW", Title = "Sales Orders", TitleLocalizationKey = "SALESORDERLIST", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0, Properties = "" });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "EDITLISTVIEWCOLUMN", MetaCode = "LV_ID", DataColumn1MetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 1 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "EDITLISTVIEWCOLUMN", MetaCode = "LF_ORDERID", DataColumn1MetaCode = "ORDERNO", Title = "Order No", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 2 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "EDITLISTVIEWCOLUMN", MetaCode = "LF_CUSTNAME", DataColumn1MetaCode = "CUSTNAME", Title = "Customer", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 3 });

            //UI
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "CREATEVIEW", MetaCode = "ADDEDITVIEW", Title = "Create a new sales order", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "" });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "SECTION", MetaCode = "SECT_HDR", Title = "Sales Header", ParentMetaCode = "ADDEDITVIEW", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "PANEL", MetaCode = "PANEL1", Title = "", ParentMetaCode = "SECT_HDR", RowOrder = 1, ColumnOrder = 1 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "TEXTBOX", MetaCode = "TB_ORDERNO", DataColumn1MetaCode = "ORDERNO", Title = "Order No", ParentMetaCode = "PANEL1", RowOrder = 1, ColumnOrder = 1, Properties = "READONLY=TRUE" });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "DATEPICKER", MetaCode = "DP_ORDERDATE", DataColumn1MetaCode = "ORDERDATE", Title = "Order Date", ParentMetaCode = "PANEL1", RowOrder = 2, ColumnOrder = 1 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "LOOKUP", MetaCode = "LOOKUP_CUSTOMER", DataColumn1MetaCode = "CUSTID", DataColumn2MetaCode = "CUSTNAME", DataViewMetaCode = "CUSTOMERVIEW", DataViewColumn1MetaCode = "VCUSTID", DataViewColumn2MetaCode = "VCUSTNAME", Title = "Customer", ParentMetaCode = "PANEL1", RowOrder = 3, ColumnOrder = 1 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "SECTION", MetaCode = "SECT_LINES", Title = "Sales Lines", ParentMetaCode = "ADDEDITVIEW", RowOrder = 2, ColumnOrder = 1, Properties = "COLLAPSIBLE=TRUE#STARTEXPANDED=TRUE" });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "PANEL", MetaCode = "PANEL2", Title = "", ParentMetaCode = "SECT_LINES", RowOrder = 1, ColumnOrder = 1 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "EDITGRID", MetaCode = "LINETABLE", DataTableMetaCode = "DTORDLINE", Title = "Sales Lines", ParentMetaCode = "PANEL2", RowOrder = 4, ColumnOrder = 1 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "EDITGRID_LOOKUP", MetaCode = "LINE_ITEMID", DataColumn1MetaCode = "ITEMID", DataViewMetaCode = "ITEMLOOKUP", DataViewColumn1MetaCode = "ITEMID", Title = "Item", ParentMetaCode = "LINETABLE", RowOrder = 1, ColumnOrder = 1 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "EDITGRID_NUMBOX", MetaCode = "LINE_QTY", DataColumn1MetaCode = "QTY", Title = "Qty", ParentMetaCode = "LINETABLE", RowOrder = 1, ColumnOrder = 2 });
            #endregion

            #region Vendor

            //DATABASE - MAINTABLE
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "VENDOR", MetaType = "DATACOLUMN", MetaCode = "VENDORID", DbName = "VendorId", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "DEFVALUE=AUTO#DEFVALUE_START=1000#DEFVALUE_PREFIX=VEND#DEFVALUE_SEED=100#UNIQUE=TRUE#MANDATORY=TRUE" });
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "VENDOR", MetaType = "DATACOLUMN", MetaCode = "VENDORNAME", DbName = "VendorName", ParentMetaCode = "ROOT", DataType = "STRING" });


            //UI
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "VENDOR", MetaType = "CREATEVIEW", MetaCode = "ADDEDITVIEW", Title = "Add a vendor", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "" });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "VENDOR", MetaType = "SECTION", MetaCode = "MAINSECTION", Title = "Sales Header", ParentMetaCode = "ADDEDITVIEW", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "VENDOR", MetaType = "PANEL", MetaCode = "PNL1",  Title = "Basics", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "VENDOR", MetaType = "TEXTBOX", MetaCode = "TB_VENDID", DataColumn1MetaCode = "VENDORID", Title = "Vendor ID", ParentMetaCode = "PNL1", RowOrder = 1, ColumnOrder = 1, Properties = "READONLY=TRUE" });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "VENDOR", MetaType = "TEXTBOX", MetaCode = "TB_VENDNAME", DataColumn1MetaCode = "VENDORNAME", Title = "Vendor Name", ParentMetaCode = "PNL1", RowOrder = 2, ColumnOrder = 1 });
            
            //EDITLISTVIEW
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "VENDOR", MetaType = "EDITLISTVIEW", MetaCode = "MAIN_EDITLISTVIEW", Title = "Vendor List", TitleLocalizationKey = "VENDORLIST", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "VENDOR", MetaType = "EDITLISTVIEWCOLUMN", MetaCode = "LV_ID", DataColumn1MetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 1 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "VENDOR", MetaType = "EDITLISTVIEWCOLUMN", MetaCode = "LV_VENDID", DataColumn1MetaCode = "VENDORID", Title = "Vendor ID", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 2 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "VENDOR", MetaType = "EDITLISTVIEWCOLUMN", MetaCode = "LV_VENDNAME", DataColumn1MetaCode = "VENDORNAME", Title = "Vendor Name", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 3 });

            #endregion

            #region Blog

            //DATABASE - MAINTABLE
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "BLOGAPP", MetaType = "DATACOLUMN", MetaCode = "POSTHEADER", DbName = "ArticleHeader", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "" });
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "BLOGAPP", MetaType = "DATACOLUMN", MetaCode = "POSTTEXT", DbName = "ArticleText", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "" });
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "BLOGAPP", MetaType = "DATACOLUMN", MetaCode = "POSTIMAGE", DbName = "ArticleImage", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "" });
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "BLOGAPP", MetaType = "DATATABLE", MetaCode = "POSTCOMMENTS", DbName = "ArticleComments", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "" });
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "BLOGAPP", MetaType = "DATACOLUMN", MetaCode = "COMMENTHEADER", DbName = "CommentLabel", ParentMetaCode = "POSTCOMMENTS", DataType = "STRING", Properties = "" });
            client.InsertEntity(new DatabaseItem() { AppMetaCode = "BLOGAPP", MetaType = "DATACOLUMN", MetaCode = "COMMENTTEXT", DbName = "CommentText", ParentMetaCode = "POSTCOMMENTS", DataType = "STRING", Properties = "" });


            //UI
            /*
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "VENDOR", MetaType = "CREATEVIEW", MetaCode = "ADDEDITVIEW", DataMetaCode = "", Title = "", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "" });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "VENDOR", MetaType = "SECTION", MetaCode = "MAINSECTION", DataMetaCode = "", Title = "Sales Header", ParentMetaCode = "ADDEDITVIEW", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "VENDOR", MetaType = "PANEL", MetaCode = "PNL1", DataMetaCode = "", Title = "Basics", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "VENDOR", MetaType = "TEXTBOX", MetaCode = "TB_VENDID", DataMetaCode = "VENDORID", Title = "Vendor ID", ParentMetaCode = "PNL1", RowOrder = 1, ColumnOrder = 1, Properties = "READONLY=TRUE" });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "VENDOR", MetaType = "TEXTBOX", MetaCode = "TB_VENDNAME", DataMetaCode = "VENDORNAME", Title = "Vendor Name", ParentMetaCode = "PNL1", RowOrder = 2, ColumnOrder = 1 });
            */
            //EDITLISTVIEW
            /*
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "VENDOR", MetaType = "EDITLISTVIEW", MetaCode = "MAIN_EDITLISTVIEW", DataMetaCode = "", Title = "Vendor List", TitleLocalizationKey = "VENDORLIST", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "VENDOR", MetaType = "EDITLISTVIEWCOLUMN", MetaCode = "LV_ID", DataMetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 1 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "VENDOR", MetaType = "EDITLISTVIEWCOLUMN", MetaCode = "LV_VENDID", DataMetaCode = "VENDORID", Title = "Vendor ID", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 2 });
            client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "VENDOR", MetaType = "EDITLISTVIEWCOLUMN", MetaCode = "LV_VENDNAME", DataMetaCode = "VENDORNAME", Title = "Vendor Name", ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 3 });
            */
            #endregion

            #region dataviews
            //DATAVIEWS
            //---------
            //CUSTOMER
            client.InsertEntity(new DataViewItem() { MetaType = "DATAVIEW", MetaCode = "CUSTOMERVIEW", ParentMetaCode = "ROOT", SQLQuery = "select CustomerId, CustomerName from Customer order by CustomerId asc", Title = "Customers", TitleLocalizationKey = "CUSTOMERS", SQLQueryFieldName = "" });
            client.InsertEntity(new DataViewItem() { MetaType = "DATAVIEWKEYCOLUMN", MetaCode = "VCUSTID", ParentMetaCode = "CUSTOMERVIEW", SQLQuery = "", Title = "Id", TitleLocalizationKey = "", SQLQueryFieldName = "CustomerId", SQLQueryFieldDataType = "STRING" });
            client.InsertEntity(new DataViewItem() { MetaType = "DATAVIEWCOLUMN", MetaCode = "VCUSTNAME", ParentMetaCode = "CUSTOMERVIEW", SQLQuery = "", Title = "Name", TitleLocalizationKey = "NAME", SQLQueryFieldName = "CustomerName", SQLQueryFieldDataType = "STRING" });


            //VENDOR
            client.InsertEntity(new DataViewItem() { MetaType = "DATAVIEW", MetaCode = "VENDORVIEW", ParentMetaCode = "ROOT", SQLQuery = "select VendorId, VendorName from Vendor order by VendorId asc", Title = "Vendors", SQLQueryFieldName = "" });
            client.InsertEntity(new DataViewItem() { MetaType = "DATAVIEWKEYCOLUMN", MetaCode = "VF_VENDID", ParentMetaCode = "VENDORVIEW", SQLQuery = "", Title = "Id", SQLQueryFieldName = "VendorId", SQLQueryFieldDataType = "STRING" });
            client.InsertEntity(new DataViewItem() { MetaType = "DATAVIEWCOLUMN", MetaCode = "VF_VENDNAME", ParentMetaCode = "VENDORVIEW", SQLQuery = "", Title = "Name", SQLQueryFieldName = "VendorName", SQLQueryFieldDataType = "STRING" });

            //ITEM
            client.InsertEntity(new DataViewItem() { MetaType = "DATAVIEW", MetaCode = "ITEMLOOKUP", ParentMetaCode = "ROOT", SQLQuery = "select ItemId, ItemName  from Item order by ItemId asc", Title = "Items", SQLQueryFieldName = "" });
            client.InsertEntity(new DataViewItem() { MetaType = "DATAVIEWKEYCOLUMN", MetaCode = "ITEMID", ParentMetaCode = "ITEMLOOKUP", SQLQuery = "", Title = "Id", SQLQueryFieldName = "ItemId", SQLQueryFieldDataType = "STRING" });
            client.InsertEntity(new DataViewItem() { MetaType = "DATAVIEWCOLUMN", MetaCode = "ITEMNAME", ParentMetaCode = "ITEMLOOKUP", SQLQuery = "", Title = "Name", SQLQueryFieldName = "ItemName", SQLQueryFieldDataType = "STRING" });

            #endregion

            #region translation

            var temp = new List<TranslationItem>();

             temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "CUSTOMER", Text = "Customer" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "CUSTOMER", Text = "Kund" });
             temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "CUSTOMERS", Text = "Customers" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "CUSTOMERS", Text = "Kunder" });
             temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "CUSTOMERLIST", Text = "Customer list" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "CUSTOMERLIST", Text = "Kunder" });
             temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "CUSTOMERID", Text = "Customer ID" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "CUSTOMERID", Text = "Kund ID" });
             temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "CUSTOMERNAME", Text = "Customer Name" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "CUSTOMERNAME", Text = "Kund namn" });
             temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "CUSTOMERCONTACT", Text = "Contact" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "CUSTOMERCONTACT", Text = "Kontakt" });
             temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "CUSTOMERPHONE", Text = "Phone" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "CUSTOMERPHONE", Text = "Telefon" });
             temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "CUSTOMEREMAIL", Text = "E-Mail" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "CUSTOMEREMAIL", Text = "E-Post" });
             temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "NAME", Text = "Name" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "NAME", Text = "Namn" });
             temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "MENU", Text = "Menu" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "MENU", Text = "Meny" });
             temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "ITEM", Text = "Item" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "ITEM", Text = "Artikel" });
             temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "ITEMLIST", Text = "Item list" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "ITEMLIST", Text = "Artiklar" });
             temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "SALESORDER", Text = "Sales Order" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "SALESORDER", Text = "Säljorder" });
             temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "SALESORDERLIST", Text = "Sales Orders" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "SALESORDERLIST", Text = "Säljordrar" });
             temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "VENDOR", Text = "Vendor" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "VENDOR", Text = "Tillverkare" });
             temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "VENDORLIST", Text = "Vendors" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "VENDORLIST", Text = "Tillverkare" });

            var existing = client.GetEntities<TranslationItem>();
            foreach (var t in temp)
            {
                if (!existing.Exists(p => p.Culture == t.Culture && p.TransKey == t.TransKey))
                    client.InsertEntity(t);
            }

            #endregion

            #region endpoints

            //Define som endpoints for the customer application (Actions must be in the ENDPOINT_TABLE_ACTION domain)
            client.InsertEntity(new EndpointItem() { AppMetaCode = "CUSTOMER", MetaType = "TABLEGETBYID", MetaCode = "EP_CUSTOMER_GETLATEST", DataMetaCode = "CUSTOMER", Path = "MyAPI/Customer", Title = "Get customer", Description = "Get latest version of a customer by id", ParentMetaCode = "ROOT" });
            client.InsertEntity(new EndpointItem() { AppMetaCode = "CUSTOMER", MetaType = "TABLEGETALL", MetaCode = "EP_CUSTOMER_GETALL", DataMetaCode = "CUSTOMER", Path = "MyAPI/Customer", Title = "Get all customers", Description = "Get all customers in the database", ParentMetaCode = "ROOT" });
            client.InsertEntity(new EndpointItem() { AppMetaCode = "CUSTOMER", MetaType = "TABLESAVE", MetaCode = "EP_CUSTOMER_SAVE", DataMetaCode = "CUSTOMER", Path = "MyAPI/Customer", Title = "Save customer", Description = "Create or update customer", ParentMetaCode = "EP_CUSTOMER" });

            //Define som endpoints for dataviews (Actions must be in the ENDPOINT_DATAVIEW_ACTION domain)
            client.InsertEntity(new EndpointItem() { AppMetaCode = "", MetaType = "DATAVIEWGETDATA", MetaCode = "EP_DV_OP_1", DataMetaCode = "VENDORVIEW", Path = "MyAPI/Vendor", Title = "Get Vendors", Description = "Get vendors by using an intwenty dataview", ParentMetaCode = "ROOT" });

            #endregion

            client.Close();

        }

        public static void ConfigureDataBase(IServiceProvider services)
        {
            var modelservice = services.GetRequiredService<IIntwentyModelService>();
            modelservice.ConfigureDatabase();

        }

       
    }
}
