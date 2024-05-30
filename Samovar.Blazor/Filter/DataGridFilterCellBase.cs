﻿using Microsoft.AspNetCore.Components;
using System.Reactive.Linq;

namespace Samovar.Blazor.Filter
{
	public abstract partial class DataGridFilterCellBase<TFilterCell>
        : SmDesignComponentBase, IAsyncDisposable
    {

        [SmInject]
        public required IJsService JsService { get; set; }

        [SmInject]
        public required ILayoutService LayoutService { get; set; }

        [SmInject]
        public required IFilterService FilterService { get; set; }

        [SmInject]
        public required IColumnService ColumnService { get; set; }

        [Parameter]
        public required IDataColumnModel ColMetadata { get; set; }

        protected string DropdownMenuButtonId { get; } = $"dropdownmenubtn{Guid.NewGuid().ToString().Replace("-", "")}";

        //0 =
        //1 *A*
        //2 A*
        //3 *A
        protected byte _menuMode { get; set; }

        protected DataGridFilterCellInfo? FilterCellInfo;

        private TFilterCell? _innerValue = default;

        protected TFilterCell? InnerValue
        {
            set
            {
                _innerValue = value;
                FilterCellInfo = new DataGridFilterCellInfo { ColumnMetadata = ColMetadata, FilterCellValue = _innerValue, FilterCellMode = _menuMode };
                FilterService.Filter(FilterCellInfo);
            }
            get
            {
                return _innerValue;
            }
        }
        protected string WidthStyle = "";
        protected override Task OnInitializedAsync()
        {
            _innerValue = FilterService.TryGetFilterCellValue<TFilterCell>(ColMetadata);
            FilterService.FilterCleared += FilterService_FilterCleared;
            //ColumnService.ColumnResizingEndedObservable.Where(c => c.Id == ColMetadata.Id).Subscribe(c => StateHasChanged());
            
			ColMetadata.WidthStyle.Subscribe(w => {
				WidthStyle = w;
                StateHasChanged();
            });
			return base.OnInitializedAsync();
		}

		private Task FilterService_FilterCleared()
        {
            _innerValue = default;
            return Task.CompletedTask;
        }

        bool filterMenuOpen = false;

        protected async Task ShowMenu()
        {
            filterMenuOpen = !filterMenuOpen;
            //await (await JsService.JsModule()).InvokeVoidAsync("toggleFilterPopupMenu", DropdownMenuButtonId, ColMetadata.FilterMenuContainerId, ColMetadata.FilterMenuId, filterMenuOpen);
        }

        public ValueTask DisposeAsync()
        {
            FilterService.FilterCleared -= FilterService_FilterCleared;
            return ValueTask.CompletedTask;
        }
    }
}
