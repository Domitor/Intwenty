using Intwenty.DataClient.Reflection;
using Intwenty.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Areas.Identity.Entity
{
    [DbTableName("security_OrganizationMembers")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyOrganizationMember
    {
        [AutoIncrement]
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }

    }
}
