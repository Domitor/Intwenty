using Intwenty.DataClient.Model;
using System.Collections.Generic;


namespace Intwenty.DataClient.Databases
{
    abstract class BaseSqlBuilder
    {

        public BaseSqlBuilder() 
        {
          
        }

        public abstract string GetCreateTableSql(IntwentyDbTableDefinition model);

        public abstract string GetCreateIndexSql(IntwentyDbIndexDefinition model);

        public abstract string GetInsertSql<T>(IntwentyDbTableDefinition model, T instance, List<IntwentySqlParameter> parameters);

        public abstract string GetUpdateSql<T>(IntwentyDbTableDefinition model, T instance, List<IntwentySqlParameter> parameters, List<IntwentySqlParameter> keyparameters);

        public abstract string GetDeleteSql<T>(IntwentyDbTableDefinition model, T instance, List<IntwentySqlParameter> parameters);

        protected abstract string GetCreateColumnSql(IntwentyDbColumnDefinition model);

    }
}
