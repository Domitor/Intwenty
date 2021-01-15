using Intwenty.DataClient.Reflection;
using Microsoft.AspNetCore.Identity;

namespace Intwenty.Areas.Identity.Entity
{
    [DbTableName("security_ProductRole")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyProductRole : IdentityRole
    {
       public string ProductId { get; set; }
       
    }
}
