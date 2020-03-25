using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Intwenty.Data.Entity
{

    public class DatabaseItem
    {
        public DatabaseItem()
        {


        }

        [Key]
        public int Id { get; set; }

        public string MetaType { get; set; }

        public string Description { get; set; }

        public string AppMetaCode { get; set; }

        public string MetaCode { get; set; }

        public string ParentMetaCode { get; set; }

        public string DbName { get; set; }

        public string DataType { get; set; }

        public string Domain { get; set; }

        public string Properties { get; set; }

        public bool Mandatory { get; set; }
    }

    public class DatabaseItemMap
    {
        public DatabaseItemMap(EntityTypeBuilder<DatabaseItem> entityBuilder)
        {
            entityBuilder.HasIndex(p => new { p.AppMetaCode, p.MetaCode }).IsUnique(true);
            entityBuilder.HasIndex(p => new { p.AppMetaCode, p.ParentMetaCode, p.MetaType, p.DbName }).IsUnique(true);
        }
    }

}
