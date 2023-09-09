using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public interface ILayoutService
    {
        DotNetObjectReference<ILayoutService> DataGridDotNetRef { get; }
        ISubject<string> SelectedRowClass { get; }
        ISubject<string> TableTagClass { get; }
        ISubject<string> TheadTagClass { get; }
        ISubject<string> PaginationClass { get; }

        ISubject<string> FilterToggleButtonClass { get; }

        ISubject<double> MinGridWidth { get; }
        ISubject<bool> ShowDetailRow { get; }
        ISubject<DataGridFilterMode> FilterMode { get; }
        IObservable<bool> ShowFilterRow { get; }
        
        double ScrollbarWidth { get; }
        Task<double> ScrollbarWidth2();
        
        double FilterRowHeight { get; }

        Task<double> TableRowHeight();

        ISubject<string> Height { get; }
        ISubject<string> Width {  get; }

        ISubject<string> OuterStyle { get; }
        ISubject<string> FooterStyle { get; }

        ElementReference GridFilterRef { get; set; }
        ElementReference GridOuterRef { get; set; }
        ElementReference GridInnerRef { get; set; }
        ElementReference TableBodyInnerRef { get; set; }
        
        double ActualScrollbarWidth { get; set; }
        //string innerGridBodyId { get; }

        ISubject<bool> ShowColumnHeader { get; }
        ISubject<bool> ShowDetailHeader { get; }

        Task InitHeader();

        Task<bool> CheckScrollBarWidth();

        event Func<DataGridStyleInfo, Task> DataGridInnerCssStyleChanged;

        bool FitColumnsToTableWidth { get; set; }
        double GridColWidthSum { get; set; }
    }
}
