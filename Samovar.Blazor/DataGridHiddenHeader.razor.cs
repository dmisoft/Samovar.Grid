using System;

namespace Samovar.Blazor
{
	public partial class DataGridHiddenHeader<TItem>
        : SmDesignComponentBase, IDisposable
    {
        [SmInject]
        protected IColumnService GridColumnService { get; set; }

        [SmInject]
        protected ILayoutService GridLayoutService { get; set; }

        public void Dispose()
        {
        }
    }
}
