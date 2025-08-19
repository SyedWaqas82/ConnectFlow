using Asp.Versioning;
using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using ConnectFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ConnectFlow.Web.Endpoints;

public class WeatherForecasts : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var apiVersionSet = app.NewApiVersionSet()
                        .HasApiVersion(new ApiVersion(1, 0))
                        .HasApiVersion(new ApiVersion(2, 0))
                        .ReportApiVersions()
                        .Build();

        app.MapGroup(this, apiVersionSet)
            .AllowAnonymous()
            .MapGet(GetWeatherForecastsV1, apiVersion: new ApiVersion(1, 0))
            .MapGet(GetWeatherForecastsV2, apiVersion: new ApiVersion(2, 0))
            .MapPost(ResetDatabase, "ResetDatabase");
    }

    public async Task<Ok<IEnumerable<WeatherForecast>>> GetWeatherForecastsV1(ISender sender)
    {
        var forecasts = await sender.Send(new GetWeatherForecastsQuery());

        return TypedResults.Ok(forecasts);
    }

    public async Task<Ok<IEnumerable<WeatherForecast>>> GetWeatherForecastsV2(ISender sender)
    {
        var forecasts = await sender.Send(new GetWeatherForecastsQuery());

        return TypedResults.Ok(forecasts);
    }

    public async Task<Ok> ResetDatabase(IServiceProvider serviceProvider, ApplicationDbContext applicationDbContext)
    {
        // Remove all data from tables (order matters due to FK constraints)
        applicationDbContext.TenantUserRoles.RemoveRange(applicationDbContext.TenantUserRoles);
        applicationDbContext.TenantUsers.RemoveRange(applicationDbContext.TenantUsers);
        applicationDbContext.Subscriptions.RemoveRange(applicationDbContext.Subscriptions);
        applicationDbContext.Tenants.RemoveRange(applicationDbContext.Tenants);
        applicationDbContext.Users.RemoveRange(applicationDbContext.Users);
        //applicationDbContext.Roles.RemoveRange(applicationDbContext.Roles);
        applicationDbContext.UserRoles.RemoveRange(applicationDbContext.UserRoles);

        await applicationDbContext.SaveChangesAsync();

        // Get the database initialiser and initialise the database and data
        var initialiser = serviceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.SeedAsync();

        return TypedResults.Ok();
    }
}