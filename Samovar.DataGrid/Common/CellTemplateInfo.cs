using Microsoft.AspNetCore.Components;

namespace Samovar.DataGrid
{
    internal class CellTemplateInfo
    {
        internal CellTemplateInfo()
        {
        }
        internal RenderFragment<object> CellShowTemplate { get; set; }
        internal RenderFragment<object> CellEditTemplate { get; set; }
    }
}
