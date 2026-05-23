namespace Samovar.Grid;

public abstract class GridViewRow<T> { }

public sealed class GridDataViewRow<T> : GridViewRow<T>
{
    public required GridRowModel<T> RowModel { get; init; }
}

public sealed class GridGroupViewRow<T> : GridViewRow<T>
{
    public required GridGroupRowModel<T> GroupModel { get; init; }
}
