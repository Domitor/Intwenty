using Intwenty.Areas.Identity.Entity;
using Intwenty.DataClient.Reflection;
using Intwenty.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Areas.Identity.Models
{
   
    /// <summary>
    /// A product that the user has access to via an organization
    /// </summary>
    public class IntwentyUserProductVm
    {
        public IntwentyUserProductVm()
        {
            AuthorizationItems = new List<IntwentyProductAuthorizationItem>();
        }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }

        public List<IntwentyProductAuthorizationItem> AuthorizationItems { get; set; }

        public bool ModelSaved { get; set; }


    }
}
