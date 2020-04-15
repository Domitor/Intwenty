using System;
using Intwenty.Data.DBAccess.Annotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Intwenty.Data.Entity
{
    [DbTablePrimaryKey("Id")]
    [DbTableName("sysdata_DefaultValues")]
    public class DefaultValue
    {
        [BsonId]
        [AutoIncrement]
        public int Id { get; set; }

        public int ApplicationId { get; set; }

        public string TableName { get; set; }

        public string ColumnName { get; set; }

        public int Count { get; set; }

        public string LatestValue { get; set; }

        public DateTime GeneratedDate { get; set; }

    }

}
