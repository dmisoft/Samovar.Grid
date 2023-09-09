using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public class RowDetailService<T>
        : IRowDetailService<T>, IDisposable
    {
		public ISubject<IList<T>> ExpandedRowDetails { get; } = new ParameterSubject<IList<T>>(new List<T>());

        public Task ExpandOrCloseRowDetails(T dataItem)
        {
            if (!ExpandedRowDetails.SubjectValue.Contains(dataItem))
                ExpandedRowDetails.SubjectValue.Add(dataItem);
            else
                ExpandedRowDetails.SubjectValue.Remove(dataItem);

            return Task.CompletedTask;
        }

        public Task ExpandRowDetails(T dataItem)
        {
            if (!ExpandedRowDetails.SubjectValue.Contains(dataItem))
                ExpandedRowDetails.SubjectValue.Add(dataItem);

            return Task.CompletedTask;
        }

        public Task CloseRowDetails(T dataItem)
        {
            if (!ExpandedRowDetails.SubjectValue.Contains(dataItem))
                ExpandedRowDetails.SubjectValue.Remove(dataItem);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }

		
	}
}
