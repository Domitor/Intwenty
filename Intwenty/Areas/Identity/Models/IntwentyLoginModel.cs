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
    public class IntwentyLoginModel
    {
        public IntwentyLoginModel()
        {
            UserName = string.Empty;
            Password = string.Empty;
            RedirectUrl = string.Empty;
            ReturnUrl = string.Empty;
            ResultCode = string.Empty;
            ActionCode = string.Empty;
            AuthServiceQRCode = string.Empty;
            AuthServiceUrl = string.Empty;
            AuthServiceStartToken = string.Empty;
        }

        public AccountTypes AccountType { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string RedirectUrl { get; set; }
        public string ReturnUrl { get; set; }
        public string ResultCode { get; set; }
        public string ActionCode { get; set; }
        public string AuthServiceQRCode { get; set; }
        public string AuthServiceUrl { get; set; }
        public string AuthServiceStartToken { get; set; }




    }
}
