using Intwenty.Data.Identity;
using Intwenty.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Intwenty.Data.Localization
{
    public class UserCultureProvider : RequestCultureProvider
    {
        private UserManager<IntwentyUser> UserManager { get; }

        private IntwentySettings Settings { get; }

        public UserCultureProvider(IOptions<IntwentySettings> settings, UserManager<IntwentyUser> usermanager)
        {
            Settings = settings.Value;
            UserManager = usermanager;
        }

        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return Task.FromResult(new ProviderCultureResult(Settings.SiteLanguage));
            }

            var user = UserManager.GetUserAsync(httpContext.User);
            if (user == null)
            {
                return Task.FromResult(new ProviderCultureResult(Settings.SiteLanguage));
            }

            if (string.IsNullOrEmpty(user.Result.Language))
            {
                return Task.FromResult(new ProviderCultureResult(Settings.SiteLanguage));
            }

            return Task.FromResult(new ProviderCultureResult(user.Result.Language));
        }
    }
}
