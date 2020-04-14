using Intwenty.Data.DBAccess.Annotations;
using Intwenty.Data.DBAccess.Helpers;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intwenty.Data.Entity;
using MongoDB.Bson;

namespace Intwenty.Data.DBAccess
{
    public class IntwentyNoSqlDbClient : IIntwentyDbNoSql
    {


        private DBMS DBMSType { get; set; }

        private string ConnectionString { get; set; }

        private string DatabaseName { get; set; }


        public IntwentyNoSqlDbClient()
        {
            ConnectionString = string.Empty;
            DBMSType = DBMS.MSSqlServer;
        }

        public IntwentyNoSqlDbClient(DBMS d, string connectionstring, string databasename)
        {
            DBMSType = d;
            ConnectionString = connectionstring;
            DatabaseName = databasename;
            if (DBMSType != DBMS.MongoDb)
                throw new InvalidOperationException("IntwentyNoSqlDbClient configured with wrong DBMS setting");
        }

        public DBMS DbEngine
        {
            get { return DBMSType; }
        }

        public bool IsNoSql
        {
            get { return (DBMSType == DBMS.MongoDb); }
        }


        public void CreateTable<T>(bool checkexisting = false)
        {
           
            var workingtype = typeof(T);
            var tablename = workingtype.Name;

            //TABLENAME
            var annot_tablename = workingtype.GetCustomAttributes(typeof(DbTableName), false);
            if (annot_tablename != null && annot_tablename.Length > 0)
                tablename = ((DbTableName)annot_tablename[0]).Name;

         
            if (DBMSType == DBMS.MongoDb)
            {
                var client = new MongoDB.Driver.MongoClient(ConnectionString);
                var database = client.GetDatabase(DatabaseName);
                if (checkexisting)
                {
                    var table = database.GetCollection<T>(tablename);
                    if (table == null)
                    {
                        database.CreateCollection(tablename);
                    }
                }
                else
                {
                    database.CreateCollection(tablename);
                }
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

            BsonIdAttribute pk = null;
            int colvalue = 0;
            var memberproperties = workingtype.GetProperties();
            foreach (var m in memberproperties)
            {
                var annot_pk = m.GetCustomAttributes(typeof(BsonIdAttribute), false);
                if (annot_pk != null && annot_pk.Length > 0)
                {
                    pk = ((BsonIdAttribute)annot_pk[0]);
                    colvalue = (int)m.GetValue(model);
                    break;
                }

            }

            if (DBMSType == DBMS.MongoDb)
            {
                if (pk == null)
                    return 0;

                var client = new MongoDB.Driver.MongoClient(ConnectionString);
                var database = client.GetDatabase(DatabaseName);
                var jsonfilter = string.Format("\"{0}\":{1}", "_id", colvalue);
                var result = database.GetCollection<T>(tablename).DeleteOne("{"+ jsonfilter + "}");
                return Convert.ToInt32(result.DeletedCount);

            }

            return 0;
        }

      

        public List<T> Get<T>() where T : new()
        {
            var workingtype = typeof(T);
            var tablename = workingtype.Name;

            //TABLENAME
            var annot_tablename = workingtype.GetCustomAttributes(typeof(DbTableName), false);
            if (annot_tablename != null && annot_tablename.Length > 0)
                tablename = ((DbTableName)annot_tablename[0]).Name;

            if (DBMSType == DBMS.MongoDb)
            {
                var client = new MongoDB.Driver.MongoClient(ConnectionString);
                var database = client.GetDatabase(DatabaseName);
                var result = database.GetCollection<T>(tablename).Find(f => true).ToList();
                return result;

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

            var newid = 0;
            BsonIdAttribute pk = null;
            var memberproperties = workingtype.GetProperties();
            foreach (var m in memberproperties)
            {
                var annot_pk = m.GetCustomAttributes(typeof(BsonIdAttribute), false);
                if (annot_pk != null && annot_pk.Length > 0)
                {
                    pk = ((BsonIdAttribute)annot_pk[0]);
                    var annot_autoinc = m.GetCustomAttributes(typeof(AutoIncrement), false);
                    if (annot_autoinc != null && annot_autoinc.Length > 0)
                    {
                        newid = GetNewSystemId(tablename);
                        m.SetValue(model, newid, null);

                    }
                    break;
                }
            }

            if (DBMSType == DBMS.MongoDb)
            {
                if (pk == null)
                    return 0;

                var client = new MongoClient(ConnectionString);
                var database = client.GetDatabase(DatabaseName);
                database.GetCollection<T>(tablename).InsertOne(model);

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

            BsonIdAttribute pk = null;
            int colvalue = 0;
            var memberproperties = workingtype.GetProperties();
            foreach (var m in memberproperties)
            {
                var annot_pk = m.GetCustomAttributes(typeof(BsonIdAttribute), false);
                if (annot_pk != null && annot_pk.Length > 0)
                {
                    pk = ((BsonIdAttribute)annot_pk[0]);
                    colvalue = (int)m.GetValue(model);
                    break;
                }

            }

            if (DBMSType == DBMS.MongoDb)
            {
                if (pk == null)
                    return 0;

                var client = new MongoDB.Driver.MongoClient(ConnectionString);
                var database = client.GetDatabase(DatabaseName);
                var jsonfilter = string.Format("\"{0}\":{1}", "_id", colvalue);
                var result = database.GetCollection<T>(tablename).ReplaceOne("{" + jsonfilter + "}", model, new ReplaceOptions { IsUpsert = true });
                return Convert.ToInt32(result.ModifiedCount);

            }

            return 0;
        }

        public int GetNewSystemId(SystemID model)
        {
            if (DBMSType == DBMS.MongoDb)
            {
                var client = new MongoDB.Driver.MongoClient(ConnectionString);
                var database = client.GetDatabase(DatabaseName);
                var result = database.GetCollection<SystemID>("sysdata_SystemId").Find(f => true).Sort("{ _id: -1}").Limit(1).FirstOrDefault();
                if (result == null)
                {
                    model.Id = 1;
                    database.GetCollection<SystemID>("sysdata_SystemId").InsertOne(model);
                    return model.Id;
                }
                else
                {
                    model.Id = result.Id;
                    model.Id += 1;
                    database.GetCollection<SystemID>("sysdata_SystemId").InsertOne(model);
                    return model.Id;
                }

            }

            return -1;
        }

        private int GetNewSystemId(string tablename)
        {
            return GetNewSystemId(new SystemID() { ApplicationId=0, GeneratedDate=DateTime.Now, MetaType="INTERNAL", MetaCode= tablename, Properties = string.Empty });
        }

        public int InsertJsonDocument(string json, string collectionname, int id, int version)
        {

            if (DBMSType == DBMS.MongoDb)
            {
                var client = new MongoDB.Driver.MongoClient(ConnectionString);
                var database = client.GetDatabase(DatabaseName);
                var collection = database.GetCollection<BsonDocument>(collectionname);
                if (collection == null)
                {
                    database.CreateCollection(collectionname);
                    collection = database.GetCollection<BsonDocument>(collectionname);
                }
                var t = BsonDocument.Parse(json);
                t.Add(new BsonElement("_id", id));
                collection.InsertOne(t);
            }

            return 0;
        }

        public int UpdateJsonDocument(string json, string collectionname, int id, int version)
        {
            if (DBMSType == DBMS.MongoDb)
            {
                var client = new MongoDB.Driver.MongoClient(ConnectionString);
                var database = client.GetDatabase(DatabaseName);
                var jsonfilter = string.Format("\"{0}\":{1}", "_id", id);
                var t = BsonDocument.Parse(json);
                var result = database.GetCollection<BsonDocument>(collectionname).ReplaceOne("{" + jsonfilter + "}", t, new ReplaceOptions { IsUpsert = true });
                return Convert.ToInt32(result.ModifiedCount);

            }

            return 0;
        }

      

        public int GetCollectionCount(string collectionname)
        {
            if (DBMSType == DBMS.MongoDb)
            {
                var client = new MongoDB.Driver.MongoClient(ConnectionString);
                var database = client.GetDatabase(DatabaseName);
                return Convert.ToInt32(database.GetCollection<BsonDocument>(collectionname).CountDocuments(new BsonDocument()));

            }

            return 0;
        }

        public StringBuilder GetAsJSONArray(string collectionname, int minrow = 0, int maxrow = 0)
        {
            var jsonresult = new StringBuilder("[");

            if (DBMSType == DBMS.MongoDb)
            {
                var client = new MongoDB.Driver.MongoClient(ConnectionString);
                var database = client.GetDatabase(DatabaseName);
                var result = database.GetCollection<BsonDocument>(collectionname).Find(f => true).ToList();
                var rindex = 0;
                foreach (var doc in result)
                {
                    rindex += 1;
                    if (maxrow > minrow && (minrow > 0 || maxrow > 0))
                    {
                        if (!(minrow <= rindex && maxrow > rindex))
                            continue;
                    }
                    doc.Add(new BsonElement("Id", doc.GetValue("_id")));
                    jsonresult.Append(doc.ToJson());

                }
                jsonresult.Append("]");

            }

            return jsonresult;
        }

        public T Get<T>(int id, int version) where T : new()
        {
            var workingtype = typeof(T);
            var tablename = workingtype.Name;

            //TABLENAME
            var annot_tablename = workingtype.GetCustomAttributes(typeof(DbTableName), false);
            if (annot_tablename != null && annot_tablename.Length > 0)
                tablename = ((DbTableName)annot_tablename[0]).Name;

            if (DBMSType == DBMS.MongoDb)
            {
                var filter = new BsonDocument();
                filter.Add(new BsonElement("_id", id));
                var client = new MongoClient(ConnectionString);
                var database = client.GetDatabase(DatabaseName);
                var result = database.GetCollection<T>(tablename).Find(filter).FirstOrDefault();
                return result;
            }

            return default(T);
        }

        public StringBuilder GetAsJSONObject(string collectionname, int id, int version)
        {

            if (DBMSType == DBMS.MongoDb)
            {
                var filter = new BsonDocument();
                filter.Add(new BsonElement("_id", id));
                var client = new MongoDB.Driver.MongoClient(ConnectionString);
                var database = client.GetDatabase(DatabaseName);
                var doc = database.GetCollection<BsonDocument>(collectionname).Find(filter).FirstOrDefault();
                doc.Add(new BsonElement("Id", doc.GetValue("_id")));
                return new StringBuilder(doc.ToJson());
            }

            return new StringBuilder("{}");
        }
    }
}
