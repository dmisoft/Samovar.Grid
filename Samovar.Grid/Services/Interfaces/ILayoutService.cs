using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Reactive.Subjects;

namespace Samovar.Grid;

public interface ILayoutService
{
    bool OriginalColumnsWidthChanged { get; set; }
    DotNetObjectReference<ILayoutService> DataGridDotNetRef { get; }
    BehaviorSubject<string> SelectedRowClass { get; }
    BehaviorSubject<string> CssClass { get; }
    BehaviorSubject<string> PaginationClass { get; }

    BehaviorSubject<string> FilterToggleButtonClass { get; }

    BehaviorSubject<double> MinGridWidth { get; }
    BehaviorSubject<bool> ShowDetailRow { get; }
    BehaviorSubject<GridFilterMode> FilterMode { get; }
    BehaviorSubject<bool> ShowFilterRow { get; }
    BehaviorSubject<string> Height { get; }
    BehaviorSubject<string> Width { get; }

    BehaviorSubject<string> OuterStyle { get; }
    BehaviorSubject<string> FooterStyle { get; }

    ElementReference GridFilterRef { get; set; }
    ElementReference GridOuterRef { get; set; }
    ElementReference GridInnerRef { get; set; }
    ElementReference TableBodyInnerRef { get; set; }

    BehaviorSubject<bool> ShowColumnHeader { get; }
    BehaviorSubject<bool> ShowDetailHeader { get; }

    Task InitHeader();

    IObservable<Task<GridStyleInfo>> DataGridInnerStyle { get; }

    BehaviorSubject<GridColumnResizeMode> ColumnResizeMode { get; }
    double ActualColumnsWidthSum { get; }
}
