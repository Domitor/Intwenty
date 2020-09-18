using Intwenty.Data.DBAccess.Annotations;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Areas.Identity.Models
{
    [DbTableName("security_UserLogin")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyUserLogin : IdentityUserLogin<string>
    {
        public string Id { get; set; }
    }
}
