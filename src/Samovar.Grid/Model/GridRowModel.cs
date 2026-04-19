using Samovar.Grid.Formulas;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.Reflection;

namespace Samovar.Grid;

public interface IComponentModel
{
    public string HtmlElementId { get; }
}

public class GridRowModel<T>
    : IGridRowModel<T>, IAsyncDisposable
{
    public bool RowDetailExpanded { get; set; }

    public string HtmlElementId { get; } = $"sdatagridrow{Guid.NewGuid().ToString().Replace("-", "")}";

    internal BehaviorSubject<GridRowState> RowState { get; set; } = new BehaviorSubject<GridRowState>(GridRowState.Idle);

    public int DataItemPosition { get; set; }
    public bool IsRowSelected { get; set; }
    public T DataItem { get; set; }
    public T? EditingDataItem { get; set; }
    public List<DataGridRowCellModel<T>> GridCellModels { get; set; }
    public List<DataGridRowCellModel<T>> EditingGridCellModels { get; set; } = [];

    public readonly IEnumerable<IDataColumnModel> ColumnMetadata;

    private readonly Dictionary<string, PropertyInfo> PropDict;


    public GridRowModel(
        T dataItem
        , IEnumerable<IDataColumnModel> columnModel
        , int dataItemPosition
        , Dictionary<string, PropertyInfo> propDict
        , bool detailExpanded)
    {
        DataItemPosition = dataItemPosition;
        DataItem = dataItem;
        ColumnMetadata = columnModel;
        PropDict = propDict;
        RowDetailExpanded = detailExpanded;
        GridCellModels = CreateGridRowCellModelCollection2(dataItem);
    }

    internal List<DataGridRowCellModel<T>> CreateGridRowCellModelCollection(IEnumerable<IDataColumnModel> columnMetadata, T dataItem)//, CancellationToken token)
    {
        List<DataGridRowCellModel<T>> gridCellModelCollection = [];

        foreach (var cm in columnMetadata.Where(c => c.ColumnType == ColumnType.Data))
        {
            gridCellModelCollection.Add(BuildCellModel(dataItem, cm));
        }
        return gridCellModelCollection;
    }

    internal List<DataGridRowCellModel<T>> CreateGridRowCellModelCollection2(T dataItem)
    {
        List<DataGridRowCellModel<T>> gridCellModelCollection = [];

        foreach (var cm in ColumnMetadata)
        {
            gridCellModelCollection.Add(BuildCellModel(dataItem, cm));
        }

        return gridCellModelCollection;
    }

    private DataGridRowCellModel<T> BuildCellModel(T dataItem, IDataColumnModel cm)
    {
        if (cm is IExpressionColumnModel expr)
        {
            var getter = (Func<T, decimal?>?)expr.CompiledGetter;
            if (getter is null)
            {
                getter = expr.Ast is null
                    ? _ => null
                    : FormulaCompiler.Compile<T>(expr.Ast, PropDict);
                expr.CompiledGetter = getter;
            }
            var capturedGetter = getter;
            return new DataGridRowCellModel<T>(dataItem, r => r is null ? null : capturedGetter(r), cm);
        }

        return new DataGridRowCellModel<T>(dataItem, PropDict[cm.Field.Value], cm);
    }

    internal void CreateEditingModel()
    {
        EditingDataItem = CloneRowItem(DataItem);
        EditingGridCellModels = CreateGridRowCellModelCollection(ColumnMetadata, EditingDataItem);
    }
    internal void EditCommit()
    {
        if (EditingDataItem is null)
            return;
        DataItem = CopyRowModelData(EditingDataItem, DataItem);
        GridCellModels = CreateGridRowCellModelCollection2(DataItem);
    }

    internal void CommitCustomEdit()
    {
        GridCellModels = CreateGridRowCellModelCollection2(DataItem);
    }

    public static T CloneRowItem(T sourceData)
    {
        T? retVal = (T?)Activator.CreateInstance(typeof(T));
        if (retVal is null)
            throw new InvalidOperationException("Failed to create instance of type T");

        foreach (PropertyInfo pi in sourceData!.GetType().GetProperties())
        {
            if (pi.CanWrite)
                pi.SetValue(retVal, pi.GetValue(sourceData));
        }
        return retVal;
    }

    public static T CopyRowModelData(T sourceData, T targetData)
    {
        foreach (PropertyInfo pi in sourceData!.GetType().GetProperties())
        {
            if (pi.CanWrite)
                pi.SetValue(targetData, pi.GetValue(sourceData));
        }
        return targetData;
    }

    public ValueTask DisposeAsync()
    {
        GridCellModels?.Clear();
        EditingGridCellModels?.Clear();
        return ValueTask.CompletedTask;
    }
}
