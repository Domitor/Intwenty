using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Intwenty.Model.Dto
{
    public class AuthenticatedProductInfo
    {

        public string ForceAppVersion { get; set; }

        public string ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductURI { get; set; }

        public string ProductAPIPath { get; set; }

        public string UserApiKey { get; set; }

        public string UserFullName { get; set; }

        public string Organization { get; set; }

      
    }
}
