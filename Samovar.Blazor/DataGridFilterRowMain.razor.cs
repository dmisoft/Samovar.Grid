namespace Samovar.Blazor
{
    public partial class DataGridFilterRowMain<T>
        : SmDesignComponentBase , IAsyncDisposable
    {
        [SmInject]
        public ILayoutService? GridLayoutService { get; set; }

        [SmInject]
        public IConstantService? ConstantService { get; set; }

        public DataGridStyleInfo? Style { get; set; } //Default style


        protected override void OnInitialized()
        {
            if(GridLayoutService is null)
                throw new ArgumentNullException();

            Style = new DataGridStyleInfo { 
                CssStyle = GridLayoutService.OuterStyle.Value,
                ActualScrollbarWidth = GridLayoutService.ActualScrollbarWidth!
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

