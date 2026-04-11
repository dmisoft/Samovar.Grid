using Samovar.Grid;
using Samovar.Grid.TestPages.Data;
using Samovar.Grid.WebAppAutoTest.Client;
using Samovar.Grid.WebAppAutoTest.Components;

namespace Samovar.Grid.WebAppAutoTest;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        builder.Services.AddGridTestPages();
        builder.Services.AddSamovarGrid(options => options.InjectCss = true);

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
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(
                typeof(_Imports).Assembly,
                typeof(Samovar.Grid.TestPages.Pages.Home).Assembly);

        app.Run();
    }
}
