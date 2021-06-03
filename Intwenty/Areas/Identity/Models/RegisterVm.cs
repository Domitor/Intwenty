using Intwenty.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Areas.Identity.Models
{
    public class RegisterVm
    {
        public string ActionCode { get; set; }
        public string UserName { get; set; }
        public string Language { get; set; }
        public AccountTypes AccountType { get; set; }
        public string GroupName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ReturnUrl { get; set; }
        public string Message { get; set; }     
        public string ResultCode { get; set; }
        public string AuthServiceQRCode { get; set; }
        public string AuthServiceUrl { get; set; }
        public string AuthServiceOrderRef { get; set; }
        public string AuthServiceStartToken { get; set; }


    }
}
