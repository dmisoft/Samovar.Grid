using System;

namespace Samovar.Blazor.Header
{
    public partial class SmDataGridHeaderScrollbar<T>
        : SmDesignComponentBase, IDisposable
    {
        [SmInject]
        public ILayoutService GridLayoutService { get; set; }
        
        public void Dispose()
        {
        }
    }
}
