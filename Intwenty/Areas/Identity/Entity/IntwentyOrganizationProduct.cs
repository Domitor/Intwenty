
using Intwenty.DataClient.Reflection;
using Microsoft.AspNetCore.Identity;


namespace Intwenty.Areas.Identity.Entity
{
    [DbTableName("security_OrganizationProducts")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyOrganizationProduct
    {
        [AutoIncrement]
        public int Id { get; set; }

        public string ProductId { get; set; }

        public string ProductName { get; set; }

        public int OrganizationId { get; set; }

        public string ProductURI { get; set; }

        public string APIPath { get; set; }

        public string MobileClientVersion { get; set; }

    }
}
