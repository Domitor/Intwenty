using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Areas.Identity.Models
{
    public class PermissionVm
    {

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string PermissionType { get; set; }

        public string MetaCode { get; set; }

        public bool Read { get; set; }

        public bool Modify { get; set; }

        public bool Delete { get; set; }


    }
}
