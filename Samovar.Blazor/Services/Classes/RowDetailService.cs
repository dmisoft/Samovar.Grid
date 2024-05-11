using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public class RowDetailService<T>
        : IRowDetailService<T>, IDisposable
    {
        public BehaviorSubject<IList<T>> ExpandedRowDetails { get; } = new BehaviorSubject<IList<T>>(new List<T>());

        public Task ExpandOrCloseRowDetails(T dataItem)
        {
            if (!ExpandedRowDetails.Value.Contains(dataItem))
                ExpandedRowDetails.Value.Add(dataItem);
            else
                ExpandedRowDetails.Value.Remove(dataItem);

            return Task.CompletedTask;
        }

        public Task ExpandRowDetails(T dataItem)
        {
            if (!ExpandedRowDetails.Value.Contains(dataItem))
                ExpandedRowDetails.Value.Add(dataItem);

            return Task.CompletedTask;
        }

        public Task CloseRowDetails(T dataItem)
        {
            if (!ExpandedRowDetails.Value.Contains(dataItem))
                ExpandedRowDetails.Value.Remove(dataItem);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }


    }
}
