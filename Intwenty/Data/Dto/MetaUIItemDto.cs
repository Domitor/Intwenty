using Moley.Data.Entity;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Moley.Data.Dto
{

    public class MetaUIItemDto : MetaModelDto
    {

        public MetaUIItemDto()
        {
        }

        public MetaUIItemDto(MetaUIItem entity)
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
            Domain = string.IsNullOrEmpty(entity.Domain) ? "" : entity.Domain;
            Properties = entity.Properties;
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string AppMetaCode { get; set; }

        public string DataMetaCode { get; set; }

        public string CssClass { get; set; }

        public int ColumnOrder { get; set; }

        public int RowOrder { get; set; }

        public string Domain { get; set; }

        public MetaDataItemDto DataInfo { get; set; }

        public MetaDataViewDto ViewInfo { get; set; }

 
        public bool IsDataConnected
        {
            get { return DataInfo != null && !string.IsNullOrEmpty(DataMetaCode); }
        }

        public bool IsDataViewConnected
        {
            get { return Domain.Contains("DATAVIEW") && ViewInfo != null; }
        }

        public bool HasValueDomain
        {
            get { return Domain.Contains("VALUEDOMAIN"); }
        }

        public bool HasDataViewDomain
        {
            get { return Domain.Contains("DATAVIEW"); }
        }

        public string ViewName
        {
            get
            {
                if (!Domain.Contains("DATAVIEW"))
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
                if (!Domain.Contains("VALUEDOMAIN"))
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
                if (!Domain.Contains("DATAVIEW"))
                    return string.Empty;

                var splits = Domain.Split(".".ToCharArray());
                if (splits.Length >= 3)
                    return splits[2];
                else
                    return string.Empty;

            }
        }

        public string UITypeTextBox
        {
            get { return "TEXTBOX"; }
        }

        public bool IsUITypeTextBox
        {
            get { return MetaType == UITypeTextBox; }
        }

        public string UITypeTextArea
        {
            get { return "TEXTAREA"; }
        }

        public bool IsUITypeTextArea
        {
            get { return MetaType == UITypeTextArea; }
        }

        public string UITypeLookUp
        {
            get { return "LOOKUP"; }
        }

        public bool IsUITypeLookUp
        {
            get { return MetaType == UITypeLookUp; }
        }

        public string UITypeLookUpKeyField
        {
            get { return "LOOKUPKEYFIELD"; }
        }

        public bool IsUITypeLookUpKeyField
        {
            get { return MetaType == UITypeLookUpKeyField; }
        }

        public string UITypeLookUpField
        {
            get { return "LOOKUPFIELD"; }
        }

        public bool IsUITypeLookUpField
        {
            get { return MetaType == UITypeLookUpField; }
        }

        public string UITypeNumBox
        {
            get { return "NUMBOX"; }
        }

        public bool IsUITypeNumBox
        {
            get { return MetaType == UITypeNumBox; }
        }

        public string UITypeCheckBox
        {
            get { return "CHECKBOX"; }
        }

        public bool IsUITypeCheckBox
        {
            get { return MetaType == UITypeCheckBox; }
        }

        public string UITypeComboBox
        {
            get { return "COMBOBOX"; }
        }

        public bool IsUITypeComboBox
        {
            get { return MetaType == UITypeComboBox; }
        }

        public string UITypeListView
        {
            get { return "LISTVIEW"; }
        }

        public bool IsUITypeListView
        {
            get { return MetaType == UITypeListView; }
        }

        public string UITypeListViewField
        {
            get { return "LISTVIEWFIELD"; }
        }

        public bool IsUITypeListViewField
        {
            get { return MetaType == UITypeListViewField; }
        }

        public string UITypePanel
        {
            get { return "PANEL"; }
        }

        public bool IsUITypePanel
        {
            get { return MetaType == UITypePanel; }
        }

        public string UITypeDatePicker
        {
            get { return "DATEPICKER"; }
        }

        public bool IsUITypeDatePicker
        {
            get { return MetaType == UITypeDatePicker; }
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

        protected override List<string> ValidProperties
        {
            get
            {
                var t = new List<string>();

                if (this.IsUITypeListView)
                    t.Add("HIDEFILTER");

                return t;
            }
        }

        public override List<string> ValidMetaTypes
        {
            get
            {
                var t = new List<string>();
                t.Add("TEXTBOX");
                t.Add("TEXTAREA");
                t.Add("LOOKUP");
                t.Add("LOOKUPKEYFIELD");
                t.Add("LOOKUPFIELD");
                t.Add("NUMBOX");
                t.Add("CHECKBOX"); 
                t.Add("COMBOBOX"); 
                t.Add("LISTVIEW");
                t.Add("LISTVIEWFIELD");
                t.Add("PANEL");
                t.Add("DATEPICKER");
                return t;
            }
        }


    }

}
