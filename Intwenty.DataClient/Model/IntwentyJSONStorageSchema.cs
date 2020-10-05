using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.DataClient.Model
{
    public enum IntwentyJSONFieldDataType { INTEGER, DECIMAL, BOOLEAN, TEXT, DATE, DATETIME };

   

    public class IntwentyJSONObjectSchema : IIntwentyJSONObjectSchema
    {
        public string TableName { get; set; }

        public string JSONStoreColumnName { get; set; }

        public List<IntwentyJSONField> Fields { get; set; }

        IEnumerable<IIntwentyJSONField> IIntwentyJSONObjectSchema.Fields => Fields;

        public IntwentyJSONObjectSchema()
        {
            Fields = new List<IntwentyJSONField>();
        }

    }

    public class IntwentyJSONField : IIntwentyJSONField
    {
        public string Name { get; set; }
        public bool IsAutoIncremental { get; set; }
        public bool IsPrimaryKey { get; set; }
        public IntwentyJSONFieldDataType DataType { get; set; }

    }
}
