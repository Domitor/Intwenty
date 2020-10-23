using Intwenty.Data.Helpers;
using Intwenty.Interface;
using Intwenty.Model;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
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
                op.Tags.Add(new OpenApiTag() { Name = a.Application.DbName, Description = string.Format("Get all {0} applications", a.Application.DbName) });
                resp = new OpenApiResponse() { Description = "SUCCESS" };
                resp.Content.Add("application/json", new OpenApiMediaType());
                op.Responses.Add("200", resp);
                resp = new OpenApiResponse() { Description = "ERROR" };
                op.Responses.Add("400", resp);
                resp = new OpenApiResponse() { Description = "UNAUTHORIZED" };
                op.Responses.Add("401", resp);
                path.AddOperation(OperationType.Get, op);
                swaggerDoc.Paths.Add("/" + a.Application.DbName + "/API/GetAll", path);

                path = new OpenApiPathItem();
                op = new OpenApiOperation() { Description = "Save" };
                op.RequestBody = new OpenApiRequestBody();
                var content = new KeyValuePair<string, OpenApiMediaType>("application/json", new OpenApiMediaType());
                content.Value.Schema = new OpenApiSchema();
                content.Value.Schema.Example = GetSaveUpdateSchema(a);
                op.RequestBody.Content.Add(content);
                op.RequestBody.Required = true;
               
                op.Tags.Add(new OpenApiTag() { Name = a.Application.DbName, Description = string.Format("Create or update {0}", a.Application.DbName) });
                resp = new OpenApiResponse() { Description = "SUCCESS" };
                resp.Content.Add("application/json", new OpenApiMediaType());
                op.Responses.Add("200", resp);
                resp = new OpenApiResponse() { Description = "ERROR" };
                op.Responses.Add("400", resp);
                resp = new OpenApiResponse() { Description = "UNAUTHORIZED" };
                op.Responses.Add("401", resp);
                path.AddOperation(OperationType.Post, op);
                swaggerDoc.Paths.Add("/" + a.Application.DbName + "/API/Save", path);

            }

          
           
        }

        private OpenApiString GetSaveUpdateSchema(ApplicationModel model)
        {
          

            try
            {

                var sep = "";
                var sb = new StringBuilder();
                sb.Append("{");
                sb.Append("\"" + model.Application.DbName + "\":{");

                foreach (var col in model.DataStructure.Where(p => p.IsMetaTypeDataColumn && p.IsRoot))
                {
                    if (col.DbName.ToUpper() == "APPLICATIONID")
                        sb.Append(sep + DBHelpers.GetJSONValue(col.DbName, model.Application.Id));
                    else if (col.IsNumeric)
                        sb.Append(sep + DBHelpers.GetJSONValue(col.DbName, 0));
                    else if (col.IsDateTime)
                        sb.Append(sep + DBHelpers.GetJSONValue(col.DbName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                    else
                        sb.Append(sep + DBHelpers.GetJSONValue(col.DbName, "string"));

                    sep = ",";
                }
               
                sb.Append("}");

                foreach (var dbtbl in model.DataStructure)
                {
                    if (dbtbl.IsMetaTypeDataTable && dbtbl.IsRoot)
                    {
                        sb.Append(",\"" + dbtbl.DbName + "\":[{");
                        sep = "";
                        foreach (var col in model.DataStructure.Where(p => p.IsMetaTypeDataColumn && !p.IsRoot && p.ParentMetaCode == dbtbl.MetaCode))
                        {
                            if (col.DbName.ToUpper() == "APPLICATIONID")
                                sb.Append(sep + DBHelpers.GetJSONValue(col.DbName, model.Application.Id));
                            else if (col.IsNumeric)
                                sb.Append(sep + DBHelpers.GetJSONValue(col.DbName, 0));
                            else if (col.IsDateTime)
                                sb.Append(sep + DBHelpers.GetJSONValue(col.DbName, DateTime.Now.AddYears((DateTime.Now.Year-1973)*-1).ToString("yyyy-MM-dd HH:mm:ss")));
                            else
                                sb.Append(sep + DBHelpers.GetJSONValue(col.DbName, "string"));

                            sep = ",";
                        }

                        sb.Append("}]");
                    }
                }

                sb.Append("}");

                //var doc = System.Text.Json.JsonDocument.Parse(sb.ToString());

                var t = new OpenApiString(sb.ToString());

                return t;

            }
            catch
            {
                
            }
           

            return null;
        }

    }
}
