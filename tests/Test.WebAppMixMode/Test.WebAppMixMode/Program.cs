using Test.WebAppMixMode.Client.Pages;
using Test.WebAppMixMode.Components;
using Samovar.Grid;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddSamovarGrid(options => options.InjectCss = true);


var app = builder.Build();

//Localization
var uiCultures = new[] { "en-US", "fr-FR" };

var formattingCultures = CultureInfo
    .GetCultures(CultureTypes.SpecificCultures)
    .Select(c => c.Name)
    .ToArray();

var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(uiCultures[1])
    .AddSupportedCultures(formattingCultures)
    .AddSupportedUICultures(uiCultures);
localizationOptions.RequestCultureProviders.Clear();

app.UseRequestLocalization(localizationOptions);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Test.WebAppMixMode.Client._Imports).Assembly);

app.Run();
