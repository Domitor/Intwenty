using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;


namespace Intwenty.Data.Entity
{
    public class DataViewItem
    {
        [Key]
        public int Id { get; set; }

        public string MetaType { get; set; }

        public string MetaCode { get; set; }

        public string ParentMetaCode { get; set; }

        public string Title { get; set; }

        public string SQLQuery { get; set; }

        public string SQLQueryFieldName { get; set; }

        public int Order { get; set; }

    }

    public class DataViewItemMap
    {
        public DataViewItemMap(EntityTypeBuilder<DataViewItem> entityBuilder)
        {
            entityBuilder.HasIndex(p => new {  p.MetaCode, p.ParentMetaCode }).IsUnique(true);
        }
    }
}
