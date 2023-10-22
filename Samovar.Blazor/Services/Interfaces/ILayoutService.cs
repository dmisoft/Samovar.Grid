using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public interface ILayoutService
    {
        DotNetObjectReference<ILayoutService> DataGridDotNetRef { get; }
        BehaviorSubject<string> SelectedRowClass { get; }
        BehaviorSubject<string> TableTagClass { get; }
        BehaviorSubject<string> TheadTagClass { get; }
        BehaviorSubject<string> PaginationClass { get; }

        BehaviorSubject<string> FilterToggleButtonClass { get; }

        BehaviorSubject<double> MinGridWidth { get; }
        BehaviorSubject<bool> ShowDetailRow { get; }
        BehaviorSubject<DataGridFilterMode> FilterMode { get; }
        BehaviorSubject<bool> ShowFilterRow { get; }
        
        double ScrollbarWidth { get; }
        Task<double> ScrollbarWidth2();
        
        double FilterRowHeight { get; }

        Task<double> TableRowHeight();

        BehaviorSubject<string> Height { get; }
        BehaviorSubject<string> Width {  get; }

        BehaviorSubject<string> OuterStyle { get; }
        BehaviorSubject<string> FooterStyle { get; }

        ElementReference GridFilterRef { get; set; }
        ElementReference GridOuterRef { get; set; }
        ElementReference GridInnerRef { get; set; }
        ElementReference TableBodyInnerRef { get; set; }
        
        double ActualScrollbarWidth { get; set; }
        //string innerGridBodyId { get; }

        BehaviorSubject<bool> ShowColumnHeader { get; }
        BehaviorSubject<bool> ShowDetailHeader { get; }

        Task InitHeader();

        Task<bool> CheckScrollBarWidth();

        event Func<DataGridStyleInfo, Task> DataGridInnerCssStyleChanged;

        bool FitColumnsToTableWidth { get; set; }
        double GridColWidthSum { get; set; }
    }
}
