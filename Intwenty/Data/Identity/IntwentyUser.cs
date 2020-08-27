using Intwenty.Data.DBAccess.Annotations;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson.Serialization.Attributes;

namespace Intwenty.Data.Identity
{
    [DbTableName("security_User")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyUser : IdentityUser
    {
        //[BsonId]
        //public override string Id { get => base.Id; set => base.Id = value; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string APIKey { get; set; }

        public string Culture { get; set; }

    }
}
