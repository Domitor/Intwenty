using Intwenty.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Intwenty.Model.Dto
{
    public class ValueDomainVm
    {
        public int Id { get; set; }

        public string DomainName { get; set; }

        public string Code { get; set; }

        public string Value { get; set; }

        public string Display { get; set; }

    }
}
