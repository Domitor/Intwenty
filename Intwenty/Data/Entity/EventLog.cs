using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;


namespace Intwenty.Data.Entity
{
    public class EventLog
    {
        [Key]
        public int Id { get; set; }

        public DateTime EventDate { get; set; }

        public string Verbosity { get; set; }

        public string Message { get; set; }

        public string AppMetaCode { get; set; }

        public int ApplicationId { get; set; }

        public string UserName { get; set; }

    }

    public class EventLogMap
    {
        public EventLogMap(EntityTypeBuilder<EventLog> entityBuilder)
        {
            
        }
    }
}
