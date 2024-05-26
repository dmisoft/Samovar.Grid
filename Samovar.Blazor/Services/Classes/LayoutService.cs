using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Samovar.Blazor.Columns;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Samovar.Blazor
{
	public class LayoutService
		: ILayoutService, IAsyncDisposable
	{
		public BehaviorSubject<ColumnResizeMode> ColumnResizeMode { get; } = new BehaviorSubject<ColumnResizeMode>(Columns.ColumnResizeMode.None);

		public BehaviorSubject<string> SelectedRowClass { get; } = new BehaviorSubject<string>("bg-warning");

		public BehaviorSubject<string> TableTagClass { get; } = new BehaviorSubject<string>("table table-bordered");

		public BehaviorSubject<string> TheadTagClass { get; } = new BehaviorSubject<string>("table-light");

		public BehaviorSubject<double> MinGridWidth { get; } = new BehaviorSubject<double>(0d);

		public BehaviorSubject<bool> ShowDetailRow { get; } = new BehaviorSubject<bool>(false);

		public BehaviorSubject<string> PaginationClass { get; } = new BehaviorSubject<string>("pagination");
		public BehaviorSubject<string> FilterToggleButtonClass { get; } = new BehaviorSubject<string>("btn btn-secondary");

		public BehaviorSubject<bool> ShowFilterRow { get; } = new BehaviorSubject<bool>(false);

		public BehaviorSubject<DataGridFilterMode> FilterMode { get; } = new BehaviorSubject<DataGridFilterMode>(DataGridFilterMode.None);

		public ElementReference GridFilterRef { get; set; }
		public ElementReference GridOuterRef { get; set; }
		public ElementReference GridInnerRef { get; set; }
		public ElementReference TableBodyInnerRef { get; set; }

		public double ActualColumnsWidthSum
		{
			get
			{
				return _columnService.AllColumnModels.Sum(c => c.VisibleAbsoluteWidthValue) +
						(ShowDetailRow.Value ? _columnService.DetailExpanderColumnModel.VisibleAbsoluteWidthValue : 0d);
			}
		}

		private readonly IConstantService _constantService;
		private readonly IJsService _jsService;
		private readonly IColumnService _columnService;

		public DotNetObjectReference<ILayoutService> DataGridDotNetRef { get; }

		private readonly Lazy<Task<double>> TableRowHeightLazy;

		public LayoutService(
			  IConstantService constantService
			, IJsService jsService
			, IInitService initService
			, IColumnService columnService)
		{
			_constantService = constantService;
			_jsService = jsService;
			_columnService = columnService;

			initService.IsInitialized.Subscribe(DataGridInitializerCallback);

			DataGridDotNetRef = DotNetObjectReference.Create(this as ILayoutService);
			DataGridInnerStyle = Observable.CombineLatest(Width, Height)
				.Select(_ =>
				{
					string outerStyle = $"height:{_[1]};";
					string footerStyle = "";
					if (!string.IsNullOrEmpty(_[0]))
					{
						outerStyle += $"width:{_[0]};";
						footerStyle = $"width:{_[0]};";
					}

					return Task.FromResult(new DataGridStyleInfo { CssStyle = outerStyle });
				}

				);
			//TableRowHeightLazy = new(() => _jsService.MeasureTableRowHeight(TableTagClass.Value).AsTask());
		}

		private void DataGridInitializerCallback(bool obj)
		{
			Task.Run(async () => await HeightWidthChanged(height: Height.Value, width: Width.Value));
		}

		public event Func<DataGridStyleInfo, Task>? DataGridInnerCssStyleChanged = null;

		public double FilterRowHeight { get; private set; }

		public Task<double> TableRowHeight()
		{
			return TableRowHeightLazy.Value;
		}

		//public double ActualScrollbarWidth { get; set; }

		public BehaviorSubject<string> OuterStyle { get; } = new BehaviorSubject<string>("");

		public BehaviorSubject<string> FooterStyle { get; } = new BehaviorSubject<string>("");

		public BehaviorSubject<string> Height { get; } = new BehaviorSubject<string>("400px");

		public BehaviorSubject<string> Width { get; } = new BehaviorSubject<string>("");

		public BehaviorSubject<bool> ShowColumnHeader { get; } = new BehaviorSubject<bool>(true);

		public BehaviorSubject<bool> ShowDetailHeader => throw new NotImplementedException();

		public IObservable<Task<DataGridStyleInfo>> DataGridInnerStyle { get; }
		public bool OriginalColumnsWidthChanged { get; set; }

		private async Task HeightWidthChanged(string height, string width)
		{
			string outerStyle = $"height:{height};";
			string footerStyle = "";
			if (!string.IsNullOrEmpty(width))
			{
				outerStyle += $"width:{width};";
				footerStyle = $"width:{width};";
			}

			OuterStyle.OnNext(outerStyle);
			FooterStyle.OnNext(footerStyle);
			await OnDataGridInnerCssStyleChanged();
		}

		[JSInvokable]
		public async Task JS_AfterWindowResize()
		{
			var tBodyWidth = await GridOuterRef.GetElementWidthByRef(await _jsService.JsModule());
			//CalculateEmptyColumn(tBodyWidth);
		}

		public async Task InitHeader()
		{
			if (OriginalColumnsWidthChanged)
				return;

			await GridInnerRef.SynchronizeGridHeaderScroll(await _jsService.JsModule(), _constantService.GridHeaderContainerId);
			if (FilterMode.Value == DataGridFilterMode.FilterRow)
			{
				await GridInnerRef.SynchronizeGridHeaderScroll(await _jsService.JsModule(), _constantService.GridFilterContainerId);
			}

			FilterRowHeight = await _jsService.MeasureTableFilterHeight(TableTagClass.Value, TheadTagClass.Value, FilterToggleButtonClass.Value);

			//GridColWidthSum = 0;
			//MinGridWidth.OnNext(0);

			//var allColumnsCount = _columnService.AllColumnModels.Count + (ShowDetailRow.Value ? 1 : 0);

			//var columnsCountWithAbsoluteWidth = _columnService.AllColumnModels.Count(c => c.WidthInfo.DeclaratedWidthMode == ColumnMetadataWidthInfo.DeclaratedColumnWidthMode.Absolute) + (ShowDetailRow.Value ? 1 : 0);

			//if (ColumnResizeMode.Value == Columns.ColumnResizeMode.Block)
			//{
			//    var widthSumOfAllAbsoluteWidth = _columnService.AllColumnModels
			//        .Where(c => c.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Absolute).Sum(c => c.WidthInfo.WidthValue) +
			//        (ShowDetailRow.Value ? _columnService.DetailExpanderColumnModel.WidthInfo.WidthValue : 0d);

			//    MinGridWidth.OnNext(widthSumOfAllAbsoluteWidth + _columnService.AllColumnModels
			//        .Where(c => c.WidthInfo.WidthMode == ColumnMetadataWidthInfo.ColumnWidthMode.Relative).Sum(c => c.WidthInfo.MinWidthValue));
			//}
			//else
			//{
			//GridColWidthSum = _columnService.AllColumnModels.Sum(c => c.VisibleAbsoluteWidthValue) + (ShowDetailRow.Value ? _columnService.DetailExpanderColumnModel.WidthInfo.WidthValue : 0d);
			//}
			//if (ColumnResizeMode.Value)
			await ShowDynamicHeader();
			//else
			//    await ShowFixHeader(0);
		}

		private async Task ShowDynamicHeader()
		{
			try
			{
				double gridInnerWidth = await GridInnerRef.GetElementWidthByRef(await _jsService.JsModule()) - 1;
				var tBodyWidth = await GridOuterRef.GetElementWidthByRef(await _jsService.JsModule());

				var declaratedAbsoluteColumnsWidthSum = _columnService.AllColumnModels.
					Where(cmt => cmt.DeclaratedWidthMode == DeclaratedColumnWidthMode.Absolute).Sum(cmt => cmt.DeclaratedWidth) +
					(ShowDetailRow.Value ? _columnService.DetailExpanderColumnModel.DeclaratedWidth : 0d);

				var relativePortionSum = _columnService.AllColumnModels.Where(cmt => cmt.DeclaratedWidthMode == DeclaratedColumnWidthMode.Relative).Sum(cmt => cmt.DeclaratedWidth);
				var absoluteColumnsWidthSumForRelative = gridInnerWidth - declaratedAbsoluteColumnsWidthSum;

				var emptyColWidth = tBodyWidth - declaratedAbsoluteColumnsWidthSum - absoluteColumnsWidthSumForRelative - 1;
				var portionValue = (gridInnerWidth - declaratedAbsoluteColumnsWidthSum) / relativePortionSum;

				_columnService.EmptyColumnModel.VisibleAbsoluteWidthValue = emptyColWidth;
				_columnService.ColumnResizingEndedObservable.OnNext(_columnService.EmptyColumnModel);

				Dictionary<IColumnModel, double> widthList = new Dictionary<IColumnModel, double>();

				foreach (var m in _columnService.AllColumnModels.Where(cmt => cmt.DeclaratedWidthMode == DeclaratedColumnWidthMode.Relative))
				{
					double nw = portionValue * m.DeclaratedWidth;
					widthList.Add(m, nw);
				}

				foreach (var m in _columnService.AllColumnModels.Where(cmt => cmt.DeclaratedWidthMode == DeclaratedColumnWidthMode.Absolute))
				{
					//double nw = m.DeclaratedWidth;
					widthList.Add(m, m.DeclaratedWidth);
				}

				foreach (var m in _columnService.AllColumnModels)
				{
					m.VisibleAbsoluteWidthValue = widthList[m];
					_columnService.ColumnResizingEndedObservable.OnNext(m);

					//m.VisiblePercentWidthValue = widthList[m].VisiblePercentWidthValue;
				}


			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		//private async Task ShowFixHeader(int newWidth)
		//{
		//    double gridInnerWidth = 0;

		//    gridInnerWidth = await GridInnerRef.GetElementWidthByRef(await _jsService.JsModule());

		//    Dictionary<IColumnModel, TempColumnMetadata> widthList = new Dictionary<IColumnModel, TempColumnMetadata>();

		//    foreach (var m in _columnService.AllColumnModels.Where(cmt => cmt.WidthInfo.DeclaratedWidthMode == DeclaratedColumnWidthMode.Absolute))
		//    {
		//        double nw = m.WidthInfo.WidthValue / gridInnerWidth;
		//        widthList.Add(m, new TempColumnMetadata { VisibleAbsoluteWidthValue = m.WidthInfo.WidthValue, VisiblePercentWidthValue = nw * 100d });
		//    }

		//    //Werte aus TempObjekt transferieren
		//    foreach (var m in _columnService.AllColumnModels.Where(widthList.ContainsKey))
		//    {
		//        m.VisibleAbsoluteWidthValue = widthList[m].VisibleAbsoluteWidthValue;
		//        m.VisiblePercentWidthValue = widthList[m].VisiblePercentWidthValue;
		//    }

		//    double tBodyWidth = 0;
		//    if (newWidth == 0)
		//        tBodyWidth = await GridOuterRef.GetElementWidthByRef(await _jsService.JsModule());
		//    else
		//        tBodyWidth = newWidth;

		//    CalculateEmptyColumn(tBodyWidth);
		//}

		private double CalculateEmptyColumn(double tBodyWidth)
		{
			double emptyColWidth = 0;

			emptyColWidth = tBodyWidth - ActualColumnsWidthSum;// - ActualScrollbarWidth - 1;
			_columnService.EmptyColumnModel.VisibleAbsoluteWidthValue = emptyColWidth;
			_columnService.ColumnResizingEndedObservable.OnNext(_columnService.EmptyColumnModel);
			return emptyColWidth;
			//_columnService.EmptyColumnModel.WidthInfo = new ColumnMetadataWidthInfo { DeclaratedWidthMode = DeclaratedColumnWidthMode.Absolute, WidthValue = emptyColWidth };
		}

		internal async Task OnDataGridInnerCssStyleChanged()
		{
			if (DataGridInnerCssStyleChanged != null)
			{
				DataGridStyleInfo info = new DataGridStyleInfo
				{
					CssStyle = OuterStyle.Value,
					ActualScrollbarWidth = 0//ActualScrollbarWidth
				};
				await DataGridInnerCssStyleChanged.Invoke(info);
			}
		}

		public ValueTask DisposeAsync()
		{
			return ValueTask.CompletedTask;
		}

		//class TempColumnMetadata
		//{
		//    public double VisibleAbsoluteWidthValue { get; set; }
		//    //public double VisiblePercentWidthValue { get; set; }
		//}
	}
}
