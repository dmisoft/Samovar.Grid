using Microsoft.Extensions.DependencyInjection;

namespace Test.Pages.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGridTestPages(this IServiceCollection services)
    {
        services.AddSingleton<EmployeeService>();
        return services;
    }
}
