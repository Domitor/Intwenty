using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Model
{
    public class IntwentyPropertyVm
    {
        public string Name { get; set; }

        public string Title { get; set; }

        public string ValueType { get; set; }

        public List<string> ValidFor { get; set; }

        public List<PropertyPresentation> ValidValues { get; set; }


        public IntwentyPropertyVm()
        {
            Title = string.Empty;
            ValueType = string.Empty;
            Name = string.Empty;
            ValidFor = new List<string>();
            ValidValues = new List<PropertyPresentation>();
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
