using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Areas.Identity.Models
{
    public class IntwentyUserPermissionVm
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public List<IntwentyUserRoleItem> UserRoles { get; set; }

        public List<IntwentyUserPermissionItem> UserPermissions { get; set; }

        public List<IntwentyUserRoleItem> Roles { get; set; }

        public List<IntwentyUserPermissionItem> Permissions { get; set; }

        public IntwentyUserPermissionVm()
        {
            UserRoles = new List<IntwentyUserRoleItem>();
            Roles = new List<IntwentyUserRoleItem>();
            UserPermissions = new List<IntwentyUserPermissionItem>();
            Permissions = new List<IntwentyUserPermissionItem>();
        }
    }
}
