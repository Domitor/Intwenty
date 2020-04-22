using Microsoft.AspNetCore.Mvc;
using Intwenty.Model;
using Intwenty.Data.DBAccess;
using Intwenty.Data.Entity;
using Microsoft.Extensions.Options;

namespace Intwenty.Controllers
{
    
    //[Authorize(Roles="Administrator")]
    public class ExamplesController : Controller
    {

        private readonly IIntwentyModelService _modelservice;
        private readonly IntwentySettings _settings;

        public ExamplesController(IIntwentyModelService modelservice, IOptions<IntwentySettings> settings)
        {
            _modelservice = modelservice;
            _settings = settings.Value;
        }

        public IActionResult Example1()
        {

            //1. Get Intwenty from github (SEE RELEASES) or use the nuget package

            //2. IF USING THE NUGET PACKAGE, ENSURE TO INCLUDE CLIENT DEPENDENCIES
            /*
            - bootstrap 4.3.7  
            - popper.js 1.16.1
            - vue.js 2.6.11 
            - jquery 3.3.1  )
            - alasql 0.5.5 
            - fontawesome-free-5.12.1-web
            - Intwenty.js
            - Intwenty.css
            */

            //3. ENSURE YOU HAVE AN appsetting.json with the IntwentySettings included.
            /*
             "IntwentySettings": 
             {
                "DefaultConnection": "Data Source=wwwroot/sqlite/IntwentyDb.db",
                "IsDevelopment": true,
                "ReCreateModelOnStartUp": true,
                "DefaultConnectionDBMS": "SQLite" // MSSqlServer, MySql, MariaDB, PostgreSQL, SQLite, MongoDb, LiteDb
              }
            */


            //CREATE A DB CONNECTION
            IIntwentyDbORM dbstore = null;
            if (_settings.IsNoSQL)
                dbstore = new IntwentyNoSqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection, "IntwentyDb");
            else
                dbstore = new IntwentySqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);

            //NEED ONLY IF FIRST RUN (CREATES TABLES NEEDED FOR STORING THE INTWENTY MODEL)
            _modelservice.CreateIntwentyDatabase();

            //GET CURRENT MODELS
            var models = _modelservice.GetApplicationModels();

            //CREATE A NEW MINIMALISTIC APP ON THE FLY

            //CHECK IF APP ALREADY EXISTS
            if (!models.Exists(p => p.Application.MetaCode == "MYNEWAPP"))
            {
                //ADD THE APP DESCRIPTION
                dbstore.Insert(new ApplicationItem() { Id = 10000, Description = "An app for testing intwenty", MetaCode = "MYNEWAPP", Title = "My test application", DbName = "MyNewApp", IsHierarchicalApplication = false, UseVersioning = false });

                //ADD TWO DB COLUMNS NAMED Header and Description in the application table: MyNewApp 
                dbstore.Insert(new DatabaseItem() { AppMetaCode = "MYNEWAPP", MetaType = "DATACOLUMN", MetaCode = "HEADER", DbName = "Header", ParentMetaCode = "ROOT", DataType = "STRING" });
                dbstore.Insert(new DatabaseItem() { AppMetaCode = "MYNEWAPP", MetaType = "DATACOLUMN", MetaCode = "DESCRIPTION", DbName = "Description", ParentMetaCode = "ROOT", DataType = "TEXT" });

                //ADD UI, A Textbox and a Textarea, connect the to the DataBaseItem (DataMetaCode)
                dbstore.Insert(new UserInterfaceItem() { AppMetaCode = "MYNEWAPP", MetaType = "SECTION", MetaCode = "MAINSECTION", DataMetaCode = "", Title = "Sales Header", ParentMetaCode = "ROOT", RowOrder = 1, ColumnOrder = 1, Properties = "COLLAPSIBLE=FALSE#STARTEXPANDED=FALSE" });
                dbstore.Insert(new UserInterfaceItem() { AppMetaCode = "MYNEWAPP", MetaType = "PANEL", MetaCode = "PNL1", DataMetaCode = "", Title = "Basics", ParentMetaCode = "MAINSECTION", RowOrder = 1, ColumnOrder = 1 });
                dbstore.Insert(new UserInterfaceItem() { AppMetaCode = "MYNEWAPP", MetaType = "TEXTBOX", MetaCode = "TB_HDR", DataMetaCode = "HEADER", Title = "Header", ParentMetaCode = "PNL1", RowOrder = 1, ColumnOrder = 1 });
                dbstore.Insert(new UserInterfaceItem() { AppMetaCode = "MYNEWAPP", MetaType = "TEXTAREA", MetaCode = "TA_DESC", DataMetaCode = "DESCRIPTION", Title = "Description", ParentMetaCode = "PNL1", RowOrder = 2, ColumnOrder = 1 });

                //ADD LISTVIEW
                dbstore.Insert(new UserInterfaceItem() { AppMetaCode = "MYNEWAPP", MetaType = "LISTVIEW", MetaCode = "MAIN_LISTVIEW", DataMetaCode = "", Title = "My New App List", ParentMetaCode = "ROOT", RowOrder = 0, ColumnOrder = 0 });
                dbstore.Insert(new UserInterfaceItem() { AppMetaCode = "MYNEWAPP", MetaType = "LISTVIEWFIELD", MetaCode = "LV_ID", DataMetaCode = "ID", Title = "ID", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 1 });
                dbstore.Insert(new UserInterfaceItem() { AppMetaCode = "MYNEWAPP", MetaType = "LISTVIEWFIELD", MetaCode = "LV_HDR", DataMetaCode = "HEADER", Title = "Header", ParentMetaCode = "MAIN_LISTVIEW", RowOrder = 1, ColumnOrder = 2 });

                //CONFIGURE DATABASE
                var result = _modelservice.ConfigureDatabase();


                //DONE, YOU SHOULD NOW HAVE
                //- A table named MyNewApp in your database, with two columns
                //- UI ~/Application/GetList/10000 
                //- Save endpoint at ~/Application/Save

            }


            return View();
        }

       
    }
}
