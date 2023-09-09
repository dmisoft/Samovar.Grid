using System;
using System.Linq;
using System.Threading.Tasks;

namespace Samovar.Grid.NugetTest.Server.Data
{
    public class WeatherForecastService
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
        {
            var rng = new Random();
            return Task.FromResult(Enumerable.Range(1, 5000).Select(index => new WeatherForecast
            {
                Date = startDate.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                SummaryDbl = rng.Next(-20, 55) + 0.245,
                Summary = Summaries[rng.Next(Summaries.Length)]
            }).ToArray());
        }
    }
}
