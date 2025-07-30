using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ConnectFlow.Web.Common;

public class ConfigureSwaggerGenOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(string? name, SwaggerGenOptions options)
    {
        // Clear any existing Swagger documents
        options.SwaggerGeneratorOptions.SwaggerDocs.Clear();

        // Add a SwaggerDoc for each discovered API version
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            var info = new OpenApiInfo
            {
                Title = "ConnectFlow API",
                Version = description.ApiVersion.ToString(),
                Description = $"ConnectFlow API {description.GroupName}",
            };

            options.SwaggerDoc(description.GroupName, info);
        }

        // Add JWT Bearer token authentication support
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

        //     // Ensure proper operation ID generation
        //     options.CustomOperationIds(apiDesc =>
        //     {
        //         var controllerAction = apiDesc.ActionDescriptor.RouteValues;
        //         return controllerAction is not null ?
        //             $"{controllerAction["controller"]}_{controllerAction["action"]}_{apiDesc.HttpMethod}" :
        //             $"operation_{apiDesc.HttpMethod}";
        //     });

        //     // Configure proper versioning display
        //     options.DocInclusionPredicate((docName, apiDesc) =>
        //     {
        //         if (!apiDesc.TryGetMethodInfo(out var methodInfo) || methodInfo.DeclaringType is null)
        //             return false;

        //         var versions = methodInfo.DeclaringType
        //             .GetCustomAttributes(true)
        //             .OfType<ApiVersionAttribute>()
        //             .SelectMany(attr => attr.Versions)
        //             .ToList();

        //         return versions.Any(v => $"v{v.ToString()}" == docName);
        //     });
    }

    public void Configure(SwaggerGenOptions options) => Configure(null, options);
}