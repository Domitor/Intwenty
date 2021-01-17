
using Intwenty.Areas.Identity.Entity;
using Intwenty.DataClient.Reflection;
using Microsoft.AspNetCore.Identity;


namespace Intwenty.Areas.Identity.Models
{

    public class IntwentyOrganizationProductVm
    {
        public IntwentyOrganizationProductVm()
        {

        }
        public IntwentyOrganizationProductVm(IntwentyOrganizationProduct entity)
        {
            Id = entity.Id;
            ProductId = entity.ProductId;
            ProductName = entity.ProductName;
            OrganizationId = entity.OrganizationId;
        }
        public int Id { get; set; }

        public string ProductId { get; set; }

        public string ProductName { get; set; }

        public int OrganizationId { get; set; }

    }
}
