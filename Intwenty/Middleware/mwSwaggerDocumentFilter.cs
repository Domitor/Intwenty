using Intwenty.Helpers;
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

namespace Intwenty.Middleware
{
    public class mwSwaggerDocumentFilter : IDocumentFilter
    {

        private readonly IIntwentyModelService _modelservice;

        public mwSwaggerDocumentFilter(IIntwentyModelService ms)
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

            var epmodels = _modelservice.GetEndpointModels();
            var domains = _modelservice.GetValueDomains();

            var endpoinggroup = new OpenApiTag() { Name = "Endpoints"};

            foreach (var ep in epmodels)
            {


                if (ep.IsMetaTypeTableGetById && ep.IsDataTableConnected)
                {
                    var path = new OpenApiPathItem();
                    var op = new OpenApiOperation() { Description = ep.Description };
                    if (string.IsNullOrEmpty(ep.Title))
                        op.Summary = string.Format("Retrieve data from the {0} table", ep.DataTableInfo.DbName);
                    else
                        op.Summary = ep.Title;
                    op.Tags.Add(endpoinggroup);
                    op.Parameters.Add(new OpenApiParameter() { Name = "id", In = ParameterLocation.Path, Required = true, Schema = new OpenApiSchema() { Type = "integer", Format = "int32" } });
                    var resp = new OpenApiResponse() { Description = "SUCCESS" };
                    resp.Content.Add("application/json", new OpenApiMediaType());
                    op.Responses.Add("200", resp);
                    resp = new OpenApiResponse() { Description = "ERROR" };
                    op.Responses.Add("400", resp);
                    resp = new OpenApiResponse() { Description = "UNAUTHORIZED" };
                    op.Responses.Add("401", resp);
                    path.AddOperation(OperationType.Get, op);
                    swaggerDoc.Paths.Add(ep.Path + ep.Action + "/{id}", path);
                }

                if (ep.IsMetaTypeTableGetAll && ep.IsDataTableConnected)
                {
                    var path = new OpenApiPathItem();
                    var op = new OpenApiOperation() { Description = ep.Description };
                    if (string.IsNullOrEmpty(ep.Title))
                        op.Summary = string.Format("Retrieve data from the {0} table", ep.DataTableInfo.DbName);
                    else
                        op.Summary = ep.Title;
                    op.Tags.Add(endpoinggroup);
                    var resp = new OpenApiResponse() { Description = "SUCCESS" };
                    resp.Content.Add("application/json", new OpenApiMediaType());
                    op.Responses.Add("200", resp);
                    resp = new OpenApiResponse() { Description = "ERROR" };
                    op.Responses.Add("400", resp);
                    resp = new OpenApiResponse() { Description = "UNAUTHORIZED" };
                    op.Responses.Add("401", resp);
                    path.AddOperation(OperationType.Get, op);
                    swaggerDoc.Paths.Add(ep.Path + ep.Action, path);
                }

                if (ep.IsMetaTypeTableSave && ep.DataTableInfo.MetaCode == ep.AppMetaCode)
                {
                    var path = new OpenApiPathItem();
                    var op = new OpenApiOperation() { Description = ep.Description, Summary = ep.Title };
                    op.RequestBody = new OpenApiRequestBody();
                    var content = new KeyValuePair<string, OpenApiMediaType>("application/json", new OpenApiMediaType());
                    content.Value.Schema = new OpenApiSchema();
                    content.Value.Schema.Example = GetSaveUpdateSchema(ep);
                    op.RequestBody.Content.Add(content);
                    op.RequestBody.Required = true;
                    op.Tags.Add(endpoinggroup);
                    var resp = new OpenApiResponse() { Description = "SUCCESS" };
                    resp.Content.Add("application/json", new OpenApiMediaType());
                    op.Responses.Add("200", resp);
                    resp = new OpenApiResponse() { Description = "ERROR" };
                    op.Responses.Add("400", resp);
                    resp = new OpenApiResponse() { Description = "UNAUTHORIZED" };
                    op.Responses.Add("401", resp);
                    path.AddOperation(OperationType.Post, op);
                    swaggerDoc.Paths.Add(ep.Path + ep.Action, path);

                }


                if (ep.IsMetaTypeDataViewGetData && ep.IsDataViewConnected)
                {
                    var path = new OpenApiPathItem();
                    var op = new OpenApiOperation() { Description = ep.Description };
                    if (string.IsNullOrEmpty(ep.Title))
                        op.Summary = string.Format("Retrieve data from the '{0}' data view", ep.DataViewInfo.Title);
                    else
                        op.Summary = ep.Title;
                    op.Tags.Add(endpoinggroup);
                    var resp = new OpenApiResponse() { Description = "SUCCESS" };
                    resp.Content.Add("application/json", new OpenApiMediaType());
                    op.Responses.Add("200", resp);
                    resp = new OpenApiResponse() { Description = "ERROR" };
                    op.Responses.Add("400", resp);
                    resp = new OpenApiResponse() { Description = "UNAUTHORIZED" };
                    op.Responses.Add("401", resp);
                    path.AddOperation(OperationType.Get, op);
                    swaggerDoc.Paths.Add(ep.Path + ep.Action, path);
                }


            }
            

        }

        private OpenApiString GetSaveUpdateSchema(EndpointModelItem epitem)
        {
          

            try
            {

                if (!epitem.IsDataViewConnected || string.IsNullOrEmpty(epitem.AppMetaCode))
                    return new OpenApiString("");

                var models = _modelservice.GetApplicationModels();
                var model = models.Find(p => p.Application.MetaCode == epitem.AppMetaCode);
                if (model==null)
                    return new OpenApiString("");

                var sep = "";
                var sb = new StringBuilder();
                sb.Append("{");
                sb.Append("\"" + model.Application.DbName + "\":{");

                foreach (var col in model.DataStructure.Where(p => p.IsMetaTypeDataColumn && p.IsRoot))
                {
                    if (col.DbName.ToUpper() == "APPLICATIONID")
                        continue;
                        //sb.Append(sep + DBHelpers.GetJSONValue(col.DbName, model.Application.Id));
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
                                continue;
                            //sb.Append(sep + DBHelpers.GetJSONValue(col.DbName, model.Application.Id));
                            else if (col.IsNumeric)
                                sb.Append(sep + DBHelpers.GetJSONValue(col.DbName, 0));
                            else if (col.IsDateTime)
                                sb.Append(sep + DBHelpers.GetJSONValue(col.DbName, DateTime.Now.AddYears((DateTime.Now.Year - 1973) * -1).ToString("yyyy-MM-dd HH:mm:ss")));
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
