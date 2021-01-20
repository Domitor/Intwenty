using Intwenty.Areas.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Intwenty.Model.Dto
{
    public class UserClientInfo
    {

        public string UserApiKey { get; set; }

        public string UserFullName { get; set; }

        public List<IntwentyOrganizationProductInfoVm> ProductInfo { get; set; }

      
    }
}
