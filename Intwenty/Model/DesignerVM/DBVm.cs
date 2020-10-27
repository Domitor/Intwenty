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
            res.AddRange(model.Tables.Select(p => new DatabaseModelItem(p.MetaType) { Id = p.Id, DbName = p.DbName, Description = p.Description, Properties = p.CompilePropertyString(), DataType = "", MetaCode = p.MetaCode,ParentMetaCode = "ROOT" }));
            res.AddRange(model.Columns.Select(p => new DatabaseModelItem(p.MetaType) { Id = p.Id, DbName = p.DbName, Description = p.Description, DataType = p.DataType, Properties = p.CompilePropertyString(), TableName = p.TableName, MetaCode=p.MetaCode }));


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
            var table = new DatabaseTableVm() { Id = 0, DbName = app.Application.DbName, ApplicationId = app.Application.Id, MetaCode = "VIRTUAL", ParentMetaCode = "ROOT", MetaType = "DATATABLE", Description = "Main table for " + app.Application.Title, IsDefaultTable = true };
            table.BuildPropertyList();
            res.Add(table);

            foreach (var t in app.DataStructure)
            {
                if (t.IsMetaTypeDataColumn && t.IsRoot)
                {
                    var col = new DatabaseTableColumnVm() { DbName = t.DbName, Id = t.Id, MetaCode = t.MetaCode, ParentMetaCode = t.ParentMetaCode, MetaType = t.MetaType, Properties = t.Properties, DataType = t.DataType, Description = t.Description, TableName = app.Application.DbName, ApplicationId = app.Application.Id, IsFrameworkItem = t.IsFrameworkItem };
                    col.BuildPropertyList();
                    res[0].Columns.Add(col);
                }

                if (t.IsMetaTypeDataTable)
                {
                    var subtable = new DatabaseTableVm() { Id = t.Id, DbName = t.DbName, MetaCode = t.MetaCode, ParentMetaCode = "ROOT", MetaType = t.MetaType, Properties = t.Properties, Description = t.Description, ApplicationId = app.Application.Id };
                    subtable.BuildPropertyList();
                    foreach (var col in app.DataStructure)
                    {
                        if (col.IsMetaTypeDataColumn && col.ParentMetaCode == t.MetaCode)
                        {
                            var subtablecolumn = new DatabaseTableColumnVm() { DbName = col.DbName, Id = col.Id, MetaCode = col.MetaCode, ParentMetaCode = col.ParentMetaCode, MetaType = col.MetaType, Properties = col.Properties, DataType = col.DataType, Description = col.Description, TableName = t.DbName, ApplicationId = app.Application.Id, IsFrameworkItem = col.IsFrameworkItem };
                            subtablecolumn.BuildPropertyList();
                            subtable.Columns.Add(subtablecolumn);
                        }
                    }
                    res.Add(subtable);
                }
            }

            return res;
        }

        public static DatabaseTableVm GetListViewTableVm(ApplicationModel app)
        {
            var table = new DatabaseTableVm() { Id = 0, DbName = app.Application.DbName, ApplicationId = app.Application.Id, MetaCode = "VIRTUAL", ParentMetaCode = "ROOT", MetaType = "DATATABLE", Description = "Main table for " + app.Application.Title, IsDefaultTable = true };
            table.BuildPropertyList();

            foreach (var t in app.DataStructure)
            {
                if (t.IsMetaTypeDataColumn && t.IsRoot)
                {
                    var col = new DatabaseTableColumnVm() { DbName = t.DbName, Id = t.Id, MetaCode = t.MetaCode, ParentMetaCode = t.ParentMetaCode, MetaType = t.MetaType, Properties = t.Properties, DataType = t.DataType, Description = t.Description, TableName = app.Application.DbName, ApplicationId = app.Application.Id, IsFrameworkItem = t.IsFrameworkItem };
                    col.BuildPropertyList();
                    table.Columns.Add(col);
                }

               
            }

            return table;
        }

    }

   

    public class DBVm
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public List<DatabaseTableVm> Tables { get; set; }
        public List<DatabaseTableColumnVm> Columns { get; set; }
      

        public DBVm()
        {
            Title = "";
            Tables = new List<DatabaseTableVm>();
            Columns = new List<DatabaseTableColumnVm>();
           
        }
    }


    public class DatabaseTableVm : BaseModelVm
    {
        public int ApplicationId { get; set; }
        public string DbName { get; set; }
        public string MetaCode { get; set; }
        public string ParentMetaCode { get; set; }
        public string MetaType { get; set; }
        public string Description { get; set; }

        public bool IsDefaultTable { get; set; }
        public List<DatabaseTableColumnVm> Columns { get; set; }

        public DatabaseTableVm()
        {
            DbName = "";
            MetaCode = "";
            ParentMetaCode = "";
            MetaType = "";
            Description = "";
            Columns = new List<DatabaseTableColumnVm>();
        }

        public override List<IntwentyProperty> SelectableProperties
        {
            get
            {
                return DatabaseModelItem.GetAvailableProperties().Where(p => p.ValidFor.Contains(DatabaseModelItem.MetaTypeDataTable)).ToList();
            }
        }


    }

    public class DatabaseTableColumnVm : BaseModelVm
    {
        public int ApplicationId { get; set; }
        public string DbName { get; set; }
        public string MetaCode { get; set; }
        public string ParentMetaCode { get; set; }
        public string MetaType { get; set; }
        public string DataType { get; set; }
        public string Description { get; set; }
        public string TableName { get; set; }
        public bool IsFrameworkItem { get; set; }



        public DatabaseTableColumnVm()
        {
            DbName = "";
            MetaCode = "";
            ParentMetaCode = "";
            MetaType = "";
            Properties = "";
            DataType = "";
            Description = "";
            TableName = "";

        }

        public override List<IntwentyProperty> SelectableProperties 
        {
            get {
                return DatabaseModelItem.GetAvailableProperties().Where(p => p.ValidFor.Contains(DatabaseModelItem.MetaTypeDataColumn)).ToList();
            }
        }



    }

}
