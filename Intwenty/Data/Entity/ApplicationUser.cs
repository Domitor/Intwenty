using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Moley.Data.Entity
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {

        public string FirstName { get; set;  }

        public string LastName { get; set; }

        public string Culture { get; set; }

        public bool IsPublicUser { get; set; }

        public string APIKey { get; set; }
    }
}
