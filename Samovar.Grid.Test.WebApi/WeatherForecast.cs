using Swashbuckle.AspNetCore.Filters;
using System;

namespace Samovar.Grid.Test.WebApi
{
    public class WeatherForecast
    {
        /// <example>2023-07-11T00:00:00</example>
        public DateTime Date { get; set; }

        /// <example>100</example>
        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }
    }

    public class WeatherForecastExample : IExamplesProvider<WeatherForecast>
    {
        public WeatherForecast GetExamples()
        {
            return new WeatherForecast
            {
                Date = DateTime.Now,
                TemperatureC = 100,
                Summary = $"Example text {Guid.NewGuid().ToString()}"
            };
        }
    }
}
