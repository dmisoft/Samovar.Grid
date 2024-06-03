using Microsoft.AspNetCore.Components;
using Samovar.Grid.Edit;

namespace Samovar.Grid;

public class ComponentBuilderService<T1>
    : IComponentBuilderService, IAsyncDisposable
{
    private readonly ITemplateService<T1> _templateService;
    private readonly IEditingService<T1> _editingService;
    private readonly INavigationService _navigationService;

    public ComponentBuilderService(
        ITemplateService<T1> templateService
        , IEditingService<T1> editingService
        , INavigationService navigationService
        )
    {
        _templateService = templateService;
        _editingService = editingService;
        _navigationService = navigationService;
    }

    public RenderFragment GetInsertingPopup<U>(GridRowModel<U> model)
    {
        if (_templateService.EditFormTemplate.Value != null)
        {
            return (builder) =>
            {
                builder.OpenComponent(0, typeof(GridRowInserting_PopupTemplate<U>));
                builder.AddAttribute(1, "RowModel", model);
                builder.AddAttribute(2, "Template", _templateService.InsertFormTemplate.Value);
                builder.CloseComponent();
            };
        }
        else
        {
            return (builder) =>
            {
                builder.OpenComponent(0, typeof(GridRowInserting_Popup<U>));
                builder.AddAttribute(1, "RowModel", model);
                builder.CloseComponent();
            };
        }
    }

    public RenderFragment GetEditingPopup<U>(GridRowModel<U> model)
    {
        RenderFragment? rf;

        if (_templateService.EditFormTemplate.Value != null)
        {
            rf = (builder) =>
            {
                builder.OpenComponent(0, typeof(GridRowEditing_PopupTemplate<U>));
                builder.AddAttribute(1, "RowModel", model);
                builder.AddAttribute(2, "Template", _templateService.EditFormTemplate.Value);
                builder.CloseComponent();
            };
        }
        else
        {
            rf = (builder) =>
            {
                builder.OpenComponent(0, typeof(GridRowEditing_Popup<U>));
                builder.AddAttribute(1, "RowModel", model);
                builder.CloseComponent();
            };
        }

        return rf;
    }
    public RenderFragment GetInsertingForm<U>(GridRowModel<U> model)
    {
        RenderFragment rf = (builder) =>
        {
            builder.OpenComponent(0, typeof(GridRowInserting_Form<U>));
            builder.AddAttribute(1, "RowModel", model);
            builder.CloseComponent();
        };

        return rf;
    }

    public RenderFragment GetRow<U>(GridRowModel<U> model)
    {
        switch (model.RowState)
        {
            case GridRowState.Editing:
                if (_navigationService.NavigationMode.Value == NavigationMode.VirtualScrolling)
                {
                    return GetDefaultRow(model);
                }
                else if (_navigationService.NavigationMode.Value == NavigationMode.Paging)
                {
                    switch (_editingService.EditMode.Value)
                    {
                        case GridEditMode.Form:
                            if (_templateService.EditFormTemplate.Value != null)
                            {
                                return (builder) =>
                                {
                                    builder.OpenComponent(0, typeof(GridRowEditing_FormTemplate<U>));
                                    builder.AddAttribute(1, "RowModel", model);
                                    builder.CloseComponent();
                                };
                            }
                            else
                            {
                                return (builder) =>
                                {
                                    builder.OpenComponent(0, typeof(GridRowEditing_Form<U>));
                                    builder.AddAttribute(1, "RowModel", model);
                                    builder.CloseComponent();
                                };
                            }
                        case GridEditMode.Popup:
                            return GetDefaultRow(model);
                    }
                }
                return GetDefaultRow(model);
            default:
                return GetDefaultRow(model);
        }

    }


    private RenderFragment GetDefaultRow<U>(GridRowModel<U> model)
    {
        RenderFragment rf = (builder) =>
           {
               builder.OpenComponent(0, typeof(GridRowDefault<U>));
               builder.AddAttribute(1, "RowModel", model);
               builder.CloseComponent();
           };
        return rf;
    }

    public RenderFragment GetNoDataPanel()
    {
        RenderFragment rf = (builder) =>
        {
            builder.OpenComponent(0, typeof(NoDataPanel));
            builder.CloseComponent();
        };
        return rf;
    }

    public RenderFragment GetNoDataFoundPanel()
    {
        throw new NotImplementedException();
    }

    public RenderFragment GetProcessingDataPanel()
    {
        RenderFragment rf = (builder) =>
        {
            builder.OpenComponent(0, typeof(LoadingSpinner));
            builder.CloseComponent();
        };
        return rf;

    }

    public RenderFragment GetDataPanel<T>()
    {
        RenderFragment rf = (builder) =>
        {
            builder.OpenComponent(0, typeof(PagingGrid<T>));
            builder.CloseComponent();
        };
        return rf;
    }

    public RenderFragment GetPagingPanel<T>()
    {
        RenderFragment rf = (builder) =>
        {
            builder.OpenComponent(0, typeof(PagingFooter));
            builder.CloseComponent();
        };
        return rf;
    }

    public ValueTask DisposeAsync()
    {
        return new ValueTask(Task.CompletedTask);
    }
}