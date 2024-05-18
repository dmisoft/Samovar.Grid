﻿using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public class NavigationService
        : INavigationService
    {
        public BehaviorSubject<DataGridNavigationMode> NavigationMode { get; } = new BehaviorSubject<DataGridNavigationMode>(DataGridNavigationMode.Paging);

        public INavigationStrategy NavigationStrategy { get; set; }

        public BehaviorSubject<NavigationStrategyDataLoadingSettings> DataLoadingSettings { get; set; } = new BehaviorSubject<NavigationStrategyDataLoadingSettings>(NavigationStrategyDataLoadingSettings.Empty);

        private readonly INewVirtualScrollingNavigationStrategy _newVirtualScrollingStrategy;
        private readonly IVirtualScrollingNavigationStrategy _virtualScrollingStrategy;
        private readonly IPagingNavigationStrategy _pagingStrategy;

        public NavigationService(
              INewVirtualScrollingNavigationStrategy newVirtualScrollingService
            , IVirtualScrollingNavigationStrategy virtualScrollingService
            , IPagingNavigationStrategy pagingNavigationStrategy
            , IInitService initService
            )
        {
            _newVirtualScrollingStrategy = newVirtualScrollingService;
            _virtualScrollingStrategy = virtualScrollingService;
            _pagingStrategy = pagingNavigationStrategy;

            //initService.IsInitialized.Subscribe(DataGridInitializerCallback);
            NavigationMode.Subscribe(SetNavigationStrategy);

            //NavigationStrategy = pagingNavigationStrategy;
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
                DataGridNavigationMode.NewVirtualScrolling => _newVirtualScrollingStrategy,
                _ => throw new NotImplementedException()
            };
        }
    }
}
