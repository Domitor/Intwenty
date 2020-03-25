
using Intwenty.MetaDataService.Model;

namespace Intwenty.Models
{

    public class UIBinding : IUIBinding
    {
        public string UIId { get; set; }

        public string Title { get; set; }

        public bool Mandatory { get; set; }

        public string DataTableMetaCode { get; set; }

        public string DataColumnMetaCode { get; set; }


        public UIBinding()
        {
            DataTableMetaCode = "";
            UIId = "";
            Title = "";
            DataColumnMetaCode = "";

        }
    }

    public class UIComplexBinding : UIBinding, IUIComplexBinding
    {

        public string DataColumn2MetaCode { get; set; }

        public string DataViewMetaCode { get; set; }

        public string DataViewColumnDbName{ get; set; }

        public string DataViewColumn2DbName { get; set; }

        public UIComplexBinding()
        {
            DataTableMetaCode = "";
            UIId = "";
            Title = "";
            DataColumnMetaCode = "";
            DataColumn2MetaCode = "";
            DataViewMetaCode = "";
            DataViewColumnDbName = "";
            DataViewColumn2DbName = "";
           

        }
    }


}
