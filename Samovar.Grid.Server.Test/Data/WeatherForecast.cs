
using System;

namespace Samovar.Grid.Server.Test.Data;
public class WeatherForecast
{
    public int Position { get; set; }
    public DateTime Date { get; set; }
    public DateOnly Date2 { get; set; }

    public int? TemperatureC { get; set; }
    public int? NullableInterger { get; set; } = null;

    public int TemperatureF => 32 + (int)(TemperatureC??0/ 0.5556);

    public string? Summary { get; set; }
}
