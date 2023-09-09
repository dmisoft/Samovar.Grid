using Microsoft.AspNetCore.Components;
using System;
using System.Text.RegularExpressions;

namespace Samovar.DataGrid
{
    public partial class GridCommandColumn
    {
        [CascadingParameter(Name = "datagrid-gridcolumnservice")]
        private GridColumnService GridColumnService { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Width { get; set; }

        [Parameter]
        public bool NewButtonVisible { get; set; } = true;

        [Parameter]
        public bool EditButtonVisible { get; set; } = true;

        [Parameter]
        public bool DeleteButtonVisible { get; set; } = true;

        public Guid guid { get; } = Guid.NewGuid();
        internal ColumnMetadata colMeta;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            int absoluteWidth = 0;
            ColumnMetadataWidthInfo widthInfo = new ColumnMetadataWidthInfo();
            bool isAbsoluteWidth = !string.IsNullOrEmpty(Width) && Regex.IsMatch(this.Width, "^[^0][0-9]*px$");
            bool isRelativeWidth = !string.IsNullOrEmpty(Width) && Regex.IsMatch(this.Width, @"^[^0][0-9]*\*$");
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
                ColumnType = GridColumnType.Command,
                Title = "",
                WidthInfo = widthInfo,
                VisibleAbsoluteWidthValue = absoluteWidth,
                EditButtonVisible = EditButtonVisible,
                DeleteButtonVisible=DeleteButtonVisible,
                NewButtonVisible = NewButtonVisible
            };
            GridColumnService.RegisterColumn(guid, colMeta);
        }
    }
}
