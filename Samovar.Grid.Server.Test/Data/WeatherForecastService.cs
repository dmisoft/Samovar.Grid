
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Samovar.Grid.Server.Test.Data;
public class WeatherForecastService
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
    {
        return Task.FromResult(Enumerable.Range(1, 20).Select(index => new WeatherForecast
        {
            Position = index,
            Date = startDate.AddDays(index),
            Date2 = DateOnly.FromDateTime(startDate).AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        }).ToArray());
    }

    public Task<HashSet<WeatherForecast>> GetForecastHashSetAsync(DateTime startDate)
    {
        return Task.FromResult(Enumerable.Range(1, 1000000).Select(index => new WeatherForecast
        {
            Position = index,
            Date = startDate.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        }).ToHashSet());
    }
}
