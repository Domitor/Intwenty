using Intwenty.Interface;
using Intwenty.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intwenty.Data.Routing
{
   

    public static class IntwentyRoutingMiddlewareBuilder
    {
        public static void UseIntwentyRouting(this IApplicationBuilder builder)
        {
            builder.UseEndpoints(endpoints =>
            {

                endpoints.MapDefaultControllerRoute();

                //INTWENTY ROUTING
                endpoints.MapControllerRoute("approute_1", "{controller=Application}/{action=All}/{id}");
                endpoints.MapControllerRoute("approute_2", "{controller=Application}/{action=Edit}/{applicationid}/{id}");
                endpoints.MapControllerRoute("apiroute_1", "Application/API/{action=All}/{id?}", defaults: new { controller = "ApplicationAPI" });
                endpoints.MapControllerRoute("apiroute_2", "Application/API/{action=All}/{applicationid?}/{id?}", defaults: new { controller = "ApplicationAPI" });

                //Register endpoints
                var modelservice = builder.ApplicationServices.GetRequiredService<IIntwentyModelService>();
                var epmodels = modelservice.GetEndpointModels();

                foreach (var ep in epmodels)
                {
                    endpoints.MapControllerRoute(ep.MetaCode, ep.Path + "{action="+ep.Action+"}/{id?}", defaults: new { controller = "DynamicApplication" });
                }

                //endpoints.MapDynamicControllerRoute<IntwentyEndpointTransformer>("/cp/{**slug}");


                endpoints.MapRazorPages();

                endpoints.MapHub<Intwenty.PushData.ServerToClientPush>("/serverhub");
            });


        }
    }

    /*
    class IntwentyEndpointTransformer : DynamicRouteValueTransformer
    {
        private readonly IIntwentyModelService _modelservice;

        public IntwentyEndpointTransformer(IIntwentyModelService ms)
        {
            this._modelservice = ms;
        }

        public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            var path = httpContext.Request.Path.ToString();
            var ep = _modelservice.GetEndpointModels().Find(p => (p.Path + p.Action).ToUpper() == path.ToUpper());
            if (ep != null)
            {
                

            }

            //var productString = values[“product”] as string;
            //var id = await this._productLocator.FindProduct(“product”, out var controller);

            //values[“controller”] = controller;
            //values[“action”] = "Get";
            //values[“id”] = id;

            return values;
        }
    }
    */
}
