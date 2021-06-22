using Intwenty.Entity;
using Intwenty.DataClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Intwenty.Model
{
    public class DatabaseModelItem : BaseModelItem, IIntwentyResultColumn
    {
        //DATATYPES
        public static readonly string DataTypeBool = "BOOLEAN";
        public static readonly string DataTypeString = "STRING";
        public static readonly string DataTypeText = "TEXT";
        public static readonly string DataTypeInt = "INTEGER";
        public static readonly string DataTypeDateTime = "DATETIME";
        public static readonly string DataType1Decimal = "1DECIMAL";
        public static readonly string DataType2Decimal = "2DECIMAL";
        public static readonly string DataType3Decimal = "3DECIMAL";


        //META TYPES
        public static readonly string MetaTypeDataColumn = "DATACOLUMN";
        public static readonly string MetaTypeDataTable = "DATATABLE";

        public DatabaseModelItem()
        {
            SetDefaults();
        }

        public DatabaseModelItem(string metatype)
        {
            MetaType = metatype;
            SetDefaults();

        }

        public DatabaseModelItem(DatabaseItem entity)
        {
            Id = entity.Id;
            MetaType = entity.MetaType;
            Description = entity.Description;
            AppMetaCode = entity.AppMetaCode;
            MetaCode = entity.MetaCode;
            ParentMetaCode = entity.ParentMetaCode;
            DbName = entity.DbName;
            Title = entity.DbName;
            DataType = entity.DataType;
            Properties = entity.Properties;
            SystemMetaCode = entity.SystemMetaCode;
            if (IsMetaTypeDataTable)
                TableName = DbName;
            if (IsMetaTypeDataColumn)
                ColumnName = DbName;


            SetDefaults();
        }

        public static DatabaseModelItem CreateFrameworkColumn(int id, ApplicationModelItem app, string tablename, string dbname, string datatype, string parentmetacode="ROOT")
        {
            var res = new DatabaseModelItem();
            res.ApplicationInfo = app;
            res.Id = id;
            res.MetaType = MetaTypeDataColumn;
            res.AppMetaCode = app.MetaCode;
            res.ColumnName = dbname;
            res.DbName = dbname;
            res.TableName = tablename;
            res.DataType = datatype;
            res.Title = dbname;
            res.MetaCode = dbname.ToUpper();
            res.ParentMetaCode = parentmetacode;
            res.Properties = string.Empty;
            res.IsFrameworkItem = true;
            if (app.SystemInfo != null)
                res.SystemMetaCode = app.SystemInfo.MetaCode;
            res.SystemInfo = app.SystemInfo;
            res.Description = "Required by Intwenty";

            return res;
        }

        private void SetDefaults()
        {
            if (string.IsNullOrEmpty(Description)) Description = string.Empty;
            if (string.IsNullOrEmpty(AppMetaCode)) AppMetaCode = string.Empty;
            if (string.IsNullOrEmpty(MetaCode)) MetaCode = string.Empty;
            if (string.IsNullOrEmpty(ParentMetaCode)) ParentMetaCode = string.Empty;
            if (string.IsNullOrEmpty(DbName)) DbName = string.Empty;
            if (string.IsNullOrEmpty(DataType)) DataType = string.Empty;
            if (string.IsNullOrEmpty(Properties)) Properties = string.Empty;
            if (string.IsNullOrEmpty(TableName)) TableName = string.Empty;
            if (string.IsNullOrEmpty(ColumnName)) ColumnName = string.Empty;
            if (string.IsNullOrEmpty(Title)) Title = string.Empty;
            if (string.IsNullOrEmpty(SystemMetaCode)) SystemMetaCode = string.Empty;
        }

        public SystemModelItem SystemInfo { get; set; }
        public ApplicationModelItem ApplicationInfo { get; set; }
        public string SystemMetaCode { get; set; }

        public string Description { get; set; }

        public string AppMetaCode { get; set; }

        public string DbName { get; set; }

        public string TableName { get; set; }

        public string ColumnName { get; set; }

        public string DataType { get; set; }

        public bool IsFrameworkItem { get; set; }

        public string Domain
        {
            get { return GetPropertyValue("DOMAIN"); }
        }

        public bool Mandatory
        {
            get { return HasPropertyWithValue("MANDATORY", "TRUE"); }
        }

        public bool Unique
        {
            get { return HasPropertyWithValue("UNIQUE", "TRUE"); }
        }


        public string Name
        {
            get { return DbName;  }
        }

        public override string ModelCode
        {
            get { return "DATAMODEL"; }
        }

        public static List<string> GetAvailableDataTypes()
        {
         
            var t = new List<string>();
            t.Add(DataType1Decimal);
            t.Add(DataType2Decimal);
            t.Add(DataType3Decimal);
            t.Add(DataTypeInt);
            t.Add(DataTypeBool);
            t.Add(DataTypeString);
            t.Add(DataTypeText);
            t.Add(DataTypeDateTime);
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


        public bool IsDataTypeBool
        {
            get { return DataType == DataTypeBool; }
        }

        public bool IsDataTypeString
        {
            get { return DataType == DataTypeString; }
        }


        public bool IsDataTypeText
        {
            get { return DataType == DataTypeText; }
        }

        public bool IsDataTypeInt
        {
            get { return DataType == DataTypeInt; }
        }


        public bool IsDataTypeDateTime
        {
            get { return DataType == DataTypeDateTime; }
        }

      
        public bool IsDataType1Decimal
        {
            get { return DataType == DataType1Decimal; }
        }


        public bool IsDataType2Decimal
        {
            get { return DataType == DataType2Decimal; }
        }


        public bool IsDataType3Decimal
        {
            get { return DataType == DataType3Decimal; }
        }

       
        public bool HasValidDataType
        {
            get
            {
                if (string.IsNullOrEmpty(DataType))
                    return false;

                if (GetAvailableDataTypes().Contains(DataType))
                    return true;

                return false;

            }
        }
      
        public bool IsMetaTypeDataColumn
        {
            get { return MetaType == MetaTypeDataColumn; }
        }


        public bool IsMetaTypeDataTable
        {
            get { return MetaType == MetaTypeDataTable; }
        }

       


        public bool HasDomain
        {
            get { return Domain.Contains("APP."); }
        }

        public bool IsNumeric
        {
            get
            {

                return (DataType == DataType1Decimal) ||
                       (DataType == DataType2Decimal) ||
                       (DataType == DataType3Decimal) ||
                       (DataType == DataTypeInt) ||
                       (DataType == DataTypeBool);
            }
        }
        public bool IsDateTime
        {
            get
            {
                return (DataType == DataTypeDateTime);
            }
        }

        public bool HasSystemInfo
        {
            get
            {
                return this.SystemInfo != null;
            }

        }


    }

}
