using Intwenty.DataClient.Reflection;
using Intwenty.Model.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntwentyDemo.Models
{
    [DbTableName("wms_Customer")]
    public class Customer : InformationStatus
    {
        public Customer()
        {

        }

        public string CustomerId { get; set; }

        public string CustomerName{ get; set; }

        public string CustomerPhone{ get; set; }

        public string CustomerEmail { get; set; }
    }

  
}
