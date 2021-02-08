using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace Intwenty.Areas.Identity.Data
{

    public static class IdentityExtensions
    {
        public static string GetOrganizationId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("OrganizationId");
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetOrganizationName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("OrganizationName");
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetOrganizationTablePrefix(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("OrganizationTablePrefix");
            return (claim != null) ? claim.Value : string.Empty;
        }


        public static string GetUserFirstName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("FirstName");
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetUserLastName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("LastName");
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetUserTablePrefix(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("UserTablePrefix");
            return (claim != null) ? claim.Value : string.Empty;
        }
    }
    
}
