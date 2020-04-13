using System;
using Intwenty.Data.DBAccess.Annotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Intwenty.Data.Entity
{
    [DbTablePrimaryKey("Id")]
    [DbTableName("sysdata_SystemId")]
    public class SystemID
    {
        [BsonId]
        [AutoIncrement]
        public int Id { get; set; }

        public int ApplicationId { get; set; }

        public string MetaType { get; set; }

        public string MetaCode { get; set; }

        public DateTime GeneratedDate { get; set; }

        public string Properties { get; set; }

    }

}
