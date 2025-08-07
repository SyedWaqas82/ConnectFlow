using Asp.Versioning;
using ConnectFlow.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using Microsoft.AspNetCore.Http.HttpResults;

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
            .MapGet(GetWeatherForecastsV2, apiVersion: new ApiVersion(2, 0));
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
}