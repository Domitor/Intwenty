using System;
using System.Collections.Generic;

namespace Intwenty.DataClient
{
    public class ResultSet : IResultSet
    {
        public string Name { get; set; }

        public List<IResultSetRow> Rows { get; set; }


        public bool HasRows
        {
            get
            {
                if (Rows == null)
                    return false;

                if (Rows.Count == 0)
                    return false;

                return true;
            }

        }

        public int? FirstRowGetAsInt(string name)
        {
            if (!HasRows)
                return 0;

            var value = this.Rows[0].Values.Find(p => p.Name.ToLower() == name.ToLower());
            if (value == null)
                return 0;
            if (!value.GetAsInt().HasValue)
                return 0;

            return value.GetAsInt().Value;
        }

        public string FirstRowGetAsString(string name)
        {
            if (!HasRows)
                return string.Empty;

            var value = this.Rows[0].Values.Find(p => p.Name.ToLower() == name.ToLower());
            if (value == null)
                return string.Empty;

            return value.GetAsString();
        }

        public bool? FirstRowGetAsBool(string name)
        {
            if (!HasRows)
                return null;

            var value = this.Rows[0].Values.Find(p => p.Name.ToLower() == name.ToLower());
            if (value == null)
                return null;

            return value.GetAsBool();
        }

        public decimal? FirstRowGetAsDecimal(string name)
        {
            if (!HasRows)
                return null;

            var value = this.Rows[0].Values.Find(p => p.Name.ToLower() == name.ToLower());
            if (value == null)
                return null;

            return value.GetAsDecimal();
        }

        public DateTime? FirstRowGetAsDateTime(string name)
        {
            if (!HasRows)
                return null;

            var value = this.Rows[0].Values.Find(p => p.Name.ToLower() == name.ToLower());
            if (value == null)
                return null;

            return value.GetAsDateTime();
        }


        public ResultSet()
        {
            Rows = new List<IResultSetRow>();
        }

    }



    public class ResultSetRow : IResultSetRow
    {

        public List<IResultSetValue> Values { get; set; }

        public ResultSetRow()
        {
            Values = new List<IResultSetValue>();
        }

        public int? GetAsInt(string name)
        {
            var value = this.Values.Find(p => p.Name.ToLower() == name.ToLower());
            if (value == null)
                return 0;
            if (!value.GetAsInt().HasValue)
                return 0;

            return value.GetAsInt().Value;
        }

        public string GetAsString(string name)
        {
            var value = this.Values.Find(p => p.Name.ToLower() == name.ToLower());
            if (value == null)
                return string.Empty;

            return value.GetAsString();
        }

        public bool? GetAsBool(string name)
        {
            var value = this.Values.Find(p => p.Name.ToLower() == name.ToLower());
            if (value == null)
                return null;

            return value.GetAsBool();
        }

        public decimal? GetAsDecimal(string name)
        {
            var value = this.Values.Find(p => p.Name.ToLower() == name.ToLower());
            if (value == null)
                return null;

            return value.GetAsDecimal();
        }

        public DateTime? GetAsDateTime(string name)
        {
            var value = this.Values.Find(p => p.Name.ToLower() == name.ToLower());
            if (value == null)
                return null;

            return value.GetAsDateTime();
        }

        public void SetValue(string name, object value)
        {
            var t = this.Values.Find(p => p.Name.ToLower() == name.ToLower());
            if (t != null)
            {
                t.SetValue(value);
            }
            else
            {
                this.Values.Add(new ResultSetValue() { Name = name, Value = value });
            }
        }

    }

    public class ResultSetValue : IResultSetValue 
    {
        public string Name { get; set; }

        public object Value { get; set; }


        public ResultSetValue()
        {
            Name = string.Empty;
            Value = string.Empty;
        }

        public void SetValue(object value)
        {
            Value = value;
        }

        public int? GetAsInt()
        {
            if (HasValue)
                return Convert.ToInt32(Value);

            return null;
        }

        public string GetAsString()
        {
            if (HasValue)
                return Convert.ToString(Value);

            return string.Empty;
        }

        public bool? GetAsBool()
        {
            if (HasValue)
                return Convert.ToBoolean(Value);

            return null;
        }

        public decimal? GetAsDecimal()
        {
            if (HasValue)
                return Convert.ToDecimal(Value);

            return null;
        }

        public DateTime? GetAsDateTime()
        {
            if (HasValue)
                return Convert.ToDateTime(Value);

            return null;
        }

        public bool HasValue
        {
            get
            {
                if (Value == null)
                    return false;

                if (Value == DBNull.Value)
                    return false;

                return true;

            }

        }
    }


}
