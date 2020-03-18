using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using MySql.Data.MySqlClient;
using Npgsql;
using NpgsqlTypes;
using System.Text;

namespace NetCoreDBAccess
{
    public enum DBMS { MSSqlServer, MySql, MariaDB, Postgres, OleDB };

    public class NetCoreDBClient : IDisposable
    {
        private SqlConnection sql_connection;
        private SqlCommand sql_cmd;

        private OleDbConnection oledb_connection;
        private OleDbCommand oledb_cmd;

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
            if (d == DBMS.OleDB)
                DBTYPE = 2;

            ConnStr = connectionstring;
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
            else if (DBTYPE == 2)
            {
                if (this.oledb_connection != null)
                {
                    if (this.oledb_connection.State != ConnectionState.Closed)
                    {
                        this.oledb_connection.Dispose();
                    }

                }

                this.oledb_cmd = null;
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
            else if (DBTYPE == 2)
            {

                oledb_connection = new OleDbConnection();
                oledb_connection.ConnectionString = this.ConnStr;
                oledb_connection.Open();
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
            else if (DBTYPE == 2)
            {
                if (oledb_connection != null)
                {
                    if (this.oledb_connection.State != ConnectionState.Closed)
                    {
                        this.oledb_connection.Close();
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

       

        public void AddParameter(string name, SqlDbType type, object value)
        {

            if (DBTYPE == 1)
            {
                this.sql_cmd.Parameters.Add(name, type).Value = value;
            }
            else if (DBTYPE == 2)
            {
                this.oledb_cmd.Parameters.Add(name, this.GetOleDBType(type)).Value = value;
            }
            else if (DBTYPE == 3)
            {
                this.pgres_cmd.Parameters.Add(name, this.GetPgresDBType(type)).Value = value;
            }
            else if (DBTYPE == 4)
            {
                this.mysql_cmd.Parameters.Add(name, this.GetMysqlDBType(type)).Value = value;
            }
        }

        public void AddParameter(string name, object value)
        {
            if (DBTYPE == 1)
            {
                this.sql_cmd.Parameters.AddWithValue(name, value);
            }
            else if (DBTYPE == 2)
            {
                this.oledb_cmd.Parameters.AddWithValue(name, value);
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

        public void AddParameter(SqlParameter p)
        {
            if (DBTYPE == 1)
            {
                this.sql_cmd.Parameters.Add(p);
            }
            else if (DBTYPE == 2)
            {
                this.oledb_cmd.Parameters.Add(new OleDbParameter() { ParameterName = p.ParameterName, Value = p.Value });
            }
            else if (DBTYPE == 3)
            {
                this.pgres_cmd.Parameters.Add(new NpgsqlParameter() { ParameterName = p.ParameterName, Value = p.Value });
            }
            else if (DBTYPE == 4)
            {
                this.mysql_cmd.Parameters.Add(new MySqlParameter() { ParameterName = p.ParameterName, Value = p.Value });
            }

        }



        public void SetSelectCommandTimeout(Int32 seconds)
        {
            if (DBTYPE == 1)
            {
                this.sql_cmd.CommandTimeout = seconds;
            }
            else if (DBTYPE == 2)
            {
                this.oledb_cmd.CommandTimeout = seconds;
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






        public void SetParameterDirection(String name, ParameterDirection param_direction)
        {
            if (DBTYPE == 1)
                this.sql_cmd.Parameters[name].Direction = param_direction;
            if (DBTYPE == 2)
                this.oledb_cmd.Parameters[name].Direction = param_direction;
            if (DBTYPE == 3)
                this.pgres_cmd.Parameters[name].Direction = param_direction;
            if (DBTYPE == 4)
                this.mysql_cmd.Parameters[name].Direction = param_direction;

        }

        public object GetParameterValue(string name)
        {
            if (DBTYPE == 1)
                return this.sql_cmd.Parameters[name].Value;
            else if (DBTYPE == 2)
                return this.oledb_cmd.Parameters[name].Value;
            else if (DBTYPE == 3)
                return this.pgres_cmd.Parameters[name].Value;
            else if (DBTYPE == 4)
                return this.mysql_cmd.Parameters[name].Value;
            else
                return null;

        }

        public void FillDataset(DataSet ds, string srcTable)
        {


            if (DBTYPE == 1)
            {
                var adapt = new SqlDataAdapter(sql_cmd);
                adapt.MissingMappingAction = MissingMappingAction.Passthrough;
                adapt.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                adapt.Fill(ds, srcTable);
            }
            else if (DBTYPE == 2)
            {
                var adapt = new OleDbDataAdapter(oledb_cmd);
                adapt.MissingMappingAction = MissingMappingAction.Passthrough;
                adapt.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                adapt.Fill(ds, srcTable);
            }
            else if (DBTYPE == 3)
            {
                var adapt = new NpgsqlDataAdapter(pgres_cmd);
                adapt.MissingMappingAction = MissingMappingAction.Passthrough;
                adapt.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                adapt.Fill(ds, srcTable);
            }
            else if (DBTYPE == 4)
            {
                var adapt = new MySqlDataAdapter(mysql_cmd);
                adapt.MissingMappingAction = MissingMappingAction.Passthrough;
                adapt.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                adapt.Fill(ds, srcTable);
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
            else if (DBTYPE == 2)
            {
                var adapt = new OleDbDataAdapter(oledb_cmd);
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
            else if (DBTYPE == 2)
            {
                return oledb_cmd.ExecuteScalar();
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
            else if (DBTYPE == 2)
            {
                return oledb_cmd.ExecuteNonQuery();
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

        public OleDbDataReader ExecuteOleDBDataReader(CommandBehavior cbv)
        {
            if (DBTYPE == 2)
            {
                return oledb_cmd.ExecuteReader(cbv);
            } else {
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

        public SqlCommand GetNewSqlCommand()
        {
            this.sql_cmd = new SqlCommand();
            this.sql_cmd.Connection = this.sql_connection;
            return this.sql_cmd;
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
            else if (DBTYPE == 2)
            {
                if (this.oledb_cmd == null)
                {
                    this.oledb_cmd = new OleDbCommand();
                }
                else
                {
                    this.oledb_cmd.Dispose();
                    this.oledb_cmd = new OleDbCommand();
                }
                this.oledb_cmd.Connection = this.oledb_connection;
                this.oledb_cmd.CommandText = sqlcode;
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

        private OleDbType GetOleDBType(SqlDbType type)
        {
            OleDbType res;
            if (type == SqlDbType.Bit)
            {
                res = OleDbType.Boolean;
            }
            else if (type == SqlDbType.Char)
            {
                res = OleDbType.Char;
            }
            else if (type == SqlDbType.DateTime)
            {
                res = OleDbType.DBTimeStamp;
            }
            else if (type == SqlDbType.Decimal)
            {
                res = OleDbType.Decimal;
            }
            else if (type == SqlDbType.Float)
            {
                res = OleDbType.Double;
            }
            else if (type == SqlDbType.Int)
            {
                res = OleDbType.Integer;
            }
            else if (type == SqlDbType.NText)
            {
                res = OleDbType.LongVarChar;
            }
            else if (type == SqlDbType.NVarChar)
            {
                res = OleDbType.VarWChar;
            }
            else if (type == SqlDbType.Timestamp)
            {
                res = OleDbType.Binary;
            }
            else if (type == SqlDbType.UniqueIdentifier)
            {
                res = OleDbType.Guid;
            }
            else
            {
                res = OleDbType.VarWChar;
            }
            return res;
        }

        private NpgsqlDbType GetPgresDBType(SqlDbType type)
        {
            NpgsqlDbType res;

            if (type == SqlDbType.Bit)
            {
                res = NpgsqlDbType.Bit;
            }
            else if (type == SqlDbType.Char)
            {
                res = NpgsqlDbType.Char;
            }
            else if (type == SqlDbType.DateTime)
            {
                res = NpgsqlDbType.Date;
            }
            else if (type == SqlDbType.Decimal)
            {
                res = NpgsqlDbType.Double;
            }
            else if (type == SqlDbType.Float)
            {
                res = NpgsqlDbType.Double;
            }
            else if (type == SqlDbType.Int)
            {
                res = NpgsqlDbType.Integer;
            }
            else if (type == SqlDbType.NText)
            {
                res = NpgsqlDbType.Text;
            }
            else if (type == SqlDbType.NVarChar)
            {
                res = NpgsqlDbType.Varchar;
            }
            else if (type == SqlDbType.Timestamp)
            {
                res = NpgsqlDbType.Timestamp;
            }
            else if (type == SqlDbType.UniqueIdentifier)
            {
                res = NpgsqlDbType.Timestamp;
            }
            else
            {
                res = NpgsqlDbType.Varchar;
            }


            return res;
        }

        private MySqlDbType GetMysqlDBType(SqlDbType type)
        {
            MySqlDbType res;

            if (type == SqlDbType.Bit)
            {
                res = MySqlDbType.Bit;
            }
            else if (type == SqlDbType.Char)
            {
                res = MySqlDbType.VarChar;
            }
            else if (type == SqlDbType.DateTime)
            {
                res = MySqlDbType.DateTime;
            }
            else if (type == SqlDbType.Decimal)
            {
                res = MySqlDbType.Decimal;
            }
            else if (type == SqlDbType.Float)
            {
                res = MySqlDbType.Float;
            }
            else if (type == SqlDbType.Int)
            {
                res = MySqlDbType.Int32;
            }
            else if (type == SqlDbType.NText)
            {
                res = MySqlDbType.Text;
            }
            else if (type == SqlDbType.NVarChar)
            {
                res = MySqlDbType.VarChar;
            }
            else if (type == SqlDbType.Timestamp)
            {
                res = MySqlDbType.Timestamp;
            }
            else if (type == SqlDbType.UniqueIdentifier)
            {
                res = MySqlDbType.Timestamp;
            }
            else
            {
                res = MySqlDbType.VarChar;
            }


            return res;
        }

      

    

    }
}
