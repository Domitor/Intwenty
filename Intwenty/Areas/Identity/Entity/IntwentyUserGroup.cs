using Intwenty.DataClient.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Areas.Identity.Entity
{
    [DbTableName("security_UserGroup")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyUserGroup
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string GroupId { get; set; }

        public string GroupName { get; set; }

        public string MembershipType { get; set; }

        public string MembershipStatus { get; set; }

    }
}
