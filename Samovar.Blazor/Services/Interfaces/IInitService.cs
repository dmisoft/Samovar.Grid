using Microsoft.JSInterop;

namespace Samovar.Blazor
{
    public interface IInitService
    {
        ISubject<bool> IsInitialized { get; set; }
    }
}
