using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Samovar.Grid;
using Samovar.Grid.TestPages.Data;

namespace Samovar.Grid.WebAppAutoTest.Client;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.Services.AddGridTestPages();
        builder.Services.AddSamovarGrid();

        await builder.Build().RunAsync();
    }
}
