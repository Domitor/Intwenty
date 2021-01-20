
using Intwenty.Areas.Identity.Entity;
using Intwenty.DataClient.Reflection;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Intwenty.Areas.Identity.Models
{

    public class IntwentyOrganizationProductVm
    {
        public IntwentyOrganizationProductVm()
        {
            RoleAuthorizations = new List<IntwentyAuthorizationVm>();
            ViewAuthorizations = new List<IntwentyAuthorizationVm>();
            SystemAuthorizations = new List<IntwentyAuthorizationVm>();
            ApplicationAuthorizations = new List<IntwentyAuthorizationVm>();
        }
        public IntwentyOrganizationProductVm(IntwentyOrganizationProduct entity)
        {
            Id = entity.Id;
            ProductId = entity.ProductId;
            ProductName = entity.ProductName;
            OrganizationId = entity.OrganizationId;
            APIPath = entity.APIPath;
            ProductURI = entity.ProductURI;
            MobileClientVersion = entity.MobileClientVersion;
            RoleAuthorizations = new List<IntwentyAuthorizationVm>();
            ViewAuthorizations = new List<IntwentyAuthorizationVm>();
            SystemAuthorizations = new List<IntwentyAuthorizationVm>();
            ApplicationAuthorizations = new List<IntwentyAuthorizationVm>();
        }

        public int Id { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string ProductURI { get; set; }
        public string APIPath { get; set; }
        public string MobileClientVersion { get; set; }


        public List<IntwentyAuthorizationVm> RoleAuthorizations { get; set; }

        public List<IntwentyAuthorizationVm> ViewAuthorizations { get; set; }

        public List<IntwentyAuthorizationVm> SystemAuthorizations { get; set; }

        public List<IntwentyAuthorizationVm> ApplicationAuthorizations { get; set; }

    }
}
