using System;
using Intwenty.Model;
using System.Collections.Generic;

namespace Intwenty.Data.Dto
{
    public class ApplicationValue
    {
        public string DbName { get; set; }

        public object Value { get; set; }

        public DatabaseModelItem Model { get; set; }

        public ApplicationValue()
        {
            DbName = string.Empty;
            Value = string.Empty;
        }

        public bool HasModel
        {
            get
            {
                return Model != null;
            }
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

    public class ApplicationTable
    {
        public string DbName { get; set; }

        public List<ApplicationTableRow> Rows { get; set; }

        public DatabaseModelItem Model { get; set; }

        public bool HasModel
        {
            get
            {
                return Model != null;
            }
        }

        public int GetFirstRowIntValue(string dbname)
        {
            if (Rows == null)
                return 0;
            if (Rows.Count == 0)
                return 0;

            var value = Rows[0].Values.Find(p => p.DbName.ToLower() == dbname.ToLower());
            if (value == null)
                return 0;
            if (!value.GetAsInt().HasValue)
                return 0;

            return value.GetAsInt().Value;
        }

        public string GetFirstRowStringValue(string dbname)
        {
            if (Rows == null)
                return string.Empty;
            if (Rows.Count == 0)
                return string.Empty;

            var value = Rows[0].Values.Find(p => p.DbName.ToLower() == dbname.ToLower());
            if (value == null)
                return string.Empty;

            return value.GetAsString();
        }

        public ApplicationTable()
        {
            Rows = new List<ApplicationTableRow>();
        }

    }

    public class ApplicationTableRow
    {
        public int Id { get; set; }

        public int Version { get; set; }

        public int ParentId { get; set; }

        public ApplicationTable Table { get; set; }

        public List<ApplicationValue> Values { get; set; }

        public ApplicationTableRow()
        {
            Values = new List<ApplicationValue>();
        }

        public int GetIntValue(string dbname)
        {
           
            var value = this.Values.Find(p => p.DbName.ToLower() == dbname.ToLower());
            if (value == null)
                return 0;
            if (!value.GetAsInt().HasValue)
                return 0;

            return value.GetAsInt().Value;
        }

        public string GetStringValue(string dbname)
        {
            var value = this.Values.Find(p => p.DbName.ToLower() == dbname.ToLower());
            if (value == null)
                return string.Empty;

            return value.GetAsString();
        }

    }
}
