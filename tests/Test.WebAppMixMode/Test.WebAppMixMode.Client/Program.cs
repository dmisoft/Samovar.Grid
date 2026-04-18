using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Samovar.Grid;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddSamovarGrid(options => options.InjectCss = true);

var defaultCulture = new CultureInfo("fr-FR");
CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

await builder.Build().RunAsync();
