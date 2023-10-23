using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public interface IInitService
    {
        Subject<bool> IsInitialized { get; set; }
    }
}
