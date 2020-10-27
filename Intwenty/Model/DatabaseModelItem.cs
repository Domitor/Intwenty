﻿using Intwenty.Data.Entity;
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
            SetEmptyStrings();
        }

        public DatabaseModelItem(string metatype)
        {
            MetaType = metatype;
            SetEmptyStrings();

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
            if (IsMetaTypeDataTable)
                TableName = DbName;
            if (IsMetaTypeDataColumn)
                ColumnName = DbName;


            SetEmptyStrings();
        }

        public static DatabaseModelItem CreateFrameworkColumn(int id, string appmetacode, string tablename, string dbname, string datatype, string parentmetacode="ROOT")
        {
            var res = new DatabaseModelItem();
            res.Id = id;
            res.MetaType = MetaTypeDataColumn;
            res.AppMetaCode = appmetacode;
            res.ColumnName = dbname;
            res.DbName = dbname;
            res.TableName = tablename;
            res.DataType = datatype;
            res.Title = dbname;
            res.MetaCode = dbname.ToUpper();
            res.ParentMetaCode = parentmetacode;
            res.Properties = string.Empty;
            res.IsFrameworkItem = true;
            res.Description = "Required by Intwenty";

            return res;
        }

        private void SetEmptyStrings()
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
        }

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

        public static List<IntwentyProperty> GetAvailableProperties()
        {
     
            var res = new List<IntwentyProperty>();

            var prop = new IntwentyProperty("DEFVALUE", "Default Value", "LIST");
            prop.ValidFor.Add(MetaTypeDataColumn);
            prop.ValidValues.Add(new PropertyListValue() {  CodeValue = "NONE", DisplayValue = "None" });
            prop.ValidValues.Add(new PropertyListValue() { CodeValue = "AUTO", DisplayValue = "Automatic" });
            res.Add(prop);

            prop = new IntwentyProperty("DEFVALUE_START", "Default Value Start", "NUMERIC");
            prop.ValidFor.Add(MetaTypeDataColumn);
            res.Add(prop);

            prop = new IntwentyProperty("DEFVALUE_PREFIX", "Default Value Prefix", "STRING");
            prop.ValidFor.Add(MetaTypeDataColumn);
            res.Add(prop);

            prop = new IntwentyProperty("DEFVALUE_SEED", "Default Value Seed", "NUMERIC");
            prop.ValidFor.Add(MetaTypeDataColumn);
            res.Add(prop);

            prop = new IntwentyProperty("MANDATORY", "Required", "BOOLEAN");
            prop.ValidFor.Add(MetaTypeDataColumn);
            res.Add(prop);

            prop = new IntwentyProperty("UNIQUE", "Unique", "BOOLEAN");
            prop.ValidFor.Add(MetaTypeDataColumn);
            res.Add(prop);

            prop = new IntwentyProperty("DOMAIN", "Domain", "STRING");
            prop.ValidFor.Add(MetaTypeDataColumn);
            res.Add(prop);

            return res;
            
        }

        public static List<IntwentyMetaType> GetAvailableMetaTypes()
        {
            
            var t = new List<IntwentyMetaType>();
            t.Add(new IntwentyMetaType() { Code = MetaTypeDataColumn, Title = "Column" });
            t.Add(new IntwentyMetaType() { Code = MetaTypeDataTable, Title = "Table" });
            return t;
            
        }

        public override bool HasValidMetaType
        {
            get
            {
                if (string.IsNullOrEmpty(MetaType))
                    return false;

                if (GetAvailableMetaTypes().Exists(p => p.Code == MetaType))
                    return true;

                return false;

            }
        }

        public override bool HasValidProperties
        {
            get
            {
                foreach (var prop in GetProperties())
                {
                    if (!GetAvailableProperties().Exists(p => p.CodeName == prop))
                        return false;
                }
                return true;
            }
        }

        public override List<IntwentyProperty> SelectableProperties
        {
            get 
            { 
                var t = GetAvailableProperties().Where(p => p.ValidFor.Contains(MetaType));
                if (t != null)
                    return t.ToList();
                else
                    return new List<IntwentyProperty>();
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

     
    }

}
