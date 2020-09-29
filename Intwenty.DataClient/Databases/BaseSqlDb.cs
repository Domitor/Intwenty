using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Channels;

namespace Intwenty.DataClient.Databases
{
    public class BaseSqlDb
    {
        protected string ConnectionString {get; set;}

        public BaseSqlDb(string connectionstring)
        {
            ConnectionString = connectionstring;
        }

        protected virtual IDbConnection GetConnection()
        {
            return null;
        }
        protected virtual IDbCommand GetCommand()
        {
            return null;
        }

        public virtual void Open() { }

        public virtual T GetOne<T>(int id) where T : new() { return default; }

        public virtual void Close()
        {
            var t = GetConnection();
            if (t != null)
                t.Close();
        }

        public virtual void AddParameter(string name, object value){}

    }
}
