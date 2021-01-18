using Intwenty.DataClient.Reflection;
using Microsoft.AspNetCore.Identity;

namespace Intwenty.Areas.Identity.Entity
{

    [DbTableName("security_ProductAuthorizationItem")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyProductAuthorizationItem : IdentityRole
    {

        public string ProductId { get; set; }


        //ROLE, SYSTEM, APP, VIEW
        public string AuthorizationType { get; set; }

        /// <summary>
        /// Reference to an Intwenty.Model.SystemModelItem or Intwenty.Model.ApplicationModelItem  
        /// </summary>
        public string MetaCode 
        {
            get
            {
                return NormalizedName;
            }
        }

       
    }
}
