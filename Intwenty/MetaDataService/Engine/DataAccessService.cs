using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Web;
using System.Configuration;
using NetCoreDBAccess;
using Microsoft.Extensions.Options;
using System.Text;

namespace Moley.MetaDataService.Engine.Common
{


    public class DataAccessClient : NetCoreDBClient   
	{

        public DataAccessClient() : base(DBMS.MSSqlServer, "Data Source=localhost;Initial Catalog=IntwentyDB;User ID=sa;Password=thriller;MultipleActiveResultSets=true")
        {

        }	
	}


    interface IDataAccessService
    {
        void Open();
        void Close();
        void CreateCommand(string sqlcode);
        void CreateSPCommand(string procname);
        void AddParameter(string name, object value);
        void FillDataset(DataSet ds, string srcTable);
        object ExecuteScalarQuery();
        int ExecuteNonQuery();
        StringBuilder GetAsJSONArray();
        StringBuilder GetAsJSONObject();
    }


    public class DataAccessService : IDataAccessService
    {

        private IOptions<SystemSettings> SysSettings { get; }

        private NetCoreDBClient DBClient { get; }

        public DataAccessService(IOptions<SystemSettings> sysconfig)
        {
            SysSettings = sysconfig;
            DBClient = new NetCoreDBClient(DBMS.MSSqlServer, SysSettings.Value.DBConn);
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
            return DBClient.GetAsJSONArray();
        }

        public StringBuilder GetAsJSONObject()
        {
            return DBClient.GetAsJSONObject();
        }
    }

      


}
