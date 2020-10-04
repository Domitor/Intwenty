using System.Collections.Generic;
using System.Data;

namespace Intwenty.DataClient
{
    public enum SqlDBMS { MSSqlServer, MySql, MariaDB, PostgreSQL, SQLite };

    public class SqlClient : ISqlClient
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


        public void RunCommand(string sql, bool isprocedure = false, IIntwentySqlParameter[] parameters = null)
        {
            InternalClient.RunCommand(sql, isprocedure, parameters);
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

        public string GetJSONObject(string sql, bool isprocedure = false, IIntwentySqlParameter[] parameters = null, IIntwentyResultColumn[] resultcolumns = null)
        {
            return InternalClient.GetJSONObject(sql,isprocedure,parameters,resultcolumns);
        }

        public bool TableExists(string tablename)
        {
            return InternalClient.TableExists(tablename);
        }

        public bool ColumnExists(string tablename, string columnname)
        {
            return InternalClient.ColumnExists(tablename, columnname);
        }

       

        public List<T> GetEntities<T>(string sql, bool isprocedure = false, IIntwentySqlParameter[] parameters = null) where T : new()
        {
            return InternalClient.GetEntities<T>(sql,isprocedure,parameters);
        }

        public int UpdateEntity<T>(T entity)
        {
            return InternalClient.UpdateEntity<T>(entity);
        }

        public int DeleteEntity<T>(T entity)
        {
            return InternalClient.DeleteEntity<T>(entity);
        }

        public int DeleteEntities<T>(IEnumerable<T> entities)
        {
            return InternalClient.DeleteEntities<T>(entities);
        }

        public string GetJSONArray(string sql, int minrow = 0, int maxrow = 0, bool isprocedure = false, IIntwentySqlParameter[] parameters = null, IIntwentyResultColumn[] resultcolumns = null)
        {
            return InternalClient.GetJSONArray(sql,minrow,maxrow,isprocedure,parameters,resultcolumns);
        }

        public ResultSet GetResultSet(string sql, int minrow = 0, int maxrow = 0, bool isprocedure = false, IIntwentySqlParameter[] parameters = null, IIntwentyResultColumn[] resultcolumns = null)
        {
            return InternalClient.GetResultSet(sql, minrow, maxrow, isprocedure, parameters, resultcolumns);
        }

        public DataTable GetDataTable(string sql, int minrow = 0, int maxrow = 0, bool isprocedure = false, IIntwentySqlParameter[] parameters = null, IIntwentyResultColumn[] resultcolumns = null)
        {
            return InternalClient.GetDataTable(sql, minrow, maxrow, isprocedure, parameters, resultcolumns);
        }
    }


}
