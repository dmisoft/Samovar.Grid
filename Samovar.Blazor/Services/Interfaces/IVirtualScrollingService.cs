using Microsoft.JSInterop;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public interface IVirtualScrollingService
        : INavigationStrategy
    {
        DotNetObjectReference<IVirtualScrollingService> DotNetRef { get; }

        int VisibleItems { get; set; }
        int ItemsToShow { get; set; }
        double DummyRowHeight { get; set; }
        int DummyItemsCount { get; set; }

        int TopVisibleDataItemPosition { get; set; }
        int StartGridItemPosition { get; set; }
        int EndGridItemPosition { get; set; }

        BehaviorSubject<string> TranslatableDivHeightValue { get; }
        BehaviorSubject<DataGridVirtualScrollingInfo> VirtualScrollingInfo { get; }
        Task<double> TranslatableDivHeight(int itemCount);
        double TranslateYOffset { get; }
    }
}
