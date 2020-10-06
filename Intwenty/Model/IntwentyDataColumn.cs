using Intwenty.DataClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Model
{
    public class IntwentyDataColumn : IIntwentyResultColumn
    {
        public string Name { get; set; }
        public string DataType { get; set; }


        public bool IsNumeric
        {
            get
            {

                return (DataType == DatabaseModelItem.DataType1Decimal) ||
                       (DataType == DatabaseModelItem.DataType2Decimal) ||
                       (DataType == DatabaseModelItem.DataType3Decimal) ||
                       (DataType == DatabaseModelItem.DataTypeInt) ||
                       (DataType == DatabaseModelItem.DataTypeBool);
            }
        }
        public bool IsDateTime
        {
            get
            {
                return (DataType == DatabaseModelItem.DataTypeDateTime);
            }
        }
    }
}
