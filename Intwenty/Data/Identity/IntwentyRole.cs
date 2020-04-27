using Intwenty.Data.DBAccess.Annotations;
using Microsoft.AspNetCore.Identity;


namespace Intwenty.Data.Identity
{
    [DbTableName("security_Role")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyRole : IdentityRole
    {

    }
}
