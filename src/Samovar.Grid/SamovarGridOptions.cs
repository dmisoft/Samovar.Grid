using System.Globalization;

namespace Samovar.Grid;

/// <summary>
/// Configuration options for the SamovarGrid component library.
/// </summary>
public sealed class SamovarGridOptions
{
    /// <summary>
    /// When <see langword="true"/>, the <see cref="SamovarGridStyles"/> component will render
    /// the library's CSS link tag into the document head.
    /// Defaults to <see langword="false"/> to preserve backward compatibility with
    /// apps that link the stylesheet manually in App.razor.
    /// </summary>
    public bool InjectCss { get; set; } = false;

    internal Dictionary<CultureInfo, Dictionary<string, string>> CustomLanguages { get; } = [];

    /// <summary>
    /// Registers a consumer-supplied set of translations for a culture. Entries in this dictionary
    /// take precedence over the bundled satellite resources for the same culture/key; other keys
    /// still fall back to the satellite or the neutral (English) resource.
    /// Use this to add a language the grid does not ship, or to replace a subset of a shipped
    /// language wholesale.
    /// </summary>
    public SamovarGridOptions AddLanguage(CultureInfo culture, IReadOnlyDictionary<string, string> strings)
    {
        ArgumentNullException.ThrowIfNull(culture);
        ArgumentNullException.ThrowIfNull(strings);
        if (!CustomLanguages.TryGetValue(culture, out var table))
            CustomLanguages[culture] = table = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var kvp in strings)
            table[kvp.Key] = kvp.Value;
        return this;
    }

    /// <summary>
    /// Overrides a single localized string for the given culture. Takes precedence over the bundled
    /// satellite resource for that culture/key.
    /// </summary>
    public SamovarGridOptions OverrideString(CultureInfo culture, string key, string value)
    {
        ArgumentNullException.ThrowIfNull(culture);
        ArgumentException.ThrowIfNullOrEmpty(key);
        ArgumentNullException.ThrowIfNull(value);
        if (!CustomLanguages.TryGetValue(culture, out var table))
            CustomLanguages[culture] = table = new Dictionary<string, string>(StringComparer.Ordinal);
        table[key] = value;
        return this;
    }
}
