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
     

        public IntwentyUserPermissionItem(IntwentyUserPermission p)
        {
            Id = p.Id;
            UserId = p.UserId;
            UserName = p.UserName;
            Title = p.Title;
            PermissionType = p.PermissionType;
            MetaCode = p.MetaCode;
            Read = p.Read;
            Modify = p.Modify;
            Delete = p.Delete;
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

        public string Id { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

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
