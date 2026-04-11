using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Samovar.Grid.Test.Data;

public class WeatherForecast
{
    [DisplayName("Pos.")]
    public int Position { get; set; }
    public DateTime Date { get; set; }
    
    [Browsable(false)]
    public DateOnly DateOnly { get; set; }

    [Display(Name ="Temp.C")]
    public int? TemperatureC { get; set; }
    
    [Browsable(false)]
    public int? NullableInterger { get; set; } = null;

    [Browsable(false)]
    public int TemperatureF => 32 + (int)(TemperatureC ?? 0 / 0.5556);
    
    [Browsable(false)]
    public double TemperatureFD => 12.5;
    
    public bool Active { get; set; }

    public string Summary { get; set; } = string.Empty;

    public WeatherForecastDetail Detail { get; set; } = new WeatherForecastDetail { SomeDetails = "Hello from details" };
}

public class WeatherForecastDetail
{
    public string SomeDetails { get; set; } = string.Empty;
}
