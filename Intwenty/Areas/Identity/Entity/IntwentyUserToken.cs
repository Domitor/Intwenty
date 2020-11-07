using Intwenty.DataClient.Reflection;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Areas.Identity.Entity
{
    [DbTableName("security_UserToken")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyUserToken : IdentityUserToken<string>
    {
       public string Id { get; set; }

    }
}
