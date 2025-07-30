using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.Builder;

namespace ConnectFlow.Web.Infrastructure;

public static class WebApplicationExtensions
{
    public static RouteGroupBuilder MapGroup(this WebApplication app, EndpointGroupBase group, ApiVersionSet? apiVersionSet = null)
    {
        var groupName = group.GetType().Name;

        if (apiVersionSet == null)
        {
            apiVersionSet = app.NewApiVersionSet().HasApiVersion(new ApiVersion(1, 0)).ReportApiVersions().Build();
        }

        // Create a versioned route group - use format that matches our Swagger configuration
        return app
            .MapGroup($"/api/v{{apiVersion:apiVersion}}/{groupName.ToLowerInvariant()}").WithTags(groupName)
            .WithApiVersionSet(apiVersionSet);
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var endpointGroupType = typeof(EndpointGroupBase);

        var assembly = Assembly.GetExecutingAssembly();

        var endpointGroupTypes = assembly.GetExportedTypes()
            .Where(t => t.IsSubclassOf(endpointGroupType));

        foreach (var type in endpointGroupTypes)
        {
            if (Activator.CreateInstance(type) is EndpointGroupBase instance)
            {
                instance.Map(app);
            }
        }

        return app;
    }
}