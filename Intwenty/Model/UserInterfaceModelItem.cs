using Intwenty.Entity;
using Intwenty.Interface;
using Intwenty.Model;
using System.Collections.Generic;
using System.Linq;

namespace Intwenty.Model
{


    public class UserInterfaceModelItem : BaseModelItem
    {

        //META TYPES
        public static readonly string MetaTypeInputInterface = "INPUTINTERFACE";
        public static readonly string MetaTypeListInterface = "LISTINTERFACE";



        public UserInterfaceModelItem()
        {
            SetEmptyStrings();
            UIStructure = new List<UserInterfaceStructureModelItem>();
            Sections = new List<UISection>();
        }

        public UserInterfaceModelItem(UserInterfaceItem entity)
        {
            Id = entity.Id;
            SystemMetaCode = entity.SystemMetaCode;
            AppMetaCode = entity.AppMetaCode;
            SystemMetaCode = entity.SystemMetaCode;
            ViewMetaCode = entity.ViewMetaCode;
            MetaCode = entity.MetaCode;
            MetaType = entity.MetaType;
            DataTableMetaCode = entity.DataTableMetaCode;
            if (MetaType == MetaTypeInputInterface)
            {
                Title = "Input UI";
            }
            if (MetaType == MetaTypeListInterface)
            {
                Title = "List UI";
            }
            SetEmptyStrings();
            UIStructure = new List<UserInterfaceStructureModelItem>();
            Sections = new List<UISection>();
        }

        private void SetEmptyStrings()
        {

            if (string.IsNullOrEmpty(MetaCode)) MetaCode = string.Empty;
            if (string.IsNullOrEmpty(Title)) Title = string.Empty;
            if (string.IsNullOrEmpty(ViewMetaCode)) ViewMetaCode = string.Empty;
            if (string.IsNullOrEmpty(SystemMetaCode)) SystemMetaCode = string.Empty;
            if (string.IsNullOrEmpty(AppMetaCode)) AppMetaCode = string.Empty;
            if (string.IsNullOrEmpty(DataTableMetaCode)) DataTableMetaCode = string.Empty;
        }


        public List<UserInterfaceStructureModelItem> UIStructure { get; set; }
        public List<UISection> Sections { get; set; }
        public List<IntwentyProperty> PropertyCollection { get; set; }
        public List<IntwentyMetaType> UIControls { get; set; }
        public ApplicationModelItem ApplicationInfo { get; set; }
        public SystemModelItem SystemInfo { get; set; }
        public string DataTableMetaCode { get; set; }
        public DatabaseModelItem DataTableInfo { get; set; }
        public string SystemMetaCode { get; set; }
        public string AppMetaCode { get; set; }
        public string ViewMetaCode { get; set; }
        public int PageSize { get; set; }
      
  

        public bool IsMetaTypeInputInterface
        {
            get { return MetaType == MetaTypeInputInterface; }
        }

        public bool IsMetaTypeListInterface
        {
            get { return MetaType == MetaTypeListInterface; }
        }


        public override string ModelCode
        {
            get { return "UIMODEL"; }
        }

        public string Description
        {
            get {

                var s = DataTableDbName;
                if (string.IsNullOrEmpty(s))
                    return Title;
                else
                    return string.Format(Title + " ({0})",s);
            
            }
        }

        public override bool HasValidMetaType
        {
            get
            {
                if (string.IsNullOrEmpty(MetaType))
                    return false;


                if (!IntwentyRegistry.IntwentyMetaTypes.Exists(p => p.Code == MetaType && p.ModelCode == ModelCode))
                    return false;

                return true;

            }
        }

        public override bool HasValidProperties
        {
            get
            {
                foreach (var prop in GetProperties())
                {
                    if (!IntwentyRegistry.IntwentyProperties.Exists(p => p.CodeName == prop && p.ValidFor.Contains(MetaType)))
                        return false;
                }
                return true;
            }
        }

        public bool HasSystemInfo
        {
            get
            {
                return this.SystemInfo != null;
            }

        }

        public bool HasApplicationInfo
        {
            get
            {
                return this.ApplicationInfo != null;
            }

        }

        public bool IsDataTableConnected
        {
            get { return (DataTableInfo != null && !string.IsNullOrEmpty(DataTableMetaCode) && DataTableInfo.IsMetaTypeDataTable); }
        }

        public string DataTableDbName
        {
            get
            {
                if (IsDataTableConnected)
                    return DataTableInfo.DbName;

                return string.Empty;
            }
        }

        public List<IUIControl> GetModals()
        {
            var res = new List<IUIControl>();
            foreach (var ctrl in UIStructure)
            {
                if (ctrl.IsMetaTypeLookUp)
                    res.Add(ctrl);
            }
            return res;
        }

        /*
        public List<UISection> GetSections()
        {

            var res = new List<UISection>();

            foreach (var sect in UIStructure.Where(p => p.IsMetaTypeSection).OrderBy(p => p.RowOrder).ThenBy(p => p.ColumnOrder))
            {
                var section = new UISection() { Properties = sect.Properties, Title = sect.Title, LocalizedTitle = sect.LocalizedTitle, TitleLocalizationKey = sect.TitleLocalizationKey };
                foreach (var pnl in UIStructure.Where(p => p.IsMetaTypePanel && p.ParentMetaCode == sect.MetaCode).OrderBy(p => p.RowOrder).ThenBy(p => p.ColumnOrder))
                {
                    var panel = new UIPanel() { Properties = pnl.Properties, Title = pnl.Title, LocalizedTitle = pnl.LocalizedTitle, TitleLocalizationKey = pnl.TitleLocalizationKey };
                    if (!string.IsNullOrEmpty(panel.Title))
                        panel.UseFieldSet = true;

                    foreach (var ctrl in UIStructure.Where(p => p.ParentMetaCode == pnl.MetaCode).OrderBy(p => p.RowOrder).ThenBy(p => p.ColumnOrder))
                    {
                        //Should not happen, just in case, remove container controls
                        if (ctrl.IsUIContainerType)
                            continue;

                        if ((ctrl.IsUIBindingType || ctrl.IsUIComplexBindingType) && (!ctrl.IsDataColumn1Connected || !ctrl.IsDataTableConnected))
                            continue;

                        if (ctrl.IsUIComplexBindingType && (!ctrl.IsDataViewColumn1Connected || !ctrl.IsDataViewConnected))
                            continue;

                        if (IsMetaTypeInputInterface)
                            ctrl.JavaScriptObjectName = "model";
                        if (IsMetaTypeListInterface)
                            ctrl.JavaScriptObjectName = "item";

       
        
                        panel.Controls.Add(ctrl);
                    }
                    section.Panels.Add(panel);
                }
                res.Add(section);
            }


            return res;


        }
        */
       


    }


    public class UISection : HashTagPropertyObject, ILocalizableTitle
    {
        public int Id { get; set; }
        public string TitleLocalizationKey { get; set; }
        public string Title { get; set; }
        public string LocalizedTitle { get; set; }
        public int RowOrder { get; set; }
        public int ColumnOrder { get; set; }
        public string MetaCode { get; set; }
        public string ParentMetaCode { get; set; }
        public int LayoutPanelCount { get; set; }
        public bool Collapsible { get; set; }
        public bool StartExpanded { get; set; }
        public bool IsRemoved { get; set; }
        public List<UIPanel> LayoutPanels { get; set; }
        public List<LayoutRow> LayoutRows { get; set; }

        public UISection()
        {
            Title = "";
            ParentMetaCode = "";
            MetaCode = "";
            LayoutPanels = new List<UIPanel>();
            LayoutRows = new List<LayoutRow>();

        }
       
    }

    public class UIPanel : HashTagPropertyObject, ILocalizableTitle
    {
        public int Id { get; set; }
        public int ColumnOrder { get; set; }
        public int RowOrder { get; set; }
        public string TitleLocalizationKey { get; set; }
        public string Title { get; set; }
        public string MetaCode { get; set; }
        public string ParentMetaCode { get; set; }
        public string LocalizedTitle { get; set; }
        public bool UseFieldSet { get; set; }

        public List<UserInterfaceStructureModelItem> Controls { get; set; }

        public UIPanel()
        {
            Controls = new List<UserInterfaceStructureModelItem>();
        }
    }

    public class LayoutRow
    {
        public int RowOrder { get; set; }

        public List<UserInterfaceStructureModelItem> UserInputs { get; set; }

        public LayoutRow()
        {
            UserInputs = new List<UserInterfaceStructureModelItem>();
        }
    }

   

}
