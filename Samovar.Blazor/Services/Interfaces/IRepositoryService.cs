using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public interface IRepositoryService<T>
    {
        IEnumerable<SmDataGridRowModel<T>> ViewCollection { get; }

        BehaviorSubject<HashSet<T>> Data { get; }
        
        //event Func<IEnumerable<SmDataGridRowModel<T>>, Task> ViewCollectionChanged;
        IObservable<IEnumerable<SmDataGridRowModel<T>>> ViewCollectionObservable { get; }
        public Dictionary<string, PropertyInfo> PropInfo { get; }

        //ISubject<int> PageSize { get; }
        //ISubject<int> PagerSize { get; }
        //ISubject<int> CurrentPage { get; }
        //ISubject<int> PageCount { get; }

        //ISubject<DataGridPagerInfo> PagerInfo { get; set; }
        //ISubject<IQueryable<T>> TotalItemsCount { get; set; }

        void AttachViewCollectionSubscription();
        void DetachViewCollectionSubscription();

        //Task NavigateToNextPage();
        //Task NavigateToPreviousPage();
        //Task NavigateToPage(int pageNumber);

        //Task NavigateToNextPager();
        //Task NavigateToPreviousPager();
        //Task NavigateToStartPage();
        //Task NavigateToEndPage();
    }
}
