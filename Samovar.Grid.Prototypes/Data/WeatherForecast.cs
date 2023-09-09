using System;
using System.Threading;
using System.Threading.Tasks;

namespace Samovar.Grid.Prototypes.Data
{
    public class WeatherForecast
    {
        public int Position { get; set; }
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }

        public bool IsLoaded { get; set; }
        public string LongRunnigTask(CancellationToken token)
        {
            Thread.Sleep(2000);
            if (token.IsCancellationRequested)
            {
                return "WeatherForecast - canceled";
                //token.ThrowIfCancellationRequested();
            }
            else
            {
                IsLoaded = true;
                return Guid.NewGuid().ToString();
            }
            //return Task.Run(() =>
            //{


            //}, token);
        }

        public override string ToString()
        {
            return $"{Position.ToString()} {TemperatureC.ToString()} {Summary}";
        }
    }
}
