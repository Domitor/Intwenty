using Intwenty.DataClient.Reflection;
using System;

namespace Intwenty.Entity
{

    [DbTablePrimaryKey("Id")]
    [DbTableName("sysmodel_ViewItem")]
   public class ViewItem
    {
        public ViewItem()
        {

        }

        [AutoIncrement]
        public int Id { get; set; }
        public string SystemMetaCode { get; set; }
        public string AppMetaCode { get; set; }
        public string MetaCode { get; set; }
        public string MetaType { get; set; }
        public string Title { get; set; }
        public string TitleLocalizationKey { get; set; }
        public string Description { get; set; }
        public string DescriptionLocalizationKey { get; set; }

        /// <summary>
        /// How to navigate to this view
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Reference to an Intwenty DataView
        /// </summary>
        public string DataViewMetaCode { get; set; }

        /// <summary>
        /// Reference to a DataTable
        /// </summary>
        public string DataTableMetaCode { get; set; }

        public bool IsPrimaryView { get; set; }

        public string Properties { get; set; }

    }

   

}
