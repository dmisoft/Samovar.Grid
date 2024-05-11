namespace Samovar.Blazor.Header
{
    public partial class SmDataGridHeaderScrollbar
        : SmDesignComponentBase
    {
        [SmInject]
        public required ILayoutService GridLayoutService { get; set; }
    }
}
