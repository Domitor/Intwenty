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
using Microsoft.Extensions.DependencyInjection;

namespace Intwenty.Data.Localization
{
    public class UserCultureProvider : RequestCultureProvider
    {
      

        public UserCultureProvider()
        {
            
        }

        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            IntwentySettings Settings = httpContext.RequestServices.GetRequiredService<IOptions<IntwentySettings>>().Value;

            UserManager<IntwentyUser> UserManager = httpContext.RequestServices.GetRequiredService<UserManager<IntwentyUser>>();

            if (httpContext.Request.Path.HasValue && httpContext.Request.Path.Value.Contains("Model/"))
                return Task.FromResult(new ProviderCultureResult(Settings.DefaultCulture));

            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return Task.FromResult(new ProviderCultureResult(Settings.DefaultCulture));
            }

            var user = UserManager.GetUserAsync(httpContext.User).Result;
            if (user == null)
            {
                return Task.FromResult(new ProviderCultureResult(Settings.DefaultCulture));
            }

            if (string.IsNullOrEmpty(user.Culture))
            {
                return Task.FromResult(new ProviderCultureResult(Settings.DefaultCulture));
            }

            return Task.FromResult(new ProviderCultureResult(user.Culture));
        }
    }
}
