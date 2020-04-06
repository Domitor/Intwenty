using System.Data;
using Microsoft.Extensions.Options;
using System.Text;
using Shared;
using Intwenty.Data.DBAccess;
using System.Collections.Generic;

namespace Intwenty
{


    public interface IDataAccessService
    {
        void Open();
        void Close();
        void CreateCommand(string sqlcode);
        void CreateSPCommand(string procname);
        void AddParameter(string name, object value);
        void AddParameter(IntwentySqlParameter p);
        void FillDataset(DataSet ds, string srcTable);
        object ExecuteScalarQuery();
        NonQueryResult ExecuteNonQuery();
        DBMS GetDBMS();
        StringBuilder GetAsJSONArray();
        StringBuilder GetAsJSONArray(int minrow = 0, int maxrow = 0);
        StringBuilder GetAsJSONObject();
        void CreateTable<T>();
        List<T> Get<T>() where T : new();
        int Insert<T>(T model);
        int Update<T>(T model);
        int Delete<T>(T model);
    }



    public class IntwentyDbAccessService : IDataAccessService
    {

        private IOptions<SystemSettings> Settings { get; }

        private IOptions<ConnectionStrings> Connections { get; }

        private IntwentyDBClient DBClient { get; }

        public IntwentyDbAccessService(IOptions<SystemSettings> settings, IOptions<ConnectionStrings> connections)
        {
            Settings = settings;
            Connections = connections;
            DBClient = new IntwentyDBClient((DBMS)Settings.Value.DBMS, Connections.Value.DefaultConnection);
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

        public void CreateSPCommand(string procname)
        {
            DBClient.CreateSPCommand(procname);
        }

        public void AddParameter(string name, object value)
        {
            DBClient.AddParameter(name, value);
        }

        public void AddParameter(IntwentySqlParameter p)
        {
            DBClient.AddParameter(p);
        }

        public void FillDataset(DataSet ds, string srcTable)
        {
            DBClient.FillDataset(ds, srcTable);
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

        public void CreateTable<T>()
        {
            DBClient.CreateTable<T>();
        }

        public List<T> Get<T>() where T : new()
        {
            return DBClient.Get<T>();
        }

        public int Insert<T>(T model)
        {
            return DBClient.Insert<T>(model);
        }

        public int Update<T>(T model)
        {
            return DBClient.Update<T>(model);
        }

        public int Delete<T>(T model)
        {
            return DBClient.Delete<T>(model);
        }
    }

      


}
