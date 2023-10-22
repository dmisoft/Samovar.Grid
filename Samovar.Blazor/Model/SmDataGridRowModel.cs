using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Samovar.Blazor {
    public interface IComponentModel
    {
        public string HtmlElementId { get; }
    }
    public interface IRowModel<T> 
        : IComponentModel {
        public T DataItem { get; set; }
        public T EditingDataItem { get; set; }
    }

    public class SmDataGridRowModel<T>
        : IDisposable, IRowModel<T>
    {
        public bool RowDetailExpanded { get; set; }

        public string HtmlElementId { get; } = $"sdatagridrow{Guid.NewGuid().ToString().Replace("-", "")}";

        internal SmDataGridRowState RowState { get; set; } = SmDataGridRowState.Idle;

        //Item position in CollectionView
        public int DataItemPosition { get; set; }
        public int DataItemIndex { get => DataItemPosition - 1; }
        public bool RowSelected { get; set; }
        public T DataItem { get; set; }
        public T EditingDataItem { get; set; }
        public List<DataGridRowCellModel<T>> GridCellModels { get; set; }
        public List<DataGridRowCellModel<T>> EditingGridCellModels { get; set; }

        public readonly IEnumerable<IDataColumnModel> ColumnMetadata;
        
        private readonly Dictionary<string, PropertyInfo> PropDict;

        #region ctor

        public SmDataGridRowModel(T dataItem, IEnumerable<IDataColumnModel> columnMetadata, int dataItemPosition, Dictionary<string, PropertyInfo> propDict, bool detailExpanded)
        {
            DataItemPosition = dataItemPosition;
            DataItem = dataItem;
            ColumnMetadata = columnMetadata;
            PropDict = propDict;
            RowDetailExpanded = detailExpanded;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            GridCellModels = CreateGridRowCellModelCollection2(dataItem);
            stopWatch.Stop();
        }

        #endregion 
        
        internal List<DataGridRowCellModel<T>> CreateGridRowCellModelCollection(IEnumerable<IDataColumnModel> columnMetadata, T dataItem)//, CancellationToken token)
        {
            List<DataGridRowCellModel<T>> gridCellModelCollection = new List<DataGridRowCellModel<T>>();

            foreach (var cm in columnMetadata.Where(c => c.ColumnType == DataGridColumnType.Data))
            {
                if (!string.IsNullOrEmpty(cm.Field.Value))
                    gridCellModelCollection.Add(new DataGridRowCellModel<T>(dataItem, PropDict[cm.Field.Value], cm));
                else
                    gridCellModelCollection.Add(new DataGridRowCellModel<T>(dataItem, cm));
            }
            return gridCellModelCollection;
        }

        internal List<DataGridRowCellModel<T>> CreateGridRowCellModelCollection2(T dataItem)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            List<DataGridRowCellModel<T>> gridCellModelCollection = new List<DataGridRowCellModel<T>>();

            foreach (var cm in ColumnMetadata)
            {
                if (!string.IsNullOrEmpty(cm.Field.Value))
                    gridCellModelCollection.Add(new DataGridRowCellModel<T>(dataItem, PropDict[cm.Field.Value], cm));
                else
                    gridCellModelCollection.Add(new DataGridRowCellModel<T>(dataItem, cm));
            }

            stopWatch.Stop();

            return gridCellModelCollection;
        }

        //TODO rename to StartEdit() or EditBegin()
        internal void CreateEditingModel()
        {
            EditingDataItem = CloneRowItem(DataItem);
            EditingGridCellModels = CreateGridRowCellModelCollection(ColumnMetadata, EditingDataItem);// CreateRowModel(CloneRowItem(RowModel.Data), ColumnMetadata);
        }
        internal void CommitEditingModel()
        {
            DataItem = CopyRowModelData(EditingDataItem, DataItem);
            GridCellModels = CreateGridRowCellModelCollection2(DataItem);
        }
        public static T CloneRowItem(T sourceData)
        {
            T retVal = (T)Activator.CreateInstance(typeof(T));

            foreach (PropertyInfo pi in sourceData.GetType().GetProperties())
            {
                if (pi.CanWrite)
                    pi.SetValue(retVal, pi.GetValue(sourceData));
            }
            return retVal;
        }

        public static T CopyRowModelData(T sourceData, T targetData)
        {
            foreach (PropertyInfo pi in sourceData.GetType().GetProperties())
            {
                if (pi.CanWrite)
                    pi.SetValue(targetData, pi.GetValue(sourceData));
            }
            return targetData;
        }

        private DataGridRowInnerModel<T> CreateRowModel(T dataItem, IEnumerable<IDataColumnModel> columnMetadata)
        {
            return new DataGridRowInnerModel<T>
            {
                Data = dataItem,
                GridCellModelCollection = CreateGridRowCellModelCollection(columnMetadata, dataItem)
            };
        }

        public void Dispose()
        {
            GridCellModels?.Clear();
            EditingGridCellModels?.Clear();
        }
    }
}
