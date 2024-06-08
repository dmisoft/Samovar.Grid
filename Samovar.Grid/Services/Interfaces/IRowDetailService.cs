﻿using System.Reactive.Subjects;

namespace Samovar.Grid
{
    public interface IRowDetailService<T>
    {
        BehaviorSubject<IList<T>> ExpandedRowDetails { get; }

        Task ExpandOrCloseRowDetails(T dataItem);

        Task ExpandRowDetails(T dataItem);

        Task CloseRowDetails(T dataItem);
    }
}