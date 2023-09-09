using Microsoft.JSInterop;
using System;

namespace Samovar.Blazor
{
    public class InitService
        : IInitService, IDisposable
    {
        public IJSObjectReference jsModule { get; private set; }

        public ISubject<bool> IsInitialized { get; set; }
        public InitService()
        {
            IsInitialized = new ParameterSubject<bool>(false);
        }

        public void Dispose()
        {
            
        }
    }
}
