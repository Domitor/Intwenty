using Intwenty.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Interface
{
    public interface IUIControl : IUIComplexBinding
    {
        int ColumnOrder { get; }
        int RowOrder { get;  }
        bool IsMetaTypeStaticHTML { get; }
        bool IsMetaTypeImage { get; }
        bool IsMetaTypeTextBlock { get; }
        bool IsMetaTypeLabel { get; }
        bool IsMetaTypeEmailBox { get; }
        bool IsMetaTypePasswordBox { get; }
        bool IsMetaTypeTextBox { get; }
        bool IsMetaTypeTextArea { get; }
        bool IsMetaTypeLookUp { get; }
        bool IsMetaTypeNumBox { get; }
        bool IsMetaTypeCheckBox { get; }
        bool IsMetaTypeComboBox { get; }
        bool IsMetaTypeImageBox { get; }
        bool IsMetaTypeDatePicker { get; }
        bool IsMetaTypeEditGrid { get; }
        bool IsMetaTypeEditGridCheckBox { get; }
        bool IsMetaTypeEditGridComboBox { get; }
        bool IsMetaTypeEditGridDatePicker { get; }
        bool IsMetaTypeEditGridTextBox { get; }
        bool IsMetaTypeEditGridNumBox { get; }
        bool IsMetaTypeEditGridLookUp { get; }
        bool IsMetaTypeEditGridStaticHTML { get; }
        bool IsMetaTypeEditGridEmailBox { get; }
        bool IsDataTableConnected { get; }
        bool IsDataColumnConnected { get; }
        bool IsDataColumn2Connected { get; }
        bool IsDataViewConnected { get; }
        bool IsDataViewColumnConnected { get; }
        bool IsDataViewColumn2Connected { get; }
        bool HasValueDomain { get; }
        bool HasDataViewDomain { get; }
        DataViewModelItem DataViewInfo { get;}
        DataViewModelItem DataViewColumnInfo { get; }
        DataViewModelItem DataViewColumnInfo2 { get; }
        List<IUIControl> Children { get; set; }
    }

    public interface IEditListViewColumn : IUIBinding
    {
        int ColumnOrder { get; }
       

    }
}
