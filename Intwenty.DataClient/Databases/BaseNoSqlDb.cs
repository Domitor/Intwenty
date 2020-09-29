using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.DataClient.Databases
{
    public class BaseNoSqlDb
    {
        protected string ConnectionString { get; set; }

        public BaseNoSqlDb(string connectionstring)
        {
            ConnectionString = connectionstring;
        }

        public virtual T GetOne<T>(int id) where T : new() { return default; }
    }
}
