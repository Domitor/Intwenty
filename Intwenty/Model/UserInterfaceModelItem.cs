using Intwenty.Entity;
using Intwenty.Interface;
using System.Collections.Generic;
using System.Linq;

namespace Intwenty.Model
{


    public class UserInterfaceModelItem : BaseModelItem, IUIBinding, IUIComplexBinding, ILocalizableTitle, IUIControl, IEditListViewColumn
    {
        //META TYPES
        public static readonly string MetaTypeTextBlock = "TEXTBLOCK";
        public static readonly string MetaTypeLabel = "LABEL";
        public static readonly string MetaTypeImage = "IMAGE";
        public static readonly string MetaTypeStaticHTML = "STATICHTML";
        public static readonly string MetaTypeCreateView = "CREATEVIEW";
        public static readonly string MetaTypeEditView = "EDITVIEW";
        public static readonly string MetaTypeDetailView = "DETAILVIEW";
        public static readonly string MetaTypeListView = "LISTVIEW";
        public static readonly string MetaTypeEmailBox = "EMAILBOX";
        public static readonly string MetaTypePasswordBox = "PASSWORDBOX";
        public static readonly string MetaTypeTextBox = "TEXTBOX";
        public static readonly string MetaTypeTextArea = "TEXTAREA";
        public static readonly string MetaTypeLookUp = "LOOKUP";
        public static readonly string MetaTypeNumBox = "NUMBOX";
        public static readonly string MetaTypeCheckBox = "CHECKBOX";
        public static readonly string MetaTypeComboBox = "COMBOBOX";
        public static readonly string MetaTypeEditListView = "EDITLISTVIEW";
        public static readonly string MetaTypeEditListViewColumn = "EDITLISTVIEWCOLUMN";
        public static readonly string MetaTypePanel = "PANEL";
        public static readonly string MetaTypeDatePicker = "DATEPICKER";
        public static readonly string MetaTypeEditGrid = "EDITGRID";
        public static readonly string MetaTypeSection = "SECTION";
        public static readonly string MetaTypeImageBox = "IMAGEBOX";
        public static readonly string MetaTypeEditGridTextBox = "EDITGRID_TEXTBOX";
        public static readonly string MetaTypeEditGridNumBox = "EDITGRID_NUMBOX";
        public static readonly string MetaTypeEditGridDatePicker = "EDITGRID_DATEPICKER";
        public static readonly string MetaTypeEditGridCheckBox = "EDITGRID_CHECKBOX";
        public static readonly string MetaTypeEditGridComboBox = "EDITGRID_COMBOBOX";
        public static readonly string MetaTypeEditGridLookUp = "EDITGRID_LOOKUP";
        public static readonly string MetaTypeEditGridEmailBox = "EDITGRID_EMAILBOX";
        public static readonly string MetaTypeEditGridStaticHTML = "EDITGRID_STATICHTML";


        public UserInterfaceModelItem()
        {
            SetEmptyStrings();
            Children = new List<IUIControl>();
        }

        public UserInterfaceModelItem(string metatype)
        {
            MetaType = metatype;
            SetEmptyStrings();
            Children = new List<IUIControl>();
        }

        public UserInterfaceModelItem(UserInterfaceItem entity)
        {
            Id = entity.Id;
            MetaType = entity.MetaType;
            Title = entity.Title;
            TitleLocalizationKey = entity.TitleLocalizationKey;
            Description = entity.Description;
            AppMetaCode = entity.AppMetaCode;
            MetaCode = entity.MetaCode;
            ParentMetaCode = entity.ParentMetaCode;
            ColumnOrder = entity.ColumnOrder;
            RowOrder = entity.RowOrder;
            DataTableMetaCode = entity.DataTableMetaCode;
            DataColumn1MetaCode = entity.DataColumn1MetaCode;
            DataColumn2MetaCode = entity.DataColumn2MetaCode;
            DataViewMetaCode = entity.DataViewMetaCode;
            DataViewColumn1MetaCode = entity.DataViewColumn1MetaCode;
            DataViewColumn2MetaCode = entity.DataViewColumn2MetaCode;
            Domain = entity.Domain;
            Properties = entity.Properties;
            RawHTML = entity.RawHTML;
            SetEmptyStrings();
            Children = new List<IUIControl>();
        }

        private void SetEmptyStrings()
        {
            if (string.IsNullOrEmpty(Description)) Description = string.Empty;
            if (string.IsNullOrEmpty(AppMetaCode)) AppMetaCode = string.Empty;
            if (string.IsNullOrEmpty(MetaCode)) MetaCode = string.Empty;
            if (string.IsNullOrEmpty(ParentMetaCode)) ParentMetaCode = string.Empty;
            if (string.IsNullOrEmpty(DataTableMetaCode)) DataTableMetaCode = string.Empty;
            if (string.IsNullOrEmpty(DataColumn1MetaCode)) DataColumn1MetaCode = string.Empty;
            if (string.IsNullOrEmpty(DataColumn2MetaCode)) DataColumn2MetaCode = string.Empty;
            if (string.IsNullOrEmpty(DataViewMetaCode)) DataViewMetaCode = string.Empty;
            if (string.IsNullOrEmpty(DataViewColumn1MetaCode)) DataViewColumn1MetaCode = string.Empty;
            if (string.IsNullOrEmpty(DataViewColumn2MetaCode)) DataViewColumn2MetaCode = string.Empty;
            if (string.IsNullOrEmpty(Domain)) Domain = string.Empty;
            if (string.IsNullOrEmpty(Properties)) Properties = string.Empty;
            if (string.IsNullOrEmpty(Title)) Title = string.Empty;
            if (string.IsNullOrEmpty(RawHTML)) RawHTML = string.Empty;
            if (string.IsNullOrEmpty(TitleLocalizationKey)) TitleLocalizationKey = string.Empty;
        }

        public string TitleLocalizationKey { get; set; }
        public string Description { get; set; }
        public string AppMetaCode { get; set; }
        public string DataTableMetaCode { get; set; }
        public string DataViewMetaCode { get; set; }
        public string DataColumn1MetaCode { get; set; }
        public string DataColumn2MetaCode { get; set; }
        public string DataViewColumn1MetaCode { get; set; }
        public string DataViewColumn2MetaCode { get; set; }
        public int ColumnOrder { get; set; }
        public int RowOrder { get; set; }
        public string Domain { get; set; }
        public string RawHTML { get; set; }
        public DatabaseModelItem DataTableInfo { get; set; }
        public DatabaseModelItem DataColumn1Info { get; set; }
        public DatabaseModelItem DataColumn2Info { get; set; }
        public DataViewModelItem DataViewInfo { get; set; }
        public DataViewModelItem DataViewColumn1Info { get; set; }
        public DataViewModelItem DataViewColumn2Info { get; set; }
        public List<IUIControl> Children { get; set; }

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

        public bool IsUIBindingType
        {
            get
            {
                return IsMetaTypeCheckBox || IsMetaTypeComboBox || IsMetaTypeDatePicker ||
                       IsMetaTypeEmailBox || IsMetaTypeImage || IsMetaTypeImageBox ||
                       IsMetaTypeLabel || IsMetaTypeNumBox || IsMetaTypePasswordBox ||
                       IsMetaTypeTextArea || IsMetaTypeTextBlock || IsMetaTypeTextBox;

            }

        }

        public bool IsUIComplexBindingType
        {
            get
            {
                return IsMetaTypeLookUp;

            }

        }

        public bool IsEditGridUIBindingType
        {
            get
            {
                return IsMetaTypeEditGridCheckBox || IsMetaTypeEditGridComboBox || IsMetaTypeEditGridDatePicker ||
                       IsMetaTypeEditGridEmailBox || IsMetaTypeEditGridNumBox || IsMetaTypeEditGridTextBox;

            }

        }

        public bool IsEditGridUIComplexBindingType
        {
            get
            {
                return IsMetaTypeEditGridLookUp;

            }

        }

        public bool IsUIViewType
        {
            get
            {
                return IsMetaTypeCreateView || IsMetaTypeEditView || IsMetaTypeListView ||
                       IsMetaTypeDetailView || IsMetaTypeEditListView;

            }

        }

      


        public bool IsDataTableConnected
        {
            get { return (DataTableInfo != null && !string.IsNullOrEmpty(DataTableMetaCode) && DataTableInfo.IsMetaTypeDataTable); }
        }

        public bool IsDataColumn1Connected
        {
            get { return (DataColumn1Info != null && !string.IsNullOrEmpty(DataColumn1MetaCode) && DataColumn1Info.IsMetaTypeDataColumn); }
        }

        public bool IsDataColumn2Connected
        {
            get { return (DataColumn2Info != null && !string.IsNullOrEmpty(DataColumn2MetaCode) && DataColumn2Info.IsMetaTypeDataColumn); }
        }

        public bool IsDataViewConnected
        {
            get { return (DataViewInfo != null && !string.IsNullOrEmpty(DataViewMetaCode) && DataViewInfo.IsMetaTypeDataView); }
        }

        public bool IsDataViewColumn1Connected
        {
            get { return (DataViewColumn1Info != null && !string.IsNullOrEmpty(DataViewColumn1MetaCode)); }
        }

        public bool IsDataViewColumn2Connected
        {
            get { return (DataViewColumn2Info != null && !string.IsNullOrEmpty(DataViewColumn2MetaCode)); }
        }

        public bool HasValueDomain
        {
            get { return Domain.Contains("VALUEDOMAIN."); }
        }


        public string DomainName
        {
            get
            {
                if (!HasValueDomain)
                    return string.Empty;

                var splits = Domain.Split(".".ToCharArray());
                if (splits.Length >= 2)
                    return splits[1];
                else
                    return string.Empty;

            }
        }

        public bool IsMetaTypeCreateView
        {
            get { return MetaType == MetaTypeCreateView; }
        }

        public bool IsMetaTypeEditView
        {
            get { return MetaType == MetaTypeEditView; }
        }

        public bool IsMetaTypeDetailView
        {
            get { return MetaType == MetaTypeDetailView; }
        }

        public bool IsMetaTypeListView
        {
            get { return MetaType == MetaTypeListView; }
        }

        public bool IsMetaTypeEditListView
        {
            get { return MetaType == MetaTypeEditListView; }
        }

        public bool IsMetaTypeStaticHTML
        {
            get { return MetaType == MetaTypeStaticHTML; }
        }

        public bool IsMetaTypeImage
        {
            get { return MetaType == MetaTypeImage; }
        }

        public bool IsMetaTypeTextBlock
        {
            get { return MetaType == MetaTypeTextBlock; }
        }

        public bool IsMetaTypeLabel
        {
            get { return MetaType == MetaTypeLabel; }
        }


        public bool IsMetaTypeEmailBox
        {
            get { return MetaType == MetaTypeEmailBox; }
        }


        public bool IsMetaTypePasswordBox
        {
            get { return MetaType == MetaTypePasswordBox; }
        }

        public bool IsMetaTypeTextBox
        {
            get { return MetaType == MetaTypeTextBox; }
        }


        public bool IsMetaTypeTextArea
        {
            get { return MetaType == MetaTypeTextArea; }
        }

 
        public bool IsMetaTypeLookUp
        {
            get { return MetaType == MetaTypeLookUp; }
        }

    
        public bool IsMetaTypeNumBox
        {
            get { return MetaType == MetaTypeNumBox; }
        }

       
        public bool IsMetaTypeCheckBox
        {
            get { return MetaType == MetaTypeCheckBox; }
        }


        public bool IsMetaTypeComboBox
        {
            get { return MetaType == MetaTypeComboBox; }
        }

        public bool IsMetaTypeImageBox
        {
            get { return MetaType == MetaTypeImageBox; }
        }

        public bool IsMetaTypeEditListViewColumn
        {
            get { return MetaType == MetaTypeEditListViewColumn; }
        }

       

        public bool IsMetaTypePanel
        {
            get { return MetaType == MetaTypePanel; }
        }


        public bool IsMetaTypeDatePicker
        {
            get { return MetaType == MetaTypeDatePicker; }
        }

        public bool IsMetaTypeEditGrid
        {
            get { return MetaType == MetaTypeEditGrid; }
        }

        public bool IsMetaTypeSection
        {
            get { return MetaType == MetaTypeSection; }
        }

        public bool IsMetaTypeEditGridCheckBox
        {
            get { return MetaType == MetaTypeEditGridCheckBox; }
        }


        public bool IsMetaTypeEditGridComboBox
        {
            get { return MetaType == MetaTypeEditGridComboBox; }
        }

        public bool IsMetaTypeEditGridDatePicker
        {
            get { return MetaType == MetaTypeEditGridDatePicker; }
        }

        public bool IsMetaTypeEditGridTextBox
        {
            get { return MetaType == MetaTypeEditGridTextBox; }
        }

        public bool IsMetaTypeEditGridNumBox
        {
            get { return MetaType == MetaTypeEditGridNumBox; }
        }

        public bool IsMetaTypeEditGridLookUp
        {
            get { return MetaType == MetaTypeEditGridLookUp; }
        }

        public bool IsMetaTypeEditGridStaticHTML
        {
            get { return MetaType == MetaTypeEditGridStaticHTML; }
        }

        public bool IsMetaTypeEditGridEmailBox
        {
            get { return MetaType == MetaTypeEditGridEmailBox; }
        }

     

        public string UIId
        {
            get { return MetaCode; }
        }

        public bool Mandatory
        {
            get 
            {
                if (IsDataColumn1Connected)
                    return DataColumn1Info.Mandatory;

                return false;
            }
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

        public string DataColumnDbName => DataColumn1DbName;
        public string DataColumn1DbName
        {
            get
            {
                if (IsDataColumn1Connected)
                    return DataColumn1Info.DbName;

                return string.Empty;
            }
        }

        public string DataColumn2DbName
        {
            get
            {
                if (IsDataColumn2Connected)
                    return DataColumn2Info.DbName;

                return string.Empty;
            }
        }

        public string DataViewTitle
        {
            get
            {
                if (IsDataViewConnected)
                    return DataViewInfo.Title;

                return string.Empty;
            }
        }

        public string DataViewColumnDbName => DataViewColumn1DbName;
        public string DataViewColumn1DbName
        {
            get
            {
                if (IsDataViewColumn1Connected)
                    return DataViewColumn1Info.SQLQueryFieldName;

                return string.Empty;
            }
        }

        public string DataViewColumn2DbName
        {
            get
            {
                if (IsDataViewColumn2Connected)
                    return DataViewColumn2Info.SQLQueryFieldName;

                return string.Empty;
            }
        }

        public string DataViewColumnTitle => DataViewColumn1Title;
        public string DataViewColumn1Title
        {
            get
            {
                if (IsDataViewColumn1Connected)
                    return DataViewColumn1Info.Title;

                return string.Empty;
            }
        }

       



        public string DataViewColumn2Title
        {
            get
            {
                if (IsDataViewColumn2Connected)
                    return DataViewColumn2Info.Title;

                return string.Empty;
            }
        }



    }

}
