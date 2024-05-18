using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Samovar.Blazor
{
    public class SmDataGrid<T>
        : SmDataGridBase<T>
    {
        [SmInject]
        public required IInitService InitService { get; set; }

        [SmInject]
        public required IRepositoryService<T> RepositoryService { get; set; }

        [SmInject]
        public required IEditingService<T> EditingService { get; set; }

        [SmInject]
        public required INavigationService NavigationService { get; set; }

        [SmInject]
        public required IPagingNavigationStrategy PagingNavigationStrategy { get; set; }

        [SmInject]
        public required IConstantService ConstantService { get; set; }

        [SmInject]
        public required ILayoutService LayoutService { get; set; }

        [SmInject]
        public required ITemplateService<T> TemplateService { get; set; }

        [SmInject]
        public required IGridSelectionService<T> GridSelectionService { get; set; }

        [SmInject]
        public required IComponentBuilderService ComponentBuilderService { get; set; }

        [SmInject]
        public required IDataSourceService<T> DataSourceService { get; set; }

        [Parameter]
        public required RenderFragment Columns { get; set; }

        [Parameter]
        public IEnumerable<T>? Data { get; set; }

        [Parameter]
        public DataGridFilterMode? FilterMode { get; set; }

        [Parameter]
        public int PageSize { get; set; }

        [Parameter]
        public EventCallback<int> PageSizeChanged { get; set; }

        [Parameter]
        public int PagerSize { get; set; }

        [Parameter]
        public string? Height { get; set; }

        [Parameter]
        public string? Width { get; set; }

        [Parameter]
        public string? FilterToggleButtonClass { get; set; }

        [Parameter]
        public string? CssClass { get; set; }

        [Parameter]
        public bool? ShowColumnHeader { get; set; }


        [Parameter]
        public bool? ShowDetailRow { get; set; }

        [Parameter]
        public DataGridNavigationMode? DataNavigationMode { get; set; }

        [Parameter]
        public DataGridEditMode EditMode { get; set; }

        [Parameter]
        public RenderFragment<T>? DetailRowTemplate { get; set; }

        [Parameter]
        public RenderFragment<T>? EditFormTemplate { get; set; }

        [Parameter]
        public RenderFragment<T>? InsertFormTemplate { get; set; }


        [Parameter]
        public EventCallback<T> RowEditBegin { get; set; }

        [Parameter]
        public EventCallback RowInsertBegin { get; set; }

        [Parameter]
        public EventCallback<T> RowInserting { get; set; }

        [Parameter]
        public EventCallback<T> RowRemoving { get; set; }

        [Parameter]
        public GridSelectionMode SelectionMode { get; set; }

        [Parameter]
        public T? SingleSelectedDataRow { get; set; }

        [Parameter]
        public EventCallback<T> SingleSelectedDataRowChanged { get; set; }

        [Parameter]
        public IEnumerable<T>? MultipleSelectedDataRows { get; set; }

        [Parameter]
        public EventCallback<IEnumerable<T>> MultipleSelectedDataRowsChanged { get; set; }

        [Parameter]
        public Func<T, Task<string>>? EditingFormTitleDelegate { get; set; }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            RenderFragment? columns = parameters.GetValueOrDefault<RenderFragment>(nameof(Columns));
            if (columns is null)
                throw new ArgumentException(nameof(Columns));
            else
                ChildContent = columns;

            int pageSize = parameters.GetValueOrDefault<int>(nameof(PageSize));
            pageSize = pageSize == 0 ? 50 : pageSize;
            PagingNavigationStrategy.PageSize.OnNext(pageSize);

            int pagerSize = parameters.GetValueOrDefault<int>(nameof(PagerSize));
            pagerSize = pagerSize == 0 ? 10 : pagerSize;
            PagingNavigationStrategy.PagerSize.OnNext(pagerSize);

            string? height = parameters.GetValueOrDefault<string>(nameof(Height));
            if (height != null)
                LayoutService.Height.OnNext(height);

            string? width = parameters.GetValueOrDefault<string>(nameof(Width));
            if (width != null)
                LayoutService.Width.OnNext(width);

            string? filterToggleButtonClass = parameters.GetValueOrDefault<string>(nameof(FilterToggleButtonClass));
            if (filterToggleButtonClass != null)
                LayoutService.FilterToggleButtonClass.OnNext(filterToggleButtonClass);

            string? cssClass = parameters.GetValueOrDefault<string>(nameof(CssClass));
            if (cssClass != null)
                LayoutService.TableTagClass.OnNext(cssClass);

            bool? showColumnHeader = parameters.GetValueOrDefault<bool?>(nameof(ShowColumnHeader));
            showColumnHeader ??= true;
            LayoutService.ShowColumnHeader.OnNext(showColumnHeader.Value);

            bool? showDetailRow = parameters.GetValueOrDefault<bool?>(nameof(ShowDetailRow));
            showDetailRow ??= false;
            LayoutService.ShowDetailRow.OnNext(showDetailRow.Value);


            DataGridFilterMode? dataGridFilterMode = parameters.GetValueOrDefault<DataGridFilterMode?>(nameof(FilterMode));
            dataGridFilterMode ??= DataGridFilterMode.None;
            LayoutService.FilterMode.OnNext(dataGridFilterMode.Value);

            DataGridEditMode? dataGridEditMode = parameters.GetValueOrDefault<DataGridEditMode?>(nameof(EditMode));
            dataGridEditMode ??= DataGridEditMode.None;
            EditingService.EditMode.OnNext(dataGridEditMode.Value);

            GridSelectionMode? dataGridSelectionMode = parameters.GetValueOrDefault<GridSelectionMode?>(nameof(SelectionMode));
            dataGridSelectionMode ??= GridSelectionMode.None;
            GridSelectionService.SelectionMode.OnNext(dataGridSelectionMode.Value);

            T? singleSelectedDataRow = parameters.GetValueOrDefault<T?>(nameof(SingleSelectedDataRow));
            if (singleSelectedDataRow is not null)
                GridSelectionService.SingleSelectedDataRow.OnNext(singleSelectedDataRow);

            IEnumerable<T>? multipleSelectedDataRows = parameters.GetValueOrDefault<IEnumerable<T>?>(nameof(MultipleSelectedDataRows));
            if (multipleSelectedDataRows is not null && multipleSelectedDataRows.Any())
                GridSelectionService.MultipleSelectedDataRows.OnNext(multipleSelectedDataRows);

            Func<T, Task<string>>? editingFormTitleDelegate = parameters.GetValueOrDefault<Func<T, Task<string>>?>(nameof(EditingFormTitleDelegate));
            if (editingFormTitleDelegate is not null)
                EditingService.EditingFormTitleDelegate = editingFormTitleDelegate;

            RenderFragment<T>? detailRowTemplate = parameters.GetValueOrDefault<RenderFragment<T>>(nameof(DetailRowTemplate));
            if (detailRowTemplate != null)
                TemplateService.DetailRowTemplate.OnNext(detailRowTemplate);

            RenderFragment<T>? editFormTemplate = parameters.GetValueOrDefault<RenderFragment<T>>(nameof(EditFormTemplate));
            if (editFormTemplate != null)
                TemplateService.EditFormTemplate.OnNext(editFormTemplate);

            RenderFragment<T>? insertFormTemplate = parameters.GetValueOrDefault<RenderFragment<T>>(nameof(InsertFormTemplate));
            if (insertFormTemplate != null)
                TemplateService.InsertFormTemplate.OnNext(insertFormTemplate);

            EventCallback<T> rowEditBegin = parameters.GetValueOrDefault<EventCallback<T>>(nameof(RowEditBegin));
            if (rowEditBegin.HasDelegate)
                EditingService.OnRowEditBegin.OnNext(rowEditBegin);

            EventCallback rowInsertBegin = parameters.GetValueOrDefault<EventCallback>(nameof(RowInsertBegin));
            if (rowInsertBegin.HasDelegate)
                EditingService.OnRowInsertBegin.OnNext(rowInsertBegin);

            EventCallback<T> rowInserting = parameters.GetValueOrDefault<EventCallback<T>>(nameof(RowInserting));
            if (rowInserting.HasDelegate)
                EditingService.OnRowInserting.OnNext(rowInserting);

            EventCallback<T> rowRemoving = parameters.GetValueOrDefault<EventCallback<T>>(nameof(RowRemoving));
            if (rowRemoving.HasDelegate)
                EditingService.OnRowRemoving.OnNext(rowRemoving);


            DataGridNavigationMode? dataNavigationMode = parameters.GetValueOrDefault<DataGridNavigationMode?>(nameof(DataNavigationMode));
            dataNavigationMode ??= DataGridNavigationMode.Paging;
            NavigationService.NavigationMode.OnNext(dataNavigationMode.Value);

            IEnumerable<T>? data = parameters.GetValueOrDefault<IEnumerable<T>>(nameof(Data));
            if (data != null)
                DataSourceService.Data.OnNext(data);

        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            RenderFragment del;
            switch (NavigationService.NavigationMode.Value)
            {
                case DataGridNavigationMode.Paging:
                    del = delegate (RenderTreeBuilder builder2)
                    {
                        Columns?.Invoke(builder2);

                        builder2.OpenComponent<SmDataGridPagingTableInner<T>>(5);
                        builder2.CloseComponent();
                    };
                    break;
                case DataGridNavigationMode.NewVirtualScrolling:
                    del = delegate (RenderTreeBuilder builder2)
                    {
                        Columns?.Invoke(builder2);

                        builder2.OpenComponent<SmDataGridVirtualTableInner<T>>(5);
                        builder2.CloseComponent();
                    };
                    break;
                default:
                    throw new NotImplementedException();
            }

            builder.OpenComponent<CascadingValue<IComponentServiceProvider>>(1);
            builder.AddAttribute(2, "Value", this);
            builder.AddAttribute(3, "Name", "ServiceProvider");
            builder.AddAttribute(4, "ChildContent", del);
            builder.CloseComponent();
        }

        protected override Task OnInitializedAsync()
        {

            GridSelectionService.SingleSelectedRowCallback = async () => { await SingleSelectedDataRowChanged.InvokeAsync(GridSelectionService.SingleSelectedDataRow.Value); };

            GridSelectionService.MultipleSelectedRowsCallback = async () => { await MultipleSelectedDataRowsChanged.InvokeAsync(GridSelectionService.MultipleSelectedDataRows.Value); };

            return base.OnInitializedAsync();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await JsService.AttachWindowResizeEvent(ConstantService.DataGridId, LayoutService.DataGridDotNetRef);

                await LayoutService.InitHeader();

                InitService.IsInitialized.OnNext(true);

                StateHasChanged();
            }
        }

        public Task CancelRowEdit()
        {
            return EditingService.RowEditCancel();
        }

        public Task CancelRowInsert()
        {
            return EditingService.RowInsertCancel();
        }

        public Task ExpandAllDetailRows()
        {

            return Task.CompletedTask;
        }

        public Task CollapseAllDetailRows()
        {
            return Task.CompletedTask;
        }
    }
}