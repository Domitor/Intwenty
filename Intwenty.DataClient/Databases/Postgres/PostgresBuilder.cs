using Intwenty.DataClient.Databases;
using Intwenty.DataClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Xml;

namespace Intwenty.DataClient.Databases.Postgres
{
    sealed class PostgresBuilder : BaseSqlBuilder
    {
        private static string CACHETYPE = "POSTGRES_SQL";

        public override string GetCreateTableSql(IntwentyDbTableDefinition model)
        {
            string result;
            var cachekey = CACHETYPE + "_CREATE_" + model.Id;
            var cache = MemoryCache.Default;
            result = cache.Get(cachekey) as string;
            if (!string.IsNullOrEmpty(result))
                return result;

            var sb = new StringBuilder();
            sb.Append("CREATE TABLE " + model.Name + " (");
            foreach (var m in model.Columns)
            {
                sb.Append(GetCreateColumnSql(m));
            }

            if (model.HasPrimaryKeyColumn)
                sb.Append(", " + string.Format("PRIMARY KEY ({0})", model.PrimaryKeyColumnNames));

            sb.Append(")");

            result = sb.ToString();

            cache.Add(cachekey, result, DateTime.Now.AddYears(1));

            return result;
        }

        public override string GetCreateIndexSql(IntwentyDbIndexDefinition model)
        {
            string result;
            var cachekey = CACHETYPE + "_CREATEINDEX_" + model.Id;
            var cache = MemoryCache.Default;
            result = cache.Get(cachekey) as string;
            if (!string.IsNullOrEmpty(result))
                return result;

            var sb = new StringBuilder();

            if (model.IsUnique)
                sb.Append(string.Format("CREATE UNIQUE INDEX {0} ON {1} ({2})", new object[] { model.Name, model.TableName, model.ColumnNames }));
            if (!model.IsUnique)
                sb.Append(string.Format("CREATE INDEX {0} ON {1} ({2})", new object[] { model.Name, model.TableName, model.ColumnNames }));

            result = sb.ToString();

            cache.Add(cachekey, result, DateTime.Now.AddYears(1));

            return result;
        }

        public override string GetInsertSql<T>(IntwentyDbTableDefinition model, T instance, List<IntwentySqlParameter> parameters)
        {
            string result;
            var cachekey = CACHETYPE + "_INSERT_" + model.Id;
            var cache = MemoryCache.Default;
            result = cache.Get(cachekey) as string;
            if (!string.IsNullOrEmpty(result))
            {

                foreach (var col in model.Columns.Where(p => !p.IsIgnore).OrderBy(p => p.Order))
                {
                    if (col.IsAutoIncremental)
                        continue;

                    var value = col.Property.GetValue(instance);
                    var prm = new IntwentySqlParameter();
                    prm.Name = "@" + col.Name;

                    if (value == null)
                        prm.Value = DBNull.Value;
                    else
                        prm.Value = value;

                    parameters.Add(prm);
                }


                return result;
            }

            var separator = "";
            var query = new StringBuilder(string.Format("INSERT INTO {0} (", model.Name));
            var values = new StringBuilder(" VALUES (");

            foreach (var col in model.Columns.Where(p => !p.IsIgnore).OrderBy(p => p.Order))
            {
                if (col.IsAutoIncremental)
                    continue;

                var value = col.Property.GetValue(instance);
                var prm = new IntwentySqlParameter();
                prm.Name = "@" + col.Name;

                if (value == null)
                    prm.Value = DBNull.Value;
                else
                    prm.Value = value;

                parameters.Add(prm);

                query.Append(string.Format("{0}{1}", separator, col.Name));
                values.Append(string.Format("{0}@{1}", separator, col.Name));
                separator = ",";

            }
            query.Append(") ");
            values.Append(")");

            result = query.Append(values).ToString();

            cache.Add(cachekey, result, DateTime.Now.AddYears(1));

            return result;
        }

        public override string GetUpdateSql<T>(IntwentyDbTableDefinition model, T instance, List<IntwentySqlParameter> parameters, List<IntwentySqlParameter> keyparameters)
        {
            string result;
            var cachekey = CACHETYPE + "_UPDATE_" + model.Id;
            var cache = MemoryCache.Default;
            result = cache.Get(cachekey) as string;
            if (!string.IsNullOrEmpty(result))
            {
                foreach (var col in model.Columns)
                {

                    var value = col.Property.GetValue(model);

                    if (!keyparameters.Exists(p => p.Name == col.Name) && value != null && col.IsAutoIncremental)
                        keyparameters.Add(new IntwentySqlParameter() { Name = col.Name, Value = value });


                    if (!keyparameters.Exists(p => p.Name == col.Name) && value != null && col.IsPrimaryKeyColumn)
                        keyparameters.Add(new IntwentySqlParameter() { Name = col.Name, Value = value });

                    if (keyparameters.Exists(p => p.Name == col.Name))
                        continue;

                    var prm = new IntwentySqlParameter() { Name = "@" + col.Name };
                    if (value == null)
                        prm.Value = DBNull.Value;
                    else
                        prm.Value = value;

                    parameters.Add(prm);

                }


                return result;

            }


            var separator = "";
            var query = new StringBuilder(string.Format("UPDATE {0} SET ", model.Name));

            foreach (var col in model.Columns)
            {

                var value = col.Property.GetValue(model);

                if (!keyparameters.Exists(p => p.Name == col.Name) && value != null && col.IsAutoIncremental)
                    keyparameters.Add(new IntwentySqlParameter() { Name = col.Name, Value = value });


                if (!keyparameters.Exists(p => p.Name == col.Name) && value != null && col.IsPrimaryKeyColumn)
                    keyparameters.Add(new IntwentySqlParameter() { Name = col.Name, Value = value });

                if (keyparameters.Exists(p => p.Name == col.Name))
                    continue;


                var prm = new IntwentySqlParameter() { Name = "@" + col.Name };
                if (value == null)
                    prm.Value = DBNull.Value;
                else
                    prm.Value = value;

                parameters.Add(prm);

                query.Append(separator + string.Format("{0}=@{0}", col.Name));
                separator = ", ";
            }



            query.Append(" WHERE ");
            var wheresep = "";
            foreach (var p in keyparameters)
            {
                query.Append(wheresep + string.Format("{0}=@{0}", p.Name));
                wheresep = " AND ";
            }

            result = query.ToString();
            cache.Add(cachekey, result, DateTime.Now.AddYears(1));

            return result;

        }

        public override string GetDeleteSql<T>(IntwentyDbTableDefinition model, T instance, List<IntwentySqlParameter> parameters)
        {
            string result;
            var cachekey = CACHETYPE + "_DELETE_" + model.Id;
            var cache = MemoryCache.Default;
            result = cache.Get(cachekey) as string;
            if (!string.IsNullOrEmpty(result))
            {
                foreach (var col in model.Columns)
                {

                    var value = col.Property.GetValue(model);

                    if (!parameters.Exists(p => p.Name == col.Name) && value != null && col.IsAutoIncremental)
                        parameters.Add(new IntwentySqlParameter() { Name = col.Name, Value = value });


                    if (!parameters.Exists(p => p.Name == col.Name) && value != null && col.IsPrimaryKeyColumn)
                        parameters.Add(new IntwentySqlParameter() { Name = col.Name, Value = value });
                }

                return result;

            }

            var query = new StringBuilder(string.Format("DELETE FROM {0} WHERE ", model.Name));

            foreach (var col in model.Columns)
            {

                var value = col.Property.GetValue(model);

                if (!parameters.Exists(p => p.Name == col.Name) && value != null && col.IsAutoIncremental)
                    parameters.Add(new IntwentySqlParameter() { Name = col.Name, Value = value });


                if (!parameters.Exists(p => p.Name == col.Name) && value != null && col.IsPrimaryKeyColumn)
                    parameters.Add(new IntwentySqlParameter() { Name = col.Name, Value = value });

            }

            query.Append(" WHERE ");
            var wheresep = "";
            foreach (var p in parameters)
            {
                query.Append(wheresep + string.Format("{0}=@{0}", p.Name));
                wheresep = " AND ";
            }

            result = query.ToString();
            cache.Add(cachekey, result, DateTime.Now.AddYears(1));

            return result;

        }


        protected override string GetCreateColumnSql(IntwentyDbColumnDefinition model)
        {
            var result = string.Empty;
            var allownullvalue = "NULL";
            var datatype = string.Empty;
            var longtext = false;


            var dtmap = TypeMap.GetTypeMap().Find(p => p.NetType == model.GetNetType() && ((longtext && p.Length == StringLength.Long) || (!longtext && p.Length == StringLength.Standard)) && p.DbEngine == DBMS.PostgreSQL);
            if (dtmap == null)
                throw new InvalidOperationException(string.Format("Could not find DBMS specific datatype for {0} and {1}", model.GetNetType(), DBMS.PostgreSQL));

            datatype = dtmap.DBMSDataType;

            var autoincmap = new CommandMapItem() { Key = "AUTOINC" };
            if (model.IsAutoIncremental)
            {
                allownullvalue = "NOT NULL";
                datatype = CommandMap.GetCommandMap().Find(p => p.DbEngine == DBMS.PostgreSQL && p.Key == "AUTOINC").Command;
            }

            if (model.IsNullNotAllowed)
                allownullvalue = "NOT NULL";

            result = string.Format("{0} {1} {2}", new object[] { model.Name, datatype, allownullvalue });
            if (model.Order > 0)
                result = ", " + result;

            if (string.IsNullOrEmpty(result))
                throw new InvalidOperationException("Could not generate sql column definition");

            return result;
        }

    }
}
