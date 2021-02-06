using Intwenty.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Intwenty.Model.Dto
{
    public class UserInfo
    {
        public static readonly string DEFAULT_USERID = "INTWENTY_USER";

        public string UserName { get; set; }

        public string UserTablePrefix { get; set; }

        public string OrganizationId { get; set; }

        public string OrganizationName { get; set; }

        public string OrganizationTablePrefix { get; set; }

        public bool HasValidUserId
        {
            get { return !string.IsNullOrEmpty(UserName) && UserName != DEFAULT_USERID; }
        }


        public bool HasValidOrganizationId
        {
            get { return !string.IsNullOrEmpty(OrganizationId); }
        }

       
        public UserInfo()
        {
            UserName = DEFAULT_USERID;
            UserTablePrefix = string.Empty;
            OrganizationId = string.Empty;
            OrganizationName = string.Empty;
            OrganizationTablePrefix = string.Empty;
        }
        public UserInfo(ClaimsPrincipal user)
        {
            UserName = user.Identity.Name;
            UserTablePrefix = user.Identity.GetUserTablePrefix();
            OrganizationId = user.Identity.GetOrganizationId();
            OrganizationName = user.Identity.GetOrganizationName();
            OrganizationTablePrefix = user.Identity.GetOrganizationTablePrefix();
        }

    }
}
