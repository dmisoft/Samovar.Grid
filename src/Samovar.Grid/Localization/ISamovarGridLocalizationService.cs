using System.Globalization;
using System.Reactive;

namespace Samovar.Grid;

public interface ISamovarGridLocalizationService
{
    IObservable<Unit> CultureChanged { get; }

    string this[string key] { get; }

    string Get(string key, params object?[] args);

    void NotifyCultureChanged();

    void SetCulture(CultureInfo culture);
}
