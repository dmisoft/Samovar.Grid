using Microsoft.Extensions.DependencyInjection;

namespace Samovar.Grid.TestPages.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGridTestPages(this IServiceCollection services)
    {
        services.AddSingleton<WeatherForecastService>();
        return services;
    }
}
