using Intwenty.Entity;
using Intwenty.Interface;
using System.Collections.Generic;
using System.Linq;

namespace Intwenty.Model
{


    public class UserInterfaceModelItem : BaseModelItem, IUIBinding, IUIComplexBinding, ILocalizableTitle
    {
        //META TYPES
        public static readonly string MetaTypeTextBlock = "TEXTBLOCK";
        public static readonly string MetaTypeLabel = "LABEL";
        public static readonly string MetaTypeImage = "IMAGE";
        public static readonly string MetaTypeStaticHTML = "STATICHTML";
        public static readonly string MetaTypeCreateView = "CREATEVIEW";
        public static readonly string MetaTypeUpdateView = "UPDATEVIEW";
        public static readonly string MetaTypePresentationView = "PRESENTATIONVIEW";
        public static readonly string MetaTypePresentationListView = "PRESENTATIONLISTVIEW";
        public static readonly string MetaTypeEmailBox = "EMAILBOX";
        public static readonly string MetaTypePasswordBox = "PASSWORDBOX";
        public static readonly string MetaTypeTextBox = "TEXTBOX";
        public static readonly string MetaTypeTextArea = "TEXTAREA";
        public static readonly string MetaTypeLookUp = "LOOKUP";
        public static readonly string MetaTypeNumBox = "NUMBOX";
        public static readonly string MetaTypeCheckBox = "CHECKBOX";
        public static readonly string MetaTypeComboBox = "COMBOBOX";
        public static readonly string MetaTypeListView = "LISTVIEW";
        public static readonly string MetaTypeListViewColumn = "LISTVIEWCOLUMN";
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
        }

        public UserInterfaceModelItem(string metatype)
        {
            MetaType = metatype;
            SetEmptyStrings();
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
            DataMetaCode = entity.DataMetaCode;
            ViewMetaCode = entity.ViewMetaCode;
            DataMetaCode2 = entity.DataMetaCode2;
            ViewMetaCode2 = entity.ViewMetaCode2;
            Domain = entity.Domain;
            Properties = entity.Properties;
            RawHTML = entity.RawHTML;
            SetEmptyStrings();
        }

        private void SetEmptyStrings()
        {
            if (string.IsNullOrEmpty(Description)) Description = string.Empty;
            if (string.IsNullOrEmpty(AppMetaCode)) AppMetaCode = string.Empty;
            if (string.IsNullOrEmpty(MetaCode)) MetaCode = string.Empty;
            if (string.IsNullOrEmpty(ParentMetaCode)) ParentMetaCode = string.Empty;
            if (string.IsNullOrEmpty(ViewMetaCode)) ViewMetaCode = string.Empty;
            if (string.IsNullOrEmpty(DataMetaCode)) DataMetaCode = string.Empty;
            if (string.IsNullOrEmpty(ViewMetaCode2)) ViewMetaCode2 = string.Empty;
            if (string.IsNullOrEmpty(DataMetaCode2)) DataMetaCode2 = string.Empty;
            if (string.IsNullOrEmpty(Domain)) Domain = string.Empty;
            if (string.IsNullOrEmpty(Properties)) Properties = string.Empty;
            if (string.IsNullOrEmpty(Title)) Title = string.Empty;
            if (string.IsNullOrEmpty(RawHTML)) RawHTML = string.Empty;
            if (string.IsNullOrEmpty(TitleLocalizationKey)) TitleLocalizationKey = string.Empty;
        }

        public string TitleLocalizationKey { get; set; }

        public string Description { get; set; }

        public string AppMetaCode { get; set; }

        public string DataMetaCode { get; set; }

        public string ViewMetaCode { get; set; }

        public string DataMetaCode2 { get; set; }

        public string ViewMetaCode2 { get; set; }

        public int ColumnOrder { get; set; }

        public int RowOrder { get; set; }

        public string Domain { get; set; }

        public string RawHTML { get; set; }

        public DatabaseModelItem DataTableInfo { get; set; }

        public DatabaseModelItem DataColumnInfo { get; set; }

        public DatabaseModelItem DataColumnInfo2 { get; set; }

        public DataViewModelItem DataViewInfo { get; set; }

        public DataViewModelItem DataViewColumnInfo { get; set; }

        public DataViewModelItem DataViewColumnInfo2 { get; set; }

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
                return IsMetaTypeCreateView || IsMetaTypeUpdateView || IsMetaTypeListView ||
                       IsMetaTypePresentationView || IsMetaTypePresentationListView;

            }

        }

      


        public bool IsDataTableConnected
        {
            get { return (DataTableInfo != null && !string.IsNullOrEmpty(DataMetaCode) && DataTableInfo.IsMetaTypeDataTable); }
        }

        public bool IsDataColumnConnected
        {
            get { return (DataColumnInfo != null && !string.IsNullOrEmpty(DataMetaCode) && DataColumnInfo.IsMetaTypeDataColumn); }
        }

        public bool IsDataColumn2Connected
        {
            get { return (DataColumnInfo2 != null && !string.IsNullOrEmpty(DataMetaCode2) && DataColumnInfo2.IsMetaTypeDataColumn); }
        }

        public bool IsDataViewConnected
        {
            get { return (Domain.Contains(DataViewModelItem.MetaTypeDataView) && DataViewInfo != null && DataViewInfo.IsMetaTypeDataView); }
        }

        public bool IsDataViewColumnConnected
        {
            get { return (DataViewColumnInfo != null && !string.IsNullOrEmpty(ViewMetaCode)); }
        }

        public bool IsDataViewColumn2Connected
        {
            get { return (DataViewColumnInfo2 != null && !string.IsNullOrEmpty(ViewMetaCode2)); }
        }

        public bool HasValueDomain
        {
            get { return Domain.Contains("VALUEDOMAIN."); }
        }

        public bool HasDataViewDomain
        {
            get { return Domain.Contains(DataViewModelItem.MetaTypeDataView + "."); }
        }

        public string ViewName
        {
            get
            {
                if (!HasDataViewDomain)
                    return string.Empty;

                var splits = Domain.Split(".".ToCharArray());
                if (splits.Length >= 2)
                    return splits[1];
                else
                    return string.Empty;
                
            }
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

        public bool IsMetaTypeUpdateView
        {
            get { return MetaType == MetaTypeUpdateView; }
        }

        public bool IsMetaTypePresentationView
        {
            get { return MetaType == MetaTypePresentationView; }
        }

        public bool IsMetaTypePresentationListView
        {
            get { return MetaType == MetaTypePresentationListView; }
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

        public bool IsMetaTypeListView
        {
            get { return MetaType == MetaTypeListView; }
        }


        public bool IsMetaTypeListViewColumn
        {
            get { return MetaType == MetaTypeListViewColumn; }
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
                if (IsDataColumnConnected)
                    return DataColumnInfo.Mandatory;

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

        public string DataColumnDbName
        {
            get
            {
                if (IsDataColumnConnected)
                    return DataColumnInfo.DbName;

                return string.Empty;
            }
        }

        public string DataColumn2DbName
        {
            get
            {
                if (IsDataColumn2Connected)
                    return DataColumnInfo2.DbName;

                return string.Empty;
            }
        }

      
        public string DataViewColumnDbName
        {
            get
            {
                if (IsDataViewColumnConnected)
                    return DataViewColumnInfo.SQLQueryFieldName;

                return string.Empty;
            }
        }

        public string DataViewColumn2DbName
        {
            get
            {
                if (IsDataViewColumn2Connected)
                    return DataViewColumnInfo2.SQLQueryFieldName;

                return string.Empty;
            }
        }





    }

}
