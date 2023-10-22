using System;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public partial class DataGridHeaderRow<T>
        : SmDesignComponentBase , IDisposable //TODO kein DesignComponent
    {
        [SmInject]
        public ILayoutService GridLayoutService { get; set; }

        [SmInject]
        public IConstantService ConstantService { get; set; }

        public DataGridStyleInfo Style { get; set; } //Default style

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

        public void Dispose()
        {
            GridLayoutService.DataGridInnerCssStyleChanged -= GridLayoutService_DataGridInnerCssStyleChanged;
        }
    }
}
