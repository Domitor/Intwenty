﻿using Intwenty.Data.Entity;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Intwenty.Model
{
    public class DatabaseModelItem : BaseModelItem
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
            Domain = entity.Domain;
            Mandatory = entity.Mandatory;
            Properties = entity.Properties;
            SetEmptyStrings();
        }

        private void SetEmptyStrings()
        {
            if (string.IsNullOrEmpty(Description)) Description = string.Empty;
            if (string.IsNullOrEmpty(AppMetaCode)) AppMetaCode = string.Empty;
            if (string.IsNullOrEmpty(MetaCode)) MetaCode = string.Empty;
            if (string.IsNullOrEmpty(ParentMetaCode)) ParentMetaCode = string.Empty;
            if (string.IsNullOrEmpty(DbName)) DbName = string.Empty;
            if (string.IsNullOrEmpty(DataType)) DataType = string.Empty;
            if (string.IsNullOrEmpty(Domain)) Domain = string.Empty;
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

        public string Domain { get; set; }

        public bool Mandatory { get; set; }


        public static List<string> DataTypes
        {
            get
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
        }

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
                t.Add(MetaTypeDataColumn);
                t.Add(MetaTypeDataTable);
                return t;
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

                if (DataTypes.Contains(DataType))
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


        public string SQLDataType
        {
            get
            {

                if (DataType == DataTypeString)
                    return "NVarChar(300)";

                if (DataType == DataTypeText)
                    return "NVarChar(max)";

                if (DataType == DataTypeDateTime)
                    return "Datetime";

                if (DataType == DataType1Decimal)
                    return "Decimal(18,1)";

                if (DataType == DataType2Decimal)
                    return "Decimal(18,2)";

                if (DataType == DataType3Decimal)
                    return "Decimal(18,3)";

                if (DataType == DataTypeBool ||
                    DataType == DataTypeInt)
                    return "Int";

                return "NVarChar(50)";
            }
        }

        public bool IsValidSQLDataType(string sqltype)
        {
            
            if (DataType == DataTypeString && sqltype.ToUpper() == "NVARCHAR")
                return true;

            if (DataType == DataTypeText && sqltype.ToUpper() == "NVARCHAR")
                return true;

            if (DataType == DataTypeDateTime && sqltype.ToUpper() == "DATETIME")
                return true;

            if ((DataType == DataType1Decimal) && sqltype.ToUpper() == "DECIMAL")
                return true;

            if ((DataType == DataType2Decimal) && sqltype.ToUpper() == "DECIMAL")
                return true;

            if ((DataType == DataType3Decimal) && sqltype.ToUpper() == "DECIMAL")
                return true;

            if ((DataType == DataTypeInt || DataType == DataTypeBool) && sqltype.ToUpper() == "INT")
                return true;

            return false;
        }

        public bool HasDomain
        {
            get { return Domain.Contains("APP."); }
        }


    }

}