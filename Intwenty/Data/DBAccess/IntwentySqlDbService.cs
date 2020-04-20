using System.Data;
using Microsoft.Extensions.Options;
using System.Text;
using Shared;
using Intwenty.Data.DBAccess;
using System.Collections.Generic;
using Intwenty.Data.DBAccess.Helpers;

namespace Intwenty.Data.DBAccess
{



    public class IntwentySqlDbService : IIntwentyDbSql
    {

        private IOptions<SystemSettings> Settings { get; }

        private IOptions<ConnectionStrings> Connections { get; }

        private IntwentySqlDbClient DBClient { get; }

        public IntwentySqlDbService(IOptions<SystemSettings> settings, IOptions<ConnectionStrings> connections)
        {
            Settings = settings;
            Connections = connections;
            DBClient = new IntwentySqlDbClient((DBMS)Settings.Value.IntwentyDBMS, Connections.Value.IntwentyConnection);
        }

        public string ConnectionString
        {
            get { return DBClient.ConnectionString; }
        }

        public DBMS DbEngine
        {
            get { return DBClient.DbEngine; }
        }

        public bool IsNoSql
        {
            get { return DBClient.IsNoSql; }
        }

        public void Open()
        {
            DBClient.Open();
        }

        public void Close()
        {
            DBClient.Close();
        }

        public void CreateCommand(string sqlstmt)
        {
            DBClient.CreateCommand(sqlstmt);
        }

        public void CreateSPCommand(string procedurename)
        {
            DBClient.CreateSPCommand(procedurename);
        }

        public void AddParameter(string name, object value)
        {
            DBClient.AddParameter(name, value);
        }

        public void AddParameter(IntwentySqlParameter p)
        {
            DBClient.AddParameter(p);
        }

        public void FillDataset(DataSet ds, string tablename)
        {
            DBClient.FillDataset(ds, tablename);
        }

        public object ExecuteScalarQuery()
        {
            return DBClient.ExecuteScalarQuery();
        }

        public NonQueryResult ExecuteNonQuery()
        {
            return DBClient.ExecuteNonQuery();
        }

        public StringBuilder GetJSONArray(List<IIntwentyDataColum> returnfields, int minrow = 0, int maxrow = 0)
        {
            return DBClient.GetJSONArray(returnfields, minrow, maxrow);
        }

        public StringBuilder GetJSONArray(int minrow = 0, int maxrow=0)
        {
            return DBClient.GetJSONArray(minrow, maxrow);
        }

        public StringBuilder GetJSONObject()
        {
            return DBClient.GetJSONObject();
        }

        public StringBuilder GetJSONObject(List<IIntwentyDataColum> returnfields)
        {
            return DBClient.GetJSONObject(returnfields);
        }


        public void CreateTable<T>(bool checkexisting = false)
        {
            DBClient.CreateTable<T>(checkexisting, false);
        }

        public void CreateTable<T>(bool checkexisting=false, bool use_current_connection = false)
        {
            DBClient.CreateTable<T>(checkexisting, use_current_connection);
        }

        public T GetOne<T>(int id, bool use_current_connection = false) where T : new()
        {
            return DBClient.GetOne<T>(id, use_current_connection);
        }

        public T GetOne<T>(int id) where T : new()
        {
            return DBClient.GetOne<T>(id);
        }

        public T GetOne<T>(string id) where T : new()
        {
            return DBClient.GetOne<T>(id);
        }

        public List<T> GetAll<T>() where T : new()
        {
            return DBClient.GetAll<T>(false);
        }
        public List<T> GetAll<T>(bool use_current_connection = false) where T : new()
        {
            return DBClient.GetAll<T>(use_current_connection);
        }

        public int Insert<T>(T model)
        {
            return DBClient.Insert(model, false);
        }

        public int Insert<T>(T model, bool use_current_connection = false)
        {
            return DBClient.Insert(model, use_current_connection);
        }

        public int Update<T>(T model)
        {
            return DBClient.Update(model, false);
        }

        public int Update<T>(T model, bool use_current_connection = false)
        {
            return DBClient.Update(model, use_current_connection);
        }

        public int Delete<T>(T model)
        {
            return DBClient.Delete(model, false);
        }

        public int Delete<T>(T model, bool use_current_connection = false)
        {
            return DBClient.Delete(model, use_current_connection);
        }

        public int DeleteRange<T>(IEnumerable<T> model)
        {
            return DBClient.DeleteRange(model, false);
        }

        public int DeleteRange<T>(IEnumerable<T> model, bool use_current_connection = false)
        {
            return DBClient.DeleteRange(model, use_current_connection);
        }

        public bool TableExist(string tablename)
        {
            return DBClient.TableExist(tablename);
        }

        public bool ColumnExist(string tablename, string columnname)
        {
            return DBClient.ColumnExist(tablename, columnname);
        }

      
    }

      


}
