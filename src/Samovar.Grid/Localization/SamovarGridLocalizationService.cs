using System.Globalization;
using System.Reactive;
using System.Reactive.Subjects;
using Microsoft.Extensions.Localization;

namespace Samovar.Grid;

internal sealed class SamovarGridLocalizationService(
    IStringLocalizer<SamovarGridStrings> localizer,
    SamovarGridOptions options) : ISamovarGridLocalizationService
{
    private readonly IReadOnlyDictionary<CultureInfo, IReadOnlyDictionary<string, string>> _customLanguages = options.CustomLanguages
        .ToDictionary(
            kvp => kvp.Key,
            kvp => (IReadOnlyDictionary<string, string>)new Dictionary<string, string>(kvp.Value));
    private readonly Subject<Unit> _cultureChanged = new();

    public IObservable<Unit> CultureChanged => _cultureChanged;

    public string this[string key] => Lookup(key);

    public string Get(string key, params object?[] args)
    {
        var template = Lookup(key);
        return args is { Length: > 0 }
            ? string.Format(CultureInfo.CurrentCulture, template, args)
            : template;
    }

    public void NotifyCultureChanged() => _cultureChanged.OnNext(Unit.Default);

    public void SetCulture(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        NotifyCultureChanged();
    }

    private string Lookup(string key)
    {
        var culture = CultureInfo.CurrentUICulture;

        // 1) Consumer overrides — exact, then neutral language
        if (TryGetFromCustom(culture, key, out var overridden))
            return overridden;

        if (!culture.IsNeutralCulture)
        {
            var neutral = culture.Parent;
            if (!neutral.Equals(CultureInfo.InvariantCulture)
                && TryGetFromCustom(neutral, key, out var neutralOverride))
                return neutralOverride;
        }

        // 2) Built-in satellite resources via IStringLocalizer
        var localized = localizer[key];
        if (!localized.ResourceNotFound)
            return localized.Value;

        // 3) Last resort: the key itself
        return key;
    }

    private bool TryGetFromCustom(CultureInfo culture, string key, out string value)
    {
        if (_customLanguages.TryGetValue(culture, out var table)
            && table.TryGetValue(key, out var v))
        {
            value = v;
            return true;
        }
        value = string.Empty;
        return false;
    }
}
