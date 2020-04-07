
using Intwenty.Data.Entity;
using System.Collections.Generic;

namespace Intwenty.Model
{
    public class DataViewModelItem : BaseModelItem
    {
        //META TYPES
        public static readonly string MetaTypeDataView = "DATAVIEW";
        public static readonly string MetaTypeDataViewColumn = "DATAVIEWCOLUMN";
        public static readonly string MetaTypeDataViewKeyColumn = "DATAVIEWKEYCOLUMN";


        public DataViewModelItem()
        {
        }

        public DataViewModelItem(string metatype)
        {
            MetaType = metatype;
        }


        public DataViewModelItem(DataViewItem entity)
        {
            Id = entity.Id;
            SQLQuery = entity.SQLQuery;
            SQLQueryFieldName = entity.SQLQueryFieldName;
            MetaType = entity.MetaType;
            Title = entity.Title;
            MetaCode = entity.MetaCode;
            ParentMetaCode = entity.ParentMetaCode;
            OrderNo = entity.OrderNo;
            Properties = "";
        }


        public string SQLQuery { get; set; }

        public string SQLQueryFieldName { get; set; }


        public int OrderNo { get; set; }


        public static List<string> ValidProperties
        {
            get
            {
                var t = new List<string>();
                return t;
            }
        }

        public static List<string> ValidMetaTypes
        {
            get
            {
                var t = new List<string>();
                t.Add(MetaTypeDataView);
                t.Add(MetaTypeDataViewColumn);
                t.Add(MetaTypeDataViewKeyColumn);
                return t;
            }
        }

        public bool IsMetaTypeDataView
        {
            get { return MetaType == MetaTypeDataView; }
        }

        public bool IsMetaTypeDataViewColumn
        {
            get { return MetaType == MetaTypeDataViewColumn; }
        }


        public bool IsMetaTypeDataViewKeyColumn
        {
            get { return MetaType == MetaTypeDataViewKeyColumn; }
        }

        public override bool HasValidMetaType
        {
            get
            {
                if (string.IsNullOrEmpty(MetaType))
                    return false;

                if (ValidMetaTypes.Contains(MetaType))
                    return true;

                return false;

            }
        }

        public bool HasNonSelectSql
        {
            get
            {
                if (!string.IsNullOrEmpty(SQLQuery))
                {

                    if (SQLQuery.ToUpper().Contains("ALTER ") ||
                        SQLQuery.ToUpper().Contains("DROP ") ||
                        SQLQuery.ToUpper().Contains("TRUNCATE ") ||
                        SQLQuery.ToUpper().Contains("UPDATE ") ||
                        SQLQuery.ToUpper().Contains("DELETE "))
                    {
                        return true;
                    }
                }

                return false;
            }

        }

      

      
    }

  
}
