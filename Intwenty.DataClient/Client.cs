using System;

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
                InternalClient = new Databases.SqlLite(connectionstring);
            if (DBMSType == SqlDBMS.MySql)
                InternalClient = new Databases.MariaDb(connectionstring);
            if (DBMSType == SqlDBMS.MariaDB)
                InternalClient = new Databases.MariaDb(connectionstring);
        }

        public void Open()
        {
            InternalClient.Open();
        }

        public void Close()
        {
            InternalClient.Close();
        }

        public T GetOne<T>(int id) where T : new() 
        {
            return InternalClient.GetOne<T>(id);
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
                InternalClient = new Databases.LiteDb(connectionstring);
        }

        public T GetOne<T>(int id) where T : new()
        {
            return InternalClient.GetOne<T>(id);
        }

    }

}
