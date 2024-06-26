using Microsoft.AspNetCore.Components;
using System.Reactive.Subjects;

namespace Samovar.Grid;

public class TemplateService<T>
    : ITemplateService<T>
{
    public BehaviorSubject<RenderFragment<T>?> DetailRowTemplate { get; } = new BehaviorSubject<RenderFragment<T>?>(null);
    public BehaviorSubject<RenderFragment<T>?> EditFormTemplate { get; } = new BehaviorSubject<RenderFragment<T>?>(null);
		public BehaviorSubject<RenderFragment<T>?> EditTitleTemplate { get; } = new BehaviorSubject<RenderFragment<T>?>(null);
		public BehaviorSubject<RenderFragment<T>?> InsertFormTemplate { get; } = new BehaviorSubject<RenderFragment<T>?>(null);
	}
