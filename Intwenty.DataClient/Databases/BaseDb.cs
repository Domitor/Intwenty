using Intwenty.DataClient.Model;
using Intwenty.DataClient.Reflection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Intwenty.DataClient.Databases
{
    abstract class BaseDb : IDisposable, IDataClient
    {
        protected string ConnectionString { get; set; }

        protected bool IsInTransaction { get; set; }

        public abstract DBMS Database { get; }

        public BaseDb(string connectionstring) 
        {
            ConnectionString = connectionstring;
        }
        public abstract void Dispose();
        public abstract void Open();
        public abstract void Close();
        protected abstract BaseSqlBuilder GetSqlBuilder();



        public List<TypeMapItem> GetDbTypeMap()
        {
            return TypeMap.GetTypeMap();
        }

        public List<CommandMapItem> GetDbCommandMap()
        {
            return CommandMap.GetCommandMap();
        }

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
            try
            {
                using (var command = GetCommand())
                {
                    command.CommandText = sql;
                    if (isprocedure)
                        command.CommandType = CommandType.StoredProcedure;
                    else
                        command.CommandType = CommandType.Text;

                    AddCommandParameters(parameters, command);

                    command.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {
                Dispose();
                throw ex;
            }
        }

        public object GetScalarValue(string sql, bool isprocedure = false, IIntwentySqlParameter[] parameters = null)
        {
            object res;

            try
            {
                using (var command = GetCommand())
                {
                    command.CommandText = sql;
                    if (isprocedure)
                        command.CommandType = CommandType.StoredProcedure;
                    else
                        command.CommandType = CommandType.Text;

                    AddCommandParameters(parameters, command);

                    res = command.ExecuteScalar();
                }

            }
            catch (Exception ex)
            {
                Dispose();
                throw ex;
            }

            return res;
        }


        public string GetJSONObject(string sql, bool isprocedure = false, IIntwentySqlParameter[] parameters = null, IIntwentyResultColumn[] resultcolumns = null)
        {
            var sb = new StringBuilder();

            try
            {
                using (var command = GetCommand())
                {
                    command.CommandText = sql;
                    if (isprocedure)
                        command.CommandType = CommandType.StoredProcedure;
                    else
                        command.CommandType = CommandType.Text;

                    AddCommandParameters(parameters, command);

                    var reader = command.ExecuteReader();

                    var separator = "";


                    while (reader.Read())
                    {
                        
                         var adjusted_columns = AdjustResultColumns(reader, resultcolumns);

                        sb.Append("{");
                        foreach (var rc in adjusted_columns)
                        {
                            if (reader.IsDBNull(rc.Index))
                                continue;

                            sb.Append(separator + GetJSONValue(reader, rc));
                            separator = ",";
                        }
                        sb.Append("}");
                        break;
                    }

                    reader.Close();
                    reader.Dispose();
                   

                }

            }
            catch (Exception ex)
            {
                Dispose();
                throw ex;
            }

            return sb.ToString();
        }

        public string GetJSONArray(string sql, int minrow = 0, int maxrow = 0, bool isprocedure = false, IIntwentySqlParameter[] parameters = null, IIntwentyResultColumn[] resultcolumns = null)
        {
            var sb = new StringBuilder();

            try
            {
                using (var command = GetCommand())
                {
                    command.CommandText = sql;
                    if (isprocedure)
                        command.CommandType = CommandType.StoredProcedure;
                    else
                        command.CommandType = CommandType.Text;

                    AddCommandParameters(parameters, command);

                    var reader = command.ExecuteReader();


                    var adjusted_columns = new List<IntwentyResultColumn>();
                    var rindex = 0;
                    char objectseparator = ' ';
                    char valueseparator;

                    sb.Append("[");
                    while (reader.Read())
                    {
                        valueseparator = ' ';
                        rindex += 1;
                        if (maxrow > minrow && (minrow > 0 || maxrow > 0))
                        {
                            if (rindex <= minrow)
                                continue;
                            if (rindex > maxrow)
                                break;
                        }
                      
                        if (adjusted_columns.Count == 0)
                            adjusted_columns = AdjustResultColumns(reader, resultcolumns);


                        sb.Append(objectseparator + "{");
                        foreach (var rc in adjusted_columns)
                        {
                            if (reader.IsDBNull(rc.Index))
                                continue;

                            sb.Append(valueseparator + GetJSONValue(reader, rc));
                            valueseparator = ',';
                        }
                        sb.Append("}");
                        objectseparator = ',';
                    }
                    sb.Append("]");

                    reader.Close();
                    reader.Dispose();
                }

            }
            catch (Exception ex)
            {
                Dispose();
                throw ex;
            }

            return sb.ToString();
        }

        public IResultSet GetResultSet(string sql, int minrow = 0, int maxrow = 0, bool isprocedure = false, IIntwentySqlParameter[] parameters = null, IIntwentyResultColumn[] resultcolumns = null)
        {
            var res = new ResultSet();

            try
            {
                using (var command = GetCommand())
                {
                    command.CommandText = sql;
                    if (isprocedure)
                        command.CommandType = CommandType.StoredProcedure;
                    else
                        command.CommandType = CommandType.Text;

                    AddCommandParameters(parameters, command);

                    var reader = command.ExecuteReader();


                    var adjusted_columns = new List<IntwentyResultColumn>();
                    var rindex = 0;

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
                        if (adjusted_columns.Count == 0)
                            adjusted_columns = AdjustResultColumns(reader, resultcolumns);

                        var row = new ResultSetRow();
                        foreach (var rc in adjusted_columns)
                        {
                            if (reader.IsDBNull(rc.Index))
                                continue;

                            row.SetValue(rc.Name, reader.GetValue(rc.Index));

                        }
                        res.Rows.Add(row);

                    }

                    reader.Close();
                    reader.Dispose();
                }

            }
            catch (Exception ex)
            {
                Dispose();
                throw ex;
            }

            return res;
        }

        public DataTable GetDataTable(string sql, int minrow = 0, int maxrow = 0, bool isprocedure = false, IIntwentySqlParameter[] parameters = null, IIntwentyResultColumn[] resultcolumns = null)
        {
            var table = new DataTable();

            try
            {
                using (var command = GetCommand())
                {
                    command.CommandText = sql;
                    if (isprocedure)
                        command.CommandType = CommandType.StoredProcedure;
                    else
                        command.CommandType = CommandType.Text;

                    AddCommandParameters(parameters, command);

                    var reader = command.ExecuteReader();

                    table.Load(reader);

                    reader.Close();
                    reader.Dispose();

                }

            }
            catch (Exception ex)
            {
                Dispose();
                throw ex;
            }

            return table;
        }

        public void CreateTable<T>()
        {
            var info = TypeDataHandler.GetDbTableDefinition<T>();

            if (TableExists<T>())
                return;

            try
            {
                using (var command = GetCommand())
                {
                    command.CommandText = GetSqlBuilder().GetCreateTableSql(info);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }

                foreach (var index in info.Indexes)
                {
                    using (var command = GetCommand())
                    {
                        command.CommandText = GetSqlBuilder().GetCreateIndexSql(index);
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                Dispose();
                throw ex;
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
                using (var command = GetCommand())
                {
                    command.CommandText = string.Format("SELECT 1 FROM {0}", tablename);
                    command.CommandType = CommandType.Text;
                    command.ExecuteScalar();
                }
                return true;
            }
            catch
            {
                Dispose();
            }

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
            catch { Dispose();  }

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

            try
            {
                using (var command = GetCommand())
                {
                    command.CommandText = string.Format("SELECT * FROM {0} WHERE {1}=@P1", info.Name, info.PrimaryKeyColumnNamesList[0]);
                    command.CommandType = CommandType.Text;

                    AddCommandParameters(new IIntwentySqlParameter[] { new IntwentySqlParameter("@P1", id) }, command);

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

                    reader.Close();
                    reader.Dispose();
                }
            }
            catch (Exception ex)
            {
                Dispose();
                throw ex;
            }

            return res;
        }

        public virtual List<T> GetEntities<T>() where T : new()
        {
            var res = new List<T>();
            var info = TypeDataHandler.GetDbTableDefinition<T>();

            try
            {
                using (var command = GetCommand())
                {
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

                    reader.Close();
                    reader.Dispose();
                }
            }
            catch (Exception ex)
            {
                Dispose();
                throw ex;
            }

            return res;
        }

        public virtual List<T> GetEntities<T>(string sql, bool isprocedure = false, IIntwentySqlParameter[] parameters = null) where T : new()
        {
            var res = new List<T>();
            var info = TypeDataHandler.GetDbTableDefinition<T>(sql);

            try
            {
                using (var command = GetCommand())
                {
                    command.CommandText = sql;
                    if (isprocedure)
                        command.CommandType = CommandType.StoredProcedure;
                    else
                        command.CommandType = CommandType.Text;

                    AddCommandParameters(parameters, command);

                    var reader = command.ExecuteReader();

                    TypeDataHandler.AdjustColumnDefinitionToQueryResult(info, reader);

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

                    reader.Close();
                    reader.Dispose();
                }
            }
            catch (Exception ex)
            {
                Dispose();
                throw ex;
            }

            return res;
        }


        public virtual int InsertEntity<T>(T entity)
        {
            var info = TypeDataHandler.GetDbTableDefinition<T>();
            var parameters = new List<IntwentySqlParameter>();
            int res;

            try
            {
                using (var command = GetCommand())
                {
                    command.CommandText = GetSqlBuilder().GetInsertSql(info, entity, parameters);
                    command.CommandType = CommandType.Text;

                    AddCommandParameters(parameters.ToArray(), command);

                    res = command.ExecuteNonQuery();

                    InferAutoIncrementalValue(info, parameters, entity, command);

                }

              

            }
            catch (Exception ex)
            {
                Dispose();
                throw ex;
            }

            return res;

        }

        public int InsertEntity(string json, string tablename)
        {
            throw new NotImplementedException();
        }

        public int InsertEntity(JsonElement json, string tablename)
        {
            throw new NotImplementedException();
        }


        public int UpdateEntity<T>(T entity)
        {
            var info = TypeDataHandler.GetDbTableDefinition<T>();
            var parameters = new List<IntwentySqlParameter>();
            var keyparameters = new List<IntwentySqlParameter>();
            int res;

            var sql = GetSqlBuilder().GetUpdateSql(info, entity, parameters,keyparameters);
            if (keyparameters.Count == 0)
                throw new InvalidOperationException("Can't update a table without 'Primary Key' or an 'Auto Increment' column, please use annotations.");

            try
            {
                using (var command = GetCommand())
                {
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;

                    AddCommandParameters(keyparameters.ToArray(), command);
                    AddCommandParameters(parameters.ToArray(), command);

                    res = command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Dispose();
                throw ex;
            }

            return res;
        }

        public int UpdateEntity(string json, string tablename)
        {
            throw new NotImplementedException();
        }

        public int UpdateEntity(JsonElement json, string tablename)
        {
            throw new NotImplementedException();
        }

        public int DeleteEntities<T>(IEnumerable<T> entities)
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
            int res;
            var info = TypeDataHandler.GetDbTableDefinition<T>();
            var parameters = new List<IntwentySqlParameter>();

            var sql = GetSqlBuilder().GetDeleteSql(info, entity, parameters);
            if (parameters.Count == 0)
                throw new InvalidOperationException("Can't delete rows in a table without 'Primary Key' or an 'Auto Increment' column, please use annotations.");

            try
            {
                using (var command = GetCommand())
                {
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;

                    AddCommandParameters(parameters.ToArray(), command);

                    res = command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Dispose();
                throw ex;
            }

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

        protected abstract void AddCommandParameters(IIntwentySqlParameter[] parameters, IDbCommand command);
            

        protected abstract void InferAutoIncrementalValue<T>(IntwentyDbTableDefinition info, List<IntwentySqlParameter> parameters, T entity, IDbCommand command);

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
            if (resultcolumn.IsNumeric)
                return true;

            if (datatypename.ToUpper() == "REAL")
                return true;
            if (datatypename.ToUpper() == "INTEGER")
                return true;
            if (datatypename.ToUpper() == "INT")
                return true;
            if (datatypename.ToUpper().Contains("DECIMAL"))
                return true;


            return false;
        }

        protected bool IsDateTime(string datatypename, IntwentyResultColumn resultcolumn)
        {
            if (resultcolumn.IsDateTime)
                return true;

            if (datatypename.ToUpper() == "DATETIME")
                return true;
           

            return false;
        }

        private List<IntwentyResultColumn> AdjustResultColumns(IDataReader reader, IIntwentyResultColumn[] resultcolumns)
        {
            var res = new List<IntwentyResultColumn>();

            var schema = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                schema.Add(reader.GetName(i));
            }

            if (resultcolumns == null || (resultcolumns != null && resultcolumns.Count() == 0))
            {
                for (int i = 0; i < schema.Count; i++)
                {
                    var rc = new IntwentyResultColumn() { Name = schema[i], Index = i };
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
                        var rc = new IntwentyResultColumn() { Name = col.Name, Index = i  };
                        res.Add(rc);
                    }
                }
            }

         
            return res;
        }

       
      
    }
}
