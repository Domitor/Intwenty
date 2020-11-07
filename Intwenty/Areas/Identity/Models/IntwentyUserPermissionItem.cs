using Intwenty.Areas.Identity.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Areas.Identity.Models
{
    public enum IntwentyPermission
    {
        Read, Modify, Delete
    }
    public class IntwentyUserPermissionVm
    {
        public IntwentyUserPermissionVm(IntwentyUserPermission p)
        {
            Id = p.Id;
            UserId = p.UserId;
            UserName = p.UserName;
            PermissionType = p.PermissionType;
            PermissionType = p.PermissionType;
            MetaCode = p.MetaCode;
            Read = p.Read;
            Modify = p.Modify;
            Delete = p.Delete;
        }

        public string Id { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string PermissionType { get; set; }

        public string MetaCode { get; set; }

        public bool Read { get; set; }

        public bool Modify { get; set; }

        public bool Delete { get; set; }


    }
}
