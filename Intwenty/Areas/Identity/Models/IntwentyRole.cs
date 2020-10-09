using Intwenty.DataClient.Reflection;
using Microsoft.AspNetCore.Identity;

namespace Intwenty.Areas.Identity.Models
{
    [DbTableName("security_Role")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyRole : IdentityRole
    {
       
       
    }
}
