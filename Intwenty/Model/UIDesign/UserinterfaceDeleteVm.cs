using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Model.UIDesign
{
    public class UserinterfaceDeleteVm : BaseModelVm
    {
        public int ApplicationId { get; set; }

        public string ViewMetaCode { get; set; }

        public string MetaCode { get; set; }

        public string Method { get; set; }
    }
}
