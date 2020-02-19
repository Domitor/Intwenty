using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Web;
using System.Configuration;
using NetCoreDBAccess;

namespace Moley.MetaDataService.Engine.Common
{

    public class DataAccessService : NetCoreDBClient   
	{
 
        public DataAccessService() : base(DBMS.MSSqlServer, "Data Source=localhost;Initial Catalog=MoleyDB;User ID=sa;Password=thriller;MultipleActiveResultSets=true")
        {


        }	
	}
}
