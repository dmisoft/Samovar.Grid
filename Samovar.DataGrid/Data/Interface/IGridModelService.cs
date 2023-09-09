using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Samovar.DataGrid
{
    internal interface IGridModelService<T>
        : IDisposable
    {
        GridRowModel<T> SingleSelectedRowModel { get; set; }
        T SingleSelectedDataRow { get; set; }
        IEnumerable<T> MultipleSelectedDataRows { get; set; }
        void Init(IEnumerable<T> data, Dictionary<Guid, ColumnMetadata> columnMetadataList);
        IGridDataRepository<T> Repository { get; set; }
        Dictionary<string, PropertyInfo> PropInfo { get; }
        Dictionary<Guid, ColumnMetadata> ColumnMetadataList { get; set; }
        void InitRepository(IEnumerable<T> data);
        List<GridRowModel<T>> ViewCollection { get; set; }
        void LoadViewCollection_V6(int actualPage, int pageSize, GridFilterMode filterMode, string sortingColumn, bool? ascending);
        Task LoadViewCollection(int pageNumber, int pageSize, GridFilterMode filterMode, string sortingColumn, bool? ascending);
        Task LoadVirtualViewCollection(int skip, int take);
        //void InsertItemsByScroll(int shouldDummyItems, int skip, int take, string sortingColumn, bool? ascending, bool scrollToTop, int itemsToRemove, int bottomDummies, GridFilterMode filterMode);
        //void SortData(string sortingColumn, bool ascending, GridFilterMode filterMode);
        void Clear();

        //Filter
        Dictionary<string, FilterCellInfo> FilterData { get; }
        void SetFilterData(string fieldName, FilterCellInfo filterCellInfo);
        void ResetFilter();
        int DataItemsCount { get; }
        int TotalDataItemsCount { get; }
        int CurrentSelectedItemIndex { get; set; }
        Guid CurrentSelectedDataItemId { get; set; }

        int virtualScroll_VisibleItems { get; set; }
        int virtualScroll_ItemsToShow { get; set; }
        double virtualScroll_RowHeight { get; set; }
        double virtualScroll_DummyRowHeight { get; set; }
        int virtualScroll_DummyItemsCount { get; set; }

        int virtualScroll_TopVisibleDataItemPosition { get; set; }
        int virtualScroll_StartGridItemPosition { get; set; }
        int virtualScroll_EndGridItemPosition { get; set; }
        Func<T, bool> CustomFilter { get; set; }

        void CreateColumnsMetadata();

        Task ArrowDown();
        Task ArrowUp();
        bool Pageable { get; set; }
        string innerGridId { get; set; }
        GridFilterMode FilterMode { get; set; }
    }
}
