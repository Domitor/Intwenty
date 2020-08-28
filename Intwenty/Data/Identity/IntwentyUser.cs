using Intwenty.Data.DBAccess.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MongoDB.Bson.Serialization.Attributes;
using System;

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

        public string AuthenticatorKey { get; set; }

        public string LastLogin { get; set; }


    }
}
