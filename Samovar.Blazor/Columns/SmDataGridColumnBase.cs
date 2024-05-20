namespace Samovar.Blazor;
public class SmDataGridColumnBase<T>
    : SmDesignComponentBase
{
    [SmInject]
    public required IColumnService ColumnService { get; set; }

    [SmInject]
    public required IModelFactoryService ModelFactoryService { get; set; }

    [SmInject]
    public required T Model { get; set; }
}
