using ConnectFlow.Infrastructure.Data;
using ConnectFlow.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddKeyVaultIfConfigured();
builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();
builder.AddApiVersioning();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseExceptionHandler(options => { });
app.UseCustomHealthChecks();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseContextSettings();
app.UseCorrelationIdMapping();
app.UseRequestLogging(); // Add request/response logging
app.UseRateLimiter();
app.MapPrometheusScrapingEndpoint("/metrics");

// Automatically discover and map all EndpointGroupBase implementations before Swagger UI for proper API version discovery.
app.MapEndpoints();

// Configure Swagger and Swagger UI
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    var descriptions = app.DescribeApiVersions();

    // // Add a swagger endpoint for each discovered API version
    foreach (var description in descriptions)
    {
        var url = $"/swagger/{description.GroupName}/swagger.json";
        var apiName = $"ConnectFlow API {description.GroupName}";

        options.SwaggerEndpoint(url, apiName);
    }

    options.RoutePrefix = string.Empty; // Serve Swagger UI at the root URL

    //options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    //options.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
});

// Default route redirect to Swagger UI
//app.Map("/", () => Results.Redirect("/swagger"));

app.Run();

public partial class Program { }