using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Model.UIDesign
{
    public class FunctionVm : BaseModelVm
    {

        public int ApplicationId { get; set; }

        public string ViewMetaCode { get; set; }

        public string MetaCode { get; set; }

        public string FunctionType { get; set; }

        public string DataTableMetaCode { get; set; }

        public string Title { get; set; }

        public string Path { get; set; }
    }
}
