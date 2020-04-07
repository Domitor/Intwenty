using System;
using Intwenty.Data.DBAccess.Annotations;



namespace Intwenty.Data.Entity
{
    [DbTablePrimaryKey("Id")]
    [DbTableName("sysdata_InformationStatus")]
    public class InformationStatus
    {

        public int Id { get; set; }

        public int Version { get; set; }

        public int OwnerId { get; set; }

        public int ApplicationId { get; set; }

        public string MetaCode { get; set; }

        public string CreatedBy { get; set; }

        public string ChangedBy { get; set; }

        public string OwnedBy { get; set; }

        public DateTime ChangedDate { get; set; }

        public DateTime PerformDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }

}
