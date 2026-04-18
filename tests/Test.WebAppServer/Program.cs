using Test.Pages.Data;
using Test.WebAppServer.Components;
using Samovar.Grid;
using System.Globalization;

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

        //builder.Services.AddLocalization();

        var app = builder.Build();

        //Localization
        var uiCultures = new[] { "en-US", "de-DE" };

        var formattingCultures = CultureInfo
            .GetCultures(CultureTypes.SpecificCultures)
            .Select(c => c.Name)
            .ToArray();

        var localizationOptions = new RequestLocalizationOptions()
            .SetDefaultCulture(uiCultures[0])
            .AddSupportedCultures(formattingCultures)
            .AddSupportedUICultures(uiCultures);
        localizationOptions.RequestCultureProviders.Clear();

        app.UseRequestLocalization(localizationOptions);

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
