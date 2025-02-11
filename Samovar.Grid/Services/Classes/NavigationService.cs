﻿using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Samovar.Grid
{
    public class NavigationService
        : INavigationService
    {
        public BehaviorSubject<NavigationMode> NavigationMode { get; } = new BehaviorSubject<NavigationMode>(Grid.NavigationMode.Paging);

        public required INavigationStrategy NavigationStrategy { get; set; }

        public BehaviorSubject<NavigationStrategyDataLoadingSettings> DataLoadingSettings { get; set; } = new BehaviorSubject<NavigationStrategyDataLoadingSettings>(NavigationStrategyDataLoadingSettings.Empty);

        private readonly IVirtualScrollingNavigationStrategy _virtualScrollingStrategy;
        private readonly IPagingNavigationStrategy _pagingStrategy;

        public NavigationService(
              IVirtualScrollingNavigationStrategy virtualScrollingService
            , IPagingNavigationStrategy pagingNavigationStrategy
            )
        {
            _virtualScrollingStrategy = virtualScrollingService;
            _pagingStrategy = pagingNavigationStrategy;

            NavigationMode.Subscribe(SetNavigationStrategy);
        }

        void SetNavigationStrategy(NavigationMode strategy)
        {
            NavigationStrategy = strategy switch
            {
                Grid.NavigationMode.Paging => _pagingStrategy,
                Grid.NavigationMode.Virtual => _virtualScrollingStrategy,
                _ => throw new NotImplementedException()
            };
        }
    }
}
