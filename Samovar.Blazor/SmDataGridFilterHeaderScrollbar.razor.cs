using System;

namespace Samovar.Blazor
{
    public partial class SmDataGridFilterHeaderScrollbar<TItem>
        : SmDesignComponentBase ,IDisposable
    {
        [SmInject]
        public ILayoutService LayoutService { get; set; }

        public void Dispose()
        {
        }
    }
}
