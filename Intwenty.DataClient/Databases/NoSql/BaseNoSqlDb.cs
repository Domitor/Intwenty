using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.DataClient.Databases.NoSql
{
    public abstract class BaseNoSqlDb : BaseDb
    {

        public BaseNoSqlDb(string connectionstring) : base(connectionstring)
        {
         
        }

        public virtual T GetOne<T>(int id) where T : new() { return default; }
    }
}
