using Intwenty.Data.DBAccess.Annotations;
using Intwenty.Data.DBAccess.Helpers;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intwenty.Data.Entity;

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
        }

        public DBMS GetDBMS()
        {
            return DBMSType;
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
                        newid = GetNewSystemId();
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

        public int GetNewSystemId()
        {
            if (DBMSType == DBMS.MongoDb)
            {
                var client = new MongoDB.Driver.MongoClient(ConnectionString);
                var database = client.GetDatabase(DatabaseName);
                var result = database.GetCollection<SystemID>("sysdata_SystemId").Find(f => true).Sort("{ _id: -1}").Limit(1).FirstOrDefault();
                if (result == null)
                {
                    database.GetCollection<SystemID>("sysdata_SystemId").InsertOne(new SystemID() { Id = 1, ApplicationId = 0, GeneratedDate = DateTime.Now, MetaType = "UNKNOWN", MetaCode = "UNKNOWN", Properties = string.Empty });
                    return 1;
                }
                else
                {
                    var id = result.Id;
                    id += 1;
                    database.GetCollection<SystemID>("sysdata_SystemId").InsertOne(new SystemID() { Id=id, ApplicationId=0, GeneratedDate=DateTime.Now, MetaType = "UNKNOWN", MetaCode= "UNKNOWN", Properties=string.Empty });
                    return id;
                }

            }

            return -1;
        }
    }
}
