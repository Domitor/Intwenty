using Intwenty.Interface;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace Intwenty.Data.API
{
    public class IntwentySwagerDocHandler : IDocumentFilter
    {

        private readonly IIntwentyModelService _modelservice;

        public IntwentySwagerDocHandler(IIntwentyModelService ms)
        {
            _modelservice = ms;
        }

       
        

      

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {

            swaggerDoc.Components.Schemas.Clear();
            swaggerDoc.Paths.Clear();
            swaggerDoc.Tags = new List<OpenApiTag>();
            swaggerDoc.Info.Title = "Intwenty";
            swaggerDoc.Info.License = new OpenApiLicense() { Name = "MIT" };

            var apps = _modelservice.GetApplicationModels();
            foreach (var a in apps)
            {
                swaggerDoc.Tags.Add(new OpenApiTag() { Name = a.Application.DbName, Description = string.Format("The {0} endpoint", a.Application.DbName) } );


                var path = new OpenApiPathItem();
                var op = new OpenApiOperation() { Description = "GetLatestVersion" }; //Parameters = new List<OpenApiParameter>()
                op.Tags.Add(new OpenApiTag() { Name=a.Application.DbName, Description="Gets the latest version by id" });
                op.Parameters.Add(new OpenApiParameter() { Name = "id", In = ParameterLocation.Path , Required = true, Schema = new OpenApiSchema() { Type = "integer", Format="int32" }});
                var resp = new OpenApiResponse() { Description = "SUCCESS" };
                resp.Content.Add("application/json", new OpenApiMediaType());
                op.Responses.Add("200", resp);
                resp = new OpenApiResponse() { Description = "ERROR" };
                op.Responses.Add("400", resp);
                resp = new OpenApiResponse() { Description = "UNAUTHORIZED" };
                op.Responses.Add("401", resp);
                path.AddOperation(OperationType.Get, op);
                swaggerDoc.Paths.Add("/"+ a.Application.DbName + "/API/GetLatestVersion/{id}", path);

                 path = new OpenApiPathItem();
                op = new OpenApiOperation() { Description = "GetAll" }; 
                op.Tags.Add(new OpenApiTag() { Name = a.Application.DbName, Description = string.Format("Get all", a.Application.DbName) });
                resp = new OpenApiResponse();
                resp = new OpenApiResponse() { Description = "SUCCESS" };
                resp.Content.Add("application/json", new OpenApiMediaType());
                op.Responses.Add("200", resp);
                resp = new OpenApiResponse() { Description = "ERROR" };
                op.Responses.Add("400", resp);
                resp = new OpenApiResponse() { Description = "UNAUTHORIZED" };
                op.Responses.Add("401", resp);
                path.AddOperation(OperationType.Get, op);
                swaggerDoc.Paths.Add("/" + a.Application.DbName + "/API/GetAll", path);

            }

          
           
        }
    }
}
