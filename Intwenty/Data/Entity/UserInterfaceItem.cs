using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Intwenty.Data.Entity
{

    public class UserInterfaceItem
    {
        public UserInterfaceItem()
        {
        }

        [Key]
        public int Id { get; set; }

        public string MetaType { get; set; }

        public string Title { get; set; }

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

    public class UserInterfaceItemMap
    {
        public UserInterfaceItemMap(EntityTypeBuilder<UserInterfaceItem> entityBuilder)
        {
            entityBuilder.HasIndex(p => new { p.AppMetaCode, p.MetaCode }).IsUnique(true);
        }
    }

}
