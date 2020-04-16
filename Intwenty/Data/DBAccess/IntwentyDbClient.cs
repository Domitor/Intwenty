using Intwenty.Data.DBAccess.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Data.DBAccess
{
    public abstract class IntwentyDbClient : IIntwentyDb
    {
         public string ConnectionString { get; set; }

        public DBMS DbEngine { get; set; }

        public bool IsNoSql
        {
            get 
            {
                if (DbEngine == DBMS.LiteDb || DbEngine == DBMS.MongoDb)
                    return true;

                return false;
            } 
        }

    }
}
