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
        [NotNull]
        public string ProductId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        [NotNull]
        public int OrganizationId { get; set; }
        [NotNull]
        public string OrganizationName { get; set; }
        [NotNull]
        public string AuthorizationId { get; set; }
        [NotNull]
        public string AuthorizationName { get; set; }
        [NotNull]
        public string AuthorizationNormalizedName { get; set; }
        [NotNull]
        public string AuthorizationType { get; set; }
        public bool ReadAuth { get; set; }
        public bool ModifyAuth { get; set; }
        public bool DeleteAuth { get; set; }

    }
}
