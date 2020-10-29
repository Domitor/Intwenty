using System;
using Intwenty.Entity;

namespace Intwenty.Model.Dto
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
