using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Areas.Identity.Models
{
    public class IntwentyUserPermissionVm
    {

        public string Id { get; set; }

        public string UserName { get; set; }

        public bool ModelSaved { get; set; }

        public List<IntwentyUserRoleItem> UserRoles { get; set; }

        public List<IntwentyUserPermissionItem> UserSystemPermissions { get; set; }

        public List<IntwentyUserPermissionItem> UserApplicationPermissions { get; set; }

        public List<IntwentyUserRoleItem> Roles { get; set; }

        public List<IntwentyUserPermissionItem> ApplicationPermissions { get; set; }

        public List<IntwentyUserPermissionItem> SystemPermissions { get; set; }

        public IntwentyUserPermissionVm()
        {
            UserRoles = new List<IntwentyUserRoleItem>();
            Roles = new List<IntwentyUserRoleItem>();
            UserSystemPermissions = new List<IntwentyUserPermissionItem>();
            UserApplicationPermissions = new List<IntwentyUserPermissionItem>();
            SystemPermissions = new List<IntwentyUserPermissionItem>();
            ApplicationPermissions = new List<IntwentyUserPermissionItem>();
        }
    }
}
