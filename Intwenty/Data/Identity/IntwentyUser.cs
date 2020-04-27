using Intwenty.Data.DBAccess.Annotations;
using Microsoft.AspNetCore.Identity;


namespace Intwenty.Data.Identity
{
    [DbTableName("security_User")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyUser : IdentityUser
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string APIKey { get; set; }

    }
}
