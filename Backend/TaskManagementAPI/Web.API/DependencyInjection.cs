using Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Web.API.Common.Errors;
using Web.API.Common.Hateoas;
using Web.API.Common.Swagger;
using Web.API.Middlewares;

namespace Web.API;
public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.CustomSchemaIds(type => type.ToString());

            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "API administracion de tareas",
                Version = "v1",
                Description = "API para la gestión de tareas"
            });

            c.DescribeAllParametersInCamelCase();

            c.MapType<Dictionary<string, string>>(() => new OpenApiSchema
            {
                Type = "object",
                AdditionalPropertiesAllowed = true,
                AdditionalProperties = new OpenApiSchema { Type = "string" },
                Description = "Filtros dinámicos (use filters[key]=value en la query)",
                Example = new OpenApiObject
                {
                    ["name"] = new OpenApiString("example")
                }
            });

            c.OperationFilter<DeepObjectQueryParametersFilter>();
        });

        services.AddTransient<GloblalExceptionHandlingMiddleware>();
        services.AddScoped<HateoasService>();
        services.AddHttpContextAccessor();
        services.AddSingleton<ProblemDetailsFactory, FormsApiProblemDetailsFactory>();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new QueryStringApiVersionReader("api-version"),  // ?api-version=1.0
                new HeaderApiVersionReader("X-Version"),          // Header: X-Version: 1.0
                new MediaTypeApiVersionReader("ver")              // Accept: application/json;ver=1.0
            );
        });
        return services;
    }
}