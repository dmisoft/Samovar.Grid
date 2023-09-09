using Microsoft.AspNetCore.Components;

namespace Samovar.Blazor
{
    public interface ITemplateService<T>
    {
        public ISubject<RenderFragment<T>> DetailRowTemplate { get; }
        public ISubject<RenderFragment<T>> EditFormTemplate { get; }
        public ISubject<RenderFragment<T>> InsertFormTemplate { get; }
    }
}
