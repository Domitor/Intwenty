using System;
using Microsoft.AspNetCore.Hosting;


[assembly: HostingStartup(typeof(IntwentyDemo.Areas.Identity.IdentityHostingStartup))]
namespace IntwentyDemo.Areas.Identity
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