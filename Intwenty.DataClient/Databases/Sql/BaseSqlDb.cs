using Intwenty.DataClient.Model;
using Intwenty.DataClient.Reflection;
using Intwenty.DataClient.SQLBuilder;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Intwenty.DataClient.Databases.Sql
{
    abstract class BaseSqlDb : BaseDb, IDisposable, ISqlClient
    {

        public BaseSqlDb(string connectionstring) : base(connectionstring)
        {
        }


        public abstract void Dispose();
        public abstract void Open();
        public abstract void Close();
        protected abstract BaseSqlBuilder GetSqlBuilder();

        public void BeginTransaction()
        {
            IsInTransaction = true;
        }

        public void CommitTransaction()
        {
            var transaction = GetTransaction();
            if (IsInTransaction && transaction != null)
            {
                IsInTransaction = false;
                transaction.Commit();
                Close();
            }
           
        }

        public void RollbackTransaction() 
        {
            var transaction = GetTransaction();
            if (IsInTransaction && transaction != null)
            {
                IsInTransaction = false;
                transaction.Rollback();
                Close();
            }
        }


        public void RunCommand(string sql, bool isprocedure = false, IIntwentySqlParameter[] parameters = null)
        {
            var command = GetCommand();
            command.CommandText = sql;
            if (isprocedure)
                command.CommandType = CommandType.StoredProcedure;
            else
                command.CommandType = CommandType.Text;

            AddCommandParameters(parameters);

            command.ExecuteNonQuery();
        }

        public string GetJSONObject(string sql, bool isprocedure = false, IIntwentySqlParameter[] parameters = null, IIntwentyResultColumn[] resultcolumns = null)
        {
            var command = GetCommand();
            command.CommandText = sql;
            if (isprocedure)
                command.CommandType = CommandType.StoredProcedure;
            else
                command.CommandType = CommandType.Text;

            AddCommandParameters(parameters);

            var reader = command.ExecuteReader();

            var adjusted_columns = new List<IntwentyResultColumn>();
            var separator = "";
            var sb = new StringBuilder();
            sb.Append("{");
            while (reader.Read())
            {
                var names = new List<string>();
                for (int i = 0; i < reader.FieldCount; i++)
                    names.Add(reader.GetName(i));

                adjusted_columns = AdjustResultColumns(names, resultcolumns);

                foreach (var rc in adjusted_columns)
                {
                    if (reader.IsDBNull(rc.Index))
                        continue;

                    sb.Append(separator + GetJSONValue(reader, rc));
                    separator = ",";
                }
                break;
            }
            sb.Append("}");

            return sb.ToString();
        }

        public string GetJSONArray(string sql, int minrow = 0, int maxrow = 0, bool isprocedure = false, IIntwentySqlParameter[] parameters = null, IIntwentyResultColumn[] resultcolumns = null)
        {
            var command = GetCommand();
            command.CommandText = sql;
            if (isprocedure)
                command.CommandType = CommandType.StoredProcedure;
            else
                command.CommandType = CommandType.Text;

            AddCommandParameters(parameters);

            var reader = command.ExecuteReader();

            var adjusted_columns = new List<IntwentyResultColumn>();
            var rindex = 0;
            var objectseparator = "";
            var valueseparator = "";
            var sb = new StringBuilder();
            sb.Append("[");
            while (reader.Read())
            {
                rindex += 1;
                if (maxrow > minrow && (minrow > 0 || maxrow > 0))
                {
                    if (rindex <= minrow)
                        continue;
                    if (rindex > maxrow)
                        break;
                }
                if (rindex == 1)
                {
                    var names = new List<string>();
                    for (int i = 0; i < reader.FieldCount; i++)
                        names.Add(reader.GetName(i));

                    adjusted_columns = AdjustResultColumns(names, resultcolumns);
                }

                sb.Append(objectseparator + "{");
                foreach (var rc in adjusted_columns)
                {
                    if (reader.IsDBNull(rc.Index))
                        continue;

                    sb.Append(valueseparator + GetJSONValue(reader, rc));
                    valueseparator = ",";
                }
                sb.Append("}");
                objectseparator = ",";
            }
            sb.Append("]");

            return sb.ToString();
        }

        public void CreateTable<T>()
        {
            var info = TypeDataHandler.GetDbTableDefinition<T>();

            if (TableExists<T>())
                return;

            var command = GetCommand();
            command.CommandText = GetSqlBuilder().GetCreateTableSql(info);
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();

            foreach (var index in info.Indexes)
            {
                command = GetCommand();
                command.CommandText = GetSqlBuilder().GetCreateIndexSql(index);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
        }

       

        public bool TableExists<T>()
        {
            var info = TypeDataHandler.GetDbTableDefinition<T>();
            return TableExists(info.Name);
        }

        public bool TableExists(string tablename)
        {
            try
            {
                var checkcommand = GetCommand();
                checkcommand.CommandText = string.Format("SELECT 1 FROM {0}", tablename);
                checkcommand.CommandType = CommandType.Text;
                checkcommand.ExecuteScalar();

                return true;
            }
            catch { }

            return false;
        }

        public bool ColumnExists(string tablename, string columnname)
        {
            try
            {
                var checkcommand = GetCommand();
                checkcommand.CommandText = string.Format("SELECT {0} FROM {1} WHERE 1=2", columnname,tablename);
                checkcommand.CommandType = CommandType.Text;
                checkcommand.ExecuteScalar();

                return true;
            }
            catch { }

            return false;
        }

        public virtual T GetEntity<T>(int id) where T : new()
        {
            return GetEntity<T>(Convert.ToString(id));
        }

        public virtual T GetEntity<T>(string id) where T : new() 
        {
            var res = new T();
            var info = TypeDataHandler.GetDbTableDefinition<T>();

            if (info.PrimaryKeyColumnNamesList.Count == 0)
                throw new InvalidOperationException("No primary key column found");
            if (info.PrimaryKeyColumnNamesList.Count > 1)
                throw new InvalidOperationException(string.Format("The table {0} uses a composite primary key", info.Name));

            var command = GetCommand();
            command.CommandText = string.Format("SELECT * FROM {0} WHERE {1}={2}", new[] {info.Name,info.PrimaryKeyColumnNamesList[0], id });
            command.CommandType = CommandType.Text;

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                foreach (var col in info.Columns.Where(p => !p.IsIgnore).OrderBy(p => p.Order))
                {

                    if (reader.IsDBNull(col.Order))
                        continue;

                    SetPropertyValues(reader, col, res);

                }

            }

            return res;
        }

        public virtual List<T> GetEntities<T>() where T : new()
        {
            var res = new List<T>();
            var info = TypeDataHandler.GetDbTableDefinition<T>();

            var command = GetCommand();
            command.CommandText = string.Format("SELECT * FROM {0}", info.Name);
            command.CommandType = CommandType.Text;

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var m = new T();
                foreach (var col in info.Columns.Where(p => !p.IsIgnore).OrderBy(p => p.Order))
                {

                    if (reader.IsDBNull(col.Order))
                        continue;

                    SetPropertyValues(reader, col, m);

                }
                res.Add(m);
            }

            return res;
        }

        public virtual List<T> GetEntities<T>(string sql, bool isprocedure = false, IIntwentySqlParameter[] parameters = null) where T : new()
        {
            var res = new List<T>();
            var info = TypeDataHandler.GetDbTableDefinition<T>(sql);

            var command = GetCommand();
            command.CommandText = sql;
            if (isprocedure)
                command.CommandType = CommandType.StoredProcedure;
            else
                command.CommandType = CommandType.Text;

            AddCommandParameters(parameters);

            var reader = command.ExecuteReader();

            TypeDataHandler.AdjustColumnDefinitionToQueryResult(info, reader);

            while (reader.Read())
            {
                var m = new T();
                foreach (var col in info.Columns.Where(p=> !p.IsIgnore).OrderBy(p=> p.Order))
                {
                 
                    if (reader.IsDBNull(col.Order))
                        continue;

                    SetPropertyValues(reader, col, m);

                }
                res.Add(m);
            }

            return res;
        }


        public virtual int InsertEntity<T>(T entity)
        {
            var info = TypeDataHandler.GetDbTableDefinition<T>();
            var parameters = new List<IntwentySqlParameter>();


            var command = GetCommand();
            command.CommandText = GetSqlBuilder().GetInsertSql(info, entity, parameters);
            command.CommandType = CommandType.Text;

            AddCommandParameters(parameters.ToArray());

            var res = command.ExecuteNonQuery();

            HandleInsertAutoIncrementation(info, parameters, entity);

            return res;

        }

        public int UpdateEntity<T>(T entity)
        {
            var info = TypeDataHandler.GetDbTableDefinition<T>();
            var parameters = new List<IntwentySqlParameter>();
            var keyparameters = new List<IntwentySqlParameter>();

            var sql = GetSqlBuilder().GetUpdateSql(info, entity, parameters,keyparameters);
            if (keyparameters.Count == 0)
                throw new InvalidOperationException("Can't update a table without 'Primary Key' or an 'Auto Increment' column, please use annotations.");

            var command = GetCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;

            AddCommandParameters(keyparameters.ToArray());
            AddCommandParameters(parameters.ToArray());

            var res = command.ExecuteNonQuery();

            return res;
        }

        public int DeleteEntities<T>(List<T> entities)
        {
            var res = 0;
            foreach (var t in entities)
            {
                res += DeleteEntity(t);
            }
            return res;
        }

        public int DeleteEntity<T>(T entity)
        {
            var info = TypeDataHandler.GetDbTableDefinition<T>();
            var parameters = new List<IntwentySqlParameter>();

            var sql = GetSqlBuilder().GetDeleteSql(info, entity, parameters);
            if (parameters.Count == 0)
                throw new InvalidOperationException("Can't delete rows in a table without 'Primary Key' or an 'Auto Increment' column, please use annotations.");

            var command = GetCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;

            AddCommandParameters(parameters.ToArray());

            var res = command.ExecuteNonQuery();

            return res;
        }

        protected abstract IDbCommand GetCommand();

        protected abstract IDbTransaction GetTransaction();

        protected virtual void SetPropertyValues<T>(IDataReader reader, IntwentyDbColumnDefinition column, T instance)
        {
            if (column.Property.PropertyType.ToString().ToUpper() == "SYSTEM.INT32")
                column.Property.SetValue(instance, reader.GetInt32(column.Order), null);
            else if (column.Property.PropertyType.ToString().ToUpper() == "SYSTEM.BOOLEAN")
                column.Property.SetValue(instance, reader.GetBoolean(column.Order), null);
            else if (column.Property.PropertyType.ToString().ToUpper() == "SYSTEM.DECIMAL")
                column.Property.SetValue(instance, reader.GetDecimal(column.Order), null);
            else if (column.Property.PropertyType.ToString().ToUpper() == "SYSTEM.SINGLE")
                column.Property.SetValue(instance, reader.GetFloat(column.Order), null);
            else if (column.Property.PropertyType.ToString().ToUpper() == "SYSTEM.DOUBLE")
                column.Property.SetValue(instance, reader.GetDouble(column.Order), null);
            else
                column.Property.SetValue(instance, reader.GetValue(column.Order), null);
        }

        protected abstract void AddCommandParameters(IIntwentySqlParameter[] parameters);
            

        protected abstract void HandleInsertAutoIncrementation<T>(IntwentyDbTableDefinition info, List<IntwentySqlParameter> parameters, T entity);

        protected string GetJSONValue(IDataReader r, IntwentyResultColumn resultcol)
        {
            if (r.IsDBNull(resultcol.Index))
                return string.Empty;

            var columnname = r.GetName(resultcol.Index);
            var datatypename = r.GetDataTypeName(resultcol.Index);

            if (IsNumeric(datatypename, resultcol))
                return "\"" + columnname + "\":" + Convert.ToString(r.GetValue(resultcol.Index)).Replace(",", ".");
            else if (IsDateTime(datatypename, resultcol))
                return "\"" + columnname + "\":" + "\"" + System.Text.Json.JsonEncodedText.Encode(Convert.ToDateTime(r.GetValue(resultcol.Index)).ToString("yyyy-MM-dd")).ToString() + "\"";
            else
                return "\"" + columnname + "\":" + "\"" + System.Text.Json.JsonEncodedText.Encode(Convert.ToString(r.GetValue(resultcol.Index))).ToString() + "\"";

        }

        protected bool IsNumeric(string datatypename, IntwentyResultColumn resultcolumn)
        {
            //if (resultcolumn != null)
            //    return resultcolumn.IsNumeric;

            if (datatypename.ToUpper() == "REAL")
                return true;
            if (datatypename.ToUpper() == "INTEGER")
                return true;

 
            return false;
        }

        protected bool IsDateTime(string datatypename, IntwentyResultColumn resultcolumn)
        {
            //if (resultcolumn != null)
            //    return resultcolumn.IsDateTime;

            if (datatypename.ToUpper() == "DATETIME")
                return true;
           

            return false;
        }

        private List<IntwentyResultColumn> AdjustResultColumns(List<string> schema, IIntwentyResultColumn[] resultcolumns)
        {
            var res = new List<IntwentyResultColumn>();
            if (resultcolumns == null || (resultcolumns != null && resultcolumns.Count() == 0))
            {
                for (int i = 0; i < schema.Count; i++)
                {
                    var rc = new IntwentyResultColumn() { Name = schema[i], Index = i };
                    //TODO: IsNumeric, IsDateTime
                    res.Add(rc);
                }
            }
            else
            {
                for (int i = 0; i < schema.Count; i++)
                {
                    var col = resultcolumns.FirstOrDefault(p => p.Name.ToLower() == schema[i].ToLower());
                    if (col != null)
                    {
                        var rc = new IntwentyResultColumn() { Name = schema[i], Index = i };
                        //TODO: IsNumeric, IsDateTime
                        res.Add(rc);
                    }
                }
            }

            return res;
        }

      
    }
}
