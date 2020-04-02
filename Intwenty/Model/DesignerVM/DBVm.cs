using System;
using Intwenty.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Intwenty.Model.DesignerVM
{

    public static class DatabaseModelCreator
    {

        public static List<DatabaseModelItem> GetDatabaseModel(DBVm model)
        {
            var res = new List<DatabaseModelItem>();
            res.AddRange(model.Tables.Select(p => new DatabaseModelItem(p.MetaType) { Id = p.Id, DbName = p.DbName, Description = p.Description, Properties = p.Properties, DataType = "", MetaCode = p.MetaCode,ParentMetaCode = "ROOT" }));
            res.AddRange(model.Columns.Select(p => new DatabaseModelItem(p.MetaType) { Id = p.Id, DbName = p.DbName, Description = p.Description, Domain = p.Domain, DataType = p.DataType, Mandatory = p.Mandatory, Properties = p.Properties, TableName = p.TableName, MetaCode=p.MetaCode }));
            return res;
        }

        public static DBVm GetDatabaseVm(ApplicationModel app)
        {
            var res = new DBVm();
            res.Id = app.Application.Id;
            res.Title = app.Application.Title;

            var tables = GetDatabaseTableVm(app);
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

        public static List<DatabaseTableVm> GetDatabaseTableVm(ApplicationModel app)
        {
            var res = new List<DatabaseTableVm>();

            res.Add(new DatabaseTableVm() { Id = 0, DbName = app.Application.DbName, ApplicationId = app.Application.Id, MetaCode = "VIRTUAL", ParentMetaCode = "ROOT", MetaType = "DATATABLE", Description = "Main table for " + app.Application.Title, IsDefaultTable = true });

            foreach (var t in app.DataStructure)
            {
                if (t.IsMetaTypeDataColumn && t.IsRoot)
                {
                    res[0].Columns.Add(new DatabaseTableFieldVm() { DbName = t.DbName, Id = t.Id, MetaCode = t.MetaCode, ParentMetaCode = t.ParentMetaCode, MetaType = t.MetaType, Properties = t.Properties, DataType = t.DataType, Description = t.Description, Domain = t.Domain, TableName = app.Application.DbName, Mandatory = t.Mandatory, ApplicationId = app.Application.Id });
                }

                if (t.IsMetaTypeDataTable)
                {
                    var tbl = new DatabaseTableVm() { Id = t.Id, DbName = t.DbName, MetaCode = t.MetaCode, ParentMetaCode = "ROOT", MetaType = t.MetaType, Properties = t.Properties, Description = t.Description, ApplicationId = app.Application.Id };
                    foreach (var col in app.DataStructure)
                    {
                        if (col.IsMetaTypeDataColumn && col.ParentMetaCode == t.MetaCode)
                        {
                            tbl.Columns.Add(new DatabaseTableFieldVm() { DbName = col.DbName, Id = col.Id, MetaCode = col.MetaCode, ParentMetaCode = col.ParentMetaCode, MetaType = col.MetaType, Mandatory = col.Mandatory, Properties = col.Properties, DataType = col.DataType, Description = col.Description, Domain = col.Domain, TableName = t.DbName, ApplicationId = app.Application.Id });
                        }
                    }
                    res.Add(tbl);
                }
            }

            return res;
        }

    }

   

    public class DBVm
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public List<DatabaseTableVm> Tables { get; set; }
        public List<DatabaseTableFieldVm> Columns { get; set; } 

        public DBVm()
        {
            Title = "";
            Tables = new List<DatabaseTableVm>();
            Columns = new List<DatabaseTableFieldVm>();
        }
    }


    public class DatabaseTableVm
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public string DbName { get; set; }
        public string MetaCode { get; set; }
        public string ParentMetaCode { get; set; }
        public string MetaType { get; set; }
        public string Properties { get; set; }
        public string Description { get; set; }

        public bool IsDefaultTable { get; set; }
        public List<DatabaseTableFieldVm> Columns { get; set; } 

        public DatabaseTableVm()
        {
            DbName = "";
            MetaCode = "";
            ParentMetaCode = "";
            MetaType = "";
            Description = "";
            Columns= new List<DatabaseTableFieldVm>();
        }

       
    }

    public class DatabaseTableFieldVm
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

        public DatabaseTableFieldVm()
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
