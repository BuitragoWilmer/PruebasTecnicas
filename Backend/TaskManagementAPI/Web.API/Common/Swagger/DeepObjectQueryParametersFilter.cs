using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Web.API.Common.Swagger
{
    public class DeepObjectQueryParametersFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                return;

            foreach (var parameter in operation.Parameters)
            {
                if (parameter.Name == "filters")
                {
                    // Estilo deepObject para que pueda desglosar filters[key]=value
                    parameter.Style = ParameterStyle.DeepObject;
                    parameter.Explode = true;

                    parameter.Schema ??= new OpenApiSchema();
                    parameter.Schema.Type = "object";
                    parameter.Schema.AdditionalProperties = new OpenApiSchema { Type = "string" };

                    // Descripción orientativa
                    parameter.Description ??= "Filtros dinámicos en formato filters[key]=value";
                }
            }
        }
    }
}