using Intwenty.DataClient.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.DataClient.SQLBuilder
{
    abstract class BaseSqlBuilder
    {

        public BaseSqlBuilder() 
        {
          
        }

        public abstract string GetCreateTableSql(IntwentyDataTable model);

        public abstract string GetCreateIndexSql(IntwentyDataTableIndex model);

        public abstract string GetInsertSql<T>(IntwentyDataTable model, T instance, List<IntwentySqlParameter> parameters);

        protected abstract string GetCreateColumnSql(IntwentyDataColumn model);

    }
}
