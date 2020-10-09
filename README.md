![alt text](https://github.com/Domitor/Intwenty/blob/master/IntwentyDemo/wwwroot/images/intwenty_loggo_small.png)


# Intwenty
Create metadata driven applications with java script and ASP.NET Core. 
- Define metadata for your datamodel and let Intwenty generate your database structure along with your needs for storing and retriving information.
- Define your UI model and let intwenty generate your UI.
- Implements Asp.Net Core Identity, without any need of entity framework.
- Uses Intwenty.DataClient a small but fast Db connection library with ORM functions and JSON support


# How to ?

| Task |
| ------------- |
|  <a href="https://github.com/Domitor/Intwenty/wiki/How-to-get-started">Quick start</a> |  
|  <a href="https://github.com/Domitor/Intwenty/wiki/Intwenty-Settings">Configure intwenty</a> |  
|  <a href="https://github.com/Domitor/Intwenty/wiki/Application-startup">Configure Asp.Net Core Identity with Intwenty</a> | 
| <a href="https://github.com/Domitor/Intwenty/wiki/The-Intwenty-DataService">The Intwenty DataService</a> |
| <a href="https://github.com/Domitor/Intwenty/wiki/The-Intwenty-ModelService">The Intwenty ModelService</a> | |
| Run an Intwenty self test |
| Create an Intwenty Application |
| Save an Intwenty Application |
| Use Intwenty as a tradional ORM |
| Accessing the Admin UI |
| Sharing your model |
| Import a model |
| Generate model documentation |


# Create a model using the built in designers

## Create application
![alt text](https://github.com/Domitor/Intwenty/blob/master/IntwentyDemo/wwwroot/images/manage_applications.png)

## Add database model to the application
![alt text](https://github.com/Domitor/Intwenty/blob/master/IntwentyDemo/wwwroot/images/manage_db.png)

## Add a listview UI to the application
![alt text](https://github.com/Domitor/Intwenty/blob/master/IntwentyDemo/wwwroot/images/manage_ui_list.png)

## Add create/edit UI to the application
![alt text](https://github.com/Domitor/Intwenty/blob/master/IntwentyDemo/wwwroot/images/manage_ui.png)

# Create an application model programmaticly

## Create application
```
client.InsertEntity(new ApplicationItem() { Id = 10, Description = "An app for managing customers", MetaCode = "CUSTOMER", Title = "Customer", TitleLocalizationKey="CUSTOMER", DbName = "Customer", IsHierarchicalApplication = false, UseVersioning = false });
```
## Add database model to the application
```
client.InsertEntity(new DatabaseItem() { AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMERID", DbName = "CustomerId", ParentMetaCode = "ROOT", DataType = "STRING", Mandatory = true, IsUnique = true, Properties= "DEFVALUE=AUTO#DEFVALUE_START=1000#DEFVALUE_PREFIX=CUST#DEFVALUE_SEED=100" });
client.InsertEntity(new DatabaseItem() { AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMERNAME", DbName = "CustomerName", ParentMetaCode = "ROOT", DataType = "STRING" });
client.InsertEntity(new DatabaseItem() { AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMERPHONE", DbName = "CustomerPhone", ParentMetaCode = "ROOT", DataType = "STRING" });
client.InsertEntity(new DatabaseItem() { AppMetaCode = "CUSTOMER", MetaType = "DATACOLUMN", MetaCode = "CUSTOMEREMAIL", DbName = "CustomerEmail", ParentMetaCode = "ROOT", DataType = "STRING" });
 ```
 
 ## Add a listview UI to the application
 ```
client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "LISTVIEW", MetaCode = "MAIN_LISTVIEW", DataMetaCode = "", Title = "Customer List", TitleLocalizationKey = "CUSTOMERLIST", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "LISTVIEWCOLUMN", MetaCode = "LV_ID", DataMetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 1 });
client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "LISTVIEWCOLUMN", MetaCode = "LV_CUSTID", DataMetaCode = "CUSTOMERID", Title = "Customer ID", TitleLocalizationKey = "CUSTOMERID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 2 });
client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "LISTVIEWCOLUMN", MetaCode = "LV_CUSTNAME", DataMetaCode = "CUSTOMERNAME", Title = "Customer Name", TitleLocalizationKey = "CUSTOMERNAME", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 3 });
 ```
 ## Add create/edit UI to the application
  ```
  client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "SECTION", MetaCode = "MAINSECTION", DataMetaCode = "", Title = "", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "PANEL", MetaCode = "CUSTPNL1", DataMetaCode = "", Title = "Basics", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1 });
client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "TEXTBOX", MetaCode = "TB_CUSTID", DataMetaCode = "CUSTOMERID", Title = "Customer ID", TitleLocalizationKey = "CUSTOMERID", ParentMetaCode = "CUSTPNL1", RowOrder = 1, ColumnOrder = 1 });
client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "TEXTBOX", MetaCode = "TB_CUSTNAME", DataMetaCode = "CUSTOMERNAME", Title = "Customer Name", TitleLocalizationKey = "CUSTOMERNAME", ParentMetaCode = "CUSTPNL1", RowOrder = 2, ColumnOrder = 1 });
client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "PANEL", MetaCode = "CUSTPNL2", DataMetaCode = "", Title = "Contact", TitleLocalizationKey = "CUSTOMERCONTACT", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 2 });
client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "EMAILBOX", MetaCode = "TBCUSTMAIL", DataMetaCode = "CUSTOMEREMAIL", Title = "Email", TitleLocalizationKey = "CUSTOMERPHONE", ParentMetaCode = "CUSTPNL2", RowOrder = 3, ColumnOrder = 2 });
client.InsertEntity(new UserInterfaceItem() { AppMetaCode = "CUSTOMER", MetaType = "NUMBOX", MetaCode = "TBCUSTPHONE", DataMetaCode = "CUSTOMERPHONE", Title = "Phone", TitleLocalizationKey = "CUSTOMEREMAIL", ParentMetaCode = "CUSTPNL2", RowOrder = 3, ColumnOrder = 2 });
 ```

# Generated UI
![alt text](https://github.com/Domitor/Intwenty/blob/master/IntwentyDemo/wwwroot/images/app_list.png)
![alt text](https://github.com/Domitor/Intwenty/blob/master/IntwentyDemo/wwwroot/images/app_create.png)

# Endpoints supporting the generated application
 ```
/Application/GetList/{applicationid} 
/Application/Create/{applicationid} 
/Application/Edit/{applicationid}/{id}
/Application/API/Save
/Application/API/Delete
/Application/API/GetLatestVersion/{applicationid}/{id}
/Application/API/GetLatestByLoggedInUser/{applicationid}
/Application/API/GetListView
/Application/API/GetValueDomains/{id}
 ```
 
# Intentions
1. Boost productivity
2. To be lightweight
3. Keep dependencies to a minimum.

# Backend Dependencies
- asp.net core 3.1
- Microsoft.AspNetCore.Mvc 2.2.0
- Intwenty.DataClient

# Frontend Dependencies
- bootstrap 4.3.7
- popper.js 1.16.1
- vue.js 2.6.11
- jquery 3.3.1
- alasql 0.5.5
- fontawesome-free-5.12.1-web

# Works with the following databases
- MS Sql Server
- MySql
- Maria DB
- PostgreSQL
- SQLite


# How to get started
<a href="https://github.com/Domitor/Intwenty/wiki">Consult the Wiki</a>

# How to get Intwenty
- Fork this Repository.
- Download the latest release on github.
- Use the nuget package.










