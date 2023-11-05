using System;
using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public interface INavigationStrategy
    {
        IObservable<NavigationStrategyDataLoadingSettings> DataLoadingSettings { get; }
    }
}
