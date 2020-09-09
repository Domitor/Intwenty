using Intwenty.Data.DBAccess.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Data.Identity
{
    [DbTableName("security_UserGroup")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyUserGroup
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string GroupName { get; set; }

        public string MembershipType { get; set; }

        public string MembershipStatus { get; set; }

    }
}
