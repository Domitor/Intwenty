using Intwenty.DataClient.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.DataClient.SQLBuilder
{
    sealed class MariaDbSqlBuilder : BaseSqlBuilder
    {
        public override string GetCreateIndexSql(IntwentyDataTableIndex model)
        {
            throw new NotImplementedException();
        }

        public override string GetCreateTableSql(IntwentyDataTable model)
        {
            throw new NotImplementedException();
        }

        public override string GetInsertSql<T>(IntwentyDataTable model, T instance, List<IntwentySqlParameter> parameters)
        {
            throw new NotImplementedException();
        }

        protected override string GetCreateColumnSql(IntwentyDataColumn model)
        {
            throw new NotImplementedException();
        }
    }
}
