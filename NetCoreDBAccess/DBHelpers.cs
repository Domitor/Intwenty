using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;

namespace NetCoreDBAccess
{
    public static class DBHelpers
    {

        public static bool GetRowBoolValue(System.Data.DataRow r, string columnname)
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

        public static Double GetRowDoubleValue(System.Data.DataRow r, string columnname)
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

        public static Decimal GetRowDecimalValue(System.Data.DataRow r, string columnname)
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

        public static Decimal GetRowDecimalValue(System.Data.DataSet ds, string columnname)
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

        public static string GetRowDateStringValue(System.Data.DataRow r, string columnname)
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

        public static string GetRowStringValue(System.Data.DataRow r, string columnname)
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

     

     

        public static string GetRowStringValue(System.Data.DataTable t, string columnname)
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

        public static string GetRowStringValue(System.Data.DataSet t, string columnname)
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

        public static Int64 GetRowInt64Value(System.Data.DataRow r, string columnname)
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

        public static Int32 GetRowIntValue(System.Data.DataRow r, string columnname)
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

     

        public static Int32 GetRowIntValue(System.Data.DataTable t, string columnname)
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

      

        public static DateTime? GetRowDateTimeValue(System.Data.DataRow r, string columnname)
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

        public static DateTime? GetRowDateTimeValue(System.Data.DataSet ds, string columnname)
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

        public static string GetJSONValue(DataRow r, DataColumn c)
        {

            if (r == null || c == null)
                return string.Empty;

            if (r[c] == null)
                return string.Empty;

            if (r[c] == DBNull.Value)
                return string.Empty;

            if (IsNumeric(c))
            {
                return "\"" + c.ColumnName + "\":" + Convert.ToString(r[c]).Replace(",", ".");
            }
            else if (IsDateTime(c))
            {
                return "\"" + c.ColumnName + "\":" + "\"" + System.Text.Json.JsonEncodedText.Encode(Convert.ToDateTime(r[c]).ToString("yyyy-MM-dd")).ToString() + "\"";
            }
            else
            {
                return "\"" + c.ColumnName + "\":" + "\"" + System.Text.Json.JsonEncodedText.Encode(Convert.ToString(r[c])).ToString() + "\"";
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



        /*
       public static System.Data.DataSet GetDataSet(string sql, System.Data.Common.DbConnection connection, bool useprocedure)
       {

           var ds = new System.Data.DataSet();
           var cn = new System.Data.SqlClient.SqlConnection(connection.ConnectionString);
           cn.Open();

           var sqlcmd = new System.Data.SqlClient.SqlCommand();
           sqlcmd.Connection = cn;
           if (useprocedure)
               sqlcmd.CommandType = System.Data.CommandType.StoredProcedure;

           sqlcmd.CommandText = sql;


           var sqladapter = new System.Data.SqlClient.SqlDataAdapter();
           sqladapter.SelectCommand = sqlcmd;
           sqladapter.Fill(ds);

           cn.Close();

           return ds;

       }

       public static System.Data.DataSet GetDataSet(string sql, System.Data.Common.DbConnection connection, bool useprocedure, List<System.Data.SqlClient.SqlParameter> sqlparams)
       {

           var ds = new System.Data.DataSet();
           var cn = new System.Data.SqlClient.SqlConnection(connection.ConnectionString);
           cn.Open();

           var sqlcmd = new System.Data.SqlClient.SqlCommand();
           sqlcmd.Connection = cn;
           if (useprocedure)
               sqlcmd.CommandType = System.Data.CommandType.StoredProcedure;

           sqlcmd.CommandText = sql;

           if (sqlparams != null)
           {
               foreach (var l in sqlparams)
               {
                   sqlcmd.Parameters.Add(l);
               }
           }

           var sqladapter = new System.Data.SqlClient.SqlDataAdapter();
           sqladapter.SelectCommand = sqlcmd;
           sqladapter.Fill(ds);


           cn.Close();

           return ds;

       }


       public static int ExecuteNonQuery(string sql, System.Data.Common.DbConnection connection, bool useprocedure, List<System.Data.SqlClient.SqlParameter> sqlparams)
       {
           int res = 0;

           var cn = new System.Data.SqlClient.SqlConnection(connection.ConnectionString);
           cn.Open();

           var sqlcmd = new System.Data.SqlClient.SqlCommand();
           sqlcmd.Connection = cn;
           if (useprocedure)
               sqlcmd.CommandType = System.Data.CommandType.StoredProcedure;

           if (sqlparams != null)
           {
               foreach (var l in sqlparams)
               {
                   sqlcmd.Parameters.Add(l);
               }
           }

           sqlcmd.CommandText = sql;

           res = sqlcmd.ExecuteNonQuery();

           cn.Close();

           return res;

       }



       public static object ExecuteScalar(string sql, System.Data.Common.DbConnection connection, bool useprocedure, List<System.Data.SqlClient.SqlParameter> sqlparams)
       {
           object res = null;

           var cn = new System.Data.SqlClient.SqlConnection(connection.ConnectionString);
           cn.Open();

           var sqlcmd = new System.Data.SqlClient.SqlCommand();
           sqlcmd.Connection = cn;
           if (useprocedure)
               sqlcmd.CommandType = System.Data.CommandType.StoredProcedure;

           if (sqlparams != null)
           {
               foreach (var l in sqlparams)
               {
                   sqlcmd.Parameters.Add(l);
               }
           }

           sqlcmd.CommandText = sql;

           res = sqlcmd.ExecuteScalar();

           cn.Close();

           return res;

       }

     */




    }

}