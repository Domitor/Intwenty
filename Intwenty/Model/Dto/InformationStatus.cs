using System;
using Intwenty.Entity;

namespace Intwenty.Model.Dto
{
    public class InformationStatus
    {

        public InformationStatus()
        {

        }

        public InformationStatus(Entity.InformationStatus entity)
        {
            Id = entity.Id;
            Version = entity.Version;
            ApplicationId = entity.ApplicationId;
            MetaCode = entity.MetaCode;
            ChangedBy = entity.ChangedBy;
            ChangedDate = entity.ChangedDate;
            CreatedBy = entity.CreatedBy;
            OwnedBy = entity.OwnedBy;
            PerformDate = entity.PerformDate;
            StartDate = entity.StartDate;
            EndDate = entity.EndDate;
        }

        public int Id { get; set; }

        public int Version { get; set; }

        public int ApplicationId { get; set; }

        public string MetaCode { get; set; }

        public string ChangedBy { get; set; }

        public string CreatedBy { get; set; }

        public string OwnedBy { get; set; }

        public DateTime ChangedDate { get; set; }

        public DateTime PerformDate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }

  
}
