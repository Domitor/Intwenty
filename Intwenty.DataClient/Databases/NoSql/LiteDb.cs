using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.DataClient.Databases.NoSql
{
    public sealed class LiteDb : BaseNoSqlDb, INoSqlClient
    {
        public LiteDb(string connectionstring) : base(connectionstring)
        {

        }
    }
}
