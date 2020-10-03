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

        public abstract string GetCreateTableSql(IntwentyDbTableDefinition model);

        public abstract string GetCreateIndexSql(IntwentyDbIndexDefinition model);

        public abstract string GetInsertSql<T>(IntwentyDbTableDefinition model, T instance, List<IntwentySqlParameter> parameters);

        protected abstract string GetCreateColumnSql(IntwentyDbColumnDefinition model);

    }
}
