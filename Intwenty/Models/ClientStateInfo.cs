using Intwenty.Data.Dto;
using Intwenty.MetaDataService.Model;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Intwenty.Models
{

    public class ClientStateInfo
    {
        public int Id { get; set; }

        public int Version { get; set; }

        public int OwnerId { get; set; }

        public int ApplicationId { get; set; }

        public string Properties { get; set; }

        public List<ApplicationValue> Values { get; set; }

        public List<ApplicationTable> SubTables { get; set; }


        public ClientStateInfo()
        {
            Properties = "";
            Values = new List<ApplicationValue>();
            SubTables = new List<ApplicationTable>();
        }

        public bool HasData
        {
            get
            {
                return Values.Count > 0;
            }
        }

        public bool HasDataAndModel
        {
            get
            {
                return Values.Count > 0 && Values.Exists(p=> p.Model != null);
            }
        }

        public void InferModel(ApplicationModel model)
        {
            foreach (var rootitem in model.DataStructure)
            {

                if (rootitem.IsMetaTypeDataColumn && rootitem.IsRoot)
                {
                    var v = Values.Find(p => p.DbName == rootitem.DbName);
                    if (v != null)
                        v.Model = rootitem;

                }

                if (rootitem.IsMetaTypeDataTable && rootitem.IsRoot)
                {
                    var table = SubTables.Find(p => p.DbName == rootitem.DbName);
                    if (table == null)
                        continue;

                    table.Model = rootitem;

                    foreach (var row in table.Rows)
                    {
                        foreach (var item in model.DataStructure)
                        {
                            if (item.IsMetaTypeDataColumn && item.ParentMetaCode == rootitem.MetaCode)
                            {
                                var v = row.Values.Find(p => p.DbName == item.DbName);
                                if (v != null)
                                    v.Model = item;

                            }
                        }
                    }
                }
            }


        }

        public static ClientStateInfo CreateFromJSON(System.Text.Json.JsonElement model)
        {
            var res = new ClientStateInfo();

            var jsonarr = model.EnumerateObject();
            foreach (var j in jsonarr)
            {
                if (j.Value.ValueKind == System.Text.Json.JsonValueKind.Object)
                {
                    var jsonobjarr = j.Value.EnumerateObject();
                    foreach (var av in jsonobjarr)
                    {
                        if (av.Value.ValueKind == System.Text.Json.JsonValueKind.String || av.Value.ValueKind == System.Text.Json.JsonValueKind.Undefined)
                            res.Values.Add(new ApplicationValue() { DbName = av.Name, Value = av.Value.GetString() });
                        if (av.Value.ValueKind == System.Text.Json.JsonValueKind.Number)
                            res.Values.Add(new ApplicationValue() { DbName = av.Name, Value = av.Value.GetDecimal() });
                        if (av.Value.ValueKind == System.Text.Json.JsonValueKind.False || av.Value.ValueKind == System.Text.Json.JsonValueKind.True)
                            res.Values.Add(new ApplicationValue() { DbName = av.Name, Value = av.Value.GetBoolean() });

                    }
                }
                if (j.Value.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    var tabledata = new ApplicationTable() { DbName = j.Name };
                    res.SubTables.Add(tabledata);
                    var jsonrows = j.Value.EnumerateObject();
                    foreach (var row in jsonrows)
                    {
                        var tablerow = new ApplicationTableRow() { Table = tabledata };
                        tabledata.Rows.Add(tablerow);
                        var jsonobjarr = row.Value.EnumerateObject();
                        foreach (var av in jsonobjarr)
                        {
                            if (av.Value.ValueKind == System.Text.Json.JsonValueKind.String || av.Value.ValueKind == System.Text.Json.JsonValueKind.Undefined)
                                tablerow.Values.Add(new ApplicationValue() { DbName = av.Name, Value = av.Value.GetString() });
                            if (av.Value.ValueKind == System.Text.Json.JsonValueKind.Number)
                                tablerow.Values.Add(new ApplicationValue() { DbName = av.Name, Value = av.Value.GetDecimal() });
                            if (av.Value.ValueKind == System.Text.Json.JsonValueKind.False || av.Value.ValueKind == System.Text.Json.JsonValueKind.True)
                                tablerow.Values.Add(new ApplicationValue() { DbName = av.Name, Value = av.Value.GetBoolean() });
                        }
                        var rowid = tablerow.Values.Find(p => p.DbName == "Id");
                        if (rowid != null)
                            tablerow.Id = rowid.GetAsInt().Value;

                        var rowversion = tablerow.Values.Find(p => p.DbName == "Version");
                        if (rowversion != null)
                            tablerow.Version = rowversion.GetAsInt().Value;
                    }

                }
            }

            var appid = res.Values.Find(p => p.DbName == "ApplicationId");
            if (appid != null)
                res.ApplicationId = appid.GetAsInt().Value;

            var dataid = res.Values.Find(p => p.DbName == "Id");
            if (dataid != null)
                res.Id = dataid.GetAsInt().Value;

            var dataversion = res.Values.Find(p => p.DbName == "Version");
            if (dataversion != null)
                res.Version = dataversion.GetAsInt().Value;

            return res;
        }


    }

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
                return Model!=null;
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

        public List<ApplicationValue> Values { get; set; }

        public ApplicationTableRow()
        {
            Values = new List<ApplicationValue>();
        }

    }



}
