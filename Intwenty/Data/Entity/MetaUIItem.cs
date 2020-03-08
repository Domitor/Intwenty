using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Intwenty.Data.Entity
{

    public class MetaUIItem
    {
        public MetaUIItem()
        {
        }

        [Key]
        public int Id { get; set; }

        public string MetaType { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string AppMetaCode { get; set; }

        /// <summary>
        /// Reference to a MataDataItem
        /// </summary>
        public string DataMetaCode { get; set; }

        public string MetaCode { get; set; }

        public string ParentMetaCode { get; set; }

        public string CssClass { get; set; }

        public int ColumnOrder { get; set; }

        public int RowOrder { get; set; }

        public string Domain { get; set; }

        public string Properties { get; set; }
    }

    public class MetaUIItemMap
    {
        public MetaUIItemMap(EntityTypeBuilder<MetaUIItem> entityBuilder)
        {
            entityBuilder.HasIndex(p => new { p.AppMetaCode, p.MetaCode }).IsUnique(true);
        }
    }

}
