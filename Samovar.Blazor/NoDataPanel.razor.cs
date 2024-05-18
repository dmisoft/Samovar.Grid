namespace Samovar.Blazor
{
    public partial class NoDataPanel
        : SmDesignComponentBase, IAsyncDisposable
    {
        protected double ContainerHeight = 0;

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                ContainerHeight = 330;

            return base.OnAfterRenderAsync(firstRender);
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
