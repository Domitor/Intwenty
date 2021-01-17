using Intwenty.Areas.Identity.Entity;
using Intwenty.DataClient.Reflection;
using Intwenty.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Areas.Identity.Models
{
   
    public class IntwentyOrganizationVm
    {
        public IntwentyOrganizationVm()
        {
            Members = new List<IntwentyOrganizationMemberVm>();
            Products = new List<IntwentyOrganizationProductVm>();
        }

        public IntwentyOrganizationVm(IntwentyOrganization entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Members = new List<IntwentyOrganizationMemberVm>();
            Products = new List<IntwentyOrganizationProductVm>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public List<IntwentyOrganizationMemberVm> Members { get; set; }

        public List<IntwentyOrganizationProductVm> Products { get; set; }

        public bool ModelSaved { get; set; }



    }
}
