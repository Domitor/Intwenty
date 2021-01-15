
using Intwenty.DataClient.Reflection;
using Microsoft.AspNetCore.Identity;


namespace Intwenty.Areas.Identity.Entity
{
    [DbTableName("security_UserRoles")]
    [DbTablePrimaryKey("UserId,RoleId,ProductId")]
    public class IntwentyUserProductRole : IdentityUserRole<string>
    {

        public string Id { get; set; }

        public string ProductId { get; set; }

        public override string UserId
        {
            get { return base.UserId; }
            set
            {
                base.UserId = value;
                Id = base.UserId + "#" + base.RoleId;
             
            }

        }

        public override string RoleId
        {
            get { return base.RoleId; }
            set
            {
                base.RoleId = value;
                Id = base.UserId + "#" + base.RoleId;
      
            }

        }


    }
}
