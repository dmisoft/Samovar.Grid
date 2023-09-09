using System;
using System.Collections.Generic;
using System.Linq;

namespace Samovar.DataGrid
{
    public class GridColumnService
    {
        internal Dictionary<Guid, ColumnMetadata> Columns { get; set; } = new Dictionary<Guid, ColumnMetadata>();
        internal Dictionary<Guid, CellTemplateInfo> CellTemplateList { get; set; } = new Dictionary<Guid, CellTemplateInfo>();
        internal Dictionary<Guid, List<GridRowCommandType>> ColumnCommandMetadataList { get; set; } = new Dictionary<Guid, List<GridRowCommandType>>();
        internal KeyValuePair<Guid, ColumnMetadata> DetailExpanderColumn { get; set; }

        #region Ordering
        //public bool SortingDesc { get; set; }
        internal bool? SortingAscending { get; set; } = null;
        internal string SortingColumn { get; set; } = string.Empty;
        #endregion

        #region ctor
        public GridColumnService()
        {
        }
        #endregion

        public void Init(string sortingFieldByDefault, bool sortingDesc)
        {
            //SortingDesc = orderDesc;
            if (!string.IsNullOrEmpty(sortingFieldByDefault))
            {
                SortingAscending = !sortingDesc;
                SortingColumn = sortingFieldByDefault;
            }

            //Define detail expander column
            double detailExpanderColWidth = 30;
            Guid detailExpanderId = Guid.NewGuid();
            ColumnMetadataWidthInfo widthInfo = new ColumnMetadataWidthInfo();
            widthInfo.WidthMode = ColumnMetadataWidthInfo.ColumnWidthMode.Absolute;
            widthInfo.WidthValue = detailExpanderColWidth;

            ColumnMetadata colMeta = new ColumnMetadata
            {
                Id = detailExpanderId,
                ColumnType = GridColumnType.DetailExpanderColumn,
                Title = "",
                WidthInfo = widthInfo,
                VisibleAbsoluteWidthValue = detailExpanderColWidth,
            };
            DetailExpanderColumn = new KeyValuePair<Guid, ColumnMetadata>(detailExpanderId, colMeta);
        }

        internal void RegisterColumn(Guid columnId, ColumnMetadata colMeta)
        {
            if (!Columns.ContainsKey(columnId))
            {
                colMeta.ColumnOrder = Columns.ToList().Count() + 1;
                Columns.Add(columnId, colMeta);
            }

            //FilterService.RegisterFilter<string>(colMeta);
        }

        internal void ReplaceColumn(Guid sourceColumnId, Guid targetColumnId)
        {
            int sourceColumnOrder = Columns[sourceColumnId].ColumnOrder;
            int targetColumnOrder = Columns[targetColumnId].ColumnOrder;

            List<ColumnMetadata> changeList = new List<ColumnMetadata>();

            if (sourceColumnOrder > targetColumnOrder)
            {
                for (int i = targetColumnOrder; i < sourceColumnOrder; i++)
                {
                    changeList.Add(Columns.Values.Single(c => c.ColumnOrder == i));
                }
                foreach (var colMeta in changeList)
                {
                    colMeta.ColumnOrder += 1;
                }
                Columns[sourceColumnId].ColumnOrder = targetColumnOrder;
            }
            else if (sourceColumnOrder < targetColumnOrder)
            {
                for (int i = sourceColumnOrder + 1; i <= targetColumnOrder; i++)
                {
                    changeList.Add(Columns.Values.Single(c => c.ColumnOrder == i));
                }
                foreach (var colMeta in changeList)
                {
                    colMeta.ColumnOrder -= 1;
                }
                Columns[sourceColumnId].ColumnOrder = targetColumnOrder;
            }
        }

        internal ColumnMetadata GetColumnMetadataById(Guid id)
        {
            return Columns[id];
        }

        internal void ResetOrder() {
            SortingColumn = string.Empty;
            Columns.Values.ToList().ForEach(c => c.SortingAscending = null);
        }
    }
}
