using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Model.Design
{
    public class FunctionVm : BaseModelVm
    {

        public FunctionVm()
        {

        }

        public FunctionVm(FunctionModelItem model)
        {

            ApplicationId = model.ApplicationInfo.Id; 
            Id = model.Id; 
            MetaCode = model.MetaCode;
            MetaType = model.MetaType;
            OwnerMetaCode = model.OwnerMetaCode;
            OwnerMetaType = model.OwnerMetaType;
            ActionPath = model.ActionPath;
            ActionMetaCode = model.ActionMetaCode;
            ActionMetaType = model.ActionMetaType;
            Properties = model.Properties;
            Title = model.Title;
            IsModalAction = model.IsModalAction;
        }

        public int ApplicationId { get; set; }

        public string OwnerMetaCode { get; set; }

        public string OwnerMetaType { get; set; }

        public string MetaCode { get; set; }

        public string MetaType { get; set; }

        public string DataTableMetaCode { get; set; }

        public string Title { get; set; }

        public string ActionPath { get; set; }

        public string ActionMetaCode { get; set; }

        public string ActionMetaType { get; set; }

        public bool IsRemoved { get; set; }

        public bool IsModalAction { get; set; }

    }
}
