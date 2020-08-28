using System;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using Npgsql;
using System.Text;
using System.Collections.Generic;
using Intwenty.Data.DBAccess.Helpers;
using Intwenty.Data.DBAccess.Annotations;
using System.Data.SQLite;
using Intwenty.Model;
using Intwenty.Data.Dto;
using System.Collections.Concurrent;

namespace Intwenty.Data.DBAccess
{
   


    public class IntwentySqlDbClient : IntwentyDbClient, IDisposable, IIntwentyDbSql
    {
        private SqlConnection sql_connection;
        private SqlCommand sql_cmd;

        private NpgsqlConnection pgres_connection;
        private NpgsqlCommand pgres_cmd;

        private MySqlConnection mysql_connection;
        private MySqlCommand mysql_cmd;

        private SQLiteConnection sqlite_connection;
        private SQLiteCommand sqlite_cmd;

        private static ConcurrentDictionary<string, object> Cache;

        public IntwentySqlDbClient()
        {
            ConnectionString = string.Empty;
            DbEngine = DBMS.MSSqlServer;
            if (Cache == null)
                Cache = new ConcurrentDictionary<string, object>();
        }

        public IntwentySqlDbClient(IntwentySettings settings)
        {
            DbEngine = settings.DefaultConnectionDBMS;
            ConnectionString = settings.DefaultConnection;
            if (DbEngine == DBMS.MongoDb || DbEngine == DBMS.LiteDb)
                throw new InvalidOperationException("IntwentySqlDbClient configured with wrong DBMS setting");

            if (Cache == null)
                Cache = new ConcurrentDictionary<string, object>();
        }

        public IntwentySqlDbClient(DBMS d, string connectionstring)
        {
            DbEngine = d;
            ConnectionString = connectionstring;
            if (DbEngine == DBMS.MongoDb || DbEngine == DBMS.LiteDb)
                throw new InvalidOperationException("IntwentySqlDbClient configured with wrong DBMS setting");

            if (Cache == null)
                Cache = new ConcurrentDictionary<string, object>();
        }


        public void Dispose()
        {

            if (DbEngine ==  DBMS.MSSqlServer)
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
            else if (DbEngine ==  DBMS.PostgreSQL)
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
            else if (DbEngine ==  DBMS.MariaDB || DbEngine == DBMS.MySql)
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
            else if (DbEngine == DBMS.SQLite)
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
            if (DbEngine == DBMS.MSSqlServer)
            {

                sql_connection = new SqlConnection();
                sql_connection.ConnectionString = ConnectionString;
                sql_connection.Open();
            }
            else if (DbEngine == DBMS.PostgreSQL)
            {

                pgres_connection = new NpgsqlConnection();
                pgres_connection.ConnectionString = ConnectionString;
                pgres_connection.Open();
            }
            else if (DbEngine == DBMS.MariaDB || DbEngine == DBMS.MySql)
            {

                mysql_connection = new MySqlConnection();
                mysql_connection.ConnectionString = ConnectionString;
                mysql_connection.Open();
            }
            else if (DbEngine == DBMS.SQLite)
            {

                sqlite_connection = new SQLiteConnection();
                sqlite_connection.ConnectionString = ConnectionString;
                sqlite_connection.Open();
            }
        }

        private void OpenIfNeeded()
        {
            if (DbEngine == DBMS.MSSqlServer)
            {
                if (sql_connection != null && sql_connection.State == ConnectionState.Open)
                    return;

                sql_connection = new SqlConnection();
                sql_connection.ConnectionString = this.ConnectionString;
                sql_connection.Open();
            }
            else if (DbEngine == DBMS.PostgreSQL)
            {
                if (pgres_connection != null && pgres_connection.State == ConnectionState.Open)
                    return;

                pgres_connection = new NpgsqlConnection();
                pgres_connection.ConnectionString = this.ConnectionString;
                pgres_connection.Open();
            }
            else if (DbEngine == DBMS.MariaDB || DbEngine == DBMS.MySql)
            {
                if (mysql_connection != null && mysql_connection.State == ConnectionState.Open)
                    return;

                mysql_connection = new MySqlConnection();
                mysql_connection.ConnectionString = this.ConnectionString;
                mysql_connection.Open();
            }
            else if (DbEngine == DBMS.SQLite)
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
            if (DbEngine == DBMS.MSSqlServer)
            {
                if (sql_connection != null)
                {
                    if (this.sql_connection.State != ConnectionState.Closed)
                    {
                        this.sql_connection.Close();
                    }
                }

            }
            else if (DbEngine == DBMS.PostgreSQL)
            {
                if (pgres_connection != null)
                {
                    if (this.pgres_connection.State != ConnectionState.Closed)
                    {
                        this.pgres_connection.Close();
                    }
                }

            }
            else if (DbEngine == DBMS.MariaDB || DbEngine == DBMS.MySql)
            {
                if (mysql_connection != null)
                {
                    if (this.mysql_connection.State != ConnectionState.Closed)
                    {
                        this.mysql_connection.Close();
                    }
                }

            }
            else if (DbEngine == DBMS.SQLite)
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
            if (DbEngine == DBMS.MSSqlServer)
            {
                this.sql_cmd.Parameters.AddWithValue(name, value);
            }
            else if (DbEngine == DBMS.PostgreSQL)
            {
                this.pgres_cmd.Parameters.Add(new NpgsqlParameter() { Value = value, ParameterName = name });
            }
            else if (DbEngine == DBMS.MariaDB || DbEngine == DBMS.MySql)
            {
                this.mysql_cmd.Parameters.Add(new MySqlParameter() { Value = value, ParameterName = name });
            }
            else if (DbEngine == DBMS.SQLite)
            {
                this.sqlite_cmd.Parameters.Add(new SQLiteParameter() { Value = value, ParameterName = name });
            }
        }

        public void AddParameter(IntwentyParameter p)
        {
            if (DbEngine == DBMS.MSSqlServer)
            {
                var param = new SqlParameter() { ParameterName = p.ParameterName, Value = p.Value, Direction = p.Direction };
                if (param.Direction == ParameterDirection.Output)
                    param.DbType = p.DataType;

                this.sql_cmd.Parameters.Add(param);
            }
            else if (DbEngine == DBMS.PostgreSQL)
            {
              
                var param = new NpgsqlParameter() { ParameterName = p.ParameterName, Value = p.Value, Direction = p.Direction };
                if (param.Direction == ParameterDirection.Output)
                    param.DbType = p.DataType;

                this.pgres_cmd.Parameters.Add(param);
            }
            else if (DbEngine == DBMS.MariaDB || DbEngine == DBMS.MySql)
            {
                var param = new MySqlParameter() { ParameterName = p.ParameterName, Value = p.Value, Direction = p.Direction };
                if (param.Direction == ParameterDirection.Output)
                    param.DbType = p.DataType;

                this.mysql_cmd.Parameters.Add(param);

            }
            else if (DbEngine == DBMS.SQLite)
            {
                var param = new SQLiteParameter() { ParameterName = p.ParameterName, Value = p.Value, Direction = p.Direction };
                if (param.Direction == ParameterDirection.Output)
                    param.DbType = p.DataType;

                this.sqlite_cmd.Parameters.Add(param);
            }
        }



        public void SetSelectCommandTimeout(Int32 seconds)
        {
            if (DbEngine == DBMS.MSSqlServer)
                this.sql_cmd.CommandTimeout = seconds;
            else if (DbEngine == DBMS.PostgreSQL)
                this.pgres_cmd.CommandTimeout = seconds;
            else if (DbEngine == DBMS.MariaDB || DbEngine == DBMS.MySql)
                this.mysql_cmd.CommandTimeout = seconds;
            else if (DbEngine == DBMS.SQLite)
                this.sqlite_cmd.CommandTimeout = seconds;

        }


        private void FillDataset(DataSet ds, string tablename)
        {
            ds.CaseSensitive = false;

            if (DbEngine == DBMS.MSSqlServer)
            {
                var adapt = new SqlDataAdapter(sql_cmd);
                adapt.MissingMappingAction = MissingMappingAction.Passthrough;
                adapt.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                adapt.Fill(ds, tablename);
            }
            else if (DbEngine == DBMS.PostgreSQL)
            {
                var adapt = new NpgsqlDataAdapter(pgres_cmd);
                adapt.MissingMappingAction = MissingMappingAction.Passthrough;
                adapt.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                adapt.Fill(ds, tablename);
            }
            else if (DbEngine == DBMS.MariaDB || DbEngine == DBMS.MySql)
            {
                var adapt = new MySqlDataAdapter(mysql_cmd);
                adapt.MissingMappingAction = MissingMappingAction.Passthrough;
                adapt.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                adapt.Fill(ds, tablename);
            }
            else if (DbEngine == DBMS.SQLite)
            {
                var adapt = new SQLiteDataAdapter(sqlite_cmd);
                //The line below gave error: no current row
                //adapt.MissingMappingAction = MissingMappingAction.Passthrough;
                //adapt.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                adapt.Fill(ds, tablename);
            }

        }

        public ApplicationTable GetDataSet()
        {
            var result = new ApplicationTable();
            var ds = new DataSet();
            FillDataset(ds, "tempdt");
            foreach (DataRow t in ds.Tables[0].Rows)
            {
                var row = new ApplicationTableRow();
                foreach (DataColumn dc in ds.Tables[0].Columns)
                {
                    row.Values.Add(new ApplicationValue() { DbName = dc.ColumnName, Value = t[dc] });
                }

                result.Rows.Add(row);

                var dataid = row.Values.Find(p => p.DbName == "Id");
                if (dataid != null)
                    row.Id = dataid.GetAsInt().Value;

                var dataversion = row.Values.Find(p => p.DbName == "Version");
                if (dataversion != null)
                    row.Version = dataversion.GetAsInt().Value;

                var parentid = row.Values.Find(p => p.DbName == "ParentId");
                if (parentid != null)
                    row.ParentId = parentid.GetAsInt().Value;

            }
            return result;
        }



        public StringBuilder GetJsonArray(int minrow=0, int maxrow=0)
        {
            var sb = new StringBuilder();

            var ds = new DataSet();
            var rindex = 0;
            var sep1 = "";
          

            FillDataset(ds, "NONE");

            sb.Append("[");
            foreach (DataRow r in ds.Tables[0].Rows)
            {

                rindex += 1;
                if (maxrow > minrow && (minrow > 0 || maxrow > 0))
                {
                    if (rindex <= minrow)
                        continue;
                    if (rindex > maxrow)
                        break;
                }

                sb.Append(sep1 + "{");
                sep1 = ",";

                var sep2 = "";
                foreach (DataColumn dc in ds.Tables[0].Columns)
                {
                    var val = DBHelpers.GetJSONValue(r, dc);
                    if (string.IsNullOrEmpty(val))
                        continue;

                    sb.Append(sep2 + val);
                    sep2 = ",";
                }

                sb.Append("}");

            }

            sb.Append("]");

            return sb;
        }

        public StringBuilder GetJsonArray(List<IIntwentyDataColum> columns, int minrow = 0, int maxrow = 0)
        {

            if (columns == null)
                throw new InvalidOperationException("Parameter columns can't be null");

            var sb = new StringBuilder();
            var ds = new DataSet();
            var rindex = 0;
            var sep1 = "";
           

            FillDataset(ds, "NONE");

            sb.Append("[");
            foreach (DataRow r in ds.Tables[0].Rows)
            {

                
                rindex += 1;
                if (maxrow > minrow && (minrow > 0 || maxrow > 0))
                {
                    if (rindex <= minrow)
                        continue;
                    if (rindex > maxrow)
                        break;
                }


                sb.Append(sep1 + "{");
                sep1 = ",";

                var sep2 = "";
                foreach (DataColumn dc in ds.Tables[0].Columns)
                {
                    var icol = columns.Find(p => p.ColumnName.ToLower() == dc.ColumnName.ToLower());
                    if (icol == null)
                        continue;

                    var val = DBHelpers.GetJSONValue(r[dc], icol);
                    if (string.IsNullOrEmpty(val))
                        continue;

                    sb.Append(sep2 + val);
                    sep2 = ",";
                }

                sb.Append("}");

            }

            sb.Append("]");

            return sb;
        }

        public StringBuilder GetJsonObject()
        {
            var sb = new StringBuilder();
            var ds = new DataSet();
            var sep = "";

            FillDataset(ds, "NONE");
            
            if (ds.Tables[0].Rows.Count == 0)
            {
                sb.Append("{}");
                return sb;
            }

            sb.Append("{");

            foreach (DataColumn dc in ds.Tables[0].Columns)
            {
                var val = DBHelpers.GetJSONValue(ds.Tables[0].Rows[0], dc);
                if (string.IsNullOrEmpty(val))
                    continue;

                sb.Append(sep + val);
                sep = ",";

            }

            sb.Append("}");

            return sb;


        }

        public StringBuilder GetJsonObject(List<IIntwentyDataColum> columns)
        {
            if (columns == null)
                throw new InvalidOperationException("Parameter columns can't be null");

            var sb = new StringBuilder();
            var ds = new DataSet();
            var sep = "";

            FillDataset(ds, "NONE");

            if (ds.Tables[0].Rows.Count == 0)
            {
                sb.Append("{}");
                return sb;
            }


            sb.Append("{");

            foreach (DataColumn dc in ds.Tables[0].Columns)
            {
                var icol = columns.Find(p => p.ColumnName.ToLower() == dc.ColumnName.ToLower());
                if (icol == null)
                    continue;

                var val = DBHelpers.GetJSONValue(ds.Tables[0].Rows[0][dc], icol);
                if (string.IsNullOrEmpty(val))
                    continue;

                sb.Append(sep + val);
                sep = ",";
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
            sb.Append("CREATE TABLE " + tablename + " (");
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
            if (pk != null && (DbEngine == DBMS.MariaDB || DbEngine == DBMS.MySql))
            {
                sb.Append(colsep + string.Format("PRIMARY KEY ({0})", pk.Columns));
            }
            else if (pk != null && DbEngine == DBMS.PostgreSQL)
            {
                sb.Append(colsep + string.Format("PRIMARY KEY ({0})", pk.Columns));
            }
            else if (pk != null && DbEngine == DBMS.MSSqlServer)
            {
                sb.Append(colsep + string.Format("CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED ({1} ASC)", tablename, pk.Columns));
            }
            else if (pk != null && DbEngine == DBMS.SQLite && string.IsNullOrEmpty(autoinccolumn))
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
                    if (DbEngine != DBMS.PostgreSQL)
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

        public T GetOne<T>(int id, bool use_current_connection = false) where T : new()
        {
            var parameters = new List<IntwentyParameter>();
            parameters.Add(new IntwentyParameter() { ParameterName = "@KEY1", Value = id });
            var result = GetFromTableByType<T>(parameters, null, use_current_connection);
            if (result.Count > 0)
                return result[0];
            else
                return default(T);
        }

        public T GetOne<T>(int id) where T : new()
        {
            var parameters = new List<IntwentyParameter>();
            parameters.Add(new IntwentyParameter() { ParameterName = "@KEY1", Value = id });
            var result = GetFromTableByType<T>(parameters, null, false);
            if (result.Count > 0)
                return result[0];
            else
                return default(T);
        }

        public T GetOne<T>(string id) where T : new()
        { 
            var parameters = new List<IntwentyParameter>();
            parameters.Add(new IntwentyParameter() { ParameterName="@KEY1", Value=id });
            var result = GetFromTableByType<T>(parameters, null, false);
            if (result.Count > 0)
                return result[0];
            else
                return default(T);
        }

        public List<T> GetAll<T>() where T : new()
        {
            var workingtype = typeof(T);
            var tname = workingtype.Name;
            var annot_tablename = workingtype.GetCustomAttributes(typeof(DbTableName), false);
            if (annot_tablename != null && annot_tablename.Length > 0)
                tname = ((DbTableName)annot_tablename[0]).Name;

            object objectresult;
            if (Cache.TryGetValue(string.Format("ALL_{0}", tname), out objectresult))
            {
                return (List<T>)objectresult;
            }

            var result = GetFromTableByType<T>(new List<IntwentyParameter>(), null, false);

            if (result.Count < 50000) 
            {
                Cache.TryAdd(string.Format("ALL_{0}", tname), result);
            }
            return result;
        }

        public List<T> GetAll<T>(bool use_current_connection = false) where T : new()
        {
            return GetFromTableByType<T>(new List<IntwentyParameter>(), null, use_current_connection);
        }

        public List<T> GetByExpression<T>(IntwentyExpression expression) where T : new()
        {
            return GetFromTableByType<T>(new List<IntwentyParameter>(), expression, false);
        }

        private List<T> GetFromTableByType<T>(List<IntwentyParameter> parameters, IntwentyExpression expression=null, bool use_current_connection = false) where T : new()
        {


            var res = new List<T>();

            string wherestmt = string.Empty;
            var isparameterized = false;
            var workingtype = typeof(T);
            var tname = workingtype.Name;
            var annot_tablename = workingtype.GetCustomAttributes(typeof(DbTableName), false);
            if (annot_tablename != null && annot_tablename.Length > 0)
                tname = ((DbTableName)annot_tablename[0]).Name;

            if (expression==null)
            {
                DbTablePrimaryKey pk = null;
                var annot_pk = workingtype.GetCustomAttributes(typeof(DbTablePrimaryKey), false);
                if (annot_pk != null && annot_pk.Length > 0)
                    pk = ((DbTablePrimaryKey)annot_pk[0]);

                if (pk == null && parameters != null && parameters.Count > 0)
                    throw new InvalidOperationException(string.Format("Can't query {0} with parameters, it has no primary key.", tname));

                if (pk != null && !string.IsNullOrEmpty(pk.Columns) && parameters != null && parameters.Count > 0)
                    isparameterized = true;


                if (isparameterized)
                {
                    wherestmt = " WHERE";
                    var keycols = pk.Columns.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < keycols.Length; i++)
                    {
                        if (parameters.Count > i)
                        {
                            var prm = parameters[i];
                            prm.ParameterName = "@" + keycols[i];
                            if (i == 0)
                                wherestmt += string.Format(" {0}={1}", keycols[i], prm.ParameterName);
                            else
                                wherestmt += string.Format(" AND {0}={1}", keycols[i], prm.ParameterName);
                        }
                    }

                }


            }
            else
            {
                parameters = expression.GetParameters();
                if (parameters == null)
                    throw new InvalidOperationException(string.Format("Can't query {0} with expression without parameters.", tname));
                if (parameters.Count < 1)
                    throw new InvalidOperationException(string.Format("Can't query {0} with expression without parameters.", tname));

                wherestmt = expression.GetSqlWhere();

            }

            var ds = new DataSet();

            if (!use_current_connection)
                OpenIfNeeded();

            var stmt = string.Format("SELECT * FROM {0}", tname);
            if (isparameterized)
                stmt = stmt + wherestmt;

            CreateCommand(stmt);
            if (isparameterized)
            {
                foreach (var p in parameters)
                    AddParameter(p);
            }
            FillDataset(ds, "tempdt");

            if (!use_current_connection)
                Close();

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                var m = new T();
                var memberproperties = workingtype.GetProperties();
                foreach (var property in memberproperties)
                {
                    var colname = property.Name;
                    var annot_colname = property.GetCustomAttributes(typeof(DbColumnName), false);
                    if (annot_colname != null && annot_colname.Length > 0)
                        colname = ((DbColumnName)annot_colname[0]).Name;

                    if (DbEngine == DBMS.PostgreSQL)
                        colname = colname.ToLower();

                    if (r[colname] == DBNull.Value)
                        continue;

                    if (!r.Table.Columns.Contains(colname))
                        continue;

                    if (property.PropertyType.ToString().ToUpper() == "SYSTEM.INT32")
                        property.SetValue(m, Convert.ToInt32(r[colname]), null);
                    else if (property.PropertyType.ToString().ToUpper() == "SYSTEM.BOOLEAN")
                        property.SetValue(m, Convert.ToBoolean(r[colname]), null);
                    else if (property.PropertyType.ToString().ToUpper() == "SYSTEM.DECIMAL")
                        property.SetValue(m, Convert.ToDecimal(r[colname]), null);
                    else if (property.PropertyType.ToString().ToUpper() == "SYSTEM.SINGLE")
                        property.SetValue(m, Convert.ToSingle(r[colname]), null);
                    else if (property.PropertyType.ToString().ToUpper() == "SYSTEM.DOUBLE")
                        property.SetValue(m, Convert.ToDouble(r[colname]), null);
                    else if (property.PropertyType.FullName.ToUpper().Contains("SYSTEM.DATETIMEOFFSET") && DbEngine == DBMS.SQLite)
                        property.SetValue(m, new DateTimeOffset(Convert.ToDateTime(r[colname])), null);
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
            var autoinccolumn = string.Empty;
            var colsep = "";
            var workingtype = typeof(T);
            var tname = workingtype.Name;
            var annot_tablename = workingtype.GetCustomAttributes(typeof(DbTableName), false);
            if (annot_tablename != null && annot_tablename.Length > 0)
                tname = ((DbTableName)annot_tablename[0]).Name;

            object rm;
            Cache.Remove(string.Format("ALL_{0}", tname), out rm);

            var query = new StringBuilder(string.Format("INSERT INTO {0} (", tname));
            var values = new StringBuilder(" VALUES (");
            var parameters = new List<IntwentyParameter>();

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

                //SPECIAL SINCE SQL LITE CANT HANDLE DATETIMEOFFSET
                if (value != null && value != DBNull.Value && m.PropertyType.FullName.ToUpper().Contains("SYSTEM.DATETIMEOFFSET") && DbEngine == DBMS.SQLite)
                {
                    value = ((DateTimeOffset)value).DateTime;
                }

                var prm = new IntwentyParameter();
                prm.ParameterName = "@" + colname;

                if (value == null)
                    prm.Value = DBNull.Value;
                else
                    prm.Value = value;

                if (DbEngine == DBMS.MSSqlServer)
                    query.Append(colsep + string.Format("{0}", colname));
                else if (DbEngine == DBMS.PostgreSQL)
                    query.Append(colsep + string.Format("{0}", colname));
                else if (DbEngine == DBMS.MariaDB || DbEngine == DBMS.MySql)
                    query.Append(colsep + string.Format("`{0}`", colname));
                else if (DbEngine == DBMS.SQLite)
                    query.Append(colsep + string.Format("{0}", colname));



                values.Append(colsep + string.Format("@{0}", colname));
                parameters.Add(prm);
                colsep = ", ";
            }
            query.Append(") ");
            values.Append(")");

            if (!string.IsNullOrEmpty(autoinccolumn) && DbEngine == DBMS.MSSqlServer)
            {
                values.Append(" select @NewId=Scope_Identity()");
                parameters.Add(new IntwentyParameter() { ParameterName = "@NewId", Direction = ParameterDirection.Output, DataType = DbType.Int32 });
            }
          

            if (!use_current_connection)
                OpenIfNeeded();

            CreateCommand(query.ToString() + values.ToString());
            foreach (var p in parameters)
            {
                AddParameter(p);
            }

            var res = ExecuteNonQuery();
           
            if (!string.IsNullOrEmpty(autoinccolumn))
            {
                if (DbEngine == DBMS.MSSqlServer)
                {
                    var output = res.OutputParameters.Find(p => p.ParameterName == "@NewId");
                    if (output != null)
                    {
                        var property = workingtype.GetProperty(autoinccolumn);
                        if (property != null)
                            property.SetValue(model, output.Value, null);
                    }
                }

                if (DbEngine == DBMS.PostgreSQL)
                {
                    var property = workingtype.GetProperty(autoinccolumn);
                    if (property != null)
                    {
                        CreateCommand(string.Format("SELECT currval('{0}')", tname.ToLower() + "_" + autoinccolumn.ToLower() + "_seq"));
                        property.SetValue(model, Convert.ToInt32(ExecuteScalarQuery()), null);
                    }
                }

                if (DbEngine == DBMS.SQLite)
                {
                    var property = workingtype.GetProperty(autoinccolumn);
                    if (property != null)
                    {
                        CreateCommand(string.Format("SELECT Last_Insert_Rowid()"));
                        property.SetValue(model, Convert.ToInt32(ExecuteScalarQuery()), null);
                    }
                }

                if ((DbEngine == DBMS.MariaDB || DbEngine == DBMS.MySql))
                {
                    var property = workingtype.GetProperty(autoinccolumn);
                    if (property != null)
                    {
                        CreateCommand(string.Format("SELECT LAST_INSERT_ID()"));
                        property.SetValue(model, Convert.ToInt32(ExecuteScalarQuery()), null);
                    }
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

            object rm;
            Cache.Remove(string.Format("ALL_{0}", tname), out rm);

            DbTablePrimaryKey pk = null;
            var annot_pk = t.GetCustomAttributes(typeof(DbTablePrimaryKey), false);
            if (annot_pk != null && annot_pk.Length > 0)
                pk = ((DbTablePrimaryKey)annot_pk[0]);

            var query = new StringBuilder(string.Format("UPDATE {0} SET ", tname));
            var parameters = new List<IntwentyParameter>();
            var keyparameters = new List<IntwentyParameter>();

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
                        var keyprm = new IntwentyParameter() { ParameterName = colname, Value = value };
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
                            var keyprm = new IntwentyParameter() { ParameterName = colname, Value = value };
                            keyparameters.Add(keyprm);
                        }
                    }

                }

                if (keyparameters.Exists(p => p.ParameterName == colname))
                    continue;

                //SPECIAL SINCE SQL LITE CANT HANDLE DATETIMEOFFSET
                if (value != null && value != DBNull.Value && m.PropertyType.FullName.ToUpper().Contains("SYSTEM.DATETIMEOFFSET") && DbEngine == DBMS.SQLite)
                {
                    value = ((DateTimeOffset)value).DateTime;
                }

                var prm = new IntwentyParameter();
                prm.ParameterName = "@" + colname;
                if (value == null)
                    prm.Value = DBNull.Value;
                else
                    prm.Value = value;

                if (DbEngine == DBMS.MSSqlServer)
                    query.Append(colsep + string.Format("{0}=@{0}", colname));
                else if (DbEngine == DBMS.PostgreSQL)
                    query.Append(colsep + string.Format("{0}=@{0}", colname));
                else if (DbEngine == DBMS.MariaDB || DbEngine == DBMS.MySql)
                    query.Append(colsep + string.Format("`{0}`=@{0}", colname));
                else if (DbEngine == DBMS.SQLite)
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
                if (DbEngine == DBMS.MSSqlServer)
                    query.Append(wheresep + string.Format("{0}=@{0}", p.ParameterName));
                else if (DbEngine == DBMS.PostgreSQL)
                    query.Append(wheresep + string.Format("{0}=@{0}", p.ParameterName));
                else if (DbEngine == DBMS.MariaDB || DbEngine == DBMS.MySql)
                    query.Append(wheresep + string.Format("`{0}`=@{0}", p.ParameterName));
                else if (DbEngine == DBMS.SQLite)
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

            object rm;
            Cache.Remove(string.Format("ALL_{0}", tname), out rm);

            DbTablePrimaryKey pk = null;
            var annot_pk = t.GetCustomAttributes(typeof(DbTablePrimaryKey), false);
            if (annot_pk != null && annot_pk.Length > 0)
                pk = ((DbTablePrimaryKey)annot_pk[0]);

            var query = new StringBuilder(string.Format("DELETE FROM {0} ", tname));
            var keyparameters = new List<IntwentyParameter>();

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
                        var keyprm = new IntwentyParameter() { ParameterName = colname, Value = value };
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
                            var keyprm = new IntwentyParameter() { ParameterName = colname, Value = value };
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
                if (DbEngine == DBMS.MSSqlServer)
                    query.Append(wheresep + string.Format("{0}=@{0}", p.ParameterName));
                else if (DbEngine == DBMS.PostgreSQL)
                    query.Append(wheresep + string.Format("{0}=@{0}",p.ParameterName));
                else if (DbEngine == DBMS.MariaDB || DbEngine == DBMS.MySql)
                    query.Append(wheresep + string.Format("`{0}`=@{0}", p.ParameterName));
                else if (DbEngine == DBMS.SQLite)
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

            if (DbEngine == DBMS.MSSqlServer)
            {
                return sql_cmd.ExecuteScalar();
            }
            else if (DbEngine == DBMS.PostgreSQL)
            {
                return pgres_cmd.ExecuteScalar();
            }
            else if (DbEngine == DBMS.MariaDB || DbEngine == DBMS.MySql)
            {
                return mysql_cmd.ExecuteScalar();
            }
            else if (DbEngine == DBMS.SQLite)
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

            if (DbEngine == DBMS.MSSqlServer)
            {
                var res = new NonQueryResult();
                res.Value = sql_cmd.ExecuteNonQuery();
                foreach (SqlParameter p in sql_cmd.Parameters)
                {
                    if (p.Direction == ParameterDirection.Output)
                        res.OutputParameters.Add(new IntwentyParameter() { ParameterName = p.ParameterName, Direction = ParameterDirection.Output, DataType = p.DbType, Value = p.Value });
                }

                return res;
            }
            else if (DbEngine == DBMS.PostgreSQL)
            {
                var res = new NonQueryResult();
                res.Value = pgres_cmd.ExecuteNonQuery();
                return res;
            }
            else if (DbEngine == DBMS.SQLite)
            {
                var res = new NonQueryResult();
                res.Value = sqlite_cmd.ExecuteNonQuery();
                return res;
            }
            else if (DbEngine == DBMS.MariaDB || DbEngine == DBMS.MySql)
            {
                var res = new NonQueryResult();
                res.Value = mysql_cmd.ExecuteNonQuery();
                return res;
            }

            return null;
            

        }

     


        private void MakeCommand(string sqlcode, bool isstoredprocedure)
        {


            if (DbEngine == DBMS.MSSqlServer)
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
            else if (DbEngine == DBMS.PostgreSQL)
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
            else if (DbEngine == DBMS.MariaDB || DbEngine == DBMS.MySql)
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
            else if (DbEngine == DBMS.SQLite)
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

            if (nettype.Contains("["))
            {
                var index1 = nettype.IndexOf("[");
                var index2 = nettype.IndexOf("]");
                nettype = nettype.Substring(index1 + 1, (index2) - (index1 + 1));

            }

            var dtmap = DBHelpers.GetDataTypeMap().Find(p => p.NetType == nettype.ToUpper() && ((longtext && p.Length == StringLength.Long) || (!longtext && p.Length == StringLength.Standard)) && p.DbEngine == DbEngine);
            if (dtmap==null)
                throw new InvalidOperationException(string.Format("Could not find DBMS specific datatype for {0} and {1}", nettype.ToUpper(), DbEngine));

            datatype = dtmap.DBMSDataType;

            var autoincmap = new DBMSCommandMap() { Key = "AUTOINC" };
            if (autoincrement)
                autoincmap = DBHelpers.GetDBMSCommandMap().Find(p => p.DbEngine == DbEngine && p.Key== "AUTOINC");

            if (autoincrement)
            {
                allownullvalue = "NOT NULL";
                defaultvalue = "";
                if (DbEngine == DBMS.PostgreSQL)
                    datatype = autoincmap.Command;
            }
            if (notnull)
            {
                allownullvalue = "NOT NULL";
                defaultvalue = "";
            }


            if (DbEngine == DBMS.MSSqlServer) 
                result = string.Format("{0} {1} {2} {3}", new object[] { name, datatype, autoincmap.Command, allownullvalue });

            if (DbEngine == DBMS.MariaDB || DbEngine == DBMS.MySql) 
                result = string.Format("`{0}` {1} {2} {3} {4}", new object[] { name, datatype, allownullvalue, autoincmap.Command, defaultvalue });

            if (DbEngine == DBMS.PostgreSQL)
                result = string.Format("{0} {1} {2}", new object[] { name, datatype, allownullvalue });

            if (DbEngine == DBMS.SQLite)
                result = string.Format("{0} {1} {2} {3}", new object[] { name, datatype, allownullvalue, autoincmap.Command });

            if (string.IsNullOrEmpty(result))
                throw new InvalidOperationException("Could not generate sql column definition");

            return result;
        }

       
    }
}
