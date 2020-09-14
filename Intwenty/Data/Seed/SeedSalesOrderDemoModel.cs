using System;
using System.Linq;
using Intwenty.Data.DBAccess;
using Intwenty.Data.DBAccess.Helpers;
using Intwenty.Data.Entity;
using Intwenty.Areas.Identity.Models;
using Intwenty.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Intwenty.Data.Seed
{
    public static class SeedSalesOrderDemoModel
    {
     

        public static void Seed(IServiceProvider services)
        {
            var Settings = services.GetRequiredService<IOptions<IntwentySettings>>();

            if (!Settings.Value.SeedDatabaseOnStartUp ||
                !Settings.Value.ReCreateDatabaseOnStartup)
                return;

            IIntwentyDbORM DataRepository = null;
            if (Settings.Value.IsNoSQL)
                DataRepository = new IntwentyNoSqlDbClient(Settings.Value.DefaultConnectionDBMS, Settings.Value.DefaultConnection);
            else
                DataRepository = new IntwentySqlDbClient(Settings.Value.DefaultConnectionDBMS, Settings.Value.DefaultConnection);


        
            DataRepository.DeleteRange(DataRepository.GetAll<ApplicationItem>());
            DataRepository.DeleteRange(DataRepository.GetAll<DatabaseItem>());
            DataRepository.DeleteRange(DataRepository.GetAll<DataViewItem>());
            DataRepository.DeleteRange(DataRepository.GetAll<MenuItem>());
            DataRepository.DeleteRange(DataRepository.GetAll<UserInterfaceItem>());


            //APPLICATIONS
            DataRepository.Insert(new ApplicationItem() { Id = 10, Description = "An app for managing customers", MetaCode = "CUSTOMER", Title = "Customer", TitleLocalizationKey="CUSTOMER", DbName = "Customer", IsHierarchicalApplication = false, UseVersioning = false });
            DataRepository.Insert(new ApplicationItem() { Id = 20, Description = "An app for managing items", MetaCode = "ITEM", Title = "Item", TitleLocalizationKey = "ITEM", DbName = "Item", IsHierarchicalApplication = false, UseVersioning = false });
            DataRepository.Insert(new ApplicationItem() { Id = 30, Description = "An app for managing sales orders", MetaCode = "SALESORDER", Title = "Sales Order", TitleLocalizationKey = "SALESORDER", DbName = "SalesHeader", IsHierarchicalApplication = false, UseVersioning = false });
            DataRepository.Insert(new ApplicationItem() { Id = 40, Description = "An app for managing vendors", MetaCode = "VENDOR", Title = "Vendor", TitleLocalizationKey = "VENDOR", DbName = "Vendor", IsHierarchicalApplication = false, UseVersioning = false });


            //MENU
            DataRepository.Insert(new MenuItem() { AppMetaCode = "", MetaType = "MAINMENU", MetaCode = "SYSMENU", ParentMetaCode = "ROOT", Title = "Menu", TitleLocalizationKey = "MENU", OrderNo = 1, Action = "", Controller = "" });
            DataRepository.Insert(new MenuItem() { AppMetaCode = "VENDOR", MetaType = "MENUITEM", MetaCode = "M_VEND", ParentMetaCode = "SYSMENU", Title = "Vendor", TitleLocalizationKey = "VENDOR", OrderNo = 40, Action = "", Controller = "" });
            DataRepository.Insert(new MenuItem() { AppMetaCode = "CUSTOMER", MetaType = "MENUITEM", MetaCode = "M_CUST", ParentMetaCode = "SYSMENU", Title = "Customer", TitleLocalizationKey = "CUSTOMER", OrderNo = 10, Action = "", Controller = "" });
            DataRepository.Insert(new MenuItem() { AppMetaCode = "ITEM", MetaType = "MENUITEM", MetaCode = "M_ITEM", ParentMetaCode = "SYSMENU", Title = "Item", TitleLocalizationKey = "ITEM", OrderNo = 20, Action = "", Controller = "" });
            DataRepository.Insert(new MenuItem() { AppMetaCode = "SALESORDER", MetaType = "MENUITEM", MetaCode = "M_SORD", ParentMetaCode = "SYSMENU", Title = "Sales Order", TitleLocalizationKey = "SALESORDER", OrderNo = 30, Action = "", Controller = "" });
           

            //VALUEDOMAIN (USED IN COMBOBOXES ETC)
            DataRepository.DeleteRange(DataRepository.GetAll<ValueDomainItem>().Where(p=> p.DomainName == "ITEMCATEGORY"));
            DataRepository.Insert(new ValueDomainItem() { DomainName = "ITEMCATEGORY", Code = "PROD", Value = "Products" });
            DataRepository.Insert(new ValueDomainItem() { DomainName = "ITEMCATEGORY", Code = "SERV", Value = "Services" });


            #region customer
            //APPLICATION CUSTOMER
            //--------------------
            //DATABASE - MAINTABLE
            DataRepository.Insert(new DatabaseItem() { AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMERID", DbName = "CustomerId", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true, IsUnique = true, Properties= "DEFVALUE=AUTO#DEFVALUE_START=1000#DEFVALUE_PREFIX=CUST#DEFVALUE_SEED=100" });
            DataRepository.Insert(new DatabaseItem() { AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMERNAME", DbName = "CustomerName", ParentMetaCode = "ROOT", DataType = "STRING" });
            DataRepository.Insert(new DatabaseItem() { AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMERPHONE", DbName = "CustomerPhone", ParentMetaCode = "ROOT", DataType = "STRING" });
            DataRepository.Insert(new DatabaseItem() { AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMEREMAIL", DbName = "CustomerEmail", ParentMetaCode = "ROOT", DataType = "STRING" });


            //UI
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "SECTION", MetaCode = "MAINSECTION", DataMetaCode = "", Title = "", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "PANEL", MetaCode = "CUSTPNL1", DataMetaCode = "", Title = "Basics", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "TEXTBOX", MetaCode = "TB_CUSTID", DataMetaCode = "CUSTOMERID", Title = "Customer ID", TitleLocalizationKey = "CUSTOMERID", ParentMetaCode = "CUSTPNL1", RowOrder = 1, ColumnOrder = 1 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "TEXTBOX", MetaCode = "TB_CUSTNAME", DataMetaCode = "CUSTOMERNAME", Title = "Customer Name", TitleLocalizationKey = "CUSTOMERNAME", ParentMetaCode = "CUSTPNL1", RowOrder = 2, ColumnOrder = 1 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "PANEL", MetaCode = "CUSTPNL2", DataMetaCode = "", Title = "Contact", TitleLocalizationKey = "CUSTOMERCONTACT", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 2 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "EMAILBOX", MetaCode = "TBCUSTMAIL", DataMetaCode = "CUSTOMEREMAIL", Title = "Phone", TitleLocalizationKey = "CUSTOMERPHONE", ParentMetaCode = "CUSTPNL2", RowOrder = 3, ColumnOrder = 2 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "NUMBOX", MetaCode = "TBCUSTPHONE", DataMetaCode = "CUSTOMERPHONE", Title = "Email", TitleLocalizationKey = "CUSTOMEREMAIL", ParentMetaCode = "CUSTPNL2", RowOrder = 3, ColumnOrder = 2 });

            //LISTVIEW
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "LISTVIEW", MetaCode = "MAIN_LISTVIEW", DataMetaCode = "", Title = "Customer List", TitleLocalizationKey = "CUSTOMERLIST", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "LISTVIEWCOLUMN", MetaCode = "LV_ID", DataMetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 1 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "LISTVIEWCOLUMN", MetaCode = "LV_CUSTID", DataMetaCode = "CUSTOMERID", Title = "Customer ID", TitleLocalizationKey = "CUSTOMERID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 2 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "LISTVIEWCOLUMN", MetaCode = "LV_CUSTNAME", DataMetaCode = "CUSTOMERNAME", Title = "Customer Name", TitleLocalizationKey = "CUSTOMERNAME", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 3 });
            #endregion

            #region item
            //APPLICATION ITEM
            //---------------------
            //DATABASE - MAINTABLE
            DataRepository.Insert(new DatabaseItem() { AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "ITEMID", DbName = "ItemId", ParentMetaCode = "ROOT", DataType = "STRING", IsUnique = true, Properties = "DEFVALUE=AUTO#DEFVALUE_START=1000#DEFVALUE_PREFIX=ITEM#DEFVALUE_SEED=100" });
            DataRepository.Insert(new DatabaseItem() { AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "NAME", DbName = "ItemName", ParentMetaCode = "ROOT", DataType = "STRING" });
            DataRepository.Insert(new DatabaseItem() { AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "CATCODE", DbName = "ItemCategoryCode", ParentMetaCode = "ROOT", DataType = "STRING", Domain = "VALUEDOMAIN.ITEMCATEGORY.CODE" });
            DataRepository.Insert(new DatabaseItem() { AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "CATVALUE", DbName = "ItemCategoryValue", ParentMetaCode = "ROOT", DataType = "STRING", Domain = "VALUEDOMAIN.ITEMCATEGORY.VALUE" });
            DataRepository.Insert(new DatabaseItem() { AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "MODIFIED", DbName = "Modified", ParentMetaCode = "ROOT", DataType = "DATETIME", Properties = "DEFVALUE=AUTO" });
            DataRepository.Insert(new DatabaseItem() { AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "ACTIVE", DbName = "Active", ParentMetaCode = "ROOT", DataType = "BOOLEAN" });
            DataRepository.Insert(new DatabaseItem() { AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "PURCHASEPRICE", DbName = "PurchasePrice", ParentMetaCode = "ROOT", DataType = "2DECIMAL" });
            DataRepository.Insert(new DatabaseItem() { AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "SALESPRICE", DbName = "SalesPrice", ParentMetaCode = "ROOT", DataType = "2DECIMAL" });
            DataRepository.Insert(new DatabaseItem() { AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "VENDORCODE", DbName = "VendorCode", ParentMetaCode = "ROOT", DataType = "STRING", Domain = "APP.VENDOR.VENDORID" });
            DataRepository.Insert(new DatabaseItem() { AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "VENDORTXT", DbName = "VendorName", ParentMetaCode = "ROOT", DataType = "STRING", Domain = "APP.VENDOR.VENDORNAME" });

            //UI
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "SECTION", MetaCode = "MAINSECTION", DataMetaCode = "", Title = "Basics", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "PANEL", MetaCode = "ITMPNL_A", DataMetaCode = "", Title = "Basics", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "TEXTBOX", MetaCode = "TB_ITID", DataMetaCode = "ITEMID", Title = "Item ID", ParentMetaCode = "ITMPNL_A", RowOrder = 1, ColumnOrder = 1 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "TEXTBOX", MetaCode = "TB_ITNAME", DataMetaCode = "NAME", Title = "Item Name", ParentMetaCode = "ITMPNL_A", RowOrder = 2, ColumnOrder = 1 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "DATEPICKER", MetaCode = "DP_MOD", DataMetaCode = "MODIFIED", Title = "Modified Date", ParentMetaCode = "ITMPNL_A", RowOrder = 3, ColumnOrder = 1 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "CHECKBOX", MetaCode = "CB_ACTIVE", DataMetaCode = "ACTIVE", Title = "Is Active", ParentMetaCode = "ITMPNL_A", RowOrder = 4, ColumnOrder = 1 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "PANEL", MetaCode = "ITMPNL_B", DataMetaCode = "", Title = "Basics", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 2 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "COMBOBOX", MetaCode = "CB_CATEGORY", DataMetaCode = "CATCODE", Title = "Category", ParentMetaCode = "ITMPNL_B", RowOrder = 1, ColumnOrder = 1, Domain = "VALUEDOMAIN.ITEMCATEGORY" });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "NUMBOX", MetaCode = "NUMBOX_PURCHPRICE", DataMetaCode = "PURCHASEPRICE", Title = "Purchase Price", ParentMetaCode = "ITMPNL_B", RowOrder = 2, ColumnOrder = 2 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "NUMBOX", MetaCode = "NUMBOX_SALESPRICE", DataMetaCode = "SALESPRICE", Title = "Sales Pricee", ParentMetaCode = "ITMPNL_B", RowOrder = 3, ColumnOrder = 2 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "LOOKUP", MetaCode = "LOOKUPVEND", DataMetaCode = "VENDORCODE", DataMetaCode2= "VENDORTXT", ViewMetaCode= "VF_VENDID", ViewMetaCode2 = "VF_VENDNAME", Title = "Vendor", ParentMetaCode = "ITMPNL_B", RowOrder = 4, ColumnOrder = 2, Domain = "DATAVIEW.VENDORVIEW" });

            //LISTVIEW
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "LISTVIEW", MetaCode = "MAIN_LISTVIEW", DataMetaCode = "", Title = "Item List", TitleLocalizationKey = "ITEMLIST", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "LISTVIEWCOLUMN", MetaCode = "LV_ID", DataMetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 1 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "LISTVIEWCOLUMN", MetaCode = "LV_ITEMID", DataMetaCode = "ITEMID", Title = "Item ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 2 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "LISTVIEWCOLUMN", MetaCode = "LV_ITEMNAME", DataMetaCode = "NAME", Title = "Item Name", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 3 });
            #endregion

            #region sales order
            //APPLICATION SALESORDER
            //--------------------
            //DATABASE - MAINTABLE
            DataRepository.Insert(new DatabaseItem() { AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "ORDERNO", DbName = "OrderNo", ParentMetaCode = "ROOT", DataType = "STRING", IsUnique = true, Properties = "DEFVALUE=AUTO#DEFVALUE_START=1000#DEFVALUE_PREFIX=SO#DEFVALUE_SEED=100" });
            DataRepository.Insert(new DatabaseItem() { AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "ORDERDATE", DbName = "OrderDate", ParentMetaCode = "ROOT", DataType = "DATETIME" });
            DataRepository.Insert(new DatabaseItem() { AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "CUSTID", DbName = "CustomerId", ParentMetaCode = "ROOT", DataType = "STRING" });
            DataRepository.Insert(new DatabaseItem() { AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "CUSTNAME", DbName = "CustomerName", ParentMetaCode = "ROOT", DataType = "STRING" });
            //DATABASE - SUBTABLE (SALESLINES)
            DataRepository.Insert(new DatabaseItem() { AppMetaCode = "SALESORDER", MetaType = "DATATABLE", MetaCode = "DTORDLINE", DbName = "SalesLine", ParentMetaCode = "ROOT", DataType = "" });
            DataRepository.Insert(new DatabaseItem() { AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "ITEMID", DbName = "ItemNo", ParentMetaCode = "DTORDLINE", DataType = "STRING" });
            DataRepository.Insert(new DatabaseItem() { AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "ITEMNAME", DbName = "ItemName", ParentMetaCode = "DTORDLINE", DataType = "STRING" });
            DataRepository.Insert(new DatabaseItem() { AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "QTY", DbName = "Qty", ParentMetaCode = "DTORDLINE", DataType = "INTEGER" });

            //LISTVIEW
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "LISTVIEW", MetaCode = "MAIN_LISTVIEW", DataMetaCode = "", Title = "Sales Orders", TitleLocalizationKey = "SALESORDERLIST", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0, Properties = "" });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "LISTVIEWCOLUMN", MetaCode = "LV_ID", DataMetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 1 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "LISTVIEWCOLUMN", MetaCode = "LF_ORDERID", DataMetaCode = "ORDERNO", Title = "Order No", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 2 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "LISTVIEWCOLUMN", MetaCode = "LF_CUSTNAME", DataMetaCode = "CUSTNAME", Title = "Customer", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 3 });

            //UI
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "SECTION", MetaCode = "SECT_HDR", DataMetaCode = "", Title = "Sales Header", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "PANEL", MetaCode = "PANEL1", DataMetaCode = "", Title = "", ParentMetaCode = "SECT_HDR", RowOrder = 1, ColumnOrder = 1 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "TEXTBOX", MetaCode = "TB_ORDERNO", DataMetaCode = "ORDERNO", Title = "Order No", ParentMetaCode = "PANEL1", RowOrder = 1, ColumnOrder = 1 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "DATEPICKER", MetaCode = "DP_ORDERDATE", DataMetaCode = "ORDERDATE", Title = "Order Date", ParentMetaCode = "PANEL1", RowOrder = 2, ColumnOrder = 1 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "LOOKUP", MetaCode = "LOOKUP_CUSTOMER", DataMetaCode = "CUSTID", ViewMetaCode = "VCUSTID", DataMetaCode2 = "CUSTNAME", ViewMetaCode2 = "VCUSTNAME", Title = "Customer", ParentMetaCode = "PANEL1", RowOrder = 3, ColumnOrder = 1, Domain = "DATAVIEW.CUSTOMERVIEW" });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "SECTION", MetaCode = "SECT_LINES", DataMetaCode = "", Title = "Sales Lines", ParentMetaCode = "ROOT", RowOrder = 2, ColumnOrder = 1, Properties = "COLLAPSIBLE=TRUE#STARTEXPANDED=TRUE" });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "PANEL", MetaCode = "PANEL2", DataMetaCode = "", Title = "", ParentMetaCode = "SECT_LINES", RowOrder = 1, ColumnOrder = 1 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "EDITGRID", MetaCode = "LINETABLE", DataMetaCode = "DTORDLINE", Title = "Sales Lines", ParentMetaCode = "PANEL2", RowOrder = 4, ColumnOrder = 1 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "EDITGRID_LOOKUP", MetaCode = "LINE_ITEMID", DataMetaCode = "ITEMID", ViewMetaCode = "ITEMID", Title = "Item", ParentMetaCode = "LINETABLE", RowOrder = 1, ColumnOrder = 1, Domain = "DATAVIEW.ITEMLOOKUP" });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "EDITGRID_TEXTBOX", MetaCode = "LINE_ITEMNAME", DataMetaCode = "ITEMNAME", ViewMetaCode = "ITEMNAME", Title = "Item Name", ParentMetaCode = "LINETABLE", RowOrder = 1, ColumnOrder = 1, Domain = "DATAVIEW.ITEMLOOKUP" });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "EDITGRID_NUMBOX", MetaCode = "LINE_QTY", DataMetaCode = "QTY", Title = "Qty", ParentMetaCode = "LINETABLE", RowOrder = 1, ColumnOrder = 2 });
            #endregion

            #region Vendor

            //DATABASE - MAINTABLE
            DataRepository.Insert(new DatabaseItem() { AppMetaCode = "VENDOR", MetaType = "DATACOLUMN", MetaCode = "VENDORID", DbName = "VendorId", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true, IsUnique = true, Properties = "DEFVALUE=AUTO#DEFVALUE_START=1000#DEFVALUE_PREFIX=VEND#DEFVALUE_SEED=100" });
            DataRepository.Insert(new DatabaseItem() { AppMetaCode = "VENDOR", MetaType = "DATACOLUMN", MetaCode = "VENDORNAME", DbName = "VendorName", ParentMetaCode = "ROOT", DataType = "STRING" });


            //UI
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "VENDOR", MetaType = "SECTION", MetaCode = "MAINSECTION", DataMetaCode = "", Title = "Sales Header", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "VENDOR", MetaType = "PANEL", MetaCode = "PNL1", DataMetaCode = "", Title = "Basics", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "VENDOR", MetaType = "TEXTBOX", MetaCode = "TB_VENDID", DataMetaCode = "VENDORID", Title = "Vendor ID", ParentMetaCode = "PNL1", RowOrder = 1, ColumnOrder = 1 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "VENDOR", MetaType = "TEXTBOX", MetaCode = "TB_VENDNAME", DataMetaCode = "VENDORNAME", Title = "Vendor Name", ParentMetaCode = "PNL1", RowOrder = 2, ColumnOrder = 1 });
            
            //LISTVIEW
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "VENDOR", MetaType = "LISTVIEW", MetaCode = "MAIN_LISTVIEW", DataMetaCode = "", Title = "Vendor List", TitleLocalizationKey = "VENDORLIST", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "VENDOR", MetaType = "LISTVIEWCOLUMN", MetaCode = "LV_ID", DataMetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 1 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "VENDOR", MetaType = "LISTVIEWCOLUMN", MetaCode = "LV_VENDID", DataMetaCode = "VENDORID", Title = "Vendor ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 2 });
            DataRepository.Insert(new UserInterfaceItem() { AppMetaCode = "VENDOR", MetaType = "LISTVIEWCOLUMN", MetaCode = "LV_VENDNAME", DataMetaCode = "VENDORNAME", Title = "Vendor Name", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 3 });

            #endregion

            #region dataviews
            //DATAVIEWS
            //---------
            //CUSTOMER
            DataRepository.Insert(new DataViewItem() { MetaType = "DATAVIEW", MetaCode = "CUSTOMERVIEW", ParentMetaCode = "ROOT", SQLQuery = "select CustomerId, CustomerName from Customer order by CustomerId asc", Title = "Customers", TitleLocalizationKey = "CUSTOMERS", SQLQueryFieldName = "" });
            DataRepository.Insert(new DataViewItem() { MetaType = "DATAVIEWKEYCOLUMN", MetaCode = "VCUSTID", ParentMetaCode = "CUSTOMERVIEW", SQLQuery = "", Title = "Id", TitleLocalizationKey = "", SQLQueryFieldName = "CustomerId", SQLQueryFieldDataType = "STRING" });
            DataRepository.Insert(new DataViewItem() { MetaType = "DATAVIEWCOLUMN", MetaCode = "VCUSTNAME", ParentMetaCode = "CUSTOMERVIEW", SQLQuery = "", Title = "Name", TitleLocalizationKey = "NAME", SQLQueryFieldName = "CustomerName", SQLQueryFieldDataType = "STRING" });


            //VENDOR
            DataRepository.Insert(new DataViewItem() { MetaType = "DATAVIEW", MetaCode = "VENDORVIEW", ParentMetaCode = "ROOT", SQLQuery = "select VendorId, VendorName from Vendor order by VendorId asc", Title = "Vendors", SQLQueryFieldName = "" });
            DataRepository.Insert(new DataViewItem() { MetaType = "DATAVIEWKEYCOLUMN", MetaCode = "VF_VENDID", ParentMetaCode = "VENDORVIEW", SQLQuery = "", Title = "Id", SQLQueryFieldName = "VendorId", SQLQueryFieldDataType = "STRING" });
            DataRepository.Insert(new DataViewItem() { MetaType = "DATAVIEWCOLUMN", MetaCode = "VF_VENDNAME", ParentMetaCode = "VENDORVIEW", SQLQuery = "", Title = "Name", SQLQueryFieldName = "VendorName", SQLQueryFieldDataType = "STRING" });

            //ITEM
            DataRepository.Insert(new DataViewItem() { MetaType = "DATAVIEW", MetaCode = "ITEMLOOKUP", ParentMetaCode = "ROOT", SQLQuery = "select ItemId, ItemName  from Item order by ItemId asc", Title = "Items", SQLQueryFieldName = "" });
            DataRepository.Insert(new DataViewItem() { MetaType = "DATAVIEWKEYCOLUMN", MetaCode = "ITEMID", ParentMetaCode = "ITEMLOOKUP", SQLQuery = "", Title = "Id", SQLQueryFieldName = "ItemId", SQLQueryFieldDataType = "STRING" });
            DataRepository.Insert(new DataViewItem() { MetaType = "DATAVIEWCOLUMN", MetaCode = "ITEMNAME", ParentMetaCode = "ITEMLOOKUP", SQLQuery = "", Title = "Name", SQLQueryFieldName = "ItemName", SQLQueryFieldDataType = "STRING" });

            #endregion

            #region translation

            var temp = new List<TranslationItem>();

             temp.Add(new TranslationItem() { Culture = "en-US", Key = "CUSTOMER", Text = "Customer" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", Key = "CUSTOMER", Text = "Kund" });
             temp.Add(new TranslationItem() { Culture = "en-US", Key = "CUSTOMERLIST", Text = "Customer list" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", Key = "CUSTOMERLIST", Text = "Kunder" });
             temp.Add(new TranslationItem() { Culture = "en-US", Key = "CUSTOMERID", Text = "Customer ID" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", Key = "CUSTOMERID", Text = "Kund ID" });
             temp.Add(new TranslationItem() { Culture = "en-US", Key = "CUSTOMERNAME", Text = "Customer Name" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", Key = "CUSTOMERNAME", Text = "Kund namn" });
             temp.Add(new TranslationItem() { Culture = "en-US", Key = "CUSTOMERCONTACT", Text = "Contact" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", Key = "CUSTOMERCONTACT", Text = "Kontakt" });
             temp.Add(new TranslationItem() { Culture = "en-US", Key = "CUSTOMERPHONE", Text = "Phone" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", Key = "CUSTOMERPHONE", Text = "Telefon" });
             temp.Add(new TranslationItem() { Culture = "en-US", Key = "CUSTOMEREMAIL", Text = "E-Mail" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", Key = "CUSTOMEREMAIL", Text = "E-Post" });
             temp.Add(new TranslationItem() { Culture = "en-US", Key = "NAME", Text = "Name" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", Key = "NAME", Text = "Namn" });
             temp.Add(new TranslationItem() { Culture = "en-US", Key = "MENU", Text = "Menu" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", Key = "MENU", Text = "Meny" });
             temp.Add(new TranslationItem() { Culture = "en-US", Key = "ITEM", Text = "Item" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", Key = "ITEM", Text = "Artikel" });
             temp.Add(new TranslationItem() { Culture = "en-US", Key = "ITEMLIST", Text = "Item list" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", Key = "ITEMLIST", Text = "Artiklar" });
             temp.Add(new TranslationItem() { Culture = "en-US", Key = "SALESORDER", Text = "Sales Order" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", Key = "SALESORDER", Text = "Säljorder" });
             temp.Add(new TranslationItem() { Culture = "en-US", Key = "SALESORDERLIST", Text = "Sales Orders" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", Key = "SALESORDERLIST", Text = "Säljordrar" });
             temp.Add(new TranslationItem() { Culture = "en-US", Key = "VENDOR", Text = "Vendor" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", Key = "VENDOR", Text = "Tillverkare" });
             temp.Add(new TranslationItem() { Culture = "en-US", Key = "VENDORLIST", Text = "Vendors" });
             temp.Add(new TranslationItem() { Culture = "sv-SE", Key = "VENDORLIST", Text = "Tillverkare" });

            var existing = DataRepository.GetAll<TranslationItem>();
            foreach (var t in temp)
            {
                if (!existing.Exists(p => p.Culture == t.Culture && p.Key == t.Key))
                    DataRepository.Insert(t);
            }

            #endregion



        }

        public static void ConfigureDataBase(IServiceProvider services)
        {
            var modelservice = services.GetRequiredService<IIntwentyModelService>();
            modelservice.ConfigureDatabase();

        }

       
    }
}
