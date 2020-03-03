using Moley.Data.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moley.Models
{
    public class DBVm
    {
        public int Id = 0;
        public string Title = "";
        public List<MetaDataItemDto> Tables = new List<MetaDataItemDto>();
        public List<MetaDataItemDto> Columns = new List<MetaDataItemDto>();
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
