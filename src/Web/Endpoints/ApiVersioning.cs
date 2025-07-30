using Asp.Versioning;

namespace ConnectFlow.Web.Endpoints;

public class ApiVersioning : EndpointGroupBase
{
    // Override to explicitly set API version

    public override void Map(WebApplication app)
    {
        var apiVersionSet = app.NewApiVersionSet()
                        .HasApiVersion(new ApiVersion(1, 0))
                        .HasApiVersion(new ApiVersion(2, 0))
                        .ReportApiVersions()
                        .Build();

        // Map the group with versioning
        var group = app.MapGroup(this, apiVersionSet);

        // Map test endpoints with versioning to verify Swagger generation
        // Use a named method instead of a lambda to avoid Guard.Against.AnonymousMethod error
        group.MapGet(SayHello1, "sayhello", new ApiVersion(1, 0));
        group.MapGet(SayHello2, "sayhello", new ApiVersion(2, 0));
    }

    // Named handler method to avoid anonymous method issues
    private static string SayHello1() => "Hello from v1";
    private static string SayHello2() => "Hello from v2";
}