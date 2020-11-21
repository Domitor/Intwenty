﻿using Microsoft.AspNetCore.Mvc;
using Intwenty.Model;
using Intwenty.Model.Dto;
using Intwenty.Entity;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Intwenty.Areas.Identity.Entity;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Intwenty.PushData;
using Intwenty.DataClient;
using Intwenty.DataClient.Model;
using Intwenty.DataClient.Reflection;
using Intwenty.Interface;
using Intwenty.Areas.Identity.Data;

namespace Intwenty.Controllers
{


    public class SystemTestController : Controller
    {

        private readonly IIntwentyModelService _modelservice;
        private readonly IIntwentyDataService _dataservice;
        private readonly IntwentySettings _settings;
        private readonly IntwentyUserManager _usermanager;
        private readonly SignInManager<IntwentyUser> _signinmanager;
        private readonly RoleManager<IntwentyRole> _rolemanager;
        private readonly IHubContext<ServerToClientPush> _hubContext;

        public SystemTestController(IIntwentyModelService modelservice,
                                    IIntwentyDataService dataservice,
                                    IOptions<IntwentySettings> settings,
                                    IntwentyUserManager usermgr,
                                    SignInManager<IntwentyUser> signinmgr,
                                    RoleManager<IntwentyRole> rolemgr,
                                    IHubContext<ServerToClientPush> hubcontext)
        {
            _modelservice = modelservice;
            _dataservice = dataservice;
            _settings = settings.Value;
            _usermanager = usermgr;
            _signinmanager = signinmgr;
            _rolemanager = rolemgr;
            _hubContext = hubcontext;

        }

        public IActionResult RunTests()
        {

            return View();
        }


        [HttpPost]
        public JsonResult RunSystemTests()
        {
            var res = new List<TestResult>();

            //CLEAN UP PREV TEST
            var model = _modelservice.GetAppModels().Find(p => p.Id == 10000);
          

            var dvmodels = _modelservice.GetDataViewModels().Where(p => p.MetaCode == "DVEVENTLOG" || p.ParentMetaCode == "DVEVENTLOG").ToList();
           

            var db = new Connection(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);
            db.Open();

            db.RunCommand("DELETE FROM sysmodel_ValueDomainItem WHERE DOMAINNAME = 'TESTDOMAIN'");

            if (db.TableExists(model.DbName))
            {
                db.RunCommand(string.Format("DROP TABLE {0}", model.DbName));
            }
            if (db.TableExists(model.VersioningTableName))
            {
                db.RunCommand(string.Format("DROP TABLE {0}", model.VersioningTableName));
            }
            if (db.TableExists("def_TestAppSubTable"))
            {
                db.RunCommand("DROP TABLE def_TestAppSubTable");
            }
            if (db.TableExists("tests_TestDataAutoInc"))
            {
                db.RunCommand("DROP TABLE tests_TestDataAutoInc");
            }
            if (db.TableExists("tests_TestDataIndexNoAutoInc"))
            {
                db.RunCommand("DROP TABLE tests_TestDataIndexNoAutoInc");
            }

            if (db.TableExists("tests_TestData2AutoInc"))
            {
                db.RunCommand("DROP TABLE tests_TestData2AutoInc");
            }

            db.Close();

            if (model != null)
                _modelservice.DeleteAppModel(model);

            if (dvmodels != null && dvmodels.Count > 0)
            {
                foreach (var dv in dvmodels)
                    _modelservice.DeleteDataViewModel(dv.Id);
            }

            _hubContext.Clients.All.SendAsync("ReceiveMessage", Test1ORMCreateTable());
            _hubContext.Clients.All.SendAsync("ReceiveMessage", Test2ORMInsert());
            _hubContext.Clients.All.SendAsync("ReceiveMessage", Test3ORMUpdate());
            _hubContext.Clients.All.SendAsync("ReceiveMessage", Test4ORMDelete());
            _hubContext.Clients.All.SendAsync("ReceiveMessage", Test5CreateIntwentyDb());
            _hubContext.Clients.All.SendAsync("ReceiveMessage", Test6CreateIntwentyExampleModel());
            _hubContext.Clients.All.SendAsync("ReceiveMessage", Test7CreateIntwentyApplication());
            _hubContext.Clients.All.SendAsync("ReceiveMessage", Test8GetListOfIntwentyApplication());
            _hubContext.Clients.All.SendAsync("ReceiveMessage", Test9GetListOfIntwentyApplicationByOwnerUser());
            _hubContext.Clients.All.SendAsync("ReceiveMessage", Test10GetLatestVersionByOwnerUser());
            _hubContext.Clients.All.SendAsync("ReceiveMessage", Test11UpdateIntwentyApplication());
            _hubContext.Clients.All.SendAsync("ReceiveMessage", Test111GetTypedApplication());
            _hubContext.Clients.All.SendAsync("ReceiveMessage", Test12DeleteIntwentyApplication());
            _hubContext.Clients.All.SendAsync("ReceiveMessage", Test13GetAllValueDomains());
            _hubContext.Clients.All.SendAsync("ReceiveMessage", Test14GetDataSet());
            _hubContext.Clients.All.SendAsync("ReceiveMessage", Test15Transactions());
            _hubContext.Clients.All.SendAsync("ReceiveMessage", Test16CachePerformance());
            _hubContext.Clients.All.SendAsync("ReceiveMessage", Test17GetLists());
            _hubContext.Clients.All.SendAsync("ReceiveMessage", Test18TestIdentity());
            _hubContext.Clients.All.SendAsync("ReceiveMessage", Test500GetDataView());
           


            //TEST ALL SUPPORTED DB
            if (!string.IsNullOrEmpty(_settings.TestDbConnectionSqlite))
            {
                _hubContext.Clients.All.SendAsync("ReceiveMessage", Test100SqliteInsertPerformance());
                _hubContext.Clients.All.SendAsync("ReceiveMessage", Test101SqliteGetJSONArrayPerformance());
                _hubContext.Clients.All.SendAsync("ReceiveMessage", Test102SqliteGetEntitiesPerformance());
                _hubContext.Clients.All.SendAsync("ReceiveMessage", Test103SqliteGetResultSetPerformance());
                _hubContext.Clients.All.SendAsync("ReceiveMessage", Test104SqliteGetDataTablePerformance());
            }

            if (!string.IsNullOrEmpty(_settings.TestDbConnectionSqlServer))
            {
                _hubContext.Clients.All.SendAsync("ReceiveMessage", Test200SqlServerInsertPerformance());
                _hubContext.Clients.All.SendAsync("ReceiveMessage", Test201SqlServerGetJSONArrayPerformance());
                _hubContext.Clients.All.SendAsync("ReceiveMessage", Test202SqlServerGetEntitiesPerformance());
                _hubContext.Clients.All.SendAsync("ReceiveMessage", Test203SqlServerGetResultSetPerformance());
                _hubContext.Clients.All.SendAsync("ReceiveMessage", Test204SqlServerGetDataTablePerformance());
            }

            if (!string.IsNullOrEmpty(_settings.TestDbConnectionMariaDb))
            {
                _hubContext.Clients.All.SendAsync("ReceiveMessage", Test300MariaDbInsertPerformance());
                _hubContext.Clients.All.SendAsync("ReceiveMessage", Test301MariaDbGetJSONArrayPerformance());
                _hubContext.Clients.All.SendAsync("ReceiveMessage", Test302MariaDbGetEntitiesPerformance());
                _hubContext.Clients.All.SendAsync("ReceiveMessage", Test303MariaDbGetResultSetPerformance());
                _hubContext.Clients.All.SendAsync("ReceiveMessage", Test304MariaDbGetDataTablePerformance());
            }

            if (!string.IsNullOrEmpty(_settings.TestDbConnectionPostgres))
            {
                int expected = 5000;
                for (int i = 0; i < 5; i++)
                {
                    if (i==0)
                        _hubContext.Clients.All.SendAsync("ReceiveMessage", Test400PostgresInsertPerformance(true));
                    else
                        _hubContext.Clients.All.SendAsync("ReceiveMessage", Test400PostgresInsertPerformance(false));

                    _hubContext.Clients.All.SendAsync("ReceiveMessage", Test401PostgresGetJSONArrayPerformance(expected));
                    _hubContext.Clients.All.SendAsync("ReceiveMessage", Test402PostgresGetEntitiesPerformance(expected));
                    _hubContext.Clients.All.SendAsync("ReceiveMessage", Test403PostgresGetResultSetPerformance(expected));
                    _hubContext.Clients.All.SendAsync("ReceiveMessage", Test404PostgresGetDataTablePerformance(expected));
                    expected += 5000;
                }
            }


            return new JsonResult(res);

        }


        private TestResult Test1ORMCreateTable()
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, "DataClient.CreateTable<T>");
            try
            {

                var dbstore = new Connection(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);
                dbstore.Open();
                dbstore.CreateTable<TestDataAutoInc>();
                dbstore.Close();

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: Test1ORMCreateTable (DataClient.CreateTable) lasted  {0} ms", result.Duration));

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test2ORMInsert()
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, "DataClient.InsertEntity(T) - 100 Records using auto increment");
            var dbstore = new Connection(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);

            try
            {
                dbstore.Open();
                for (int i = 0; i < 100; i++)
                {
                    var t = new TestDataAutoInc() { BoolValue = true, IntValue = 777 + i, DecimalValue = 666.66M, Description = "Test data record/document " + i, Header = "Test2ORMInsertTable", FloatValue = 666.66F };
                    dbstore.InsertEntity(t);

                }

                var check = dbstore.GetEntities<TestDataAutoInc>();
                if (check.Count != 100)
                    throw new InvalidOperationException("Could not retrieve 100 inserted records with IIntwentyDbORM.GetAll<T>");

                if (check.Exists(p => p.Id < 1))
                    throw new InvalidOperationException("AutoInc failed on DataClient.InsertEntity<T>");

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: Test2ORMInsert (Create 100 records and retrieve them) lasted  {0} ms", result.Duration));

                dbstore.Close();
            }
            catch (Exception ex)
            {
                dbstore.Close();
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test3ORMUpdate()
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, "DataClient.Update(T) - Retrieve last record and update");
            var dbstore = new Connection(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);

            try
            {

               
                dbstore.Open();
                var check = dbstore.GetEntities<TestDataAutoInc>();
                if (check.Count < 100)
                    throw new InvalidOperationException("Test could not be performed beacause of dependency to previous test.");


                var last = check[check.Count - 1];
                var checkone = dbstore.GetEntity<TestDataAutoInc>(last.Id);
                if (checkone == null)
                    throw new InvalidOperationException("Could not retrieve last inserted record with IIntwentyDbORM.GetOne<T>");


                last.Header = "Test3ORMUpdateTable";
                last.IntValue = 555;
                last.FloatValue = 555.55F;
                last.DecimalValue = 555.55M;
                last.BoolValue = true;

                dbstore.UpdateEntity(last);
                checkone = dbstore.GetEntity<TestDataAutoInc>(last.Id);
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

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: Test3ORMUpdate lasted (GetAll, GetOne, Update) {0} ms", result.Duration));
                dbstore.Close();
            }
            catch (Exception ex)
            {
                dbstore.Close();
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test4ORMDelete()
        {

            TestResult result = new TestResult(true, MessageCode.RESULT, "DataClient.Delete(T) - Retrieve a list of inserted records and delete them one by one");
            var dbstore = new Connection(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);

            try
            {

                dbstore.Open();
                var check = dbstore.GetEntities<TestDataAutoInc>();
                if (check.Count < 100)
                    throw new InvalidOperationException("Test could not be performed beacause of dependency to previous test.");

                foreach (var t in check)
                {
                    dbstore.DeleteEntity(t);
                }

                check = dbstore.GetEntities<TestDataAutoInc>();
                if (check.Count > 0)
                    throw new InvalidOperationException("The deleted records was still present in the data store after deletion");


                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: Test4ORMDelete (Delete 100 records one by one) lasted  {0} ms", result.Duration));
                dbstore.Close();
            }
            catch (Exception ex)
            {
                dbstore.Close();
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test5CreateIntwentyDb()
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, "IIntwentyModelService.CreateIntwentyDatabase()");
            try
            {

                _modelservice.CreateIntwentyDatabase();

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: Test5CreateIntwentyDb lasted  {0} ms", result.Duration));

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test6CreateIntwentyExampleModel()
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, "Create an intwenty application (My test application)");
            var dbstore = new Connection(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);

            try
            {

                var system = _modelservice.GetSystemModels().Find(p => p.MetaCode == "INTWENTYDEFAULTSYS");
                if (system == null)
                    throw new InvalidOperationException("The default system could not be found");

                dbstore.Open();
                dbstore.InsertEntity(new ApplicationItem() { Id = 10000, SystemMetaCode=system.MetaCode, Description = "An app for testing intwenty", MetaCode = "TESTAPP", Title = "My test application", DbName = "def_TestApp", IsHierarchicalApplication = false, UseVersioning = true });
                dbstore.InsertEntity(new DatabaseItem() { SystemMetaCode = system.MetaCode, AppMetaCode = "TESTAPP", MetaType = "DATACOLUMN", MetaCode = "HEADER", DbName = "Header", ParentMetaCode = "ROOT", DataType = "STRING" });
                dbstore.InsertEntity(new DatabaseItem() { SystemMetaCode = system.MetaCode, AppMetaCode = "TESTAPP", MetaType = "DATACOLUMN", MetaCode = "DESCRIPTION", DbName = "Description", ParentMetaCode = "ROOT", DataType = "TEXT" });
                dbstore.InsertEntity(new DatabaseItem() { SystemMetaCode = system.MetaCode, AppMetaCode = "TESTAPP", MetaType = "DATACOLUMN", MetaCode = "BOOLVALUE", DbName = "BoolValue", ParentMetaCode = "ROOT", DataType = "BOOLEAN" });
                dbstore.InsertEntity(new DatabaseItem() { SystemMetaCode = system.MetaCode, AppMetaCode = "TESTAPP", MetaType = "DATACOLUMN", MetaCode = "INTVALUE", DbName = "IntValue", ParentMetaCode = "ROOT", DataType = "INTEGER" });
                dbstore.InsertEntity(new DatabaseItem() { SystemMetaCode = system.MetaCode, AppMetaCode = "TESTAPP", MetaType = "DATACOLUMN", MetaCode = "DECVALUE", DbName = "DecValue", ParentMetaCode = "ROOT", DataType = "3DECIMAL" });
                dbstore.InsertEntity(new DatabaseItem() { SystemMetaCode = system.MetaCode, AppMetaCode = "TESTAPP", MetaType = "DATACOLUMN", MetaCode = "DECVALUE2", DbName = "DecValue2", ParentMetaCode = "ROOT", DataType = "2DECIMAL" });
                dbstore.InsertEntity(new DatabaseItem() { SystemMetaCode = system.MetaCode, AppMetaCode = "TESTAPP", MetaType = "DATATABLE", MetaCode = "TESTAPP_SUBTABLE", DbName = "def_TestAppSubTable", ParentMetaCode = "ROOT" });
                dbstore.InsertEntity(new DatabaseItem() { SystemMetaCode = system.MetaCode, AppMetaCode = "TESTAPP", MetaType = "DATACOLUMN", MetaCode = "LINETEXT", DbName = "LineHeader", ParentMetaCode = "TESTAPP_SUBTABLE", DataType = "STRING" });
                dbstore.InsertEntity(new DatabaseItem() { SystemMetaCode = system.MetaCode, AppMetaCode = "TESTAPP", MetaType = "DATACOLUMN", MetaCode = "LINEDESCRIPTION", DbName = "LineDescription", ParentMetaCode = "TESTAPP_SUBTABLE", DataType = "STRING" });

                dbstore.InsertEntity(new ValueDomainItem() { DomainName = "TESTDOMAIN", Value = "Domain Value 1", Code = "1" });
                dbstore.InsertEntity(new ValueDomainItem() { DomainName = "TESTDOMAIN", Value = "Domain Value 2", Code = "2" });
                dbstore.InsertEntity(new ValueDomainItem() { DomainName = "TESTDOMAIN", Value = "Domain Value 2", Code = "3" });


                dbstore.InsertEntity(new DataViewItem() { MetaType = DataViewModelItem.MetaTypeDataView, MetaCode = "DVEVENTLOG", SQLQuery ="select id,verbosity,message from sysdata_eventlog", Title = "Eventlog" });
                dbstore.InsertEntity(new DataViewItem() { MetaType = DataViewModelItem.MetaTypeDataViewKeyColumn, MetaCode = "DVEVENTLOG_COL1", ParentMetaCode = "DVEVENTLOG", Title = "ID", SQLQueryFieldName = "id" });
                dbstore.InsertEntity(new DataViewItem() { MetaType = DataViewModelItem.MetaTypeDataViewColumn, MetaCode = "DVEVENTLOG_COL2", ParentMetaCode = "DVEVENTLOG", Title = "MsgType", SQLQueryFieldName = "verbosity" });
                dbstore.InsertEntity(new DataViewItem() { MetaType = DataViewModelItem.MetaTypeDataViewColumn, MetaCode = "DVEVENTLOG_COL3", ParentMetaCode = "DVEVENTLOG", Title = "Message", SQLQueryFieldName = "message" });
                dbstore.Close();

                _modelservice.ClearCache();

                var model = _modelservice.GetAppModels().Find(p => p.Id == 10000);
                if (model==null)
                    throw new InvalidOperationException("The created intwenty 'TESTAPP' model could not be found");

                var configres = _modelservice.ConfigureDatabase(model);
                
                if (!configres.IsSuccess)
                    throw new InvalidOperationException("The created intwenty model could not be configured with success");

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: Test6CreateIntwentyExampleModel lasted  {0} ms", result.Duration));
              
            }
            catch (Exception ex)
            {
                dbstore.Close();
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test7CreateIntwentyApplication()
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, "Create 100 intwenty application based on the created test model");

            try
            {
                for (int i = 0; i < 100; i++)
                {
                    var state = new ClientStateInfo();
                    state.ApplicationId = 10000;
                    state.UserId = "TESTUSER";

                    if (i > 25)
                        state.UserId = "OTHERUSER";
                    if (i > 50)
                        state.UserId = "OTHERUSER1";
                    if (i > 75)
                        state.UserId = "OTHERUSER2";

                    state.Data.Values.Add(new ApplicationValue() { DbName = "Header", Value = "Test Header " + i });
                    state.Data.Values.Add(new ApplicationValue() { DbName = "Description", Value = "Test description " + i });
                    state.Data.Values.Add(new ApplicationValue() { DbName = "BoolValue", Value = true });
                    state.Data.Values.Add(new ApplicationValue() { DbName = "IntValue", Value = 25 + i });
                    state.Data.Values.Add(new ApplicationValue() { DbName = "DecValue", Value = 777.77 });
                    var subtable = new ApplicationTable() { DbName = "def_TestAppSubTable" };
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

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: Test7CreateIntwentyApplication (Create 100 applications) lasted  {0} ms", result.Duration));

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test8GetListOfIntwentyApplication()
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, "Get a list just of created intwenty applications");

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

                var rownum = 0;
                var dataid = 0;
                var filter = new ListFilter() { ApplicationId = 10000, BatchSize = 10 };
                for (int i = 1; i < 4; i++)
                {
                    var pageresult = _dataservice.GetPagedList(filter);
                    if (pageresult.Data.Length < 20)
                        throw new InvalidOperationException("GetPagedList - No result");

                    if (pageresult.ListFilter.NextDataId == 0)
                        throw new InvalidOperationException("GetPagedList - ListFilter.CurrentDataId was 0");
                    if (pageresult.ListFilter.CurrentRowNum == 0)
                        throw new InvalidOperationException("GetPagedList - ListFilter.CurrentRowNum was 0");
                    if (pageresult.ListFilter.MaxCount == 0)
                        throw new InvalidOperationException("GetPagedList - ListFilter.MaxCount was 0");


                    if (i == 1)
                    {
                        rownum = pageresult.ListFilter.CurrentRowNum;
                        dataid = pageresult.ListFilter.NextDataId;
                    }
                    else
                    {
                        if (pageresult.ListFilter.CurrentRowNum == rownum)
                            throw new InvalidOperationException("GetPagedList - ListFilter.CurrentRowNum does not seem to increase when paging");
                        if (pageresult.ListFilter.NextDataId == dataid)
                            throw new InvalidOperationException("GetPagedList - ListFilter.CurrentDataId does not seem to increase when paging");
                    }

                }



                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: Test8GetListOfIntwentyApplication (Get 100 Applications) lasted  {0} ms", result.Duration));

                
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test9GetListOfIntwentyApplicationByOwnerUser()
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, "Get a list of created intwenty applications by owner user");

            try
            {
                var getlistresult = _dataservice.GetListByOwnerUser(10000, "OTHERUSER");
                if (!getlistresult.IsSuccess)
                    throw new InvalidOperationException("IntwentyDataService.GetListByOwnerUser(1000, OTHERUSER) failed: " + getlistresult.SystemError);

                var state = ClientStateInfo.CreateFromJSON(System.Text.Json.JsonDocument.Parse(getlistresult.Data).RootElement);
                if (state.Data.SubTables.Count < 1)
                    throw new InvalidOperationException("Could not create ClientStateInfo.SubTable from string json array");

                if (state.Data.SubTables[0].Rows.Count < 20)
                    throw new InvalidOperationException("Could not get list of intwenty applications owned by OTHERUSER, should be 20 records");

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: Test9GetListOfIntwentyApplicationByOwnerUser lasted  {0} ms", result.Duration));

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test10GetLatestVersionByOwnerUser()
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, "Get the latest version of an intwenty application by owner user");

            try
            {
                var state = new ClientStateInfo();
                state.ApplicationId = 10000;
                state.FilterValues.Add(new FilterValue() { Name = "OwnedBy", Value = "TESTUSER" });
                var getresult = _dataservice.GetLatestVersionByOwnerUser(state);
                if (!getresult.IsSuccess)
                    throw new InvalidOperationException("IntwentyDataService.GetLatestVersionByOwnerUser(state) failed: " + getresult.SystemError);

                var newstate = ClientStateInfo.CreateFromJSON(System.Text.Json.JsonDocument.Parse(getresult.Data).RootElement);
                if (newstate.Data.Values.Count < 1)
                    throw new InvalidOperationException("Could not create ClientStateInfo from application json string");

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: Test10GetLatestVersionByOwnerUser lasted  {0} ms", result.Duration));


            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test11UpdateIntwentyApplication()
        {

            TestResult result = new TestResult(true, MessageCode.RESULT, "Update intwenty application");

            try
            {
                var state = new ClientStateInfo();
                state.ApplicationId = 10000;
                state.FilterValues.Add(new FilterValue() { Name = "OwnedBy", Value = "OTHERUSER" });
                var getresult = _dataservice.GetLatestVersionByOwnerUser(state);
                if (!getresult.IsSuccess)
                    throw new InvalidOperationException("IntwentyDataService.GetLatestVersionByOwnerUser(state) failed: " + getresult.SystemError);

                var newstate = ClientStateInfo.CreateFromJSON(System.Text.Json.JsonDocument.Parse(getresult.Data).RootElement);
                if (!newstate.Data.HasData)
                    throw new InvalidOperationException("Could not create ClientStateInfo from application json string");

                newstate.UserId = "OTHERUSER2";
                newstate.Data.SetValue("Description", "Updated test application");
                newstate.Data.SetValue("DecValue", 333.777M);
                newstate.Data.SetValue("DecValue2", 444.55);

   

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

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: Test11UpdateIntwentyApplication (Get Application, Update, JSONOperations) lasted  {0} ms", result.Duration));

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test111GetTypedApplication()
        {

            TestResult result = new TestResult(true, MessageCode.RESULT, "Get typed intwenty application");

            try
            {
                var args = new ListFilter() { ApplicationId = 10000,  BatchSize = 500 };
                var model = _modelservice.GetApplicationModels().Find(p => p.Application.Id == 10000);
                if (model == null)
                    throw new InvalidOperationException("Model not found");

                var data = _dataservice.GetPagedList<def_TestApp>(args, model);
                if (!data.IsSuccess)
                    throw new InvalidOperationException(data.SystemError);

                if (data.Data.Count < 1)
                    throw new InvalidOperationException("No data found when using GetPagedList<def_TestApp>()");

                var state = new ClientStateInfo() { ApplicationId = 10000, Id = data.Data[0].Id };
                var data2 = _dataservice.GetLatestVersionById<def_TestApp>(state, model);
                if (!data2.IsSuccess)
                    throw new InvalidOperationException(data2.SystemError);

                if (data2.Data == null)
                    throw new InvalidOperationException("No data found when using GetLatestVersionById<def_TestApp>()");

                if (data2.Data.Id < 1)
                    throw new InvalidOperationException("No data found when using GetLatestVersionById<def_TestApp>()");

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: Test111GetTypedApplication (Get typed intwenty application) lasted  {0} ms", result.Duration));

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }


        private TestResult Test12DeleteIntwentyApplication()
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, "Delete an intwenty application");

            try
            {
                var state = new ClientStateInfo();
                state.ApplicationId = 10000;
                state.FilterValues.Add(new FilterValue() { Name = "OwnedBy", Value = "OTHERUSER" });
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

                var deleteresult = _dataservice.Delete(newstate);
                if (!deleteresult.IsSuccess)
                    throw new InvalidOperationException("IntwentyDataService.DeleteById(state) failed: " + deleteresult.SystemError);


                getbyidresult = _dataservice.GetLatestVersionById(newstate);
                if (getbyidresult.IsSuccess)
                    throw new InvalidOperationException("IntwentyDataService.GetLatestVersionById(state) returnded success but is deleted: " + getresult.SystemError);

                appdata = getbyidresult.GetAsApplicationData();
                if (appdata.HasData)
                    throw new InvalidOperationException("IntwentyDataService.GetLatestVersionById(state) returned values but is deleted");


                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: Test12DeleteIntwentyApplication (Delete Subtable Row, Delete Application) lasted  {0} ms", result.Duration));



            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test13GetAllValueDomains()
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, "Get all value domain items");
            try
            {

                var vd = _dataservice.GetValueDomains();
                if (!vd.IsSuccess)
                    throw new InvalidOperationException("IntwentyDataService.GetValueDomains() failed: " + vd.SystemError);

                var data = vd.GetAsApplicationData();
                var state = ClientStateInfo.CreateFromJSON(System.Text.Json.JsonDocument.Parse(vd.Data).RootElement);
                if (data.SubTables.Count < 1)
                    throw new InvalidOperationException("Could not create ApplicationData.SubTable from string json array");

                if (!data.SubTables.Exists(p => p.DbName == "VALUEDOMAIN_TESTDOMAIN"))
                    throw new InvalidOperationException("Could not get list of intwenty value domain items.");

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: Test13GetAllValueDomains lasted  {0} ms", result.Duration));
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test14GetDataSet()
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, "Get eventlog dataset <EventLog>");

            try
            {

                var dbstore = new Connection(_settings.DefaultConnectionDBMS, _settings.DefaultConnection);
                dbstore.Open();
                var tbl = dbstore.GetResultSet("select * from sysdata_EventLog");
                dbstore.Close();
             

                if (tbl == null)
                    throw new InvalidOperationException("GetDataSet based sysdata_EventLog returned null");

                if (tbl.Rows.Count == 0)
                    throw new InvalidOperationException("GetDataSet on sysdata_EventLog returned 0 rows");

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: Test14GetDataSet (Eventlog records) lasted  {0} ms", result.Duration));

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test15Transactions()
        {

            TestResult result = new TestResult(true, MessageCode.RESULT, "DataClient.Transactions");
            try
            {

                var client = _dataservice.GetDataClient();
                client.Open();
                client.DeleteEntities<TestDataAutoInc>(client.GetEntities<TestDataAutoInc>());
                var count = (long)client.GetScalarValue("select count(*) from tests_TestDataAutoInc");
                if (count > 0)
                    throw new InvalidOperationException("Prepare test delete operation failed.");

                client.Close();


                client.Open();
                client.BeginTransaction();

                client.InsertEntity<TestDataAutoInc>(new TestDataAutoInc() { Description = "TRANS TEST 1" });
                client.InsertEntity<TestDataAutoInc>(new TestDataAutoInc() { Description = "TRANS TEST 2" });

                client.RollbackTransaction();

                count = (long)client.GetScalarValue("select count(*) from tests_TestDataAutoInc");
                client.Close();

                if (count > 0)
                    throw new InvalidOperationException("The table tests_TestDataAutoInc should be empty since transaction were rollbacked");

                client.Open();
                client.BeginTransaction();

                client.InsertEntity<TestDataAutoInc>(new TestDataAutoInc() { Description = "TRANS TEST 3" });
                client.InsertEntity<TestDataAutoInc>(new TestDataAutoInc() { Description = "TRANS TEST 4" });

                client.CommitTransaction();

                count = (long)client.GetScalarValue("select count(*) from tests_TestDataAutoInc");
                client.Close();

                if (count < 2)
                    throw new InvalidOperationException("The table tests_TestDataAutoInc should not be empty since transaction were commited");


                result.Finish();
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test16CachePerformance()
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, "Test logging and Intwenty application cache.");

            try
            {

                var state = new ClientStateInfo();
                state.ApplicationId = 10000;
                state.FilterValues.Add(new FilterValue() { Name = "OwnedBy", Value = "TESTUSER" });
                var getresult = _dataservice.GetLatestVersionByOwnerUser(state);
                if (!getresult.IsSuccess)
                    throw new InvalidOperationException("IntwentyDataService.GetLatestVersionByOwnerUser(state) failed: " + getresult.SystemError);

                var newstate = new ClientStateInfo();
                newstate.Id = getresult.Id;
                newstate.ApplicationId = 10000;

                for (var i = 0; i < 10; i++)
                {
                    getresult = _dataservice.GetLatestVersionById(newstate);
                    if (!getresult.IsSuccess)
                        throw new InvalidOperationException("IntwentyDataService.GetLatestVersionById(state) failed: " + getresult.SystemError);

                }

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: Test16CachePerformance (GetGetLatestVersionById 10 times) lasted  {0} ms", result.Duration));

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test17GetLists()
        {

            TestResult result = new TestResult(true, MessageCode.RESULT, "Test GetList(args) and paging");
            try
            {

                var args = new ListFilter();
                args.ApplicationId = 10000;
                args.BatchSize = 20;

                var getlistresult = _dataservice.GetPagedList(args);
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

                getlistresult = _dataservice.GetPagedList(args);
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

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: Test17GetLists (GetLists, JSON Operations) lasted  {0} ms", result.Duration));

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test18TestIdentity()
        {

            TestResult result = new TestResult(true, MessageCode.RESULT, "Test Asp.Net.Core.Identity.");

            try
            {
                var retrievedrole = _rolemanager.FindByNameAsync("TESTXROLE").Result;
                if (retrievedrole != null)
                    _rolemanager.DeleteAsync(retrievedrole);

                var retrieveduser = _usermanager.FindByNameAsync("systemtest@systemtest.com").Result;
                if (retrieveduser != null)
                {
                    _usermanager.RemoveFromRoleAsync(retrieveduser, "TESTXROLE");
                    _usermanager.DeleteAsync(retrieveduser);
                }

                var user = new IntwentyUser() { Email = "systemtest@systemtest.com", FirstName = "Testony", LastName = "Testson", UserName = "systemtest@systemtest.com" };
                _usermanager.CreateAsync(user, "testpassword");

                retrieveduser = _usermanager.FindByNameAsync(user.UserName).Result;
                if (retrieveduser == null)
                    throw new InvalidOperationException("The inserted user could not be retrieved, with FindByNameAsync");
                if (string.IsNullOrEmpty(retrieveduser.Id))
                    throw new InvalidOperationException("The inserted user has no id");

                var role = new IntwentyRole() { Name = "TESTXROLE", NormalizedName = "TESTXROLE" };
                _rolemanager.CreateAsync(role);
                retrievedrole = _rolemanager.FindByNameAsync("TESTXROLE").Result;
                if (retrievedrole == null)
                    throw new InvalidOperationException("The inserted role could not be retrieved");
                if (string.IsNullOrEmpty(retrievedrole.Id))
                    throw new InvalidOperationException("The inserted role has no id");

                _usermanager.AddToRoleAsync(retrieveduser, "TESTXROLE");
                if (!_usermanager.IsInRoleAsync(retrieveduser, "TESTXROLE").Result)
                    throw new InvalidOperationException("UserManager.IsInRoleAsync returned false for TESTXROLE, despite it was assigned to the user");


                var roles = _usermanager.GetRolesAsync(retrieveduser).Result;
                if (roles.Count != 1)
                    throw new InvalidOperationException("UserManager.GetRolesAsync() returned the wrong number of roles");

                _usermanager.RemoveFromRoleAsync(retrieveduser, "TESTXROLE");
                roles = _usermanager.GetRolesAsync(retrieveduser).Result;
                if (roles.Count > 0)
                    throw new InvalidOperationException("UserManager.GetRolesAsync() returned a role, despite it was removed from the user");


                _rolemanager.DeleteAsync(retrievedrole);
                retrievedrole = _rolemanager.FindByNameAsync("TESTXROLE").Result;
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

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: Test18TestIdentity lasted  {0} ms", result.Duration));

            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        //SQLite performance
        private TestResult Test100SqliteInsertPerformance()
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, "SQLite performance - Insert 5000 - Autoincrementation");
            var dbstore = new Connection(DBMS.SQLite, _settings.TestDbConnectionSqlite);

            try
            {
                dbstore.Open();

                dbstore.RunCommand("delete from tests_TestDataAutoInc");

                for (int i = 0; i < 5000; i++)
                {
                    var t = new TestDataAutoInc() { BoolValue = true, IntValue = 777 + i, DecimalValue = 666.66M, Description = "Test data record/document " + i, Header = "Test19SqlitePerformanec", FloatValue = 666.66F };
                    dbstore.InsertEntity(t);

                }

                var check = (long)dbstore.GetScalarValue("select count(*) from tests_TestDataAutoInc");
                if (check < 5000)
                    throw new InvalidOperationException("Insert 5000 failed");


                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: SQLite performance - Insert 5000 - Autoincrementation, lasted  {0} ms", result.Duration));

                dbstore.Close();
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test101SqliteGetJSONArrayPerformance()
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, "SQLite performance - GetJSONArray 5000");
            var dbstore = new Connection(DBMS.SQLite, _settings.TestDbConnectionSqlite);

            try
            {
                dbstore.Open();

                var check = (long)dbstore.GetScalarValue("select count(*) from tests_TestDataAutoInc");
                if (check < 5000)
                    throw new InvalidOperationException("Cannot run since prevoius case failed");

                var json = dbstore.GetJSONArray(new SqlQuery() { SqlStatement = "select * from tests_TestDataAutoInc" });
                if (json.Data.Length < 100)
                    throw new InvalidOperationException("JSON could not be feteched");

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: SQLite performance - GetJSONArray 5000, lasted  {0} ms", result.Duration));

                dbstore.Close();
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test102SqliteGetEntitiesPerformance()
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, "SQLite performance - GetEntities 5000");
            var dbstore = new Connection(DBMS.SQLite, _settings.TestDbConnectionSqlite);

            try
            {
                dbstore.Open();

                var check = (long)dbstore.GetScalarValue("select count(*) from tests_TestDataAutoInc");
                if (check < 5000)
                    throw new InvalidOperationException("Cannot run since prevoius case failed");

                var res = dbstore.GetEntities<TestDataAutoInc>();
                if (res.Count < 5000)
                    throw new InvalidOperationException("Entities could not be feteched");

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: SQLite performance - GetEntities 5000, lasted  {0} ms", result.Duration));

                dbstore.Close();
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test103SqliteGetResultSetPerformance()
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, "SQLite performance - GetResultSet 5000");
            var dbstore = new Connection(DBMS.SQLite, _settings.TestDbConnectionSqlite);

            try
            {
                dbstore.Open();

                var check = (long)dbstore.GetScalarValue("select count(*) from tests_TestDataAutoInc");
                if (check < 5000)
                    throw new InvalidOperationException("Cannot run since prevoius case failed");

                var res = dbstore.GetResultSet("select * from tests_TestDataAutoInc");
                if (res.Rows.Count < 5000)
                    throw new InvalidOperationException("ResultSet could not be feteched");

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: SQLite performance - GetResultSet 5000, lasted  {0} ms", result.Duration));

                dbstore.Close();
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test104SqliteGetDataTablePerformance()
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, "SQLite performance - GetDataTable 5000");
            var dbstore = new Connection(DBMS.SQLite, _settings.TestDbConnectionSqlite);

            try
            {
                dbstore.Open();

                var check = (long)dbstore.GetScalarValue("select count(*) from tests_TestDataAutoInc");
                if (check < 5000)
                    throw new InvalidOperationException("Cannot run since prevoius case failed");

                var res = dbstore.GetDataTable("select * from tests_TestDataAutoInc");
                if (res.Rows.Count < 5000)
                    throw new InvalidOperationException("DataTable could not be feteched");

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: SQLite performance - GetDataTable 5000, lasted  {0} ms", result.Duration));

                dbstore.Close();
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        //SQL Server performance
        private TestResult Test200SqlServerInsertPerformance()
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, "SQLServer performance - Insert 5000 - Autoincrementation");
            var dbstore = new Connection(DBMS.MSSqlServer, _settings.TestDbConnectionSqlServer);

            try
            {
                dbstore.Open();

                dbstore.RunCommand("delete from tests_TestDataAutoInc");

                for (int i = 0; i < 5000; i++)
                {
                    var t = new TestDataAutoInc() { BoolValue = true, IntValue = 777 + i, DecimalValue = 666.66M, Description = "Test data record/document " + i, Header = "Test200SqServerInsertPerformance", FloatValue = 666.66F };
                    dbstore.InsertEntity(t);

                }

                var check = (int)dbstore.GetScalarValue("select count(*) from tests_TestDataAutoInc");
                if (check < 5000)
                    throw new InvalidOperationException("Insert 5000 failed");


                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: SQLServer performance - Insert 5000 - Autoincrementation, lasted  {0} ms", result.Duration));

                dbstore.Close();
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test201SqlServerGetJSONArrayPerformance()
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, "SQLServer performance - GetJSONArray 5000");
            var dbstore = new Connection(DBMS.MSSqlServer, _settings.TestDbConnectionSqlServer);

            try
            {
                dbstore.Open();

                var check = (int)dbstore.GetScalarValue("select count(*) from tests_TestDataAutoInc");
                if (check < 5000)
                    throw new InvalidOperationException("Cannot run since prevoius case failed");

                var json = dbstore.GetJSONArray(new SqlQuery() { SqlStatement = "select * from tests_TestDataAutoInc" });
                if (json.Data.Length < 100)
                    throw new InvalidOperationException("JSON could not be feteched");

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: SQLServer performance - GetJSONArray 5000, lasted  {0} ms", result.Duration));

                dbstore.Close();
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test202SqlServerGetEntitiesPerformance()
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, "SQLServer performance - GetEntities 5000");
            var dbstore = new Connection(DBMS.MSSqlServer, _settings.TestDbConnectionSqlServer);

            try
            {
                dbstore.Open();

                var check = (int)dbstore.GetScalarValue("select count(*) from tests_TestDataAutoInc");
                if (check < 5000)
                    throw new InvalidOperationException("Cannot run since prevoius case failed");

                var res = dbstore.GetEntities<TestDataAutoInc>();
                if (res.Count < 5000)
                    throw new InvalidOperationException("Entities could not be feteched");

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: SQLServer performance - GetEntities 5000, lasted  {0} ms", result.Duration));

                dbstore.Close();
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test203SqlServerGetResultSetPerformance()
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, "SQLServer performance - GetResultSet 5000");
            var dbstore = new Connection(DBMS.MSSqlServer, _settings.TestDbConnectionSqlServer);

            try
            {
                dbstore.Open();

                var check = (int)dbstore.GetScalarValue("select count(*) from tests_TestDataAutoInc");
                if (check < 5000)
                    throw new InvalidOperationException("Cannot run since prevoius case failed");

                var res = dbstore.GetResultSet("select * from tests_TestDataAutoInc");
                if (res.Rows.Count < 5000)
                    throw new InvalidOperationException("ResultSet could not be feteched");

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: SQLServer performance - GetResultSet 5000, lasted  {0} ms", result.Duration));

                dbstore.Close();
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test204SqlServerGetDataTablePerformance()
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, "SQLServer performance - GetDataTable 5000");
            var dbstore = new Connection(DBMS.MSSqlServer, _settings.TestDbConnectionSqlServer);

            try
            {
                dbstore.Open();

                var check = (int)dbstore.GetScalarValue("select count(*) from tests_TestDataAutoInc");
                if (check < 5000)
                    throw new InvalidOperationException("Cannot run since prevoius case failed");

                var res = dbstore.GetDataTable("select * from tests_TestDataAutoInc");
                if (res.Rows.Count < 5000)
                    throw new InvalidOperationException("DataTable could not be feteched");

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: SQLServer performance - GetDataTable 5000, lasted  {0} ms", result.Duration));

                dbstore.Close();
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }


        //MariaDB performance
        private TestResult Test300MariaDbInsertPerformance()
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, "MariaDb performance - Insert 5000 - Autoincrementation");
            var dbstore = new Connection(DBMS.MariaDB, _settings.TestDbConnectionMariaDb);

            try
            {
                dbstore.Open();

                dbstore.RunCommand("delete from tests_TestDataAutoInc");

                for (int i = 0; i < 5000; i++)
                {
                    var t = new TestDataAutoInc() { BoolValue = true, IntValue = 777 + i, DecimalValue = 666.66M, Description = "Test data record/document " + i, Header = "Test300SMariaDbInsertPerformance", FloatValue = 666.66F };
                    dbstore.InsertEntity(t);

                }

                var check = (long)dbstore.GetScalarValue("select count(*) from tests_TestDataAutoInc");
                if (check < 5000)
                    throw new InvalidOperationException("Insert 5000 failed");


                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: MariaDb performance - Insert 5000 - Autoincrementation, lasted  {0} ms", result.Duration));

                dbstore.Close();
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test301MariaDbGetJSONArrayPerformance()
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, "MariaDb performance - GetJSONArray 5000");
            var dbstore = new Connection(DBMS.MariaDB, _settings.TestDbConnectionMariaDb);

            try
            {
                dbstore.Open();

                var check = (long)dbstore.GetScalarValue("select count(*) from tests_TestDataAutoInc");
                if (check < 5000)
                    throw new InvalidOperationException("Cannot run since prevoius case failed");

                var json = dbstore.GetJSONArray(new SqlQuery() { SqlStatement = "select * from tests_TestDataAutoInc" });
                if (json.Data.Length < 100)
                    throw new InvalidOperationException("JSON could not be feteched");

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: MariaDb performance - GetJSONArray 5000, lasted  {0} ms", result.Duration));

                dbstore.Close();
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test302MariaDbGetEntitiesPerformance()
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, "MariaDb performance - GetEntities 5000");
            var dbstore = new Connection(DBMS.MariaDB, _settings.TestDbConnectionMariaDb);

            try
            {
                dbstore.Open();

                var check = (long)dbstore.GetScalarValue("select count(*) from tests_TestDataAutoInc");
                if (check < 5000)
                    throw new InvalidOperationException("Cannot run since prevoius case failed");

                var res = dbstore.GetEntities<TestDataAutoInc>();
                if (res.Count < 5000)
                    throw new InvalidOperationException("Entities could not be feteched");

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: MariaDb performance - GetEntities 5000, lasted  {0} ms", result.Duration));

                dbstore.Close();
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test303MariaDbGetResultSetPerformance()
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, "MariaDb performance - GetResultSet 5000");
            var dbstore = new Connection(DBMS.MariaDB, _settings.TestDbConnectionMariaDb);

            try
            {
                dbstore.Open();

                var check = (long)dbstore.GetScalarValue("select count(*) from tests_TestDataAutoInc");
                if (check < 5000)
                    throw new InvalidOperationException("Cannot run since prevoius case failed");

                var res = dbstore.GetResultSet("select * from tests_TestDataAutoInc");
                if (res.Rows.Count < 5000)
                    throw new InvalidOperationException("ResultSet could not be feteched");

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: MariaDb performance - GetResultSet 5000, lasted  {0} ms", result.Duration));

                dbstore.Close();
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test304MariaDbGetDataTablePerformance()
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, "MariaDb performance - GetDataTable 5000");
            var dbstore = new Connection(DBMS.MariaDB, _settings.TestDbConnectionMariaDb);

            try
            {
                dbstore.Open();

                var check = (long)dbstore.GetScalarValue("select count(*) from tests_TestDataAutoInc");
                if (check < 5000)
                    throw new InvalidOperationException("Cannot run since prevoius case failed");

                var res = dbstore.GetDataTable("select * from tests_TestDataAutoInc");
                if (res.Rows.Count < 5000)
                    throw new InvalidOperationException("DataTable could not be feteched");

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: MariaDb performance - GetDataTable 5000, lasted  {0} ms", result.Duration));

                dbstore.Close();
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        //Postgres performance
        private TestResult Test400PostgresInsertPerformance(bool deleteprev)
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, string.Format("Postgres performance - Insert {0} - Autoincrementation", 5000));
            var dbstore = new Connection(DBMS.PostgreSQL, _settings.TestDbConnectionPostgres);

            try
            {
                dbstore.Open();

                if (deleteprev)
                    dbstore.RunCommand("delete from tests_TestDataAutoInc");

                for (int i = 0; i < 5000; i++)
                {
                    var t = new TestDataAutoInc() { BoolValue = true, IntValue = 777 + i, DecimalValue = 666.66M, Description = "Test data record/document " + i, Header = "Test400SPostgresInsertPerformance", FloatValue = 666.66F };
                    dbstore.InsertEntity(t);

                }

                if (deleteprev)
                {
                    var check = (long)dbstore.GetScalarValue("select count(*) from tests_TestDataAutoInc");
                    if (check < 5000)
                        throw new InvalidOperationException("Insert 5000 failed");
                }

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: Postgres performance - Insert {0} - Autoincrementation, lasted  {1} ms", 5000, result.Duration));

                dbstore.Close();
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test401PostgresGetJSONArrayPerformance(int expectedtotal)
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, string.Format("Postgres performance - GetJSONArray {0}", expectedtotal));
            var dbstore = new Connection(DBMS.PostgreSQL, _settings.TestDbConnectionPostgres);

            try
            {
                dbstore.Open();

                var check = (long)dbstore.GetScalarValue("select count(*) from tests_TestDataAutoInc");
                if (check < expectedtotal)
                    throw new InvalidOperationException("Cannot run since prevoius case failed");

                var json = dbstore.GetJSONArray(new SqlQuery() { SqlStatement = "select * from tests_TestDataAutoInc" });
                if (json.Data.Length < 100)
                    throw new InvalidOperationException("JSON could not be fetched");

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: Postgres performance - GetJSONArray {0}, lasted  {1} ms", expectedtotal, result.Duration));

                dbstore.Close();
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test402PostgresGetEntitiesPerformance(int expectedtotal)
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, string.Format("Postgres performance - GetEntities {0}", expectedtotal));
            var dbstore = new Connection(DBMS.PostgreSQL, _settings.TestDbConnectionPostgres);

            try
            {
                dbstore.Open();

                var check = (long)dbstore.GetScalarValue("select count(*) from tests_TestDataAutoInc");
                if (check < expectedtotal)
                    throw new InvalidOperationException("Cannot run since prevoius case failed");

                var res = dbstore.GetEntities<TestDataAutoInc>();
                if (res.Count < expectedtotal)
                    throw new InvalidOperationException("Entities could not be fetched");

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: Postgres performance - GetEntities {0}, lasted  {1} ms", expectedtotal, result.Duration));

                dbstore.Close();
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test403PostgresGetResultSetPerformance(int expectedtotal)
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, string.Format("Postgres performance - GetResultSet {0}", expectedtotal));
            var dbstore = new Connection(DBMS.PostgreSQL, _settings.TestDbConnectionPostgres);

            try
            {
                dbstore.Open();

                var check = (long)dbstore.GetScalarValue("select count(*) from tests_TestDataAutoInc");
                if (check < expectedtotal)
                    throw new InvalidOperationException("Cannot run since prevoius case failed");

                var res = dbstore.GetResultSet("select * from tests_TestDataAutoInc");
                if (res.Rows.Count < expectedtotal)
                    throw new InvalidOperationException("ResultSet could not be fetched");

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: Postgres performance - GetResultSet {0}, lasted  {1} ms", expectedtotal, result.Duration));

                dbstore.Close();
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test404PostgresGetDataTablePerformance(int expectedtotal)
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, string.Format("Postgres performance - GetDataTable {0}", expectedtotal));
            var dbstore = new Connection(DBMS.PostgreSQL, _settings.TestDbConnectionPostgres);

            try
            {
                dbstore.Open();

                var check = (long)dbstore.GetScalarValue("select count(*) from tests_TestDataAutoInc");
                if (check < expectedtotal)
                    throw new InvalidOperationException("Cannot run since prevoius case failed");

                var res = dbstore.GetDataTable("select * from tests_TestDataAutoInc");
                if (res.Rows.Count < expectedtotal)
                    throw new InvalidOperationException("DataTable could not be fetched");

                result.Finish();
                _dataservice.LogInfo(string.Format("Test Case: Postgres performance - GetDataTable {0}, lasted  {1} ms", expectedtotal, result.Duration));

                dbstore.Close();
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }

        private TestResult Test500GetDataView()
        {
            TestResult result = new TestResult(true, MessageCode.RESULT, "Get an intwenty DataView");

            try
            {

                var res = _dataservice.GetDataView(new ListFilter() { BatchSize = 1000000, DataViewMetaCode = "DVEVENTLOG" });
                if (!res.IsSuccess)
                    throw new InvalidOperationException("DataView could not execute");


                result.Finish();

                _dataservice.LogInfo(string.Format("Test Case: Get an intwenty DataView lasted  {0} ms", result.Duration));


            }
            catch (Exception ex)
            {
                result.SetError(ex.Message, "Test failed");
            }

            return result;
        }









    }



   
    public class def_TestApp : InformationHeader
    {
        public string Header { get; set; }

        public string Description { get; set; }

        public int IntValue { get; set; }

        public bool BoolValue { get; set; }

        public decimal DecValue { get; set; }


    }


    [DbTablePrimaryKey("Id")]
    [DbTableName("tests_TestDataAutoInc")]
    public class TestDataAutoInc {

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
