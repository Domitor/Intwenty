using Intwenty.DataClient.Reflection;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Areas.Identity.Entity
{
    [DbTableName("security_UserProductClaim")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyUserProductClaim : IdentityUserClaim<string>
    {
        [AutoIncrement]
        public override int Id { get => base.Id; set => base.Id = value; }

        public string ProductId { get; set; }
    }
}
