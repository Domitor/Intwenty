using System;
using Intwenty.MetaDataService.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Intwenty.Models.MetaDesigner
{

    public static class DBVmCreator
    {

        public static DBVm GetDBVm(ApplicationModel app)
        {
            var res = new DBVm();
            res.Id = app.Application.Id;
            res.Title = app.Application.Title;

            var tables = UIDbTable.GetTables(app);
            res.Tables = tables;
            foreach (var t in tables)
            {
                foreach (var c in t.Columns)
                {
                    res.Columns.Add(c);
                }
            }

            foreach (var t in tables)
            {
                t.Columns.Clear();
            }

            return res;

        }

    }

    public static class MetaDataItemCreator
    {
        public static List<DatabaseModelItem> GetMetaDataItems(DBVm model)
        {
            var res = new List<DatabaseModelItem>();
            res.AddRange(model.Tables.Select(p => new DatabaseModelItem(p.MetaType) { Id = p.Id, DbName = p.DbName, Description = p.Description, Properties = p.Properties, DataType = "" }));
            res.AddRange(model.Columns.Select(p => new DatabaseModelItem(p.MetaType) { Id = p.Id, DbName = p.DbName, Description = p.Description, Domain = p.Domain, DataType = p.DataType, Mandatory = p.Mandatory, Properties = p.Properties, TableName = p.TableName }));
            return res;
        }
    }


    public class DBVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<UIDbTable> Tables { get; set; }
        public List<UIDbTableField> Columns { get; set; } 

        public DBVm()
        {
            Title = "";
            Tables = new List<UIDbTable>();
            Columns = new List<UIDbTableField>();
        }
    }


    public class UIDbTable
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public string DbName { get; set; }
        public string MetaCode { get; set; }
        public string ParentMetaCode { get; set; }
        public string MetaType { get; set; }
        public string Properties { get; set; }
        public string Description { get; set; }
        public List<UIDbTableField> Columns { get; set; } 

        public UIDbTable()
        {
            DbName = "";
            MetaCode = "";
            ParentMetaCode = "";
            MetaType = "";
            Description = "";
            Columns= new List<UIDbTableField>();
        }

        public static List<UIDbTable> GetTables(ApplicationModel app)
        {
            var res = new List<UIDbTable>();

            res.Add(new UIDbTable() { Id = 0, DbName = app.Application.MainTableName, ApplicationId = app.Application.Id, MetaCode = "VIRTUAL", ParentMetaCode = "ROOT", MetaType = "DATAVALUETABLE", Description = "Main table for " + app.Application.Title, Properties= "DEFAULTTABLE=TRUE" });

            foreach (var t in app.DataStructure)
            {
                if (t.IsMetaTypeDataValue && t.IsRoot)
                {
                    res[0].Columns.Add(new UIDbTableField() { DbName = t.DbName, Id = t.Id, MetaCode = t.MetaCode, ParentMetaCode = t.ParentMetaCode, MetaType = t.MetaType, Properties = t.Properties, DataType = t.DataType, Description = t.Description, Domain = t.Domain, TableName = app.Application.DbName, Mandatory = t.Mandatory, ApplicationId = app.Application.Id });
                }

                if (t.IsMetaTypeDataValueTable)
                {
                    var tbl = new UIDbTable() { Id = 0, DbName = t.DbName, MetaCode = t.MetaCode, ParentMetaCode = "ROOT", MetaType = t.MetaType, Properties = t.Properties, Description = t.Description, ApplicationId = app.Application.Id };
                    foreach (var col in app.DataStructure)
                    {
                        if (col.IsMetaTypeDataValue && col.ParentMetaCode == t.MetaCode)
                        {
                            tbl.Columns.Add(new UIDbTableField() { DbName = col.DbName, Id = col.Id, MetaCode = col.MetaCode, ParentMetaCode = col.ParentMetaCode, MetaType = col.MetaType, Mandatory = col.Mandatory, Properties = col.Properties, DataType = col.DataType, Description = col.Description, Domain = col.Domain, TableName = t.DbName, ApplicationId = app.Application.Id });
                        }
                    }
                    res.Add(tbl);
                }
            }

            return res;
        }
    }

    public class UIDbTableField
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public string DbName { get; set; }
        public string MetaCode { get; set; }
        public string ParentMetaCode { get; set; }
        public bool Mandatory { get; set; }
        public string MetaType { get; set; }
        public string Properties { get; set; }
        public string DataType { get; set; }
        public string Description { get; set; }
        public string Domain { get; set; }

        public string TableName { get; set; }

        public UIDbTableField()
        {
            DbName = "";
            MetaCode = "";
            ParentMetaCode = "";
            MetaType = "";
            Properties = "";
            DataType = "";
            Description = "";
            Domain = "";
            TableName = "";

        }
    }

}
