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
using Intwenty.Model;
using System.Data.SQLite;
using System.Data.Common;

namespace Intwenty.Data.DBAccess
{
   


    public class IntwentySqlDbClient : IDisposable, IIntwentyDbSql
    {
        private SqlConnection sql_connection;
        private SqlCommand sql_cmd;

        private NpgsqlConnection pgres_connection;
        private NpgsqlCommand pgres_cmd;

        private MySqlConnection mysql_connection;
        private MySqlCommand mysql_cmd;

        private SQLiteConnection sqlite_connection;
        private SQLiteCommand sqlite_cmd;

        private DBMS DBMSType { get; set; }

        private string ConnectionString { get; set; }


        public IntwentySqlDbClient()
        {
            ConnectionString = string.Empty;
            DBMSType = DBMS.MSSqlServer;
        }

        public IntwentySqlDbClient(DBMS d, string connectionstring)
        {
            DBMSType = d;
            ConnectionString = connectionstring;
            if (DBMSType == DBMS.MongoDb)
                throw new InvalidOperationException("IntwentySqlDbClient configured with wrong DBMS setting");
        }

        public DBMS DbEngine
        {
            get { return DBMSType; }
        }

        public bool IsNoSql
        {
            get { return (DBMSType == DBMS.MongoDb); }
        }

        public void Dispose()
        {

            if (DBMSType ==  DBMS.MSSqlServer)
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
            else if (DBMSType ==  DBMS.PostgreSQL)
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
            else if (DBMSType ==  DBMS.MariaDB || DBMSType == DBMS.MySql)
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
            else if (DBMSType == DBMS.SQLite)
            {
                if (this.sqlite_connection != null)
                {
                    if (this.sqlite_connection.State != ConnectionState.Closed)
                    {
                        this.sqlite_connection.Dispose();
                    }

                }

                this.sqlite_cmd = null;
            }
        }



        public virtual void Open()
        {
            if (DBMSType == DBMS.MSSqlServer)
            {

                sql_connection = new SqlConnection();
                sql_connection.ConnectionString = ConnectionString;
                sql_connection.Open();
            }
            else if (DBMSType == DBMS.PostgreSQL)
            {

                pgres_connection = new NpgsqlConnection();
                pgres_connection.ConnectionString = ConnectionString;
                pgres_connection.Open();
            }
            else if (DBMSType == DBMS.MariaDB || DBMSType == DBMS.MySql)
            {

                mysql_connection = new MySqlConnection();
                mysql_connection.ConnectionString = ConnectionString;
                mysql_connection.Open();
            }
            else if (DBMSType == DBMS.SQLite)
            {

                sqlite_connection = new SQLiteConnection();
                sqlite_connection.ConnectionString = ConnectionString;
                sqlite_connection.Open();
            }
        }

        private void OpenIfNeeded()
        {
            if (DBMSType == DBMS.MSSqlServer)
            {
                if (sql_connection != null && sql_connection.State == ConnectionState.Open)
                    return;

                sql_connection = new SqlConnection();
                sql_connection.ConnectionString = this.ConnectionString;
                sql_connection.Open();
            }
            else if (DBMSType == DBMS.PostgreSQL)
            {
                if (pgres_connection != null && pgres_connection.State == ConnectionState.Open)
                    return;

                pgres_connection = new NpgsqlConnection();
                pgres_connection.ConnectionString = this.ConnectionString;
                pgres_connection.Open();
            }
            else if (DBMSType == DBMS.MariaDB || DBMSType == DBMS.MySql)
            {
                if (mysql_connection != null && mysql_connection.State == ConnectionState.Open)
                    return;

                mysql_connection = new MySqlConnection();
                mysql_connection.ConnectionString = this.ConnectionString;
                mysql_connection.Open();
            }
            else if (DBMSType == DBMS.SQLite)
            {
                if (sqlite_connection != null && sqlite_connection.State == ConnectionState.Open)
                    return;

                sqlite_connection = new SQLiteConnection();
                sqlite_connection.ConnectionString = this.ConnectionString;
                sqlite_connection.Open();
            }
        }



        public void Close()
        {
            if (DBMSType == DBMS.MSSqlServer)
            {
                if (sql_connection != null)
                {
                    if (this.sql_connection.State != ConnectionState.Closed)
                    {
                        this.sql_connection.Close();
                    }
                }

            }
            else if (DBMSType == DBMS.PostgreSQL)
            {
                if (pgres_connection != null)
                {
                    if (this.pgres_connection.State != ConnectionState.Closed)
                    {
                        this.pgres_connection.Close();
                    }
                }

            }
            else if (DBMSType == DBMS.MariaDB || DBMSType == DBMS.MySql)
            {
                if (mysql_connection != null)
                {
                    if (this.mysql_connection.State != ConnectionState.Closed)
                    {
                        this.mysql_connection.Close();
                    }
                }

            }
            else if (DBMSType == DBMS.SQLite)
            {
                if (sqlite_connection != null)
                {
                    if (this.sqlite_connection.State != ConnectionState.Closed)
                    {
                        this.sqlite_connection.Close();
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
            if (DBMSType == DBMS.MSSqlServer)
            {
                this.sql_cmd.Parameters.AddWithValue(name, value);
            }
            else if (DBMSType == DBMS.PostgreSQL)
            {
                this.pgres_cmd.Parameters.Add(new NpgsqlParameter() { Value = value, ParameterName = name });
            }
            else if (DBMSType == DBMS.MariaDB || DBMSType == DBMS.MySql)
            {
                this.mysql_cmd.Parameters.Add(new MySqlParameter() { Value = value, ParameterName = name });
            }
            else if (DBMSType == DBMS.SQLite)
            {
                this.sqlite_cmd.Parameters.Add(new SQLiteParameter() { Value = value, ParameterName = name });
            }
        }

        public void AddParameter(IntwentySqlParameter p)
        {
            if (DBMSType == DBMS.MSSqlServer)
            {
                var param = new SqlParameter() { ParameterName = p.ParameterName, Value = p.Value, Direction = p.Direction };
                if (param.Direction == ParameterDirection.Output)
                    param.DbType = p.DataType;

                this.sql_cmd.Parameters.Add(param);
            }
            else if (DBMSType == DBMS.PostgreSQL)
            {
                var param = new NpgsqlParameter() { ParameterName = p.ParameterName, Value = p.Value, Direction = p.Direction };
                if (param.Direction == ParameterDirection.Output)
                    param.DbType = p.DataType;

                this.pgres_cmd.Parameters.Add(param);
            }
            else if (DBMSType == DBMS.MariaDB || DBMSType == DBMS.MySql)
            {
                var param = new MySqlParameter() { ParameterName = p.ParameterName, Value = p.Value, Direction = p.Direction };
                if (param.Direction == ParameterDirection.Output)
                    param.DbType = p.DataType;

                this.mysql_cmd.Parameters.Add(param);
            }
            else if (DBMSType == DBMS.SQLite)
            {
                var param = new SQLiteParameter() { ParameterName = p.ParameterName, Value = p.Value, Direction = p.Direction };
                if (param.Direction == ParameterDirection.Output)
                    param.DbType = p.DataType;

                this.sqlite_cmd.Parameters.Add(param);
            }
        }



        public void SetSelectCommandTimeout(Int32 seconds)
        {
            if (DBMSType == DBMS.MSSqlServer)
                this.sql_cmd.CommandTimeout = seconds;
            else if (DBMSType == DBMS.PostgreSQL)
                this.pgres_cmd.CommandTimeout = seconds;
            else if (DBMSType == DBMS.MariaDB || DBMSType == DBMS.MySql)
                this.mysql_cmd.CommandTimeout = seconds;
            else if (DBMSType == DBMS.SQLite)
                this.sqlite_cmd.CommandTimeout = seconds;

        }


        public void FillDataset(DataSet ds, string tablename)
        {
            ds.CaseSensitive = false;

            if (DBMSType == DBMS.MSSqlServer)
            {
                var adapt = new SqlDataAdapter(sql_cmd);
                adapt.MissingMappingAction = MissingMappingAction.Passthrough;
                adapt.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                adapt.Fill(ds, tablename);
            }
            else if (DBMSType == DBMS.PostgreSQL)
            {
                var adapt = new NpgsqlDataAdapter(pgres_cmd);
                adapt.MissingMappingAction = MissingMappingAction.Passthrough;
                adapt.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                adapt.Fill(ds, tablename);
            }
            else if (DBMSType == DBMS.MariaDB || DBMSType == DBMS.MySql)
            {
                var adapt = new MySqlDataAdapter(mysql_cmd);
                adapt.MissingMappingAction = MissingMappingAction.Passthrough;
                adapt.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                adapt.Fill(ds, tablename);
            }
            else if (DBMSType == DBMS.SQLite)
            {
                var adapt = new SQLiteDataAdapter(sqlite_cmd);
                adapt.MissingMappingAction = MissingMappingAction.Passthrough;
                adapt.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                adapt.Fill(ds, tablename);
            }

        }

      

        public StringBuilder GetAsJSONArray(int minrow=0, int maxrow=0)
        {
            var sb = new StringBuilder();

            var ds = new DataSet();
            FillDataset(ds, "NONE");

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

                var firstcol = true;
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

        public StringBuilder GetAsJSONArray(List<IIntwentyDataColum> columns, int minrow = 0, int maxrow = 0)
        {
            var sb = new StringBuilder();
            var ds = new DataSet();
            FillDataset(ds, "NONE");


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

                var firstcol = true;
                foreach (DataColumn dc in ds.Tables[0].Columns)
                {
                    if (!columns.Exists(p=> p.ColumnName.ToLower() == dc.ColumnName.ToLower()))
                        continue;

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
            var ds = new DataSet();
            FillDataset(ds, "NONE");

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

        public StringBuilder GetAsJSONObject(List<IIntwentyDataColum> columns)
        {
            if (columns == null)
                throw new InvalidOperationException("Parameter columns can't be null");

            var sb = new StringBuilder();
            var ds = new DataSet();
            FillDataset(ds, "NONE");

            if (ds.Tables[0].Rows.Count == 0)
            {
                sb.Append("{}");
                return sb;
            }

            var firstcol = true;

            sb.Append("{");

            foreach (DataColumn dc in ds.Tables[0].Columns)
            {
                if (!columns.Exists(p => p.ColumnName.ToLower() == dc.ColumnName.ToLower()))
                    continue;

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
        public void CreateTable<T>(bool checkexisting = false)
        {
            CreateTable<T>(checkexisting, false);
        }

        public void CreateTable<T>(bool checkexisting=false, bool use_current_connection = false)
        {
            if (!use_current_connection)
                OpenIfNeeded();

            var autoinccolumn = "";
            var colsep = "";
            var workingtype = typeof(T);
            var tablename = workingtype.Name;

            //TABLENAME
            var annot_tablename = workingtype.GetCustomAttributes(typeof(DbTableName), false);
            if (annot_tablename != null && annot_tablename.Length > 0)
                tablename = ((DbTableName)annot_tablename[0]).Name;

            if (checkexisting)
            {
                var exists = true;
                try
                {
                    CreateCommand(string.Format("select 1 from {0}", tablename));
                    ExecuteScalarQuery();
                }
                catch { exists = false; }
                if (exists)
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
                {
                    autoinc = true;
                    autoinccolumn = colname;
                }
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
            if (pk != null && (DBMSType == DBMS.MariaDB || DBMSType == DBMS.MySql))
            {
                sb.Append(colsep + string.Format("PRIMARY KEY (`{0}`)", pk.Columns));
            }
            else if (pk != null && DBMSType == DBMS.PostgreSQL)
            {
                sb.Append(colsep + string.Format("PRIMARY KEY ({0})", pk.Columns));
            }
            else if (pk != null && DBMSType == DBMS.MSSqlServer)
            {
                sb.Append(colsep + string.Format("CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED ([{1}] ASC)", tablename, pk.Columns));
            }
            else if (pk != null && DBMSType == DBMS.SQLite && string.IsNullOrEmpty(autoinccolumn))
            {
                //IF SQLLITE, PK ONLY IF NOT AUTOINC. IF AUTOINC THAT COL IS PK.
                sb.Append(colsep + string.Format("PRIMARY KEY ({0})", pk.Columns));
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
                    if (DBMSType != DBMS.PostgreSQL)
                    {
                        if (index.IsUnique)
                            indexbuilder = string.Format("CREATE UNIQUE INDEX {0} ON {1} ({2})", new object[] { index.Name, tablename, index.Columns });
                        if (!index.IsUnique)
                            indexbuilder = string.Format("CREATE INDEX {0} ON {1} ({2})", new object[] { index.Name, tablename, index.Columns });
                    }
                    else
                    {
                        if (index.IsUnique)
                            indexbuilder = string.Format("CREATE UNIQUE INDEX {0} ON {1} (", index.Name, tablename);
                        if (!index.IsUnique)
                            indexbuilder = string.Format("CREATE INDEX {0} ON {1} (", index.Name, tablename);
                        var indexcols = index.Columns.Split(",", StringSplitOptions.RemoveEmptyEntries);
                        colsep = "";
                        foreach (var indcol in indexcols)
                        {
                            indexbuilder += colsep + string.Format("{0}", indcol);
                            colsep = ",";
                        }
                        indexbuilder += ")";

                    }

                    if (!string.IsNullOrEmpty(indexbuilder))
                    {
                        CreateCommand(indexbuilder);
                        ExecuteNonQuery();
                    }

                }
            }

            if (!use_current_connection)
                Close();

        }

        public List<T> Get<T>() where T : new()
        {
            return Get<T>(false);
        }

        public List<T> Get<T>(bool use_current_connection = false) where T : new()
        {
          

            var res = new List<T>();

            var t = typeof(T);
            var tname = t.Name;
            var annot_tablename = t.GetCustomAttributes(typeof(DbTableName), false);
            if (annot_tablename != null && annot_tablename.Length > 0)
                tname = ((DbTableName)annot_tablename[0]).Name;

            var ds = new DataSet();

            if (!use_current_connection)
                OpenIfNeeded();

            CreateCommand(string.Format("SELECT * FROM {0}", tname));
            FillDataset(ds, "NONE");

            if (!use_current_connection)
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

                    if (!r.Table.Columns.Contains(colname))
                        continue;

                    if (property.PropertyType.ToString().ToUpper() == "SYSTEM.INT32" && DBMSType == DBMS.SQLite)
                        property.SetValue(m, Convert.ToInt32(r[colname]), null);
                    else if (property.PropertyType.ToString().ToUpper() == "SYSTEM.BOOLEAN" && DBMSType == DBMS.SQLite)
                        property.SetValue(m, Convert.ToBoolean(r[colname]), null);
                    else
                        property.SetValue(m, r[colname], null);
                }
               
                res.Add(m);
            }

            return res;
        }

        public int Insert<T>(T model)
        {
            return Insert<T>(model, false);
        }

        public int Insert<T>(T model, bool use_current_connection = false)
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
                    prm.Value = DBNull.Value;
                else
                    prm.Value = value;

                if (DBMSType == DBMS.MSSqlServer)
                    query.Append(colsep + string.Format("{0}", colname));
                else if (DBMSType == DBMS.PostgreSQL)
                    query.Append(colsep + string.Format("{0}", colname));
                else if (DBMSType == DBMS.MariaDB || DBMSType == DBMS.MySql)
                    query.Append(colsep + string.Format("`{0}`", colname));
                else if (DBMSType == DBMS.SQLite)
                    query.Append(colsep + string.Format("{0}", colname));



                values.Append(colsep + string.Format("@{0}", colname));
                parameters.Add(prm);
                colsep = ", ";
            }
            query.Append(") ");
            values.Append(")");

            if (!string.IsNullOrEmpty(autoinccolumn) && DBMSType == DBMS.MSSqlServer)
            {
                //values.Append(" SELECT SCOPE_IDENTITY()");
                values.Append(" select @NewId=Scope_Identity()");
                parameters.Add(new IntwentySqlParameter() { ParameterName = "@NewId", Direction = ParameterDirection.Output, DataType = DbType.Int32 });
            }
            else if (!string.IsNullOrEmpty(autoinccolumn) && (DBMSType == DBMS.MariaDB || DBMSType == DBMS.MySql))
            {
                //values.Append(" SELECT LAST_INSERT_ID()");
                values.Append(" select @NewId=LAST_INSERT_ID()");
                parameters.Add(new IntwentySqlParameter() { ParameterName = "@NewId", Direction = ParameterDirection.Output, DataType = DbType.Int32 });
            }
           

            if (!use_current_connection)
                OpenIfNeeded();

            CreateCommand(query.ToString() + values.ToString());
            foreach (var p in parameters)
            {
                AddParameter(p);
            }

            var res = ExecuteNonQuery();
           
            var output = res.OutputParameters.Find(p => p.ParameterName == "@NewId");
            if (output != null && !string.IsNullOrEmpty(autoinccolumn))
            {
                var property = workingtype.GetProperty(autoinccolumn);
                if (property != null)
                    property.SetValue(model, output.Value, null);
            }

            if (DBMSType == DBMS.PostgreSQL && !string.IsNullOrEmpty(autoinccolumn))
            {
                var property = workingtype.GetProperty(autoinccolumn);
                if (property != null)
                {
                    CreateCommand(string.Format("SELECT currval('{0}')", tname.ToLower() + "_" + autoinccolumn.ToLower() + "_seq"));
                    property.SetValue(model, Convert.ToInt32(ExecuteScalarQuery()), null);
                }  
            }

            if (DBMSType == DBMS.SQLite && !string.IsNullOrEmpty(autoinccolumn))
            {
                var property = workingtype.GetProperty(autoinccolumn);
                if (property != null)
                {
                    CreateCommand(string.Format("SELECT Last_Insert_Rowid()"));
                    property.SetValue(model, Convert.ToInt32(ExecuteScalarQuery()), null);
                }

            }

            if (!use_current_connection)
                Close();


            return res.Value;
        }

        public int Update<T>(T model)
        {
            return Update<T>(model, false);
        }

        public int Update<T>(T model, bool use_current_connection = false)
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
                    prm.Value = DBNull.Value;
                else
                    prm.Value = value;

                if (DBMSType == DBMS.MSSqlServer)
                    query.Append(colsep + string.Format("{0}=@{0}", colname));
                else if (DBMSType == DBMS.PostgreSQL)
                    query.Append(colsep + string.Format("{0}=@{0}", colname));
                else if (DBMSType == DBMS.MariaDB || DBMSType == DBMS.MySql)
                    query.Append(colsep + string.Format("`{0}`=@{0}", colname));
                else if (DBMSType == DBMS.SQLite)
                    query.Append(colsep + string.Format("{0}=@{0}", colname));


                parameters.Add(prm);
                colsep = ", ";
            }

            if (keyparameters.Count == 0)
                throw new InvalidOperationException("Can't update a table without 'Primary Key' or an 'Auto Increment' column, use annotations.");

            query.Append(" WHERE ");
            var wheresep = "";
            foreach (var p in keyparameters)
            {
                if (DBMSType == DBMS.MSSqlServer)
                    query.Append(wheresep + string.Format("{0}=@{0}", p.ParameterName));
                else if (DBMSType == DBMS.PostgreSQL)
                    query.Append(wheresep + string.Format("{0}=@{0}", p.ParameterName));
                else if (DBMSType == DBMS.MariaDB || DBMSType == DBMS.MySql)
                    query.Append(wheresep + string.Format("`{0}`=@{0}", p.ParameterName));
                else if (DBMSType == DBMS.SQLite)
                    query.Append(wheresep + string.Format("{0}=@{0}", p.ParameterName));

                wheresep = " AND ";
            }

            if (!use_current_connection)
                OpenIfNeeded();

            CreateCommand(query.ToString());
            foreach (var p in keyparameters)
                AddParameter(p);
            foreach (var p in parameters)
                AddParameter(p);

            var res = ExecuteNonQuery().Value;

            if (!use_current_connection)
                Close();

            return res;

        }

        public int DeleteRange<T>(IEnumerable<T> model)
        {
            return DeleteRange<T>(model, false);
        }

        public int DeleteRange<T>(IEnumerable<T> model, bool use_current_connection = false)
        {
            var result = 0;
            foreach (var t in model)
            {
                result+=Delete(t, use_current_connection);
            }
            return result;
        }

        public int Delete<T>(T model)
        {
            return Delete<T>(model, false);
        }

        public int Delete<T>(T model, bool use_current_connection = false)
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
                if (DBMSType == DBMS.MSSqlServer)
                    query.Append(wheresep + string.Format("{0}=@{0}", p.ParameterName));
                else if (DBMSType == DBMS.PostgreSQL)
                    query.Append(wheresep + string.Format("{0}=@{0}",p.ParameterName));
                else if (DBMSType == DBMS.MariaDB || DBMSType == DBMS.MySql)
                    query.Append(wheresep + string.Format("`{0}`=@{0}", p.ParameterName));
                else if (DBMSType == DBMS.SQLite)
                    query.Append(wheresep + string.Format("{0}=@{0}", p.ParameterName));

                wheresep = " AND ";
            }

            if (!use_current_connection)
                OpenIfNeeded();

            CreateCommand(query.ToString());
            foreach (var p in keyparameters)
                AddParameter(p);

            var res = ExecuteNonQuery().Value;

            if (!use_current_connection)
                Close();

            return res;
        }



        public object ExecuteScalarQuery()
        {

            if (DBMSType == DBMS.MSSqlServer)
            {
                return sql_cmd.ExecuteScalar();
            }
            else if (DBMSType == DBMS.PostgreSQL)
            {
                return pgres_cmd.ExecuteScalar();
            }
            else if (DBMSType == DBMS.MariaDB || DBMSType == DBMS.MySql)
            {
                return mysql_cmd.ExecuteScalar();
            }
            else if (DBMSType == DBMS.SQLite)
            {
                return sqlite_cmd.ExecuteScalar();
            }
            else
            {
                return null;
            }
        }

        public bool TableExist(string tablename)
        {
            if (string.IsNullOrEmpty(tablename))
                return false;

            var result = true;

            try
            {
                CreateCommand(string.Format("SELECT 1 FROM {0}",tablename));
                ExecuteScalarQuery();

            }
            catch 
            {
                result = false;
            }

            return result;
        }

        public bool ColumnExist(string tablename, string columnname)
        {
            if (string.IsNullOrEmpty(tablename) || string.IsNullOrEmpty(columnname))
                return false;

            var result = true;

            try
            {
                CreateCommand(string.Format("SELECT {0} FROM {1} WHERE 1=2", columnname, tablename));
                ExecuteScalarQuery();

            }
            catch
            {
                result = false;
            }

            return result;
        }

        public NonQueryResult ExecuteNonQuery()
        {

            if (DBMSType == DBMS.MSSqlServer)
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
            else if (DBMSType == DBMS.PostgreSQL)
            {
                var res = new NonQueryResult();
                res.Value = pgres_cmd.ExecuteNonQuery();
                return res;
            }
            else if (DBMSType == DBMS.SQLite)
            {
                var res = new NonQueryResult();
                res.Value = sqlite_cmd.ExecuteNonQuery();
                return res;
            }
            else if (DBMSType == DBMS.MariaDB || DBMSType == DBMS.MySql)
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

            return null;
            

        }

        public SqlDataReader ExecuteSqlServerDataReader(CommandBehavior cbv)
        {
            if (DBMSType == DBMS.MSSqlServer)
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
            if (DBMSType == DBMS.PostgreSQL)
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
            if (DBMSType == DBMS.MariaDB || DBMSType == DBMS.MySql)
            {
                return mysql_cmd.ExecuteReader(cbv);
            }
            else
            {
                return null;
            }
        }

        public SQLiteDataReader ExecuteSqliteDataReader(CommandBehavior cbv)
        {
            if (DBMSType == DBMS.SQLite)
            {
                return sqlite_cmd.ExecuteReader(cbv);
            }
            else
            {
                return null;
            }
        }



        private void MakeCommand(string sqlcode, bool isstoredprocedure)
        {


            if (DBMSType == DBMS.MSSqlServer)
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
            else if (DBMSType == DBMS.PostgreSQL)
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
            else if (DBMSType == DBMS.MariaDB || DBMSType == DBMS.MySql)
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
            else if (DBMSType == DBMS.SQLite)
            {
                if (this.sqlite_cmd == null)
                {
                    this.sqlite_cmd = new SQLiteCommand();
                }
                else
                {
                    this.sqlite_cmd.Dispose();
                    this.sqlite_cmd = new SQLiteCommand();
                }
                this.sqlite_cmd.Connection = this.sqlite_connection;
                this.sqlite_cmd.CommandText = sqlcode;
                if (isstoredprocedure)
                {
                    this.sqlite_cmd.CommandType = CommandType.StoredProcedure;
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

            var dtmap = DBHelpers.GetDataTypeMap().Find(p => p.NetType == nettype.ToUpper() && ((longtext && p.Length == StringLength.Long) || (!longtext && p.Length == StringLength.Standard)) && p.DBMSType == DBMSType);
            if (dtmap==null)
                throw new InvalidOperationException(string.Format("Could not find DBMS specific datatype for {0} and {1}", nettype.ToUpper(), DBMSType));

            datatype = dtmap.DBMSDataType;

            var autoincmap = new DBMSCommandMap() { Key = "AUTOINC" };
            if (autoincrement)
                autoincmap = DBHelpers.GetDBMSCommandMap().Find(p => p.DBMSType == DBMSType && p.Key== "AUTOINC");

            if (autoincrement)
            {
                allownullvalue = "NOT NULL";
                defaultvalue = "";
                if (DBMSType == DBMS.PostgreSQL)
                    datatype = autoincmap.Command;
            }
            if (notnull)
            {
                allownullvalue = "NOT NULL";
                defaultvalue = "";
            }


            if (DBMSType == DBMS.MSSqlServer) 
                result = string.Format("{0} {1} {2} {3}", new object[] { name, datatype, autoincmap.Command, allownullvalue });

            if (DBMSType == DBMS.MariaDB || DBMSType == DBMS.MySql) 
                result = string.Format("`{0}` {1} {2} {3} {4}", new object[] { name, datatype, allownullvalue, autoincmap.Command, defaultvalue });

            if (DBMSType == DBMS.PostgreSQL)
                result = string.Format("{0} {1} {2}", new object[] { name, datatype, allownullvalue });

            if (DBMSType == DBMS.SQLite)
                result = string.Format("{0} {1} {2} {3}", new object[] { name, datatype, allownullvalue, autoincmap.Command });

            if (string.IsNullOrEmpty(result))
                throw new InvalidOperationException("Could not generate sql column definition");

            return result;
        }

       

       

      

     

      

       
    }
}
