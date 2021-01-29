using Intwenty.Areas.Identity.Models;
using Intwenty.Entity;
using Intwenty.Interface;
using System.Collections.Generic;
using System.Linq;

namespace Intwenty.Model
{


    public class FunctionModelItem : BaseModelItem
    {
        //META TYPES
        public static readonly string MetaTypeCreate = "CREATE";
        public static readonly string MetaTypeEdit = "EDIT";
        public static readonly string MetaTypeSave = "SAVE";
        public static readonly string MetaTypeDelete = "DELETE";
        public static readonly string MetaTypeBack = "BACK";



        public FunctionModelItem()
        {
            SetEmptyStrings();
        }

        public FunctionModelItem(string metatype)
        {
            MetaType = metatype;
            SetEmptyStrings();
        }

        public FunctionModelItem(FunctionItem entity)
        {
            Id = entity.Id;
            MetaType = entity.MetaType;
            Title = entity.Title;
            LocalizedTitle = entity.Title;
            TitleLocalizationKey = entity.TitleLocalizationKey;
            SystemMetaCode = entity.SystemMetaCode;
            AppMetaCode = entity.AppMetaCode;
            ParentMetaCode = "ROOT";
            Properties = entity.Properties;
            SystemMetaCode = entity.SystemMetaCode;
            Path = entity.Path;
            ViewMetaCode = entity.ViewMetaCode;
            DataTableMetaCode = entity.DataTableMetaCode;
            RequiredAuthorization = (IntwentyPermission)entity.RequiredAuthorization;
            SetEmptyStrings();
        }

        private void SetEmptyStrings()
        {
            if (string.IsNullOrEmpty(AppMetaCode)) AppMetaCode = string.Empty;
            if (string.IsNullOrEmpty(MetaCode)) MetaCode = string.Empty;
            if (string.IsNullOrEmpty(ParentMetaCode)) ParentMetaCode = string.Empty;
            if (string.IsNullOrEmpty(Properties)) Properties = string.Empty;
            if (string.IsNullOrEmpty(Title)) Title = string.Empty;
            if (string.IsNullOrEmpty(LocalizedTitle)) LocalizedTitle = string.Empty;
            if (string.IsNullOrEmpty(TitleLocalizationKey)) TitleLocalizationKey = string.Empty;
            if (string.IsNullOrEmpty(SystemMetaCode)) SystemMetaCode = string.Empty;
            if (string.IsNullOrEmpty(Path)) Path = string.Empty;
            if (string.IsNullOrEmpty(ViewMetaCode)) ViewMetaCode = string.Empty;
            if (string.IsNullOrEmpty(DataTableMetaCode)) DataTableMetaCode = string.Empty;
        }

        public ApplicationModelItem ApplicationInfo { get; set; }
        public SystemModelItem SystemInfo { get; set; }
        public string SystemMetaCode { get; set; }
        public string TitleLocalizationKey { get; set; }
        public string AppMetaCode { get; set; }
        public string ViewMetaCode { get; set; }
        public string DataTableMetaCode { get; set; }


        public IntwentyPermission RequiredAuthorization { get; set; }
        public string Path { get; set; }

        public override string ModelCode
        {
            get { return "FUNCTIONMODEL"; }
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

    


        public bool IsMetaTypeBack
        {
            get { return MetaType == MetaTypeBack; }
        }

        public bool IsMetaTypeCreate
        {
            get { return MetaType == MetaTypeCreate; }
        }

        public bool IsMetaTypeDelete
        {
            get { return MetaType == MetaTypeDelete; }
        }

        public bool IsMetaTypeEdit
        {
            get { return MetaType == MetaTypeEdit; }
        }

        public bool IsMetaTypeSave
        {
            get { return MetaType == MetaTypeSave; }
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
