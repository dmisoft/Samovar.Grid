using Microsoft.JSInterop;
using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public interface IVirtualScrollingNavigationStrategy
        : INavigationStrategy
    {
        DotNetObjectReference<IVirtualScrollingNavigationStrategy> DotNetRef { get; }

        int VisibleItems { get; set; }
        int ItemsToShow { get; set; }
        double TopPlaceholderRowHeight { get; set; }
        double BottomPlaceholderRowHeight { get; set; }

        int TopVisibleDataItemPosition { get; set; }
        int StartGridItemPosition { get; set; }
        int EndGridItemPosition { get; set; }

        BehaviorSubject<DataGridVirtualScrollingInfo> VirtualScrollingInfo { get; }
        Task<double> GetTranslatableDivHeight(int itemCount);
        BehaviorSubject<double> ScrollTop { get; }
        double TranslateYOffset { get; }
    }
}
