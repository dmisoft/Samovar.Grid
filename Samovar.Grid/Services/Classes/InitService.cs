using System.Reactive.Subjects;

namespace Samovar.Grid
{
    public class InitService
        : IInitService
    {
        public Subject<bool> IsInitialized { get; set; } = new Subject<bool>();
    }
}
