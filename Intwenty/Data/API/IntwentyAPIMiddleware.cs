using Intwenty.Data.Routing;
using Intwenty.Interface;
using Intwenty.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Intwenty.Data.API
{
   

    public static class IntwentyAPIMiddleware
    {

        public static void AddIntwentyAPI(this IServiceCollection services)
        {

            //services.AddTransient<IntwentyEndpointTransformer>();

            services.AddSwaggerGen(options => 
            {
                options.DocumentFilter<IntwentySwagerDocHandler>();
                options.AddSecurityDefinition("API-Key", new OpenApiSecurityScheme
                 {
                     Description = "API-Key",
                     Name = "Authorization",
                     In = ParameterLocation.Header,
                     Type = SecuritySchemeType.ApiKey,
                     Scheme = "API-Key"
                      
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "API-Key"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

            }
          );

        }

        public static void UseIntwentyAPI(this IApplicationBuilder builder)
        {

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            builder.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            builder.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Intwenty API V1");
                
               
            });

          

        }
    }
}
