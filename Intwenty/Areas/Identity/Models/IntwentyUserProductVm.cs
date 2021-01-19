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
    public class IntwentyUserProductVm : IntwentyOrganizationProductVm
    {
        public IntwentyUserProductVm()
        {
        }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool ModelSaved { get; set; }


    }
}
