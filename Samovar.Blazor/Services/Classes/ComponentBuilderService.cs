using Microsoft.AspNetCore.Components;
using Samovar.Blazor.Edit;
using System;

namespace Samovar.Blazor
{
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

        public RenderFragment GetInsertingPopup<U>(SmDataGridRowModel<U> model)
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

        public RenderFragment GetEditingPopup<U>(SmDataGridRowModel<U> model)
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
        public RenderFragment GetInsertingForm<U>(SmDataGridRowModel<U> model)
        {
            RenderFragment rf = (builder) =>
            {
                builder.OpenComponent(0, typeof(GridRowInserting_Form<U>));
                builder.AddAttribute(1, "RowModel", model);
                builder.CloseComponent();
            };

            return rf;
        }

        public RenderFragment GetRow<U>(SmDataGridRowModel<U> model)
        {
            switch (model.RowState)
            {
                case SmDataGridRowState.Editing:
                    if (_navigationService.NavigationMode.Value == DataGridNavigationMode.VirtualScrolling)
                    {
                        return GetDefaultRow(model);
                    }
                    else if (_navigationService.NavigationMode.Value == DataGridNavigationMode.Paging)
                    {
                        switch (_editingService.EditMode.Value)
                        {
                            case DataGridEditMode.Form:
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
                            case DataGridEditMode.Popup:
                                return GetDefaultRow(model);
                        }
                    }
                    return GetDefaultRow(model);
                default:
                    return GetDefaultRow(model);
            }

        }


        private RenderFragment GetDefaultRow<U>(SmDataGridRowModel<U> model)
        {
            RenderFragment rf = (builder) =>
               {
                   builder.OpenComponent(0, typeof(SmDataGridRowDefault<U>));
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
                builder.OpenComponent(0, typeof(SmDataGridLoadingSpinner));
                builder.CloseComponent();
            };
            return rf;

        }

        public RenderFragment GetDataPanel<T>()
        {
            RenderFragment rf = (builder) =>
            {
                builder.OpenComponent(0, typeof(DataGridPagingTablePanel<T>));
                builder.CloseComponent();
            };
            return rf;
        }

        public RenderFragment GetPagingPanel<T>()
        {
            RenderFragment rf = (builder) =>
            {
                builder.OpenComponent(0, typeof(DataGridPagingFooter));
                builder.CloseComponent();
            };
            return rf;
        }

        public ValueTask DisposeAsync()
        {
            return new ValueTask(Task.CompletedTask);
        }
    }
}