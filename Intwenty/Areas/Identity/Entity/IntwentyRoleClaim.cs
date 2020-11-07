using Intwenty.DataClient.Reflection;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Areas.Identity.Entity
{
    [DbTableName("security_RoleClaim")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyRoleClaim : IdentityRoleClaim<string>
    {
       
    }
}
