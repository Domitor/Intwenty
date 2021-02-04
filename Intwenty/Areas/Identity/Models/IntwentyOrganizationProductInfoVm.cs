
using Intwenty.Areas.Identity.Entity;
using Intwenty.DataClient.Reflection;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Intwenty.Areas.Identity.Models
{

    public class IntwentyOrganizationProductInfoVm
    {
        public IntwentyOrganizationProductInfoVm()
        {
        }

      
        public int Id { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationTablePrefix { get; set; }
        public string ProductURI { get; set; }
        public string APIPath { get; set; }
        public string MobileClientVersion { get; set; }


    }
}
