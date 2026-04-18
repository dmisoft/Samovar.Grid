using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Reactive;
using System.Reflection;

namespace Samovar.Grid;

public class SmGridBase<T>
    : ComponentBase, IComponentServiceProvider, IDisposable
{
    [Inject]
    public required IJSRuntime JsRuntime { get; set; } = default!;

    [Inject]
    public required ISamovarGridLocalizationService L10n { get; set; } = default!;

    [SmInject]
    public required IJsService JsService { get; set; } = default!;

    public SmComponentServiceProvider ServiceProvider { get; } = new();

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private IDisposable? _cultureChangedSubscription;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        await InitializeDependencies();
        await base.SetParametersAsync(parameters);
    }

    private bool _dependenciesInitialized;

    private Task InitializeDependencies()
    {
        if (!_dependenciesInitialized)
        {
            _dependenciesInitialized = true;

            ServiceProvider.InitServices<T>(L10n);

#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
            var propertyInfos = GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var propertiesWithSmInjectAttribute = propertyInfos.Where(prop =>
                Attribute.IsDefined(prop, typeof(SmInjectAttribute)));
#pragma warning restore S3011

            propertiesWithSmInjectAttribute.ToList().ForEach(property =>
            {
                object service = ServiceProvider.GetService(property.PropertyType);
                property.SetValue(this, service);
            });

            _cultureChangedSubscription = L10n.CultureChanged.Subscribe(OnCultureChanged);
        }

        return Task.CompletedTask;
    }

    private void OnCultureChanged(Unit unit) => InvokeAsync(StateHasChanged);

    public void Dispose()
    {
        _cultureChangedSubscription?.Dispose();
        _cultureChangedSubscription = null;
        GC.SuppressFinalize(this);
    }
}
