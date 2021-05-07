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

            _metatypes.Add(new IntwentyMetaType() { Code = ApplicationModelItem.MetaTypeApplication, Title = "Application", ModelCode = "APPMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeInputInterface, Title = "Input interface", ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceModelItem.MetaTypeListInterface, Title = "List interface", ModelCode = "UIMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceStructureModelItem.MetaTypeTable, Title = "Table", ModelCode = "UISTRUCTUREMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceStructureModelItem.MetaTypeTableTextColumn, Title = "Text Table Column", ModelCode = "UISTRUCTUREMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceStructureModelItem.MetaTypeCheckBox, Title = "Checkbox" , ModelCode = "UISTRUCTUREMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceStructureModelItem.MetaTypeComboBox, Title = "Combobox" , ModelCode = "UISTRUCTUREMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceStructureModelItem.MetaTypeDatePicker, Title = "Datepicker" , ModelCode = "UISTRUCTUREMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceStructureModelItem.MetaTypeEmailBox, Title = "Emailbox" , ModelCode = "UISTRUCTUREMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceStructureModelItem.MetaTypeImage, Title = "Image" , ModelCode = "UISTRUCTUREMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceStructureModelItem.MetaTypeImageBox, Title = "Image Upload" , ModelCode = "UISTRUCTUREMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceStructureModelItem.MetaTypeLabel, Title = "Label" , ModelCode = "UISTRUCTUREMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceStructureModelItem.MetaTypeSearchBox, Title = "Search Box" , ModelCode = "UISTRUCTUREMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceStructureModelItem.MetaTypeNumBox, Title = "Numbox" , ModelCode = "UISTRUCTUREMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceStructureModelItem.MetaTypePanel, Title = "Panel" , ModelCode = "UISTRUCTUREMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceStructureModelItem.MetaTypePasswordBox, Title = "Password Box" , ModelCode = "UISTRUCTUREMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceStructureModelItem.MetaTypeSection, Title = "Section" , ModelCode = "UISTRUCTUREMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceStructureModelItem.MetaTypeStaticHTML, Title = "HTML" , ModelCode = "UISTRUCTUREMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceStructureModelItem.MetaTypeTextArea, Title = "Text Area" , ModelCode = "UISTRUCTUREMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceStructureModelItem.MetaTypeTextBlock, Title = "Text Block" , ModelCode = "UISTRUCTUREMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = UserInterfaceStructureModelItem.MetaTypeTextBox, Title = "Textbox" , ModelCode = "UISTRUCTUREMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = DatabaseModelItem.MetaTypeDataColumn, Title = "Column", ModelCode = "DATAMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = DatabaseModelItem.MetaTypeDataTable, Title = "Table", ModelCode = "DATAMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = EndpointModelItem.MetaTypeTableGet, Title = "Get (GET)", ModelCode = "ENDPOINTMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = EndpointModelItem.MetaTypeTableList, Title = "List (POST)", ModelCode = "ENDPOINTMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = EndpointModelItem.MetaTypeTableSave, Title = "Save (POST)", ModelCode = "ENDPOINTMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = EndpointModelItem.MetaTypeCustomPost, Title = "Custom (POST)", ModelCode = "ENDPOINTMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = EndpointModelItem.MetaTypeCustomGet, Title = "Custom (GET)", ModelCode = "ENDPOINTMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = ViewModel.MetaTypeUIView, Title = "Input View", ModelCode = "UIVIEWMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = FunctionModelItem.MetaTypeNavigate, Title = "Navigate", ModelCode = "FUNCTIONMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = FunctionModelItem.MetaTypeCreate, Title = "Create", ModelCode = "FUNCTIONMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = FunctionModelItem.MetaTypeEdit, Title = "Edit", ModelCode = "FUNCTIONMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = FunctionModelItem.MetaTypeSave, Title = "Save", ModelCode = "FUNCTIONMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = FunctionModelItem.MetaTypeDelete, Title = "Delete", ModelCode = "FUNCTIONMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = FunctionModelItem.MetaTypeExport, Title = "Export", ModelCode = "FUNCTIONMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = FunctionModelItem.MetaTypeFilter, Title = "Filtering", ModelCode = "FUNCTIONMODEL" });
            _metatypes.Add(new IntwentyMetaType() { Code = FunctionModelItem.MetaTypePaging, Title = "Paging", ModelCode = "FUNCTIONMODEL" });



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

           

            prop = new IntwentyProperty("COLLAPSIBLE", "Collapsible", "BOOLEAN");
            prop.ValidFor.Add(UserInterfaceStructureModelItem.MetaTypeSection);
            _properties.Add(prop);

            prop = new IntwentyProperty("STARTEXPANDED", "Start expanded", "BOOLEAN");
            prop.ValidFor.Add(UserInterfaceStructureModelItem.MetaTypeSection);
            _properties.Add(prop);

            prop = new IntwentyProperty("READONLY", "Read Only", "BOOLEAN");
            prop.ValidFor.Add(UserInterfaceStructureModelItem.MetaTypeSearchBox);
            prop.ValidFor.Add(UserInterfaceStructureModelItem.MetaTypeCheckBox);
            prop.ValidFor.Add(UserInterfaceStructureModelItem.MetaTypeComboBox);
            prop.ValidFor.Add(UserInterfaceStructureModelItem.MetaTypeDatePicker);
            prop.ValidFor.Add(UserInterfaceStructureModelItem.MetaTypeEmailBox);
            prop.ValidFor.Add(UserInterfaceStructureModelItem.MetaTypeNumBox);
            prop.ValidFor.Add(UserInterfaceStructureModelItem.MetaTypePasswordBox);
            prop.ValidFor.Add(UserInterfaceStructureModelItem.MetaTypeTextArea);
            prop.ValidFor.Add(UserInterfaceStructureModelItem.MetaTypeTextBox);
            _properties.Add(prop);

            prop = new IntwentyProperty("ROWS", "Rows", "NUMERIC");
            prop.ValidFor.Add(UserInterfaceStructureModelItem.MetaTypeTextArea);
            _properties.Add(prop);


            prop = new IntwentyProperty("TABLEEDITMODE", "Edit Mode", "LIST");
            prop.ValidFor.Add(UserInterfaceStructureModelItem.MetaTypeTable);
            prop.ValidValues.Add(new SelectablePropertyValue() { CodeValue = "CELL", DisplayValue = "Line" });
            prop.ValidValues.Add(new SelectablePropertyValue() { CodeValue = "MODAL", DisplayValue = "Modal" });
            prop.ValidValues.Add(new SelectablePropertyValue() { CodeValue = "EXPANDER", DisplayValue = "Expander" });
            _properties.Add(prop);



            prop = new IntwentyProperty("TABLELAYOUT", "Layout", "LIST");
            prop.ValidFor.Add(UserInterfaceStructureModelItem.MetaTypeTable);
            prop.ValidValues.Add(new SelectablePropertyValue() { CodeValue = "GRID", DisplayValue = "Grid" });
            prop.ValidValues.Add(new SelectablePropertyValue() { CodeValue = "CARD", DisplayValue = "Card" });
            _properties.Add(prop);

            prop = new IntwentyProperty("AFTERSAVEACTION", "After Save Action", "LIST");
            prop.ValidFor.Add(FunctionModelItem.MetaTypeSave);
            prop.ValidFor.Add(ViewModel.MetaTypeUIView);
            prop.ValidValues.Add(new SelectablePropertyValue() { CodeValue = "GOTOVIEW", DisplayValue = "Go to view" });
            prop.ValidValues.Add(new SelectablePropertyValue() { CodeValue = "REFRESH", DisplayValue = "Refresh saved data" });
            _properties.Add(prop);

            prop = new IntwentyProperty("GOTOVIEWPATH", "View path", "STRING");
            prop.ValidFor.Add(FunctionModelItem.MetaTypeSave);
            prop.ValidFor.Add(ViewModel.MetaTypeUIView);
            _properties.Add(prop);

            prop = new IntwentyProperty("IMGWIDTH", "Width", "NUMERIC");
            prop.ValidFor.Add(UserInterfaceStructureModelItem.MetaTypeImage);
            prop.ValidFor.Add(UserInterfaceStructureModelItem.MetaTypeImageBox);
            _properties.Add(prop);

            prop = new IntwentyProperty("IMGLAYOUT", "Layout", "LIST");
            prop.ValidFor.Add(UserInterfaceStructureModelItem.MetaTypeImage);
            prop.ValidValues.Add(new SelectablePropertyValue() { CodeValue = "DEFAULT", DisplayValue = "Default" });
            _properties.Add(prop);

            prop = new IntwentyProperty("LABELSIZE", "Size", "LIST");
            prop.ValidFor.Add(UserInterfaceStructureModelItem.MetaTypeLabel);
            prop.ValidValues.Add(new SelectablePropertyValue() { CodeValue = "H1", DisplayValue = "h1" });
            prop.ValidValues.Add(new SelectablePropertyValue() { CodeValue = "H2", DisplayValue = "h2" });
            prop.ValidValues.Add(new SelectablePropertyValue() { CodeValue = "H3", DisplayValue = "h3" });
            prop.ValidValues.Add(new SelectablePropertyValue() { CodeValue = "H4", DisplayValue = "h4" });
            prop.ValidValues.Add(new SelectablePropertyValue() { CodeValue = "H5", DisplayValue = "h5" });
            prop.ValidValues.Add(new SelectablePropertyValue() { CodeValue = "H6", DisplayValue = "h6" });
            _properties.Add(prop);

            prop = new IntwentyProperty("PAGESIZE", "Page Size", "NUMERIC");
            _properties.Add(prop);

            prop = new IntwentyProperty("MULTISELECT", "Multi Select", "BOOLEAN");
            prop.ValidFor.Add(UserInterfaceStructureModelItem.MetaTypeSearchBox);
            _properties.Add(prop);

            prop = new IntwentyProperty("USESEARCH", "Use Search", "BOOLEAN");
            prop.ValidFor.Add(UserInterfaceStructureModelItem.MetaTypeSearchBox);
            _properties.Add(prop);

            prop = new IntwentyProperty("ALLOWCREATE", "Allow Create", "BOOLEAN");
            prop.ValidFor.Add(UserInterfaceStructureModelItem.MetaTypeSearchBox);
            _properties.Add(prop);

            return _properties;

        }
    }
}
