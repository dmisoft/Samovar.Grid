using CommonServiceLocator;
using Microsoft.AspNetCore.Components;
using Samovar.DataGrid.Data.Interface;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Samovar.DataGrid
{
    public partial class GridColumn
    {
        [CascadingParameter(Name = "datagrid-gridcolumnservice")]
        private GridColumnService GridColumnService { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string Field { get; set; }

        [Parameter]
        public string Width { get; set; }

        [Parameter]
        public RenderFragment<object> CellShowTemplate { get; set; }

        [Parameter]
        public RenderFragment<object> CellEditTemplate { get; set; }

        public Guid guid { get; } = Guid.NewGuid();

        internal ColumnMetadata colMeta;
        protected override void OnInitialized()
        {
            //var srv = (MichelService)ServiceLocator.Current.GetInstance<IMichelService>();

            //await InvokeAsync(() =>
            {
                int absoluteWidth = 0;
                ColumnMetadataWidthInfo widthInfo = new ColumnMetadataWidthInfo();
                bool isAbsoluteWidth = !string.IsNullOrEmpty(Width) && Regex.IsMatch(Width, "^[^0][0-9]*px$");
                bool isRelativeWidth = !string.IsNullOrEmpty(Width) && Regex.IsMatch(Width, @"^[^0][0-9]*\*$");
                if (isAbsoluteWidth)
                {
                    widthInfo.WidthMode = ColumnMetadataWidthInfo.ColumnWidthMode.Absolute;
                    widthInfo.WidthValue = int.Parse(Width.Replace("px", ""));
                }
                else if (isRelativeWidth)
                {
                    widthInfo.WidthMode = ColumnMetadataWidthInfo.ColumnWidthMode.Relative;
                    widthInfo.WidthValue = int.Parse(Width.Replace("*", ""));
                }

                colMeta = new ColumnMetadata
                {
                    Id = guid,
                    ColumnType = GridColumnType.Data,
                    Title = String.IsNullOrEmpty(Title) ? Field : Title,
                    Field = Field,
                    WidthInfo = widthInfo,
                    VisibleAbsoluteWidthValue = absoluteWidth,
                };
                if (!string.IsNullOrEmpty(GridColumnService.SortingColumn) && Field == GridColumnService.SortingColumn)
                {
                    colMeta.SortingAscending = GridColumnService.SortingAscending;
                }
                GridColumnService.RegisterColumn(guid, colMeta);
                GridColumnService.CellTemplateList.Add(guid, new CellTemplateInfo { CellShowTemplate = CellShowTemplate, CellEditTemplate = CellEditTemplate });
            }
        }
    }
}
