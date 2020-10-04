using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.DataClient.Databases.NoSql
{
    public abstract class BaseNoSqlDb : BaseDb
    {
        public string DatabaseName { get; }

        public BaseNoSqlDb(string connectionstring, string databasename) : base(connectionstring)
        {
            DatabaseName = databasename;
        }

    }
}
