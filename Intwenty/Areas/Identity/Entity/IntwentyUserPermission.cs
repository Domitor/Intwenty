using Intwenty.DataClient.Reflection;
using Intwenty.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Areas.Identity.Entity
{
    [DbTableName("security_UserPermission")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyUserPermission
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string Title { get; set; }

        public string PermissionType { get; set; }

        /// <summary>
        /// Reference to an Intwenty.Model.SystemModelItem or Intwenty.Model.ApplicationModelItem  
        /// </summary>
        public string MetaCode { get; set; }

        public bool ReadPermission { get; set; }

        public bool ModifyPermission { get; set; }

        public bool DeletePermission { get; set; }

       
    }
}
