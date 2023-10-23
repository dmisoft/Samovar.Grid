using Microsoft.JSInterop;
using System;
using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public class InitService
        : IInitService, IDisposable
    {
        public IJSObjectReference jsModule { get; private set; }

        public Subject<bool> IsInitialized { get; set; }
        public InitService()
        {
            IsInitialized = new Subject<bool>();
        }

        public void Dispose()
        {
            
        }
    }
}
