/// <summary>
/// Generates seeding code for the current model
/// This function should be used in a class that inherits IntwentySeeder and implements IIntwentySeeder
/// </summary>
public override void SeedModel()
{

//Systems
var systems = new List<SystemItem>();
systems.Add(new SystemItem() { MetaCode = "INTWENTYDEFAULTSYS", Title = "Default", DbPrefix = "def" });
systems.Add(new SystemItem() { MetaCode = "WAREHOUSE", Title = "WMS", DbPrefix = "wms" });
systems.Add(new SystemItem() { MetaCode = "BLOG", Title = "The blog engine", DbPrefix = "blog" });

//Applications
var applications = new List<ApplicationItem>();
applications.Add(new ApplicationItem() { Id = 10, Description = "An app for managing customers", SystemMetaCode = "WAREHOUSE", MetaCode = "CUSTOMER", Title = "Customer", TitleLocalizationKey = "CUSTOMER", DbName = "wms_Customer", DataMode = 0, UseVersioning = False, TenantIsolationLevel = 0, TenantIsolationMethod = 0 });
applications.Add(new ApplicationItem() { Id = 20, Description = "An app for managing items", SystemMetaCode = "WAREHOUSE", MetaCode = "ITEM", Title = "Item", TitleLocalizationKey = "ITEM", DbName = "wms_Item", DataMode = 0, UseVersioning = False, TenantIsolationLevel = 0, TenantIsolationMethod = 0 });
applications.Add(new ApplicationItem() { Id = 30, Description = "An app for managing sales orders", SystemMetaCode = "WAREHOUSE", MetaCode = "SALESORDER", Title = "Sales Order", TitleLocalizationKey = "SALESORDER", DbName = "wms_SalesHeader", DataMode = 0, UseVersioning = False, TenantIsolationLevel = 2, TenantIsolationMethod = 2 });
applications.Add(new ApplicationItem() { Id = 40, Description = "An app for managing vendors", SystemMetaCode = "WAREHOUSE", MetaCode = "VENDOR", Title = "Vendor", TitleLocalizationKey = "VENDOR", DbName = "wms_Vendor", DataMode = 0, UseVersioning = False, TenantIsolationLevel = 0, TenantIsolationMethod = 0 });
applications.Add(new ApplicationItem() { Id = 50, Description = "An app for blogging", SystemMetaCode = "BLOG", MetaCode = "BLOGAPP", Title = "The blog", TitleLocalizationKey = "", DbName = "blog_Blog", DataMode = 0, UseVersioning = False, TenantIsolationLevel = 0, TenantIsolationMethod = 0 });


var dbitems = new List<DatabaseItem>();
var views = new List<ViewItem>();
var userinterface = new List<UserInterfaceItem>();
var functions = new List<FunctionItem>();
var userinterfacestructure = new List<UserInterfaceStructureItem>();

//Application - Customer
//Database
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMERID", DbName = "CustomerId", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "DEFVALUE=AUTO#DEFVALUE_START=1000#DEFVALUE_PREFIX=CUST#DEFVALUE_SEED=100#UNIQUE=TRUE#MANDATORY=TRUE" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMERNAME", DbName = "CustomerName", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMERPHONE", DbName = "CustomerPhone", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMEREMAIL", DbName = "CustomerEmail", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "MANDATORY=TRUE" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMERSTATUS", DbName = "CustomerStatus", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMERTAGS", DbName = "CustomerTags", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMERTAGSTEXT", DbName = "CustomerTagsText", ParentMetaCode = "ROOT", DataType = "TEXT", Properties = "" });

//Views
views.Add(new ViewItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaCode = "LV_CUSTOMER1", MetaType = "UIVIEW", Title = "Customer List", TitleLocalizationKey = "CUSTOMERLIST", Path = "Customers/List/{requestinfo}", IsPrimary = True, IsPublic = False });
views.Add(new ViewItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaCode = "INPUT_CUSTOMER1", MetaType = "UIVIEW", Title = "Create Customer", TitleLocalizationKey = "CUST_ADD", Path = "Customers/Create/{requestinfo}", IsPrimary = False, IsPublic = False });
views.Add(new ViewItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaCode = "INPUT_CUSTOMER2", MetaType = "UIVIEW", Title = "Edit Customer", TitleLocalizationKey = "CUST_EDIT", Path = "Customers/Edit/{id}/{requestinfo}", IsPrimary = False, IsPublic = False });

//Userinterface
userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", ViewMetaCode = "INPUT_CUSTOMER1", MetaCode = "CUST_INPUTVIEW", MetaType = "INPUTINTERFACE", DataTableMetaCode = "CUSTOMER" });
userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", ViewMetaCode = "INPUT_CUSTOMER2", MetaCode = "CUST_INPUTVIEW", MetaType = "INPUTINTERFACE", DataTableMetaCode = "CUSTOMER" });
userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", ViewMetaCode = "LV_CUSTOMER1", MetaCode = "CUST_LISTVIEW", MetaType = "LISTINTERFACE", DataTableMetaCode = "CUSTOMER" });

//UI Components
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "CUSTOMER", UserInterfaceMetaCode = "CUST_INPUTVIEW", MetaType = "SECTION", MetaCode = "MAINSECTION", Title = "", TitleLocalizationKey="" ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "CUSTOMER", UserInterfaceMetaCode = "CUST_INPUTVIEW", MetaType = "PANEL", MetaCode = "CUSTPNL1", Title = "Basics", TitleLocalizationKey="CUSTOMERBASIC" ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "CUSTOMER", UserInterfaceMetaCode = "CUST_INPUTVIEW", MetaType = "TEXTBOX", MetaCode = "TB_CUSTID", Title = "Customer ID", TitleLocalizationKey="CUSTOMERID" ParentMetaCode = "CUSTPNL1", RowOrder = 1, ColumnOrder = 1, Properties = "READONLY=TRUE" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "CUSTOMER", UserInterfaceMetaCode = "CUST_INPUTVIEW", MetaType = "TEXTBOX", MetaCode = "TB_CUSTNAME", Title = "Customer Name", TitleLocalizationKey="CUSTOMERNAME" ParentMetaCode = "CUSTPNL1", RowOrder = 2, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "CUSTOMER", UserInterfaceMetaCode = "CUST_INPUTVIEW", MetaType = "SEARCHBOX", MetaCode = "MS_TAGS", Title = "Tags", TitleLocalizationKey="MS_TAGS" ParentMetaCode = "CUSTPNL1", RowOrder = 3, ColumnOrder = 1, Properties = "MULTISELECT=TRUE#ALLOWCREATE=TRUE#USESEARCH=TRUE" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "CUSTOMER", UserInterfaceMetaCode = "CUST_INPUTVIEW", MetaType = "PANEL", MetaCode = "CUSTPNL2", Title = "Contact", TitleLocalizationKey="CUSTOMERCONTACT" ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 2, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "CUSTOMER", UserInterfaceMetaCode = "CUST_INPUTVIEW", MetaType = "EMAILBOX", MetaCode = "TBCUSTMAIL", Title = "Email", TitleLocalizationKey="CUSTOMEREMAIL" ParentMetaCode = "CUSTPNL2", RowOrder = 1, ColumnOrder = 2, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "CUSTOMER", UserInterfaceMetaCode = "CUST_INPUTVIEW", MetaType = "NUMBOX", MetaCode = "TBCUSTPHONE", Title = "Phone", TitleLocalizationKey="CUSTOMERPHONE" ParentMetaCode = "CUSTPNL2", RowOrder = 2, ColumnOrder = 2, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "CUSTOMER", UserInterfaceMetaCode = "CUST_INPUTVIEW", MetaType = "TEXTBOX", MetaCode = "TBCUSTSTAT", Title = "Status", TitleLocalizationKey="" ParentMetaCode = "CUSTPNL2", RowOrder = 3, ColumnOrder = 2, Properties = "READONLY=TRUE" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "CUSTOMER", UserInterfaceMetaCode = "CUST_LISTVIEW", MetaType = "TABLE", MetaCode = "CUST_LISTVIEW", Title = "Customer List", TitleLocalizationKey="CUSTOMERLIST" ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "CUSTOMER", UserInterfaceMetaCode = "CUST_LISTVIEW", MetaType = "TABLETEXTCOLUMN", MetaCode = "LV_ID", Title = "ID", TitleLocalizationKey="" ParentMetaCode = "CUST_LISTVIEW", RowOrder = 1, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "CUSTOMER", UserInterfaceMetaCode = "CUST_LISTVIEW", MetaType = "TABLETEXTCOLUMN", MetaCode = "LV_CUSTID", Title = "Customer ID", TitleLocalizationKey="CUSTOMERID" ParentMetaCode = "CUST_LISTVIEW", RowOrder = 1, ColumnOrder = 2, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "CUSTOMER", UserInterfaceMetaCode = "CUST_LISTVIEW", MetaType = "TABLETEXTCOLUMN", MetaCode = "LV_CUSTNAME", Title = "Customer Name", TitleLocalizationKey="CUSTOMERNAME" ParentMetaCode = "CUST_LISTVIEW", RowOrder = 1, ColumnOrder = 3, Properties = "" });

//Functions
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "CUSTOMER", OwnerMetaCode = "CUST_LISTVIEW", OwnerMetaType = "LISTINTERFACE", MetaType = "CREATE", MetaCode = "CUST_FUNC_CREATE", ActionPath = "", ActionMetaCode = "INPUT_CUSTOMER1", ActionMetaType = "UIVIEW", IsModalAction = False, Title = "Create" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "CUSTOMER", OwnerMetaCode = "CUST_LISTVIEW", OwnerMetaType = "LISTINTERFACE", MetaType = "EDIT", MetaCode = "CUST_FUNC_EDIT", ActionPath = "", ActionMetaCode = "INPUT_CUSTOMER2", ActionMetaType = "UIVIEW", IsModalAction = False, Title = "Edit" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "CUSTOMER", OwnerMetaCode = "CUST_LISTVIEW", OwnerMetaType = "LISTINTERFACE", MetaType = "DELETE", MetaCode = "CUST_FUNC_DELETE", ActionPath = "", ActionMetaCode = "", ActionMetaType = "", IsModalAction = False, Title = "Delete" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "CUSTOMER", OwnerMetaCode = "CUST_LISTVIEW", OwnerMetaType = "LISTINTERFACE", MetaType = "FILTER", MetaCode = "CUST_FUNC_FILTER", ActionPath = "", ActionMetaCode = "", ActionMetaType = "", IsModalAction = False, Title = "Filter" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "CUSTOMER", OwnerMetaCode = "CUST_LISTVIEW", OwnerMetaType = "LISTINTERFACE", MetaType = "PAGING", MetaCode = "CUST_FUNC_PAGING", ActionPath = "", ActionMetaCode = "", ActionMetaType = "", IsModalAction = False, Title = "Paging" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "CUSTOMER", OwnerMetaCode = "INPUT_CUSTOMER1", OwnerMetaType = "UIVIEW", MetaType = "SAVE", MetaCode = "CUST_FUNC_SAVE", ActionPath = "", ActionMetaCode = "", ActionMetaType = "", IsModalAction = False, Title = "Save" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "CUSTOMER", OwnerMetaCode = "INPUT_CUSTOMER1", OwnerMetaType = "UIVIEW", MetaType = "NAVIGATE", MetaCode = "CUST_FUNC_BACK", ActionPath = "", ActionMetaCode = "LV_CUSTOMER1", ActionMetaType = "UIVIEW", IsModalAction = False, Title = "Back to list" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "CUSTOMER", OwnerMetaCode = "INPUT_CUSTOMER2", OwnerMetaType = "UIVIEW", MetaType = "SAVE", MetaCode = "CUST_FUNC_SAVE1", ActionPath = "", ActionMetaCode = "", ActionMetaType = "", IsModalAction = False, Title = "Save" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "CUSTOMER", OwnerMetaCode = "INPUT_CUSTOMER2", OwnerMetaType = "UIVIEW", MetaType = "NAVIGATE", MetaCode = "CUST_FUNC_BACK1", ActionPath = "", ActionMetaCode = "LV_CUSTOMER1", ActionMetaType = "UIVIEW", IsModalAction = False, Title = "Back to list" });

//Application - Item
//Database
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "ITEMID", DbName = "ItemId", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "DEFVALUE=AUTO#DEFVALUE_START=1000#DEFVALUE_PREFIX=ITEM#DEFVALUE_SEED=100#UNIQUE=TRUE#MANDATORY=TRUE" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "NAME", DbName = "ItemName", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "CATCODE", DbName = "ItemCategoryCode", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "CATVALUE", DbName = "ItemCategoryValue", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "MODIFIED", DbName = "Modified", ParentMetaCode = "ROOT", DataType = "DATETIME", Properties = "" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "ACTIVE", DbName = "Active", ParentMetaCode = "ROOT", DataType = "BOOLEAN", Properties = "" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "PURCHASEPRICE", DbName = "PurchasePrice", ParentMetaCode = "ROOT", DataType = "2DECIMAL", Properties = "" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "SALESPRICE", DbName = "SalesPrice", ParentMetaCode = "ROOT", DataType = "2DECIMAL", Properties = "" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "VENDORCODE", DbName = "VendorCode", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaType = "DATACOLUMN", MetaCode = "VENDORTXT", DbName = "VendorName", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "" });

//Views
views.Add(new ViewItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaCode = "ITEM_LISTVIEW", MetaType = "UIVIEW", Title = "Item List", TitleLocalizationKey = "ITEMLIST", Path = "Items/List", IsPrimary = True, IsPublic = False });
views.Add(new ViewItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaCode = "ITEM_CREATE_VIEW", MetaType = "UIVIEW", Title = "Create Item", TitleLocalizationKey = "", Path = "Items/Create", IsPrimary = False, IsPublic = False });
views.Add(new ViewItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", MetaCode = "ITEM_EDIT_VIEW", MetaType = "UIVIEW", Title = "Edit Item", TitleLocalizationKey = "", Path = "Items/Edit/{id}", IsPrimary = False, IsPublic = False });

//Userinterface
userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", ViewMetaCode = "ITEM_CREATE_VIEW", MetaCode = "ITEMINPUTUI", MetaType = "INPUTINTERFACE", DataTableMetaCode = "ITEM" });
userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", ViewMetaCode = "ITEM_EDIT_VIEW", MetaCode = "ITEMINPUTUI", MetaType = "INPUTINTERFACE", DataTableMetaCode = "ITEM" });
userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "ITEM", ViewMetaCode = "ITEM_LISTVIEW", MetaCode = "ITEMLISTUI", MetaType = "LISTINTERFACE", DataTableMetaCode = "ITEM" });

//UI Components
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMINPUTUI", MetaType = "SECTION", MetaCode = "MAINSECTION", Title = "Basics", TitleLocalizationKey="" ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMINPUTUI", MetaType = "PANEL", MetaCode = "ITMPNL_A", Title = "Basics", TitleLocalizationKey="" ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMINPUTUI", MetaType = "TEXTBOX", MetaCode = "TB_ITID", Title = "Item ID", TitleLocalizationKey="" ParentMetaCode = "ITMPNL_A", RowOrder = 1, ColumnOrder = 1, Properties = "READONLY=TRUE" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMINPUTUI", MetaType = "TEXTBOX", MetaCode = "TB_ITNAME", Title = "Item Name", TitleLocalizationKey="" ParentMetaCode = "ITMPNL_A", RowOrder = 2, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMINPUTUI", MetaType = "DATEPICKER", MetaCode = "DP_MOD", Title = "Modified Date", TitleLocalizationKey="" ParentMetaCode = "ITMPNL_A", RowOrder = 3, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMINPUTUI", MetaType = "CHECKBOX", MetaCode = "CB_ACTIVE", Title = "Is Active", TitleLocalizationKey="" ParentMetaCode = "ITMPNL_A", RowOrder = 4, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMINPUTUI", MetaType = "PANEL", MetaCode = "ITMPNL_B", Title = "Basics", TitleLocalizationKey="" ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 2, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMINPUTUI", MetaType = "COMBOBOX", MetaCode = "CB_CATEGORY", Title = "Category", TitleLocalizationKey="" ParentMetaCode = "ITMPNL_B", RowOrder = 1, ColumnOrder = 2, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMINPUTUI", MetaType = "NUMBOX", MetaCode = "NUMBOX_PURCHPRICE", Title = "Purchase Price", TitleLocalizationKey="" ParentMetaCode = "ITMPNL_B", RowOrder = 2, ColumnOrder = 2, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMINPUTUI", MetaType = "NUMBOX", MetaCode = "NUMBOX_SALESPRICE", Title = "Sales Pricee", TitleLocalizationKey="" ParentMetaCode = "ITMPNL_B", RowOrder = 3, ColumnOrder = 2, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMINPUTUI", MetaType = "SEARCHBOX", MetaCode = "LOOKUPVEND", Title = "Vendor", TitleLocalizationKey="" ParentMetaCode = "ITMPNL_B", RowOrder = 4, ColumnOrder = 2, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMLISTUI", MetaType = "TABLE", MetaCode = "MAIN_EDITLISTVIEW", Title = "Item List", TitleLocalizationKey="ITEMLIST" ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMLISTUI", MetaType = "TABLETEXTCOLUMN", MetaCode = "LV_ID", Title = "ID", TitleLocalizationKey="" ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMLISTUI", MetaType = "TABLETEXTCOLUMN", MetaCode = "LV_ITEMID", Title = "Item ID", TitleLocalizationKey="" ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 2, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "ITEM", UserInterfaceMetaCode = "ITEMLISTUI", MetaType = "TABLETEXTCOLUMN", MetaCode = "LV_ITEMNAME", Title = "Item Name", TitleLocalizationKey="" ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 3, Properties = "" });

//Functions
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "ITEM", OwnerMetaCode = "ITEM_CREATE_VIEW", OwnerMetaType = "UIVIEW", MetaType = "SAVE", MetaCode = "ITEM_FUNC_SAVE", ActionPath = "", ActionMetaCode = "", ActionMetaType = "", IsModalAction = False, Title = "Save" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "ITEM", OwnerMetaCode = "ITEM_CREATE_VIEW", OwnerMetaType = "UIVIEW", MetaType = "NAVIGATE", MetaCode = "ITEM_FUNC_BACK", ActionPath = "Items/List", ActionMetaCode = "ITEM_LISTVIEW", ActionMetaType = "UIVIEW", IsModalAction = False, Title = "Back to list" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "ITEM", OwnerMetaCode = "ITEM_EDIT_VIEW", OwnerMetaType = "UIVIEW", MetaType = "SAVE", MetaCode = "ITEM_FUNC_SAVE1", ActionPath = "", ActionMetaCode = "", ActionMetaType = "", IsModalAction = False, Title = "Save" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "ITEM", OwnerMetaCode = "ITEM_EDIT_VIEW", OwnerMetaType = "UIVIEW", MetaType = "NAVIGATE", MetaCode = "ITEM_FUNC_BACK1", ActionPath = "Items/List", ActionMetaCode = "ITEM_LISTVIEW", ActionMetaType = "UIVIEW", IsModalAction = False, Title = "Back to list" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "ITEM", OwnerMetaCode = "ITEMLISTUI", OwnerMetaType = "LISTINTERFACE", MetaType = "CREATE", MetaCode = "ITEM_FUNC_CREATE", ActionPath = "Items/Create", ActionMetaCode = "ITEM_CREATE_VIEW", ActionMetaType = "UIVIEW", IsModalAction = False, Title = "Create" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "ITEM", OwnerMetaCode = "ITEMLISTUI", OwnerMetaType = "LISTINTERFACE", MetaType = "EDIT", MetaCode = "ITEM_FUNC_EDIT", ActionPath = "Items/Edit/{id}", ActionMetaCode = "ITEM_EDIT_VIEW", ActionMetaType = "UIVIEW", IsModalAction = False, Title = "Edit" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "ITEM", OwnerMetaCode = "ITEMLISTUI", OwnerMetaType = "LISTINTERFACE", MetaType = "DELETE", MetaCode = "ITEM_FUNC_DELETE", ActionPath = "", ActionMetaCode = "", ActionMetaType = "", IsModalAction = False, Title = "Delete" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "ITEM", OwnerMetaCode = "ITEMLISTUI", OwnerMetaType = "LISTINTERFACE", MetaType = "FILTER", MetaCode = "ITEM_FUNC_FILTER", ActionPath = "", ActionMetaCode = "", ActionMetaType = "", IsModalAction = False, Title = "Filter" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "ITEM", OwnerMetaCode = "ITEMLISTUI", OwnerMetaType = "LISTINTERFACE", MetaType = "PAGING", MetaCode = "ITEM_FUNC_PAGING", ActionPath = "", ActionMetaCode = "", ActionMetaType = "", IsModalAction = False, Title = "Paging" });

//Application - Sales Order
//Database
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "ORDERNO", DbName = "OrderNo", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "DEFVALUE=AUTO#DEFVALUE_START=1000#DEFVALUE_PREFIX=SO#DEFVALUE_SEED=100#UNIQUE=TRUE#MANDATORY=TRUE" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "ORDERDATE", DbName = "OrderDate", ParentMetaCode = "ROOT", DataType = "DATETIME", Properties = "" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "CUSTID", DbName = "CustomerId", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "CUSTNAME", DbName = "CustomerName", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "DATATABLE", MetaCode = "DTORDLINE", DbName = "wms_SalesLine", ParentMetaCode = "ROOT", DataType = "", Properties = "" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "DTORDLINE_ITEMID", DbName = "ItemNo", ParentMetaCode = "DTORDLINE", DataType = "STRING", Properties = "" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "DTORDLINE_ITEMNAME", DbName = "ItemName", ParentMetaCode = "DTORDLINE", DataType = "STRING", Properties = "" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "DTORDLINE_QTY", DbName = "Qty", ParentMetaCode = "DTORDLINE", DataType = "INTEGER", Properties = "" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaType = "DATACOLUMN", MetaCode = "DTORDLINE_ORDERNO", DbName = "OrderNo", ParentMetaCode = "DTORDLINE", DataType = "STRING", Properties = "" });

//Views
views.Add(new ViewItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaCode = "SO_LISTVIEW", MetaType = "UIVIEW", Title = "Sales Orders", TitleLocalizationKey = "SALESORDERLIST", Path = "SalesOrders/List", IsPrimary = True, IsPublic = False });
views.Add(new ViewItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaCode = "SO_CREATE_VIEW", MetaType = "UIVIEW", Title = "Create Sales Order", TitleLocalizationKey = "", Path = "SalesOrders/Create", IsPrimary = False, IsPublic = False });
views.Add(new ViewItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", MetaCode = "SO_EDIT_VIEW", MetaType = "UIVIEW", Title = "Edit Sales Order", TitleLocalizationKey = "", Path = "SalesOrders/Edit/{id}", IsPrimary = False, IsPublic = False });

//Userinterface
userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", ViewMetaCode = "SO_CREATE_VIEW", MetaCode = "SO_INPUTUI", MetaType = "INPUTINTERFACE", DataTableMetaCode = "SALESORDER" });
userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", ViewMetaCode = "SO_EDIT_VIEW", MetaCode = "SO_INPUTUI", MetaType = "INPUTINTERFACE", DataTableMetaCode = "SALESORDER" });
userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", ViewMetaCode = "SO_EDIT_VIEW", MetaCode = "SO_ORDLINE_UI", MetaType = "LISTINTERFACE", DataTableMetaCode = "DTORDLINE" });
userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", ViewMetaCode = "SO_EDIT_VIEW", MetaCode = "SO_CREATE_ORDLINE_UI", MetaType = "INPUTINTERFACE", DataTableMetaCode = "DTORDLINE" });
userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", ViewMetaCode = "SO_EDIT_VIEW", MetaCode = "SO_EDIT_ORDLINE_UI", MetaType = "INPUTINTERFACE", DataTableMetaCode = "DTORDLINE" });
userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "SALESORDER", ViewMetaCode = "SO_LISTVIEW", MetaCode = "SO_LISTUI", MetaType = "LISTINTERFACE", DataTableMetaCode = "SALESORDER" });

//UI Components
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", UserInterfaceMetaCode = "SO_CREATE_ORDLINE_UI", MetaType = "SECTION", MetaCode = "SECT_CRORDLINE_HDR", Title = "Create Order Line", TitleLocalizationKey="CRORDLINE" ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", UserInterfaceMetaCode = "SO_CREATE_ORDLINE_UI", MetaType = "PANEL", MetaCode = "CR_ORDLINE_PANEL", Title = "", TitleLocalizationKey="" ParentMetaCode = "SECT_CRORDLINE_HDR", RowOrder = 1, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", UserInterfaceMetaCode = "SO_CREATE_ORDLINE_UI", MetaType = "SEARCHBOX", MetaCode = "CR_TB_LINEITEM", Title = "Item", TitleLocalizationKey="" ParentMetaCode = "CR_ORDLINE_PANEL", RowOrder = 1, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", UserInterfaceMetaCode = "SO_CREATE_ORDLINE_UI", MetaType = "NUMBOX", MetaCode = "CR_TB_LINEQTY", Title = "Qty", TitleLocalizationKey="" ParentMetaCode = "CR_ORDLINE_PANEL", RowOrder = 2, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", UserInterfaceMetaCode = "SO_EDIT_ORDLINE_UI", MetaType = "SECTION", MetaCode = "SECT_ORDLINE_HDR", Title = "Edit Order Line", TitleLocalizationKey="EDORDLINE" ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", UserInterfaceMetaCode = "SO_EDIT_ORDLINE_UI", MetaType = "PANEL", MetaCode = "ORDLINE_PANEL", Title = "", TitleLocalizationKey="" ParentMetaCode = "SECT_ORDLINE_HDR", RowOrder = 1, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", UserInterfaceMetaCode = "SO_EDIT_ORDLINE_UI", MetaType = "SEARCHBOX", MetaCode = "TB_LINEITEM", Title = "Item", TitleLocalizationKey="" ParentMetaCode = "ORDLINE_PANEL", RowOrder = 1, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", UserInterfaceMetaCode = "SO_EDIT_ORDLINE_UI", MetaType = "NUMBOX", MetaCode = "TB_LINEQTY", Title = "Qty", TitleLocalizationKey="" ParentMetaCode = "ORDLINE_PANEL", RowOrder = 2, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", UserInterfaceMetaCode = "SO_INPUTUI", MetaType = "SECTION", MetaCode = "SECT_HDR", Title = "Sales Header", TitleLocalizationKey="" ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", UserInterfaceMetaCode = "SO_INPUTUI", MetaType = "PANEL", MetaCode = "PANEL1", Title = "", TitleLocalizationKey="" ParentMetaCode = "SECT_HDR", RowOrder = 1, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", UserInterfaceMetaCode = "SO_INPUTUI", MetaType = "TEXTBOX", MetaCode = "TB_ORDERNO", Title = "Order No", TitleLocalizationKey="" ParentMetaCode = "PANEL1", RowOrder = 1, ColumnOrder = 1, Properties = "READONLY=TRUE" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", UserInterfaceMetaCode = "SO_INPUTUI", MetaType = "DATEPICKER", MetaCode = "DP_ORDERDATE", Title = "Order Date", TitleLocalizationKey="" ParentMetaCode = "PANEL1", RowOrder = 2, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", UserInterfaceMetaCode = "SO_INPUTUI", MetaType = "SEARCHBOX", MetaCode = "LOOKUP_CUSTOMER", Title = "Customer", TitleLocalizationKey="" ParentMetaCode = "PANEL1", RowOrder = 3, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", UserInterfaceMetaCode = "SO_LISTUI", MetaType = "TABLE", MetaCode = "MAIN_EDITLISTVIEW", Title = "Sales Orders", TitleLocalizationKey="SALESORDERLIST" ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", UserInterfaceMetaCode = "SO_LISTUI", MetaType = "TABLETEXTCOLUMN", MetaCode = "LV_ID", Title = "ID", TitleLocalizationKey="" ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", UserInterfaceMetaCode = "SO_LISTUI", MetaType = "TABLETEXTCOLUMN", MetaCode = "LF_ORDERID", Title = "Order No", TitleLocalizationKey="" ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 2, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", UserInterfaceMetaCode = "SO_LISTUI", MetaType = "TABLETEXTCOLUMN", MetaCode = "LF_CUSTNAME", Title = "Customer", TitleLocalizationKey="" ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 3, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", UserInterfaceMetaCode = "SO_ORDLINE_UI", MetaType = "TABLE", MetaCode = "ORDERLINE_LISTVIEW", Title = "Order Lines", TitleLocalizationKey="ORDLINES" ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", UserInterfaceMetaCode = "SO_ORDLINE_UI", MetaType = "TABLETEXTCOLUMN", MetaCode = "OL_ITEMID", Title = "Item", TitleLocalizationKey="" ParentMetaCode = "ORDERLINE_LISTVIEW", RowOrder = 1, ColumnOrder = 2, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", UserInterfaceMetaCode = "SO_ORDLINE_UI", MetaType = "TABLETEXTCOLUMN", MetaCode = "OL_ITEMNAME", Title = "Name", TitleLocalizationKey="" ParentMetaCode = "ORDERLINE_LISTVIEW", RowOrder = 1, ColumnOrder = 3, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", UserInterfaceMetaCode = "SO_ORDLINE_UI", MetaType = "TABLETEXTCOLUMN", MetaCode = "OL_QTY", Title = "Qty", TitleLocalizationKey="" ParentMetaCode = "ORDERLINE_LISTVIEW", RowOrder = 1, ColumnOrder = 4, Properties = "" });

//Functions
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", OwnerMetaCode = "SO_CREATE_VIEW", OwnerMetaType = "UIVIEW", MetaType = "SAVE", MetaCode = "SO_FUNC_SAVE", ActionPath = "", ActionMetaCode = "", ActionMetaType = "", IsModalAction = False, Title = "Save" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", OwnerMetaCode = "SO_CREATE_VIEW", OwnerMetaType = "UIVIEW", MetaType = "NAVIGATE", MetaCode = "SO_FUNC_BACK", ActionPath = "SalesOrders/List", ActionMetaCode = "SO_LISTVIEW", ActionMetaType = "UIVIEW", IsModalAction = False, Title = "Back to list" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", OwnerMetaCode = "SO_EDIT_VIEW", OwnerMetaType = "UIVIEW", MetaType = "SAVE", MetaCode = "SO_FUNC_SAVE1", ActionPath = "", ActionMetaCode = "", ActionMetaType = "", IsModalAction = False, Title = "Save" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", OwnerMetaCode = "SO_EDIT_VIEW", OwnerMetaType = "UIVIEW", MetaType = "NAVIGATE", MetaCode = "SO_FUNC_BACK1", ActionPath = "SalesOrders/List", ActionMetaCode = "SO_LISTVIEW", ActionMetaType = "UIVIEW", IsModalAction = False, Title = "Back to list" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", OwnerMetaCode = "SO_LISTUI", OwnerMetaType = "LISTINTERFACE", MetaType = "CREATE", MetaCode = "SO_FUNC_CREATE", ActionPath = "SalesOrders/Create", ActionMetaCode = "SO_CREATE_VIEW", ActionMetaType = "UIVIEW", IsModalAction = False, Title = "Create" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", OwnerMetaCode = "SO_LISTUI", OwnerMetaType = "LISTINTERFACE", MetaType = "EDIT", MetaCode = "SO_FUNC_EDIT", ActionPath = "SalesOrders/Edit/{id}", ActionMetaCode = "SO_EDIT_VIEW", ActionMetaType = "UIVIEW", IsModalAction = False, Title = "Edit" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", OwnerMetaCode = "SO_LISTUI", OwnerMetaType = "LISTINTERFACE", MetaType = "DELETE", MetaCode = "SO_FUNC_DELETE", ActionPath = "", ActionMetaCode = "", ActionMetaType = "", IsModalAction = False, Title = "Delete" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", OwnerMetaCode = "SO_LISTUI", OwnerMetaType = "LISTINTERFACE", MetaType = "FILTER", MetaCode = "SO_FUNC_FILTER", ActionPath = "", ActionMetaCode = "", ActionMetaType = "", IsModalAction = False, Title = "Filter" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", OwnerMetaCode = "SO_LISTUI", OwnerMetaType = "LISTINTERFACE", MetaType = "PAGING", MetaCode = "SO_FUNC_PAGING", ActionPath = "", ActionMetaCode = "", ActionMetaType = "", IsModalAction = False, Title = "Paging" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", OwnerMetaCode = "SO_ORDLINE_UI", OwnerMetaType = "LISTINTERFACE", MetaType = "CREATE", MetaCode = "OL_FUNC_CREATE", ActionPath = "", ActionMetaCode = "SO_CREATE_ORDLINE_UI", ActionMetaType = "INPUTINTERFACE", IsModalAction = True, Title = "Create" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", OwnerMetaCode = "SO_ORDLINE_UI", OwnerMetaType = "LISTINTERFACE", MetaType = "EDIT", MetaCode = "OL_FUNC_EDIT", ActionPath = "", ActionMetaCode = "SO_EDIT_ORDLINE_UI", ActionMetaType = "INPUTINTERFACE", IsModalAction = True, Title = "Edit" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", OwnerMetaCode = "SO_ORDLINE_UI", OwnerMetaType = "LISTINTERFACE", MetaType = "DELETE", MetaCode = "OL_FUNC_DELETE", ActionPath = "", ActionMetaCode = "", ActionMetaType = "", IsModalAction = False, Title = "Delete" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", OwnerMetaCode = "SO_ORDLINE_UI", OwnerMetaType = "LISTINTERFACE", MetaType = "FILTER", MetaCode = "OL_FUNC_FILTER", ActionPath = "", ActionMetaCode = "", ActionMetaType = "", IsModalAction = False, Title = "Filter" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "SALESORDER", OwnerMetaCode = "SO_ORDLINE_UI", OwnerMetaType = "LISTINTERFACE", MetaType = "PAGING", MetaCode = "OL_FUNC_PAGING", ActionPath = "", ActionMetaCode = "", ActionMetaType = "", IsModalAction = False, Title = "Paging" });

//Application - Vendor
//Database
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", MetaType = "DATACOLUMN", MetaCode = "VENDORID", DbName = "VendorId", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "DEFVALUE=AUTO#DEFVALUE_START=1000#DEFVALUE_PREFIX=VEND#DEFVALUE_SEED=100#UNIQUE=TRUE#MANDATORY=TRUE" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", MetaType = "DATACOLUMN", MetaCode = "VENDORNAME", DbName = "VendorName", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", MetaType = "DATACOLUMN", MetaCode = "H7314920PMKGB8ZHEUTU", DbName = "IsValidated", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", MetaType = "DATACOLUMN", MetaCode = "9YW15546PMBPNB2BKQHW", DbName = "PrioValue", ParentMetaCode = "ROOT", DataType = "TEXT", Properties = "" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", MetaType = "DATACOLUMN", MetaCode = "T3D15546PMR3OMIUSBWN", DbName = "PrioText", ParentMetaCode = "ROOT", DataType = "TEXT", Properties = "" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", MetaType = "DATACOLUMN", MetaCode = "WXR15630PMDJCUCIKCNH", DbName = "AttrId", ParentMetaCode = "ROOT", DataType = "TEXT", Properties = "" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", MetaType = "DATACOLUMN", MetaCode = "IQ615630PMPKUOSPUYPX", DbName = "AttrText", ParentMetaCode = "ROOT", DataType = "TEXT", Properties = "" });

//Views
views.Add(new ViewItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", MetaCode = "VEND_LISTVIEW", MetaType = "UIVIEW", Title = "Vendor List", TitleLocalizationKey = "VENDORLIST", Path = "Vendors/List", IsPrimary = True, IsPublic = False });
views.Add(new ViewItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", MetaCode = "VEND_CREATE_VIEW", MetaType = "UIVIEW", Title = "Create Vendor", TitleLocalizationKey = "", Path = "Vendors/Create", IsPrimary = False, IsPublic = False });
views.Add(new ViewItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", MetaCode = "VEND_EDIT_VIEW", MetaType = "UIVIEW", Title = "Edit Vendor", TitleLocalizationKey = "", Path = "Vendors/Edit/{id}", IsPrimary = False, IsPublic = False });

//Userinterface
userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", ViewMetaCode = "VEND_CREATE_VIEW", MetaCode = "VEND_INPUTUI", MetaType = "INPUTINTERFACE", DataTableMetaCode = "VENDOR" });
userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", ViewMetaCode = "VEND_EDIT_VIEW", MetaCode = "VEND_INPUTUI", MetaType = "INPUTINTERFACE", DataTableMetaCode = "VENDOR" });
userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "VENDOR", ViewMetaCode = "VEND_LISTVIEW", MetaCode = "VEND_LISTUI", MetaType = "LISTINTERFACE", DataTableMetaCode = "VENDOR" });

//UI Components
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "VENDOR", UserInterfaceMetaCode = "VEND_INPUTUI", MetaType = "SECTION", MetaCode = "MAINSECTION", Title = "", TitleLocalizationKey="" ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "VENDOR", UserInterfaceMetaCode = "VEND_INPUTUI", MetaType = "PANEL", MetaCode = "PNL1", Title = "Basics", TitleLocalizationKey="UI_LOC_Q5W20526PMYEUPIGESBE" ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "VENDOR", UserInterfaceMetaCode = "VEND_INPUTUI", MetaType = "TEXTBOX", MetaCode = "TB_VENDID", Title = "Vendor ID", TitleLocalizationKey="" ParentMetaCode = "PNL1", RowOrder = 1, ColumnOrder = 1, Properties = "READONLY=TRUE" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "VENDOR", UserInterfaceMetaCode = "VEND_INPUTUI", MetaType = "TEXTBOX", MetaCode = "TB_VENDNAME", Title = "Vendor Name", TitleLocalizationKey="" ParentMetaCode = "PNL1", RowOrder = 2, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "VENDOR", UserInterfaceMetaCode = "VEND_INPUTUI", MetaType = "SECTION", MetaCode = "RKS14920PMFINERTUGP5", Title = "", TitleLocalizationKey="" ParentMetaCode = "ROOT", RowOrder = 2, ColumnOrder = 1, Properties = "STARTEXPANDED=TRUE" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "VENDOR", UserInterfaceMetaCode = "VEND_INPUTUI", MetaType = "PANEL", MetaCode = "ERH14920PMHRGKHSEWOY", Title = "Classification", TitleLocalizationKey="UI_LOC_1N220526PMWWWRFWKABG" ParentMetaCode = "RKS14920PMFINERTUGP5", RowOrder = 1, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "VENDOR", UserInterfaceMetaCode = "VEND_INPUTUI", MetaType = "YESNOUNKNOWN", MetaCode = "NC14920PMLBFEVZECRDO", Title = "Is Validated", TitleLocalizationKey="" ParentMetaCode = "ERH14920PMHRGKHSEWOY", RowOrder = 1, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "VENDOR", UserInterfaceMetaCode = "VEND_INPUTUI", MetaType = "RADIOLIST", MetaCode = "PLX15546PM9JYPUGU6QL", Title = "Priority", TitleLocalizationKey="" ParentMetaCode = "ERH14920PMHRGKHSEWOY", RowOrder = 2, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "VENDOR", UserInterfaceMetaCode = "VEND_INPUTUI", MetaType = "CHECKLIST", MetaCode = "KZT15630PMA9YI7K1WJV", Title = "Attributes", TitleLocalizationKey="" ParentMetaCode = "ERH14920PMHRGKHSEWOY", RowOrder = 3, ColumnOrder = 1, Properties = "ORIENTATION=VERTICAL" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "VENDOR", UserInterfaceMetaCode = "VEND_LISTUI", MetaType = "TABLE", MetaCode = "MAIN_EDITLISTVIEW", Title = "Vendor List", TitleLocalizationKey="" ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "VENDOR", UserInterfaceMetaCode = "VEND_LISTUI", MetaType = "TABLETEXTCOLUMN", MetaCode = "LV_ID", Title = "ID", TitleLocalizationKey="" ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "VENDOR", UserInterfaceMetaCode = "VEND_LISTUI", MetaType = "TABLETEXTCOLUMN", MetaCode = "LV_VENDID", Title = "Vendor ID", TitleLocalizationKey="" ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 2, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "VENDOR", UserInterfaceMetaCode = "VEND_LISTUI", MetaType = "TABLETEXTCOLUMN", MetaCode = "LV_VENDNAME", Title = "Vendor Name", TitleLocalizationKey="" ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 3, Properties = "" });

//Functions
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "VENDOR", OwnerMetaCode = "VEND_CREATE_VIEW", OwnerMetaType = "UIVIEW", MetaType = "SAVE", MetaCode = "VEND_FUNC_SAVE", ActionPath = "", ActionMetaCode = "", ActionMetaType = "", IsModalAction = False, Title = "Save" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "VENDOR", OwnerMetaCode = "VEND_CREATE_VIEW", OwnerMetaType = "UIVIEW", MetaType = "NAVIGATE", MetaCode = "VEND_FUNC_BACK", ActionPath = "Vendors/List", ActionMetaCode = "VEND_LISTVIEW", ActionMetaType = "UIVIEW", IsModalAction = False, Title = "Back to list" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "VENDOR", OwnerMetaCode = "VEND_EDIT_VIEW", OwnerMetaType = "UIVIEW", MetaType = "SAVE", MetaCode = "VEND_FUNC_SAVE1", ActionPath = "", ActionMetaCode = "", ActionMetaType = "", IsModalAction = False, Title = "Save" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "VENDOR", OwnerMetaCode = "VEND_EDIT_VIEW", OwnerMetaType = "UIVIEW", MetaType = "NAVIGATE", MetaCode = "VEND_FUNC_BACK1", ActionPath = "Vendors/List", ActionMetaCode = "VEND_LISTVIEW", ActionMetaType = "UIVIEW", IsModalAction = False, Title = "Back to list" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "VENDOR", OwnerMetaCode = "VEND_LISTUI", OwnerMetaType = "LISTINTERFACE", MetaType = "CREATE", MetaCode = "VEND_FUNC_CREATE", ActionPath = "Vendors/Create", ActionMetaCode = "VEND_CREATE_VIEW", ActionMetaType = "UIVIEW", IsModalAction = False, Title = "Create" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "VENDOR", OwnerMetaCode = "VEND_LISTUI", OwnerMetaType = "LISTINTERFACE", MetaType = "EDIT", MetaCode = "VEND_FUNC_EDIT", ActionPath = "Vendors/Edit/{id}", ActionMetaCode = "VEND_EDIT_VIEW", ActionMetaType = "UIVIEW", IsModalAction = False, Title = "Edit" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "VENDOR", OwnerMetaCode = "VEND_LISTUI", OwnerMetaType = "LISTINTERFACE", MetaType = "DELETE", MetaCode = "VEND_FUNC_DELETE", ActionPath = "", ActionMetaCode = "", ActionMetaType = "", IsModalAction = False, Title = "Delete" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "VENDOR", OwnerMetaCode = "VEND_LISTUI", OwnerMetaType = "LISTINTERFACE", MetaType = "FILTER", MetaCode = "VEND_FUNC_FILTER", ActionPath = "", ActionMetaCode = "", ActionMetaType = "", IsModalAction = False, Title = "Filter" });
functions.Add(new FunctionItem() { SystemMetaCode = "WAREHOUSE",AppMetaCode = "VENDOR", OwnerMetaCode = "VEND_LISTUI", OwnerMetaType = "LISTINTERFACE", MetaType = "PAGING", MetaCode = "VEND_FUNC_PAGING", ActionPath = "", ActionMetaCode = "", ActionMetaType = "", IsModalAction = False, Title = "Paging" });

//Application - The blog
//Database
dbitems.Add(new DatabaseItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "DATACOLUMN", MetaCode = "POSTHEADER", DbName = "ArticleHeader", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "DATACOLUMN", MetaCode = "POSTTEXT", DbName = "ArticleText", ParentMetaCode = "ROOT", DataType = "TEXT", Properties = "" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "DATACOLUMN", MetaCode = "POSTIMAGE", DbName = "ArticleImage", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "DATATABLE", MetaCode = "POSTCOMMENTS", DbName = "blog_ArticleComments", ParentMetaCode = "ROOT", DataType = "STRING", Properties = "" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "DATACOLUMN", MetaCode = "COMMENTHEADER", DbName = "CommentLabel", ParentMetaCode = "POSTCOMMENTS", DataType = "STRING", Properties = "" });
dbitems.Add(new DatabaseItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaType = "DATACOLUMN", MetaCode = "COMMENTTEXT", DbName = "CommentText", ParentMetaCode = "POSTCOMMENTS", DataType = "TEXT", Properties = "" });

//Views
views.Add(new ViewItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaCode = "BLOG_LISTVIEW", MetaType = "UIVIEW", Title = "Blog Posts", TitleLocalizationKey = "", Path = "/Blog/List", IsPrimary = True, IsPublic = False });
views.Add(new ViewItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaCode = "BLOG_CREATE_VIEW", MetaType = "UIVIEW", Title = "Create Blog Post", TitleLocalizationKey = "", Path = "/Blog/Create", IsPrimary = False, IsPublic = False });
views.Add(new ViewItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaCode = "BLOG_EDIT_VIEW", MetaType = "UIVIEW", Title = "Edit Blog Post", TitleLocalizationKey = "", Path = "/Blog/Edit/{id}", IsPrimary = False, IsPublic = False });
views.Add(new ViewItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", MetaCode = "BLOG_PUBLIC_VIEW", MetaType = "UIVIEW", Title = "The Intwenty Blog", TitleLocalizationKey = "", Path = "Blog/Index", IsPrimary = False, IsPublic = True });

//Userinterface
userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", ViewMetaCode = "BLOG_CREATE_VIEW", MetaCode = "BLOG_INPUTUI", MetaType = "INPUTINTERFACE", DataTableMetaCode = "BLOGAPP" });
userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", ViewMetaCode = "BLOG_EDIT_VIEW", MetaCode = "BLOG_INPUTUI", MetaType = "INPUTINTERFACE", DataTableMetaCode = "BLOGAPP" });
userinterface.Add(new UserInterfaceItem() { SystemMetaCode = "BLOG", AppMetaCode = "BLOGAPP", ViewMetaCode = "BLOG_LISTVIEW", MetaCode = "BLOG_LISTUI", MetaType = "LISTINTERFACE", DataTableMetaCode = "BLOGAPP" });

//UI Components
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "BLOG",AppMetaCode = "BLOGAPP", UserInterfaceMetaCode = "BLOG_INPUTUI", MetaType = "SECTION", MetaCode = "MAINSECTION", Title = "", TitleLocalizationKey="" ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "BLOG",AppMetaCode = "BLOGAPP", UserInterfaceMetaCode = "BLOG_INPUTUI", MetaType = "PANEL", MetaCode = "PNL1", Title = "", TitleLocalizationKey="" ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "BLOG",AppMetaCode = "BLOGAPP", UserInterfaceMetaCode = "BLOG_INPUTUI", MetaType = "TEXTBOX", MetaCode = "TB_HEADER", Title = "Article Header", TitleLocalizationKey="" ParentMetaCode = "PNL1", RowOrder = 1, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "BLOG",AppMetaCode = "BLOGAPP", UserInterfaceMetaCode = "BLOG_INPUTUI", MetaType = "TEXTAREA", MetaCode = "TB_TEXT", Title = "Article Text", TitleLocalizationKey="" ParentMetaCode = "PNL1", RowOrder = 2, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "BLOG",AppMetaCode = "BLOGAPP", UserInterfaceMetaCode = "BLOG_INPUTUI", MetaType = "PANEL", MetaCode = "PNL2", Title = "", TitleLocalizationKey="" ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "BLOG",AppMetaCode = "BLOGAPP", UserInterfaceMetaCode = "BLOG_INPUTUI", MetaType = "IMAGEBOX", MetaCode = "IMGBOX_ARTIMAGE", Title = "Image", TitleLocalizationKey="" ParentMetaCode = "PNL2", RowOrder = 1, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "BLOG",AppMetaCode = "BLOGAPP", UserInterfaceMetaCode = "BLOG_LISTUI", MetaType = "TABLE", MetaCode = "MAIN_EDITLISTVIEW", Title = "Articles", TitleLocalizationKey="" ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "BLOG",AppMetaCode = "BLOGAPP", UserInterfaceMetaCode = "BLOG_LISTUI", MetaType = "TABLETEXTCOLUMN", MetaCode = "ELV_ID", Title = "ID", TitleLocalizationKey="" ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 1, Properties = "" });
userinterfacestructure.Add(new UserInterfaceStructureItem() { SystemMetaCode = "BLOG",AppMetaCode = "BLOGAPP", UserInterfaceMetaCode = "BLOG_LISTUI", MetaType = "TABLETEXTCOLUMN", MetaCode = "ELV_POSTHEADER", Title = "Article Header", TitleLocalizationKey="" ParentMetaCode = "MAIN_EDITLISTVIEW", RowOrder = 1, ColumnOrder = 2, Properties = "" });

//Functions
functions.Add(new FunctionItem() { SystemMetaCode = "BLOG",AppMetaCode = "BLOGAPP", OwnerMetaCode = "BLOG_CREATE_VIEW", OwnerMetaType = "UIVIEW", MetaType = "SAVE", MetaCode = "BLOG_FUNC_SAVE", ActionPath = "", ActionMetaCode = "", ActionMetaType = "", IsModalAction = False, Title = "Save" });
functions.Add(new FunctionItem() { SystemMetaCode = "BLOG",AppMetaCode = "BLOGAPP", OwnerMetaCode = "BLOG_CREATE_VIEW", OwnerMetaType = "UIVIEW", MetaType = "NAVIGATE", MetaCode = "BLOG_FUNC_BACK", ActionPath = "/Blog/List", ActionMetaCode = "BLOG_LISTVIEW", ActionMetaType = "UIVIEW", IsModalAction = False, Title = "Back to list" });
functions.Add(new FunctionItem() { SystemMetaCode = "BLOG",AppMetaCode = "BLOGAPP", OwnerMetaCode = "BLOG_EDIT_VIEW", OwnerMetaType = "UIVIEW", MetaType = "SAVE", MetaCode = "BLOG_FUNC_SAVE1", ActionPath = "", ActionMetaCode = "", ActionMetaType = "", IsModalAction = False, Title = "Save" });
functions.Add(new FunctionItem() { SystemMetaCode = "BLOG",AppMetaCode = "BLOGAPP", OwnerMetaCode = "BLOG_EDIT_VIEW", OwnerMetaType = "UIVIEW", MetaType = "NAVIGATE", MetaCode = "BLOG_FUNC_BACK1", ActionPath = "/Blog/List", ActionMetaCode = "BLOG_LISTVIEW", ActionMetaType = "UIVIEW", IsModalAction = False, Title = "Back to list" });
functions.Add(new FunctionItem() { SystemMetaCode = "BLOG",AppMetaCode = "BLOGAPP", OwnerMetaCode = "BLOG_LISTUI", OwnerMetaType = "LISTINTERFACE", MetaType = "CREATE", MetaCode = "BLOG_FUNC_CREATE", ActionPath = "Blog/Create", ActionMetaCode = "BLOG_CREATE_VIEW", ActionMetaType = "UIVIEW", IsModalAction = False, Title = "Create" });
functions.Add(new FunctionItem() { SystemMetaCode = "BLOG",AppMetaCode = "BLOGAPP", OwnerMetaCode = "BLOG_LISTUI", OwnerMetaType = "LISTINTERFACE", MetaType = "EDIT", MetaCode = "BLOG_FUNC_EDIT", ActionPath = "Blog/Edit/{id}", ActionMetaCode = "BLOG_EDIT_VIEW", ActionMetaType = "UIVIEW", IsModalAction = False, Title = "Edit" });
functions.Add(new FunctionItem() { SystemMetaCode = "BLOG",AppMetaCode = "BLOGAPP", OwnerMetaCode = "BLOG_LISTUI", OwnerMetaType = "LISTINTERFACE", MetaType = "DELETE", MetaCode = "BLOG_FUNC_DELETE", ActionPath = "", ActionMetaCode = "", ActionMetaType = "", IsModalAction = False, Title = "Delete" });
functions.Add(new FunctionItem() { SystemMetaCode = "BLOG",AppMetaCode = "BLOGAPP", OwnerMetaCode = "BLOG_LISTUI", OwnerMetaType = "LISTINTERFACE", MetaType = "FILTER", MetaCode = "BLOG_FUNC_FILTER", ActionPath = "", ActionMetaCode = "", ActionMetaType = "", IsModalAction = False, Title = "Filter" });
functions.Add(new FunctionItem() { SystemMetaCode = "BLOG",AppMetaCode = "BLOGAPP", OwnerMetaCode = "BLOG_LISTUI", OwnerMetaType = "LISTINTERFACE", MetaType = "PAGING", MetaCode = "BLOG_FUNC_PAGING", ActionPath = "", ActionMetaCode = "", ActionMetaType = "", IsModalAction = False, Title = "Paging" });

//Domains
var valuedomains = new List<ValueDomainItem>();
valuedomains.Add(new ValueDomainItem() { DomainName = "ITEMCATEGORY",Code = "PROD", Value = "Products" });
valuedomains.Add(new ValueDomainItem() { DomainName = "ITEMCATEGORY",Code = "SERV", Value = "Services" });
valuedomains.Add(new ValueDomainItem() { DomainName = "CUSTOMERTAGS",Code = "1", Value = "Tag 1" });
valuedomains.Add(new ValueDomainItem() { DomainName = "CUSTOMERTAGS",Code = "2", Value = "Tag 2" });
valuedomains.Add(new ValueDomainItem() { DomainName = "CUSTOMERTAGS",Code = "3", Value = "Tag 3" });
valuedomains.Add(new ValueDomainItem() { DomainName = "VENDORPRIO",Code = "P3", Value = "Prio 3" });
valuedomains.Add(new ValueDomainItem() { DomainName = "VENDORPRIO",Code = "P1", Value = "Prio 1" });
valuedomains.Add(new ValueDomainItem() { DomainName = "VENDORPRIO",Code = "P2", Value = "Prio 2" });
valuedomains.Add(new ValueDomainItem() { DomainName = "VENDORATTRIBUTES",Code = "400", Value = "Low Quality" });
valuedomains.Add(new ValueDomainItem() { DomainName = "VENDORATTRIBUTES",Code = "300", Value = "High Quality" });
valuedomains.Add(new ValueDomainItem() { DomainName = "VENDORATTRIBUTES",Code = "200", Value = "Slow" });
valuedomains.Add(new ValueDomainItem() { DomainName = "VENDORATTRIBUTES",Code = "100", Value = "Trusted" });

//Endpoints
var endpoints = new List<EndpointItem>();
endpoints.Add(new EndpointItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "TABLEGET", MetaCode = "EP_CUSTOMER_GETLATEST", DataMetaCode = "CUSTOMER", Path = "MyAPI/Customer", Title = "Get customer", Description = "Get latest version of a customer by id", ParentMetaCode = "ROOT" });
endpoints.Add(new EndpointItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "TABLELIST", MetaCode = "EP_CUSTOMER_GETALL", DataMetaCode = "CUSTOMER", Path = "MyAPI/Customers", Title = "Get all customers", Description = "Get all customers in the database", ParentMetaCode = "ROOT" });
endpoints.Add(new EndpointItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "CUSTOMER", MetaType = "TABLESAVE", MetaCode = "EP_CUSTOMER_SAVE", DataMetaCode = "CUSTOMER", Path = "MyAPI/Customer/Save", Title = "Save customer", Description = "Create or update customer", ParentMetaCode = "ROOT" });
endpoints.Add(new EndpointItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "", MetaType = "CUSTOMPOST", MetaCode = "EP_CUST_1", DataMetaCode = "", Path = "MyAPI/PostAnyThing", Title = "Post Anything", Description = "This endpoint must be implemented in a custom controller", ParentMetaCode = "ROOT" });
endpoints.Add(new EndpointItem() { SystemMetaCode = "WAREHOUSE", AppMetaCode = "", MetaType = "CUSTOMPOST", MetaCode = "EP_AUTH", DataMetaCode = "", Path = "MyAPI/Authenticate", Title = "Authenticate with a product", Description = "Authenticates a user", ParentMetaCode = "ROOT" });
//Insert models if not existing
var client = new Connection(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
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
 if (!current_domains.Exists(p => p.DomainName == t.DomainName))
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

var current_endpoints = client.GetEntities<EndpointItem>();
foreach (var t in endpoints)
{
 if (!current_endpoints.Exists(p => p.MetaCode == t.MetaCode && p.AppMetaCode == t.AppMetaCode))
     client.InsertEntity(t);
}
client.Close();
}
