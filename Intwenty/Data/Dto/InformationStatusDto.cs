using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Intwenty.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Intwenty.Data.Dto
{
    public class InformationStatusDto
    {

        public InformationStatusDto()
        {

        }

        public InformationStatusDto(InformationStatus entity)
        {
            Id = entity.Id;
            Version = entity.Version;
            ApplicationId = entity.ApplicationId;
            MetaCode = entity.MetaCode;
        }

        public int Id { get; set; }

        public int Version { get; set; }

        public int ApplicationId { get; set; }

        public string MetaCode { get; set; }

        public DateTime PerformDate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }

  
}
