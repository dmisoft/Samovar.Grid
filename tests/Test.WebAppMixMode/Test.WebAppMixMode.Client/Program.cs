using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Samovar.Grid;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddSamovarGrid(options => options.InjectCss = true);

await builder.Build().RunAsync();
