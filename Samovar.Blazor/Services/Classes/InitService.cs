using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public class InitService
        : IInitService
    {
        public Subject<bool> IsInitialized { get; set; } = new Subject<bool>();
    }
}
