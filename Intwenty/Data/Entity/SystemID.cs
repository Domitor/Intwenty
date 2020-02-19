using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;


namespace Moley.Data.Entity
{
    public class SystemID
    {
        [Key]
        public int Id { get; set; }

        public int ApplicationId { get; set; }

        public string MetaType { get; set; }

        public string MetaCode { get; set; }

        public DateTime GeneratedDate { get; set; }

    }

    public class SystemIDMap
    {
        public SystemIDMap(EntityTypeBuilder<SystemID> entityBuilder)
        {

        }
    }
}
