using Intwenty.Data.Dto;
using Intwenty.Model;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Intwenty.Data.Dto
{

    public class ClientStateInfo
    {
        public static readonly string DEFAULT_USERID = "SYSTEM";

        public int Id { get; set; }

        public int Version { get; set; }

        public string UserId { get; set; }

        public int ApplicationId { get; set; }

        public string Properties { get; set; }

        public List<ApplicationValue> Values { get; set; }

        public List<ApplicationTable> SubTables { get; set; }

        public System.Text.Json.JsonElement JSON { get; set; }

        private string _owneruserid { get; set; }

        public ClientStateInfo()
        {
            UserId = DEFAULT_USERID;
            OwnerUserId = DEFAULT_USERID;
            Properties = "";
            Values = new List<ApplicationValue>();
            SubTables = new List<ApplicationTable>();
        }

        public string OwnerUserId
        {
            set { _owneruserid = value; }

            get
            {
                if (_owneruserid == DEFAULT_USERID && !string.IsNullOrEmpty(UserId))
                    return UserId;
                else
                    return _owneruserid;

            }
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

            if (model.ValueKind == System.Text.Json.JsonValueKind.Object)
            {
                res.JSON = model;
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
                        var jsonrows = j.Value.EnumerateArray();
                        foreach (var row in jsonrows)
                        {
                            var tablerow = new ApplicationTableRow() { Table = tabledata };
                            tabledata.Rows.Add(tablerow);
                            var jsonobjarr = row.EnumerateObject();
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

                            var parentid = tablerow.Values.Find(p => p.DbName == "ParentId");
                            if (parentid != null)
                                tablerow.ParentId = parentid.GetAsInt().Value;
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



            }
            else if (model.ValueKind == System.Text.Json.JsonValueKind.Array)
            {
                var tabledata = new ApplicationTable() { DbName = "List" };
                res.SubTables.Add(tabledata);
                var jsonrows = model.EnumerateArray();
                foreach (var row in jsonrows)
                {
                    var tablerow = new ApplicationTableRow() { Table = tabledata };
                    tabledata.Rows.Add(tablerow);
                    var jsonobjarr = row.EnumerateObject();
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

                    var parentid = tablerow.Values.Find(p => p.DbName == "ParentId");
                    if (parentid != null)
                        tablerow.ParentId = parentid.GetAsInt().Value;
                }
            }


            return res;
        }


    }

   



}
