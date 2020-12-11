using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Areas.Identity.Models
{
    public class RegisterVm
    {
        public string Language { get; set; }
        public string AccountType { get; set; }
        public string GroupName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ReturnUrl { get; set; }
        public string Message { get; set; }

    }
}
