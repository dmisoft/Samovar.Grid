using Microsoft.AspNetCore.Components;

namespace Samovar.Blazor
{
    public interface IComponentBuilderService
    {
        RenderFragment GetEditingPopup<T>(SmDataGridRowModel<T> model);
        RenderFragment GetInsertingPopup<T>(SmDataGridRowModel<T> model);
        RenderFragment GetInsertingForm<T>(SmDataGridRowModel<T> model);
        RenderFragment GetRow<T>(SmDataGridRowModel<T> model);
        RenderFragment GetNoDataPanel();
        RenderFragment GetNoDataFoundPanel();
        RenderFragment GetProcessingDataPanel();
        RenderFragment GetDataPanel<T>();
        RenderFragment GetPagingPanel<T>();
    }
}
