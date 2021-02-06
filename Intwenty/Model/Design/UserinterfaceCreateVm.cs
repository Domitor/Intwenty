using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Model.Design
{
    public class UserinterfaceCreateVm : BaseModelVm
    {
        public int ApplicationId { get; set; }

        public string UIType { get; set; }

        public string ViewMetaCode { get; set; }

        public string Method { get; set; }

        public string MetaCode { get; set; }

        public string DataTableMetaCode { get; set; }
    }
}
