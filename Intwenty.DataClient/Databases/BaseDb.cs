using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.DataClient.Databases
{
    public class BaseDb
    {
      
        protected string ConnectionString { get; set; }

        public BaseDb(string connectionstring)
        {
            ConnectionString = connectionstring;
        }

    }
}
