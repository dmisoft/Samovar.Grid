using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public class NavigationService
        : INavigationService
    {
        public BehaviorSubject<DataGridNavigationMode> NavigationMode { get; } = new BehaviorSubject<DataGridNavigationMode>(DataGridNavigationMode.Paging);

        public INavigationStrategy NavigationStrategy { get; private set; }
        private readonly IVirtualScrollingNavigationStrategy _virtualScrollingStrategy;
        private readonly IPagingNavigationStrategy _pagingStrategy;

        public NavigationService(
              IVirtualScrollingNavigationStrategy virtualScrollingService
            , IPagingNavigationStrategy pagingNavigationStrategy
            , IInitService initService
            )
        {
            _virtualScrollingStrategy = virtualScrollingService;
            _pagingStrategy = pagingNavigationStrategy;

            initService.IsInitialized.Subscribe(DataGridInitializerCallback);

            NavigationStrategy = pagingNavigationStrategy;
        }

        private void DataGridInitializerCallback(bool obj)
        {
            NavigationMode.Subscribe(SetNavigationStrategy);
        }

        void SetNavigationStrategy(DataGridNavigationMode strategy)
        {
            NavigationStrategy = strategy switch
            {
                DataGridNavigationMode.Paging => _pagingStrategy,
                DataGridNavigationMode.VirtualScrolling => _virtualScrollingStrategy,
                _ => throw new NotImplementedException()
            };
        }
    }
}
