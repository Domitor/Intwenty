
using Intwenty.DataClient.Reflection;
using Microsoft.AspNetCore.Identity;


namespace Intwenty.Areas.Identity.Entity
{
    [DbTableName("security_UserProduct")]
    [DbTablePrimaryKey("UserId,ProductId")]
    public class IntwentyUserProduct
    {

        public string UserId { get; set; }

        public string ProductId { get; set; }


    }
}
