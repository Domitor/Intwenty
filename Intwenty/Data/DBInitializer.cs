using System;
using Microsoft.Extensions.DependencyInjection;
using Moley.Data.Entity;

namespace Moley.Data
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
                SeedApplicationDescriptions(context, true);
                SeedSystemMenus(context, true);
                SeedApplicationContent(context, true);
                SeedValueDomains(context, true);
                SeedNoSeries(context, true);


            }

           
        }


        private static void SeedApplicationDescriptions(ApplicationDbContext context, bool isupdate)
        {
            if (isupdate)
            {
                context.Set<ApplicationDescription>().RemoveRange(context.Set<ApplicationDescription>());
                context.SaveChanges();
            }


            context.Set<ApplicationDescription>().Add(new ApplicationDescription() { Id = 10, Description = "An app for managing customers", MetaCode = "CUSTOMER", Title = "Customer", DbName = "Customer", IsHierarchicalApplication = false, UseVersioning=false, TestDataAmount = 0  });
            context.Set<ApplicationDescription>().Add(new ApplicationDescription() { Id = 20, Description = "An app for managing items", MetaCode = "ITEM", Title = "Item", DbName = "Item", IsHierarchicalApplication = false, UseVersioning = false, TestDataAmount = 5 });
            context.Set<ApplicationDescription>().Add(new ApplicationDescription() { Id = 30, Description = "An app for managing item ledger", MetaCode = "ITEMLEDGER", Title = "Inventory Transactions", DbName = "ItemLedger", IsHierarchicalApplication = false, UseVersioning = false });
            context.Set<ApplicationDescription>().Add(new ApplicationDescription() { Id = 40, Description = "An app for managing vendors", MetaCode = "VENDOR", Title = "Vendor", DbName = "Vendor", IsHierarchicalApplication = false, UseVersioning = false, TestDataAmount = 10 });
            context.Set<ApplicationDescription>().Add(new ApplicationDescription() { Id = 50, Description = "An app for managing locations", MetaCode = "LOCATION", Title = "Location", DbName = "Location", IsHierarchicalApplication = false, UseVersioning = false, TestDataAmount = 3 });
            context.Set<ApplicationDescription>().Add(new ApplicationDescription() { Id = 60, Description = "An app for managing goods reciept", MetaCode = "GOODSRECEIPT", Title = "Goods Receipt", DbName = "GoodsReceipt", IsHierarchicalApplication = false, UseVersioning = false, TestDataAmount = 0 });
            context.Set<ApplicationDescription>().Add(new ApplicationDescription() { Id = 70, Description = "An app for managing shipment files", MetaCode = "SHIPMENTFILES", Title = "Shipment files", DbName = "Shipments", IsHierarchicalApplication = false, UseVersioning = false, TestDataAmount = 0 });
            context.Set<ApplicationDescription>().Add(new ApplicationDescription() { Id = 80, Description = "An app for managing goods issue", MetaCode = "GOODSISSUE", Title = "Goods Issue", DbName = "GoodsIssue", IsHierarchicalApplication = false, UseVersioning = false, TestDataAmount = 0 });
            context.Set<ApplicationDescription>().Add(new ApplicationDescription() { Id = 90, Description = "An app for managing goods transfer", MetaCode = "GOODSTRANSFER", Title = "Goods Transfer", DbName = "GoodsTransfer", IsHierarchicalApplication = false, UseVersioning = false });


            context.SaveChanges();
        }


        private static void SeedSystemMenus(ApplicationDbContext context, bool isupdate)
        {
            if (isupdate)
            {
                context.Set<MetaMenuItem>().RemoveRange(context.Set<MetaMenuItem>());
                context.SaveChanges();
            }

            context.Set<MetaMenuItem>().Add(new MetaMenuItem() { AppMetaCode = "", MetaType = "MAINMENU", MetaCode = "WMS_MAINMENU", ParentMetaCode = "ROOT", Title = "Menu", Order = 1, Action = "", Controller = ""  });
            context.Set<MetaMenuItem>().Add(new MetaMenuItem() { AppMetaCode = "ITEM", MetaType = "MENUITEM", MetaCode = "M_ITEM", ParentMetaCode = "WMS_MAINMENU", Title = "Item", Order = 10, Action = "", Controller = "" });
            context.Set<MetaMenuItem>().Add(new MetaMenuItem() { AppMetaCode = "VENDOR", MetaType = "MENUITEM", MetaCode = "M_VENDOR", ParentMetaCode = "WMS_MAINMENU", Title = "Vendor", Order = 20, Action = "", Controller = "" });
            context.Set<MetaMenuItem>().Add(new MetaMenuItem() { AppMetaCode = "LOCATION", MetaType = "MENUITEM", MetaCode = "M_LOC", ParentMetaCode = "WMS_MAINMENU", Title = "Location", Order = 30, Action = "", Controller = "" });
            context.Set<MetaMenuItem>().Add(new MetaMenuItem() { AppMetaCode = "SHIPMENTFILES", MetaType = "MENUITEM", MetaCode = "M_SF", ParentMetaCode = "WMS_MAINMENU", Title = "Shipment files", Order = 32, Action = "ShipmentFiles", Controller = "Custom" });
            context.Set<MetaMenuItem>().Add(new MetaMenuItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "MENUITEM", MetaCode = "M_GR", ParentMetaCode = "WMS_MAINMENU", Title = "Goods Receipt", Order = 40, Action = "", Controller = "" });
            context.Set<MetaMenuItem>().Add(new MetaMenuItem() { AppMetaCode = "GOODSISSUE", MetaType = "MENUITEM", MetaCode = "M_GI", ParentMetaCode = "WMS_MAINMENU", Title = "Goods Issue", Order = 50, Action = "", Controller = "" });
            context.Set<MetaMenuItem>().Add(new MetaMenuItem() { AppMetaCode = "GOODSTRANSFER", MetaType = "MENUITEM", MetaCode = "M_GT", ParentMetaCode = "WMS_MAINMENU", Title = "Goods Transfer", Order = 60, Action = "", Controller = "" });

            context.SaveChanges();
        }


        private static void SeedApplicationContent(ApplicationDbContext context, bool isupdate)
        {

            if (isupdate)
            {
                context.Set<MetaDataItem>().RemoveRange(context.Set<MetaDataItem>());
                context.Set<MetaDataView>().RemoveRange(context.Set<MetaDataView>());
                context.Set<MetaUIItem>().RemoveRange(context.Set<MetaUIItem>());
                context.SaveChanges();
            }

            //SHIPMENTFILES
            //----------------------------------------------------
            //DATA
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "SHIPMENTFILES", MetaType = "DATAVALUE", MetaCode = "SHIPMENTID", DbName = "ShipmentId", ParentMetaCode = "ROOT", DataType = "STRING" });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "SHIPMENTFILES", MetaType = "DATAVALUE", MetaCode = "FILENAME", DbName = "FileName", ParentMetaCode = "ROOT", DataType = "STRING"});
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "SHIPMENTFILES", MetaType = "DATAVALUE", MetaCode = "IMPORTDATE", DbName = "ImportDate", ParentMetaCode = "ROOT", DataType = "DATETIME" });

            //LISTVIEW
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "SHIPMENTFILES", MetaType = "LISTVIEW", MetaCode = "MAIN_LISTVIEW", DataMetaCode = "", Title = "Shipment files", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0, Properties = "HIDEFILTER=TRUE" });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "SHIPMENTFILES", MetaType = "LISTVIEWFIELD", MetaCode = "LV_ID", DataMetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 1 });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "SHIPMENTFILES", MetaType = "LISTVIEWFIELD", MetaCode = "LF_SHIPMENTID", DataMetaCode = "SHIPMENTID", Title = "Shipment ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 2 });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "SHIPMENTFILES", MetaType = "LISTVIEWFIELD", MetaCode = "LF_IMPORTDATE", DataMetaCode = "IMPORTDATE", Title = "Import Date", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 3 });


            //LOCATION
            //----------------------------------------------------
            //DATA
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "LOCATION", MetaType = "DATAVALUE", MetaCode = "LOCATIONID", DbName = "LocationId", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "LOCATION", MetaType = "DATAVALUE", MetaCode = "LOCATIONNAME", DbName = "LocationName", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "LOCATION", MetaType = "DATAVALUE", MetaCode = "LOCATIONTYPE", DbName = "LocationType", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true, Domain = "VALUEDOMAIN.LOCATIONTYPE.CODE" });

            //VIEWS
            context.Set<MetaDataView>().Add(new MetaDataView() { MetaType = "DATAVIEW", MetaCode = "LOCATIONLOOKUP", ParentMetaCode = "ROOT", SQLQuery = "select LocationId, LocationName  from Location {0} order by LocationId asc", Title = "Location", SQLQueryFieldName = "" });
            context.Set<MetaDataView>().Add(new MetaDataView() { MetaType = "DATAVIEWKEYFIELD", MetaCode = "LOCID", ParentMetaCode = "LOCATIONLOOKUP", SQLQuery = "", Title = "Id", SQLQueryFieldName = "LocationId" });
            context.Set<MetaDataView>().Add(new MetaDataView() { MetaType = "DATAVIEWFIELD", MetaCode = "LOCNAME", ParentMetaCode = "LOCATIONLOOKUP", SQLQuery = "", Title = "Name", SQLQueryFieldName = "LocationName" });

            //UI
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "LOCATION", MetaType = "PANEL", MetaCode = "PNL1", DataMetaCode = "", Title = "Basics", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, CssClass = "app_panel" });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "LOCATION", MetaType = "TEXTBOX", MetaCode = "TB_ID", DataMetaCode = "LOCATIONID", Title = "Location ID", ParentMetaCode = "PNL1", RowOrder = 1, ColumnOrder = 1 });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "LOCATION", MetaType = "TEXTBOX", MetaCode = "TB_NAME", DataMetaCode = "LOCATIONNAME", Title = "Location Name", ParentMetaCode = "PNL1", RowOrder = 2, ColumnOrder = 1 });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "LOCATION", MetaType = "COMBOBOX", MetaCode = "CB_TYPE", DataMetaCode = "LOCATIONTYPE", Title = "Location Type", ParentMetaCode = "PNL1", RowOrder = 3, ColumnOrder = 1, Domain = "VALUEDOMAIN.LOCATIONTYPE.CODE" });

            //LISTVIEW
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "LOCATION", MetaType = "LISTVIEW", MetaCode = "MAIN_LISTVIEW", DataMetaCode = "", Title = "Location List", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "LOCATION", MetaType = "LISTVIEWFIELD", MetaCode = "LV_ID", DataMetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 1 });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "LOCATION", MetaType = "LISTVIEWFIELD", MetaCode = "LV_LID", DataMetaCode = "LOCATIONID", Title = "Location ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 2 });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "LOCATION", MetaType = "LISTVIEWFIELD", MetaCode = "LV_LNAME", DataMetaCode = "LOCATIONNAME", Title = "Location Name", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 3 });


            //GOODSRECEIPT
            //----------------------------------------------------
            //DATA
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "DATAVALUE", MetaCode = "RECEIPTDATE", DbName = "ReceiptDate", ParentMetaCode = "ROOT", DataType = "DATETIME", Mandatory = true });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "DATAVALUE", MetaCode = "ITEMID", DbName = "ItemID", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true, Domain = "APP.ITEM.ITEMID" });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "DATAVALUE", MetaCode = "ITEMNAME", DbName = "ItemName", ParentMetaCode = "ROOT", DataType = "STRING" });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "DATAVALUE", MetaCode = "LOCATIONID", DbName = "LocationID", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true, Domain = "APP.LOCATION.LOCATIONID" });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "DATAVALUE", MetaCode = "LOCATIONNAME", DbName = "LocationName", ParentMetaCode = "ROOT", DataType = "STRING" });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "DATAVALUE", MetaCode = "QTY", DbName = "Qty", ParentMetaCode = "ROOT", DataType = "INTEGER", Mandatory = true });

            //UI
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "PANEL", MetaCode = "PNL1", DataMetaCode = "", Title = "Basics", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, CssClass = "app_panel" });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "DATEPICKER", MetaCode = "GR_DATE", DataMetaCode = "RECEIPTDATE", Title = "Receipt Date", ParentMetaCode = "PNL1", RowOrder = 1, ColumnOrder = 1 });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "LOOKUP", MetaCode = "MLITEM", DataMetaCode = "", Title = "Item", ParentMetaCode = "PNL1", RowOrder = 2, ColumnOrder = 1, Domain = "DATAVIEW.ITEMLOOKUP" });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "LOOKUPKEYFIELD", MetaCode = "MLITEM_CODE", DataMetaCode = "ITEMID", Title = "Item Id", ParentMetaCode = "MLITEM", RowOrder = 0, ColumnOrder = 1, Domain = "DATAVIEW.ITEMLOOKUP.ITEMID" });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "LOOKUPFIELD", MetaCode = "MLITEM_NAME", DataMetaCode = "ITEMNAME", Title = "Item Name", ParentMetaCode = "MLITEM", RowOrder = 0, ColumnOrder = 2, Domain = "DATAVIEW.ITEMLOOKUP.ITEMNAME" });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "LOOKUP", MetaCode = "MLLOCATION", DataMetaCode = "", Title = "Location", ParentMetaCode = "PNL1", RowOrder = 3, ColumnOrder = 1, Domain = "DATAVIEW.LOCATIONLOOKUP" });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "LOOKUPKEYFIELD", MetaCode = "MLLOCATION_CODE", DataMetaCode = "LOCATIONID", Title = "Location Id", ParentMetaCode = "MLLOCATION", RowOrder = 0, ColumnOrder = 1, Domain = "DATAVIEW.LOCATIONLOOKUP.LOCID" });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "LOOKUPFIELD", MetaCode = "MLLOCATION_NAME", DataMetaCode = "LOCATIONNAME", Title = "Location Name", ParentMetaCode = "MLLOCATION", RowOrder = 0, ColumnOrder = 2, Domain = "DATAVIEW.LOCATIONLOOKUP.LOCNAME" });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "NUMBOX", MetaCode = "NB_QTY", DataMetaCode = "QTY", Title = "Qty", ParentMetaCode = "PNL1", RowOrder = 4, ColumnOrder = 1 });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "PANEL", MetaCode = "PNL2", DataMetaCode = "", Title = "Info", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 2, CssClass = "app_panel" });
            

            //LISTVIEW
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "LISTVIEW", MetaCode = "MAIN_LISTVIEW", DataMetaCode = "", Title = "Goods Receipts", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "LISTVIEWFIELD", MetaCode = "LV_ID", DataMetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 1 });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "LISTVIEWFIELD", MetaCode = "LV_GR_DATE", DataMetaCode = "RECEIPTDATE", Title = "Date", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 2 });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "LISTVIEWFIELD", MetaCode = "LV_GR_ITEMNAME", DataMetaCode = "ITEMNAME", Title = "Item", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 3 });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "GOODSRECEIPT", MetaType = "LISTVIEWFIELD", MetaCode = "LV_GR_QTY", DataMetaCode = "QTY", Title = "Qty", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 4 });


            //GOODSISSUE
            //----------------------------------------------------
            //DATA
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "GOODSISSUE", MetaType = "DATAVALUE", MetaCode = "ISSUEDATE", DbName = "IssueDate", ParentMetaCode = "ROOT", DataType = "DATETIME", Mandatory = true });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "GOODSISSUE", MetaType = "DATAVALUE", MetaCode = "ITEMID", DbName = "ItemID", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true, Domain = "APP.ITEM.ITEMID" });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "GOODSISSUE", MetaType = "DATAVALUE", MetaCode = "ITEMNAME", DbName = "ItemName", ParentMetaCode = "ROOT", DataType = "STRING" });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "GOODSISSUE", MetaType = "DATAVALUE", MetaCode = "LOCATIONID", DbName = "LocationID", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true, Domain = "APP.LOCATION.LOCATIONID" });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "GOODSISSUE", MetaType = "DATAVALUE", MetaCode = "LOCATIONNAME", DbName = "LocationName", ParentMetaCode = "ROOT", DataType = "STRING" });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "GOODSISSUE", MetaType = "DATAVALUE", MetaCode = "QTY", DbName = "Qty", ParentMetaCode = "ROOT", DataType = "INTEGER", Mandatory = true });

            //UI
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "GOODSISSUE", MetaType = "PANEL", MetaCode = "PNL1", DataMetaCode = "", Title = "Basics", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, CssClass = "app_panel" });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "GOODSISSUE", MetaType = "DATEPICKER", MetaCode = "ISSUE_DATE", DataMetaCode = "ISSUEDATE", Title = "Issue Date", ParentMetaCode = "PNL1", RowOrder = 1, ColumnOrder = 1 });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "GOODSISSUE", MetaType = "LOOKUP", MetaCode = "MLITEM", DataMetaCode = "", Title = "Item", ParentMetaCode = "PNL1", RowOrder = 2, ColumnOrder = 1, Domain = "DATAVIEW.ITEMLOOKUP" });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "GOODSISSUE", MetaType = "LOOKUPKEYFIELD", MetaCode = "MLITEM_CODE", DataMetaCode = "ITEMID", Title = "Item Id", ParentMetaCode = "MLITEM", RowOrder = 0, ColumnOrder = 1, Domain = "DATAVIEW.ITEMLOOKUP.ITEMID" });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "GOODSISSUE", MetaType = "LOOKUPFIELD", MetaCode = "MLITEM_NAME", DataMetaCode = "ITEMNAME", Title = "Item Name", ParentMetaCode = "MLITEM", RowOrder = 0, ColumnOrder = 2, Domain = "DATAVIEW.ITEMLOOKUP.ITEMNAME" });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "GOODSISSUE", MetaType = "LOOKUP", MetaCode = "MLLOCATION", DataMetaCode = "", Title = "Location", ParentMetaCode = "PNL1", RowOrder = 3, ColumnOrder = 1, Domain = "DATAVIEW.LOCATIONLOOKUP" });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "GOODSISSUE", MetaType = "LOOKUPKEYFIELD", MetaCode = "MLLOCATION_CODE", DataMetaCode = "LOCATIONID", Title = "Location Id", ParentMetaCode = "MLLOCATION", RowOrder = 0, ColumnOrder = 1, Domain = "DATAVIEW.LOCATIONLOOKUP.LOCID" });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "GOODSISSUE", MetaType = "LOOKUPFIELD", MetaCode = "MLLOCATION_NAME", DataMetaCode = "LOCATIONNAME", Title = "Location Name", ParentMetaCode = "MLLOCATION", RowOrder = 0, ColumnOrder = 2, Domain = "DATAVIEW.LOCATIONLOOKUP.LOCNAME" });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "GOODSISSUE", MetaType = "NUMBOX", MetaCode = "NB_ISSUE_QTY", DataMetaCode = "QTY", Title = "Qty", ParentMetaCode = "PNL1", RowOrder = 4, ColumnOrder = 1 });


            //LISTVIEW
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "GOODSISSUE", MetaType = "LISTVIEW", MetaCode = "MAIN_LISTVIEW", DataMetaCode = "", Title = "Goods Receipts", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "GOODSISSUE", MetaType = "LISTVIEWFIELD", MetaCode = "LV_ID", DataMetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 1 });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "GOODSISSUE", MetaType = "LISTVIEWFIELD", MetaCode = "LV_ISSUE_DATE", DataMetaCode = "ISSUEDATE", Title = "Date", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 2 });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "GOODSISSUE", MetaType = "LISTVIEWFIELD", MetaCode = "LV_ISSUE_ITEMNAME", DataMetaCode = "ITEMNAME", Title = "Item", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 3 });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "GOODSISSUE", MetaType = "LISTVIEWFIELD", MetaCode = "LV_ISSUE_QTY", DataMetaCode = "QTY", Title = "Qty", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 4 });


            //GOODSTRANSFER
            //----------------------------------------------------
            //DATA
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "GOODSTRANSFER", MetaType = "DATAVALUE", MetaCode = "TRANSDATE", DbName = "TransferDate", ParentMetaCode = "ROOT", DataType = "DATETIME", Mandatory = true });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "GOODSTRANSFER", MetaType = "DATAVALUE", MetaCode = "ITEMID", DbName = "ItemID", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true, Domain = "APP.ITEM.ITEMID" });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "GOODSTRANSFER", MetaType = "DATAVALUE", MetaCode = "ITEMNAME", DbName = "ItemName", ParentMetaCode = "ROOT", DataType = "STRING" });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "GOODSTRANSFER", MetaType = "DATAVALUE", MetaCode = "FROMLOCID", DbName = "FromLocationID", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true, Domain = "APP.LOCATION.LOCATIONID" });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "GOODSTRANSFER", MetaType = "DATAVALUE", MetaCode = "FROMLOCNAME", DbName = "FromLocationName", ParentMetaCode = "ROOT", DataType = "STRING"});
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "GOODSTRANSFER", MetaType = "DATAVALUE", MetaCode = "TOLOCID", DbName = "ToLocationID", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true, Domain = "APP.LOCATION.LOCATIONID" });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "GOODSTRANSFER", MetaType = "DATAVALUE", MetaCode = "TOLOCNAME", DbName = "ToLocationName", ParentMetaCode = "ROOT", DataType = "STRING" });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "GOODSTRANSFER", MetaType = "DATAVALUE", MetaCode = "QTY", DbName = "Qty", ParentMetaCode = "ROOT", DataType = "INTEGER", Mandatory = true });


            //CUSTOMER
            //----------------------------------------------------
            //DATA
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "CUSTOMER", MetaType = "DATAVALUE", MetaCode = "CUSTOMERID", DbName = "CustomerId", ParentMetaCode = "ROOT",   DataType = "STRING", Mandatory = true });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "CUSTOMER", MetaType = "DATAVALUE", MetaCode = "CUSTOMERNAME", DbName = "CustomerName", ParentMetaCode = "ROOT",  DataType = "STRING"  });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "CUSTOMER", MetaType = "DATAVALUE", MetaCode = "CUSTOMERINFO", DbName = "CustomerInfo", ParentMetaCode = "ROOT", DataType = "TEXT" });

            //UI
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "CUSTOMER", MetaType = "PANEL", MetaCode = "CUSTPNL1", DataMetaCode = "", Title = "Basics", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, CssClass = "app_panel"  });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "CUSTOMER", MetaType = "TEXTBOX", MetaCode = "TB_CUSTID", DataMetaCode = "CUSTOMERID", Title = "Customer ID", ParentMetaCode = "CUSTPNL1", RowOrder = 1, ColumnOrder = 1  });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "CUSTOMER", MetaType = "TEXTBOX", MetaCode = "TB_CUSTNAME", DataMetaCode = "CUSTOMERNAME", Title = "Customer Name", ParentMetaCode = "CUSTPNL1", RowOrder = 2, ColumnOrder = 1  });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "CUSTOMER", MetaType = "PANEL", MetaCode = "CUSTPNL2", DataMetaCode = "", Title = "Information", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 2, CssClass = "app_panel" });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "CUSTOMER", MetaType = "TEXTAREA", MetaCode = "TA_CUSTINFO", DataMetaCode = "CUSTOMERINFO", Title = "Customer Notes", ParentMetaCode = "CUSTPNL2", RowOrder = 3, ColumnOrder = 1 });

            //LISTVIEW
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "CUSTOMER", MetaType = "LISTVIEW", MetaCode = "MAIN_LISTVIEW", DataMetaCode = "", Title = "Customer List", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "CUSTOMER", MetaType = "LISTVIEWFIELD", MetaCode = "LV_ID", DataMetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 1  });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "CUSTOMER", MetaType = "LISTVIEWFIELD", MetaCode = "LV_CUSTID", DataMetaCode = "CUSTOMERID", Title = "Customer ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 2  });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "CUSTOMER", MetaType = "LISTVIEWFIELD", MetaCode = "LV_CUSTNAME", DataMetaCode = "CUSTOMERINFO", Title = "Customer Name", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 3  });



            //ITEM
            //----------------------------------------------------
            //DATA
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "ITEM", MetaType = "DATAVALUE", MetaCode = "ITEMID", DbName = "ItemId", ParentMetaCode = "ROOT", DataType = "STRING" });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "ITEM", MetaType = "DATAVALUE", MetaCode = "NAME", DbName = "ItemName", ParentMetaCode = "ROOT", DataType = "STRING" });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "ITEM", MetaType = "DATAVALUE", MetaCode = "CATCODE", DbName = "ItemCategoryCode", ParentMetaCode = "ROOT", DataType = "STRING", Domain = "VALUEDOMAIN.ITEMCATEGORY.CODE" });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "ITEM", MetaType = "DATAVALUE", MetaCode = "CATVALUE", DbName = "ItemCategoryValue", ParentMetaCode = "ROOT", DataType = "STRING", Domain = "VALUEDOMAIN.ITEMCATEGORY.VALUE" });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "ITEM", MetaType = "DATAVALUE", MetaCode = "MODIFIED", DbName = "Modified", ParentMetaCode = "ROOT", DataType = "DATETIME" });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "ITEM", MetaType = "DATAVALUE", MetaCode = "ACTIVE", DbName = "Active", ParentMetaCode = "ROOT", DataType = "BOOLEAN" });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "ITEM", MetaType = "DATAVALUE", MetaCode = "PURCHASEPRICE", DbName = "PurchasePrice", ParentMetaCode = "ROOT", DataType = "2DECIMAL" });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "ITEM", MetaType = "DATAVALUE", MetaCode = "VENDORCODE", DbName = "VendorCode", ParentMetaCode = "ROOT", DataType = "STRING", Domain = "APP.VENDOR.VENDORID" });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "ITEM", MetaType = "DATAVALUE", MetaCode = "VENDORTXT", DbName = "VendorName", ParentMetaCode = "ROOT", DataType = "STRING", Domain = "APP.VENDOR.VENDORNAME" });

            //VIEWS
            context.Set<MetaDataView>().Add(new MetaDataView() { MetaType = "DATAVIEW", MetaCode = "ITEMLOOKUP", ParentMetaCode = "ROOT", SQLQuery = "select ItemId, ItemName  from Item {0} order by ItemId asc", Title = "Select Item", SQLQueryFieldName = "" });
            context.Set<MetaDataView>().Add(new MetaDataView() { MetaType = "DATAVIEWKEYFIELD", MetaCode = "ITEMID", ParentMetaCode = "ITEMLOOKUP", SQLQuery = "", Title = "Id", SQLQueryFieldName = "ItemId" });
            context.Set<MetaDataView>().Add(new MetaDataView() { MetaType = "DATAVIEWFIELD", MetaCode = "ITEMNAME", ParentMetaCode = "ITEMLOOKUP", SQLQuery = "", Title = "Name", SQLQueryFieldName = "ItemName" });


            //UI
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "ITEM", MetaType = "PANEL", MetaCode = "ITMPNL", DataMetaCode = "", Title = "Basics", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, CssClass = "app_panel" });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "ITEM", MetaType = "TEXTBOX", MetaCode = "TB_ITID", DataMetaCode = "ITEMID", Title = "Item ID", ParentMetaCode = "ITMPNL", RowOrder = 1, ColumnOrder = 1  });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "ITEM", MetaType = "TEXTBOX", MetaCode = "TB_ITNAME", DataMetaCode = "NAME", Title = "Item Name", ParentMetaCode = "ITMPNL", RowOrder = 2, ColumnOrder = 1  });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "ITEM", MetaType = "COMBOBOX", MetaCode = "CB_CATEGORY", DataMetaCode = "CATCODE", Title = "Category", ParentMetaCode = "ITMPNL", RowOrder = 3, ColumnOrder = 1 , Domain = "VALUEDOMAIN.ITEMCATEGORY" });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "ITEM", MetaType = "NUMBOX", MetaCode = "NUMBOX_PURCHPRICE", DataMetaCode = "PURCHASEPRICE", Title = "Purchase Price", ParentMetaCode = "ITMPNL", RowOrder = 10, ColumnOrder = 1 });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "ITEM", MetaType = "DATEPICKER", MetaCode = "DP_MOD", DataMetaCode = "MODIFIED", Title = "Modified Date", ParentMetaCode = "ITMPNL", RowOrder = 20, ColumnOrder = 1  });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "ITEM", MetaType = "CHECKBOX", MetaCode = "CB_ACTIVE", DataMetaCode = "ACTIVE", Title = "Is Active", ParentMetaCode = "ITMPNL", RowOrder = 30, ColumnOrder = 1  });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "ITEM", MetaType = "LOOKUP", MetaCode = "MLVENDOR", DataMetaCode = "", Title = "Default Vendor", ParentMetaCode = "ITMPNL", RowOrder = 40, ColumnOrder = 1 , Domain = "DATAVIEW.VENDORLOOKUP" });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "ITEM", MetaType = "LOOKUPKEYFIELD", MetaCode = "MLVENDOR_CODE", DataMetaCode = "VENDORCODE", Title = "Vendor Id", ParentMetaCode = "MLVENDOR", RowOrder = 0, ColumnOrder = 1 , Domain = "DATAVIEW.VENDORLOOKUP.VENDORID" });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "ITEM", MetaType = "LOOKUPFIELD", MetaCode = "MLVENDOR_NAME", DataMetaCode = "VENDORTXT", Title = "Vendor Name", ParentMetaCode = "MLVENDOR", RowOrder = 0, ColumnOrder = 2 , Domain = "DATAVIEW.VENDORLOOKUP.VENDORNAME" });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "ITEM", MetaType = "PANEL", MetaCode = "ITMPNL_INFO", DataMetaCode = "", Title = "Info", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 2, CssClass = "app_panel" });


            //LISTVIEW
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "ITEM", MetaType = "LISTVIEW", MetaCode = "MAIN_LISTVIEW", DataMetaCode = "", Title = "Item List", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "ITEM", MetaType = "LISTVIEWFIELD", MetaCode = "LV_ID", DataMetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 1  });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "ITEM", MetaType = "LISTVIEWFIELD", MetaCode = "LV_ITEMID", DataMetaCode = "ITEMID", Title = "Item ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 2  });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "ITEM", MetaType = "LISTVIEWFIELD", MetaCode = "LV_ITEMNAME", DataMetaCode = "NAME", Title = "Item Name", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 3  });

            //ITEMLEDGER
            //----------------------------------------------------
            //DATA
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "ITEMLEDGER", MetaType = "DATAVALUE", MetaCode = "TRANSDATE", DbName = "TransactionDate", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "ITEMLEDGER", MetaType = "DATAVALUE", MetaCode = "LOCATIONID", DbName = "LocationId", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "ITEMLEDGER", MetaType = "DATAVALUE", MetaCode = "TRANSTYPE", DbName = "TransactionType", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true, Domain = "VALUEDOMAIN.ITEMTRANSTYPE.CODE" });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "ITEMLEDGER", MetaType = "DATAVALUE", MetaCode = "QTY", DbName = "Qty", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true });



            //VENDOR
            //----------------------------------------------------
            //DATA
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "VENDOR", MetaType = "DATAVALUE", MetaCode = "VENDORID", DbName = "VendorId", ParentMetaCode = "ROOT", DataType = "STRING" });
            context.Set<MetaDataItem>().Add(new MetaDataItem() { AppMetaCode = "VENDOR", MetaType = "DATAVALUE", MetaCode = "VENDORNAME", DbName = "VendorName", ParentMetaCode = "ROOT", DataType = "STRING" });

            //VIEWS
            context.Set<MetaDataView>().Add(new MetaDataView() { MetaType = "DATAVIEW",  MetaCode = "VENDORLOOKUP", ParentMetaCode = "ROOT", SQLQuery = "select VendorId, VendorName, 'Country' as Country  from Vendor{0} order by VendorId asc", Title = "Select Vendor", SQLQueryFieldName = "" });
            context.Set<MetaDataView>().Add(new MetaDataView() { MetaType = "DATAVIEWKEYFIELD", MetaCode = "VENDORID", ParentMetaCode = "VENDORLOOKUP", SQLQuery = "", Title = "Vendor Id", SQLQueryFieldName = "VendorId" });
            context.Set<MetaDataView>().Add(new MetaDataView() { MetaType = "DATAVIEWFIELD", MetaCode = "VENDORNAME", ParentMetaCode = "VENDORLOOKUP", SQLQuery = "", Title = "Vendor Name", SQLQueryFieldName = "VendorName" });

            //UI
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "VENDOR", MetaType = "PANEL", MetaCode = "VNDPNL", DataMetaCode = "", Title = "Basics", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, CssClass = "app_panel" });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "VENDOR", MetaType = "TEXTBOX", MetaCode = "TB_VENDID", DataMetaCode = "VENDORID", Title = "Vendor ID", ParentMetaCode = "VNDPNL", RowOrder = 1, ColumnOrder = 1  });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "VENDOR", MetaType = "TEXTBOX", MetaCode = "TB_VENDNAME", DataMetaCode = "VENDORNAME", Title = "Vendor Name", ParentMetaCode = "VNDPNL", RowOrder = 2, ColumnOrder = 1  });

            //LISTVIEW
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "VENDOR", MetaType = "LISTVIEW", MetaCode = "MAIN_LISTVIEW", DataMetaCode = "", Title = "Vendor List", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "VENDOR", MetaType = "LISTVIEWFIELD", MetaCode = "LV_ID", DataMetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 1  });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "VENDOR", MetaType = "LISTVIEWFIELD", MetaCode = "LV_VID", DataMetaCode = "VENDORID", Title = "Vendor ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 2  });
            context.Set<MetaUIItem>().Add(new MetaUIItem() { AppMetaCode = "VENDOR", MetaType = "LISTVIEWFIELD", MetaCode = "LV_VNAME", DataMetaCode = "VENDORNAME", Title = "Vendor Name", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 3  });

          

            context.SaveChanges();
        }

        private static void SeedValueDomains(ApplicationDbContext context, bool isupdate)
        {
            if (isupdate)
            {
                context.Set<ValueDomain>().RemoveRange(context.Set<ValueDomain>());
                context.SaveChanges();
            }

            //ITEMCATEGORY
            context.Set<ValueDomain>().Add(new ValueDomain() { DomainName= "ITEMCATEGORY", Code = "A1", Value = "Primary" });
            context.Set<ValueDomain>().Add(new ValueDomain() { DomainName = "ITEMCATEGORY", Code = "A2", Value = "Secondary" });

            //LOCATIONTYPE
            context.Set<ValueDomain>().Add(new ValueDomain() { DomainName = "LOCATIONTYPE", Code = "LOC001", Value = "Warehouse" });


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
