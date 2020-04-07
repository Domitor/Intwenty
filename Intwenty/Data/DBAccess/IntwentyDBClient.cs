using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using MySql.Data.MySqlClient;
using Npgsql;
using NpgsqlTypes;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using Intwenty.Data.DBAccess.Helpers;
using Intwenty.Data.DBAccess.Annotations;

namespace Intwenty.Data.DBAccess
{
    public enum DBMS { MSSqlServer, MySql, MariaDB, Postgres };

    public class IntwentySqlParameter
    {
        public string ParameterName { get; set; }

        public object Value { get; set; }

        public DbType DataType { get; set; }

        public ParameterDirection Direction { get; set; }

        public IntwentySqlParameter()
        {
            Direction = ParameterDirection.Input;
            DataType = DbType.String;
        }
    }

    public class NonQueryResult
    {
        public List<IntwentySqlParameter> OutputParameters { get; set; }

        public int Value { get; set; }

        public NonQueryResult()
        {
            OutputParameters = new List<IntwentySqlParameter>();
            Value = 0;
        }
    }



    public class IntwentyDBClient : IDisposable
    {
        private SqlConnection sql_connection;
        private SqlCommand sql_cmd;

        private NpgsqlConnection pgres_connection;
        private NpgsqlCommand pgres_cmd;

        private MySqlConnection mysql_connection;
        private MySqlCommand mysql_cmd;

        private int DBTYPE = -1;

        private string ConnStr = "";


        public IntwentyDBClient()
        {
        }

        public IntwentyDBClient(DBMS d, string connectionstring)
        {
            if (d== DBMS.MSSqlServer)
                DBTYPE = 1;
            if (d == DBMS.MySql || d == DBMS.MariaDB)
                DBTYPE = 4;
            if (d == DBMS.Postgres)
                DBTYPE = 3;
           

            ConnStr = connectionstring;
        }

        public DBMS GetDBMS()
        {
            if (DBTYPE == 1)
                return DBMS.MSSqlServer;
            if (DBTYPE == 3)
                return DBMS.Postgres;
            if (DBTYPE == 4)
                return DBMS.MySql;

            return DBMS.MSSqlServer;
        }

      

        public void Dispose()
        {

            if (DBTYPE == 1)
            {
                if (this.sql_connection != null)
                {
                    if (this.sql_connection.State != ConnectionState.Closed)
                    {
                        this.sql_connection.Dispose();
                    }
                }

                this.sql_cmd = null;
            }
            else if (DBTYPE == 3)
            {
                if (this.pgres_connection != null)
                {
                    if (this.pgres_connection.State != ConnectionState.Closed)
                    {
                        this.pgres_connection.Dispose();
                    }

                }

                this.pgres_cmd = null;
            }

            else if (DBTYPE == 4)
            {
                if (this.mysql_connection != null)
                {
                    if (this.mysql_connection.State != ConnectionState.Closed)
                    {
                        this.mysql_connection.Dispose();
                    }

                }

                this.mysql_cmd = null;
            }
        }



        public virtual void Open()
        {
            if (DBTYPE == 1)
            {

                sql_connection = new SqlConnection();
                sql_connection.ConnectionString = this.ConnStr;
                sql_connection.Open();
            }
            else if (DBTYPE == 3)
            {

                pgres_connection = new NpgsqlConnection();
                pgres_connection.ConnectionString = this.ConnStr;
                pgres_connection.Open();
            }
            else if (DBTYPE == 4)
            {

                mysql_connection = new MySqlConnection();
                mysql_connection.ConnectionString = this.ConnStr;
                mysql_connection.Open();
            }
        }

        private void OpenIfNeeded()
        {
            if (DBTYPE == 1)
            {
                if (sql_connection != null && sql_connection.State == ConnectionState.Open)
                    return;

                sql_connection = new SqlConnection();
                sql_connection.ConnectionString = this.ConnStr;
                sql_connection.Open();
            }
            else if (DBTYPE == 3)
            {
                if (pgres_connection != null && pgres_connection.State == ConnectionState.Open)
                    return;

                pgres_connection = new NpgsqlConnection();
                pgres_connection.ConnectionString = this.ConnStr;
                pgres_connection.Open();
            }
            else if (DBTYPE == 4)
            {
                if (mysql_connection != null && mysql_connection.State == ConnectionState.Open)
                    return;

                mysql_connection = new MySqlConnection();
                mysql_connection.ConnectionString = this.ConnStr;
                mysql_connection.Open();
            }
        }



        public void Close()
        {
            if (DBTYPE == 1)
            {
                if (sql_connection != null)
                {
                    if (this.sql_connection.State != ConnectionState.Closed)
                    {
                        this.sql_connection.Close();
                    }
                }

            }
            else if (DBTYPE == 3)
            {
                if (pgres_connection != null)
                {
                    if (this.pgres_connection.State != ConnectionState.Closed)
                    {
                        this.pgres_connection.Close();
                    }
                }

            }
            else if (DBTYPE == 4)
            {
                if (mysql_connection != null)
                {
                    if (this.mysql_connection.State != ConnectionState.Closed)
                    {
                        this.mysql_connection.Close();
                    }
                }

            }
        }




        public virtual void CreateCommand(string sqlcode)
        {
            this.MakeCommand(sqlcode, false);
        }

        public void CreateSPCommand(string procname)
        {
            this.MakeCommand(procname, true);
        }

       

      

        public void AddParameter(string name, object value)
        {
            if (DBTYPE == 1)
            {
                this.sql_cmd.Parameters.AddWithValue(name, value);
            }
            else if (DBTYPE == 3)
            {
                this.pgres_cmd.Parameters.Add(new NpgsqlParameter() { Value = value, ParameterName = name });
            }
            else if (DBTYPE == 4)
            {
                this.mysql_cmd.Parameters.Add(new MySqlParameter() { Value = value, ParameterName = name });
            }

        }

        public void AddParameter(IntwentySqlParameter p)
        {
            if (DBTYPE == 1)
            {
                var param = new SqlParameter() { ParameterName = p.ParameterName, Value = p.Value, Direction = p.Direction };
                if (param.Direction == ParameterDirection.Output)
                    param.DbType = p.DataType;

                this.sql_cmd.Parameters.Add(param);
            }
            else if (DBTYPE == 3)
            {
                var param = new NpgsqlParameter() { ParameterName = p.ParameterName, Value = p.Value, Direction = p.Direction };
                if (param.Direction == ParameterDirection.Output)
                    param.DbType = p.DataType;

                this.pgres_cmd.Parameters.Add(param);
            }
            else if (DBTYPE == 4)
            {
                var param = new MySqlParameter() { ParameterName = p.ParameterName, Value = p.Value, Direction = p.Direction };
                if (param.Direction == ParameterDirection.Output)
                    param.DbType = p.DataType;

                this.mysql_cmd.Parameters.Add(param);
            }

        }



        public void SetSelectCommandTimeout(Int32 seconds)
        {
            if (DBTYPE == 1)
            {
                this.sql_cmd.CommandTimeout = seconds;
            }
            else if (DBTYPE == 3)
            {
                this.pgres_cmd.CommandTimeout = seconds;
            }
            else if (DBTYPE == 4)
            {
                this.mysql_cmd.CommandTimeout = seconds;
            }
        }


        public void FillDataset(DataSet ds, string tablename)
        {


            if (DBTYPE == 1)
            {
                var adapt = new SqlDataAdapter(sql_cmd);
                adapt.MissingMappingAction = MissingMappingAction.Passthrough;
                adapt.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                adapt.Fill(ds, tablename);
            }
            else if (DBTYPE == 3)
            {
                var adapt = new NpgsqlDataAdapter(pgres_cmd);
                adapt.MissingMappingAction = MissingMappingAction.Passthrough;
                adapt.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                adapt.Fill(ds, tablename);
            }
            else if (DBTYPE == 4)
            {
                var adapt = new MySqlDataAdapter(mysql_cmd);
                adapt.MissingMappingAction = MissingMappingAction.Passthrough;
                adapt.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                adapt.Fill(ds, tablename);
            }

        }

      

        public StringBuilder GetAsJSONArray(int minrow=0, int maxrow=0)
        {
            var sb = new StringBuilder();
            var ds = GetDataSet();

            var firstcol = true;
            var firstrow = true;
            var rindex = -1;
            sb.Append("[");
            foreach (DataRow r in ds.Tables[0].Rows)
            {

                rindex += 1;
                if (maxrow > minrow && (minrow > 0 || maxrow > 0))
                {
                    if (!(minrow <= rindex && maxrow > rindex))
                        continue;
                }

                if (firstrow)
                {
                    firstrow = false;
                    sb.Append("{");
                }
                else
                {
                    sb.Append(",{");
                }

                firstcol = true;
                foreach (DataColumn dc in ds.Tables[0].Columns)
                {
                    var val = DBHelpers.GetJSONValue(r, dc);
                    if (string.IsNullOrEmpty(val))
                        continue;

                    if (firstcol)
                    {
                        firstcol = false;
                        sb.Append(val);
                    }
                    else
                    {
                        sb.Append("," + val);
                    }
                }

                sb.Append("}");

            }

            sb.Append("]");

            return sb;
        }

        public StringBuilder GetAsJSONObject()
        {
            var sb = new StringBuilder();
            var ds = GetDataSet();

            if (ds.Tables[0].Rows.Count == 0)
            {
                sb.Append("{}");
                return sb;
            }

            var firstcol = true;

            sb.Append("{");

            foreach (DataColumn dc in ds.Tables[0].Columns)
            {
                var val = DBHelpers.GetJSONValue(ds.Tables[0].Rows[0], dc);
                if (string.IsNullOrEmpty(val))
                    continue;

                if (firstcol)
                {
                    sb.Append(val);
                    firstcol = false;
                }
                else
                {
                    sb.Append("," + val);
                }
            }

            sb.Append("}");

            return sb;


        }

        public void CreateTable<T>(bool checkexisting=false)
        {
            OpenIfNeeded();

            var colsep = "";
            var workingtype = typeof(T);
            var tablename = workingtype.Name;

            //TABLENAME
            var annot_tablename = workingtype.GetCustomAttributes(typeof(DbTableName), false);
            if (annot_tablename != null && annot_tablename.Length > 0)
                tablename = ((DbTableName)annot_tablename[0]).Name;

            if (checkexisting)
            {
                var exists = false;
                try
                {
                    CreateCommand(string.Format("select 1 from {0}", tablename));
                    ExecuteScalarQuery();
                }
                catch { exists = true; }
                if (!exists)
                    return;

            }

            //PK
            DbTablePrimaryKey pk = null;
            var annot_pk = workingtype.GetCustomAttributes(typeof(DbTablePrimaryKey), false);
            if (annot_pk != null && annot_pk.Length > 0)
                pk = ((DbTablePrimaryKey)annot_pk[0]);

            var memberproperties = workingtype.GetProperties();
            if (memberproperties.Length == 0)
                throw new InvalidOperationException("Can't create table from a class without properties");

            var sb = new StringBuilder();
            sb.Append("CREATE TABLE " + tablename + "(");
            foreach (var m in memberproperties)
            {
                var colname = m.Name;
                var annot_colname = m.GetCustomAttributes(typeof(DbColumnName), false);
                if (annot_colname != null && annot_colname.Length > 0)
                    colname = ((DbColumnName)annot_colname[0]).Name;

                var autoinc = false;
                var annot_autoinc = m.GetCustomAttributes(typeof(AutoIncrement), false);
                if (annot_autoinc != null && annot_autoinc.Length > 0)
                    autoinc = true;

                var notnull = false;
                var annot_notnull = m.GetCustomAttributes(typeof(NotNull), false);
                if (annot_notnull != null && annot_notnull.Length > 0)
                    notnull = true;
                if (pk != null && !string.IsNullOrEmpty(pk.Columns))
                {
                    var keycols = pk.Columns.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    foreach (var c in keycols)
                    {
                        if (c == colname)
                            notnull = true;
                    }

                }

                sb.Append(colsep + GetColumnDefinition(colname,m.PropertyType.ToString(),autoinc,notnull));
                colsep = ", ";
            }
            if (pk != null && DBTYPE == 4)
            {
                sb.Append(colsep + string.Format("PRIMARY KEY (`{0}`)", pk.Columns));
            }
            if (pk != null && DBTYPE == 1)
            {
                sb.Append(colsep + string.Format("CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED ([{1}] ASC)", tablename, pk.Columns));
            }
            sb.Append(")");

            CreateCommand(sb.ToString());
            ExecuteNonQuery();

            //INDEX
            var annot_index = workingtype.GetCustomAttributes(typeof(DbTableIndex), false);
            if (annot_index != null && annot_index.Length > 0)
            {
                var indexbuilder = "";
                foreach (var ai in annot_index)
                {
                    var index = (DbTableIndex)ai;
                    if (index.IsUnique)
                        indexbuilder = string.Format("CREATE UNIQUE INDEX {0} ON {1} ({2})", new object[] { index.Name, tablename, index.Columns });
                    if (!index.IsUnique)
                        indexbuilder = string.Format("CREATE INDEX {0} ON {1} ({2})", new object[] { index.Name, tablename, index.Columns });

                    if (!string.IsNullOrEmpty(indexbuilder))
                    {
                        CreateCommand(indexbuilder);
                        ExecuteNonQuery();
                    }

                }
            }

            Close();

        }

        public List<T> Get<T>() where T : new()
        {
          

            var res = new List<T>();

            var t = typeof(T);
            var tname = t.Name;
            var annot_tablename = t.GetCustomAttributes(typeof(DbTableName), false);
            if (annot_tablename != null && annot_tablename.Length > 0)
                tname = ((DbTableName)annot_tablename[0]).Name;

            var ds = new DataSet();
            OpenIfNeeded();
            CreateCommand(string.Format("SELECT * FROM {0}", tname));
            FillDataset(ds, "NONE");
            Close();

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                var m = new T();
                var memberproperties = t.GetProperties();
                foreach (var property in memberproperties)
                {
                    var colname = property.Name;
                    var annot_colname = property.GetCustomAttributes(typeof(DbColumnName), false);
                    if (annot_colname != null && annot_colname.Length > 0)
                        colname = ((DbColumnName)annot_colname[0]).Name;

                    if (r[colname] == DBNull.Value)
                        continue;

                    property.SetValue(m, r[colname], null);

                }
               
                res.Add(m);
            }

            return res;
        }

        public int Insert<T>(T model)
        {
            var autoinccolumn = "";
            var colsep = "";
            var workingtype = typeof(T);
            var tname = workingtype.Name;
            var annot_tablename = workingtype.GetCustomAttributes(typeof(DbTableName), false);
            if (annot_tablename != null && annot_tablename.Length > 0)
                tname = ((DbTableName)annot_tablename[0]).Name;

            var query = new StringBuilder(string.Format("INSERT INTO {0} (", tname));
            var values = new StringBuilder(" VALUES (");
            var parameters = new List<IntwentySqlParameter>();

            var memberproperties = workingtype.GetProperties();
            if (memberproperties.Length == 0)
                throw new InvalidOperationException("Can't insert to a table based on a class without properties");

            foreach (var m in memberproperties)
            {
                var colname = m.Name;
                var annot_colname = m.GetCustomAttributes(typeof(DbColumnName), false);
                if (annot_colname != null && annot_colname.Length > 0)
                    colname = ((DbColumnName)annot_colname[0]).Name;

                var annot_autoinc = m.GetCustomAttributes(typeof(AutoIncrement), false);
                if (annot_autoinc != null && annot_autoinc.Length > 0)
                {
                    autoinccolumn = colname;
                    continue;
                }
              
                var value = m.GetValue(model);
                var prm = new IntwentySqlParameter();
                prm.ParameterName = "@" + colname;

                if (value == null)
                    prm.Value = "NULL";
                else
                    prm.Value = value;

                query.Append(colsep+colname);
                values.Append(colsep + "@" + colname);
                parameters.Add(prm);
                colsep = ", ";
            }
            query.Append(") ");
            values.Append(")");

            if (!string.IsNullOrEmpty(autoinccolumn) && DBTYPE == 1)
            {
                values.Append(" select @NewId= Scope_Identity()");
                parameters.Add(new IntwentySqlParameter() { ParameterName = "@NewId", Direction = ParameterDirection.Output, DataType = DbType.Int32 });
            }

            OpenIfNeeded();
            CreateCommand(query.ToString() + values.ToString());
            Close();
            foreach (var p in parameters)
                AddParameter(p);

            var res = ExecuteNonQuery();
            var output = res.OutputParameters.Find(p => p.ParameterName == "@NewId");
            if (output != null && !string.IsNullOrEmpty(autoinccolumn))
            {
                var property = workingtype.GetProperty(autoinccolumn);
                if (property != null)
                    property.SetValue(model, output.Value, null);
            }


            return res.Value; 
        }

        public int Update<T>(T model)
        {

            var colsep = "";
            var t = typeof(T);
            var tname = t.Name;
            var annot_tablename = t.GetCustomAttributes(typeof(DbTableName), false);
            if (annot_tablename != null && annot_tablename.Length > 0)
                tname = ((DbTableName)annot_tablename[0]).Name;

            DbTablePrimaryKey pk = null;
            var annot_pk = t.GetCustomAttributes(typeof(DbTablePrimaryKey), false);
            if (annot_pk != null && annot_pk.Length > 0)
                pk = ((DbTablePrimaryKey)annot_pk[0]);

            var query = new StringBuilder(string.Format("UPDATE {0} SET ", tname));
            var parameters = new List<IntwentySqlParameter>();
            var keyparameters = new List<IntwentySqlParameter>();

            var memberproperties = t.GetProperties();
            foreach (var m in memberproperties)
            {
                var colname = m.Name;
                var annot_colname = m.GetCustomAttributes(typeof(DbColumnName), false);
                if (annot_colname != null && annot_colname.Length > 0)
                    colname = ((DbColumnName)annot_colname[0]).Name;

                var value = m.GetValue(model);

                var annot_autoinc = m.GetCustomAttributes(typeof(AutoIncrement), false);
                if (annot_autoinc != null && annot_autoinc.Length > 0)
                {
                    if (!keyparameters.Exists(p => p.ParameterName == colname) && value!=null)
                    {
                        var keyprm = new IntwentySqlParameter() { ParameterName = colname, Value = value };
                        keyparameters.Add(keyprm);
                    }
                }
                if (pk != null && !string.IsNullOrEmpty(pk.Columns))
                {
                    var keycols = pk.Columns.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    foreach (var c in keycols)
                    {
                        if (c != colname)
                            continue;

                        if (!keyparameters.Exists(p => p.ParameterName == colname) && value != null)
                        {
                            var keyprm = new IntwentySqlParameter() { ParameterName = colname, Value = value };
                            keyparameters.Add(keyprm);
                        }
                    }

                }

                if (keyparameters.Exists(p => p.ParameterName == colname))
                    continue;

                var prm = new IntwentySqlParameter();
                prm.ParameterName = "@" + colname;
                if (value == null)
                    prm.Value = "NULL";
                else
                    prm.Value = value;

                query.Append(colsep + colname + "=@"+colname);
                parameters.Add(prm);
                colsep = ", ";
            }

            if (keyparameters.Count == 0)
                throw new InvalidOperationException("Can't update a table without 'Primary Key' or an 'Auto Increment' column, use annotations.");

            query.Append(" WHERE ");
            var wheresep = "";
            foreach (var p in keyparameters)
            {
                query.Append(wheresep + p.ParameterName+"=@"+p.ParameterName);
                wheresep = " AND ";
            }

            OpenIfNeeded();
            CreateCommand(query.ToString());
            foreach (var p in keyparameters)
                AddParameter(p);
            foreach (var p in parameters)
                AddParameter(p);

            var res = ExecuteNonQuery().Value;
            Close();

            return res;

        }

        public int DeleteRange<T>(IEnumerable<T> model)
        {
            var result = 0;
            foreach (var t in model)
            {
                result+=Delete(t);
            }
            return result;
        }

        public int Delete<T>(T model)
        {
           
            var t = typeof(T);
            var tname = t.Name;
            var annot_tablename = t.GetCustomAttributes(typeof(DbTableName), false);
            if (annot_tablename != null && annot_tablename.Length > 0)
                tname = ((DbTableName)annot_tablename[0]).Name;

            DbTablePrimaryKey pk = null;
            var annot_pk = t.GetCustomAttributes(typeof(DbTablePrimaryKey), false);
            if (annot_pk != null && annot_pk.Length > 0)
                pk = ((DbTablePrimaryKey)annot_pk[0]);

            var query = new StringBuilder(string.Format("DELETE FROM {0} ", tname));
            var keyparameters = new List<IntwentySqlParameter>();

            var memberproperties = t.GetProperties();
            foreach (var m in memberproperties)
            {
                var colname = m.Name;
                var annot_colname = m.GetCustomAttributes(typeof(DbColumnName), false);
                if (annot_colname != null && annot_colname.Length > 0)
                    colname = ((DbColumnName)annot_colname[0]).Name;

                var value = m.GetValue(model);

                var annot_autoinc = m.GetCustomAttributes(typeof(AutoIncrement), false);
                if (annot_autoinc != null && annot_autoinc.Length > 0)
                {
                    if (!keyparameters.Exists(p => p.ParameterName == colname) && value != null)
                    {
                        var keyprm = new IntwentySqlParameter() { ParameterName = colname, Value = value };
                        keyparameters.Add(keyprm);
                    }
                }
                if (pk != null && !string.IsNullOrEmpty(pk.Columns))
                {
                    var keycols = pk.Columns.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    foreach (var c in keycols)
                    {
                        if (c != colname)
                            continue;

                        if (!keyparameters.Exists(p => p.ParameterName == colname) && value != null)
                        {
                            var keyprm = new IntwentySqlParameter() { ParameterName = colname, Value = value };
                            keyparameters.Add(keyprm);
                        }
                    }

                }
            }

            if (keyparameters.Count == 0)
                throw new InvalidOperationException("Can't delete from a table without 'Primary Key' or an 'Auto Increment' column, use annotations.");

            query.Append(" WHERE ");
            var wheresep = "";
            foreach (var p in keyparameters)
            {
                query.Append(wheresep + p.ParameterName + "=@" + p.ParameterName);
                wheresep = " AND ";
            }
            OpenIfNeeded();
            CreateCommand(query.ToString());
            foreach (var p in keyparameters)
                AddParameter(p);

            var res = ExecuteNonQuery().Value;
            Close();

            return res;
        }



        private DataSet GetDataSet()
        {
            var ds = new DataSet();

            if (DBTYPE == 1)
            {
                var adapt = new SqlDataAdapter(sql_cmd);
                adapt.FillSchema(ds, SchemaType.Mapped);
                adapt.Fill(ds);
            }
            else if (DBTYPE == 3)
            {
                var adapt = new NpgsqlDataAdapter(pgres_cmd);
                adapt.FillSchema(ds, SchemaType.Mapped);
                adapt.Fill(ds);
            }
            else if (DBTYPE == 4)
            {
                var adapt = new MySqlDataAdapter(mysql_cmd);
                adapt.FillSchema(ds, SchemaType.Mapped);
                adapt.Fill(ds);
            }

            return ds;

        }





        public object ExecuteScalarQuery()
        {

            if (DBTYPE == 1)
            {
                return sql_cmd.ExecuteScalar();
            }
            else if (DBTYPE == 3)
            {
                return pgres_cmd.ExecuteScalar();
            }
            else if (DBTYPE == 4)
            {
                return mysql_cmd.ExecuteScalar();
            }
            else
            {
                return sql_cmd.ExecuteScalar();
            }


        }

        public NonQueryResult ExecuteNonQuery()
        {

            if (DBTYPE == 1)
            {
                var res = new NonQueryResult();
                res.Value = sql_cmd.ExecuteNonQuery();
                foreach (SqlParameter p in sql_cmd.Parameters)
                {
                    if (p.Direction == ParameterDirection.Output)
                        res.OutputParameters.Add(new IntwentySqlParameter() { ParameterName = p.ParameterName, Direction = ParameterDirection.Output, DataType = p.DbType, Value = p.Value });
                }

                return res;
            }
            else if (DBTYPE == 3)
            {
                var res = new NonQueryResult();
                res.Value = pgres_cmd.ExecuteNonQuery();
                foreach (NpgsqlParameter p in pgres_cmd.Parameters)
                {
                    if (p.Direction == ParameterDirection.Output)
                        res.OutputParameters.Add(new IntwentySqlParameter() { ParameterName = p.ParameterName, Direction = ParameterDirection.Output, DataType = p.DbType, Value = p.Value });
                }

                return res;
            }
            else if (DBTYPE == 4)
            {
                var res = new NonQueryResult();
                res.Value = mysql_cmd.ExecuteNonQuery();
                foreach (MySqlParameter p in mysql_cmd.Parameters)
                {
                    if (p.Direction == ParameterDirection.Output)
                        res.OutputParameters.Add(new IntwentySqlParameter() { ParameterName = p.ParameterName, Direction = ParameterDirection.Output, DataType = p.DbType, Value = p.Value });
                }

                return res;
            }
            else
            {
                throw new NotImplementedException();
            }

        }

        public SqlDataReader ExecuteSqlServerDataReader(CommandBehavior cbv)
        {
            if (DBTYPE == 1)
            {
                return sql_cmd.ExecuteReader(cbv);
            }
            else
            {
                return null;
            }
        }


        public NpgsqlDataReader ExecutePgSqlDataReader(CommandBehavior cbv)
        {
            if (DBTYPE == 3)
            {
                return pgres_cmd.ExecuteReader(cbv);
            }
            else
            {
                return null;
            }
        }

        public MySqlDataReader ExecuteMySqlDataReader(CommandBehavior cbv)
        {
            if (DBTYPE == 4)
            {
                return mysql_cmd.ExecuteReader(cbv);
            }
            else
            {
                return null;
            }
        }



        private void MakeCommand(string sqlcode, bool isstoredprocedure)
        {


            if (DBTYPE == 1)
            {
                if (this.sql_cmd == null)
                {
                    this.sql_cmd = new SqlCommand();
                }
                else
                {
                    this.sql_cmd.Dispose();
                    this.sql_cmd = new SqlCommand();
                }
                this.sql_cmd.Connection = this.sql_connection;
                this.sql_cmd.CommandText = sqlcode;
                if (isstoredprocedure)
                {
                    this.sql_cmd.CommandType = CommandType.StoredProcedure;
                }


            }
            else if (DBTYPE == 3)
            {
                if (this.pgres_cmd == null)
                {
                    this.pgres_cmd = new NpgsqlCommand();
                }
                else
                {
                    this.pgres_cmd.Dispose();
                    this.pgres_cmd = new NpgsqlCommand();
                }
                this.pgres_cmd.Connection = this.pgres_connection;
                this.pgres_cmd.CommandText = sqlcode;
                if (isstoredprocedure)
                {
                    this.pgres_cmd.CommandType = CommandType.StoredProcedure;
                }

            }
            else if (DBTYPE == 4)
            {
                if (this.mysql_cmd == null)
                {
                    this.mysql_cmd = new MySqlCommand();
                }
                else
                {
                    this.mysql_cmd.Dispose();
                    this.mysql_cmd = new MySqlCommand();
                }
                this.mysql_cmd.Connection = this.mysql_connection;
                this.mysql_cmd.CommandText = sqlcode;
                if (isstoredprocedure)
                {
                    this.mysql_cmd.CommandType = CommandType.StoredProcedure;
                }

            }
        }


     
      


        private string GetColumnDefinition(string name, string nettype, bool autoincrement=false, bool notnull=false, bool longtext = false)
        {
            var result = string.Empty;
            var defaultvalue = "DEFAULT NULL";
            var allownullvalue = "NULL";

            var autoincvalue = string.Empty;
            var datatype = string.Empty;
            if (nettype.ToUpper() == "SYSTEM.STRING" && DBTYPE == 1)  datatype = "NVARCHAR(300)";
            if (nettype.ToUpper() == "SYSTEM.STRING" && DBTYPE == 4)  datatype = "VARCHAR(300)";
            if (nettype.ToUpper() == "SYSTEM.STRING" && DBTYPE == 1 && longtext) datatype = "NVARCHAR(MAX)";
            if (nettype.ToUpper() == "SYSTEM.STRING" && DBTYPE == 4 && longtext) datatype = "LONGTEXT";
            if (nettype.ToUpper() == "SYSTEM.INT32" && DBTYPE == 1)   datatype = "INT";
            if (nettype.ToUpper() == "SYSTEM.INT32" && DBTYPE == 4)   datatype = "INT(11)";
            if (nettype.ToUpper() == "SYSTEM.BOOLEAN" && DBTYPE == 1) datatype = "BIT";
            if (nettype.ToUpper() == "SYSTEM.BOOLEAN" && DBTYPE == 4) datatype = "TINYINT(1)";

            if (autoincrement && DBTYPE == 1)
            {
                allownullvalue = "NOT NULL";
                defaultvalue = "";
                if (DBTYPE == 1)
                    autoincvalue = "IDENTITY(1,1)";
                if (DBTYPE == 4)
                    autoincvalue = "AUTO_INCREMENT";
            }
            if (notnull)
            {
                allownullvalue = "NOT NULL";
                defaultvalue = "";
            }




            if (DBTYPE == 1) result = string.Format("{0} {1} {2} {3}", new object[] { name, datatype, autoincvalue, allownullvalue });
            if (DBTYPE == 4) result = string.Format("`{0}` {1} {2} {3} {4}", new object[] { name, datatype, allownullvalue, autoincvalue, defaultvalue });

            if (string.IsNullOrEmpty(result))
                throw new InvalidOperationException("Could not generate sql column definition");

            return result;
        }





    }
}
