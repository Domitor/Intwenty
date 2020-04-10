using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IntwentyDemo.Data.Entity;
using IntwentyDemo.Data;
using Shared;
using Intwenty;


namespace IntwentyDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //System Settings
            services.Configure<SystemSettings>(Configuration.GetSection("SystemSettings"));
            services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddTransient<IIntwentyModelService, IntwentyModelService>();
            services.AddTransient<IIntwentyDataService, IntwentyDataService>();
            services.AddTransient<IIntwentyDbAccessService, IntwentyDbAccessService>();

            var settings = Configuration.GetSection(nameof(SystemSettings)).Get<SystemSettings>();

            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                if (settings.DBMS == 0)
                    options.UseSqlServer(Configuration.GetConnectionString("SqlServerConnection"));
                if (settings.DBMS == 1)
                    options.UseMySql(Configuration.GetConnectionString("MySqlConnection"));
                if (settings.DBMS == 2)
                    options.UseMySql(Configuration.GetConnectionString("MySqlConnection"));
                if (settings.DBMS == 3)
                    options.UseNpgsql(Configuration.GetConnectionString("PostgreSQLConnection"));
                if (settings.DBMS == 4)
                    options.UseSqlite(Configuration.GetConnectionString("SQLiteConnection"));

            });

            /*
            services.AddDbContext<IntwentyDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

            });
            */

            services.AddDefaultIdentity<SystemUser>(options =>
            {
                //options.SignIn.RequireConfirmedAccount = true;
                //options.SignIn.RequireConfirmedPhoneNumber = true;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireNonAlphanumeric = false;
                }) 
               .AddRoles<SystemRole>()
               .AddEntityFrameworkStores<ApplicationDbContext>();

            //Intwenty controller
            var assembly = typeof(Intwenty.Controllers.ApplicationController).Assembly;

            services.AddControllersWithViews().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.WriteIndented = false;
                options.JsonSerializerOptions.DictionaryKeyPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            }).AddApplicationPart(assembly);

            services.AddRazorPages().AddRazorRuntimeCompilation();

            /*
            services.AddRazorPages().AddRazorRuntimeCompilation(options =>
            {
                options.FileProviders.Add(new EmbeddedFileProvider(assembly));

            });
            */


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
           
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
