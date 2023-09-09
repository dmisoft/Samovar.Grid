using Microsoft.AspNetCore.Components;
using Samovar.Blazor.Edit;
using System;

namespace Samovar.Blazor
{
    public class ComponentBuilderService<T1>
        : IComponentBuilderService, IDisposable
    {
        private ITemplateService<T1> _templateService;
        private IEditingService<T1> _editingService;
        private INavigationService _navigationService;

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
            RenderFragment rf = null;

            if (_templateService.EditFormTemplate.SubjectValue != null)
            {
                rf = (builder) =>
                {
                    builder.OpenComponent(0, typeof(GridRowInserting_PopupTemplate<U>));
                    builder.AddAttribute(1, "RowModel", model);
                    builder.AddAttribute(2, "Template", _templateService.InsertFormTemplate.SubjectValue);
                    builder.CloseComponent();
                };
            }
            else
            {
                rf = (builder) =>
                {
                    builder.OpenComponent(0, typeof(GridRowInserting_Popup<U>));
                    builder.AddAttribute(1, "RowModel", model);
                    builder.CloseComponent();
                };
            }

            return rf;
        }

        public RenderFragment GetEditingPopup<U>(SmDataGridRowModel<U> model)
        {
            RenderFragment rf = null;

            if (_templateService.EditFormTemplate.SubjectValue != null)
            {
                rf = (builder) =>
                {
                    builder.OpenComponent(0, typeof(GridRowEditing_PopupTemplate<U>));
                    builder.AddAttribute(1, "RowModel", model);
                    builder.AddAttribute(2, "Template", _templateService.EditFormTemplate.SubjectValue);
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
            RenderFragment rf = null;

            switch (model.RowState)
            {
                case SmDataGridRowState.Editing:
                    if (_navigationService.NavigationMode.SubjectValue == DataGridNavigationMode.VirtualScrolling)
                    {
                        rf = GetDefaultRow(model);
                    }
                    else if (_navigationService.NavigationMode.SubjectValue == DataGridNavigationMode.Paging)
                    {
                        switch (_editingService.EditMode.SubjectValue)
                        {
                            case GridEditMode.Form:
                                if (_templateService.EditFormTemplate.SubjectValue != null)
                                {
                                    rf = (builder) =>
                                    {
                                        builder.OpenComponent(0, typeof(GridRowEditing_FormTemplate<U>));
                                        builder.AddAttribute(1, "RowModel", model);
                                        builder.CloseComponent();
                                    };
                                }
                                else
                                {
                                    rf = (builder) =>
                                    {
                                        builder.OpenComponent(0, typeof(GridRowEditing_Form<U>));
                                        builder.AddAttribute(1, "RowModel", model);
                                        builder.CloseComponent();
                                    };
                                }
                                break;
                            case GridEditMode.Popup:
                                rf = GetDefaultRow(model);
                                break;
                        }
                    }

                    break;
                default:
                    rf = GetDefaultRow(model);
                    break;
            }

            return rf;
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


        public void Dispose()
        {
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
                builder.OpenComponent(0, typeof(DataGridBodyPanel<T>));
                builder.CloseComponent();
            };
            return rf;
        }

        public RenderFragment GetPagingPanel<T>()
        {
            RenderFragment rf = (builder) =>
            {
                builder.OpenComponent(0, typeof(DataGridPagingFooter<T>));
                builder.CloseComponent();
            };
            return rf;
        }

        //public RenderFragment<T> GetPagingPanel()
        //{
        //    RenderFragment rf = (builder) =>
        //    {
        //        builder.OpenComponent(0, typeof(DataGridPagingFooter<T>));
        //        builder.CloseComponent();
        //    };
        //    return rf;
        //}
    }
}