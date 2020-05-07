![alt text](https://github.com/Domitor/Intwenty/blob/master/IntwentyDemo/wwwroot/images/intwenty_loggo_small.png)


# Intwenty
Create metadata driven applications with java script and ASP.NET Core. 
- Define your metamodel and work seamless against six supported databases. (See list below)
- Define metadata for your datamodel and let Intwenty generate your database structure along with  your needs for storing and retriving information.
- Define metadata for your UI model and let intwenty generate your UI.
- It can be seen as an ORM for dynamic data where the structure is described in meta data rather than strongly typed objects
- It implements Asp.Net Core Identity, without any need of entity framework.
- Can also be used as a tradiotional ORM with strongly typed objects.

# Features
- Includes admin UI for inputing application meta data. (Describe UI and datastructure).
- Supports four sql databases and two nosql stores, se list below.
- Use Asp.Net Core Identity with all supported dbms.
- Forget about entity framework code first, database first, migrations etc. With intwenty it's possible to change both the UI and datamodel without recompile or even publish.
- Easy import and export of metamodels which makes it possible for users to share models.
- Generates documentation. 

# How
- Intwenty does not work with strongly typed models. Instead it performs work both on the data and UI level based on the information in the metamodel.
- Users/Developers can input metadata via a graphical UI, which allows for handling both the datamodel and the UI model. 
- It comes as an RCL (Razor Clas Library) which can easily be included in any asp.net core web application.

# A simple example

1. Get Intwenty from  github (SEE RELEASES) or use the nuget package
2. If using the nuget package, include client dependencies

            - bootstrap 4.3.7  
            - popper.js 1.16.1
            - vue.js 2.6.11 
            - jquery 3.3.1  )
            - alasql 0.5.5 
            - fontawesome-free-5.12.1-web
            - Intwenty.js
            - Intwenty.css
            

3. Ensure you have an appsetting.json with the IntwentySettings included.
            
             "IntwentySettings": 
             {
                "DefaultConnection": "Data Source=wwwroot/sqlite/IntwentyDb.db",
                "IsDevelopment": true,
                "ReCreateModelOnStartUp": true,
                "DefaultConnectionDBMS": "SQLite" // MSSqlServer, MySql, MariaDB, PostgreSQL, SQLite, MongoDb, LiteDb
              }
            
4. Inject services

            Create a controller (or other class) where you inject the intwenty services needed.

            //Example
            public class MyIntwentyController : Controller
            {
        
                    private readonly IIntwentyModelService _modelservice;
                    private readonly IIntwentyDataService _dataservice;
                    private readonly IntwentySettings _settings;
                    private readonly IMemoryCache _cache;

                    public MyIntwentyController(IIntwentyModelService modelservice, 
                                                IIntwentyDataService dataservice, 
                                                IOptions<IntwentySettings> settings,
                                                IMemoryCache cache)
                    {
                        _modelservice = modelservice;
                        _dataservice = dataservice;
                        _settings = settings.Value;
                        _cache = cache;
                    }
            }


5. Create a DB Connection

            IIntwentyDbORM dbstore = null;
            if (_settings.IsNoSQL)
                dbstore = new IntwentyNoSqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection, "IntwentyDb");
            else
                dbstore = new IntwentySqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);


6. Create a model for a new application

            //Only needed the first run, creates tables needed for storing the intwenty model
            _modelservice.CreateIntwentyDatabase();

             var models = _modelservice.GetApplicationModels();
            if (!models.Exists(p => p.Application.MetaCode == "MYNEWAPP"))
            {
                //ADD THE APP DESCRIPTION
                dbstore.Insert(new ApplicationItem() { Id = 10000, Description = "An app for testing intwenty", MetaCode = "MYNEWAPP", Title = "My test application", DbName = "MyNewApp", IsHierarchicalApplication = false, UseVersioning = false });

                //ADD TWO DB COLUMNS NAMED Header and Description in the application table: MyNewApp 
                dbstore.Insert(new DatabaseItem() { AppMetaCode = "MYNEWAPP", MetaType = "DATACOLUMN", MetaCode = "HEADER", DbName = "Header", ParentMetaCode = "ROOT", DataType = "STRING" });
                dbstore.Insert(new DatabaseItem() { AppMetaCode = "MYNEWAPP", MetaType = "DATACOLUMN", MetaCode = "DESCRIPTION", DbName = "Description", ParentMetaCode = "ROOT", DataType = "TEXT" });

                //ADD UI, A Textbox and a Textarea, connect the to the DataBaseItem (DataMetaCode)
                dbstore.Insert(new UserInterfaceItem() { AppMetaCode = "MYNEWAPP", MetaType = "SECTION", MetaCode = "MAINSECTION", DataMetaCode = "", Title = "", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
                dbstore.Insert(new UserInterfaceItem() { AppMetaCode = "MYNEWAPP", MetaType = "PANEL", MetaCode = "PNL1", DataMetaCode = "", Title = "Basics", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1 });
                dbstore.Insert(new UserInterfaceItem() { AppMetaCode = "MYNEWAPP", MetaType = "TEXTBOX", MetaCode = "TB_HDR", DataMetaCode = "HEADER", Title = "Header", ParentMetaCode = "PNL1", RowOrder = 1, ColumnOrder = 1 });
                dbstore.Insert(new UserInterfaceItem() { AppMetaCode = "MYNEWAPP", MetaType = "TEXTAREA", MetaCode = "TA_DESC", DataMetaCode = "DESCRIPTION", Title = "Description", ParentMetaCode = "PNL1", RowOrder = 2, ColumnOrder = 1 });

                //ADD LISTVIEW
                dbstore.Insert(new UserInterfaceItem() { AppMetaCode = "MYNEWAPP", MetaType = "LISTVIEW", MetaCode = "MAIN_LISTVIEW", DataMetaCode = "", Title = "My New App List", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
                dbstore.Insert(new UserInterfaceItem() { AppMetaCode = "MYNEWAPP", MetaType = "LISTVIEWFIELD", MetaCode = "LV_ID", DataMetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 1 });
                dbstore.Insert(new UserInterfaceItem() { AppMetaCode = "MYNEWAPP", MetaType = "LISTVIEWFIELD", MetaCode = "LV_HDR", DataMetaCode = "HEADER", Title = "Header", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 2 });

                //Configure database, eg. create table and columns according to the model, not needed for nosql
                var result = _modelservice.ConfigureDatabase();
              

                //Done, your new app is now available
                //- A table named MyNewApp in your database, with two columns
                //- UI ~/Application/GetList/10000 
                //- UI ~/Application/Create/10000 
                //- UI ~/Application/Edit/10000/1 
                //- Endpoint ~/Application/Save
            }
  


# Intentions
1. Boost productivity
2. To be lightweight
3. Keep dependencies to a minimum.

# Backend Dependencies
- asp.net core 3.1
- Microsoft.AspNetCore.Mvc 2.2.0
- Microsoft.Data.Sqlite.Core 3.1.3
- System.Data.SQLite.Core 1.0.112.1
- System.Data.SqlClient 4.8.1
- MongoDB.Driver 2.10.3
- MySql.Data 8.0.19
- Npgsql 4.1.3.1

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
- MongoDB
- LiteDB

# How to get started
<a href="https://github.com/Domitor/Intwenty/wiki">Consult the Wiki</a>

# How to get Intwenty
- Fork this Repository.
- Download the latest release on github.
- Use the nuget package.



# Status
Weekly updates, but not ready for production use.








