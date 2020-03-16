using Intwenty.Data.Entity;
using System.Collections.Generic;


namespace Intwenty.MetaDataService.Model
{

    public class UserInterfaceModelItem : BaseModelItem
    {
        //META TYPES
        public static readonly string MetaTypeTextBox = "TEXTBOX";
        public static readonly string MetaTypeTextArea = "TEXTAREA";
        public static readonly string MetaTypeLookUp = "LOOKUP";
        public static readonly string MetaTypeLookUpKeyField = "LOOKUPKEYFIELD";
        public static readonly string MetaTypeLookUpField = "LOOKUPFIELD";
        public static readonly string MetaTypeNumBox = "NUMBOX";
        public static readonly string MetaTypeCheckBox = "CHECKBOX";
        public static readonly string MetaTypeComboBox = "COMBOBOX";
        public static readonly string MetaTypeListView = "LISTVIEW";
        public static readonly string MetaTypeListViewField = "LISTVIEWFIELD";
        public static readonly string MetaTypePanel = "PANEL";
        public static readonly string MetaTypeDatePicker = "DATEPICKER";


        public UserInterfaceModelItem()
        {
            SetEmptyStrings();
        }

        public UserInterfaceModelItem(string metatype)
        {
            MetaType = metatype;
            SetEmptyStrings();
        }

        public UserInterfaceModelItem(Data.Entity.UserInterfaceItem entity)
        {
            Id = entity.Id;
            MetaType = entity.MetaType;
            Title = entity.Title;
            Description = entity.Description;
            AppMetaCode = entity.AppMetaCode;
            MetaCode = entity.MetaCode;
            ParentMetaCode = entity.ParentMetaCode;
            CssClass = entity.CssClass;
            ColumnOrder = entity.ColumnOrder;
            RowOrder = entity.RowOrder;
            DataMetaCode = entity.DataMetaCode;
            Domain = entity.Domain;
            Properties = entity.Properties;
            SetEmptyStrings();
        }

        private void SetEmptyStrings()
        {
            if (string.IsNullOrEmpty(Description)) Description = string.Empty;
            if (string.IsNullOrEmpty(AppMetaCode)) AppMetaCode = string.Empty;
            if (string.IsNullOrEmpty(MetaCode)) MetaCode = string.Empty;
            if (string.IsNullOrEmpty(ParentMetaCode)) ParentMetaCode = string.Empty;
            if (string.IsNullOrEmpty(CssClass)) CssClass = string.Empty;
            if (string.IsNullOrEmpty(DataMetaCode)) DataMetaCode = string.Empty;
            if (string.IsNullOrEmpty(Domain)) Domain = string.Empty;
            if (string.IsNullOrEmpty(Properties)) Properties = string.Empty;
            if (string.IsNullOrEmpty(Title)) Title = string.Empty;
        }

        public string Description { get; set; }

        public string AppMetaCode { get; set; }

        public string DataMetaCode { get; set; }

        public string CssClass { get; set; }

        public int ColumnOrder { get; set; }

        public int RowOrder { get; set; }

        public string Domain { get; set; }

        public DatabaseModelItem DataInfo { get; set; }

        public DataViewModelItem ViewInfo { get; set; }

        public List<string> ValidProperties
        {
            get
            {
                var t = new List<string>();

                if (this.IsMetaTypeListView)
                    t.Add("HIDEFILTER");

                return t;
            }
        }

        public static List<string> ValidMetaTypes
        {
            get
            {
                var t = new List<string>();
                t.Add(MetaTypeTextBox);
                t.Add(MetaTypeTextArea);
                t.Add(MetaTypeLookUp);
                t.Add(MetaTypeLookUpKeyField);
                t.Add(MetaTypeLookUpField);
                t.Add(MetaTypeNumBox);
                t.Add(MetaTypeCheckBox);
                t.Add(MetaTypeComboBox);
                t.Add(MetaTypeListView);
                t.Add(MetaTypeListViewField);
                t.Add(MetaTypePanel);
                t.Add(MetaTypeDatePicker);
                return t;
            }
        }

        public bool IsDataConnected
        {
            get { return DataInfo != null && !string.IsNullOrEmpty(DataMetaCode); }
        }

        public bool IsDataViewConnected
        {
            get { return Domain.Contains(DataViewModelItem.MetaTypeDataView) && ViewInfo != null; }
        }

        public bool HasValueDomain
        {
            get { return Domain.Contains(ValueDomainModelItem.MetaTypeValueDomain + "."); }
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

        public string ViewField
        {
            get
            {
                if (!HasDataViewDomain)
                    return string.Empty;

                var splits = Domain.Split(".".ToCharArray());
                if (splits.Length >= 3)
                    return splits[2];
                else
                    return string.Empty;

            }
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

    

        public bool IsMetaTypeLookUpKeyField
        {
            get { return MetaType == MetaTypeLookUpKeyField; }
        }

 
        public bool IsMetaTypeLookUpField
        {
            get { return MetaType == MetaTypeLookUpField; }
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

       

        public bool IsMetaTypeListView
        {
            get { return MetaType == MetaTypeListView; }
        }


        public bool IsMetaTypeListViewField
        {
            get { return MetaType == MetaTypeListViewField; }
        }

       

        public bool IsMetaTypePanel
        {
            get { return MetaType == MetaTypePanel; }
        }


        public bool IsMetaTypeDatePicker
        {
            get { return MetaType == MetaTypeDatePicker; }
        }

        public override bool HasValidMetaType
        {
            get
            {
               if (string.IsNullOrEmpty(MetaType))
                   return false;

               if (ValidMetaTypes.Contains(MetaType))
                   return true;

                return false;

            }
        }

        public override bool HasValidProperties
        {
            get
            {
                foreach (var s in GetProperties())
                {
                    if (!ValidProperties.Exists(p => p == s))
                        return false;
                }

                return true;
            }
        }


    }

}
