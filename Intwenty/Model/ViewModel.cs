using Intwenty.Entity;
using Intwenty.Interface;
using System.Collections.Generic;
using System.Linq;

namespace Intwenty.Model
{


    public class ViewModel : BaseModelItem, ILocalizableTitle, ILocalizableDescription
    {
        public static readonly string MetaTypeUIView = "UIVIEW";

        public ViewModel()
        {
            SetEmptyStrings();
            UserInterface = new List<UserInterfaceModelItem>();
            Functions = new List<FunctionModelItem>();
        }

        public ViewModel(string metatype)
        {
            MetaType = metatype;
            SetEmptyStrings();
            UserInterface = new List<UserInterfaceModelItem>();
            Functions = new List<FunctionModelItem>();
        }

        public ViewModel(ViewItem entity)
        {
            Id = entity.Id;
            MetaType = entity.MetaType;
            Title = entity.Title;
            LocalizedTitle = entity.Title;
            TitleLocalizationKey = entity.TitleLocalizationKey;
            Description = entity.Description;
            LocalizedDescription = entity.Description;
            DescriptionLocalizationKey = entity.DescriptionLocalizationKey;
            SystemMetaCode = entity.SystemMetaCode;
            AppMetaCode = entity.AppMetaCode;
            MetaCode = entity.MetaCode;
            ParentMetaCode = "ROOT";
            Properties = entity.Properties;
            SystemMetaCode = entity.SystemMetaCode;
            Path = entity.Path;
            IsPrimary = entity.IsPrimary;
            IsPublic = entity.IsPublic;
            SetEmptyStrings();
            UserInterface = new List<UserInterfaceModelItem>();
            Functions = new List<FunctionModelItem>();
        }

        private void SetEmptyStrings()
        {
            if (string.IsNullOrEmpty(MetaType)) MetaType = string.Empty;
            if (string.IsNullOrEmpty(Description)) Description = string.Empty;
            if (string.IsNullOrEmpty(AppMetaCode)) AppMetaCode = string.Empty;
            if (string.IsNullOrEmpty(MetaCode)) MetaCode = string.Empty;
            if (string.IsNullOrEmpty(ParentMetaCode)) ParentMetaCode = string.Empty;
            if (string.IsNullOrEmpty(Properties)) Properties = string.Empty;
            if (string.IsNullOrEmpty(Title)) Title = string.Empty;
            if (string.IsNullOrEmpty(LocalizedTitle)) LocalizedTitle = string.Empty;
            if (string.IsNullOrEmpty(JavaScriptObjectName)) JavaScriptObjectName = string.Empty;
            if (string.IsNullOrEmpty(TitleLocalizationKey)) TitleLocalizationKey = string.Empty;
            if (string.IsNullOrEmpty(SystemMetaCode)) SystemMetaCode = string.Empty;
            if (string.IsNullOrEmpty(DescriptionLocalizationKey)) DescriptionLocalizationKey = string.Empty;
            if (string.IsNullOrEmpty(Path)) Path = string.Empty;
        }

        public ApplicationModelItem ApplicationInfo { get; set; }
        public SystemModelItem SystemInfo { get; set; }
        public string SystemMetaCode { get; set; }
        public string TitleLocalizationKey { get; set; }
        public string LocalizedDescription { get; set; }
        public string Description { get; set; }
        public string DescriptionLocalizationKey { get; set; }
        public string AppMetaCode { get; set; }
      
        public string Path { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsPublic { get; set; }
        public bool CanCreateEntities { get; set; }
        public DatabaseModelItem DataTableInfo { get; set; }
        public List<UserInterfaceModelItem> UserInterface { get; set; }
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

    


     

        public bool IsMetaTypeUIView
        {
            get { return MetaType == MetaTypeUIView; }
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

        public bool HasNavigateFunction
        {
            get
            {
                if (Functions.Exists(p => p.IsMetaTypeNavigate))
                    return true;


                return false;
            }
        }

        public FunctionModelItem NavigateFunction
        {
            get
            {
                return Functions.FirstOrDefault(p => p.IsMetaTypeNavigate);
            }
        }

        public bool HasExportFunction
        {
            get
            {
                if (Functions.Exists(p => p.IsMetaTypeExport))
                    return true;


                return false;
            }
        }

        public FunctionModelItem ExportFunction
        {
            get
            {
                return Functions.FirstOrDefault(p => p.IsMetaTypeExport);
            }
        }

        public bool HasSaveFunction
        {
            get
            {
                if (Functions.Exists(p => p.IsMetaTypeSave))
                    return true;


                return false;
            }
        }

        public FunctionModelItem SaveFunction
        {
            get
            {
                return Functions.FirstOrDefault(p => p.IsMetaTypeSave);
            }
        }

        public bool HasDeleteFunction
        {
            get
            {
                if (Functions.Exists(p => p.IsMetaTypeDelete && p.DataTableMetaCode == ApplicationInfo.MetaCode))
                    return true;


                return false;
            }
        }

        public FunctionModelItem DeleteFunction
        {
            get
            {
                return Functions.FirstOrDefault(p => p.IsMetaTypeDelete && p.DataTableMetaCode == ApplicationInfo.MetaCode);
            }
        }

        public bool HasCreateFunction
        {
            get
            {
                if (Functions.Exists(p => p.IsMetaTypeCreate))
                    return true;


                return false;
            }
        }

        public FunctionModelItem CreateFunction
        {
            get
            {
                return Functions.FirstOrDefault(p => p.IsMetaTypeCreate);
            }
        }

        public bool HasEditFunction
        {
            get
            {
                if (Functions.Exists(p => p.IsMetaTypeEdit))
                    return true;


                return false;
            }
        }

        public FunctionModelItem EditFunction
        {
            get
            {
                return Functions.FirstOrDefault(p => p.IsMetaTypeEdit);
            }
        }

        public bool HasPagingFunction
        {
            get
            {
                if (Functions.Exists(p => p.IsMetaTypePaging))
                    return true;


                return false;
            }
        }

        public FunctionModelItem PagingFunction
        {
            get
            {
                return Functions.FirstOrDefault(p => p.IsMetaTypePaging);
            }
        }

        public bool HasFilterFunction
        {
            get
            {
                if (Functions.Exists(p => p.IsMetaTypeFilter))
                    return true;


                return false;
            }
        }

        public FunctionModelItem FilterFunction
        {
            get
            {
                return Functions.FirstOrDefault(p => p.IsMetaTypeFilter);
            }
        }

        public bool IsListView()
        {
            if (UserInterface.Count == 1 && UserInterface.Exists(p => p.IsMetaTypeListInterface))
                return true;


            return false;

        }

        public bool IsInputView()
        {
            if (UserInterface.Exists(p => p.IsMetaTypeInputInterface))
                return true;


            return false;

        }

        public bool IsOnPath(string path)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(this.Path))
                return false;

            var comparepath = this.Path;
            var lastindex = comparepath.IndexOf("/{");
            if (lastindex > 0)
                comparepath = comparepath.Substring(0, lastindex);

            if (path.ToUpper().Contains(comparepath.ToUpper()))
                return true;

            return false;
        }

    }

}
