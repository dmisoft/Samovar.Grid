using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public partial class SmDataGridInner2<T>
        : SmDesignComponentBase, IAsyncDisposable
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [SmInject]
        public ILayoutService GridLayoutService { get; set; }

        [SmInject]
        public IConstantService ConstantService { get; set; }

        [SmInject]
        public IVirtualScrollingNavigationStrategy VirtualScrollingService { get; set; }

        [SmInject]
        public IRepositoryService<T> RepositoryService { get; set; }

        [SmInject]
        public INavigationService NavigationService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public DataGridStyleInfo? Style { get; set; }

        protected override void OnInitialized()
        {
            Style = new DataGridStyleInfo
            {
                CssStyle = GridLayoutService.OuterStyle.Value,
                ActualScrollbarWidth = GridLayoutService.ActualScrollbarWidth
            };
            GridLayoutService.DataGridInnerCssStyleChanged += GridLayoutService_DataGridInnerCssStyleChanged;
            base.OnInitialized();
        }

        private async Task GridLayoutService_DataGridInnerCssStyleChanged(DataGridStyleInfo arg)
        {
            Style = arg;
            await InvokeAsync(StateHasChanged);
        }

        public string? ScrollStyle { get; set; }
        public double OffsetY { get; set; }

        public ValueTask DisposeAsync()
        {
            GridLayoutService.DataGridInnerCssStyleChanged -= GridLayoutService_DataGridInnerCssStyleChanged;
            return ValueTask.CompletedTask;
        }
    }
}
