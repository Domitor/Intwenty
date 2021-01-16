
using Intwenty.DataClient.Reflection;
using Microsoft.AspNetCore.Identity;


namespace Intwenty.Areas.Identity.Entity
{
    [DbTableName("security_ProductCustomer")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyProductCustomer
    {
        [AutoIncrement]
        public int Id { get; set; }

        public string ProductId { get; set; }

        public int OrganizationId { get; set; }

        public string UserId { get; set; }


    }
}
