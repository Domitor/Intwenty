using Intwenty.DataClient.Reflection;
using Microsoft.AspNetCore.Identity;

namespace Intwenty.Areas.Identity.Entity
{
    [DbTableName("security_ProductPermission")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyProductPermission
    {
        public string Id { get; set; }

        public string ProductId { get; set; }

        public string Title { get; set; }

        public string PermissionType { get; set; }

        /// <summary>
        /// Reference to an Intwenty.Model.SystemModelItem or Intwenty.Model.ApplicationModelItem  
        /// </summary>
        public string MetaCode { get; set; }

       
    }
}
