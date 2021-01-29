using Intwenty.Entity;
using Intwenty.Interface;
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
            Sections = new List<UISection>();
            Modals = new List<IUIControl>();
            UIStructure = new List<UserInterfaceStructureModelItem>();
        }

        public UserInterfaceModelItem(string systemmetacode, string appmetacode, string viewmetacode, string userinterfacemetacode)
        {
            SetEmptyStrings();
            Sections = new List<UISection>();
            Modals = new List<IUIControl>();
            UIStructure = new List<UserInterfaceStructureModelItem>();
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
            SetEmptyStrings();
            Sections = new List<UISection>();
            Modals = new List<IUIControl>();
            UIStructure = new List<UserInterfaceStructureModelItem>();
        }

        private void SetEmptyStrings()
        {

            if (string.IsNullOrEmpty(MetaCode)) MetaCode = string.Empty;
            if (string.IsNullOrEmpty(Title)) Title = string.Empty;
            if (string.IsNullOrEmpty(ViewMetaCode)) ViewMetaCode = string.Empty;
            if (string.IsNullOrEmpty(SystemMetaCode)) SystemMetaCode = string.Empty;
            if (string.IsNullOrEmpty(AppMetaCode)) AppMetaCode = string.Empty;
        }

        public ApplicationModelItem ApplicationInfo { get; set; }
        public SystemModelItem SystemInfo { get; set; }
        public string SystemMetaCode { get; set; }
        public string AppMetaCode { get; set; }
        public string ViewMetaCode { get; set; }

        public List<UISection> Sections { get; set; }
        public List<IUIControl> Modals { get; set; }

        public List<UserInterfaceStructureModelItem> UIStructure { get; set; }
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

    }

    public class UISection : HashTagPropertyObject, ILocalizableTitle
    {
        public string TitleLocalizationKey { get; set; }
        public string Title { get; set; }
        public string LocalizedTitle { get; set; }
        public List<UIPanel> Panels { get; set; }

        public UISection()
        {
            Panels = new List<UIPanel>();
        }
    }

    public class UIPanel : HashTagPropertyObject, ILocalizableTitle
    {
        public string TitleLocalizationKey { get; set; }
        public string Title { get; set; }
        public string LocalizedTitle { get; set; }

        public bool UseFieldSet { get; set; }

        public List<IUIControl> Controls { get; set; }

        public UIPanel()
        {
            Controls = new List<IUIControl>();
        }
    }

}
