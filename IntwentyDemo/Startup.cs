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
            //Settings = Configuration.GetSection("IntwentySettings").Get<IntwentySettings>();
        }

        public IConfiguration Configuration { get; }

        //public IntwentySettings Settings { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddIntwenty<CustomDataService>(Configuration);

            services.AddScoped<IAuthorizationHandler, IntwentyAllowAnonymousAuthorization>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("IntwentyAppAuthorizationPolicy", policy =>
                {
                    policy.AddRequirements(new IntwentyAllowAnonymousAuthorization());
                });

                options.AddPolicy("IntwentyModelAuthorizationPolicy", policy =>
                {
                    policy.AddRequirements(new IntwentyAllowAnonymousAuthorization());
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


            app.UseIntwenty();

        }
    }
}
