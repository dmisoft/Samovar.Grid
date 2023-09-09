using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public class SmDataGrid<T>
        : SmDataGridBase<T>
    {
        
        [SmInject]
        public IInitService InitService { get; set; }

        [SmInject]
        public IRepositoryService<T> RepositoryService { get; set; }

        [SmInject]
        public IEditingService<T> EditingService { get; set; }

        [SmInject]
        public INavigationService NavigationService { get; set; }

        [SmInject]
        public IPagingNavigationStrategy PagingNavigationStrategy { get; set; }

        [SmInject]
        public IConstantService ConstantService { get; set; }

        [SmInject]
        public ILayoutService LayoutService { get; set; }

        [SmInject]
        public IVirtualScrollingService VirtualScrollingService { get; set; }

        [SmInject]
        public ITemplateService<T> TemplateService { get; set; }

        [SmInject]
        public IGridSelectionService<T> GridSelectionService { get; set; }

        [SmInject]
        public IComponentBuilderService ComponentBuilderService { get; set; }

        [SmInject]
        public IDataSourceService<T> DataSourceService { get; set; }

        [Parameter]
        public RenderFragment Columns
        {
            get
            {
                return base.ChildContent;
            }
            set
            {
                base.ChildContent = value;
            }
        }

        [Parameter]
        public IEnumerable<T> Data
        {
            get
            {
                return null;
            }
            set
            {
                DataSourceService.Data.OnNextParameterValue(value);
            }
        }

        [Parameter]
        public DataGridFilterMode FilterMode
        {
            get
            {
                return DataGridFilterMode.None;
            }
            set
            {
                LayoutService.FilterMode.OnNextParameterValue(value);
            }
        }

        [Parameter]
        public int PageSize
        {
            get
            {
                return 0;
            }
            set
            {
                PagingNavigationStrategy.PageSize.OnNextParameterValue(value);
            }
        }

        [Parameter]
        public int PagerSize
        {
            get
            {
                return 0;
            }
            set
            {
                PagingNavigationStrategy.PagerSize.OnNextParameterValue(value);
            }
        }

        [Parameter]
        public string Height
        {
            get
            {
                return null;
            }
            set
            {
                LayoutService.Height.OnNextParameterValue(value);
            }
        }

        [Parameter]
        public string Width
        {
            get
            {
                return null;
            }
            set
            {
                LayoutService.Width.OnNextParameterValue(value);
            }
        }

        [Parameter]
        public string FilterToggleButtonClass
        {
            get
            {
                return null;
            }
            set
            {
                LayoutService.FilterToggleButtonClass.OnNextParameterValue(value);
            }
        }

        [Parameter]
        public string CssClass
        {
            get
            {
                return null;
            }
            set
            {
                LayoutService.TableTagClass.OnNextParameterValue(value);
            }
        }

        [Parameter]
        public bool ShowColumnHeader
        {
            get
            {
                return false;
            }
            set
            {
                LayoutService.ShowColumnHeader.OnNextParameterValue(value);
            }
        }

        [Parameter]
        public bool ShowDetailRow
        {
            get
            {
                return false;
            }
            set
            {
                LayoutService.ShowDetailRow.OnNextParameterValue(value);
            }
        }

        [Parameter]
        public DataGridNavigationMode DataNavigationMode
        {
            get
            {
                return NavigationService.NavigationMode.SubjectValue;
            }
            set
            {
                NavigationService.NavigationMode.OnNextParameterValue(value);
            }
        }

        [Parameter]
        public RenderFragment<T> DetailRowTemplate { get { return null; } set { TemplateService.DetailRowTemplate.OnNextParameterValue(value); } }

        [Parameter]
        public RenderFragment<T> EditFormTemplate { get { return null; } set { TemplateService.EditFormTemplate.OnNextParameterValue(value); } }

        [Parameter]
        public RenderFragment<T> InsertFormTemplate { get { return null; } set { TemplateService.InsertFormTemplate.OnNextParameterValue(value); } }

        [Parameter]
        public GridEditMode EditMode { get { return GridEditMode.None; } set { EditingService.EditMode.OnNextParameterValue(value); } }

        [Parameter]
        public EventCallback<T> RowEditBegin
        {
            get { return EditingService.OnRowEditBegin.SubjectValue; }
            set { EditingService.OnRowEditBegin.OnNextParameterValue(value); }
        }

        [Parameter]
        public EventCallback RowInsertBegin
        {
            get { return EditingService.OnRowInsertBegin.SubjectValue; }
            set { EditingService.OnRowInsertBegin.OnNextParameterValue(value); }
        }

        //[Parameter]
        //public EventCallback<Dictionary<string, object>> InitializeNewRow
        //{
        //    get { return EditingService.OnInitializeNewRow.SubjectValue; }
        //    set { EditingService.OnInitializeNewRow.OnNextParameterValue(value); }
        //}

        
        [Parameter]
        public EventCallback<T> RowInserting
        {
            get { return EditingService.OnRowInserting.SubjectValue; }
            set { EditingService.OnRowInserting.OnNextParameterValue(value); }
        }

        [Parameter]
        public EventCallback<T> RowRemoving
        {
            get { return EditingService.OnRowRemoving.SubjectValue; }
            set { EditingService.OnRowRemoving.OnNextParameterValue(value); }
        }

        [Parameter]
        public GridSelectionMode SelectionMode
        {
            get { return GridSelectionMode.None; }
            set { GridSelectionService.SelectionMode.OnNextParameterValue(value); }
        }

        [Parameter]
        public T SingleSelectedDataRow
        {
            get { return default(T); }
            set
            {
                //GridSelectionService.SingleSelectedDataRow.OnNextParameterValue(value, SingleSelectedDataRowChanged);
                GridSelectionService.SingleSelectedDataRow.OnNextParameterValue(value);//, SingleSelectedDataRowChanged);
            }
        }

        [Parameter]
        public EventCallback<T> SingleSelectedDataRowChanged { get; set; }

        [Parameter]
        public IEnumerable<T> MultipleSelectedDataRows
        {
            get { return default(IEnumerable<T>); }
            set
            {
                GridSelectionService.MultipleSelectedDataRows.OnNextParameterValue(value);
            }
        }

        [Parameter]
        public EventCallback<IEnumerable<T>> MultipleSelectedDataRowsChanged { get; set; }


        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {

            RenderFragment del = delegate (RenderTreeBuilder builder2)
            {
                Columns?.Invoke(builder2);

                builder2.OpenComponent<SmDataGridInner<T>>(5);
                builder2.CloseComponent();
            };

            builder.OpenComponent<CascadingValue<IComponentServiceProvider>>(1);
            builder.AddAttribute(2, "Value", this);
            builder.AddAttribute(3, "Name", "ServiceProvider");
            builder.AddAttribute(4, "ChildContent", del);
            builder.CloseComponent();

        }
        protected override Task OnInitializedAsync()
        {

            GridSelectionService.SingleSelectedRowCallback = async () => { await SingleSelectedDataRowChanged.InvokeAsync(GridSelectionService.SingleSelectedDataRow.SubjectValue); };
            
            GridSelectionService.MultipleSelectedRowsCallback = async () => { await MultipleSelectedDataRowsChanged.InvokeAsync(GridSelectionService.MultipleSelectedDataRows.SubjectValue); };

            return base.OnInitializedAsync();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                //Lazy<Task<IJSObjectReference>> moduleTask = new(() => JsRuntime.InvokeAsync<IJSObjectReference>(
                //    "import", "./_content/Samovar.Blazor/samovar.blazor.js").AsTask());

                //await JsService.InitJsModule2(moduleTask);

                await JsService.AttachWindowResizeEvent(ConstantService.DataGridId, LayoutService.DataGridDotNetRef);

                //JsService.AttachOnScrollollingEvent(ConstantService.InnerGridId, VirtualScrollingService.DotNetRef);

                await LayoutService.InitHeader();

                InitService.IsInitialized.OnNextParameterValue(true);

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

        public Task ExpandAllDetailRows() {

            return Task.CompletedTask;
        }

        public Task CollapseAllDetailRows()
        {
            return Task.CompletedTask;
        }
    }
}
