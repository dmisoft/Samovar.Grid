using Microsoft.Extensions.DependencyInjection;

namespace Samovar.Grid;

/// <summary>
/// Extension methods for registering SamovarGrid services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers SamovarGrid configuration with the application's service collection.
    /// </summary>
    /// <remarks>
    /// SamovarGrid manages its own internal DI container per grid instance and does not
    /// require any services to be registered in the application container. This method
    /// registers configuration options used by <see cref="SamovarGridStyles"/> and
    /// validated by <see cref="ApplicationBuilderExtensions.UseSamovarGrid"/>.
    /// </remarks>
    public static IServiceCollection AddSamovarGrid(this IServiceCollection services)
        => services.AddSamovarGrid(_ => { });

    /// <summary>
    /// Registers SamovarGrid configuration with the application's service collection
    /// and applies the provided options.
    /// </summary>
    public static IServiceCollection AddSamovarGrid(
        this IServiceCollection services,
        Action<SamovarGridOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var options = new SamovarGridOptions();
        configure(options);
        services.AddSingleton(options);

        return services;
    }
}
