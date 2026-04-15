using Test.Pages.Data;
using Test.WebAppServer.Components;
using Samovar.Grid;

namespace Test.WebAppServer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorComponents(options =>
                options.DetailedErrors = builder.Environment.IsDevelopment())
            .AddInteractiveServerComponents();

        builder.Services.AddGridTestPages();
        builder.Services.AddSamovarGrid(options => options.InjectCss = true);

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseAntiforgery();
        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddAdditionalAssemblies(typeof(Test.Pages.Pages.Home).Assembly);

        app.Run();
    }
}
