using System.Reactive.Subjects;

namespace Samovar.Grid
{
    public interface IInitService
    {
        Subject<bool> IsInitialized { get; set; }
    }
}
