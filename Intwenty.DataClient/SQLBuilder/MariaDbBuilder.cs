using Intwenty.DataClient.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.DataClient.SQLBuilder
{
    sealed class MariaDbSqlBuilder : BaseSqlBuilder
    {
        public override string GetCreateIndexSql(IntwentyDbIndexDefinition model)
        {
            throw new NotImplementedException();
        }

        public override string GetCreateTableSql(IntwentyDbTableDefinition model)
        {
            throw new NotImplementedException();
        }

        public override string GetDeleteSql<T>(IntwentyDbTableDefinition model, T instance, List<IntwentySqlParameter> parameters)
        {
            throw new NotImplementedException();
        }

        public override string GetInsertSql<T>(IntwentyDbTableDefinition model, T instance, List<IntwentySqlParameter> parameters)
        {
            throw new NotImplementedException();
        }

        public override string GetUpdateSql<T>(IntwentyDbTableDefinition model, T instance, List<IntwentySqlParameter> parameters, List<IntwentySqlParameter> keyparameters)
        {
            throw new NotImplementedException();
        }

        protected override string GetCreateColumnSql(IntwentyDbColumnDefinition model)
        {
            throw new NotImplementedException();
        }
    }
}
