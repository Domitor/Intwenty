using Intwenty.Data.DBAccess.Annotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Intwenty.Data.Entity
{

    [DbTablePrimaryKey("Id")]
    [DbTableName("sysmodel_ValueDomainItem")]
    public class ValueDomainItem
    {
        [BsonId]
        [AutoIncrement]
        public int Id { get; set; }

        public string DomainName { get; set; }

        public string Code { get; set; }

        public string Value { get; set; }

        public string Properties { get; set; }

    }

}
