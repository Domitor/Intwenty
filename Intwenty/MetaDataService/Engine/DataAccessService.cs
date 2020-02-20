using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Web;
using System.Configuration;
using NetCoreDBAccess;
using Microsoft.Extensions.Options;

namespace Moley.MetaDataService.Engine.Common
{




    public class DataAccessService : NetCoreDBClient   
	{

        public DataAccessService() : base(DBMS.MSSqlServer, "Data Source=localhost;Initial Catalog=IntwentyDB;User ID=sa;Password=thriller;MultipleActiveResultSets=true")
        {

        }	
	}

    /*

    interface IDPDataAccessService
    {
        void Open();
        void Close();
        void CreateCommand(string sqlcode);
        void CreateSPCommand(string procname);
       
    }


    public class DPDataAccessService : IDPDataAccessService
    {

        private IOptions<SystemSettings> SysSettings { get; }

        private NetCoreDBClient DBClient { get; }

        public DPDataAccessService(IOptions<SystemSettings> sysconfig)
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

  

    }

      */


}
