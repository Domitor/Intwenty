using Intwenty.Areas.Identity.Entity;
using Intwenty.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Areas.Identity.Models
{
    public enum IntwentyPermission
    {
        Read, Modify, Delete
    }
    public class IntwentyUserPermissionItem
    {
     

        public IntwentyUserPermissionItem(IntwentyUserProductPermission p)
        {
            PermissionId = p.PermissionId;
            UserId = p.UserId;
            Title = p.Title;
            PermissionType = p.PermissionType;
            MetaCode = p.MetaCode;
            Read = p.ReadPermission;
            Modify = p.ModifyPermission;
            Delete = p.DeletePermission;
        }

        public IntwentyUserPermissionItem()
        {
           
        }

        public static IntwentyUserPermissionItem CreateApplicationPermission(string metacode, string title)
        {
            var res = new IntwentyUserPermissionItem();
            res.PermissionType = ApplicationModelItem.MetaTypeApplication;
            res.MetaCode = metacode;
            res.Title = title;

            return res;
        }

        public static IntwentyUserPermissionItem CreateSystemPermission(string metacode, string title)
        {
            var res = new IntwentyUserPermissionItem();
            res.PermissionType = SystemModelItem.MetaTypeSystem;
            res.MetaCode = metacode;
            res.Title = title;

            return res;
        }

        public string PermissionId { get; set; }

        public string UserId { get; set; }

        public string PermissionType { get; set; }

        public string MetaCode { get; set; }

        public string Title { get; set; }

        public bool Read { get; set; }

        public bool Modify { get; set; }

        public bool Delete { get; set; }

        public bool IsSystemPermission
        {
            get { return PermissionType == SystemModelItem.MetaTypeSystem; }
        }

        public bool IsApplicationPermission
        {
            get { return PermissionType == ApplicationModelItem.MetaTypeApplication; }
        }


    }
}
