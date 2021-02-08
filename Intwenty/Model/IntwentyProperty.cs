using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Model
{

   

    public class IntwentyProperty
    {
        public string CodeName { get; set; }

        public string DisplayName { get; set; }

        public string CodeValue { get; set; }

        public string DisplayValue { get; set; }

        public string ValueType { get; set; }

        public List<string> ValidFor { get; set; }

        public List<SelectablePropertyValue> ValidValues { get; set; }

      

        public IntwentyProperty()
        {
            DisplayName = string.Empty;
            ValueType = string.Empty;
            CodeName = string.Empty;
            ValidFor = new List<string>();
            ValidValues = new List<SelectablePropertyValue>();
            CodeValue = string.Empty;
            DisplayValue = string.Empty;
        }

        public IntwentyProperty(string name, string title, string valuetype)
        {
            DisplayName = title;
            ValueType = valuetype;
            CodeName = name;
            ValidFor = new List<string>();
            ValidValues = new List<SelectablePropertyValue>();
            CodeValue = string.Empty;
            DisplayValue = string.Empty;
        }



        public bool IsListType
        {
            get {
                return ValueType == "LIST";
            }
        }
        public bool IsStringType
        {
            get
            {
                return ValueType == "STRING";
            }
        }
        public bool IsNumericType
        {
            get
            {
                return ValueType == "NUMERIC";
            }
        }
        public bool IsBoolType
        {
            get
            {
                return ValueType == "BOOLEAN";
            }
        }

    }

    public class SelectablePropertyValue
    {
        public string CodeValue { get; set; }

        public string DisplayValue { get; set; }


    }

    public class PropertyValue
    {
        public string CodeName { get; set; }

        public string CodeValue { get; set; }

        public string DisplayValue { get; set; }


    }



}
