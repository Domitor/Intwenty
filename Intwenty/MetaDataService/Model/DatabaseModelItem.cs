using Intwenty.Data.Entity;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Intwenty.MetaDataService.Model
{
    public class DatabaseModelItem : BaseModelItem
    {

        public DatabaseModelItem()
        {
            SetEmptyStrings();
        }

        public DatabaseModelItem(string metatype)
        {
            MetaType = metatype;
            SetEmptyStrings();

        }

        public DatabaseModelItem(MetaDataItem entity)
        {
            Id = entity.Id;
            MetaType = entity.MetaType;
            Description = entity.Description;
            AppMetaCode = entity.AppMetaCode;
            MetaCode = entity.MetaCode;
            ParentMetaCode = entity.ParentMetaCode;
            DbName = entity.DbName;
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
        }

        public int Id { get; set; }

        public string Description { get; set; }

        public string AppMetaCode { get; set; }

        public string DbName { get; set; }

        public string TableName { get; set; }

        public string ColumnName { get; set; }

        public string DataType { get; set; }

        public string Domain { get; set; }

        public bool Mandatory { get; set; }


        public int? GetInteger(object value)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {

            }

            return null;
        }

        public decimal? GetDecimal(object value)
        {
            try
            {
                return Convert.ToDecimal(value);
            }
            catch
            {

            }

            return null;
        }

        public DateTime? GetDateTime(object value)
        {
            try
            {
                return Convert.ToDateTime(value);
            }
            catch
            {

            }

            return null;
        }

        public string GetString(object value)
        {
            try
            {
                return Convert.ToString(value);
            }
            catch
            {

            }

            return null;
        }

        public int? GetBoolean(object value)
        {
            try
            {
                int val = 0;
                var s = Convert.ToString(value);
                if (s == "1") val = 1;
                if (s.ToLower() == "true") val = 1;

                return val;
            }
            catch
            {

            }

            return null;
        }

      


        public string DataTypeBool
        {
            get { return "BOOLEAN"; }
        }

        public bool IsDataTypeBool
        {
            get { return DataType == DataTypeBool; }
        }

        public string DataTypeString
        {
            get { return "STRING"; }
        }

        public bool IsDataTypeString
        {
            get { return DataType == DataTypeString; }
        }

        public string DataTypeText
        {
            get { return "TEXT"; }
        }

        public bool IsDataTypeText
        {
            get { return DataType == DataTypeText; }
        }

        public string DataTypeInt
        {
            get { return "INTEGER"; }
        }

        public bool IsDataTypeInt
        {
            get { return DataType == DataTypeInt; }
        }

        public string DataTypeDateTime
        {
            get { return "DATETIME"; }
        }

        public bool IsDataTypeDateTime
        {
            get { return DataType == DataTypeDateTime; }
        }

        public string DataType1Decimal
        {
            get { return "1DECIMAL"; }
        }

        public bool IsDataType1Decimal
        {
            get { return DataType == DataType1Decimal; }
        }

        public string DataType2Decimal
        {
            get { return "2DECIMAL"; }
        }

        public bool IsDataType2Decimal
        {
            get { return DataType == DataType2Decimal; }
        }

        public string DataType3Decimal
        {
            get { return "3DECIMAL"; }
        }

        public bool IsDataType3Decimal
        {
            get { return DataType == DataType3Decimal; }
        }

        public List<string> DataTypes
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

        public string MetaTypeDataValue
        {
            get { return "DATAVALUE"; }
        }

        public bool IsMetaTypeDataValue
        {
            get { return MetaType == MetaTypeDataValue; }
        }

        public string MetaTypeDataValueTable
        {
            get { return "DATAVALUETABLE"; }
        }

        public bool IsMetaTypeDataValueTable
        {
            get { return MetaType == MetaTypeDataValueTable; }
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

        protected override List<string> ValidProperties
        {
            get
            {
                var t = new List<string>();
                return t;
            }
        }


        public List<string> ValidMetaTypes
        {
            get
            {
                var t = new List<string>();
                t.Add("DATAVALUE");
                t.Add("DATAVALUETABLE");
                return t;
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

            if ((DataType == DataType1Decimal) && sqltype.ToUpper() == "DECIMAL(18,1)")
                return true;

            if ((DataType == DataType2Decimal) && sqltype.ToUpper() == "DECIMAL(18,2)")
                return true;

            if ((DataType == DataType3Decimal) && sqltype.ToUpper() == "DECIMAL(18,3)")
                return true;

            if ((DataType == DataTypeInt || DataType == DataTypeBool) && sqltype.ToUpper() == "INT")
                return true;

            return false;
        }

       

      

        public bool HasDomain
        {
            get { return Domain.Contains("APP"); }
        }


    }

}
