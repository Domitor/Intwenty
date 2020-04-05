using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Web;
using System.Configuration;
using Microsoft.Extensions.Options;
using System.Text;
using Shared;
using Intwenty.Data.DBAccess;

namespace Intwenty
{


    public interface IDataAccessService
    {
        void Open();
        void Close();
        void CreateCommand(string sqlcode);
        void CreateSPCommand(string procname);
        void AddParameter(string name, object value);
        void AddParameter(SqlStmtParameter p);
        void FillDataset(DataSet ds, string srcTable);
        object ExecuteScalarQuery();
        int ExecuteNonQuery();

        DBMS GetDBMS();

        StringBuilder GetAsJSONArray();

        StringBuilder GetAsJSONArray(int minrow = 0, int maxrow = 0);

        StringBuilder GetAsJSONObject();
    }



    public class IntwentyDbAccessService : IDataAccessService
    {

        private IOptions<SystemSettings> Settings { get; }

        private IOptions<ConnectionStrings> Connections { get; }

        private NetCoreDBClient DBClient { get; }

        public IntwentyDbAccessService(IOptions<SystemSettings> settings, IOptions<ConnectionStrings> connections)
        {
            Settings = settings;
            Connections = connections;
            DBClient = new NetCoreDBClient((DBMS)Settings.Value.DBMS, Connections.Value.DefaultConnection);
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

        public void AddParameter(SqlStmtParameter p)
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

        public int ExecuteNonQuery()
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
    }

      


}
