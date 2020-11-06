
using Intwenty.Entity;
using Intwenty.DataClient;
using Intwenty.Interface;
using System.Collections.Generic;

namespace Intwenty.Model
{
    public class DataViewModelItem : BaseModelItem, IIntwentyResultColumn, ILocalizableTitle
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
            SystemMetaCode = entity.SystemMetaCode;
            if (string.IsNullOrEmpty(SQLQueryFieldDataType))
                SQLQueryFieldDataType = DatabaseModelItem.DataTypeString;
        }

        public SystemModelItem SystemInfo { get; set; }

        public string SystemMetaCode { get; set; }

        public string TitleLocalizationKey { get; set; }

        public string SQLQuery { get; set; }

        public string SQLQueryFieldName { get; set; }

        public string SQLQueryFieldDataType { get; set; }

        public string Name
        {
            get { return SQLQueryFieldName; }
        }

        public int OrderNo { get; set; }

        public override string ModelCode
        {
            get { return "DATAVIEWMODEL"; }
        }


        public static List<IntwentyMetaType> GetAvailableMetaTypes()
        {
            var t = new List<IntwentyMetaType>();
            t.Add(new IntwentyMetaType() { Code = MetaTypeDataView, Title = "Data View" });
            t.Add(new IntwentyMetaType() { Code = MetaTypeDataViewColumn, Title = "Data View Column" });
            t.Add(new IntwentyMetaType() { Code = MetaTypeDataViewKeyColumn, Title = "Data View Key Column" });
            return t;
        }

        public override bool HasValidMetaType
        {
            get
            {
                if (string.IsNullOrEmpty(MetaType))
                    return false;


                if (!IntwentyRegistry.IntwentyMetaTypes.Exists(p => p.Code == MetaType && p.ModelCode == ModelCode))
                    return false;

                return true;

            }
        }

        public override bool HasValidProperties
        {
            get
            {
                foreach (var prop in GetProperties())
                {
                    if (!IntwentyRegistry.IntwentyProperties.Exists(p => p.CodeName == prop && p.ValidFor.Contains(MetaType)))
                        return false;
                }
                return true;
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

       

        public bool HasNonSelectSql
        {
            get
            {
                if (!string.IsNullOrEmpty(SQLQuery))
                {

                    if (SQLQuery.ToUpper().Contains("ALTER ") ||
                        SQLQuery.ToUpper().Contains("DROP ") ||
                        SQLQuery.ToUpper().Contains("TRUNCATE ") ||
                        SQLQuery.ToUpper().Contains("INSERT ") ||
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


        public bool HasSystemInfo
        {
            get
            {
                return this.SystemInfo != null;
            }

        }


    }

  
}
