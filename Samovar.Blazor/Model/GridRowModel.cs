using System.Diagnostics;
using System.Reflection;

namespace Samovar.Blazor;

public interface IComponentModel
{
	public string HtmlElementId { get; }
}

public class GridRowModel<T>
	: IGridRowModel<T>, IAsyncDisposable
{
    public bool RowDetailExpanded { get; set; }

	public string HtmlElementId { get; } = $"sdatagridrow{Guid.NewGuid().ToString().Replace("-", "")}";

	internal GridRowState RowState { get; set; } = GridRowState.Idle;

	public int DataItemPosition { get; set; }
	public int DataItemIndex { get => DataItemPosition - 1; }
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
		, bool detailExpanded

        )
		//, IGridSelectionService<T> gridSelectionService)
	{
        //get an service from the service provider
        //var gridSelectionService = serviceProvider.GetRequiredService<IGridSelectionService<T>>();
        // inject the service into the constructor
        //public GridRowModel(
        //    T dataItem
        //    , IEnumerable<IDataColumnModel> columnModel
        //    , int dataItemPosition
        //    , Dictionary<string, PropertyInfo> propDict
        //    , bool detailExpanded
        //    , IGridSelectionService<T> gridSelectionService)
        //{

        // can be inject an service in my class before custom constructor?


        //services.AddSingleton<IGridSelectionService<T>, GridSelectionService<T>>();



        DataItemPosition = dataItemPosition;
		DataItem = dataItem;
		ColumnMetadata = columnModel;
		PropDict = propDict;
		RowDetailExpanded = detailExpanded;
		GridCellModels = CreateGridRowCellModelCollection2(dataItem);
		//gridSelectionService.MultipleSelectedDataRows.Subscribe(selectedItems =>
  //      {
  //          IsRowSelected = selectedItems is not null && selectedItems.Any(a => a!.Equals(DataItem));
            
  //      });		
		//gridSelectionService.SingleSelectedDataRow.Subscribe(selectedItem =>
  //      {
  //          IsRowSelected = selectedItem is not null && selectedItem.Equals(DataItem);
  //      });
    }

	internal List<DataGridRowCellModel<T>> CreateGridRowCellModelCollection(IEnumerable<IDataColumnModel> columnMetadata, T dataItem)//, CancellationToken token)
	{
		List<DataGridRowCellModel<T>> gridCellModelCollection = new List<DataGridRowCellModel<T>>();

		foreach (var cm in columnMetadata.Where(c => c.ColumnType == ColumnType.Data))
		{
			gridCellModelCollection.Add(new DataGridRowCellModel<T>(dataItem, PropDict[cm.Field.Value], cm));
		}
		return gridCellModelCollection;
	}

	internal List<DataGridRowCellModel<T>> CreateGridRowCellModelCollection2(T dataItem)
	{
		Stopwatch stopWatch = new Stopwatch();
		stopWatch.Start();

		List<DataGridRowCellModel<T>> gridCellModelCollection = new List<DataGridRowCellModel<T>>();

		foreach (var cm in ColumnMetadata)
		{
			gridCellModelCollection.Add(new DataGridRowCellModel<T>(dataItem, PropDict[cm.Field.Value], cm));
		}

		stopWatch.Stop();

		return gridCellModelCollection;
	}

	internal void CreateEditingModel()
	{
		EditingDataItem = CloneRowItem(DataItem);
		EditingGridCellModels = CreateGridRowCellModelCollection(ColumnMetadata, EditingDataItem);
	}
	internal void CommitEditingModel()
	{
		if (EditingDataItem is null)
			return;
		DataItem = CopyRowModelData(EditingDataItem, DataItem);
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
