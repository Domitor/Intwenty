using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Intwenty.MetaDataService.Model
{
    public class ApplicationData
    {
        public int ApplicationId { get; set; }

        public int Id { get; set; }

        public int Version { get; set; }

        public List<ApplicationDataValue> MainTable { get; set; }

        public List<ApplicationTable> SubTables { get; set; }

        public bool HasData
        {
            get
            {
                return MainTable.Count > 0;
            }

        }

        public ApplicationData()
        {
            MainTable = new List<ApplicationDataValue>();
            SubTables = new List<ApplicationTable>();
        }

        public static ApplicationData CreateFromJSON(System.Text.Json.JsonElement model)
        {
            var res = new ApplicationData();

            var jsonarr = model.EnumerateObject();
            foreach (var j in jsonarr)
            {
                if (j.Value.ValueKind == System.Text.Json.JsonValueKind.Object)
                {
                    var jsonobjarr = j.Value.EnumerateObject();
                    foreach (var av in jsonobjarr)
                    {
                        if (av.Value.ValueKind == System.Text.Json.JsonValueKind.Number)
                            res.MainTable.Add(new ApplicationDataValue() { Code=av.Name, Value=av.Value.GetDecimal(), DataType=DatabaseModelItem.DataType2Decimal });
                        if (av.Value.ValueKind == System.Text.Json.JsonValueKind.String || av.Value.ValueKind == System.Text.Json.JsonValueKind.Undefined)
                            res.MainTable.Add(new ApplicationDataValue() { Code = av.Name, Value = av.Value.GetString(), DataType = DatabaseModelItem.DataTypeString });
                    }
                }
                if (j.Value.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    var tabledata = new ApplicationTable() { Code = j.Name };
                    res.SubTables.Add(tabledata);
                    var jsonrows= j.Value.EnumerateObject();
                    foreach (var row in jsonrows)
                    {
                        var tablerow = new ApplicationTableRow() { Table = tabledata };
                        tabledata.Rows.Add(tablerow);
                        var jsonobjarr = row.Value.EnumerateObject();
                        foreach (var av in jsonobjarr)
                        {
                            if (av.Value.ValueKind == System.Text.Json.JsonValueKind.Number)
                                tablerow.Values.Add(new ApplicationDataValue() { Code = av.Name, Value = av.Value.GetDecimal(), DataType = DatabaseModelItem.DataType2Decimal });
                            if (av.Value.ValueKind == System.Text.Json.JsonValueKind.String || av.Value.ValueKind == System.Text.Json.JsonValueKind.Undefined)
                                tablerow.Values.Add(new ApplicationDataValue() { Code = av.Name, Value = av.Value.GetString(), DataType = DatabaseModelItem.DataTypeString });
                        }
                        var rowid = tablerow.Values.Find(p => p.Code == "Id");
                        if (rowid != null)
                            tablerow.Id = rowid.GetAsInt();

                        var rowversion = tablerow.Values.Find(p => p.Code == "Version");
                        if (rowversion != null)
                            tablerow.Version = rowversion.GetAsInt();
                    }
                   
                }
            }

            var appid = res.MainTable.Find(p => p.Code == "ApplicationId");
            if (appid != null)
                res.ApplicationId = appid.GetAsInt(); 

            var dataid = res.MainTable.Find(p => p.Code == "Id");
            if (dataid != null)
                res.Id = dataid.GetAsInt();

            var dataversion = res.MainTable.Find(p => p.Code == "Version");
            if (dataversion != null)
                res.Version = dataversion.GetAsInt();

            return res;
        }

    }

    public class ApplicationDataValue
    {
        public string Code { get; set; }

        public string DataType { get; set; }

        public object Value { get; set; }

        public int GetAsInt()
        {
            if (HasValue)
                return Convert.ToInt32(Value);

            return 0;
        }

        public bool HasValue
        {
            get
            {
                if (Value == null)
                    return false;
                if (Value == DBNull.Value)
                    return false;
                if (DataType == DatabaseModelItem.DataTypeString && string.IsNullOrEmpty(Convert.ToString(Value)))
                    return false;

                return true;

            }

        }
    }

    public class ApplicationTable
    {
        public string Code { get; set; }

        public List<ApplicationTableRow> Rows { get; set; }

        public ApplicationTable()
        {
            Rows = new List<ApplicationTableRow>();
        }

    }


    public class ApplicationTableRow
    {
        public int Id { get; set; }

        public int Version { get; set; }

        public ApplicationTable Table { get; set; }

        public List<ApplicationDataValue> Values { get; set; }

        public ApplicationTableRow()
        {
            Values = new List<ApplicationDataValue>();
        }

    }

}
