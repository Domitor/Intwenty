using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Intwenty.DataClient.Model
{
    public class IntwentyResultColumn : IIntwentyResultColumn
    {
        public string Name { get; }

        public bool IsNumeric { get; set; }

        public bool IsDateTime { get; set; }
    }
}
