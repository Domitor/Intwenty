using Intwenty.DataClient.Reflection;
using Microsoft.AspNetCore.Identity;

namespace Intwenty.Areas.Identity.Entity
{

    [DbTableName("security_ProductAuthorizationItem")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyProductAuthorizationItem : IdentityRole
    {
        [NotNull]
        public string ProductId { get; set; }


        [NotNull]
        //ROLE, SYSTEM, APP, VIEW
        public string AuthorizationType { get; set; }

        /// <summary>
        /// Reference to an Intwenty.Model.SystemModelItem or Intwenty.Model.ApplicationModelItem  
        /// </summary>

        [Ignore]
        public string MetaCode 
        {
            get
            {
                return NormalizedName;
            }
        }

       
    }
}
