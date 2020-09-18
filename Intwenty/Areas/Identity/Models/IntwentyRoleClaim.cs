using Intwenty.Data.DBAccess.Annotations;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Areas.Identity.Models
{
    [DbTableName("security_RoleClaim")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyRoleClaim : IdentityRoleClaim<string>
    {
       
    }
}
