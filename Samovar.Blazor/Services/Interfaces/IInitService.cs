using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public interface IInitService
    {
        BehaviorSubject<bool> IsInitialized { get; set; }
    }
}
