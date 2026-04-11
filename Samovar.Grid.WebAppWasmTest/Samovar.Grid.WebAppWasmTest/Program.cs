using Samovar.Grid;
using Samovar.Grid.TestPages.Data;
using Samovar.Grid.WebAppWasmTest.Client;
using Samovar.Grid.WebAppWasmTest.Components;

namespace Samovar.Grid.WebAppWasmTest;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorComponents()
            .AddInteractiveWebAssemblyComponents();

        builder.Services.AddGridTestPages();
        builder.Services.AddSamovarGrid(o => o.InjectCss = true);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseAntiforgery();
        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(
                typeof(_Imports).Assembly,
                typeof(Samovar.Grid.TestPages.Pages.Home).Assembly);

        app.Run();
    }
}
