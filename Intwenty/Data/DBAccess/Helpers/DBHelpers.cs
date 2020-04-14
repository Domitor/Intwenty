using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Intwenty.Data.DBAccess.Helpers
{
    public enum StringLength { Standard, Long, Short };

    public enum DBMS { MSSqlServer, MySql, MariaDB, PostgreSQL, SQLite, MongoDb };

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

    public interface IIntwentyDataColum
    {
        string ColumnName { get; }
        bool IsNumeric { get; }
        bool IsDateTime { get; }
    }

    public interface IIntwentySqlDbDataColum : IIntwentyDataColum
    {
        DataColumn QueryResultColumn { get; set; }
    }

  

    public class IntwentyDataColum : IIntwentyDataColum, IIntwentySqlDbDataColum
    {
        public string ColumnName { get; set; }
        public bool IsNumeric { get; set; }
        public bool IsDateTime { get; set; }
        public DataColumn QueryResultColumn { get; set; }

    }


    public static class DBHelpers
    {

        public static bool GetRowBoolValue(DataRow r, string columnname)
        {
            bool res = false;

            try
            {
                if (r[columnname] == DBNull.Value)
                    return res;

                res = Convert.ToBoolean(r[columnname]);
            }
            catch
            {
                res = false;
            }

            return res;
        }

        public static Double GetRowDoubleValue(DataRow r, string columnname)
        {
            var res = 0.00;

            try
            {
                if (r[columnname] == DBNull.Value)
                    return res;

                res = Math.Round(Convert.ToDouble(r[columnname]), 2);
            }
            catch
            {
                res = 0.00;
            }

            return res;
        }

        public static Decimal GetRowDecimalValue(DataRow r, string columnname)
        {
            var res = 0.00M;

            try
            {
                if (r[columnname] == DBNull.Value)
                    return res;

                res = Convert.ToDecimal(r[columnname]);
            }
            catch
            {
                res = 0.00M;
            }

            return res;
        }

        public static Decimal GetRowDecimalValue(DataSet ds, string columnname)
        {
            var res = 0.00M;

            try
            {
                if (ds.Tables[0].Rows[0][columnname] == DBNull.Value)
                    return res;

                res = Convert.ToDecimal(ds.Tables[0].Rows[0][columnname]);
            }
            catch
            {
                res = 0.00M;
            }

            return res;
        }

        public static string GetRowDateStringValue(DataRow r, string columnname)
        {
            var res = string.Empty;

            try
            {
                if (r[columnname] == DBNull.Value)
                    return res;

                res = Convert.ToDateTime(r[columnname]).ToString("yyyy-MM-dd");
            }
            catch
            {
                res = string.Empty;
            }

            return res;
        }

        public static string GetRowStringValue(DataRow r, string columnname)
        {
            var res = string.Empty;

            try
            {
                if (r[columnname] == DBNull.Value)
                    return res;

                res = Convert.ToString(r[columnname]);
            }
            catch
            {
                res = string.Empty;
            }

            return res;
        }

     

     

        public static string GetRowStringValue(DataTable t, string columnname)
        {
            var res = string.Empty;

            try
            {
                if (t.Rows[0][columnname] == DBNull.Value)
                    return res;

                res = Convert.ToString(t.Rows[0][columnname]);
            }
            catch
            {
                res = string.Empty;
            }

            return res;
        }

        public static string GetRowStringValue(DataSet t, string columnname)
        {
            var res = string.Empty;

            try
            {
                if (t.Tables[0].Rows[0][columnname] == DBNull.Value)
                    return res;

                res = Convert.ToString(t.Tables[0].Rows[0][columnname]);
            }
            catch
            {
                res = string.Empty;
            }

            return res;
        }

        public static Int64 GetRowInt64Value(DataRow r, string columnname)
        {
            Int64 res = 0;

            try
            {
                if (r[columnname] == DBNull.Value)
                    return res;

                res = Convert.ToInt64(r[columnname]);
            }
            catch
            {
                res = 0;
            }

            return res;
        }

        public static Int32 GetRowIntValue(DataRow r, string columnname)
        {
            int res = 0;

            try
            {
                if (r[columnname] == DBNull.Value)
                    return res;

                res = Convert.ToInt32(r[columnname]);
            }
            catch
            {
                res = 0;
            }

            return res;
        }

     

        public static Int32 GetRowIntValue(DataTable t, string columnname)
        {
            int res = 0;

            try
            {
                if (t.Rows[0][columnname] == DBNull.Value)
                    return res;

                res = Convert.ToInt32(t.Rows[0][columnname]);
            }
            catch
            {
                res = 0;
            }

            return res;
        }

      

        public static DateTime? GetRowDateTimeValue(DataRow r, string columnname)
        {

            DateTime? res;

            try
            {
                if (r[columnname] == DBNull.Value)
                    return null;

                res = Convert.ToDateTime(r[columnname]);
            }
            catch
            {
                res = null;
            }

            return res;
        }

        public static DateTime? GetRowDateTimeValue(DataSet ds, string columnname)
        {

            DateTime? res;

            try
            {
                if (ds.Tables[0].Rows[0][columnname] == DBNull.Value)
                    return null;

                res = Convert.ToDateTime(ds.Tables[0].Rows[0][columnname]);
            }
            catch
            {
                res = null;
            }

            return res;
        }

        public static int GetAsInt(object obj)
        {
            if (obj == null)
                return 0;

            if (obj == DBNull.Value)
                return 0;

            try
            {
                return Convert.ToInt32(obj);
            }
            catch { }

            return 0;
        }

        public static DateTime? GetAsDateTime(object obj)
        {
            if (obj == null)
                return null;

            if (obj == DBNull.Value)
                return null;

            try
            {
                return Convert.ToDateTime(obj);
            }
            catch { }

            return null;
        }

        public static string GetJSONValue(string name, int value)
        {
            return "\"" + name + "\":" + Convert.ToString(value).Replace(",", ".");
        }

        public static string GetJSONValue(string name, string value)
        {
            return "\"" + name + "\":" + "\"" + System.Text.Json.JsonEncodedText.Encode(value).ToString() + "\"";
        }

        public static string GetJSONValue(string name, DateTime value)
        {
            return "\"" + name + "\":" + "\"" + System.Text.Json.JsonEncodedText.Encode(value.ToString("yyyy-MM-dd")).ToString() + "\"";
        }

        public static string GetJSONValue(object value, IIntwentyDataColum column)
        {

            if (column.IsNumeric)
            {
                return "\"" + column.ColumnName + "\":" + Convert.ToString(value).Replace(",", ".");
            }
            else if (column.IsDateTime)
            {
                return "\"" + column.ColumnName + "\":" + "\"" + System.Text.Json.JsonEncodedText.Encode(Convert.ToDateTime(value).ToString("yyyy-MM-dd")).ToString() + "\"";
            }
            else
            {
                return "\"" + column.ColumnName + "\":" + "\"" + System.Text.Json.JsonEncodedText.Encode(Convert.ToString(value)).ToString() + "\"";
            }
        }


        public static string GetJSONValue(DataRow r, DataColumn dc)
        {
            if (r == null || dc == null)
                return string.Empty;

            if (r[dc] == null)
                return string.Empty;

            if (r[dc] == DBNull.Value)
                return string.Empty;


            if (IsNumeric(dc))
            {
                return "\"" + dc.ColumnName + "\":" + Convert.ToString(r[dc]).Replace(",", ".");
            }
            else if (IsDateTime(dc))
            {
                return "\"" + dc.ColumnName + "\":" + "\"" + System.Text.Json.JsonEncodedText.Encode(Convert.ToDateTime(r[dc]).ToString("yyyy-MM-dd")).ToString() + "\"";
            }
            else
            {
                return "\"" + dc.ColumnName + "\":" + "\"" + System.Text.Json.JsonEncodedText.Encode(Convert.ToString(r[dc])).ToString() + "\"";
            }
        }

       


        public static bool IsNumeric(DataColumn col)
        {
            if (col == null)
                return false;

            var numericTypes = new[] { typeof(byte), typeof(decimal), typeof(double),
            typeof(short), typeof(int), typeof(long), typeof(sbyte),
            typeof(float), typeof(ushort), typeof(uint), typeof(ulong)};

            return numericTypes.Contains(col.DataType);
        }

       

        public static bool IsDateTime(DataColumn col)
        {
            if (col == null)
                return false;

            var dateTimeTypes = new[] { typeof(DateTime) };

            return dateTimeTypes.Contains(col.DataType);
        }

      


        public static List<DBMSCommandMap> GetDBMSCommandMap()
        {
            var res = new List<DBMSCommandMap>();

            res.Add(new DBMSCommandMap() { Key=  "AUTOINC",   DBMSType = DBMS.MSSqlServer, Command = "IDENTITY(1,1)" });
            res.Add(new DBMSCommandMap() { Key = "AUTOINC", DBMSType = DBMS.MariaDB, Command = "AUTO_INCREMENT" });
            res.Add(new DBMSCommandMap() { Key = "AUTOINC", DBMSType = DBMS.MySql, Command = "AUTO_INCREMENT" });
            res.Add(new DBMSCommandMap() { Key = "AUTOINC", DBMSType = DBMS.PostgreSQL, Command = "SERIAL" });
            res.Add(new DBMSCommandMap() { Key = "AUTOINC", DBMSType = DBMS.SQLite, Command = "PRIMARY KEY AUTOINCREMENT" });
            res.Add(new DBMSCommandMap() { Key = "GETDATE", DBMSType = DBMS.MSSqlServer, Command = "GETDATE()" });
            res.Add(new DBMSCommandMap() { Key = "GETDATE", DBMSType = DBMS.MariaDB, Command = "NOW()" });
            res.Add(new DBMSCommandMap() { Key = "GETDATE", DBMSType = DBMS.MySql, Command = "NOW()" });
            res.Add(new DBMSCommandMap() { Key = "GETDATE", DBMSType = DBMS.PostgreSQL, Command = "now()" });
            res.Add(new DBMSCommandMap() { Key = "GETDATE", DBMSType = DBMS.SQLite, Command = "DATETIME('now', 'localtime')" });

         

            return res;
        }
        public static List<SqlDataTypeMap> GetDataTypeMap()
        {
            var res = new List<SqlDataTypeMap>();


            res.Add(new SqlDataTypeMap() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DBMSType = DBMS.MSSqlServer , DBMSDataType ="NVARCHAR(300)"  });
            res.Add(new SqlDataTypeMap() { IntwentyType = "TEXT", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DBMSType = DBMS.MSSqlServer, DBMSDataType = "NVARCHAR(max)", Length = StringLength.Long });
            res.Add(new SqlDataTypeMap() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DBMSType = DBMS.MSSqlServer, DBMSDataType = "NVARCHAR(30)", Length = StringLength.Short });

            res.Add(new SqlDataTypeMap() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DBMSType = DBMS.MariaDB, DBMSDataType = "VARCHAR(300)" });
            res.Add(new SqlDataTypeMap() { IntwentyType = "TEXT", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DBMSType = DBMS.MariaDB, DBMSDataType = "LONGTEXT", Length = StringLength.Long });
            res.Add(new SqlDataTypeMap() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DBMSType = DBMS.MariaDB, DBMSDataType = "VARCHAR(30)", Length = StringLength.Short });

            res.Add(new SqlDataTypeMap() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DBMSType = DBMS.MySql, DBMSDataType = "VARCHAR(300)" });
            res.Add(new SqlDataTypeMap() { IntwentyType = "TEXT", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DBMSType = DBMS.MySql, DBMSDataType = "LONGTEXT", Length = StringLength.Long });
            res.Add(new SqlDataTypeMap() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DBMSType = DBMS.MySql, DBMSDataType = "VARCHAR(30)", Length = StringLength.Short });

            res.Add(new SqlDataTypeMap() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DBMSType = DBMS.PostgreSQL, DBMSDataType = "VARCHAR(300)" });
            res.Add(new SqlDataTypeMap() { IntwentyType = "TEXT", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DBMSType = DBMS.PostgreSQL, DBMSDataType = "TEXT", Length = StringLength.Long });
            res.Add(new SqlDataTypeMap() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DBMSType = DBMS.PostgreSQL, DBMSDataType = "VARCHAR(30)", Length = StringLength.Short });
            
            res.Add(new SqlDataTypeMap() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DBMSType = DBMS.SQLite, DBMSDataType = "TEXT" });
            res.Add(new SqlDataTypeMap() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DBMSType = DBMS.SQLite, DBMSDataType = "TEXT", Length = StringLength.Short });
            res.Add(new SqlDataTypeMap() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DBMSType = DBMS.SQLite, DBMSDataType = "TEXT", Length = StringLength.Long });


            res.Add(new SqlDataTypeMap() { IntwentyType = "BOOLEAN", NetType = "SYSTEM.BOOLEAN", DataDbType = DbType.Boolean, DBMSType = DBMS.MSSqlServer, DBMSDataType = "INT" });
            res.Add(new SqlDataTypeMap() { IntwentyType = "BOOLEAN", NetType = "SYSTEM.BOOLEAN", DataDbType = DbType.Boolean, DBMSType = DBMS.MariaDB, DBMSDataType = "TINYINT(1)" });
            res.Add(new SqlDataTypeMap() { IntwentyType = "BOOLEAN", NetType = "SYSTEM.BOOLEAN", DataDbType = DbType.Boolean, DBMSType = DBMS.MySql, DBMSDataType = "TINYINT(1)" });
            res.Add(new SqlDataTypeMap() { IntwentyType = "BOOLEAN", NetType = "SYSTEM.BOOLEAN", DataDbType = DbType.Boolean, DBMSType = DBMS.PostgreSQL, DBMSDataType = "BOOLEAN" });
            res.Add(new SqlDataTypeMap() { IntwentyType = "BOOLEAN", NetType = "SYSTEM.BOOLEAN", DataDbType = DbType.Boolean, DBMSType = DBMS.SQLite, DBMSDataType = "INTEGER" });

            res.Add(new SqlDataTypeMap() { IntwentyType = "INTEGER", NetType = "SYSTEM.INT32", DataDbType = DbType.Int32, DBMSType = DBMS.MSSqlServer, DBMSDataType = "INT" });
            res.Add(new SqlDataTypeMap() { IntwentyType = "INTEGER", NetType = "SYSTEM.INT32", DataDbType = DbType.Int32, DBMSType = DBMS.MariaDB, DBMSDataType = "INTEGER(11)" });
            res.Add(new SqlDataTypeMap() { IntwentyType = "INTEGER", NetType = "SYSTEM.INT32", DataDbType = DbType.Int32, DBMSType = DBMS.MySql, DBMSDataType = "INTEGER(11)" });
            res.Add(new SqlDataTypeMap() { IntwentyType = "INTEGER", NetType = "SYSTEM.INT32", DataDbType = DbType.Int32, DBMSType = DBMS.PostgreSQL, DBMSDataType = "INTEGER" });
            res.Add(new SqlDataTypeMap() { IntwentyType = "INTEGER", NetType = "SYSTEM.INT32", DataDbType = DbType.Int32, DBMSType = DBMS.SQLite, DBMSDataType = "INTEGER" });

            res.Add(new SqlDataTypeMap() { IntwentyType = "DATETIME", NetType = "SYSTEM.DATETIME", DataDbType = DbType.DateTime, DBMSType = DBMS.MSSqlServer, DBMSDataType = "DATETIME" });
            res.Add(new SqlDataTypeMap() { IntwentyType = "DATETIME", NetType = "SYSTEM.DATETIME", DataDbType = DbType.DateTime, DBMSType = DBMS.MariaDB, DBMSDataType = "DATETIME" });
            res.Add(new SqlDataTypeMap() { IntwentyType = "DATETIME", NetType = "SYSTEM.DATETIME", DataDbType = DbType.DateTime, DBMSType = DBMS.MySql, DBMSDataType = "DATETIME" });
            res.Add(new SqlDataTypeMap() { IntwentyType = "DATETIME", NetType = "SYSTEM.DATETIME", DataDbType = DbType.DateTime, DBMSType = DBMS.PostgreSQL, DBMSDataType = "TIMESTAMP" });
            res.Add(new SqlDataTypeMap() { IntwentyType = "DATETIME", NetType = "SYSTEM.DATETIME", DataDbType = DbType.DateTime, DBMSType = DBMS.SQLite, DBMSDataType = "DATETIME" });

            res.Add(new SqlDataTypeMap() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DBMSType = DBMS.MSSqlServer, DBMSDataType = "DECIMAL(18,1)" });
            res.Add(new SqlDataTypeMap() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DBMSType = DBMS.MariaDB, DBMSDataType = "DECIMAL(18,1)DECIMAL(18,1)" });
            res.Add(new SqlDataTypeMap() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DBMSType = DBMS.MySql, DBMSDataType = "DECIMAL(18,1)" });
            res.Add(new SqlDataTypeMap() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DBMSType = DBMS.PostgreSQL, DBMSDataType = "NUMERIC(18,1)" });
            res.Add(new SqlDataTypeMap() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DBMSType = DBMS.SQLite, DBMSDataType = "REAL" });


            res.Add(new SqlDataTypeMap() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DBMSType = DBMS.MSSqlServer, DBMSDataType = "DECIMAL(18,2)" });
            res.Add(new SqlDataTypeMap() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DBMSType = DBMS.MariaDB, DBMSDataType = "DECIMAL(18,2)" });
            res.Add(new SqlDataTypeMap() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DBMSType = DBMS.MySql, DBMSDataType = "DECIMAL(18,2)" });
            res.Add(new SqlDataTypeMap() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DBMSType = DBMS.PostgreSQL, DBMSDataType = "NUMERIC(18,2)" });
            res.Add(new SqlDataTypeMap() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DBMSType = DBMS.SQLite, DBMSDataType = "REAL" });

            res.Add(new SqlDataTypeMap() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DBMSType = DBMS.MSSqlServer, DBMSDataType = "DECIMAL(18,3)" });
            res.Add(new SqlDataTypeMap() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DBMSType = DBMS.MariaDB, DBMSDataType = "DECIMAL(18,3)" });
            res.Add(new SqlDataTypeMap() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.DATETIME", DataDbType = DbType.Decimal, DBMSType = DBMS.MySql, DBMSDataType = "DECIMAL(18,3)" });
            res.Add(new SqlDataTypeMap() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DBMSType = DBMS.PostgreSQL, DBMSDataType = "NUMERIC(18,3)" });
            res.Add(new SqlDataTypeMap() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DBMSType = DBMS.SQLite, DBMSDataType = "REAL" });

            return res;
        }
      



    }


    public class SqlDataTypeMap
    {

        public StringLength Length { get; set; }

        public string NetType { get; set; }

        public string IntwentyType { get; set; }

        public DbType DataDbType { get; set; }

        public DBMS DBMSType { get; set; }

        public string DBMSDataType { get; set; }

        public SqlDataTypeMap()
        {
            Length = StringLength.Standard;
        }


    }

    public class DBMSCommandMap
    {
        public string Key { get; set; }

        public DBMS DBMSType { get; set; }

        public string Command{ get; set; }

        public DBMSCommandMap()
        {
            Command = "";
        }


    }

}