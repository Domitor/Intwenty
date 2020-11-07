using Intwenty.DataClient.Reflection;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Areas.Identity.Entity
{
    [DbTableName("security_UserClaim")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyUserClaim : IdentityUserClaim<string>
    {
        [AutoIncrement]
        public override int Id { get => base.Id; set => base.Id = value; }
    }
}
