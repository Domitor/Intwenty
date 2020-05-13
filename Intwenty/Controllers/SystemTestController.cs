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
using Intwenty.Data.DBAccess.Helpers;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Intwenty.Data.Identity;
using System.Security.Claims;

namespace Intwenty.Controllers
{
    

    public class SystemTestController : Controller
    {

        private readonly IIntwentyModelService _modelservice;
        private readonly IIntwentyDataService _dataservice;
        private readonly IntwentySettings _settings;
        private readonly IMemoryCache _cache;
        private readonly UserManager<IntwentyUser> _usermanager;
        private readonly SignInManager<IntwentyUser> _signinmanager;
        private readonly RoleManager<IntwentyRole> _rolemanager;

        public SystemTestController(IIntwentyModelService modelservice, 
                                    IIntwentyDataService dataservice, 
                                    IOptions<IntwentySettings> settings,
                                    IMemoryCache cache,
                                    UserManager<IntwentyUser> usermgr,
                                    SignInManager<IntwentyUser> signinmgr,
                                    RoleManager<IntwentyRole> rolemgr)
        {
            _modelservice = modelservice;
            _dataservice = dataservice;
            _settings = settings.Value;
            _cache = cache;
            _usermanager = usermgr;
            _signinmanager = signinmgr;
            _rolemanager = rolemgr;
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
            res.Add(Test12DeleteIntwentyApplication());
            res.Add(Test13GetAllValueDomains());
            res.Add(Test14GetDataSet());
            res.Add(Test15ORMGetByExpression());
            res.Add(Test16CachePerformance());
            res.Add(Test17GetLists());
            res.Add(Test18TestIdentity());


            return new JsonResult(res);

        }


        private OperationResult Test1ORMCreateTable()
        {
            var start = DateTime.Now;
            OperationResult result = new OperationResult(true, "IIntwentyDbORM.CreateTable<T>");
            try
            {

                IIntwentyDbORM dbstore = null;
                if (_settings.IsNoSQL)
                    dbstore = new IntwentyNoSqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);
                else
                    dbstore = new IntwentySqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);

                dbstore.CreateTable<TestDataAutoInc>(true);

                _dataservice.LogInfo(string.Format("Test Case: Test1ORMCreateTable (IIntwentyDbORM.CreateTable) lasted  {0} ms", DateTime.Now.Subtract(start).TotalMilliseconds));

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private OperationResult Test2ORMInsert()
        {
            var start = DateTime.Now;
            OperationResult result = new OperationResult(true, "IIntwentyDbORM.Insert(T) - 100 Records using auto increment");
            try
            {

                IIntwentyDbORM dbstore = null;
                if (_settings.IsNoSQL)
                    dbstore = new IntwentyNoSqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);
                else
                    dbstore = new IntwentySqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);

                for (int i = 0; i < 100; i++)
                {
                    var t = new TestDataAutoInc() { BoolValue = true, IntValue = 777+i, DecimalValue = 666.66M, Description = "Test data record/document " + i, Header = "Test2ORMInsertTable", FloatValue = 666.66F };
                    dbstore.Insert(t);

                }


                var check = dbstore.GetAll<TestDataAutoInc>();
                if (check.Count != 100)
                    throw new InvalidOperationException("Could not retrieve 100 inserted records with IIntwentyDbORM.GetAll<T>");

                if (check.Exists(p=> p.Id < 1))
                    throw new InvalidOperationException("AutoInc failed on IIntwentyDbORM.Insert<T>");

                _dataservice.LogInfo(string.Format("Test Case: Test2ORMInsert (Create 100 records and retrieve them) lasted  {0} ms", DateTime.Now.Subtract(start).TotalMilliseconds));


            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private OperationResult Test3ORMUpdate()
        {
            var start = DateTime.Now;
            OperationResult result = new OperationResult(true, "IIntwentyDbORM.Update(T) - Retrieve last record and update");
            try
            {

                IIntwentyDbORM dbstore = null;
                if (_settings.IsNoSQL)
                    dbstore = new IntwentyNoSqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);
                else
                    dbstore = new IntwentySqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);


                var check = dbstore.GetAll<TestDataAutoInc>();
                if (check.Count < 100)
                    throw new InvalidOperationException("Test could not be performed beacause of dependency to previous test.");


                var last = check[check.Count - 1];
                start = DateTime.Now;
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


                _dataservice.LogInfo(string.Format("Test Case: Test3ORMUpdate lasted (GetAll, GetOne, Update) {0} ms", DateTime.Now.Subtract(start).TotalMilliseconds));

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private OperationResult Test4ORMDelete()
        {
            var start = DateTime.Now;
            OperationResult result = new OperationResult(true, "IIntwentyDbORM.Delete(T) - Retrieve a list of inserted records and delete them one by one");
            try
            {

                IIntwentyDbORM dbstore = null;
                if (_settings.IsNoSQL)
                    dbstore = new IntwentyNoSqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);
                else
                    dbstore = new IntwentySqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);


                var check = dbstore.GetAll<TestDataAutoInc>();
                if (check.Count < 100)
                    throw new InvalidOperationException("Test could not be performed beacause of dependency to previous test.");

                foreach (var t in check)
                {
                    dbstore.Delete(t);
                }

                check = dbstore.GetAll<TestDataAutoInc>();
                if (check.Count > 0)
                    throw new InvalidOperationException("The deleted records was still present in the data store after deletion");


                _dataservice.LogInfo(string.Format("Test Case: Test4ORMDelete (Delete 100 records one by one) lasted  {0} ms", DateTime.Now.Subtract(start).TotalMilliseconds));

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private OperationResult Test5CreateIntwentyDb()
        {
            var start = DateTime.Now;
            OperationResult result = new OperationResult(true, "IIntwentyModelService.CreateIntwentyDatabase()");
            try
            {

                _modelservice.CreateIntwentyDatabase();

                _dataservice.LogInfo(string.Format("Test Case: Test5CreateIntwentyDb lasted  {0} ms", DateTime.Now.Subtract(start).TotalMilliseconds));

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private OperationResult Test6CreateIntwentyExampleModel()
        {
            var start = DateTime.Now;
            OperationResult result = new OperationResult(true, "Create an intwenty application (My test application)");
            try
            {
                _cache.Remove("APPMODELS");

                IIntwentyDbORM dbstore = null;
                if (_settings.IsNoSQL)
                    dbstore = new IntwentyNoSqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);
                else
                    dbstore = new IntwentySqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);


                dbstore.Insert(new ApplicationItem() { Id = 10000, Description = "An app for testing intwenty", MetaCode = "TESTAPP", Title = "My test application", DbName = "TestApp", IsHierarchicalApplication = false, UseVersioning = true });
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

                _dataservice.LogInfo(string.Format("Test Case: Test6CreateIntwentyExampleModel lasted  {0} ms", DateTime.Now.Subtract(start).TotalMilliseconds));
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private OperationResult Test7CreateIntwentyApplication()
        {
            var start = DateTime.Now;
            OperationResult result = new OperationResult(true, "Create 100 intwenty application based on the created test model");
            try
            {
                for (int i = 0; i < 100; i++)
                {
                    var state = new ClientStateInfo();
                    state.ApplicationId = 10000;
                    state.UserId = "TESTUSER";
                    state.OwnerUserId = "TESTUSER";
                    if (i > 25)
                        state.OwnerUserId = "OTHERUSER";
                    if (i > 50)
                        state.OwnerUserId = "OTHERUSER1";
                    if (i > 75)
                        state.OwnerUserId = "OTHERUSER2";

                    state.Data.Values.Add(new ApplicationValue() { DbName = "Header", Value = "Test Header " + i });
                    state.Data.Values.Add(new ApplicationValue() { DbName = "Description", Value = "Test description " + i });
                    state.Data.Values.Add(new ApplicationValue() { DbName = "BoolValue", Value = true });
                    state.Data.Values.Add(new ApplicationValue() { DbName = "IntValue", Value = 25 + i });
                    state.Data.Values.Add(new ApplicationValue() { DbName = "DecValue", Value = 777.77 });
                    var subtable = new ApplicationTable() { DbName = "TestAppSubTable" };
                    var row = new ApplicationTableRow() { Table = subtable };
                    row.Values.Add(new ApplicationValue() { DbName = "LineHeader", Value = "First Row" });
                    row.Values.Add(new ApplicationValue() { DbName = "LineDescription", Value = "First Row Description" });
                    subtable.Rows.Add(row);
                    row = new ApplicationTableRow() { Table = subtable };
                    row.Values.Add(new ApplicationValue() { DbName = "LineHeader", Value = "Second Row" });
                    row.Values.Add(new ApplicationValue() { DbName = "LineDescription", Value = "Second Row Description" });
                    subtable.Rows.Add(row);
                    row = new ApplicationTableRow() { Table = subtable };
                    row.Values.Add(new ApplicationValue() { DbName = "LineHeader", Value = "Third Row" });
                    row.Values.Add(new ApplicationValue() { DbName = "LineDescription", Value = "Third Row Description" });
                    subtable.Rows.Add(row);
                    state.Data.SubTables.Add(subtable);

                    var saveresult = _dataservice.Save(state);
                    if (!saveresult.IsSuccess)
                        throw new InvalidOperationException("IntwentyDataService.Save() intwenty application failed: " + saveresult.SystemError);


                }

                _dataservice.LogInfo(string.Format("Test Case: Test7CreateIntwentyApplication (Create 100 applications) lasted  {0} ms", DateTime.Now.Subtract(start).TotalMilliseconds));

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private OperationResult Test8GetListOfIntwentyApplication()
        {
            var start = DateTime.Now;
            OperationResult result = new OperationResult(true, "Get a list just of created intwenty applications");
            try
            {
                var getlistresult = _dataservice.GetList(10000);
                if (!getlistresult.IsSuccess)
                    throw new InvalidOperationException("IntwentyDataService.GetList(1000) failed: " + getlistresult.SystemError);

                var state = ClientStateInfo.CreateFromJSON(System.Text.Json.JsonDocument.Parse(getlistresult.Data).RootElement);
                if (state.Data.SubTables.Count < 1)
                    throw new InvalidOperationException("Could not create ClientStateInfo.SubTable from string json array");

                if (state.Data.SubTables[0].Rows.Count < 5)
                    throw new InvalidOperationException("Could not get list of intwenty applications, should be at least 5 records");

                _dataservice.LogInfo(string.Format("Test Case: Test8GetListOfIntwentyApplication (Get 100 Applications) lasted  {0} ms", DateTime.Now.Subtract(start).TotalMilliseconds));
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private OperationResult Test9GetListOfIntwentyApplicationByOwnerUser()
        {
            var start = DateTime.Now;
            OperationResult result = new OperationResult(true, "Get a list of created intwenty applications by owner user");
            try
            {
                var getlistresult = _dataservice.GetListByOwnerUser(10000, "OTHERUSER");
                if (!getlistresult.IsSuccess)
                    throw new InvalidOperationException("IntwentyDataService.GetListByOwnerUser(1000, OTHERUSER) failed: " + getlistresult.SystemError);

                var state = ClientStateInfo.CreateFromJSON(System.Text.Json.JsonDocument.Parse(getlistresult.Data).RootElement);
                if (state.Data.SubTables.Count < 1)
                    throw new InvalidOperationException("Could not create ClientStateInfo.SubTable from string json array");

                if (state.Data.SubTables[0].Rows.Count < 20)
                    throw new InvalidOperationException("Could not get list of intwenty applications owned by OTHERUSER, should be 2 records");

                _dataservice.LogInfo(string.Format("Test Case: Test9GetListOfIntwentyApplicationByOwnerUser lasted  {0} ms", DateTime.Now.Subtract(start).TotalMilliseconds));

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private OperationResult Test10GetLatestVersionByOwnerUser()
        {
            var start = DateTime.Now;
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
                if (newstate.Data.Values.Count < 1)
                    throw new InvalidOperationException("Could not create ClientStateInfo from application json string");


                _dataservice.LogInfo(string.Format("Test Case: Test10GetLatestVersionByOwnerUser lasted  {0} ms", DateTime.Now.Subtract(start).TotalMilliseconds));


            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private OperationResult Test11UpdateIntwentyApplication()
        {
            var start = DateTime.Now;
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
                if (!newstate.Data.HasData)
                    throw new InvalidOperationException("Could not create ClientStateInfo from application json string");

                newstate.OwnerUserId = "OTHERUSER2";

                var v1 = newstate.Data.Values.Find(p => p.DbName == "Description");
                v1.Value = "Updated test application";

                var v2 = newstate.Data.Values.Find(p => p.DbName == "DecValue");
                v2.Value = 333.777M;

                var v3 = newstate.Data.Values.Find(p => p.DbName == "DecValue2");
                if (v3 == null)
                {
                    v3 = new ApplicationValue() { DbName = "DecValue2" };
                    newstate.Data.Values.Add(v3);
                }
                v3.Value = 444.55;

                var saveresult = _dataservice.Save(newstate);
                if (!saveresult.IsSuccess)
                    throw new InvalidOperationException("IntwentyDataService.Save(state) failed when updating application: " + getresult.SystemError);

                var getbyidresult = _dataservice.GetLatestVersionById(state);
                if (!getbyidresult.IsSuccess)
                    throw new InvalidOperationException("IntwentyDataService.GetLatestVersionById(state) failed: " + getresult.SystemError);

                var appdata = getbyidresult.GetAsApplicationData();
                if (!appdata.HasData)
                    throw new InvalidOperationException("Could not create applicationdata from application json string");

                var checkupdate = appdata.Values.Find(p => p.DbName == "Description");
                if (checkupdate.GetAsString() != "Updated test application")
                    throw new InvalidOperationException("Updated application string value was not persisted");

                checkupdate = appdata.Values.Find(p => p.DbName == "DecValue");
                if (checkupdate.GetAsDecimal() != 333.777M)
                    throw new InvalidOperationException("Updated application decimal value was not persisted");

                checkupdate = appdata.Values.Find(p => p.DbName == "DecValue2");
                if (Convert.ToDouble(checkupdate.GetAsDecimal()) != 444.55)
                    throw new InvalidOperationException("Updated application decimal double value was not persisted");

                if (_modelservice.GetApplicationModels().Exists(p => p.Application.Id == 10000 && p.Application.UseVersioning))
                {
                    if (newstate.Version < 2)
                        throw new InvalidOperationException("Updated application did not recieve a new version id");
                }

                getbyidresult.AddApplicationJSON("AddedTestString", "TestString", false);
                getbyidresult.AddApplicationJSON("AddedTestInt", 99, true);

                appdata = getbyidresult.GetAsApplicationData();

                getbyidresult.RemoveJSON("LineDescription");
                getbyidresult.RemoveJSON("Version");

                appdata = getbyidresult.GetAsApplicationData();

                _dataservice.LogInfo(string.Format("Test Case: Test11UpdateIntwentyApplication (Get Application, Update, JSONOperations) lasted  {0} ms", DateTime.Now.Subtract(start).TotalMilliseconds));

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private OperationResult Test12DeleteIntwentyApplication()
        {
            var start = DateTime.Now;
            OperationResult result = new OperationResult(true, "Delete an intwenty application");
            try
            {
                var state = new ClientStateInfo();
                state.ApplicationId = 10000;
                state.OwnerUserId = "OTHERUSER";
                var getresult = _dataservice.GetLatestVersionByOwnerUser(state);
                if (!getresult.IsSuccess)
                    throw new InvalidOperationException("IntwentyDataService.GetLatestVersionByOwnerUser(state) failed: " + getresult.SystemError);

                var appdata = getresult.GetAsApplicationData();

                //DELETE THE LAST SUBTABLE ROW IN APP
                var rowid = appdata.SubTables[0].Rows.Last().Id;
                var deleterowresult = _dataservice.DeleteById(appdata.ApplicationId, rowid, appdata.SubTables[0].DbName);
                if (!deleterowresult.IsSuccess)
                    throw new InvalidOperationException("IntwentyDataService.DeleteById(applicationid,id, dbname) failed when deleting row: " + deleterowresult.SystemError);

                var newstate = getresult.CreateClientState();
                var getbyidresult = _dataservice.GetLatestVersionById(newstate);
                if (!getbyidresult.IsSuccess)
                    throw new InvalidOperationException("IntwentyDataService.GetLatestVersionById(state) returned with failure: " + getresult.SystemError);

                appdata = getbyidresult.GetAsApplicationData();
                if (appdata.SubTables[0].Rows.Count != 2)
                    throw new InvalidOperationException("The deleted subtable row was returned when using IntwentyDataService.GetLatestVersionById(state)");

                var deleteresult = _dataservice.DeleteById(newstate);
                if (!deleteresult.IsSuccess)
                    throw new InvalidOperationException("IntwentyDataService.DeleteById(state) failed: " + deleteresult.SystemError);


                getbyidresult = _dataservice.GetLatestVersionById(newstate);
                if (getbyidresult.IsSuccess)
                    throw new InvalidOperationException("IntwentyDataService.GetLatestVersionById(state) returnded success but is deleted: " + getresult.SystemError);

                appdata = getbyidresult.GetAsApplicationData();
                if (appdata.HasData)
                    throw new InvalidOperationException("IntwentyDataService.GetLatestVersionById(state) returned values but is deleted");


                _dataservice.LogInfo(string.Format("Test Case: Test12DeleteIntwentyApplication (Delete Subtable Row, Delete Application) lasted  {0} ms", DateTime.Now.Subtract(start).TotalMilliseconds));



            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private OperationResult Test13GetAllValueDomains()
        {
            var start = DateTime.Now;
            OperationResult result = new OperationResult(true, "Get all value domain items");
            try
            {

                var vd = _dataservice.GetValueDomains();
                if (!vd.IsSuccess)
                    throw new InvalidOperationException("IntwentyDataService.GetValueDomains() failed: " + vd.SystemError);

                var data = vd.GetAsApplicationData();
                var state = ClientStateInfo.CreateFromJSON(System.Text.Json.JsonDocument.Parse(vd.Data).RootElement);
                if (data.SubTables.Count < 1)
                    throw new InvalidOperationException("Could not create ApplicationData.SubTable from string json array");

                if (!data.SubTables.Exists(p=> p.DbName == "VALUEDOMAIN_TESTDOMAIN"))
                    throw new InvalidOperationException("Could not get list of intwenty value domain items.");

                _dataservice.LogInfo(string.Format("Test Case: Test13GetAllValueDomains lasted  {0} ms", DateTime.Now.Subtract(start).TotalMilliseconds));
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private OperationResult Test14GetDataSet()
        {
            var start = DateTime.Now;
            OperationResult result = new OperationResult(true, "Get eventlog dataset <EventLog>");
            try
            {
                ApplicationTable tbl = null;
                if (_settings.IsNoSQL)
                {
                    var dbstore = new IntwentyNoSqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);
                    tbl = dbstore.GetDataSet("sysdata_EventLog");
                }
                else
                {
                    var dbstore = new IntwentySqlDbClient(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);
                    dbstore.Open();
                    dbstore.CreateCommand("select * from sysdata_EventLog");
                    tbl = dbstore.GetDataSet();
                    dbstore.Close();
                }

                if (tbl == null)
                    throw new InvalidOperationException("GetDataSet based sysdata_EventLog returned null");

                if (tbl.Rows.Count == 0)
                    throw new InvalidOperationException("GetDataSet on sysdata_EventLog returned 0 rows");

                _dataservice.LogInfo(string.Format("Test Case: Test14GetDataSet (Eventlog records) lasted  {0} ms", DateTime.Now.Subtract(start).TotalMilliseconds));

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private OperationResult Test15ORMGetByExpression()
        {
            var start = DateTime.Now;
            OperationResult result = new OperationResult(true, "GetByExpression<InformationStatus>(expression, parameters)");
            try
            {

                var dbstore = _dataservice.GetDbObjectMapper();
                var prms = new List<IntwentyParameter>();
                prms.Add(new IntwentyParameter() { ParameterName = "@MetaCode", Value = "TESTAPP" });
                prms.Add(new IntwentyParameter() { ParameterName = "@MetaType", Value = "APPLICATION" });
                prms.Add(new IntwentyParameter() { ParameterName = "@OwnedBy", Value = "OTHERUSER2" });
                var expression = new IntwentyExpression("(MetaCode = @MetaCode AND MetaType  = @MetaType) OR OwnedBy =@OwnedBy", prms);
                var tbl = dbstore.GetByExpression<InformationStatus>(expression);



                if (tbl == null)
                    throw new InvalidOperationException("GetByExpression<InformationStatus>(expression, parameters) returned null");

                if (tbl.Count == 0)
                    throw new InvalidOperationException("GetByExpression<InformationStatus>(expression, parameters) returned 0 rows");


                _dataservice.LogInfo(string.Format("Test Case: Test15ORMGetByExpression lasted  {0} ms", DateTime.Now.Subtract(start).TotalMilliseconds));

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private OperationResult Test16CachePerformance()
        {
            OperationResult result = new OperationResult(true, "Test logging and Intwenty application cache.");
            try
            {
                var start = DateTime.Now;

                var state = new ClientStateInfo();
                state.ApplicationId = 10000;
                state.OwnerUserId = "TESTUSER";
                var getresult = _dataservice.GetLatestVersionByOwnerUser(state);
                if (!getresult.IsSuccess)
                    throw new InvalidOperationException("IntwentyDataService.GetLatestVersionByOwnerUser(state) failed: " + getresult.SystemError);

                var newstate = new  ClientStateInfo();
                newstate.Id = getresult.Id;
                newstate.ApplicationId = 10000;

                for (var i = 0; i < 10; i++)
                {
                     start = DateTime.Now;
                     getresult = _dataservice.GetLatestVersionById(newstate);
                    if (!getresult.IsSuccess)
                        throw new InvalidOperationException("IntwentyDataService.GetLatestVersionById(state) failed: " + getresult.SystemError);

                }

                _dataservice.LogInfo(string.Format("Test Case: Test16CachePerformance (GetGetLatestVersionById 10 times) lasted  {0} ms", DateTime.Now.Subtract(start).TotalMilliseconds));

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private OperationResult Test17GetLists()
        {
            var start = DateTime.Now;
            OperationResult result = new OperationResult(true, "Test GetList(args) and paging");
            try
            {

                var args = new ListRetrivalArgs();
                args.ApplicationId = 10000;
                args.BatchSize = 20;

                var getlistresult = _dataservice.GetList(args);
                if (!getlistresult.IsSuccess)
                    throw new InvalidOperationException("IntwentyDataService.GetList(args) failed: " + getlistresult.SystemError);

                var state = ClientStateInfo.CreateFromJSON(System.Text.Json.JsonDocument.Parse(getlistresult.Data).RootElement);
                if (state.Data.SubTables.Count < 1)
                    throw new InvalidOperationException("Could not create ClientStateInfo.SubTable from string json array");

                if (state.Data.SubTables[0].Rows.Count != args.BatchSize)
                    throw new InvalidOperationException("The returned amount of records was different from batch size");

                var latestid = state.Data.SubTables[0].Rows.Max(p => p.Id);

                args.CurrentRowNum = 30;
                args.BatchSize = 10;

                getlistresult = _dataservice.GetList(args);
                if (!getlistresult.IsSuccess)
                    throw new InvalidOperationException("IntwentyDataService.GetList(args) failed: " + getlistresult.SystemError);

                state = ClientStateInfo.CreateFromJSON(System.Text.Json.JsonDocument.Parse(getlistresult.Data).RootElement);
                if (state.Data.SubTables.Count < 1)
                    throw new InvalidOperationException("Could not create ClientStateInfo.SubTable from string json array");

                var newlatestid = state.Data.SubTables[0].Rows.Min(p => p.Id);
                if (latestid >= newlatestid)
                    throw new InvalidOperationException("Paging not working properly");

                if (state.Data.SubTables[0].Rows.Count != args.BatchSize)
                    throw new InvalidOperationException("The returned amount of records was different from batch size");

                getlistresult = _dataservice.GetList(10000);
                if (!getlistresult.IsSuccess)
                    throw new InvalidOperationException("IntwentyDataService.GetList(applicationid) failed: " + getlistresult.SystemError);


                state = ClientStateInfo.CreateFromJSON(System.Text.Json.JsonDocument.Parse(getlistresult.Data).RootElement);
                if (state.Data.SubTables.Count < 1)
                    throw new InvalidOperationException("Could not create ClientStateInfo.SubTable from string json array");


                _dataservice.LogInfo(string.Format("Test Case: Test17GetLists (GetLists, JSON Operations) lasted  {0} ms", DateTime.Now.Subtract(start).TotalMilliseconds));

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private OperationResult Test18TestIdentity()
        {
            var start = DateTime.Now;
            OperationResult result = new OperationResult(true, "Test Asp.Net.Core.Identity.");
            try
            {
                var retrievedrole = _rolemanager.FindByNameAsync("SYSROLE").Result;
                if (retrievedrole != null)
                    _rolemanager.DeleteAsync(retrievedrole);

                var retrieveduser = _usermanager.FindByNameAsync("systemtest@systemtest.com").Result;
                if (retrieveduser != null)
                {
                    _usermanager.RemoveFromRoleAsync(retrieveduser, "SYSROLE");
                    _usermanager.DeleteAsync(retrieveduser);
                }

                var user = new IntwentyUser() { Email = "systemtest@systemtest.com", FirstName = "Testony", LastName = "Testson", UserName = "systemtest@systemtest.com" };
                _usermanager.CreateAsync(user, "testpassword");

                retrieveduser = _usermanager.FindByNameAsync(user.UserName).Result;
                if (retrieveduser == null)
                    throw new InvalidOperationException("The inserted user could not be retrieved, with FindByNameAsync");
                if (string.IsNullOrEmpty(retrieveduser.Id))
                    throw new InvalidOperationException("The inserted user has no id");

                var role = new IntwentyRole() { Name = "SYSROLE", NormalizedName = "SYSROLE" };
                _rolemanager.CreateAsync(role);
                retrievedrole = _rolemanager.FindByNameAsync("SYSROLE").Result;
                if (retrievedrole == null)
                    throw new InvalidOperationException("The inserted role could not be retrieved");
                if (string.IsNullOrEmpty(retrievedrole.Id))
                    throw new InvalidOperationException("The inserted role has no id");

                _usermanager.AddToRoleAsync(retrieveduser, "SYSROLE");
                if (!_usermanager.IsInRoleAsync(retrieveduser, "SYSROLE").Result)
                    throw new InvalidOperationException("UserManager.IsInRoleAsync returned false for SysRole, despite it was assigned to the user");


                var roles = _usermanager.GetRolesAsync(retrieveduser).Result;
                if (roles.Count != 1)
                    throw new InvalidOperationException("UserManager.GetRolesAsync() returned the wrong number of roles");

                _usermanager.RemoveFromRoleAsync(retrieveduser, "SYSROLE");
                roles = _usermanager.GetRolesAsync(retrieveduser).Result;
                if (roles.Count > 0)
                    throw new InvalidOperationException("UserManager.GetRolesAsync() returned a role, despite it was removed from the user");


                _rolemanager.DeleteAsync(retrievedrole);
                retrievedrole = _rolemanager.FindByNameAsync("SYSROLE").Result;
                if (retrievedrole != null)
                    throw new InvalidOperationException("The delete role could be retrieved despite it was deleted");

                _usermanager.DeleteAsync(retrieveduser);
                retrieveduser = _usermanager.FindByNameAsync(user.UserName).Result;
                if (retrieveduser != null)
                    throw new InvalidOperationException("The delete user could be retrieved despite it was deleted");


                /*
                var signinresult = _signinmanager.PasswordSignInAsync(retrieveduser.UserName, "testpassword", false, false).Result;
                if (!signinresult.Succeeded)
                    throw new InvalidOperationException("The inserted user could not be signed in");

                _signinmanager.SignOutAsync();
                */

                _dataservice.LogInfo(string.Format("Test Case: Test18TestIdentity lasted  {0} ms", DateTime.Now.Subtract(start).TotalMilliseconds));

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
