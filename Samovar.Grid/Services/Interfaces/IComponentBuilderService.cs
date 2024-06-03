using Microsoft.AspNetCore.Components;

namespace Samovar.Grid
{
    public interface IComponentBuilderService
    {
        RenderFragment GetEditingPopup<T>(GridRowModel<T> model);
        RenderFragment GetInsertingPopup<T>(GridRowModel<T> model);
        RenderFragment GetInsertingForm<T>(GridRowModel<T> model);
        RenderFragment GetRow<T>(GridRowModel<T> model);
        RenderFragment GetNoDataPanel();
        RenderFragment GetNoDataFoundPanel();
        RenderFragment GetProcessingDataPanel();
        RenderFragment GetDataPanel<T>();
        RenderFragment GetPagingPanel<T>();
    }
}
