
using Intwenty.Data.DBAccess.Helpers;
using Intwenty.Data.Entity;
using System.Collections.Generic;

namespace Intwenty.Model
{
    public class DataViewModelItem : BaseModelItem, IIntwentyDataColum
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
            TitleLocalizationKey = entity.TitleLocalizationKey;
            MetaCode = entity.MetaCode;
            ParentMetaCode = entity.ParentMetaCode;
            OrderNo = entity.OrderNo;
            Properties = "";
            SQLQueryFieldDataType = entity.SQLQueryFieldDataType;
            if (string.IsNullOrEmpty(SQLQueryFieldDataType))
                SQLQueryFieldDataType = DatabaseModelItem.DataTypeString;
        }


        public string SQLQuery { get; set; }

        public string SQLQueryFieldName { get; set; }

        public string SQLQueryFieldDataType { get; set; }


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

        public string QueryTableDbName
        {

            get
            {
                try
                {
                    var ind1 = SQLQuery.ToLower().IndexOf("from");
                    if (ind1 < 5)
                        return string.Empty;

                    ind1 += 4;

                    var searchstring = SQLQuery.Substring(ind1);
                    var namestart = 0;
                    var nameend = 0;
                    var count = 0;
                    foreach (var c in searchstring)
                    {
                        if (!char.IsWhiteSpace(c) && namestart == 0)
                            namestart = count;

                        if (namestart > 0 && char.IsWhiteSpace(c) && nameend == 0)
                            nameend = count;

                        if (nameend > 0)
                            break;

                        count += 1;
                    }

                    return searchstring.Substring(namestart, (nameend - namestart));
                }
                catch { }

                return string.Empty;
            }


        }

        public string ColumnName => this.SQLQueryFieldName;

        public bool IsNumeric
        {
            get
            {

                return (DataType == DatabaseModelItem.DataType1Decimal) ||
                       (DataType == DatabaseModelItem.DataType2Decimal) ||
                       (DataType == DatabaseModelItem.DataType3Decimal) ||
                       (DataType == DatabaseModelItem.DataTypeInt) ||
                       (DataType == DatabaseModelItem.DataTypeBool);
            }
        }
        public bool IsDateTime
        {
            get
            {
                return (DataType == DatabaseModelItem.DataTypeDateTime);
            }
        }

        public string DataType => this.SQLQueryFieldDataType;
    }

  
}
