
using Intwenty.Interface;
using Intwenty.Model;

namespace Intwenty.Model.UIRendering
{

    public class UIBinding : HashTagPropertyObject, IUIBinding
    {
        public string UIId { get; set; }

        public string Title { get; set; }

        public bool Mandatory { get; set; }

        public bool ReadOnly { get; set; }

        public string DataTableDbName { get; set; }

        public string DataColumnDbName{ get; set; }

        public string RawHTML { get; set; }

        public string JavaScriptObjectName { get; set; }

        public UIBinding()
        {
            DataTableDbName = "";
            UIId = "";
            Title = "";
            DataColumnDbName = "";
            RawHTML = "";

        }
    }

    public class UIComplexBinding : UIBinding, IUIComplexBinding
    {
        public string DataViewMetaCode { get; set; }
        public string DataViewTitle { get; set; }
        public string DomainName { get; set; }
        public string DataColumn2DbName { get; set; }
        public string DataViewColumnDbName{ get; set; }
        public string DataViewColumnTitle { get; set; }
        public string DataViewColumn2DbName { get; set; }
        public string DataViewColumn2Title { get; set; }

        public UIComplexBinding()
        {
            DataTableDbName = "";
            UIId = "";
            Title = "";
            DataColumnDbName = "";
            DataColumn2DbName = "";
            DataViewMetaCode = "";
            DomainName = "";
            DataViewColumnDbName = "";
            DataViewColumn2DbName = "";
            DataViewColumnTitle = "";
            DataViewColumn2Title = "";


        }
    }


}
