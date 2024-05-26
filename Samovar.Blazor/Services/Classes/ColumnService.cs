using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public class ColumnService
        : IColumnService
    {
        public IColumnModel EmptyColumnModel { get; } = new EmptyColumnModel();
		public IColumnModel EmptyHeaderColumnModel { get; } = new EmptyColumnModel();

		public IColumnModel DetailExpanderColumnModel { get; } = new DetailExpanderColumnModel();

        public List<IColumnModel> AllColumnModels { get; } = new List<IColumnModel>();

        public IEnumerable<IDataColumnModel> DataColumnModels => AllColumnModels.OfType<IDataColumnModel>();

		public Subject<IColumnModel> ColumnResizingEndedObservable { get; } = new();

		public void RegisterColumn(IColumnModel columntModel)
        {
            int _columnOrder = AllColumnModels.Count + 1;
            columntModel.Order = _columnOrder;
            AllColumnModels.Add(columntModel);
        }
    }
}
