using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Model.UIRendering
{
    /// <summary>
    /// Note: properties are reference from javascript in View.cshtml
    /// </summary>
    public class UITableConfiguration
    {
        
        public string SkipPaging { get;  set; }

        public int PageSize { get; set; }

        public string TableName { get; set; }

        public static UITableConfiguration GetDefaultUITableConfiguration()
        {
            return new UITableConfiguration() {SkipPaging="true", PageSize=20, TableName=""  };
        }
    }
}
