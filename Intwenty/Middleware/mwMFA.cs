using Intwenty.Areas.Identity.Models;
using Intwenty.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Intwenty.Middleware
{
  
    public class mwMFA
    {
        private readonly RequestDelegate _next;

        public mwMFA(RequestDelegate next)
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

 

}
