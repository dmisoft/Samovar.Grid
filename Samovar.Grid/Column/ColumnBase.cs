namespace Samovar.Grid;
public class ColumnBase<T>
    : DesignComponentBase
{
    [SmInject]
    public required IColumnService ColumnService { get; set; }

    [SmInject]
    public required IModelFactoryService ModelFactoryService { get; set; }

    [SmInject]
    public required T Model { get; set; }
}
