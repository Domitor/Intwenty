using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Moley.Data.Entity
{
    public class InformationStatus
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int Version { get; set; }

        public int OwnerId { get; set; }

        public int ApplicationId { get; set; }

        public string MetaCode { get; set; }

        public string CreatedBy { get; set; }

        public DateTime PerformDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }

    public class InformationStatusMap
    {
        public InformationStatusMap(EntityTypeBuilder<InformationStatus> entityBuilder)
        {

        }
    }
}
