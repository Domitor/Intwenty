using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Intwenty.DataClient.Model
{
    public class IntwentyResultColumn : IIntwentyResultColumn
    {
        public int Index { get; set; }

        public string Name { get; set; }

        public bool IsNumeric { get; set; }

        public bool IsDateTime { get; set; }
    }
}
