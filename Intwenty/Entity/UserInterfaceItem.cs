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
        public string SystemMetaCode { get; set; }
        public string AppMetaCode { get; set; }
        public string ParentMetaCode { get; set; }
        public string MetaCode { get; set; }


        /// <summary>
        /// Reference to an Intwenty DataView
        /// </summary>
        public string DataViewMetaCode { get; set; }

        /// <summary>
        /// Reference to a DataTable
        /// </summary>
        public string DataTableMetaCode { get; set; }


        /// <summary>
        /// Reference to a Database table Columns
        /// </summary>
        public string DataColumn1MetaCode { get; set; }
        public string DataColumn2MetaCode { get; set; }

        /// <summary>
        /// Reference to Intwenty DataView Columns
        /// </summary>
        public string DataViewColumn1MetaCode { get; set; }
        public string DataViewColumn2MetaCode { get; set; }



       

        public int ColumnOrder { get; set; }

        public int RowOrder { get; set; }

        /// <summary>
        /// Reference to a value domain
        /// </summary>
        public string Domain { get; set; }

        public string Properties { get; set; }

        public string RawHTML { get; set; }
    }


}
