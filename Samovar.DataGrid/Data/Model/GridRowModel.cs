using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Samovar.DataGrid
{
    public class GridRowModel<T>
        : IDisposable
    {

        public event Func<Task> NotifyAfterLoadData;
        public async Task RaiseNotifyAfterLoadData()
        {
            if (NotifyAfterLoadData != null)
            {
                await NotifyAfterLoadData.Invoke();
            }
        }

        public string HtmlElementId { get; } = $"sdatagridrow{Guid.NewGuid().ToString().Replace("-", "")}";

        public bool IsLoaded { get; set; }
        internal GridRowStatee RowState { get; set; } = GridRowStatee.Idle;
        
        //Item position in CollectionView
        public int DataItemPosition { get; set; }
        public int DataItemIndex { get => DataItemPosition - 1; }
        public bool RowSelected { get; set; }
        internal GridRowInnerModel<T> RowModel { get; set; }
        internal GridRowInnerModel<T> EditingRowModel { get; set; }

        public readonly T dataItem;

        public readonly Dictionary<Guid, ColumnMetadata> ColumnMetadata;
        public readonly Dictionary<string, PropertyInfo> PropDict;

        #region ctor
        
        public GridRowModel(T keyDataPair, Dictionary<Guid, ColumnMetadata> columnMetadata, int dataItemPosition, Dictionary<string, PropertyInfo> propDict)
        {
            DataItemPosition = dataItemPosition;
            dataItem = keyDataPair;
            ColumnMetadata = columnMetadata;
            PropDict = propDict;

            CancellationTokenSource = new CancellationTokenSource();
            Token = CancellationTokenSource.Token;
        }

        #endregion 

        internal List<GridRowCellModel<T>> CreateGridRowCellModelCollection(Dictionary<Guid, ColumnMetadata> columnMetadata, Dictionary<string, PropertyInfo> propDict, T dataItem)//, CancellationToken token)
        {
            List<GridRowCellModel<T>> gridCellModelCollection = new List<GridRowCellModel<T>>();

            foreach (var cm in columnMetadata.Values.Where(c => c.ColumnType == GridColumnType.Data))
            {
                //if (token.IsCancellationRequested)
                //{
                //    token.ThrowIfCancellationRequested();
                //}
                if (!string.IsNullOrEmpty(cm.Field))
                    gridCellModelCollection.Add(new GridRowCellModel<T>(dataItem, propDict[cm.Field], cm));
                else
                    gridCellModelCollection.Add(new GridRowCellModel<T>(dataItem, cm));
            }
            return gridCellModelCollection;
        }

        internal List<GridRowCellModel<T>> CreateGridRowCellModelCollection2(T dataItem)
        {
            List<GridRowCellModel<T>> gridCellModelCollection = new List<GridRowCellModel<T>>();

            foreach (var cm in ColumnMetadata.Values.Where(c => c.ColumnType == GridColumnType.Data))
            {
                if (!string.IsNullOrEmpty(cm.Field))
                    gridCellModelCollection.Add(new GridRowCellModel<T>(dataItem, PropDict[cm.Field], cm));
                else
                    gridCellModelCollection.Add(new GridRowCellModel<T>(dataItem, cm));
            }
            return gridCellModelCollection;
        }

        internal void RefreshGridRowCellModelCollection()
        {
            RowModel.GridCellModelCollection.Clear();
            
            foreach (var cm in ColumnMetadata.Values.Where(c => c.ColumnType == GridColumnType.Data))
            {
                if (!string.IsNullOrEmpty(cm.Field))
                    RowModel.GridCellModelCollection.Add(new GridRowCellModel<T>(dataItem, PropDict[cm.Field], cm));
                else
                    RowModel.GridCellModelCollection.Add(new GridRowCellModel<T>(dataItem, cm));
            }
        }

        public void CancelLoading()
        {
            CancellationTokenSource?.Cancel();
        }

        //TODO rename to StartEdit() or EditBegin()
        internal void CreateEditingModel()
        {
            EditingRowModel = CreateRowModel(RowModelHelper<T>.CloneRowModelData(RowModel.Data), ColumnMetadata, DataItemPosition);
        }

        private GridRowInnerModel<T> CreateRowModel(T dataItem, Dictionary<Guid, ColumnMetadata> columnMetadata, int rowPosition)
        {
            return new GridRowInnerModel<T>
            {
                Data = dataItem,
                GridCellModelCollection = CreateGridRowCellModelCollection(columnMetadata, PropDict, dataItem)
            };
        }

        private CancellationToken Token { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; private set; }

        internal Task<GridRowModel<T>> GetLoadTask()
        {
            return Task.Run(() =>
            {
                if (Token.IsCancellationRequested)
                {
                    return this;
                }
                RowModel = new GridRowInnerModel<T>
                {
                    Data = dataItem,
                    GridCellModelCollection = CreateGridRowCellModelCollection2(dataItem)
                };
                IsLoaded = true;
                return this;
            });
        }

        public override string ToString()
        {
            return dataItem.ToString();
        }

        public void Dispose()
        {
            if (CancellationTokenSource != null && !Token.IsCancellationRequested)
            {
                CancellationTokenSource.Cancel();
            }

            RowModel?.GridCellModelCollection?.Clear();
        }
    }
}
