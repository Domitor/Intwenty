using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Intwenty;
using Intwenty.Areas.Identity.Models;
using Intwenty.Model;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using Intwenty.Localization;
using System.Linq;
using Intwenty.Areas.Identity.Data;
using Intwenty.SystemEvents;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Intwenty.Services;
using Intwenty.Interface;
using IntwentyDemo.Data;
using Intwenty.Middleware;

namespace IntwentyDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            //Default IntwentyDataService
            //services.AddIntwenty<IntwentyDataService>(Configuration);

            //Customized IntwentyDataService
            services.AddIntwenty<CustomDataService>(Configuration);

            //Default is anonymus athorization, comment out this line and use policy.RequireRole to apply role base authorization
            services.AddScoped<IAuthorizationHandler, IntwentyAllowAnonymousAuthorization>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("IntwentyAppAuthorizationPolicy", policy =>
                {
                    policy.AddRequirements(new IntwentyAllowAnonymousAuthorization());
                    //policy.RequireRole(new string[] { "USER" });
                });

                options.AddPolicy("IntwentyModelAuthorizationPolicy", policy =>
                {
                    policy.AddRequirements(new IntwentyAllowAnonymousAuthorization());
                    //policy.RequireRole(new string[] { "ADMINISTRATOR" });

                });
            });

            //services.AddRazorPages().AddViewLocalization().AddRazorRuntimeCompilation();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            //Set up everything related to intwenty
            app.UseIntwenty();

        }
    }
}
