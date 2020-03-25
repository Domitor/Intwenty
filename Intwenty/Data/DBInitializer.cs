using System;
using Microsoft.Extensions.DependencyInjection;
using Intwenty.Data.Entity;

namespace Intwenty.Data
{
    public class DBInitializer
    {
        public static void Initialize(IServiceProvider provider)
        {
            var context = provider.GetRequiredService<ApplicationDbContext>();


            if (context.Database.EnsureCreated())
            {
                SeedApplicationDescriptions(context, false);
                SeedSystemMenus(context, false);
                SeedApplicationContent(context, false);
                SeedValueDomains(context, false);
                SeedNoSeries(context, false);
            }
            else
            {
                /*
                SeedApplicationDescriptions(context, true);
                SeedSystemMenus(context, true);
                SeedApplicationContent(context, true);
                SeedValueDomains(context, true);
                SeedNoSeries(context, true);
                */

            }

           
        }


        private static void SeedApplicationDescriptions(ApplicationDbContext context, bool isupdate)
        {

            if (isupdate)
            {
                context.Set<ApplicationItem>().RemoveRange(context.Set<ApplicationItem>());
                context.SaveChanges();
            }


            context.Set<ApplicationItem>().Add(new ApplicationItem() { Id = 10, Description = "An app for managing customers", MetaCode = "CUSTOMER", Title = "Customer", DbName = "Customer", IsHierarchicalApplication = false, UseVersioning=false, TestDataAmount = 0  });
            context.Set<ApplicationItem>().Add(new ApplicationItem() { Id = 20, Description = "An app for managing items", MetaCode = "ITEM", Title = "Item", DbName = "Item", IsHierarchicalApplication = false, UseVersioning = false, TestDataAmount = 5 });
            context.Set<ApplicationItem>().Add(new ApplicationItem() { Id = 30, Description = "An app for managing item ledger", MetaCode = "ITEMLEDGER", Title = "Inventory Transactions", DbName = "ItemLedger", IsHierarchicalApplication = false, UseVersioning = false });
            context.Set<ApplicationItem>().Add(new ApplicationItem() { Id = 50, Description = "An app for managing locations", MetaCode = "LOCATION", Title = "Location", DbName = "Location", IsHierarchicalApplication = false, UseVersioning = false, TestDataAmount = 3 });
            context.Set<ApplicationItem>().Add(new ApplicationItem() { Id = 60, Description = "An app for managing goods reciept", MetaCode = "GOODSRECEIPT", Title = "Goods Receipt", DbName = "GoodsReceipt", IsHierarchicalApplication = false, UseVersioning = false, TestDataAmount = 0 });
            context.Set<ApplicationItem>().Add(new ApplicationItem() { Id = 70, Description = "An app for managing shipment files", MetaCode = "SHIPMENTFILES", Title = "Shipment files", DbName = "Shipments", IsHierarchicalApplication = false, UseVersioning = false, TestDataAmount = 0 });
            context.Set<ApplicationItem>().Add(new ApplicationItem() { Id = 80, Description = "An app for managing goods issue", MetaCode = "GOODSISSUE", Title = "Goods Issue", DbName = "GoodsIssue", IsHierarchicalApplication = false, UseVersioning = false, TestDataAmount = 0 });
            context.Set<ApplicationItem>().Add(new ApplicationItem() { Id = 90, Description = "An app for managing goods transfer", MetaCode = "GOODSTRANSFER", Title = "Goods Transfer", DbName = "GoodsTransfer", IsHierarchicalApplication = false, UseVersioning = false });
            context.Set<ApplicationItem>().Add(new ApplicationItem() { Id = 100, Description = "An app for managing sales orders", MetaCode = "SALESORDER", Title = "Sales Order", DbName = "SalesHeader", IsHierarchicalApplication = false, UseVersioning = false, TestDataAmount = 0 });


            context.SaveChanges();
        }


        private static void SeedSystemMenus(ApplicationDbContext context, bool isupdate)
        {
            if (isupdate)
            {
                context.Set<MenuItem>().RemoveRange(context.Set<MenuItem>());
                context.SaveChanges();
            }


            context.Set<MenuItem>().Add(new MenuItem() { AppMetaCode = "", MetaType = "MAINMENU", MetaCode = "WMS_MAINMENU", ParentMetaCode = "ROOT", Title = "Menu", Order = 1, Action = "", Controller = ""  });
            context.Set<MenuItem>().Add(new MenuItem() { AppMetaCode = "ITEM", MetaType = "MENUITEM", MetaCode = "M_ITEM", ParentMetaCode = "WMS_MAINMENU", Title = "Item", Order = 10, Action = "", Controller = "" });
            context.Set<MenuItem>().Add(new MenuItem() { AppMetaCode = "LOCATION", MetaType = "MENUITEM", MetaCode = "M_LOC", ParentMetaCode = "WMS_MAINMENU", Title = "Location", Order = 30, Action = "", Controller = "" });
            context.Set<MenuItem>().Add(new MenuItem() { AppMetaCode = "SHIPMENTFILES", MetaType = "MENUITEM", MetaCode = "M_SF", ParentMetaCode = "WMS_MAINMENU", Title = "Shipment files", Order = 32, Action = "ShipmentFiles", Controller = "Custom" });
            context.Set<MenuItem>().Add(new MenuItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "MENUITEM", MetaCode = "M_GR", ParentMetaCode = "WMS_MAINMENU", Title = "Goods Receipt", Order = 40, Action = "", Controller = "" });
            context.Set<MenuItem>().Add(new MenuItem() { AppMetaCode = "GOODSISSUE", MetaType = "MENUITEM", MetaCode = "M_GI", ParentMetaCode = "WMS_MAINMENU", Title = "Goods Issue", Order = 50, Action = "", Controller = "" });
            context.Set<MenuItem>().Add(new MenuItem() { AppMetaCode = "GOODSTRANSFER", MetaType = "MENUITEM", MetaCode = "M_GT", ParentMetaCode = "WMS_MAINMENU", Title = "Goods Transfer", Order = 60, Action = "", Controller = "" });
            context.Set<MenuItem>().Add(new MenuItem() { AppMetaCode = "SALESORDER", MetaType = "MENUITEM", MetaCode = "M_SORD", ParentMetaCode = "WMS_MAINMENU", Title = "Sales Order", Order = 70, Action = "", Controller = "" });

            context.SaveChanges();
        }


        private static void SeedApplicationContent(ApplicationDbContext context, bool isupdate)
        {


            if (isupdate)
            {
                context.Set<DatabaseItem>().RemoveRange(context.Set<DatabaseItem>());
                context.Set<DataViewItem>().RemoveRange(context.Set<DataViewItem>());
                context.Set<UserInterfaceItem>().RemoveRange(context.Set<UserInterfaceItem>());
                context.SaveChanges();
            }

            //SALESORDER
            //--------------------------------------------------
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "ORDERNO", DbName = "OrderNo", ParentMetaCode = "ROOT", DataType = "STRING" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "ORDERDATE", DbName = "OrderDate", ParentMetaCode = "ROOT", DataType = "DATETIME" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "CUSTID", DbName = "CustomerId", ParentMetaCode = "ROOT", DataType = "STRING" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "CUSTNAME", DbName = "CustomerName", ParentMetaCode = "ROOT", DataType = "STRING" });

            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "SALESORDER", MetaType = "DATATABLE", MetaCode = "DTORDLINE", DbName = "SalesLine", ParentMetaCode = "ROOT", DataType = "" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "ITEMID", DbName = "ItemNo", ParentMetaCode = "DTORDLINE", DataType = "STRING" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "ITEMNAME", DbName = "ItemName", ParentMetaCode = "DTORDLINE", DataType = "STRING" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "QTY", DbName = "Qty", ParentMetaCode = "DTORDLINE", DataType = "INTEGER" });

            //LISTVIEW
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "LISTVIEW", MetaCode = "MAIN_LISTVIEW", DataMetaCode = "", Title = "Sales Orders", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0, Properties = "" });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "LISTVIEWFIELD", MetaCode = "LV_ID", DataMetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 1 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "LISTVIEWFIELD", MetaCode = "LF_ORDERID", DataMetaCode = "ORDERNO", Title = "Order No", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 2 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "LISTVIEWFIELD", MetaCode = "LF_CUSTNAME", DataMetaCode = "CUSTNAME", Title = "Customer", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 3 });

            //UI
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "SECTION", MetaCode = "SECT_HDR", DataMetaCode = "", Title = "Sales Header", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "PANEL", MetaCode = "PANEL1", DataMetaCode = "", Title = "", ParentMetaCode = "SECT_HDR", RowOrder = 1, ColumnOrder = 1 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "TEXTBOX", MetaCode = "TB_ORDERNO", DataMetaCode = "ORDERNO", Title = "Order No", ParentMetaCode = "PANEL1", RowOrder = 1, ColumnOrder = 1 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "DATEPICKER", MetaCode = "DP_ORDERDATE", DataMetaCode = "ORDERDATE", Title = "Order Date", ParentMetaCode = "PANEL1", RowOrder = 2, ColumnOrder = 1 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "LOOKUP", MetaCode = "LOOKUP_CUSTOMER", DataMetaCode = "CUSTID", ViewMetaCode="VCUSTID", DataMetaCode2 = "CUSTNAME", ViewMetaCode2 = "VCUSTNAME", Title = "Customer", ParentMetaCode = "PANEL1", RowOrder = 3, ColumnOrder = 1, Domain= "DATAVIEW.CUSTOMERVIEW" });

            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "SECTION", MetaCode = "SECT_LINES", DataMetaCode = "", Title = "Sales Lines", ParentMetaCode = "ROOT", RowOrder = 2, ColumnOrder = 1, Properties = "COLLAPSIBLE=TRUE#STARTEXPANDED=TRUE" });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "PANEL", MetaCode = "PANEL2", DataMetaCode = "", Title = "", ParentMetaCode = "SECT_LINES", RowOrder = 1, ColumnOrder = 1 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "EDITGRID", MetaCode = "LINETABLE", DataMetaCode = "DTORDLINE", Title = "Sales Lines", ParentMetaCode = "PANEL2", RowOrder = 4, ColumnOrder = 1 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "EDITGRID_LOOKUP", MetaCode = "LINE_ITEMID", DataMetaCode = "ITEMID", ViewMetaCode="ITEMID", Title = "Item", ParentMetaCode = "LINETABLE", RowOrder = 1, ColumnOrder = 1, Domain="DATAVIEW.ITEMLOOKUP" });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "EDITGRID_TEXTBOX", MetaCode = "LINE_ITEMNAME", DataMetaCode = "ITEMNAME", ViewMetaCode="ITEMNAME", Title = "Item Name", ParentMetaCode = "LINETABLE", RowOrder = 1, ColumnOrder = 1, Domain = "DATAVIEW.ITEMLOOKUP" });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "SALESORDER", MetaType = "EDITGRID_NUMBOX", MetaCode = "LINE_QTY", DataMetaCode = "QTY", Title = "Qty", ParentMetaCode = "LINETABLE", RowOrder = 1, ColumnOrder = 2 });


            //SHIPMENTFILES
            //----------------------------------------------------
            //DATA
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "SHIPMENTFILES", MetaType = "DATACOLUMN", MetaCode = "SHIPMENTID", DbName = "ShipmentId", ParentMetaCode = "ROOT", DataType = "STRING" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "SHIPMENTFILES", MetaType = "DATACOLUMN", MetaCode = "FILENAME", DbName = "FileName", ParentMetaCode = "ROOT", DataType = "STRING"});
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "SHIPMENTFILES", MetaType = "DATACOLUMN", MetaCode = "IMPORTDATE", DbName = "ImportDate", ParentMetaCode = "ROOT", DataType = "DATETIME" });

            //LISTVIEW
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "SHIPMENTFILES", MetaType = "LISTVIEW", MetaCode = "MAIN_LISTVIEW", DataMetaCode = "", Title = "Shipment files", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0, Properties = "HIDEFILTER=TRUE" });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "SHIPMENTFILES", MetaType = "LISTVIEWFIELD", MetaCode = "LV_ID", DataMetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 1 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "SHIPMENTFILES", MetaType = "LISTVIEWFIELD", MetaCode = "LF_SHIPMENTID", DataMetaCode = "SHIPMENTID", Title = "Shipment ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 2 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "SHIPMENTFILES", MetaType = "LISTVIEWFIELD", MetaCode = "LF_IMPORTDATE", DataMetaCode = "IMPORTDATE", Title = "Import Date", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 3 });


            //LOCATION
            //----------------------------------------------------
            //DATA
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "LOCATION", MetaType = "DATACOLUMN", MetaCode = "LOCATIONID", DbName = "LocationId", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "LOCATION", MetaType = "DATACOLUMN", MetaCode = "LOCATIONNAME", DbName = "LocationName", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "LOCATION", MetaType = "DATACOLUMN", MetaCode = "LOCATIONTYPE", DbName = "LocationType", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true, Domain = "VALUEDOMAIN.LOCATIONTYPE.CODE" });

            //VIEWS
            context.Set<DataViewItem>().Add(new DataViewItem() { MetaType = "DATAVIEW", MetaCode = "LOCATIONLOOKUP", ParentMetaCode = "ROOT", SQLQuery = "select LocationId, LocationName from Location order by LocationId asc", Title = "Locations", SQLQueryFieldName = "" });
            context.Set<DataViewItem>().Add(new DataViewItem() { MetaType = "DATAVIEWKEYCOLUMN", MetaCode = "LOCID", ParentMetaCode = "LOCATIONLOOKUP", SQLQuery = "", Title = "Id", SQLQueryFieldName = "LocationId" });
            context.Set<DataViewItem>().Add(new DataViewItem() { MetaType = "DATAVIEWCOLUMN", MetaCode = "LOCNAME", ParentMetaCode = "LOCATIONLOOKUP", SQLQuery = "", Title = "Name", SQLQueryFieldName = "LocationName" });


            //UI
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "LOCATION", MetaType = "SECTION", MetaCode = "MAINSECTION", DataMetaCode = "", Title = "Basics", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "LOCATION", MetaType = "PANEL", MetaCode = "PNL1", DataMetaCode = "", Title = "Basics", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "LOCATION", MetaType = "TEXTBOX", MetaCode = "TB_ID", DataMetaCode = "LOCATIONID", Title = "Location ID", ParentMetaCode = "PNL1", RowOrder = 1, ColumnOrder = 1 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "LOCATION", MetaType = "TEXTBOX", MetaCode = "TB_NAME", DataMetaCode = "LOCATIONNAME", Title = "Location Name", ParentMetaCode = "PNL1", RowOrder = 2, ColumnOrder = 1 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "LOCATION", MetaType = "COMBOBOX", MetaCode = "CB_TYPE", DataMetaCode = "LOCATIONTYPE", Title = "Location Type", ParentMetaCode = "PNL1", RowOrder = 3, ColumnOrder = 1, Domain = "VALUEDOMAIN.LOCATIONTYPE.CODE" });

            //LISTVIEW
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "LOCATION", MetaType = "LISTVIEW", MetaCode = "MAIN_LISTVIEW", DataMetaCode = "", Title = "Location List", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "LOCATION", MetaType = "LISTVIEWFIELD", MetaCode = "LV_ID", DataMetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 1 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "LOCATION", MetaType = "LISTVIEWFIELD", MetaCode = "LV_LID", DataMetaCode = "LOCATIONID", Title = "Location ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 2 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "LOCATION", MetaType = "LISTVIEWFIELD", MetaCode = "LV_LNAME", DataMetaCode = "LOCATIONNAME", Title = "Location Name", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 3 });


            //GOODSRECEIPT
            //----------------------------------------------------
            //DATA
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "DATACOLUMN", MetaCode = "RECEIPTDATE", DbName = "ReceiptDate", ParentMetaCode = "ROOT", DataType = "DATETIME", Mandatory = true });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "DATACOLUMN", MetaCode = "ITEMID", DbName = "ItemID", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true, Domain = "APP.ITEM.ITEMID" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "DATACOLUMN", MetaCode = "ITEMNAME", DbName = "ItemName", ParentMetaCode = "ROOT", DataType = "STRING" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "DATACOLUMN", MetaCode = "LOCATIONID", DbName = "LocationID", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true, Domain = "APP.LOCATION.LOCATIONID" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "DATACOLUMN", MetaCode = "LOCATIONNAME", DbName = "LocationName", ParentMetaCode = "ROOT", DataType = "STRING" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "DATACOLUMN", MetaCode = "QTY", DbName = "Qty", ParentMetaCode = "ROOT", DataType = "INTEGER", Mandatory = true });

            //UI
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "SECTION", MetaCode = "MAINSECTION", DataMetaCode = "", Title = "Basics", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "PANEL", MetaCode = "PNL1", DataMetaCode = "", Title = "Basics", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "DATEPICKER", MetaCode = "GR_DATE", DataMetaCode = "RECEIPTDATE", Title = "Receipt Date", ParentMetaCode = "PNL1", RowOrder = 1, ColumnOrder = 1 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "LOOKUP", MetaCode = "MLITEM", DataMetaCode = "ITEMID", ViewMetaCode= "ITEMID", DataMetaCode2= "ITEMNAME", ViewMetaCode2= "ITEMNAME", Title = "Item", ParentMetaCode = "PNL1", RowOrder = 2, ColumnOrder = 1, Domain = "DATAVIEW.ITEMLOOKUP" });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "LOOKUP", MetaCode = "MLLOCATION", DataMetaCode = "LOCATIONID", ViewMetaCode= "LOCID",DataMetaCode2= "LOCATIONNAME", ViewMetaCode2= "LOCNAME", Title = "Location", ParentMetaCode = "PNL1", RowOrder = 3, ColumnOrder = 1, Domain = "DATAVIEW.LOCATIONLOOKUP" });            
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "NUMBOX", MetaCode = "NB_QTY", DataMetaCode = "QTY", Title = "Qty", ParentMetaCode = "PNL1", RowOrder = 4, ColumnOrder = 1 });
            

            //LISTVIEW
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "LISTVIEW", MetaCode = "MAIN_LISTVIEW", DataMetaCode = "", Title = "Goods Receipts", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "LISTVIEWFIELD", MetaCode = "LV_ID", DataMetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 1 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "LISTVIEWFIELD", MetaCode = "LV_GR_DATE", DataMetaCode = "RECEIPTDATE", Title = "Date", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 2 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "LISTVIEWFIELD", MetaCode = "LV_GR_ITEMNAME", DataMetaCode = "ITEMNAME", Title = "Item", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 3 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "LISTVIEWFIELD", MetaCode = "LV_GR_QTY", DataMetaCode = "QTY", Title = "Qty", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 4 });


            //GOODSISSUE
            //----------------------------------------------------
            //DATA
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "GOODSISSUE", MetaType = "DATACOLUMN", MetaCode = "ISSUEDATE", DbName = "IssueDate", ParentMetaCode = "ROOT", DataType = "DATETIME", Mandatory = true });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "GOODSISSUE", MetaType = "DATACOLUMN", MetaCode = "ITEMID", DbName = "ItemID", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true, Domain = "APP.ITEM.ITEMID" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "GOODSISSUE", MetaType = "DATACOLUMN", MetaCode = "ITEMNAME", DbName = "ItemName", ParentMetaCode = "ROOT", DataType = "STRING" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "GOODSISSUE", MetaType = "DATACOLUMN", MetaCode = "LOCATIONID", DbName = "LocationID", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true, Domain = "APP.LOCATION.LOCATIONID" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "GOODSISSUE", MetaType = "DATACOLUMN", MetaCode = "LOCATIONNAME", DbName = "LocationName", ParentMetaCode = "ROOT", DataType = "STRING" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "GOODSISSUE", MetaType = "DATACOLUMN", MetaCode = "QTY", DbName = "Qty", ParentMetaCode = "ROOT", DataType = "INTEGER", Mandatory = true });

            //UI
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "GOODSISSUE", MetaType = "SECTION", MetaCode = "MAINSECTION", DataMetaCode = "", Title = "Basics", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "GOODSISSUE", MetaType = "PANEL", MetaCode = "PNL1", DataMetaCode = "", Title = "Basics", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "GOODSISSUE", MetaType = "DATEPICKER", MetaCode = "ISSUE_DATE", DataMetaCode = "ISSUEDATE", Title = "Issue Date", ParentMetaCode = "PNL1", RowOrder = 1, ColumnOrder = 1 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "GOODSISSUE", MetaType = "LOOKUP", MetaCode = "MLITEM", DataMetaCode = "ITEMID", ViewMetaCode = "ITEMID", DataMetaCode2 = "ITEMNAME", ViewMetaCode2 = "ITEMNAME", Title = "Item", ParentMetaCode = "PNL1", RowOrder = 2, ColumnOrder = 1, Domain = "DATAVIEW.ITEMLOOKUP" });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "GOODSISSUE", MetaType = "LOOKUP", MetaCode = "MLLOCATION", DataMetaCode = "LOCATIONID", ViewMetaCode = "LOCID", DataMetaCode2 = "LOCATIONNAME", ViewMetaCode2 = "LOCNAME", Title = "Location", ParentMetaCode = "PNL1", RowOrder = 3, ColumnOrder = 1, Domain = "DATAVIEW.LOCATIONLOOKUP" }); 
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "GOODSISSUE", MetaType = "NUMBOX", MetaCode = "NB_ISSUE_QTY", DataMetaCode = "QTY", Title = "Qty", ParentMetaCode = "PNL1", RowOrder = 4, ColumnOrder = 1 });


            //LISTVIEW
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "GOODSISSUE", MetaType = "LISTVIEW", MetaCode = "MAIN_LISTVIEW", DataMetaCode = "", Title = "Goods Receipts", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "GOODSISSUE", MetaType = "LISTVIEWFIELD", MetaCode = "LV_ID", DataMetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 1 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "GOODSISSUE", MetaType = "LISTVIEWFIELD", MetaCode = "LV_ISSUE_DATE", DataMetaCode = "ISSUEDATE", Title = "Date", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 2 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "GOODSISSUE", MetaType = "LISTVIEWFIELD", MetaCode = "LV_ISSUE_ITEMNAME", DataMetaCode = "ITEMNAME", Title = "Item", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 3 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "GOODSISSUE", MetaType = "LISTVIEWFIELD", MetaCode = "LV_ISSUE_QTY", DataMetaCode = "QTY", Title = "Qty", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 4 });


            //GOODSTRANSFER
            //----------------------------------------------------
            //DATA
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "GOODSTRANSFER", MetaType = "DATACOLUMN", MetaCode = "TRANSDATE", DbName = "TransferDate", ParentMetaCode = "ROOT", DataType = "DATETIME", Mandatory = true });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "GOODSTRANSFER", MetaType = "DATACOLUMN", MetaCode = "ITEMID", DbName = "ItemID", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true, Domain = "APP.ITEM.ITEMID" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "GOODSTRANSFER", MetaType = "DATACOLUMN", MetaCode = "ITEMNAME", DbName = "ItemName", ParentMetaCode = "ROOT", DataType = "STRING" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "GOODSTRANSFER", MetaType = "DATACOLUMN", MetaCode = "FROMLOCID", DbName = "FromLocationID", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true, Domain = "APP.LOCATION.LOCATIONID" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "GOODSTRANSFER", MetaType = "DATACOLUMN", MetaCode = "FROMLOCNAME", DbName = "FromLocationName", ParentMetaCode = "ROOT", DataType = "STRING"});
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "GOODSTRANSFER", MetaType = "DATACOLUMN", MetaCode = "TOLOCID", DbName = "ToLocationID", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true, Domain = "APP.LOCATION.LOCATIONID" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "GOODSTRANSFER", MetaType = "DATACOLUMN", MetaCode = "TOLOCNAME", DbName = "ToLocationName", ParentMetaCode = "ROOT", DataType = "STRING" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "GOODSTRANSFER", MetaType = "DATACOLUMN", MetaCode = "QTY", DbName = "Qty", ParentMetaCode = "ROOT", DataType = "INTEGER", Mandatory = true });


            //CUSTOMER
            //----------------------------------------------------
            //DATA
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMERID", DbName = "CustomerId", ParentMetaCode = "ROOT",   DataType = "STRING", Mandatory = true });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMERNAME", DbName = "CustomerName", ParentMetaCode = "ROOT",  DataType = "STRING"  });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMERINFO", DbName = "CustomerInfo", ParentMetaCode = "ROOT", DataType = "TEXT" });

            //UI
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "SECTION", MetaCode = "MAINSECTION", DataMetaCode = "", Title = "Sales Header", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "PANEL", MetaCode = "CUSTPNL1", DataMetaCode = "", Title = "Basics", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1  });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "TEXTBOX", MetaCode = "TB_CUSTID", DataMetaCode = "CUSTOMERID", Title = "Customer ID", ParentMetaCode = "CUSTPNL1", RowOrder = 1, ColumnOrder = 1  });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "TEXTBOX", MetaCode = "TB_CUSTNAME", DataMetaCode = "CUSTOMERNAME", Title = "Customer Name", ParentMetaCode = "CUSTPNL1", RowOrder = 2, ColumnOrder = 1  });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "PANEL", MetaCode = "CUSTPNL2", DataMetaCode = "", Title = "Information", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 2 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "TEXTAREA", MetaCode = "TA_CUSTINFO", DataMetaCode = "CUSTOMERINFO", Title = "Customer Notes", ParentMetaCode = "CUSTPNL2", RowOrder = 3, ColumnOrder = 1 });

            //VIEWS
            context.Set<DataViewItem>().Add(new DataViewItem() { MetaType = "DATAVIEW", MetaCode = "CUSTOMERVIEW", ParentMetaCode = "ROOT", SQLQuery = "select CustomerId, CustomerName from Customer order by CustomerId asc", Title = "Customers", SQLQueryFieldName = "" });
            context.Set<DataViewItem>().Add(new DataViewItem() { MetaType = "DATAVIEWKEYCOLUMN", MetaCode = "VCUSTID", ParentMetaCode = "CUSTOMERVIEW", SQLQuery = "", Title = "Id", SQLQueryFieldName = "CustomerId" });
            context.Set<DataViewItem>().Add(new DataViewItem() { MetaType = "DATAVIEWCOLUMN", MetaCode = "VCUSTNAME", ParentMetaCode = "CUSTOMERVIEW", SQLQuery = "", Title = "Name", SQLQueryFieldName = "CustomerName" });


            //LISTVIEW
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "LISTVIEW", MetaCode = "MAIN_LISTVIEW", DataMetaCode = "", Title = "Customer List", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "LISTVIEWFIELD", MetaCode = "LV_ID", DataMetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 1  });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "LISTVIEWFIELD", MetaCode = "LV_CUSTID", DataMetaCode = "CUSTOMERID", Title = "Customer ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 2  });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "LISTVIEWFIELD", MetaCode = "LV_CUSTNAME", DataMetaCode = "CUSTOMERINFO", Title = "Customer Name", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 3  });



            //ITEM
            //----------------------------------------------------
            //DATA
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "ITEMID", DbName = "ItemId", ParentMetaCode = "ROOT", DataType = "STRING" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "NAME", DbName = "ItemName", ParentMetaCode = "ROOT", DataType = "STRING" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "CATCODE", DbName = "ItemCategoryCode", ParentMetaCode = "ROOT", DataType = "STRING", Domain = "VALUEDOMAIN.ITEMCATEGORY.CODE" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "CATVALUE", DbName = "ItemCategoryValue", ParentMetaCode = "ROOT", DataType = "STRING", Domain = "VALUEDOMAIN.ITEMCATEGORY.VALUE" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "MODIFIED", DbName = "Modified", ParentMetaCode = "ROOT", DataType = "DATETIME" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "ACTIVE", DbName = "Active", ParentMetaCode = "ROOT", DataType = "BOOLEAN" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "PURCHASEPRICE", DbName = "PurchasePrice", ParentMetaCode = "ROOT", DataType = "2DECIMAL" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "VENDORCODE", DbName = "VendorCode", ParentMetaCode = "ROOT", DataType = "STRING", Domain = "APP.VENDOR.VENDORID" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "VENDORTXT", DbName = "VendorName", ParentMetaCode = "ROOT", DataType = "STRING", Domain = "APP.VENDOR.VENDORNAME" });

            //VIEWS
            context.Set<DataViewItem>().Add(new DataViewItem() { MetaType = "DATAVIEW", MetaCode = "ITEMLOOKUP", ParentMetaCode = "ROOT", SQLQuery = "select ItemId, ItemName  from Item order by ItemId asc", Title = "Items", SQLQueryFieldName = "" });
            context.Set<DataViewItem>().Add(new DataViewItem() { MetaType = "DATAVIEWKEYCOLUMN", MetaCode = "ITEMID", ParentMetaCode = "ITEMLOOKUP", SQLQuery = "", Title = "Id", SQLQueryFieldName = "ItemId" });
            context.Set<DataViewItem>().Add(new DataViewItem() { MetaType = "DATAVIEWCOLUMN", MetaCode = "ITEMNAME", ParentMetaCode = "ITEMLOOKUP", SQLQuery = "", Title = "Name", SQLQueryFieldName = "ItemName" });


            //UI
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "SECTION", MetaCode = "MAINSECTION", DataMetaCode = "", Title = "Basics", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
            
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "PANEL", MetaCode = "ITMPNL_A", DataMetaCode = "", Title = "Basics", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "TEXTBOX", MetaCode = "TB_ITID", DataMetaCode = "ITEMID", Title = "Item ID", ParentMetaCode = "ITMPNL_A", RowOrder = 1, ColumnOrder = 1  });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "TEXTBOX", MetaCode = "TB_ITNAME", DataMetaCode = "NAME", Title = "Item Name", ParentMetaCode = "ITMPNL_A", RowOrder = 2, ColumnOrder = 1  });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "DATEPICKER", MetaCode = "DP_MOD", DataMetaCode = "MODIFIED", Title = "Modified Date", ParentMetaCode = "ITMPNL_A", RowOrder = 3, ColumnOrder = 1 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "CHECKBOX", MetaCode = "CB_ACTIVE", DataMetaCode = "ACTIVE", Title = "Is Active", ParentMetaCode = "ITMPNL_A", RowOrder = 4, ColumnOrder = 1 });

            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "PANEL", MetaCode = "ITMPNL_B", DataMetaCode = "", Title = "Basics", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 2 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "COMBOBOX", MetaCode = "CB_CATEGORY", DataMetaCode = "CATCODE", Title = "Category", ParentMetaCode = "ITMPNL_B", RowOrder = 1, ColumnOrder = 1 , Domain = "VALUEDOMAIN.ITEMCATEGORY" });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "NUMBOX", MetaCode = "NUMBOX_PURCHPRICE", DataMetaCode = "PURCHASEPRICE", Title = "Purchase Price", ParentMetaCode = "ITMPNL_B", RowOrder = 2, ColumnOrder = 1 });
            
            //LISTVIEW
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "LISTVIEW", MetaCode = "MAIN_LISTVIEW", DataMetaCode = "", Title = "Item List", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "LISTVIEWFIELD", MetaCode = "LV_ID", DataMetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 1  });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "LISTVIEWFIELD", MetaCode = "LV_ITEMID", DataMetaCode = "ITEMID", Title = "Item ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 2  });
            context.Set<UserInterfaceItem>().Add(new UserInterfaceItem() { AppMetaCode = "ITEM", MetaType = "LISTVIEWFIELD", MetaCode = "LV_ITEMNAME", DataMetaCode = "NAME", Title = "Item Name", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 3  });


            //ITEMLEDGER
            //----------------------------------------------------
            //DATA
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "ITEMLEDGER", MetaType = "DATACOLUMN", MetaCode = "TRANSDATE", DbName = "TransactionDate", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "ITEMLEDGER", MetaType = "DATACOLUMN", MetaCode = "LOCATIONID", DbName = "LocationId", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "ITEMLEDGER", MetaType = "DATACOLUMN", MetaCode = "TRANSTYPE", DbName = "TransactionType", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true, Domain = "VALUEDOMAIN.ITEMTRANSTYPE.CODE" });
            context.Set<DatabaseItem>().Add(new DatabaseItem() { AppMetaCode = "ITEMLEDGER", MetaType = "DATACOLUMN", MetaCode = "QTY", DbName = "Qty", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true });


            context.SaveChanges();
        }

        private static void SeedValueDomains(ApplicationDbContext context, bool isupdate)
        {

            if (isupdate)
            {
                context.Set<ValueDomainItem>().RemoveRange(context.Set<ValueDomainItem>());
                context.SaveChanges();
            }

            //ITEMCATEGORY
            context.Set<ValueDomainItem>().Add(new ValueDomainItem() { DomainName= "ITEMCATEGORY", Code = "A1", Value = "Primary" });
            context.Set<ValueDomainItem>().Add(new ValueDomainItem() { DomainName = "ITEMCATEGORY", Code = "A2", Value = "Secondary" });

            //LOCATIONTYPE
            context.Set<ValueDomainItem>().Add(new ValueDomainItem() { DomainName = "LOCATIONTYPE", Code = "LOC001", Value = "Warehouse" });



            context.SaveChanges();
        }

        private static void SeedNoSeries(ApplicationDbContext context, bool isupdate)
        {

            if (isupdate)
            {
                context.Set<NoSerie>().RemoveRange(context.Set<NoSerie>());
                context.SaveChanges();
            }

            //NoSerie
            context.Set<NoSerie>().Add(new NoSerie() { AppMetaCode = "ITEM", MetaCode = "ITEMID_SERIE", DataMetaCode="ITEMID", Description= "No serie definition for Item Id", Prefix = "ITEM", StartValue = 10000, Counter = 0  });
            context.Set<NoSerie>().Add(new NoSerie() { AppMetaCode = "VENDOR", MetaCode = "VENDORID_SERIE", DataMetaCode = "VENDORID", Description = "No serie definition for Vendor Id", Prefix = "VEND", StartValue = 10000, Counter = 0 });
            context.Set<NoSerie>().Add(new NoSerie() { AppMetaCode = "LOCATION", MetaCode = "LOCATIONID_SERIE", DataMetaCode = "LOCATIONID", Description = "No serie definition for Location Id", Prefix = "LOC", StartValue = 10000, Counter = 0 });


            context.SaveChanges();
        }





    }
 }
