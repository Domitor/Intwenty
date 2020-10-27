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

        public List<PropertyListValue> ValidValues { get; set; }

      

        public IntwentyProperty()
        {
            DisplayName = string.Empty;
            ValueType = string.Empty;
            CodeName = string.Empty;
            ValidFor = new List<string>();
            ValidValues = new List<PropertyListValue>();
            CodeValue = string.Empty;
            DisplayValue = string.Empty;
        }

        public IntwentyProperty(string name, string title, string valuetype)
        {
            DisplayName = title;
            ValueType = valuetype;
            CodeName = name;
            ValidFor = new List<string>();
            ValidValues = new List<PropertyListValue>();
            CodeValue = string.Empty;
            DisplayValue = string.Empty;
        }

        public static IntwentyProperty CreateNew(string name, string value, IntwentyProperty instance=null)
        {
            if (instance == null)
                return new IntwentyProperty() { CodeName = name, CodeValue = value };
            else
            {
                var t = new IntwentyProperty();
                t.CodeName = instance.CodeName;
                t.DisplayName = instance.DisplayName;
                t.DisplayValue = value;
               
                if (instance.IsListType)
                {
                    var v = instance.ValidValues.Find(p => p.CodeValue == value);
                    if (v != null)
                        t.DisplayValue = v.DisplayValue;

                }

                t.ValueType = instance.ValueType;

                foreach (var m in instance.ValidFor)
                    t.ValidFor.Add(m);
                foreach (var m in instance.ValidValues)
                    t.ValidValues.Add(new PropertyListValue() { CodeValue = m.CodeValue, DisplayValue = m.DisplayValue });

                return t;

            }



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



}
