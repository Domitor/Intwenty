using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Intwenty.DataClient.Model
{
    sealed class IntwentyDataColumn : DbBaseObject
    {
        public int Order { get; set; }

        public bool IsPrimaryKeyColumn { get; set; }

        public bool IsIndexColumn { get; set; }

        public bool IsAutoIncremental { get; set; }

        public bool IsNullNotAllowed { get; set; }

        public bool IsIgnore { get; set; }

        public PropertyInfo Property { get; set; }

        public bool IsInQueryResult { get; set; }

        public string GetNetType()
        {
            if (Property == null)
                return string.Empty;

            var typestring = Property.PropertyType.ToString();

            if (typestring.Contains("["))
            {
                var index1 = typestring.IndexOf("[");
                var index2 = typestring.IndexOf("]");
                typestring = typestring.Substring(index1 + 1, (index2) - (index1 + 1));
            }

            return typestring.ToUpper();
        }

    }
}
