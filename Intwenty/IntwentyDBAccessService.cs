using System.Data;
using Microsoft.Extensions.Options;
using System.Text;
using Shared;
using Intwenty.Data.DBAccess;
using System.Collections.Generic;

namespace Intwenty
{


    public interface IIntwentyDbAccessService
    {
        void Open();
        void Close();
        void CreateCommand(string sql);
        void CreateSPCommand(string procedurename);
        void AddParameter(string name, object value);
        void AddParameter(IntwentySqlParameter p);
        void FillDataset(DataSet ds, string tablename);
        object ExecuteScalarQuery();
        NonQueryResult ExecuteNonQuery();
        DBMS GetDBMS();
        StringBuilder GetAsJSONArray();
        StringBuilder GetAsJSONArray(int minrow = 0, int maxrow = 0);
        StringBuilder GetAsJSONObject();
        void CreateTable<T>(bool checkexisting=false);
        List<T> Get<T>() where T : new();
        int Insert<T>(T model);
        int Update<T>(T model);
        int Delete<T>(T model);
        int DeleteRange<T>(IEnumerable<T> model);
    }



    public class IntwentyDbAccessService : IIntwentyDbAccessService
    {

        private IOptions<SystemSettings> Settings { get; }

        private IOptions<ConnectionStrings> Connections { get; }

        private IntwentyDBClient DBClient { get; }

        public IntwentyDbAccessService(IOptions<SystemSettings> settings, IOptions<ConnectionStrings> connections)
        {
            Settings = settings;
            Connections = connections;
            if (Settings.Value.DBMS==1)
                DBClient = new IntwentyDBClient((DBMS)Settings.Value.DBMS, Connections.Value.SqlServerConnection);
            if (Settings.Value.DBMS == 2)
                DBClient = new IntwentyDBClient((DBMS)Settings.Value.DBMS, Connections.Value.MySqlConnection);
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

        public StringBuilder GetAsJSONArray()
        {
            return DBClient.GetAsJSONArray(0,0);
        }

        public StringBuilder GetAsJSONArray(int minrow = 0, int maxrow=0)
        {
            return DBClient.GetAsJSONArray(minrow, maxrow);
        }

        public StringBuilder GetAsJSONObject()
        {
            return DBClient.GetAsJSONObject();
        }

        public DBMS GetDBMS()
        {
            return DBClient.GetDBMS();
        }

        public void CreateTable<T>(bool checkexisting=false)
        {
            DBClient.CreateTable<T>(checkexisting);
        }

        public List<T> Get<T>() where T : new()
        {
            return DBClient.Get<T>();
        }

        public int Insert<T>(T model)
        {
            return DBClient.Insert(model);
        }

        public int Update<T>(T model)
        {
            return DBClient.Update(model);
        }

        public int Delete<T>(T model)
        {
            return DBClient.Delete(model);
        }

        public int DeleteRange<T>(IEnumerable<T> model)
        {
            return DBClient.DeleteRange(model);
        }
    }

      


}
