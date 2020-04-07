﻿using System;
using Intwenty.Data.DBAccess.Annotations;




namespace Intwenty.Data.Entity
{
    [DbTablePrimaryKey("Id")]
    [DbTableName("sysdata_EventLog")]
    public class EventLog
    {
        [AutoIncrement]
        public int Id { get; set; }

        public DateTime EventDate { get; set; }

        public string Verbosity { get; set; }

        public string Message { get; set; }

        public string AppMetaCode { get; set; }

        public int ApplicationId { get; set; }

        public string UserName { get; set; }

    }

}
