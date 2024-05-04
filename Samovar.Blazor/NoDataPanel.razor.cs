using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            throw new NotImplementedException();
        }
    }
}
