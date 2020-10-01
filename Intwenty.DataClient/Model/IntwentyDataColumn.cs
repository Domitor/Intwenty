using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Intwenty.DataClient.Model
{
    public class IntwentyDataColumn : DbBaseObject
    {
        public int Order { get; set; }

        public bool IsPrimaryKeyColumn { get; set; }

        public bool IsIndexColumn { get; set; }

        public bool IsAutoIncremental { get; set; }

        public bool IsNullNotAllowed { get; set; }

        public bool IsIgnore { get; set; }

        public PropertyInfo Property { get; set; }

        public bool IsInQueryResult { get; set; }

    }
}
