using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using MySql.Data.MySqlClient;
using Npgsql;
using NpgsqlTypes;
using System.Text;

namespace Intwenty.Data.DBAccess
{
    public enum DBMS { MSSqlServer, MySql, MariaDB, Postgres };

    public enum DbColumnDataType { String, LongString, Integer, Decimal, Float, Boolean, TimeStamp, DateTime, Date };

    public enum SqlStmtParameterDirection { Input, Output};

    public class SqlStmtParameter
    {
        public string ParameterName { get; set; }

        public object Value { get; set; }

        public DbColumnDataType DataType { get; set; }

        public ParameterDirection Direction { get; set; }

        public SqlStmtParameter()
        {
            Direction = ParameterDirection.Input;
        }
    }

    public class NetCoreDBClient : IDisposable
    {
        private SqlConnection sql_connection;
        private SqlCommand sql_cmd;

        private NpgsqlConnection pgres_connection;
        private NpgsqlCommand pgres_cmd;

        private MySqlConnection mysql_connection;
        private MySqlCommand mysql_cmd;

        private int DBTYPE = -1;

        private string ConnStr = "";


        public NetCoreDBClient()
        {
        }

        public NetCoreDBClient(DBMS d, string connectionstring)
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

        public void AddParameter(SqlStmtParameter p)
        {
            if (DBTYPE == 1)
            {
                var param = new SqlParameter() { ParameterName = p.ParameterName, Value = p.Value, Direction = p.Direction };
                if (param.Direction == ParameterDirection.Output)
                    param.DbType = GetDBType(p.DataType);

                this.sql_cmd.Parameters.Add(param);
            }
            else if (DBTYPE == 3)
            {
                var param = new NpgsqlParameter() { ParameterName = p.ParameterName, Value = p.Value, Direction = p.Direction };
                if (param.Direction == ParameterDirection.Output)
                    param.DbType = GetDBType(p.DataType);

                this.pgres_cmd.Parameters.Add(param);
            }
            else if (DBTYPE == 4)
            {
                var param = new MySqlParameter() { ParameterName = p.ParameterName, Value = p.Value, Direction = p.Direction };
                if (param.Direction == ParameterDirection.Output)
                    param.DbType = GetDBType(p.DataType);

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

        public int ExecuteNonQuery()
        {

            if (DBTYPE == 1)
            {
                return sql_cmd.ExecuteNonQuery();
            }
            else if (DBTYPE == 3)
            {
                return pgres_cmd.ExecuteNonQuery();
            }
            else if (DBTYPE == 4)
            {
                return mysql_cmd.ExecuteNonQuery();
            }
            else
            {
                return sql_cmd.ExecuteNonQuery();
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


        private NpgsqlDbType GetPgresDBType(DbColumnDataType type)
        {
            NpgsqlDbType res;

            if (type == DbColumnDataType.Boolean)
            {
                res = NpgsqlDbType.Bit;
            }
            else if (type == DbColumnDataType.DateTime)
            {
                res = NpgsqlDbType.Date;
            }
            else if (type == DbColumnDataType.Decimal)
            {
                res = NpgsqlDbType.Double;
            }
            else if (type == DbColumnDataType.Float)
            {
                res = NpgsqlDbType.Double;
            }
            else if (type == DbColumnDataType.Integer)
            {
                res = NpgsqlDbType.Integer;
            }
            else if (type == DbColumnDataType.LongString)
            {
                res = NpgsqlDbType.Text;
            }
            else if (type == DbColumnDataType.String)
            {
                res = NpgsqlDbType.Varchar;
            }
            else if (type == DbColumnDataType.TimeStamp)
            {
                res = NpgsqlDbType.Timestamp;
            }
            else
            {
                res = NpgsqlDbType.Varchar;
            }


            return res;
        }

        private MySqlDbType GetMysqlDBType(DbColumnDataType type)
        {
            MySqlDbType res;

            if (type == DbColumnDataType.Boolean)
            {
                res = MySqlDbType.Bit;
            }
            else if (type == DbColumnDataType.DateTime)
            {
                res = MySqlDbType.DateTime;
            }
            else if (type == DbColumnDataType.Decimal)
            {
                res = MySqlDbType.Decimal;
            }
            else if (type == DbColumnDataType.Float)
            {
                res = MySqlDbType.Float;
            }
            else if (type == DbColumnDataType.Integer)
            {
                res = MySqlDbType.Int32;
            }
            else if (type == DbColumnDataType.LongString)
            {
                res = MySqlDbType.Text;
            }
            else if (type == DbColumnDataType.String)
            {
                res = MySqlDbType.VarChar;
            }
            else if (type == DbColumnDataType.TimeStamp)
            {
                res = MySqlDbType.Timestamp;
            }
            else
            {
                res = MySqlDbType.VarChar;
            }


            return res;
        }

        private SqlDbType GetSqlServerDBType(DbColumnDataType type)
        {
            SqlDbType res;

            if (type == DbColumnDataType.Boolean)
            {
                res = SqlDbType.Bit;
            }
            else if (type == DbColumnDataType.DateTime)
            {
                res = SqlDbType.DateTime;
            }
            else if (type == DbColumnDataType.Decimal)
            {
                res = SqlDbType.Decimal;
            }
            else if (type == DbColumnDataType.Float)
            {
                res = SqlDbType.Float;
            }
            else if (type == DbColumnDataType.Integer)
            {
                res = SqlDbType.Int;
            }
            else if (type == DbColumnDataType.LongString)
            {
                res = SqlDbType.NVarChar;
            }
            else if (type == DbColumnDataType.String)
            {
                res = SqlDbType.NVarChar;
            }
            else if (type == DbColumnDataType.TimeStamp)
            {
                res = SqlDbType.Timestamp;
            }
            else
            {
                res = SqlDbType.NVarChar;
            }


            return res;
        }

        private DbType GetDBType(DbColumnDataType type)
        {
            DbType res;

            if (type == DbColumnDataType.Boolean)
            {
                res = DbType.Boolean;
            }
            else if (type == DbColumnDataType.DateTime)
            {
                res = DbType.DateTime;
            }
            else if (type == DbColumnDataType.Decimal)
            {
                res = DbType.Decimal;
            }
            else if (type == DbColumnDataType.Float)
            {
                res = DbType.Double;
            }
            else if (type == DbColumnDataType.Integer)
            {
                res = DbType.Int32;
            }
            else if (type == DbColumnDataType.LongString)
            {
                res = DbType.String;
            }
            else if (type == DbColumnDataType.String)
            {
                res = DbType.String;
            }
            else
            {
                res = DbType.Object;
            }


            return res;
        }





    }
}
