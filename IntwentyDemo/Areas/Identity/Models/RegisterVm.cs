using System;
using System.Collections.Generic;
using System.Text;

namespace IntwentyDemo.Areas.Identity.Models
{
    public class RegisterVm
    {

        public string Language { get; set; }


        public string AccountType { get; set; }


        public string GroupName { get; set; }


        public string Email { get; set; }


        public string Password { get; set; }


        public string ConfirmPassword { get; set; }

        public string ReturnUrl { get; set; }

    }
}
