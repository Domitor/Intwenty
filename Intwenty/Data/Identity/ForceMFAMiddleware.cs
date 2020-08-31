using Intwenty.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Intwenty.Data.Identity
{
  
    public class ForceMFAMiddleware
    {
        private readonly RequestDelegate _next;

        public ForceMFAMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpcontext,
            UserManager<IntwentyUser> usermanager,
            IOptions<IntwentySettings> settings)
        {


            if (settings.Value.ForceTwoFactorAuthentication &&
                httpcontext.User.Identity.IsAuthenticated &&
                !string.IsNullOrEmpty(httpcontext.User.Identity.Name))
            {
                var user = await usermanager.FindByNameAsync(httpcontext.User.Identity.Name);
               
                if (!user.TwoFactorEnabled && 
                    !httpcontext.Request.Path.Value.Contains("EnableAuthenticator"))
                {
                    httpcontext.Response.Redirect("/Identity/Account/Manage/EnableAuthenticator");

                }

             
            }

            await _next(httpcontext);
        }
    }

    public static class ForceMFAMiddlewareBuilder
    {
        public static IApplicationBuilder UseForceMFAMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ForceMFAMiddleware>();
        }
    }


}
