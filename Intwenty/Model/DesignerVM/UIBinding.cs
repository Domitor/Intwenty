﻿
using Intwenty.Model;

namespace Intwenty.Model.DesignerVM
{

    public class UIBinding : IUIBinding
    {
        public string UIId { get; set; }

        public string Title { get; set; }

        public bool Mandatory { get; set; }

        public string DataTableDbName { get; set; }

        public string DataColumnDbName{ get; set; }


        public UIBinding()
        {
            DataTableDbName = "";
            UIId = "";
            Title = "";
            DataColumnDbName = "";

        }
    }

    public class UIComplexBinding : UIBinding, IUIComplexBinding
    {
        public string ViewName { get; set; }

        public string DomainName { get; set; }

        public string DataColumn2DbName { get; set; }

        public string DataViewColumnDbName{ get; set; }

        public string DataViewColumn2DbName { get; set; }

        public UIComplexBinding()
        {
            DataTableDbName = "";
            UIId = "";
            Title = "";
            DataColumnDbName = "";
            DataColumn2DbName = "";
            ViewName = "";
            DomainName = "";
            DataViewColumnDbName = "";
            DataViewColumn2DbName = "";
           

        }
    }


}