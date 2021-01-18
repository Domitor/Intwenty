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
    public class IntwentyAuthorizationVm
    {
     

        public IntwentyAuthorizationVm(IntwentyAuthorization entity)
        {
            Id = entity.Id;
            AutorizationItemType = entity.AutorizationItemType;
            AutorizationItemId = entity.AuthorizationItemId;
            AutorizationItemName = entity.AuthorizationItemName;
            AuthorizationItemNormalizedName = entity.AuthorizationItemNormalizedName;
            UserId = entity.UserId;
            UserName = entity.UserName;
            ProductId = entity.ProductId;
            OrganizationId = entity.OrganizationId;
            OrganizationName = entity.OrganizationName;
            Read = entity.Read;
            Modify = entity.Modify;
            Delete = entity.Delete;
        }

        public IntwentyAuthorizationVm()
        {
           
        }

        /*
        public static IntwentyAuthorizationVm CreateApplicationPermission(string metacode, string title)
        {
            var res = new IntwentyAuthorizationVm();
            res.PermissionType = ApplicationModelItem.MetaTypeApplication;
            res.MetaCode = metacode;
            res.Title = title;

            return res;
        }

        public static IntwentyAuthorizationVm CreateSystemPermission(string metacode, string title)
        {
            var res = new IntwentyAuthorizationVm();
            res.PermissionType = SystemModelItem.MetaTypeSystem;
            res.MetaCode = metacode;
            res.Title = title;

            return res;
        }
        */

        public int Id { get; set; }
        public string ProductId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string AutorizationItemId { get; set; }
        public string AutorizationItemName { get; set; }
        public string AuthorizationItemNormalizedName { get; set; }
        public string AutorizationItemType { get; set; }
        public bool Read { get; set; }
        public bool Modify { get; set; }
        public bool Delete { get; set; }

        public bool IsSystemAuthorization
        {
            get { return AutorizationItemType == SystemModelItem.MetaTypeSystem; }
        }

        public bool IsApplicationAuthorization
        {
            get { return AutorizationItemType == ApplicationModelItem.MetaTypeApplication; }
        }

        public bool IsProductAuthorization
        {
            get { return AutorizationItemType == "ROLE"; }
        }

        public bool IsViewAuthorization
        {
            get { return AutorizationItemType == "VIEW"; }
        }


    }
}
