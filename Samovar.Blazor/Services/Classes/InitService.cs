using Microsoft.JSInterop;
using System;
using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public class InitService
        : IInitService, IDisposable
    {
        public IJSObjectReference jsModule { get; private set; }

        public BehaviorSubject<bool> IsInitialized { get; set; }
        public InitService()
        {
            IsInitialized = new BehaviorSubject<bool>(false);
        }

        public void Dispose()
        {
            
        }
    }
}
