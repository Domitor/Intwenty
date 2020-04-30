using Intwenty.Data.DBAccess.Annotations;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson.Serialization.Attributes;

namespace Intwenty.Data.Identity
{
    [DbTableName("security_Role")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyRole : IdentityRole
    {
        [BsonId]
        public override string Id { get => base.Id; set => base.Id = value; }
    }
}
