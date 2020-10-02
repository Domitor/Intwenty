using System;
using System.Collections.Generic;

namespace Intwenty.DataClient
{
    public enum SqlDBMS { MSSqlServer, MySql, MariaDB, PostgreSQL, SQLite };

    public enum NoSqlDBMS { MongoDb, LiteDb };

    public class SqlClient
    {

        public SqlDBMS DBMSType { get; }

        public string ConnectionString { get; }

        private ISqlClient InternalClient { get; }

        public SqlClient(SqlDBMS database, string connectionstring)
        {
            DBMSType = database;
            ConnectionString = connectionstring;

            if (DBMSType == SqlDBMS.SQLite)
                InternalClient = new Databases.Sql.SqlLite(connectionstring);
            if (DBMSType == SqlDBMS.MySql)
                InternalClient = new Databases.Sql.MariaDb(connectionstring);
            if (DBMSType == SqlDBMS.MariaDB)
                InternalClient = new Databases.Sql.MariaDb(connectionstring);
        }

        public void BeginTransaction() 
        {
            InternalClient.BeginTransaction();
        }
        public void CommitTransaction()
        {
            InternalClient.CommitTransaction();
        }
        public void RollbackTransaction()
        {
            InternalClient.RollbackTransaction();
        }

        public void Open()
        {
            InternalClient.Open();
        }

        public void Close()
        {
            InternalClient.Close();
        }

        public void CreateTable<T>()
        {
            InternalClient.CreateTable<T>();
        }

        public bool TableExists<T>()
        {
            return InternalClient.TableExists<T>();
        }

        public T GetEntity<T>(string id) where T : new()
        {
            return InternalClient.GetEntity<T>(id);
        }

        public T GetEntity<T>(int id) where T : new() 
        {
            return InternalClient.GetEntity<T>(id);
        }

        public List<T> GetEntities<T>() where T : new()
        {
            return InternalClient.GetEntities<T>();
        }

        public List<T> GetEntities<T>(string sqlcommand, bool isStoredProcedure) where T : new()
        {
            return InternalClient.GetEntities<T>(sqlcommand, isStoredProcedure);
        }

        public int InsertEntity<T>(T model)
        {
            return InternalClient.InsertEntity(model);
        }

    }

    public class NoSqlClient
    {

        public NoSqlDBMS DBMSType { get; }

        public string ConnectionString { get; }

        private INoSqlClient InternalClient { get; }

        public NoSqlClient(NoSqlDBMS database, string connectionstring)
        {
            DBMSType = database;
            ConnectionString = connectionstring;

            if (DBMSType == NoSqlDBMS.LiteDb)
                InternalClient = new Databases.NoSql.LiteDb(connectionstring);
        }

        public T GetOne<T>(int id) where T : new()
        {
            return InternalClient.GetOne<T>(id);
        }

    }

}
