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
            SetDefaults();
            UIStructure = new List<UserInterfaceStructureModelItem>();
            Sections = new List<UISection>();
            Table = new UITable();
            Functions = new List<FunctionModelItem>();
            CurrentRenderContext = CurrentRenderContextOptions.View;
            ModalInterfaces = new List<UserInterfaceModelItem>();
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
            ParentMetaCode = "ROOT";
            DataTableMetaCode = entity.DataTableMetaCode;
            SetDefaults();
            UIStructure = new List<UserInterfaceStructureModelItem>();
            Sections = new List<UISection>();
            Table = new UITable();
            Functions = new List<FunctionModelItem>();
            CurrentRenderContext = CurrentRenderContextOptions.View;
            ModalInterfaces = new List<UserInterfaceModelItem>();
        }

        private void SetDefaults()
        {
            if (string.IsNullOrEmpty(ParentMetaCode)) ParentMetaCode = string.Empty;
            if (string.IsNullOrEmpty(MetaCode)) MetaCode = string.Empty;
            if (string.IsNullOrEmpty(Title)) Title = string.Empty;
            if (string.IsNullOrEmpty(ViewMetaCode)) ViewMetaCode = string.Empty;
            if (string.IsNullOrEmpty(ViewPath)) ViewPath = string.Empty;
            if (string.IsNullOrEmpty(SystemMetaCode)) SystemMetaCode = string.Empty;
            if (string.IsNullOrEmpty(AppMetaCode)) AppMetaCode = string.Empty;
            if (string.IsNullOrEmpty(DataTableMetaCode)) DataTableMetaCode = string.Empty;
        }

        public List<UserInterfaceModelItem> ModalInterfaces { get; set; }
        public List<FunctionModelItem> Functions { get; set; }
        public List<UserInterfaceStructureModelItem> UIStructure { get; set; }
        public List<UISection> Sections { get; set; }
        public UITable Table { get; set; }
        public List<IntwentyProperty> PropertyCollection { get; set; }
        public List<IntwentyMetaType> UIControls { get; set; }
        public ApplicationModelItem ApplicationInfo { get; set; }
        public SystemModelItem SystemInfo { get; set; }
        public string DataTableMetaCode { get; set; }
        public DatabaseModelItem DataTableInfo { get; set; }
        public string SystemMetaCode { get; set; }
        public string AppMetaCode { get; set; }
        public string ViewMetaCode { get; set; }
        public string ViewPath { get; set; }
        public int PageSize { get; set; }
        public CurrentRenderContextOptions CurrentRenderContext { get; set; }
        public bool IsMetaTypeInputInterface
        {
            get { return MetaType == MetaTypeInputInterface; }
        }

        public bool IsMetaTypeListInterface
        {
            get { return MetaType == MetaTypeListInterface; }
        }
        public bool IsMainApplicationTableInterface
        {
            get
            {

                if (DataTableDbName == ApplicationInfo.DbName)
                    return true;


                return false;
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

        public bool HasDeleteFunction
        {
            get
            {
                if (Functions.Exists(p => p.IsMetaTypeDelete))
                    return true;


                return false;
            }
        }

        public FunctionModelItem DeleteFunction
        {
            get
            {
                return Functions.FirstOrDefault(p => p.IsMetaTypeDelete);
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

        public override string ModelCode
        {
            get { return "UIMODEL"; }
        }

        public string Description
        {
            get {

                return Title;
            
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

        public bool IsSubTableUserInterface
        {
            get { return (DataTableInfo != null && !string.IsNullOrEmpty(DataTableMetaCode) && !DataTableInfo.IsFrameworkItem); }
        }

       


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
        public bool ExcludeOnRender { get; set; }
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
        public bool IsRemoved { get; set; }

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

    public class UITable : HashTagPropertyObject, ILocalizableTitle
    {
        public int Id { get; set; }
        public string TitleLocalizationKey { get; set; }
        public string Title { get; set; }
        public string MetaCode { get; set; }
        public string ParentMetaCode { get; set; }
        public string LocalizedTitle { get; set; }


        public List<UserInterfaceStructureModelItem> Columns { get; set; }

        public UITable()
        {
            Columns = new List<UserInterfaceStructureModelItem>();
        }
    }



}
