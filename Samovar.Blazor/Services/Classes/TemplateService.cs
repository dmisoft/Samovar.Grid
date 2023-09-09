using Microsoft.AspNetCore.Components;

namespace Samovar.Blazor
{
    public class TemplateService<T>
        : ITemplateService<T>
    {
        public ISubject<RenderFragment<T>> DetailRowTemplate { get; } = new ParameterSubject<RenderFragment<T>>();
        public ISubject<RenderFragment<T>> EditFormTemplate { get; } = new ParameterSubject<RenderFragment<T>>();
        public ISubject<RenderFragment<T>> InsertFormTemplate { get; } = new ParameterSubject<RenderFragment<T>>();
    }
}
