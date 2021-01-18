using Intwenty.DataClient.Reflection;
using Intwenty.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Areas.Identity.Entity
{
    [DbTableName("security_Authorization")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyAuthorization
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string ProductId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string AuthorizationItemId { get; set; }
        public string AuthorizationItemName { get; set; }
        public string AuthorizationItemNormalizedName { get; set; }
        public string AuthorizationItemType { get; set; }
        public bool ReadAuth { get; set; }
        public bool ModifyAuth { get; set; }
        public bool DeleteAuth { get; set; }

    }
}
