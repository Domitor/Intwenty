﻿using Intwenty.Model;
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
        bool IsMetaTypeSearchBox { get; }
        bool IsMetaTypeNumBox { get; }
        bool IsMetaTypeCheckBox { get; }
        bool IsMetaTypeComboBox { get; }
        bool IsMetaTypeImageBox { get; }
        bool IsMetaTypeDatePicker { get; }
        bool HasValueDomain { get; }
        bool HasAppDomain { get; }
        bool IsDataColumn2Connected { get; }

    }

    public interface IEditListViewColumn : IUIBinding
    {
        int ColumnOrder { get; }

        bool IsMetaTypeImage { get; }
    }

}
