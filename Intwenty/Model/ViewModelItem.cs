using Intwenty.Entity;
using Intwenty.Interface;
using System.Collections.Generic;
using System.Linq;

namespace Intwenty.Model
{


    public class ViewModelItem : BaseModelItem
    {
        //META TYPES
        public static readonly string MetaTypeInputView = "INPUTVIEW";
        public static readonly string MetaTypeListView = "LISTVIEW";
       


        public ViewModelItem()
        {
            SetEmptyStrings();
            UserInterface = new List<ViewUserInterfaceModelItem>();
            Functions = new List<FunctionModelItem>();
        }

        public ViewModelItem(string metatype)
        {
            MetaType = metatype;
            SetEmptyStrings();
            UserInterface = new List<ViewUserInterfaceModelItem>();
            Functions = new List<FunctionModelItem>();
        }

        public ViewModelItem(ViewItem entity)
        {
            Id = entity.Id;
            MetaType = entity.MetaType;
            Title = entity.Title;
            LocalizedTitle = entity.Title;
            TitleLocalizationKey = entity.TitleLocalizationKey;
            Description = entity.Description;
            DescriptionLocalizationKey = entity.DescriptionLocalizationKey;
            SystemMetaCode = entity.SystemMetaCode;
            AppMetaCode = entity.AppMetaCode;
            MetaCode = entity.MetaCode;
            ParentMetaCode = "ROOT";
            DataTableMetaCode = entity.DataTableMetaCode;
            DataViewMetaCode = entity.DataViewMetaCode;
            Properties = entity.Properties;
            SystemMetaCode = entity.SystemMetaCode;
            SetEmptyStrings();
            UserInterface = new List<ViewUserInterfaceModelItem>();
            Functions = new List<FunctionModelItem>();
        }

        private void SetEmptyStrings()
        {
            if (string.IsNullOrEmpty(Description)) Description = string.Empty;
            if (string.IsNullOrEmpty(AppMetaCode)) AppMetaCode = string.Empty;
            if (string.IsNullOrEmpty(MetaCode)) MetaCode = string.Empty;
            if (string.IsNullOrEmpty(ParentMetaCode)) ParentMetaCode = string.Empty;
            if (string.IsNullOrEmpty(DataTableMetaCode)) DataTableMetaCode = string.Empty;
            if (string.IsNullOrEmpty(DataViewMetaCode)) DataViewMetaCode = string.Empty;
            if (string.IsNullOrEmpty(Properties)) Properties = string.Empty;
            if (string.IsNullOrEmpty(Title)) Title = string.Empty;
            if (string.IsNullOrEmpty(LocalizedTitle)) LocalizedTitle = string.Empty;
            if (string.IsNullOrEmpty(JavaScriptObjectName)) JavaScriptObjectName = string.Empty;
            if (string.IsNullOrEmpty(TitleLocalizationKey)) TitleLocalizationKey = string.Empty;
            if (string.IsNullOrEmpty(SystemMetaCode)) SystemMetaCode = string.Empty;
            if (string.IsNullOrEmpty(DescriptionLocalizationKey)) DescriptionLocalizationKey = string.Empty;
        }

        public SystemModelItem SystemInfo { get; set; }
        public string SystemMetaCode { get; set; }
        public string TitleLocalizationKey { get; set; }
        public string Description { get; set; }
        public string DescriptionLocalizationKey { get; set; }
        public string AppMetaCode { get; set; }
        public string DataTableMetaCode { get; set; }
        public string DataViewMetaCode { get; set; }
        public DatabaseModelItem DataTableInfo { get; set; }
        public DataViewModelItem DataViewInfo { get; set; }
        public List<ViewUserInterfaceModelItem> UserInterface { get; set; }
        public List<FunctionModelItem> Functions { get; set; }
        public string JavaScriptObjectName { get; set; }

        public override string ModelCode
        {
            get { return "UIVIEWMODEL"; }
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

    


        public bool IsDataTableConnected
        {
            get { return (DataTableInfo != null && !string.IsNullOrEmpty(DataTableMetaCode) && DataTableInfo.IsMetaTypeDataTable); }
        }

   
        public bool IsDataViewConnected
        {
            get { return (DataViewInfo != null && !string.IsNullOrEmpty(DataViewMetaCode) && DataViewInfo.IsMetaTypeDataView); }
        }

     

        public bool IsMetaTypeInputView
        {
            get { return MetaType == MetaTypeInputView; }
        }

        public bool IsMetaTypeListView
        {
            get { return MetaType == MetaTypeListView; }
        }

   

        public string UIId
        {
            get { return MetaCode; }
        }

       
        public bool ReadOnly
        {
            get
            {
                return HasPropertyWithValue("READONLY", "TRUE"); 
            }
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


        public bool HasSystemInfo
        {
            get
            {
                return this.SystemInfo != null;
            }

        }

    }

}
