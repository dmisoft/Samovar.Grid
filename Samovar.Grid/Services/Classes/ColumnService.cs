using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reactive.Subjects;
using System.Reflection;

namespace Samovar.Grid;

public class ColumnService
    : IColumnService
{
    public IColumnModel EmptyColumnModel { get; } = new EmptyColumnModel();
    public IDeclarativeColumnModel DetailExpanderColumnModel { get; } = new DetailExpanderColumnModel();
    public IDeclarativeColumnModel RowSelectionColumnModel { get; } = new SelectionColumnModel();
    public List<IColumnModel> AllColumnModels { get; } = new List<IColumnModel>();
    public IEnumerable<IDataColumnModel> DataColumnModels => AllColumnModels.OfType<IDataColumnModel>();
    public Subject<IColumnModel> ColumnResizingEndedObservable { get; } = new();
    public IEnumerable<IDeclarativeColumnModel> DeclarativeColumnModels => AllColumnModels.OfType<IDeclarativeColumnModel>();

    public void RegisterColumn(IColumnModel columntModel)
    {
        int _columnOrder = AllColumnModels.Count + 1;
        columntModel.Order = _columnOrder;
        AllColumnModels.Add(columntModel);
    }

    public void AutoGenerateColumns<T>()
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead)
            .Where(p => IsSimpleType(p.PropertyType))
            .Where(p => p.GetCustomAttribute<BrowsableAttribute>()?.Browsable != false);

        foreach (var prop in properties)
        {
            var model = new DataColumnModel();
            model.Field.OnNext(prop.Name);
            model.Title.OnNext(GetDisplayName(prop));
            RegisterColumn(model);
        }
    }

    private static string GetDisplayName(PropertyInfo prop)
    {
        var displayNameAttr = prop.GetCustomAttribute<DisplayNameAttribute>();
        if (displayNameAttr is not null)
            return displayNameAttr.DisplayName;

        var displayAttr = prop.GetCustomAttribute<DisplayAttribute>();
        if (displayAttr?.Name is not null)
            return displayAttr.Name;

        return prop.Name;
    }

    private static bool IsSimpleType(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type) ?? type;
        return underlying.IsPrimitive
            || underlying.IsEnum
            || underlying == typeof(string)
            || underlying == typeof(decimal)
            || underlying == typeof(Half)
            || underlying == typeof(Int128)
            || underlying == typeof(UInt128)
            || underlying == typeof(DateTime)
            || underlying == typeof(DateOnly)
            || underlying == typeof(TimeOnly)
            || underlying == typeof(TimeSpan)
            || underlying == typeof(DateTimeOffset)
            || underlying == typeof(Guid);
    }
}
