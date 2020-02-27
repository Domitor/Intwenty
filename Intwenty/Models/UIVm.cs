using Moley.Data.Dto;
using System.Collections.Generic;


namespace Moley.Models
{
    public class UIVm
    {

        public int Id = 0;
        public string Title = "";
        public string Description = "";
        public string Properties = "";
        public string TableName = "";
        public string ColumnName = "";
        public string MetaType = "";
        public string LookUpKeyFieldDBColumnName = "";
        public string LookUpKeyFieldViewColumnName = "";
        public string LookUpKeyFieldTitle = "";
        public string LookUpFieldDBColumnName = "";
        public string LookUpFieldViewColumnName = "";
        public string LookUpFieldTitle = "";
        public string Domain = "";


        public List<UIFieldVm> Fields = new List<UIFieldVm>();

        public static List<UIVm> GetInput(ApplicationDto app)
        {
            var res = new List<UIVm>();

            foreach (var t in app.UIStructure)
            {
                if (t.IsUITypeListView || t.IsUITypeListViewField)
                    continue;

                if (t.IsUITypeLookUp)
                {
                    var lookup = new UIVm() { Description = t.Description, Title = t.Title, Properties = t.Properties, Domain = t.Domain, MetaType = t.MetaType, Id=t.Id };
                    foreach (var lf in app.UIStructure)
                    {
                        if (lf.IsUITypeLookUpKeyField && lf.ParentMetaCode == t.MetaCode)
                        {
                            lookup.LookUpKeyFieldTitle = lf.Title;
                            if (lf.IsDataConnected)
                            {
                                lookup.TableName = lf.DataInfo.TableName;
                                lookup.LookUpKeyFieldDBColumnName = lf.DataInfo.DbName;
                            }
                            if (lf.IsDataViewConnected)
                            {
                                lookup.LookUpKeyFieldViewColumnName = lf.ViewInfo.SQLQueryFieldName;
                            }
                        }

                        if (lf.IsUITypeLookUpField && lf.ParentMetaCode == t.MetaCode)
                        {
                            lookup.LookUpFieldTitle = lf.Title;
                            if (lf.IsDataConnected)
                            {
                                lookup.TableName = lf.DataInfo.TableName;
                                lookup.LookUpFieldDBColumnName = lf.DataInfo.DbName;
                            }
                            if (lf.IsDataViewConnected)
                            {
                                lookup.LookUpFieldViewColumnName = lf.ViewInfo.SQLQueryFieldName;
                            }
                        }
                    }

                    res.Add(lookup);

                }
                else
                {
                    if (t.IsUITypeLookUpField || t.IsUITypeLookUpKeyField)
                        continue;

                    if (t.IsDataConnected)
                        res.Add(new UIVm() { ColumnName = t.DataInfo.ColumnName, TableName = t.DataInfo.TableName, Description = t.Description, Title = t.Title, Properties = t.Properties, MetaType = t.MetaType,Id = t.Id });
                    else
                        res.Add(new UIVm() { ColumnName = "", TableName = "", Description = t.Description, Title = t.Title, Properties = t.Properties, MetaType = t.MetaType, Id = t.Id });
                }
                 

            }


            return res;
        }

        public static UIVm GetListView(ApplicationDto app)
        {
            var res = new UIVm();
            return res;
        }


    }

    public class UIFieldVm
    {

    }

    public class UIDbTable
    {
        public int Id = 0;
        public string DbName = "";
        public string MetaCode = "";
        public List<UIDbTableField> Columns = new List<UIDbTableField>();

        public static List<UIDbTable> GetTables(ApplicationDto app)
        {
            var res = new List<UIDbTable>();

            res.Add(new UIDbTable() { Id = 0, DbName = app.Application.MainTableName, MetaCode = "VIRTUAL" });

            foreach (var t in app.DataStructure)
            {
                if (t.IsMetaTypeDataValue && t.IsRoot)
                {
                    res[0].Columns.Add(new UIDbTableField() { DbName = t.DbName, Id = t.Id, MetaCode = t.MetaCode, ParentMetaCode = t.ParentMetaCode });
                }

                if (t.IsMetaTypeDataValueTable)
                {
                    var tbl = new UIDbTable() { Id = 0, DbName = app.Application.MainTableName, MetaCode = t.MetaCode };
                    foreach (var col in app.DataStructure)
                    {
                        if (col.IsMetaTypeDataValue && col.ParentMetaCode == t.MetaCode)
                        {
                            tbl.Columns.Add(new UIDbTableField() { DbName = col.DbName, Id = t.Id, MetaCode = col.MetaCode, ParentMetaCode = col.ParentMetaCode });
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
        public int Id = 0;
        public string DbName = "";
        public string MetaCode = "";
        public string ParentMetaCode = "";
    }



}
