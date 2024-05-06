﻿using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Samovar.Blazor.Filter
{
    public abstract partial class DataGridFilterCellBase<TFilterCell>
        : SmDesignComponentBase, IAsyncDisposable
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [SmInject]
        public IJsService JsService { get; set; }

        [SmInject]
        public ILayoutService LayoutService { get; set; }

        [SmInject]
        public IFilterService FilterService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


        [Parameter]
        public required IDataColumnModel ColMetadata { get; set; }

        protected string DropdownMenuButtonId { get; } = $"dropdownmenubtn{Guid.NewGuid().ToString().Replace("-", "")}";

        //0 =
        //1 *A*
        //2 A*
        //3 *A
        protected byte _menuMode { get; set; }

        protected DataGridFilterCellInfo FilterCellInfo;

        private TFilterCell _innerValue = default;

        protected TFilterCell InnerValue
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

        protected override void OnInitialized()
        {
            _innerValue = FilterService.TryGetFilterCellValue<TFilterCell>(ColMetadata);
            FilterService.FilterCleared += FilterService_FilterCleared;
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
            await (await JsService.JsModule()).InvokeVoidAsync("toggleFilterPopupMenu", DropdownMenuButtonId, ColMetadata.FilterMenuContainerId, ColMetadata.FilterMenuId, filterMenuOpen);
        }

        public ValueTask DisposeAsync()
        {
            FilterService.FilterCleared -= FilterService_FilterCleared;
            return ValueTask.CompletedTask;
        }
    }
}
