using Microsoft.Extensions.DependencyInjection;

namespace Samovar.Blazor
{
    public static class SmDataGridDIExtension
    {
        public static void AddSmDataGrid(this IServiceCollection services)
        {
            services.AddScoped<IExampleJsInterop, ExampleJsInterop>();
        }
    }
}
