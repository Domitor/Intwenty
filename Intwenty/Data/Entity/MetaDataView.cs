using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;


namespace Moley.Data.Entity
{
    public class MetaDataView
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

    public class MetaDataViewMap
    {
        public MetaDataViewMap(EntityTypeBuilder<MetaDataView> entityBuilder)
        {
            entityBuilder.HasIndex(p => new {  p.MetaCode, p.ParentMetaCode }).IsUnique(true);
        }
    }
}
