using System;
using System.Collections.Generic;

namespace Intwenty.DataClient
{

    public class ResultSet : ResultSetValueBase
    {

        public string Name { get; set; }

        public List<ResultSetTable> SubTables { get; set; }

        public System.Text.Json.JsonElement JSON { get; set; }

        public ResultSet()
        {
            Values = new List<ResultSetValue>();
            SubTables = new List<ResultSetTable>();
        }

        public bool HasData
        {
            get
            {
                return Values.Count > 0;
            }
        }

     
        public static ResultSet CreateFromJSON(System.Text.Json.JsonElement JSON)
        {
            var res = new ResultSet();

            if (JSON.ValueKind == System.Text.Json.JsonValueKind.Object)
            {
                res.JSON = JSON;
                var jsonarr = JSON.EnumerateObject();
                foreach (var j in jsonarr)
                {
                    if (j.Value.ValueKind == System.Text.Json.JsonValueKind.String || j.Value.ValueKind == System.Text.Json.JsonValueKind.Undefined)
                        res.Values.Add(new ResultSetValue() { Name = j.Name, Value = j.Value.GetString() });
                    else if (j.Value.ValueKind == System.Text.Json.JsonValueKind.Number)
                        res.Values.Add(new ResultSetValue() { Name = j.Name, Value = j.Value.GetDecimal() });
                    else if (j.Value.ValueKind == System.Text.Json.JsonValueKind.False || j.Value.ValueKind == System.Text.Json.JsonValueKind.True)
                        res.Values.Add(new ResultSetValue() { Name = j.Name, Value = j.Value.GetBoolean() });
                    else if (j.Value.ValueKind == System.Text.Json.JsonValueKind.Object)
                    {
                        res.Name = j.Name;
                        var jsonobjarr = j.Value.EnumerateObject();
                        foreach (var av in jsonobjarr)
                        {
                            if (av.Value.ValueKind == System.Text.Json.JsonValueKind.String || av.Value.ValueKind == System.Text.Json.JsonValueKind.Undefined)
                                res.Values.Add(new ResultSetValue() { Name = av.Name, Value = av.Value.GetString() });
                            if (av.Value.ValueKind == System.Text.Json.JsonValueKind.Number)
                                res.Values.Add(new ResultSetValue() { Name = av.Name, Value = av.Value.GetDecimal() });
                            if (av.Value.ValueKind == System.Text.Json.JsonValueKind.False || av.Value.ValueKind == System.Text.Json.JsonValueKind.True)
                                res.Values.Add(new ResultSetValue() { Name = av.Name, Value = av.Value.GetBoolean() });

                        }
                    }
                    else if (j.Value.ValueKind == System.Text.Json.JsonValueKind.Array)
                    {
                        var tabledata = new ResultSetTable() { Name = j.Name };
                        res.SubTables.Add(tabledata);
                        var jsonrows = j.Value.EnumerateArray();
                        foreach (var row in jsonrows)
                        {
                            var tablerow = new ResultSetTableRow() { Table = tabledata };
                            tabledata.Rows.Add(tablerow);
                            var jsonobjarr = row.EnumerateObject();
                            foreach (var av in jsonobjarr)
                            {
                                if (av.Value.ValueKind == System.Text.Json.JsonValueKind.String || av.Value.ValueKind == System.Text.Json.JsonValueKind.Undefined)
                                    tablerow.Values.Add(new ResultSetValue() { Name = av.Name, Value = av.Value.GetString() });
                                if (av.Value.ValueKind == System.Text.Json.JsonValueKind.Number)
                                    tablerow.Values.Add(new ResultSetValue() { Name = av.Name, Value = av.Value.GetDecimal() });
                                if (av.Value.ValueKind == System.Text.Json.JsonValueKind.False || av.Value.ValueKind == System.Text.Json.JsonValueKind.True)
                                    tablerow.Values.Add(new ResultSetValue() { Name = av.Name, Value = av.Value.GetBoolean() });
                            }
                           
                        }

                    }
                }

            }
            else if (JSON.ValueKind == System.Text.Json.JsonValueKind.Array)
            {
                var tabledata = new ResultSetTable() { Name = "List" };
                res.SubTables.Add(tabledata);
                var jsonrows = JSON.EnumerateArray();
                foreach (var row in jsonrows)
                {
                    var tablerow = new ResultSetTableRow() { Table = tabledata };
                    tabledata.Rows.Add(tablerow);
                    var jsonobjarr = row.EnumerateObject();
                    foreach (var av in jsonobjarr)
                    {
                        if (av.Value.ValueKind == System.Text.Json.JsonValueKind.String || av.Value.ValueKind == System.Text.Json.JsonValueKind.Undefined)
                            tablerow.Values.Add(new ResultSetValue() { Name = av.Name, Value = av.Value.GetString() });
                        if (av.Value.ValueKind == System.Text.Json.JsonValueKind.Number)
                            tablerow.Values.Add(new ResultSetValue() { Name = av.Name, Value = av.Value.GetDecimal() });
                        if (av.Value.ValueKind == System.Text.Json.JsonValueKind.False || av.Value.ValueKind == System.Text.Json.JsonValueKind.True)
                            tablerow.Values.Add(new ResultSetValue() { Name = av.Name, Value = av.Value.GetBoolean() });
                    }
                  
                }
            }


            return res;



        }

      

    }


    public class ResultSetValue
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

    public class ResultSetTable
    {
        public string Name { get; set; }

        public List<ResultSetTableRow> Rows { get; set; }


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


        public ResultSetTable()
        {
            Rows = new List<ResultSetTableRow>();
        }

    }

    public class ResultSetTableRow : ResultSetValueBase
    {

        public int ParentId { get; set; }

        public ResultSetTable Table { get; set; }

        public ResultSetTableRow()
        {
            Values = new List<ResultSetValue>();
        }


    }

    public class ResultSetValueBase
    {

        public List<ResultSetValue> Values { get; set; }

        public ResultSetValueBase()
        {
           
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

}
