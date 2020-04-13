using System.Data;
using Microsoft.Extensions.Options;
using System.Text;
using Shared;
using Intwenty.Data.DBAccess;
using System.Collections.Generic;
using Intwenty.Data.DBAccess.Helpers;

namespace Intwenty.Data.DBAccess
{


    public interface IIntwentySqlDbAccessService
    {
        void Open();
        void Close();
        void CreateCommand(string sql);
        void CreateSPCommand(string procedurename);
        void AddParameter(string name, object value);
        void AddParameter(IntwentySqlParameter p);
        void FillDataset(DataSet ds, string tablename);
        object ExecuteScalarQuery();
        bool TableExist(string tablename);
        bool ColumnExist(string tablename, string columnname);
        NonQueryResult ExecuteNonQuery();
        DBMS GetDBMS();
        StringBuilder GetAsJSONArray(List<IIntwentyDataColum> columns, int minrow = 0, int maxrow = 0);
        StringBuilder GetAsJSONArray(int minrow = 0, int maxrow = 0);
        StringBuilder GetAsJSONObject();
        StringBuilder GetAsJSONObject(List<IIntwentyDataColum> columns);
        void CreateTable<T>(bool checkexisting=false, bool use_current_connection = false);
        List<T> Get<T>(bool use_current_connection = false) where T : new();
        int Insert<T>(T model, bool use_current_connection = false);
        int Update<T>(T model, bool use_current_connection = false);
        int Delete<T>(T model, bool use_current_connection = false);
        int DeleteRange<T>(IEnumerable<T> model, bool use_current_connection = false);
    }



    public class IntwentySqlDbAccessService : IIntwentySqlDbAccessService
    {

        private IOptions<SystemSettings> Settings { get; }

        private IOptions<ConnectionStrings> Connections { get; }

        private IntwentySqlDbClient DBClient { get; }

        public IntwentySqlDbAccessService(IOptions<SystemSettings> settings, IOptions<ConnectionStrings> connections)
        {
            Settings = settings;
            Connections = connections;
            DBClient = new IntwentySqlDbClient((DBMS)Settings.Value.IntwentyDBMS, Connections.Value.IntwentyConnection);
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

        public StringBuilder GetAsJSONArray(List<IIntwentyDataColum> columns, int minrow = 0, int maxrow = 0)
        {
            return DBClient.GetAsJSONArray(columns, minrow, maxrow);
        }

        public StringBuilder GetAsJSONArray(int minrow = 0, int maxrow=0)
        {
            return DBClient.GetAsJSONArray(minrow, maxrow);
        }

        public StringBuilder GetAsJSONObject()
        {
            return DBClient.GetAsJSONObject();
        }

        public StringBuilder GetAsJSONObject(List<IIntwentyDataColum> columns)
        {
            return DBClient.GetAsJSONObject(columns);
        }

        public DBMS GetDBMS()
        {
            return DBClient.GetDBMS();
        }

        public void CreateTable<T>(bool checkexisting=false, bool use_current_connection = false)
        {
            DBClient.CreateTable<T>(checkexisting, use_current_connection);
        }

        public List<T> Get<T>(bool use_current_connection = false) where T : new()
        {
            return DBClient.Get<T>(use_current_connection);
        }

        public int Insert<T>(T model, bool use_current_connection = false)
        {
            return DBClient.Insert(model, use_current_connection);
        }

        public int Update<T>(T model, bool use_current_connection = false)
        {
            return DBClient.Update(model, use_current_connection);
        }

        public int Delete<T>(T model, bool use_current_connection = false)
        {
            return DBClient.Delete(model, use_current_connection);
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
