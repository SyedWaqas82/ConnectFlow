using System.Threading.Tasks;
using Asp.Versioning;
using ConnectFlow.Domain.Events.Mediator.Users;
using k8s.KubeConfigModels;

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

        group.MapGet(TestRabbitMq, "TestRabbitMq", new ApiVersion(1, 0));
    }

    // Named handler method to avoid anonymous method issues
    private static string SayHello1() => "Hello from v1";
    private static string SayHello2() => "Hello from v2";

    private static async Task<string> TestRabbitMq(IMediator mediator)
    {
        await mediator.Publish(new UserCreatedEvent(0, Guid.NewGuid(), "<Email>", "Test User", "Test User", "Test User", "Test User", "Test User", "Test User", "Test User", true, "Test User"));

        return "RabbitMQ test endpoint";
    }
}