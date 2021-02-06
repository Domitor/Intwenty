using Intwenty.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intwenty.Model.Design
{


    public class UserInterfaceInputDesignVm : BaseModelVm
    {
        public int ApplicationId { get; set; }
        public string MetaType { get; set; }
        public string MetaCode { get; set; }
        public bool ShowComponents { get; set; }
        public List<UISection> Sections { get; set; }
        public List<IntwentyProperty> PropertyCollection { get; set; }
        public List<IntwentyMetaType> UIControls { get; set; }

        public UserInterfaceInputDesignVm()
        {
            PropertyCollection = IntwentyRegistry.IntwentyProperties;
            UIControls = new List<IntwentyMetaType>();
            Sections = new List<UISection>();
            MetaType = UserInterfaceModelItem.MetaTypeInputInterface;
            Properties = "";

            var temp = IntwentyRegistry.IntwentyMetaTypes.Where(p => p.ModelCode == "UISTRUCTUREMODEL").ToList();

            UIControls.Add(temp.Find(p => p.Code == UserInterfaceStructureModelItem.MetaTypeCheckBox));
            UIControls.Add(temp.Find(p => p.Code == UserInterfaceStructureModelItem.MetaTypeComboBox));
            UIControls.Add(temp.Find(p => p.Code == UserInterfaceStructureModelItem.MetaTypeDatePicker));
            UIControls.Add(temp.Find(p => p.Code == UserInterfaceStructureModelItem.MetaTypeEmailBox));
            UIControls.Add(temp.Find(p => p.Code == UserInterfaceStructureModelItem.MetaTypeImage));
            UIControls.Add(temp.Find(p => p.Code == UserInterfaceStructureModelItem.MetaTypeImageBox));
            UIControls.Add(temp.Find(p => p.Code == UserInterfaceStructureModelItem.MetaTypeLabel));
            UIControls.Add(temp.Find(p => p.Code == UserInterfaceStructureModelItem.MetaTypeLookUp));
            UIControls.Add(temp.Find(p => p.Code == UserInterfaceStructureModelItem.MetaTypeNumBox));
            UIControls.Add(temp.Find(p => p.Code == UserInterfaceStructureModelItem.MetaTypePasswordBox));
            UIControls.Add(temp.Find(p => p.Code == UserInterfaceStructureModelItem.MetaTypeStaticHTML));
            UIControls.Add(temp.Find(p => p.Code == UserInterfaceStructureModelItem.MetaTypeTextArea));
            UIControls.Add(temp.Find(p => p.Code == UserInterfaceStructureModelItem.MetaTypeTextBlock));
            UIControls.Add(temp.Find(p => p.Code == UserInterfaceStructureModelItem.MetaTypeTextBox));
        }

    }

   

   

   
}
