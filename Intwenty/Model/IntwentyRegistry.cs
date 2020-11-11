using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Model
{
    public static class IntwentyRegistry
    {

        private static List<IntwentyProperty> _properties;

        private static List<IntwentyMetaType> _metatypes;

        public static List<IntwentyProperty> IntwentyProperties
        {
            get { return GetProperties(); }
        }

        public static List<IntwentyMetaType> IntwentyMetaTypes
        {
            get { return GetMetaTypes(); }
        }

        private static List<IntwentyMetaType> GetMetaTypes()
        {
            if (_metatypes != null)
                return _metatypes;

            _metatypes = new List<IntwentyMetaType>();

            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeCheckBox, Title = "Checkbox" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeComboBox, Title = "Combobox" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeCreateView, Title = "Create View" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeDatePicker, Title = "Datepicker" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeEditGrid, Title = "Grid" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeEditGridCheckBox, Title = "Grid Checkbox" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeEditGridComboBox, Title = "Grid Combobox" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeEditGridDatePicker, Title = "Grid Datepicker" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeEditGridEmailBox, Title = "Grid Emailbox" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeEditGridLookUp, Title = "Grid Look Up" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeEditGridNumBox, Title = "Grid Numbox" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeEditGridStaticHTML, Title = "Grid HTML" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeEditGridTextBox, Title = "Grid Textbox" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeEmailBox, Title = "Emailbox" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeImage, Title = "Image" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeImageBox, Title = "Image Upload" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeLabel, Title = "Label" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeEditListView, Title = "Edit List View" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeEditListViewColumn, Title = "Edit List Column" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeLookUp, Title = "Look Up" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeNumBox, Title = "Numbox" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypePanel, Title = "Panel" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypePasswordBox, Title = "Password Box" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeDetailView, Title = "Detail View" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeListView, Title = " List View", ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeSection, Title = "Section" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeStaticHTML, Title = "HTML" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeTextArea, Title = "Text Area" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeTextBlock, Title = "Text Block" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeTextBox, Title = "Textbox" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeEditView, Title = "Edit View" , ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = ApplicationModelItem.MetaTypeApplication, Title = "Application", ModelCode = "APPMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = DatabaseModelItem.MetaTypeDataColumn, Title = "Column", ModelCode = "DATAMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = DatabaseModelItem.MetaTypeDataTable, Title = "Table", ModelCode = "DATAMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = DataViewModelItem.MetaTypeDataView, Title = "Data View", ModelCode = "DATAVIEWMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = DataViewModelItem.MetaTypeDataViewColumn, Title = "Data View Column", ModelCode = "DATAVIEWMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = DataViewModelItem.MetaTypeDataViewKeyColumn, Title = "Data View Key Column", ModelCode = "DATAVIEWMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = EndpointModelItem.MetaTypeTableGet, Title = "Get", ModelCode = "ENDPOINTMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = EndpointModelItem.MetaTypeTableList, Title = "List", ModelCode = "ENDPOINTMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = EndpointModelItem.MetaTypeTableSave, Title = "Save", ModelCode = "ENDPOINTMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = EndpointModelItem.MetaTypeDataViewList, Title = "View", ModelCode = "ENDPOINTMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = MenuModelItem.MetaTypeMainMenu, Title = "Menu", ModelCode = "MENUMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = MenuModelItem.MetaTypeMenuItem, Title = "Menu Item", ModelCode = "MENUMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = MenuModelItem.MetaTypeSubMenuItem, Title = "Sub Menu Item", ModelCode = "MENUMODEL" });


            return _metatypes;
        }

        private static List<IntwentyProperty> GetProperties()
        {
            if (_properties != null)
                return _properties;

            _properties = new List<IntwentyProperty>();

            var prop = new IntwentyProperty("DEFVALUE", "Default Value", "LIST");
            prop.ValidFor.Add(DatabaseModelItem.MetaTypeDataColumn);
            prop.ValidValues.Add(new SelectablePropertyValue() { CodeValue = "NONE", DisplayValue = "None" });
            prop.ValidValues.Add(new SelectablePropertyValue() { CodeValue = "AUTO", DisplayValue = "Automatic" });
            _properties.Add(prop);

            prop = new IntwentyProperty("DEFVALUE_START", "Default Value Start", "NUMERIC");
            prop.ValidFor.Add(DatabaseModelItem.MetaTypeDataColumn);
            _properties.Add(prop);

            prop = new IntwentyProperty("DEFVALUE_PREFIX", "Default Value Prefix", "STRING");
            prop.ValidFor.Add(DatabaseModelItem.MetaTypeDataColumn);
            _properties.Add(prop);

            prop = new IntwentyProperty("DEFVALUE_SEED", "Default Value Seed", "NUMERIC");
            prop.ValidFor.Add(DatabaseModelItem.MetaTypeDataColumn);
            _properties.Add(prop);

            prop = new IntwentyProperty("MANDATORY", "Required", "BOOLEAN");
            prop.ValidFor.Add(DatabaseModelItem.MetaTypeDataColumn);
            _properties.Add(prop);

            prop = new IntwentyProperty("UNIQUE", "Unique", "BOOLEAN");
            prop.ValidFor.Add(DatabaseModelItem.MetaTypeDataColumn);
            _properties.Add(prop);

            prop = new IntwentyProperty("DOMAIN", "Domain", "STRING");
            prop.ValidFor.Add(DatabaseModelItem.MetaTypeDataColumn);
            _properties.Add(prop);

            prop = new IntwentyProperty("HIDEFILTER", "Hide filter", "BOOLEAN");
            prop.ValidFor.Add(DataViewModelItem.MetaTypeDataView);
            prop.ValidFor.Add(UserInterfaceModelItem.MetaTypeListView);
            _properties.Add(prop);


            prop = new IntwentyProperty("COLLAPSIBLE", "Collapsible", "BOOLEAN");
            prop.ValidFor.Add(UserInterfaceModelItem.MetaTypeSection);
            _properties.Add(prop);

            prop = new IntwentyProperty("STARTEXPANDED", "Start expanded", "BOOLEAN");
            prop.ValidFor.Add(UserInterfaceModelItem.MetaTypeSection);
            _properties.Add(prop);

            prop = new IntwentyProperty("READONLY", "Read Only", "BOOLEAN");
            prop.ValidFor.Add(UserInterfaceModelItem.MetaTypeCheckBox);
            prop.ValidFor.Add(UserInterfaceModelItem.MetaTypeComboBox);
            prop.ValidFor.Add(UserInterfaceModelItem.MetaTypeDatePicker);
            prop.ValidFor.Add(UserInterfaceModelItem.MetaTypeEditGridCheckBox);
            prop.ValidFor.Add(UserInterfaceModelItem.MetaTypeEditGridComboBox);
            prop.ValidFor.Add(UserInterfaceModelItem.MetaTypeEditGridDatePicker);
            prop.ValidFor.Add(UserInterfaceModelItem.MetaTypeEditGridEmailBox);
            prop.ValidFor.Add(UserInterfaceModelItem.MetaTypeEditGridLookUp);
            prop.ValidFor.Add(UserInterfaceModelItem.MetaTypeEditGridTextBox);
            prop.ValidFor.Add(UserInterfaceModelItem.MetaTypeEmailBox);
            prop.ValidFor.Add(UserInterfaceModelItem.MetaTypeLookUp);
            prop.ValidFor.Add(UserInterfaceModelItem.MetaTypeNumBox);
            prop.ValidFor.Add(UserInterfaceModelItem.MetaTypePasswordBox);
            prop.ValidFor.Add(UserInterfaceModelItem.MetaTypeTextArea);
            prop.ValidFor.Add(UserInterfaceModelItem.MetaTypeTextBox);
            _properties.Add(prop);


            prop = new IntwentyProperty("EDITMODE", "Edit Mode", "LIST");
            prop.ValidFor.Add(UserInterfaceModelItem.MetaTypeEditGrid);
            prop.ValidValues.Add(new SelectablePropertyValue() { CodeValue = "CELL", DisplayValue = "Line" });
            prop.ValidValues.Add(new SelectablePropertyValue() { CodeValue = "MODAL", DisplayValue = "Modal" });
            prop.ValidValues.Add(new SelectablePropertyValue() { CodeValue = "EXPANDER", DisplayValue = "Expander" });
            _properties.Add(prop);

            prop = new IntwentyProperty("GRIDLAYOUT", "Layout", "LIST");
            prop.ValidFor.Add(UserInterfaceModelItem.MetaTypeEditGrid);
            prop.ValidValues.Add(new SelectablePropertyValue() { CodeValue = "GRID", DisplayValue = "Grid" });
            prop.ValidValues.Add(new SelectablePropertyValue() { CodeValue = "CARD", DisplayValue = "Card" });
            _properties.Add(prop);

            prop = new IntwentyProperty("IMGWIDTH", "Width", "NUMERIC");
            prop.ValidFor.Add(UserInterfaceModelItem.MetaTypeImage);
            _properties.Add(prop);

            prop = new IntwentyProperty("IMGLAYOUT", "Layout", "LIST");
            prop.ValidFor.Add(UserInterfaceModelItem.MetaTypeImage);
            prop.ValidValues.Add(new SelectablePropertyValue() { CodeValue = "DEFAULT", DisplayValue = "Default" });
            _properties.Add(prop);

            prop = new IntwentyProperty("LABELSIZE", "Size", "LIST");
            prop.ValidFor.Add(UserInterfaceModelItem.MetaTypeLabel);
            prop.ValidValues.Add(new SelectablePropertyValue() { CodeValue = "H1", DisplayValue = "h1" });
            prop.ValidValues.Add(new SelectablePropertyValue() { CodeValue = "H2", DisplayValue = "h2" });
            prop.ValidValues.Add(new SelectablePropertyValue() { CodeValue = "H3", DisplayValue = "h3" });
            prop.ValidValues.Add(new SelectablePropertyValue() { CodeValue = "H4", DisplayValue = "h4" });
            prop.ValidValues.Add(new SelectablePropertyValue() { CodeValue = "H5", DisplayValue = "h5" });
            prop.ValidValues.Add(new SelectablePropertyValue() { CodeValue = "H6", DisplayValue = "h6" });
            _properties.Add(prop);

            prop = new IntwentyProperty("PAGESIZE", "Page Size", "NUMERIC");
            prop.ValidFor.Add(UserInterfaceModelItem.MetaTypeListView);
            prop.ValidFor.Add(UserInterfaceModelItem.MetaTypeEditListView);
            _properties.Add(prop);

            return _properties;

        }
    }
}
