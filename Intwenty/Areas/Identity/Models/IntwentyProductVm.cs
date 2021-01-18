using Intwenty.Areas.Identity.Entity;
using Intwenty.DataClient.Reflection;
using Intwenty.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Areas.Identity.Models
{
   
    public class IntwentyProductVm
    {
        public IntwentyProductVm()
        {
            AuthorizationItems = new List<IntwentyProductAuthorizationItem>();
        }

        public IntwentyProductVm(IntwentyProduct entity)
        {
            Id = entity.Id;
            ProductName = entity.ProductName;
            AuthorizationItems = new List<IntwentyProductAuthorizationItem>();
        }

        public string Id { get; set; }

        public string ProductName { get; set; }

        public List<IntwentyProductAuthorizationItem> AuthorizationItems { get; set; }

        public bool ModelSaved { get; set; }


    }
}
