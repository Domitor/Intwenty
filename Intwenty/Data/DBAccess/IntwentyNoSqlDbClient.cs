using Intwenty.Data.DBAccess.Annotations;
using Intwenty.Data.DBAccess.Helpers;
using MongoDB.Driver;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intwenty.Data.Entity;
using MongoDB.Bson;
using System.IO;
using System.Data;
using Intwenty.Model;
using Intwenty.Data.Dto;
using MongoDB.Bson.Serialization;
using Intwenty.Data.Identity;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson.IO;
using System.Collections.Concurrent;

namespace Intwenty.Data.DBAccess
{
    public class IntwentyNoSqlDbClient : IntwentyDbClient, IIntwentyDbNoSql
    {

        private string DatabaseName { get; set; }

        private static ILiteDatabase LiteDbClient { get; set; }

        private static IMongoDatabase MongoDbClient { get; set; }

        private static DataTable EvalDT { get; set; }

        private static ConcurrentDictionary<string, object> Cache;

        //MongoDb Intwenty Class Maps
        private static BsonClassMap<IntwentyUser> IntwentyUserMongoDbMap { get; set; }
        private static BsonClassMap<IntwentyRole> IntwentyRoleMongoDbMap { get; set; }
        private static BsonClassMap<IntwentyUserRole> IntwentyUserRoleMongoDbMap { get; set; }
        private static BsonClassMap<ApplicationItem> ApplicationItemMongoDbMap { get; set; }
        private static BsonClassMap<DataViewItem> DataViewItemMongoDbMap { get; set; }
        private static BsonClassMap<DefaultValue> DefaultValueMongoDbMap { get; set; }
        private static BsonClassMap<EventLog> EventLogMongoDbMap { get; set; }
        private static BsonClassMap<InformationStatus> InformationStatusMongoDbMap { get; set; }
        private static BsonClassMap<MenuItem> MenuItemMongoDbMap { get; set; }
        private static BsonClassMap<SystemID> SystemIDMongoDbMap { get; set; }
        private static BsonClassMap<UserInterfaceItem> UserInterfaceItemMongoDbMap { get; set; }
        private static BsonClassMap<ValueDomainItem> ValueDomainItemMongoDbMap { get; set; }


        public IntwentyNoSqlDbClient(IntwentySettings settings)
        {
            Initialize(settings.DefaultConnectionDBMS, settings.DefaultConnection, "IntwentyDb");
        }

        public IntwentyNoSqlDbClient(DBMS d, string connectionstring, string databasename="IntwentyDb")
        {
            Initialize(d, connectionstring, databasename);
        }

        
        private void Initialize(DBMS d, string connectionstring, string databasename)
        {
            DbEngine = d;
            ConnectionString = connectionstring;
            DatabaseName = databasename;
            if (DbEngine != DBMS.MongoDb && DbEngine != DBMS.LiteDb)
                throw new InvalidOperationException("IntwentyNoSqlDbClient configured with wrong DBMS setting");

            if (Cache == null)
                Cache = new ConcurrentDictionary<string, object>();

            if (DbEngine == DBMS.LiteDb && LiteDbClient == null)
            {

                SetLiteDbConnectionString();
                LiteDbClient = new LiteDatabase(ConnectionString);
            }
            if (DbEngine == DBMS.MongoDb && MongoDbClient == null)
            {
                SetMongoDbClassMap();

                if (!ConnectionString.ToLower().Contains("mongodb"))
                    throw new InvalidOperationException("MongoDb connectionstring must contain mongodb");

                var client = new MongoClient(ConnectionString);
                MongoDbClient = client.GetDatabase(DatabaseName);

                
            }

        }



        private void SetMongoDbClassMap()
        {

            if (IntwentyUserMongoDbMap == null)
            {
                //IntwentyUserMongoDbMap = BsonClassMap.RegisterClassMap<IntwentyUser>(cm => { cm.AutoMap(); cm.MapIdMember(c => c.Id); });
                BsonClassMap.RegisterClassMap<IdentityUser<string>>(cm =>{cm.AutoMap();cm.MapIdMember(p => p.Id);cm.SetIsRootClass(true); });
                IntwentyUserMongoDbMap = BsonClassMap.RegisterClassMap<IntwentyUser>();
            }
            if (IntwentyRoleMongoDbMap == null)
            {
                //IntwentyRoleMongoDbMap = BsonClassMap.RegisterClassMap<IntwentyRole>(cm => { cm.AutoMap(); cm.MapIdMember(c => c.Id); });
                BsonClassMap.RegisterClassMap<IdentityRole<string>>(cm => { cm.AutoMap(); cm.MapIdMember(p => p.Id); cm.SetIsRootClass(true); });
                IntwentyRoleMongoDbMap=BsonClassMap.RegisterClassMap<IntwentyRole>();
            }
            if (IntwentyUserRoleMongoDbMap == null)
            {
                //IntwentyUserRoleMongoDbMap = BsonClassMap.RegisterClassMap<IntwentyUserRole>(cm => { cm.AutoMap(); cm.MapIdMember(c => c.Id); });
                BsonClassMap.RegisterClassMap<IdentityUserRole<string>>(cm => { cm.AutoMap(); cm.SetIsRootClass(true); });
                IntwentyUserRoleMongoDbMap = BsonClassMap.RegisterClassMap<IntwentyUserRole>(cm => { cm.MapIdMember(c => c.Id); });
            }
            if (ApplicationItemMongoDbMap == null)
            {
                ApplicationItemMongoDbMap = BsonClassMap.RegisterClassMap<ApplicationItem>(cm => { cm.AutoMap(); cm.MapIdMember(c => c.Id); });
               
            }
            if (DataViewItemMongoDbMap == null)
            {
                DataViewItemMongoDbMap = BsonClassMap.RegisterClassMap<DataViewItem>(cm => { cm.AutoMap(); cm.MapIdMember(c => c.Id); });
            }
            if (DefaultValueMongoDbMap == null)
            {
                DefaultValueMongoDbMap = BsonClassMap.RegisterClassMap<DefaultValue>(cm => { cm.AutoMap(); cm.MapIdMember(c => c.Id); });
            }
            if (EventLogMongoDbMap == null)
            {
                EventLogMongoDbMap = BsonClassMap.RegisterClassMap<EventLog>(cm => { cm.AutoMap(); cm.MapIdMember(c => c.Id); });
            }
            if (InformationStatusMongoDbMap == null)
            {
                InformationStatusMongoDbMap = BsonClassMap.RegisterClassMap<InformationStatus>(cm => { cm.AutoMap(); cm.MapIdMember(c => c.Id); });
            }
            if (MenuItemMongoDbMap == null)
            {
                MenuItemMongoDbMap = BsonClassMap.RegisterClassMap<MenuItem>(cm => { cm.AutoMap(); cm.MapIdMember(c => c.Id); });
            }
            if (SystemIDMongoDbMap == null)
            {
                SystemIDMongoDbMap = BsonClassMap.RegisterClassMap<SystemID>(cm => { cm.AutoMap(); cm.MapIdMember(c => c.Id); });
            }
            if (UserInterfaceItemMongoDbMap == null)
            {
                UserInterfaceItemMongoDbMap = BsonClassMap.RegisterClassMap<UserInterfaceItem>(cm => { cm.AutoMap(); cm.MapIdMember(c => c.Id); });
            }
            if (ValueDomainItemMongoDbMap == null)
            {
                ValueDomainItemMongoDbMap = BsonClassMap.RegisterClassMap<ValueDomainItem>(cm => { cm.AutoMap(); cm.MapIdMember(c => c.Id); });
            }

        }

     

        private void SetLiteDbConnectionString()
        {
            if (!ConnectionString.ToLower().Contains("filename"))
                throw new InvalidOperationException("LiteDb connectionstring must contain filename");

            string filename = "";
            var index = ConnectionString.IndexOf("=") + 1;
            if (index < 5)
                throw new InvalidOperationException("Error in litedb connectionstring");
            var index2 = ConnectionString.IndexOf(";");
            if (index2 > 0)
            {
                filename = ConnectionString.Substring(index, index2 - index);
                filename = Path.Combine(Directory.GetCurrentDirectory(), filename);
                ConnectionString = ConnectionString.Remove(index, index2 - index);
                ConnectionString = ConnectionString.Insert(index, filename);
            }
            else
            {
                filename = ConnectionString.Substring(index);
                filename = Path.Combine(Directory.GetCurrentDirectory(), filename);
                ConnectionString = ConnectionString.Remove(index);
                ConnectionString = ConnectionString.Insert(index, filename);
            }

            if (ConnectionString.Contains("/"))
                ConnectionString = ConnectionString.Replace("/", "\\");
        }

        public void CreateTable<T>(bool checkexisting = false)
        {

            var workingtype = typeof(T);
            var tablename = workingtype.Name;

            //TABLENAME
            var annot_tablename = workingtype.GetCustomAttributes(typeof(DbTableName), false);
            if (annot_tablename != null && annot_tablename.Length > 0)
                tablename = ((DbTableName)annot_tablename[0]).Name;


            if (DbEngine == DBMS.MongoDb)
            {
                
                if (checkexisting)
                {
                    var table = MongoDbClient.GetCollection<T>(tablename);
                    if (table == null)
                    {
                        MongoDbClient.CreateCollection(tablename);
                    }
                }
                else
                {
                    MongoDbClient.CreateCollection(tablename);
                }
            }

            if (DbEngine == DBMS.LiteDb)
            {
                LiteDbClient.GetCollection<T>(tablename);
            }


        }

        public void DropCollection(string collectionname)
        {

            if (DbEngine == DBMS.MongoDb)
            {
                 MongoDbClient.DropCollection(collectionname);
            }

            if (DbEngine == DBMS.LiteDb)
            {
                LiteDbClient.DropCollection(collectionname);
            }
        }

        public int DeleteRange<T>(IEnumerable<T> model)
        {
            var res = 0;
            foreach (var t in model)
            {
                res += Delete<T>(t);
            }

            return res;
        }

        public int Delete<T>(T model)
        {
            var workingtype = typeof(T);
            var tablename = workingtype.Name;

            //TABLENAME
            var annot_tablename = workingtype.GetCustomAttributes(typeof(DbTableName), false);
            if (annot_tablename != null && annot_tablename.Length > 0)
                tablename = ((DbTableName)annot_tablename[0]).Name;

            object rm;
            Cache.Remove(string.Format("ALL_{0}", tablename), out rm);

            string stringkey = "";
            int intkey = -1;
            var memberproperties = workingtype.GetProperties();
            foreach (var m in memberproperties)
            {
                if (m.Name.ToLower() == "id")
                {
                    if (m.PropertyType.ToString().ToUpper() == "SYSTEM.STRING")
                        stringkey = (string)m.GetValue(model);
                    else
                        intkey = (int)m.GetValue(model);

                    break;
                }

            }
            

            if (string.IsNullOrEmpty(stringkey) && intkey == -1)
                return 0;


            if (DbEngine == DBMS.MongoDb)
            {
                FilterDefinition<T> deleteFilter;
                if (intkey > 0)
                    deleteFilter = Builders<T>.Filter.Eq("_id", intkey);
                else
                    deleteFilter = Builders<T>.Filter.Eq("_id", stringkey);

                var result = MongoDbClient.GetCollection<T>(tablename).DeleteOne(deleteFilter);
                return Convert.ToInt32(result.DeletedCount);

            }

            if (DbEngine == DBMS.LiteDb)
            {
                if (intkey > 0)
                    LiteDbClient.GetCollection<T>(tablename).Delete(intkey);
                else
                    LiteDbClient.GetCollection<T>(tablename).Delete(stringkey);

                return 1;

            }

            return 0;
        }

        public T GetOne<T>(int id) where T : new()
        {
            var workingtype = typeof(T);
            var tablename = workingtype.Name;

            //TABLENAME
            var annot_tablename = workingtype.GetCustomAttributes(typeof(DbTableName), false);
            if (annot_tablename != null && annot_tablename.Length > 0)
                tablename = ((DbTableName)annot_tablename[0]).Name;

            if (DbEngine == DBMS.MongoDb)
            {
                var filter = new MongoDB.Bson.BsonDocument();
                filter.Add(new MongoDB.Bson.BsonElement("_id", id));
                var result = MongoDbClient.GetCollection<T>(tablename).Find(filter).FirstOrDefault();
                return result;
            }

            if (DbEngine == DBMS.LiteDb)
            {
                var result = LiteDbClient.GetCollection<T>(tablename).FindById(id);
                return result;
            }

            return default(T);
        }

        public T GetOne<T>(string id) where T : new()
        {
            var workingtype = typeof(T);
            var tablename = workingtype.Name;

            //TABLENAME
            var annot_tablename = workingtype.GetCustomAttributes(typeof(DbTableName), false);
            if (annot_tablename != null && annot_tablename.Length > 0)
                tablename = ((DbTableName)annot_tablename[0]).Name;

            if (DbEngine == DBMS.MongoDb)
            {
                var filter = new MongoDB.Bson.BsonDocument();
                filter.Add(new MongoDB.Bson.BsonElement("_id", id));
                var result = MongoDbClient.GetCollection<T>(tablename).Find(filter).FirstOrDefault();
                return result;
            }

            if (DbEngine == DBMS.LiteDb)
            {
                var result = LiteDbClient.GetCollection<T>(tablename).FindById(id);
                return result;
            }

            return default(T);
        }

        public List<T> GetAll<T>() where T : new()
        {
            var workingtype = typeof(T);
            var tablename = workingtype.Name;

            //TABLENAME
            var annot_tablename = workingtype.GetCustomAttributes(typeof(DbTableName), false);
            if (annot_tablename != null && annot_tablename.Length > 0)
                tablename = ((DbTableName)annot_tablename[0]).Name;

            object objectresult;
            if (Cache.TryGetValue(string.Format("ALL_{0}", tablename), out objectresult))
            {
                return (List<T>)objectresult;
            }

            if (DbEngine == DBMS.MongoDb)
            {
                var result = MongoDbClient.GetCollection<T>(tablename).AsQueryable().ToList();
                if (result.Count < 50000)
                {
                    Cache.TryAdd(string.Format("ALL_{0}", tablename), result);
                }
                return result;

            }

            if (DbEngine == DBMS.LiteDb)
            {
                //var result = LiteDbClient.GetCollection<T>(tablename).Find(p => true).ToList();
                var result = LiteDbClient.GetCollection<T>(tablename).FindAll().ToList();
                if (result.Count < 50000)
                {
                    Cache.TryAdd(string.Format("ALL_{0}", tablename), result);
                }
                return result;
            }

            return null;
        }

        public List<T> GetByExpression<T>(IntwentyExpression expression) where T : new()
        {
            var workingtype = typeof(T);
            var tablename = workingtype.Name;

            //TABLENAME
            var annot_tablename = workingtype.GetCustomAttributes(typeof(DbTableName), false);
            if (annot_tablename != null && annot_tablename.Length > 0)
                tablename = ((DbTableName)annot_tablename[0]).Name;

            if (DbEngine == DBMS.MongoDb)
            {
                var result = MongoDbClient.GetCollection<T>(tablename);
                return result.Find(expression.GetMongoDbFilterDefinition<T>()).ToList();

            }

            if (DbEngine == DBMS.LiteDb)
            {
                var result = LiteDbClient.GetCollection<T>(tablename).Find(expression.GetLiteDbFilterDefinition());
                return result.ToList();
            }

            if (DbEngine == DBMS.LiteDb)
            {
                var result = LiteDbClient.GetCollection<T>(tablename).Find(expression.GetLiteDbFilterDefinition());
                return result.ToList();
            }

            return null;

        }

        public int Insert<T>(T model)
        {
            var workingtype = typeof(T);
            var tablename = workingtype.Name;

            //TABLENAME
            var annot_tablename = workingtype.GetCustomAttributes(typeof(DbTableName), false);
            if (annot_tablename != null && annot_tablename.Length > 0)
                tablename = ((DbTableName)annot_tablename[0]).Name;

            object rm;
            Cache.Remove(string.Format("ALL_{0}", tablename), out rm);

            var newid = 0;
            var memberproperties = workingtype.GetProperties();
            foreach (var m in memberproperties)
            {
                var annot_autoinc = m.GetCustomAttributes(typeof(AutoIncrement), false);
                if (annot_autoinc != null && annot_autoinc.Length > 0)
                {
                    newid = GetAutoIncrementalId(tablename);
                    m.SetValue(model, newid, null);
                    break;
                }
            }

            if (DbEngine == DBMS.MongoDb)
            {
                MongoDbClient.GetCollection<T>(tablename).InsertOne(model);
            }

            if (DbEngine == DBMS.LiteDb)
            {
                LiteDbClient.GetCollection<T>(tablename).Insert(model);
            }


            return newid;
        }

        public int Update<T>(T model)
        {
            var workingtype = typeof(T);
            var tablename = workingtype.Name;

            //TABLENAME
            var annot_tablename = workingtype.GetCustomAttributes(typeof(DbTableName), false);
            if (annot_tablename != null && annot_tablename.Length > 0)
                tablename = ((DbTableName)annot_tablename[0]).Name;

            object rm;
            Cache.Remove(string.Format("ALL_{0}", tablename), out rm);

            string stringkey = "";
            int intkey = -1;
            var memberproperties = workingtype.GetProperties();
            foreach (var m in memberproperties)
            {
                if (m.Name.ToLower() == "id")
                {
                    if (m.PropertyType.ToString().ToUpper() == "SYSTEM.STRING")
                        stringkey = (string)m.GetValue(model);
                    else
                        intkey = (int)m.GetValue(model);

                    break;
                }

            }
            

            if (string.IsNullOrEmpty(stringkey) && intkey == -1)
                return 0;



           if (DbEngine == DBMS.MongoDb)
            {

                FilterDefinition<T> updateFilter;
                if (intkey > 0)
                    updateFilter = Builders<T>.Filter.Eq("_id", intkey);
                else
                    updateFilter = Builders<T>.Filter.Eq("_id", stringkey);

                
                var result = MongoDbClient.GetCollection<T>(tablename).ReplaceOne(updateFilter, model, new ReplaceOptions { IsUpsert = true });
                return Convert.ToInt32(result.ModifiedCount);

            }

            if (DbEngine == DBMS.LiteDb)
            {

                if (intkey > 0)
                    LiteDbClient.GetCollection<T>(tablename).Update(intkey, model);
                else
                    LiteDbClient.GetCollection<T>(tablename).Update(stringkey, model);

                return 1;
            }

            return 0;
        }

        public int GetAutoIncrementalId(int applicationid = 0, int parentid=0, string metatype = "INTERNAL", string metacode = "UNSPECIFIED", string properties = "")
        {
            var model = new SystemID() { ApplicationId = applicationid, GeneratedDate = DateTime.Now, MetaCode=metacode, MetaType=metatype, Properties = properties, ParentId=parentid };

            if (DbEngine == DBMS.MongoDb)
            {

                var max = Convert.ToInt32(MongoDbClient.GetCollection<SystemID>("sysdata_SystemId").CountDocuments(p=> p.Id > 0));
                if (max == 0)
                {
                    model.Id = 1;
                    MongoDbClient.GetCollection<SystemID>("sysdata_SystemId").InsertOne(model);
                    return 1;
                }

                max = MongoDbClient.GetCollection<SystemID>("sysdata_SystemId").AsQueryable().Max(p=> p.Id);
                model.Id = max + 1;
                MongoDbClient.GetCollection<SystemID>("sysdata_SystemId").InsertOne(model);
                return model.Id;

            }

            if (DbEngine == DBMS.LiteDb)
            {

                var max = LiteDbClient.GetCollection<SystemID>("sysdata_SystemId").Count();
                if (max < 1)
                {
                    model.Id = 1;
                    LiteDbClient.GetCollection<SystemID>("sysdata_SystemId").Insert(model);
                    return 1;
                }

                max = LiteDbClient.GetCollection<SystemID>("sysdata_SystemId").Max(p=> p.Id);
                model.Id = max + 1;
                LiteDbClient.GetCollection<SystemID>("sysdata_SystemId").Insert(model);
                return model.Id;
                

            }

            return -1;
        }

        private int GetAutoIncrementalId(string tablename)
        {
            return GetAutoIncrementalId(metacode:tablename);
        }

        public bool DeleteIntwentyJsonObject(string collectionname, int id)
        {
            return DeleteJsonDocument(collectionname, string.Format("ID_{0}", id));
        }

        public bool DeleteIntwentyJsonObject(string collectionname, int id, int version)
        {
            if (version < 1) version = 1;
            return DeleteJsonDocument(collectionname, string.Format("ID_{0}_VER_{1}", id, version));
        }

 
        public bool DeleteJsonDocument(string collectionname, int id)
        {
            if (DbEngine == DBMS.MongoDb)
            {
                var filter = new MongoDB.Bson.BsonDocument();
                filter.Add(new MongoDB.Bson.BsonElement("_id", id));
                var result = MongoDbClient.GetCollection<MongoDB.Bson.BsonDocument>(collectionname).DeleteOne(filter);
                return result.DeletedCount == 1;
            }

            if (DbEngine == DBMS.LiteDb)
            {
                return LiteDbClient.GetCollection<LiteDB.BsonDocument>(collectionname).Delete(id);
            }

            return false;
        }

        public bool DeleteJsonDocument(string collectionname, string id)
        {

            if (DbEngine == DBMS.MongoDb)
            {
                var filter = new MongoDB.Bson.BsonDocument();
                filter.Add(new MongoDB.Bson.BsonElement("_id", id));
                var result = MongoDbClient.GetCollection<MongoDB.Bson.BsonDocument>(collectionname).DeleteOne(filter);
                return result.DeletedCount == 1;
            }

            if (DbEngine == DBMS.LiteDb)
            {
                return LiteDbClient.GetCollection<LiteDB.BsonDocument>(collectionname).Delete(id);
            }

            return false;
        }

        public int InsertIntwentyJsonObject(string json, string collectionname, int id)
        {
            return InsertJsonDocument(json,collectionname, string.Format("ID_{0}", id));
        }

        public int InsertIntwentyJsonObject(string json, string collectionname, int id, int version)
        {
            if (version < 1) version = 1;
            return InsertJsonDocument(json,collectionname, string.Format("ID_{0}_VER_{1}", id, version));
        }

   
        public int InsertJsonDocument(string json, string collectionname, int id)
        {
            if (DbEngine == DBMS.MongoDb)
            {
                var collection = MongoDbClient.GetCollection<MongoDB.Bson.BsonDocument>(collectionname);
                if (collection == null)
                {
                    MongoDbClient.CreateCollection(collectionname);
                    collection = MongoDbClient.GetCollection<MongoDB.Bson.BsonDocument>(collectionname);
                }
                var doc = MongoDB.Bson.BsonDocument.Parse(json);
                doc.Add(new MongoDB.Bson.BsonElement("_id", id));
                collection.InsertOne(doc);
                return 1;
            }

            if (DbEngine == DBMS.LiteDb)
            {
                var collection = LiteDbClient.GetCollection<LiteDB.BsonDocument>(collectionname);
                var val = LiteDB.JsonSerializer.Deserialize(json);
                var doc = val.AsDocument;
                doc.Add("_id", id);
                collection.Insert(doc);
                return 1;
            }

            return 0;
        }

        public int InsertJsonDocument(string json, string collectionname, string id)
        {
            if (DbEngine == DBMS.MongoDb)
            {
                var collection = MongoDbClient.GetCollection<MongoDB.Bson.BsonDocument>(collectionname);
                if (collection == null)
                {
                    MongoDbClient.CreateCollection(collectionname);
                    collection = MongoDbClient.GetCollection<MongoDB.Bson.BsonDocument>(collectionname);
                }
                var doc = MongoDB.Bson.BsonDocument.Parse(json);
                doc.Add(new MongoDB.Bson.BsonElement("_id", id));
                collection.InsertOne(doc);
                return 1;
            }

            if (DbEngine == DBMS.LiteDb)
            {
                var collection = LiteDbClient.GetCollection<LiteDB.BsonDocument>(collectionname);
                var val = LiteDB.JsonSerializer.Deserialize(json);
                var doc = val.AsDocument;
                doc.Add("_id", id);
                collection.Insert(doc);
                return 1;
            }

            return 0;
        }

        public int UpdateIntwentyJsonObject(string json, string collectionname, int id)
        {
            return UpdateJsonDocument(json, collectionname, string.Format("ID_{0}", id));
        }

        public int UpdateIntwentyJsonObject(string json, string collectionname, int id, int version)
        {
            if (version < 1) version = 1;
            return UpdateJsonDocument(json, collectionname, string.Format("ID_{0}_VER_{1}", id, version));
        }


        public int UpdateJsonDocument(string json, string collectionname, int id)
        {
            if (DbEngine == DBMS.MongoDb)
            {
                var jsonfilter = string.Format("\"{0}\":\"{1}\"", "_id", id);
                var doc = MongoDB.Bson.BsonDocument.Parse(json);
                var result = MongoDbClient.GetCollection<MongoDB.Bson.BsonDocument>(collectionname).ReplaceOne("{" + jsonfilter + "}", doc, new ReplaceOptions { IsUpsert = true });
                return Convert.ToInt32(result.ModifiedCount);

            }

            if (DbEngine == DBMS.LiteDb)
            {
                var collection = LiteDbClient.GetCollection<LiteDB.BsonDocument>(collectionname);
                var val = LiteDB.JsonSerializer.Deserialize(json);
                var doc = val.AsDocument;
                collection.Update(id, doc);
                return 1;
            }

            return 0;
        }

        public int UpdateJsonDocument(string json, string collectionname, string id)
        {
            if (DbEngine == DBMS.MongoDb)
            {
                var jsonfilter = string.Format("\"{0}\":\"{1}\"", "_id", id);
                var doc = MongoDB.Bson.BsonDocument.Parse(json);
                var result = MongoDbClient.GetCollection<MongoDB.Bson.BsonDocument>(collectionname).ReplaceOne("{" + jsonfilter + "}", doc, new ReplaceOptions { IsUpsert = true });
                return Convert.ToInt32(result.ModifiedCount);

            }

            if (DbEngine == DBMS.LiteDb)
            {
                var collection = LiteDbClient.GetCollection<LiteDB.BsonDocument>(collectionname);
                var val = LiteDB.JsonSerializer.Deserialize(json);
                var doc = val.AsDocument;
                collection.Update(id, doc);
                return 1;
            }

            return 0;
        }



        public int GetJsonDocumentCount(string collectionname)
        {
            if (DbEngine == DBMS.MongoDb)
            {
                return Convert.ToInt32(MongoDbClient.GetCollection<MongoDB.Bson.BsonDocument>(collectionname).CountDocuments(new MongoDB.Bson.BsonDocument()));
            }

            if (DbEngine == DBMS.LiteDb)
            {
                return LiteDbClient.GetCollection<LiteDB.BsonDocument>(collectionname).Count();
            }


            return 0;
        }

    
        public ApplicationTable GetDataSet(string collectionname, IntwentyExpression expression = null)
        {
            StringBuilder s;
            if (expression == null)
            {
                s = GetJsonArray(collectionname);
            }
            else
            {
                s = GetJsonArray(collectionname, expression);
            }

            var result = ClientStateInfo.CreateFromJSON(System.Text.Json.JsonDocument.Parse(s.ToString()).RootElement);
            if (result.SubTables.Count > 0)
                return result.SubTables[0];
            else
                return null;
        }


        public StringBuilder GetJsonArray(string collectionname, IntwentyExpression expression, List<IIntwentyDataColum> returnfields = null, int minrow = 0, int maxrow = 0)
        {
            var jsonresult = new StringBuilder("[");

            if (DbEngine == DBMS.MongoDb)
            {
                var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
                var projection = DBHelpers.GetMongoDbProjection(returnfields);
                List<MongoDB.Bson.BsonDocument> result=null;
                if (string.IsNullOrEmpty(projection))
                    result = MongoDbClient.GetCollection<MongoDB.Bson.BsonDocument>(collectionname).Find(p=> true).ToList();
                if (!string.IsNullOrEmpty(projection))
                    result = MongoDbClient.GetCollection<MongoDB.Bson.BsonDocument>(collectionname).Find(p => true).Project(projection).ToList();

             
                var rindex = 0;
                foreach (var doc in result)
                {
                    if (!expression.ComputeExpression(doc))
                        continue;

                    rindex += 1;
                    if (maxrow > minrow && (minrow > 0 || maxrow > 0))
                    {
                        if (!(minrow <= rindex && maxrow > rindex))
                            continue;
                    }

                    if (doc.Contains("_id"))
                        doc.Remove("_id");


                    if (rindex == 1)
                        jsonresult.Append(doc.ToJson(jsonWriterSettings));
                    else
                        jsonresult.Append("," + doc.ToJson(jsonWriterSettings));

                }
              

            }


            if (DbEngine == DBMS.LiteDb)
            {
                var result = LiteDbClient.GetCollection<LiteDB.BsonDocument>(collectionname).Find(p => true).ToList();
                var rindex = 0;
                foreach (var doc in result)
                {
                    if (!expression.ComputeExpression(doc))
                        continue;

                    rindex += 1;
                    if (maxrow > minrow && (minrow > 0 || maxrow > 0))
                    {
                        if (!(minrow <= rindex && maxrow > rindex))
                            continue;
                    }

                    if (doc.ContainsKey("_id"))
                        doc.Remove("_id");

                    if (returnfields != null && returnfields.Count > 0)
                    {
                        foreach (var e in doc)
                        {
                            if (!returnfields.Exists(p=> p.ColumnName == e.Key))
                                doc.Remove(e.Key);
                        }
                    }


                    if (rindex == 1)
                        jsonresult.Append(doc.ToString());
                    else
                        jsonresult.Append("," + doc.ToString());

                }

            }

            jsonresult.Append("]");
            return jsonresult;
        }

        public StringBuilder GetJsonArray(string collectionname, List<IIntwentyDataColum> returnfields = null, int minrow = 0, int maxrow = 0)
        {
            var jsonresult = new StringBuilder("[");

            if (DbEngine == DBMS.MongoDb)
            {
                var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
                var projection = DBHelpers.GetMongoDbProjection(returnfields);
                List<MongoDB.Bson.BsonDocument> result = null;
                if (string.IsNullOrEmpty(projection))
                    result = MongoDbClient.GetCollection<MongoDB.Bson.BsonDocument>(collectionname).Find(p => true).ToList();
                if (!string.IsNullOrEmpty(projection))
                    result = MongoDbClient.GetCollection<MongoDB.Bson.BsonDocument>(collectionname).Find(p => true).Project(projection).ToList();

                var rindex = 0;
                foreach (var doc in result)
                {

                    rindex += 1;
                    if (maxrow > minrow && (minrow > 0 || maxrow > 0))
                    {
                        if (!(minrow <= rindex && maxrow > rindex))
                            continue;
                    }

                    if (doc.Contains("_id"))
                        doc.Remove("_id");

                    if (rindex==1)
                        jsonresult.Append(doc.ToJson(jsonWriterSettings));
                    else
                        jsonresult.Append("," + doc.ToJson(jsonWriterSettings));
                }
              

            }

            if (DbEngine == DBMS.LiteDb)
            {
                var result = LiteDbClient.GetCollection<LiteDB.BsonDocument>(collectionname).Find(f => true).ToList();
                var rindex = 0;
                foreach (var doc in result)
                {
                  
                    rindex += 1;
                    if (maxrow > minrow && (minrow > 0 || maxrow > 0))
                    {
                        if (!(minrow <= rindex && maxrow > rindex))
                            continue;
                    }

                    if (doc.ContainsKey("_id"))
                        doc.Remove("_id");

                    if (returnfields != null && returnfields.Count > 0)
                    {
                        foreach (var e in doc)
                        {
                            if (!returnfields.Exists(p => p.ColumnName == e.Key))
                                doc.Remove(e.Key);
                        }
                    }

                    if (rindex == 1)
                        jsonresult.Append(doc.ToString());
                    else
                        jsonresult.Append("," + doc.ToString());
                }

            }

            jsonresult.Append("]");
            return jsonresult;
        }

        public StringBuilder GetIntwentyJsonObject(string collectionname, int id, List<IIntwentyDataColum> returnfields = null)
        {
            return GetIntwentyJsonObject(collectionname, string.Format("ID_{0}", id), returnfields);
        }

        public StringBuilder GetIntwentyJsonObject(string collectionname, int id, int version, List<IIntwentyDataColum> returnfields = null)
        {
            if (version < 1) version = 1;
            return GetIntwentyJsonObject(collectionname, string.Format("ID_{0}_VER_{1}", id, version), returnfields);
        }

        public StringBuilder GetIntwentyJsonObject(string collectionname, string id, List<IIntwentyDataColum> returnfields = null)
        {
          

            if (DbEngine == DBMS.MongoDb)
            {
                var filter = new MongoDB.Bson.BsonDocument();
                filter.Add(new MongoDB.Bson.BsonElement("_id", id));
                if (returnfields != null)
                {
                    var projection = DBHelpers.GetMongoDbProjection(returnfields);
                    var doc = MongoDbClient.GetCollection<MongoDB.Bson.BsonDocument>(collectionname).Find(filter).Project(projection).FirstOrDefault();
                    if (doc == null)
                        return new StringBuilder("{}");

                    if (doc.Contains("_id"))
                        doc.Remove("_id");

                    return new StringBuilder(doc.ToString());
                }
                else
                {

                    var doc = MongoDbClient.GetCollection<MongoDB.Bson.BsonDocument>(collectionname).Find(filter).FirstOrDefault();
                    if (doc == null)
                        return new StringBuilder("{}");

                    if (doc.Contains("_id"))
                        doc.Remove("_id");

                    return new StringBuilder(doc.ToString());
                }

            }

            if (DbEngine == DBMS.LiteDb)
            {
                var doc = LiteDbClient.GetCollection<LiteDB.BsonDocument>(collectionname).FindById(id);
                if (doc == null)
                    return new StringBuilder("{}");

                if (doc.ContainsKey("_id"))
                    doc.Remove("_id");

                if (returnfields != null && returnfields.Count > 0)
                {
                    foreach (var e in doc)
                    {
                        if (!returnfields.Exists(p => p.ColumnName == e.Key))
                            doc.Remove(e.Key);
                    }
                }


                return new StringBuilder(doc.ToString());
            }

            return new StringBuilder("{}");
        }

        public StringBuilder GetJsonObject(string collectionname, string id)
        {

            if (DbEngine == DBMS.MongoDb)
            {
                var filter = new MongoDB.Bson.BsonDocument();
                filter.Add(new MongoDB.Bson.BsonElement("_id", id));
                var doc = MongoDbClient.GetCollection<MongoDB.Bson.BsonDocument>(collectionname).Find(filter).FirstOrDefault();
                if (doc == null)
                    return new StringBuilder("{}");

                return new StringBuilder(doc.ToJson());
            }

            if (DbEngine == DBMS.LiteDb)
            {
                var doc = LiteDbClient.GetCollection<LiteDB.BsonDocument>(collectionname).FindById(id);
                if (doc == null)
                    return new StringBuilder("{}");

                return new StringBuilder(doc.ToString());
            }

            return new StringBuilder("{}");
        }

        public StringBuilder GetJsonObject(string collectionname, int id)
        {

            if (DbEngine == DBMS.MongoDb)
            {
                var filter = new MongoDB.Bson.BsonDocument();
                filter.Add(new MongoDB.Bson.BsonElement("_id", id));
                var doc = MongoDbClient.GetCollection<MongoDB.Bson.BsonDocument>(collectionname).Find(filter).FirstOrDefault();
                if (doc == null)
                    return new StringBuilder("{}");

                return new StringBuilder(doc.ToJson());
            }

            if (DbEngine == DBMS.LiteDb)
            {
                var doc = LiteDbClient.GetCollection<LiteDB.BsonDocument>(collectionname).FindById(id);
                if (doc == null)
                    return new StringBuilder("{}");

                return new StringBuilder(doc.ToString());
            }

            return new StringBuilder("{}");
        }

        public StringBuilder GetJsonObject(string collectionname, IntwentyExpression expression, List<IIntwentyDataColum> returnfields = null)
        {
            if (expression==null)
                throw new InvalidOperationException("Filter expression was empty.");

            if (DbEngine == DBMS.MongoDb)
            {
                var projection = DBHelpers.GetMongoDbProjection(returnfields);
                List<MongoDB.Bson.BsonDocument> result = null;
                if (string.IsNullOrEmpty(projection))
                    result = MongoDbClient.GetCollection<MongoDB.Bson.BsonDocument>(collectionname).Find(p => true).ToList();
                if (!string.IsNullOrEmpty(projection))
                    result = MongoDbClient.GetCollection<MongoDB.Bson.BsonDocument>(collectionname).Find(p => true).Project(projection).ToList();

                foreach (var doc in result)
                {
                    if (!expression.ComputeExpression(doc))
                        continue;

                    if (doc.Contains("_id"))
                        doc.Remove("_id");

                    return new StringBuilder(doc.ToJson());
                }

            }

            if (DbEngine == DBMS.LiteDb)
            {

                var result = LiteDbClient.GetCollection<LiteDB.BsonDocument>(collectionname).Find(p => true).ToList();
                foreach (var doc in result)
                {
                    if (!expression.ComputeExpression(doc))
                        continue;

                    if (doc.ContainsKey("_id"))
                        doc.Remove("_id");


                    if (returnfields != null && returnfields.Count > 0)
                    {
                        foreach (var e in doc)
                        {
                            if (!returnfields.Exists(p => p.ColumnName == e.Key))
                                doc.Remove(e.Key);
                        }
                    }

                    return new StringBuilder(doc.ToString());

                }

            }

            return new StringBuilder("{}");

        }



    }
}
