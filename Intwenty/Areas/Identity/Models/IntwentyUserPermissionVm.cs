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

        public List<IntwentyAuthorizationVm> UserSystemPermissions { get; set; }

        public List<IntwentyAuthorizationVm> UserApplicationPermissions { get; set; }

        public List<IntwentyUserRoleItem> Roles { get; set; }

        public List<IntwentyAuthorizationVm> ApplicationPermissions { get; set; }

        public List<IntwentyAuthorizationVm> SystemPermissions { get; set; }

        public IntwentyUserPermissionVm()
        {
            UserRoles = new List<IntwentyUserRoleItem>();
            Roles = new List<IntwentyUserRoleItem>();
            UserSystemPermissions = new List<IntwentyAuthorizationVm>();
            UserApplicationPermissions = new List<IntwentyAuthorizationVm>();
            SystemPermissions = new List<IntwentyAuthorizationVm>();
            ApplicationPermissions = new List<IntwentyAuthorizationVm>();
        }
    }
}
