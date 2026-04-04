using Microsoft.AspNetCore.Components;

namespace Samovar.Grid;

public partial class SmScrollbar : ComponentBase
{
    [Parameter]
    public ScrollbarOrientation Orientation { get; set; }

    public ElementReference TrackRef { get; set; }
    public ElementReference ThumbRef { get; set; }

    private string OrientationClass => Orientation == ScrollbarOrientation.Vertical
        ? "sm-scrollbar-vertical"
        : "sm-scrollbar-horizontal";

    private string OrientationName => Orientation == ScrollbarOrientation.Vertical
        ? "vertical"
        : "horizontal";
}
