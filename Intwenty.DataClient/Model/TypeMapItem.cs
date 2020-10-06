using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Intwenty.DataClient.Model
{
    public enum StringLength { Standard, Long, Short };

    public class TypeMapItem
    {
        public StringLength Length { get; set; }

        public string NetType { get; set; }

        public string IntwentyType { get; set; }

        public DbType DataDbType { get; set; }

        public DBMS DbEngine { get; set; }

        public string DBMSDataType { get; set; }

        public TypeMapItem()
        {
            Length = StringLength.Standard;
        }
    }
}
