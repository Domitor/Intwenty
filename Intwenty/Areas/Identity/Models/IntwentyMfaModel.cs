using Intwenty.Areas.Identity.Entity;
using Intwenty.DataClient.Reflection;
using Intwenty.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Areas.Identity.Models
{
   
    /// <summary>
    /// A product that the user has access to via an organization
    /// </summary>
    public class IntwentyMfaModel
    {
        public IntwentyMfaModel()
        {
        }

        public MfaAuthTypes MfaType { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Code { get; set; }
        public string ResultCode { get; set; }


    }
}
