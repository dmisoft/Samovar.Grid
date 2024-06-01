namespace Samovar.Blazor.Filter;

public partial class GridFilterRow<TItem>
    : DesignComponentBase, IAsyncDisposable
{
    [SmInject]
    public required IColumnService ColumnService { get; set; }

    [SmInject]
    public required ILayoutService LayoutService { get; set; }

    [SmInject]
    public required IFilterService FilterService { get; set; }

    [SmInject]
    public required IRepositoryService<TItem> RepositoryService { get; set; }

    protected readonly List<Type> Numeric_Types_For_Constant_Expression = new List<Type>

            {
                typeof(byte),
                typeof(byte?),
                typeof(sbyte),
                typeof(sbyte?),
                typeof(short),
                typeof(short?),
                typeof(ushort),
                typeof(ushort?),
                typeof(int),
                typeof(int?),
                typeof(uint),
                typeof(uint?),
                typeof(long),
                typeof(long?),
                typeof(ulong),
                typeof(ulong?),
                typeof(float),
                typeof(float?),
                typeof(double),
                typeof(double?),
                typeof(decimal),
                typeof(decimal?),
        };

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
