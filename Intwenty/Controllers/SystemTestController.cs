using Microsoft.AspNetCore.Mvc;
using Intwenty.Model;
using Intwenty.Data.DBAccess;
using Intwenty.Data.Entity;
using Microsoft.Extensions.Options;
using Intwenty.Data.Dto;
using System;
using Intwenty.Data.DBAccess.Annotations;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json;
using System.Collections.Generic;
using Intwenty.Engine;
using Microsoft.Extensions.Caching.Memory;

namespace Intwenty.Controllers
{
    

    public class SystemTestController : Controller
    {

        private readonly IIntwentyModelService _modelservice;
        private readonly IIntwentyDataService _dataservice;
        private readonly IntwentySettings _settings;
        private readonly IMemoryCache _cache;

        public SystemTestController(IIntwentyModelService modelservice, 
                                    IIntwentyDataService dataservice, 
                                    IOptions<IntwentySettings> settings,
                                    IMemoryCache cache)
        {
            _modelservice = modelservice;
            _dataservice = dataservice;
            _settings = settings.Value;
            _cache = cache;
        }

        public IActionResult RunTests()
        {

            return View();
        }

   
        [HttpPost]
        public JsonResult RunSystemTests()
        {
            var res = new List<OperationResult>();

            //CLEAN UP PREV TEST
            var model = _modelservice.GetAppModels().Find(p => p.Id == 10000);
            if (model != null)
                _modelservice.DeleteAppModel(model);

            if (!_settings.IsNoSQL)
            {
                var db = new IntwentySqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);
                db.Open();

                db.CreateCommand("DELETE FROM sysmodel_ValueDomainItem WHERE DOMAINNAME = 'TESTDOMAIN'");
                db.ExecuteNonQuery();

                if (db.TableExist("TestApp"))
                {
                    db.CreateCommand("DROP TABLE TestApp");
                    db.ExecuteNonQuery();
                }
                if (db.TableExist("TestApp_Versioning"))
                {
                    db.CreateCommand("DROP TABLE TestApp_Versioning");
                    db.ExecuteNonQuery();
                }
                if (db.TableExist("TestAppSubTable"))
                {
                    db.CreateCommand("DROP TABLE TestAppSubTable");
                    db.ExecuteNonQuery();
                }
                if (db.TableExist("tests_TestDataAutoInc"))
                {
                    db.CreateCommand("DROP TABLE tests_TestDataAutoInc");
                    db.ExecuteNonQuery();
                }
                if (db.TableExist("tests_TestDataIndexNoAutoInc"))
                {
                    db.CreateCommand("DROP TABLE tests_TestDataIndexNoAutoInc");
                    db.ExecuteNonQuery();
                }

                db.Close();
            }
            else
            {
                var db = new IntwentyNoSqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);
                var list = db.GetAll<ValueDomainItem>();
                foreach (var item in list)
                    if (item.DomainName == "TESTDOMAIN")
                        db.Delete(item);

                db.DropCollection("TestApp");
                db.DropCollection("TestApp_Versioning");
                db.DropCollection("TestAppSubTable");
                db.DropCollection("tests_TestDataAutoInc");
                db.DropCollection("tests_TestDataIndexNoAutoInc");

            }

            res.Add(Test1ORMCreateTable());
            res.Add(Test2ORMInsert());
            res.Add(Test3ORMUpdate());
            res.Add(Test4ORMDelete());
            res.Add(Test5CreateIntwentyDb());
            res.Add(Test6CreateIntwentyExampleModel());
            res.Add(Test7CreateIntwentyApplication());
            res.Add(Test8GetListOfIntwentyApplication());
            res.Add(Test9GetListOfIntwentyApplicationByOwnerUser());
            res.Add(Test10GetLatestVersionByOwnerUser());
            res.Add(Test11UpdateIntwentyApplication());
            res.Add(Test12GetAllValueDomains());
            res.Add(Test13GetDataSet());

            return new JsonResult(res);

        }


        private OperationResult Test1ORMCreateTable()
        {
            OperationResult result = new OperationResult(true, "IIntwentyDbORM.CreateTable<T>");
            try
            {

                IIntwentyDbORM dbstore = null;
                if (_settings.IsNoSQL)
                    dbstore = new IntwentyNoSqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);
                else
                    dbstore = new IntwentySqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);

                dbstore.CreateTable<TestDataAutoInc>(true);

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private OperationResult Test2ORMInsert()
        {
            OperationResult result = new OperationResult(true, "IIntwentyDbORM.Insert(T) - 2 Records using auto increment");
            try
            {

                IIntwentyDbORM dbstore = null;
                if (_settings.IsNoSQL)
                    dbstore = new IntwentyNoSqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);
                else
                    dbstore = new IntwentySqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);

                var t = new TestDataAutoInc() { BoolValue = true, IntValue = 777, DecimalValue = 666.66M, Description = "Test data record/document 1", Header = "Test2ORMInsertTable", FloatValue = 666.66F };
                dbstore.Insert(t);
                t = new TestDataAutoInc() { BoolValue = false, IntValue = 888, DecimalValue = 888.88M, Description = "Test data record/document 2", Header = "Test2ORMInsertTable", FloatValue = 888.88F };
                dbstore.Insert(t);

                var check = dbstore.GetAll<TestDataAutoInc>();
                if (check.Count < 2)
                    throw new InvalidOperationException("Could not retrieve inserted records with IIntwentyDbORM.GetAll<T>");

                if (check.Exists(p=> p.Id < 1))
                    throw new InvalidOperationException("AutoInc failed on IIntwentyDbORM.Insert<T>");

               

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private OperationResult Test3ORMUpdate()
        {
            OperationResult result = new OperationResult(true, "IIntwentyDbORM.Update(T) - Retrieve last record and update");
            try
            {

                IIntwentyDbORM dbstore = null;
                if (_settings.IsNoSQL)
                    dbstore = new IntwentyNoSqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);
                else
                    dbstore = new IntwentySqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);


                var check = dbstore.GetAll<TestDataAutoInc>();
                if (check.Count < 2)
                    throw new InvalidOperationException("Test could not be performed beacause of dependency to previous test.");

                var last = check[check.Count - 1];
                var checkone = dbstore.GetOne<TestDataAutoInc>(last.Id);
                if (checkone == null)
                    throw new InvalidOperationException("Could not retrieve last inserted record with IIntwentyDbORM.GetOne<T>");

                last.Header = "Test3ORMUpdateTable";
                last.IntValue = 555;
                last.FloatValue = 555.55F;
                last.DecimalValue = 555.55M;
                last.BoolValue = true;

                dbstore.Update(last);
                checkone = dbstore.GetOne<TestDataAutoInc>(last.Id);
                if (checkone == null)
                    throw new InvalidOperationException("Could not retrieve inserted record with IIntwentyDbORM.GetOne<T> after update");

                if (checkone.IntValue != 555)
                    throw new InvalidOperationException("Updated integer value was not persisted.");
                if (checkone.FloatValue != 555.55F)
                    throw new InvalidOperationException("Updated float value was not persisted.");
                if (checkone.DecimalValue != 555.55M)
                    throw new InvalidOperationException("Updated decimal value was not persisted.");
                if (!checkone.BoolValue)
                    throw new InvalidOperationException("Updated bool value was not persisted.");
                if (checkone.Header != "Test3ORMUpdateTable")
                    throw new InvalidOperationException("Updated string value was not persisted.");




            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private OperationResult Test4ORMDelete()
        {
            OperationResult result = new OperationResult(true, "IIntwentyDbORM.Delete(T) - Retrieve a list of inserted records and delete them one by one");
            try
            {

                IIntwentyDbORM dbstore = null;
                if (_settings.IsNoSQL)
                    dbstore = new IntwentyNoSqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);
                else
                    dbstore = new IntwentySqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);


                var check = dbstore.GetAll<TestDataAutoInc>();
                if (check.Count < 2)
                    throw new InvalidOperationException("Test could not be performed beacause of dependency to previous test.");

                foreach (var t in check)
                {

                    dbstore.Delete(t);

                }

                check = dbstore.GetAll<TestDataAutoInc>();
                if (check.Count > 0)
                    throw new InvalidOperationException("The deleted records was still present in the data store after deletion");


            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private OperationResult Test5CreateIntwentyDb()
        {
            OperationResult result = new OperationResult(true, "IIntwentyModelService.CreateIntwentyDatabase()");
            try
            {

                _modelservice.CreateIntwentyDatabase();

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private OperationResult Test6CreateIntwentyExampleModel()
        {
            OperationResult result = new OperationResult(true, "Create an intwenty application (My test application)");
            try
            {
                _cache.Remove("APPMODELS");

                IIntwentyDbORM dbstore = null;
                if (_settings.IsNoSQL)
                    dbstore = new IntwentyNoSqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);
                else
                    dbstore = new IntwentySqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);


                dbstore.Insert(new ApplicationItem() { Id = 10000, Description = "An app for testing intwenty", MetaCode = "TESTAPP", Title = "My test application", DbName = "TestApp", IsHierarchicalApplication = false, UseVersioning = false });
                dbstore.Insert(new DatabaseItem() { AppMetaCode = "TESTAPP", MetaType = "DATACOLUMN", MetaCode = "HEADER", DbName = "Header", ParentMetaCode = "ROOT", DataType = "STRING" });
                dbstore.Insert(new DatabaseItem() { AppMetaCode = "TESTAPP", MetaType = "DATACOLUMN", MetaCode = "DESCRIPTION", DbName = "Description", ParentMetaCode = "ROOT", DataType = "TEXT" });
                dbstore.Insert(new DatabaseItem() { AppMetaCode = "TESTAPP", MetaType = "DATACOLUMN", MetaCode = "BOOLVALUE", DbName = "BoolValue", ParentMetaCode = "ROOT", DataType = "BOOLEAN" });
                dbstore.Insert(new DatabaseItem() { AppMetaCode = "TESTAPP", MetaType = "DATACOLUMN", MetaCode = "INTVALUE", DbName = "IntValue", ParentMetaCode = "ROOT", DataType = "INTEGER" });
                dbstore.Insert(new DatabaseItem() { AppMetaCode = "TESTAPP", MetaType = "DATACOLUMN", MetaCode = "DECVALUE", DbName = "DecValue", ParentMetaCode = "ROOT", DataType = "3DECIMAL" });
                dbstore.Insert(new DatabaseItem() { AppMetaCode = "TESTAPP", MetaType = "DATACOLUMN", MetaCode = "DECVALUE2", DbName = "DecValue2", ParentMetaCode = "ROOT", DataType = "2DECIMAL" });
                dbstore.Insert(new DatabaseItem() { AppMetaCode = "TESTAPP", MetaType = "DATATABLE", MetaCode = "TESTAPP_SUBTABLE", DbName = "TestAppSubTable", ParentMetaCode = "ROOT" });
                dbstore.Insert(new DatabaseItem() { AppMetaCode = "TESTAPP", MetaType = "DATACOLUMN", MetaCode = "LINETEXT", DbName = "LineHeader", ParentMetaCode = "TESTAPP_SUBTABLE", DataType = "STRING" });
                dbstore.Insert(new DatabaseItem() { AppMetaCode = "TESTAPP", MetaType = "DATACOLUMN", MetaCode = "LINEDESCRIPTION", DbName = "LineDescription", ParentMetaCode = "TESTAPP_SUBTABLE", DataType = "STRING" });

                dbstore.Insert(new ValueDomainItem() { DomainName = "TESTDOMAIN", Value = "Domain Value 1", Code = "1" });
                dbstore.Insert(new ValueDomainItem() { DomainName = "TESTDOMAIN", Value = "Domain Value 2", Code = "2" });
                dbstore.Insert(new ValueDomainItem() { DomainName = "TESTDOMAIN", Value = "Domain Value 2", Code = "3" });

                var model = _modelservice.GetApplicationModels().Find(p => p.Application.Id == 10000);

                OperationResult configres;
                if (_settings.IsNoSQL)
                {
                    var t = NoSqlDbDataManager.GetDataManager(model, _modelservice, _settings, (IntwentyNoSqlDbClient)dbstore);
                    configres = t.ConfigureDatabase();


                }
                else
                {
                    var t = SqlDbDataManager.GetDataManager(model, _modelservice, _settings, (IntwentySqlDbClient)dbstore);
                    configres = t.ConfigureDatabase();
                }

                if (!configres.IsSuccess)
                    throw new InvalidOperationException("The created intwenty model could not be configured with success");


            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private OperationResult Test7CreateIntwentyApplication()
        {
            OperationResult result = new OperationResult(true, "Create 5 intwenty application based on the created test model");
            try
            {
                for (int i = 0; i < 5; i++)
                {
                    var state = new ClientStateInfo();
                    state.ApplicationId = 10000;
                    state.UserId = "TESTUSER";
                    state.OwnerUserId = "TESTUSER";
                    if (i > 2)
                        state.OwnerUserId = "OTHERUSER";

                    state.Values.Add(new ApplicationValue() { DbName = "Header", Value = "Test Header " + i });
                    state.Values.Add(new ApplicationValue() { DbName = "Description", Value = "Test description " + i });
                    state.Values.Add(new ApplicationValue() { DbName = "BoolValue", Value = true });
                    state.Values.Add(new ApplicationValue() { DbName = "IntValue", Value = 25 + i });
                    state.Values.Add(new ApplicationValue() { DbName = "DecValue", Value = 777.77 });
                    var subtable = new ApplicationTable() { DbName = "TestAppSubTable" };
                    var row = new ApplicationTableRow() { Table = subtable };
                    row.Values.Add(new ApplicationValue() { DbName = "LineHeader", Value = "First Row" });
                    row.Values.Add(new ApplicationValue() { DbName = "LineDescription", Value = "First Row Description" });
                    subtable.Rows.Add(row);
                    row = new ApplicationTableRow() { Table = subtable };
                    row.Values.Add(new ApplicationValue() { DbName = "LineHeader", Value = "Second Row" });
                    row.Values.Add(new ApplicationValue() { DbName = "LineDescription", Value = "Second Row Description" });
                    subtable.Rows.Add(row);
                    state.SubTables.Add(subtable);

                    var saveresult = _dataservice.Save(state);
                    if (!saveresult.IsSuccess)
                        throw new InvalidOperationException("IntwentyDataService.Save() intwenty application failed: " + saveresult.SystemError);


                }

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private OperationResult Test8GetListOfIntwentyApplication()
        {
            OperationResult result = new OperationResult(true, "Get a list just of created intwenty applications");
            try
            {
                var getlistresult = _dataservice.GetList(10000);
                if (!getlistresult.IsSuccess)
                    throw new InvalidOperationException("IntwentyDataService.GetList(1000) failed: " + getlistresult.SystemError);

                var state = ClientStateInfo.CreateFromJSON(System.Text.Json.JsonDocument.Parse(getlistresult.Data).RootElement);
                if (state.SubTables.Count < 1)
                    throw new InvalidOperationException("Could not create ClientStateInfo.SubTable from string json array");

                if (state.SubTables[0].Rows.Count < 5)
                    throw new InvalidOperationException("Could not get list of intwenty applications, should be at least 5 records");

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private OperationResult Test9GetListOfIntwentyApplicationByOwnerUser()
        {
            OperationResult result = new OperationResult(true, "Get a list of created intwenty applications by owner user");
            try
            {
                var getlistresult = _dataservice.GetListByOwnerUser(10000, "OTHERUSER");
                if (!getlistresult.IsSuccess)
                    throw new InvalidOperationException("IntwentyDataService.GetListByOwnerUser(1000, OTHERUSER) failed: " + getlistresult.SystemError);

                var state = ClientStateInfo.CreateFromJSON(System.Text.Json.JsonDocument.Parse(getlistresult.Data).RootElement);
                if (state.SubTables.Count < 1)
                    throw new InvalidOperationException("Could not create ClientStateInfo.SubTable from string json array");

                if (state.SubTables[0].Rows.Count != 2)
                    throw new InvalidOperationException("Could not get list of intwenty applications owned by OTHERUSER, should be 2 records");

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private OperationResult Test10GetLatestVersionByOwnerUser()
        {
            OperationResult result = new OperationResult(true, "Get the latest version of an intwenty application by owner user");
            try
            {
                var state = new ClientStateInfo();
                state.ApplicationId = 10000;
                state.OwnerUserId = "OTHERUSER";
                var getresult = _dataservice.GetLatestVersionByOwnerUser(state);
                if (!getresult.IsSuccess)
                    throw new InvalidOperationException("IntwentyDataService.GetLatestVersionByOwnerUser(state) failed: " + getresult.SystemError);

                var newstate = ClientStateInfo.CreateFromJSON(System.Text.Json.JsonDocument.Parse(getresult.Data).RootElement);
                if (newstate.Values.Count < 1)
                    throw new InvalidOperationException("Could not create ClientStateInfo from application json string");


            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private OperationResult Test11UpdateIntwentyApplication()
        {
            OperationResult result = new OperationResult(true, "Update intwenty application");
            try
            {
                var state = new ClientStateInfo();
                state.ApplicationId = 10000;
                state.OwnerUserId = "OTHERUSER";
                var getresult = _dataservice.GetLatestVersionByOwnerUser(state);
                if (!getresult.IsSuccess)
                    throw new InvalidOperationException("IntwentyDataService.GetLatestVersionByOwnerUser(state) failed: " + getresult.SystemError);

                var newstate = ClientStateInfo.CreateFromJSON(System.Text.Json.JsonDocument.Parse(getresult.Data).RootElement);
                if (newstate.Values.Count < 1)
                    throw new InvalidOperationException("Could not create ClientStateInfo from application json string");

                newstate.OwnerUserId = "OTHERUSER2";

                var v1 = newstate.Values.Find(p => p.DbName == "Description");
                v1.Value = "Updated test application";

                var v2 = newstate.Values.Find(p => p.DbName == "DecValue");
                v2.Value = 333.777M;

                var v3 = newstate.Values.Find(p => p.DbName == "DecValue2");
                if (v3 == null)
                {
                    v3 = new ApplicationValue() { DbName = "DecValue2" };
                    newstate.Values.Add(v3);
                }
                v3.Value = 444.55;

                var saveresult = _dataservice.Save(newstate);
                if (!saveresult.IsSuccess)
                    throw new InvalidOperationException("IntwentyDataService.Save(state) failed when updating application: " + getresult.SystemError);

                var getbyidresult = _dataservice.GetLatestVersionById(state);
                if (!getbyidresult.IsSuccess)
                    throw new InvalidOperationException("IntwentyDataService.GetLatestVersionById(state) failed: " + getresult.SystemError);

                newstate = ClientStateInfo.CreateFromJSON(System.Text.Json.JsonDocument.Parse(getbyidresult.Data).RootElement);
                if (newstate.Values.Count < 1)
                    throw new InvalidOperationException("Could not create ClientStateInfo from application json string");

                var checkupdate = newstate.Values.Find(p => p.DbName == "Description");
                if (checkupdate.GetAsString() != "Updated test application")
                    throw new InvalidOperationException("Updated application string value was not persisted");

                checkupdate = newstate.Values.Find(p => p.DbName == "DecValue");
                if (checkupdate.GetAsDecimal() != 333.777M)
                    throw new InvalidOperationException("Updated application decimal value was not persisted");

                checkupdate = newstate.Values.Find(p => p.DbName == "DecValue2");
                if (Convert.ToDouble(checkupdate.GetAsDecimal()) != 444.55)
                    throw new InvalidOperationException("Updated application decimal double value was not persisted");

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private OperationResult Test12GetAllValueDomains()
        {
            OperationResult result = new OperationResult(true, "Get all value domain items");
            try
            {

                var vd = _dataservice.GetValueDomains();
                if (!vd.IsSuccess)
                    throw new InvalidOperationException("IntwentyDataService.GetValueDomains() failed: " + vd.SystemError);

                var state = ClientStateInfo.CreateFromJSON(System.Text.Json.JsonDocument.Parse(vd.Data).RootElement);
                if (state.SubTables.Count < 1)
                    throw new InvalidOperationException("Could not create ClientStateInfo.SubTable from string json array");

                if (!state.SubTables.Exists(p=> p.DbName == "VALUEDOMAIN_TESTDOMAIN"))
                    throw new InvalidOperationException("Could not get list of intwenty value domain items.");


            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private OperationResult Test13GetDataSet()
        {
            OperationResult result = new OperationResult(true, "Get informationstatus dataset <InformationStatus>");
            try
            {

                ApplicationTable tbl = null;
                if (_settings.IsNoSQL)
                {
                    var dbstore = new IntwentyNoSqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);
                    tbl = dbstore.GetDataSet("sysdata_InformationStatus");
                }
                else
                {
                    var dbstore = new IntwentySqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);
                    dbstore.Open();
                    dbstore.CreateCommand("select * from sysdata_InformationStatus");
                    tbl = dbstore.GetDataSet();
                    dbstore.Close();
                }

                if (tbl == null)
                    throw new InvalidOperationException("GetDataSet on sysdata_InformationStatus returned null");

                if (tbl.Rows.Count == 0)
                    throw new InvalidOperationException("GetDataSet on sysdata_InformationStatus returned 0 rows");

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }



    }

   



    
    [DbTablePrimaryKey("Id")]
    [DbTableName("tests_TestDataAutoInc")]
    public class TestDataAutoInc {

        [BsonId]
        [AutoIncrement]
        public int Id { get; set; }

        public string Header { get; set; }

        public string Description { get; set; }

        public int IntValue { get; set; }

        public bool BoolValue { get; set; }

        public decimal DecimalValue { get; set; }

        public float FloatValue { get; set; }

        public double DoubleValue { get; set; }

    }

    [DbTableIndex("DBTESTDATA_IDX_1", false, "Header")]
    [DbTableIndex("DBTESTDATA_IDX_2", true, "IntValue")]
    [DbTablePrimaryKey("Id, Type")]
    [DbTableName("tests_TestDataIndexNoAutoInc")]
    public class TestData
    {

        [BsonId]
        public int Id { get; set; }

        public string Type { get; set; }

        public string Header { get; set; }

        public string Description { get; set; }

        public int IntValue { get; set; }

        public bool BoolValue { get; set; }

        public decimal DecimalValue { get; set; }

        public float FloatValue { get; set; }

        public double DoubleValue { get; set; }




    }


}
