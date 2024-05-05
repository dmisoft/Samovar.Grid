using System;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public partial class DataGridHeaderRow<T>
        : SmDesignComponentBase, IAsyncDisposable
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [SmInject]
        public ILayoutService GridLayoutService { get; set; }

        [SmInject]
        public IConstantService ConstantService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public DataGridStyleInfo Style { get; set; }

        protected override void OnInitialized()
        {
            Style = new DataGridStyleInfo { 
                CssStyle = GridLayoutService.OuterStyle.Value,
                ActualScrollbarWidth = GridLayoutService.ActualScrollbarWidth
            };
            GridLayoutService.DataGridInnerCssStyleChanged += GridLayoutService_DataGridInnerCssStyleChanged;
            base.OnInitialized();
        }

        private Task GridLayoutService_DataGridInnerCssStyleChanged(DataGridStyleInfo arg)
        {
            Style = arg;
            InvokeAsync(StateHasChanged);
            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            GridLayoutService.DataGridInnerCssStyleChanged -= GridLayoutService_DataGridInnerCssStyleChanged;
            return ValueTask.CompletedTask;
        }
    }
}
