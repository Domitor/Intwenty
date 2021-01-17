
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
            APIPath = entity.APIPath;
            ProductURI = entity.ProductURI;

        }
        public int Id { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int OrganizationId { get; set; }
        public string ProductURI { get; set; }
        public string APIPath { get; set; }
        public string Organization { get; set; }

    }
}
