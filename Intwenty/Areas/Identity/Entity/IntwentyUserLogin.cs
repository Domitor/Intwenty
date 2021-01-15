using Intwenty.DataClient.Reflection;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Areas.Identity.Entity
{
    [DbTableName("security_UserLogin")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyUserLogin : IdentityUserLogin<string>
    {
        public string Id { get; set; }

        public string ProductId { get; set; }
    }
}
