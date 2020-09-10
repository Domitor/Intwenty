using Intwenty.Data.DBAccess.Annotations;
using Microsoft.AspNetCore.Identity;


namespace Intwenty.Areas.Identity.Models
{
    [DbTableName("security_UserRoles")]
    [DbTablePrimaryKey("UserId,RoleId")]
    public class IntwentyUserRole : IdentityUserRole<string>
    {

        public string Id { get; set; }


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
