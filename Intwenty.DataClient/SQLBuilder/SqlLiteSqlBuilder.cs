using Intwenty.DataClient.Databases;
using Intwenty.DataClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace Intwenty.DataClient.SQLBuilder
{
    sealed class SqlLiteSqlBuilder : BaseSqlBuilder
    {
        private static string CACHETYPE = "SQLITE_SQL";

        public override string GetCreateTableSql(IntwentyDataTable model)
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

            //IF SQLLITE, PK ONLY IF NOT AUTOINC. IF AUTOINC THAT COL IS PK.
            sb.Append(", " + string.Format("PRIMARY KEY ({0})", model.PrimaryKeyColumnNames) + ")");

            result = sb.ToString();

            cache.Add(cachekey, result, DateTime.Now.AddYears(1));

            return result;
        }

        public override string GetCreateIndexSql(IntwentyDataTableIndex model)
        {
            string result;
            var cachekey = CACHETYPE + "_CREATEINDEX_" + model.Id;
            var cache = MemoryCache.Default;
            result = cache.Get(cachekey) as string;
            if (!string.IsNullOrEmpty(result))
                return result;

            var sb = new StringBuilder();

            if (model.IsUnique)
                sb.Append(string.Format("CREATE UNIQUE INDEX {0} ON {1} (", model.Name, model.TableName));
            if (!model.IsUnique)
                sb.Append(string.Format("CREATE INDEX {0} ON {1} (", model.Name, model.TableName));

            for(int i=0; i< model.ColumnNamesList.Count; i++)
            {

                if (i == 0)
                    sb.Append(string.Format("{0}", model.ColumnNamesList[i]));
               else
                    sb.Append("," + string.Format("{0}", model.ColumnNamesList[i]));

            }
            sb.Append(")");

            result = sb.ToString();

            cache.Add(cachekey, result, DateTime.Now.AddYears(1));

            return result;
        }

        public override string GetInsertSql<T>(IntwentyDataTable model, T instance, List<IntwentySqlParameter> parameters)
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

                    if (col.Property.PropertyType.FullName.ToUpper().Contains("SYSTEM.DATETIMEOFFSET") && value != null)
                        value = ((DateTimeOffset)value).DateTime;

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

            var query = new StringBuilder(string.Format("INSERT INTO {0} (", model.Name));
            var values = new StringBuilder(" VALUES (");

            foreach (var col in model.Columns.Where(p => !p.IsIgnore).OrderBy(p => p.Order))
            {
                if (col.IsAutoIncremental)
                    continue;

                var value = col.Property.GetValue(instance);

                if (col.Property.PropertyType.FullName.ToUpper().Contains("SYSTEM.DATETIMEOFFSET") && value != null)
                    value = ((DateTimeOffset)value).DateTime;

                var prm = new IntwentySqlParameter();
                prm.Name = "@" + col.Name;

                if (value == null)
                    prm.Value = DBNull.Value;
                else
                    prm.Value = value;

                if (col.Order == 0)
                    query.Append(string.Format("{0}", col.Name));
                else
                    query.Append(string.Format(",{0}", col.Name));

                if (col.Order == 0)
                    values.Append(string.Format("@{0}", col.Name));
                else
                    values.Append(string.Format(",@{0}", col.Name));

                parameters.Add(prm);

            }
            query.Append(") ");
            values.Append(")");

            result = query.Append(values).ToString();

            cache.Add(cachekey, result, DateTime.Now.AddYears(1));

            return result;
        }


        protected override string GetCreateColumnSql(IntwentyDataColumn model)
        {
            var result = string.Empty;
            var allownullvalue = "NULL";
            var autoinccommand = string.Empty;
            var datatype = string.Empty;
            var longtext = false;


            var dtmap = TypeMap.GetTypeMap().Find(p => p.NetType == model.GetNetType() && ((longtext && p.Length == StringLength.Long) || (!longtext && p.Length == StringLength.Standard)) && p.DbEngine == SqlDBMS.SQLite);
            if (dtmap == null)
                throw new InvalidOperationException(string.Format("Could not find DBMS specific datatype for {0} and {1}", model.GetNetType(), SqlDBMS.SQLite));

            datatype = dtmap.DBMSDataType;

            var autoincmap = new CommandMapItem() { Key = "AUTOINC" };
            if (model.IsAutoIncremental)
            {
                allownullvalue = "NOT NULL";
                autoinccommand = CommandMap.GetCommandMap().Find(p => p.DbEngine == SqlDBMS.SQLite && p.Key == "AUTOINC").Command;   
            }

            if (model.IsNullNotAllowed)
                allownullvalue = "NOT NULL";

            result = string.Format("{0} {1} {2} {3}", new object[] { model.Name, datatype, allownullvalue, autoinccommand });
            if (model.Order > 0)
                result = ", " + result;

            if (string.IsNullOrEmpty(result))
                throw new InvalidOperationException("Could not generate sql column definition");

            return result;
        }

      
    }
}
