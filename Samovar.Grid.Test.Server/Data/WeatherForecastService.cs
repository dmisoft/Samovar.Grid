using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Samovar.Grid.Test.Server.Data
{
    public class WeatherForecastService
    {
        public Task<IEnumerable<WeatherForecast>> GetMockData() {
            return Task.FromResult(JsonConvert.DeserializeObject<IEnumerable<WeatherForecast>>(File.ReadAllText("Data\\data.json")));
        }
        
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public Task<IEnumerable<WeatherForecast>> GetForecastAsync(DateTime startDate)
        {
            var rng = new Random();
            int pos = 0;
            return Task.FromResult(Enumerable.Range(1, 750).Select(index => new WeatherForecast
            {
                Position = ++pos,
                Date = startDate.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                SummaryDbl = rng.Next(-20, 55) + 0.125,
                Summary = Summaries[rng.Next(Summaries.Length)],
                YesNo = true,
                SByteValue = (sbyte)rng.Next(-128, 127),
                ByteValue = (byte)rng.Next(0, 255),
                CharValue = 'c'
            }));
        }

        public Task<IEnumerable<WeatherForecast>> GetFiveForecastAsync(DateTime startDate)
        {
            var rng = new Random();
            int pos = 0;
            return Task.FromResult(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Position = ++pos,
                Date = startDate.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                SummaryDbl = rng.Next(-20, 55) + 0.125,
                Summary = Summaries[rng.Next(Summaries.Length)],
                YesNo = true,
                SByteValue = (sbyte)rng.Next(-128, 127),
                ByteValue = (byte)rng.Next(0, 255),
                CharValue = 'c'
            }));
        }

        public Task<IEnumerable<WeatherForecast>> Get333ForecastAsync(DateTime startDate)
        {
            var rng = new Random();
            int pos = 0;
            return Task.FromResult(Enumerable.Range(1, 333).Select(index => new WeatherForecast
            {
                Position = ++pos,
                Date = startDate.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                SummaryDbl = rng.Next(-20, 55) + 0.125,
                Summary = Summaries[rng.Next(Summaries.Length)],
                YesNo = true,
                SByteValue = (sbyte)rng.Next(-128, 127),
                ByteValue = (byte)rng.Next(0, 255),
                CharValue = 'c'
            }));
        }

        public Task<IEnumerable<WeatherForecast>> GetForecastAsync(DateTime startDate, int dataCnt)
        {
            Debug.WriteLine("new instance creating");
            var rng = new Random();

            var retVal = Enumerable.Range(1, dataCnt).Select(index => new WeatherForecast
            {
                Position = index,
                Date = startDate.AddDays(index),
                DateNullable = startDate.AddDays(index + 1),
                TemperatureC = rng.Next(-20, 55),
                SummaryDbl = rng.Next(-20, 55) + 0.125,
                SummaryDblNullable = rng.Next(-20, 55) + 0.125,
                Summary = Summaries[rng.Next(Summaries.Length)],
                YesNo = rng.Next(-20, 55) < 0 ? false : true,
                SByteValue = (sbyte)rng.Next(-128, 127),
                ByteValue = (byte)rng.Next(0, 255),
                CharValue = char.ConvertFromUtf32(rng.Next(0, 255)).ToCharArray()[0]
            });

            var json = JsonConvert.SerializeObject(retVal);
            return Task.FromResult(retVal);


            //List<WeatherForecast> retVal = new List<WeatherForecast>();
            //retVal.Add(new WeatherForecast
            //{
            //    Position = 1,
            //    Date = startDate.AddDays(1),
            //    DateNullable = startDate.AddDays(2),
            //    TemperatureC = rng.Next(-20, 55),
            //    SummaryDbl = rng.Next(-20, 55) + 0.125,
            //    SummaryDblNullable = rng.Next(-20, 55) + 0.125,
            //    Summary = Summaries[rng.Next(Summaries.Length)],
            //    YesNo = rng.Next(-20, 55) < 0 ? false : true,
            //    SByteValue = (sbyte)rng.Next(-128, 127),
            //    ByteValue = (byte)rng.Next(0, 255),
            //    CharValue = char.ConvertFromUtf32(rng.Next(0, 255)).ToCharArray()[0]
            //});

            //retVal.Add(new WeatherForecast
            //{
            //    Position = 2,
            //    Date = startDate.AddDays(2),
            //    DateNullable = startDate.AddDays(2),
            //    TemperatureC = rng.Next(-20, 55),
            //    SummaryDbl = rng.Next(-20, 55) + 0.125,
            //    SummaryDblNullable = rng.Next(-20, 55) + 0.125,
            //    Summary = Summaries[rng.Next(Summaries.Length)],
            //    YesNo = rng.Next(-20, 55) < 0 ? false : true,
            //    SByteValue = (sbyte)rng.Next(-128, 127),
            //    ByteValue = (byte)rng.Next(0, 255),
            //    CharValue = char.ConvertFromUtf32(rng.Next(0, 255)).ToCharArray()[0]
            //});

            //return Task.FromResult(retVal.AsEnumerable());
        }

        public Task<IEnumerable<WeatherForecastMinViewModel>> GetForecastAsyncMinViewModel(DateTime startDate, int dataCnt)
        {
            var rng = new Random();
            //int pos = 1;
            return Task.FromResult(Enumerable.Range(1, dataCnt).Select(index => new WeatherForecastMinViewModel
            {
                Position = index,
                Date = startDate.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                SummaryDbl = rng.Next(-20, 55) + 0.125,
                SummaryDblNullable = rng.Next(-20, 55) + 0.125,
                Summary = Summaries[rng.Next(Summaries.Length)],
                YesNo = rng.Next(-20, 55) < 0 ? false : true,
                SByteValue = (sbyte)rng.Next(-128, 127),
                ByteValue = (byte)rng.Next(0, 255),
                CharValue = char.ConvertFromUtf32(rng.Next(0, 255)).ToCharArray()[0]
            }));
        }
    }
}
