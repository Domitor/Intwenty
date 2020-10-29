using Intwenty.DataClient.Reflection;
using System;

namespace Intwenty.Entity
{
    [DbTableIndex("UI_IDX_1", true, "AppMetaCode,MetaCode")]
    [DbTablePrimaryKey("Id")]
    [DbTableName("sysmodel_UserInterfaceItem")]
    public class UserInterfaceItem
    {

        [AutoIncrement]
        public int Id { get; set; }

        public string MetaType { get; set; }

        public string Title { get; set; }

        public string TitleLocalizationKey { get; set; }

        public string Description { get; set; }

        public string AppMetaCode { get; set; }

        /// <summary>
        /// Reference to a DatabaseItem
        /// </summary>
        public string DataMetaCode { get; set; }

        /// <summary>
        /// Reference to a DataViewItem
        /// </summary>
        public string ViewMetaCode { get; set; }

        public string DataMetaCode2 { get; set; }

        public string ViewMetaCode2 { get; set; }

        public string MetaCode { get; set; }

        public string ParentMetaCode { get; set; }

        public int ColumnOrder { get; set; }

        public int RowOrder { get; set; }

        public string Domain { get; set; }

        public string Properties { get; set; }
    }


}
