using System;
using Microsoft.AspNetCore.Hosting;


[assembly: HostingStartup(typeof(Intwenty.Areas.Identity.IdentityHostingStartup))]
namespace Intwenty.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}