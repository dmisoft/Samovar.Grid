using Microsoft.AspNetCore.Components;

namespace Samovar.DataGrid
{
    public partial class GridCellTemplate
        //: ComponentBase
    {
        [Parameter]
        public RenderFragment CellViewTemplate { get; set; }
        [Parameter]
        public RenderFragment CellEditTemplate { get; set; }
    }
}
