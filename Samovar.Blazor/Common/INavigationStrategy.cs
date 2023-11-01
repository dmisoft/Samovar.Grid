using System;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public interface INavigationStrategy
    {
        IObservable<NavigationStrategyDataLoadingSettings> DataLoadingSettings { get; }
    }
}
