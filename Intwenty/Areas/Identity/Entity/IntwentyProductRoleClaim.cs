using Intwenty.DataClient.Reflection;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Areas.Identity.Entity
{
    [DbTableName("security_ProductRoleClaim")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyProductRoleClaim : IdentityRoleClaim<string>
    {
       
    }
}
