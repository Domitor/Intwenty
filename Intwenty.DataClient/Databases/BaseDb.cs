using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.DataClient.Databases
{
    public class BaseDb
    {
        protected bool IsInTransaction { get; set; }

        protected string ConnectionString { get; set; }

        public BaseDb(string connectionstring)
        {
            ConnectionString = connectionstring;
        }

    }
}
