using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Samovar.DataGrid
{
    internal interface IGridDataRepository<T>
    {
        List<T> Data { get; }
        PageableViewCollectionInfo<T> GetPageData_V6(Dictionary<string, FilterCellInfo> filterData, int pageNumber, int pageSize, string sortingColumn, bool? ascending, Func<T,bool> customFilter, GridFilterMode filterMode);
        Task<PageableViewCollectionInfo<T>> GetPageData(Dictionary<string, FilterCellInfo> filterData, int pageNumber, int pageSize, string sortingColumn, bool? ascending, Func<T,bool> customFilter, GridFilterMode filterMode);
        Task<IEnumerable<T>> GetDataForVirtualPage(int skip, int take);
        Task<T> GetDataAtPositionAsync(int position);
        T GetDataAtPosition(int position);
        void InsertItems(IList itemsToInsert, int newStartingIndex);
        void RemoveItems(IList itemsToRemove);
        void RemoveAllItems();
        event Func<List<T>, Task> OnNotifyDataLoaded;
        Task NotifyDataLoaded(List<T> data);
        IQueryable<T> CustomFilter(Func<T,bool> customFilter);
        IQueryable<T> Filter(Dictionary<string, FilterCellInfo> filterData);
        
        //TODO optimieren
        //braucht man 2 GetData Funktionen?
        (IEnumerable<T>,int FilteredDataCount) GetData(int skip, int take, string sortingColumn, bool? ascending, Dictionary<string, FilterCellInfo> filterData, Func<T, bool> customFilter, GridFilterMode filterMode);
        (IEnumerable<T>, int) GetData(string sortingColumn, bool? ascending, Dictionary<string, FilterCellInfo> filterData, Func<T, bool> customFilter, GridFilterMode filterMode);
    }
}
