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
            UIStructure = new List<UserInterfaceStructureModelItem>();
        }

        public UserInterfaceModelItem(string systemmetacode, string appmetacode, string viewmetacode, string userinterfacemetacode)
        {
            SetEmptyStrings();
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

        public List<UserInterfaceStructureModelItem> UIStructure { get; set; }


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

}
