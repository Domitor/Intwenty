using Intwenty.Data.DBAccess.Annotations;
using Microsoft.AspNetCore.Identity;


namespace Intwenty.Data.Identity
{
    [DbTableName("security_UserRoles")]
    [DbTablePrimaryKey("UserId,RoleId")]
    public class IntwentyUserRole : IdentityUserRole<string>
    {
     
    }
}
