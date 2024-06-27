using Microsoft.AspNetCore.Components;
using System.Reactive.Subjects;

namespace Samovar.Grid
{
    public interface ITemplateService<T>
    {
        public BehaviorSubject<RenderFragment<T>?> DetailRowTemplate { get; }
        public BehaviorSubject<RenderFragment<T>?> EditPopupTitleTemplate { get; }
        public BehaviorSubject<RenderFragment<T>?> EditFormTemplate { get; }
        public BehaviorSubject<RenderFragment<T>?> InsertFormTemplate { get; }
    }
}
