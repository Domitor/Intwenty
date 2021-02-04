using Intwenty.DataClient.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace Intwenty.Areas.Identity.Entity
{
    [DbTableIndex("USR_IDX_1", true, "UserName")]
    [DbTableIndex("USR_IDX_2", true, "Email")]
    [DbTableIndex("USR_IDX_3", true, "APIKey")]
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

        public string LastLoginProduct { get; set; }

        public bool CreatedWithExternalProvider { get; set; }

        public string TablePrefix { get; set; }

        [Ignore]
        public string FullName
        {

            get
            {
                var s = "";
                if (!string.IsNullOrEmpty(FirstName))
                    s += FirstName;
                if (!string.IsNullOrEmpty(LastName))
                    s += " " + LastName;

                return s;

            }

        }


    }
}
