using Intwenty.DataClient.Reflection;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson.Serialization.Attributes;

namespace Intwenty.Areas.Identity.Models
{
    [DbTableName("security_Role")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyRole : IdentityRole
    {
       
       
    }
}
