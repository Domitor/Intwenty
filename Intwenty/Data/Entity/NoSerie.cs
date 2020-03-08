using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;


namespace Intwenty.Data.Entity
{
    public class NoSerie
    {
        [Key]
        public int Id { get; set; }

        public string AppMetaCode { get; set; }

        public string MetaCode { get; set; }

        public string DataMetaCode { get; set; }

        public int Counter { get; set; }

        public int StartValue { get; set; }

        public string Prefix { get; set; }

        public string Description { get; set; }

        public string Properties { get; set; }

    }

    public class NoSeriesMap
    {
        public NoSeriesMap(EntityTypeBuilder<NoSerie> entityBuilder)
        {
            entityBuilder.HasIndex(p => new { p.MetaCode }).IsUnique(true);
        }
    }
}
