﻿namespace ConnectFlow.Application.WeatherForecasts.Queries.GetWeatherForecasts;

public record WeatherForecast
{
    public DateTimeOffset Date { get; init; }

    public int TemperatureC { get; init; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; init; }
}
