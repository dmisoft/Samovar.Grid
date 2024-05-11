namespace Samovar.Blazor
{
    public interface IColumnService
    {
        public List<IColumnModel> AllColumnModels { get; }
        public IEnumerable<IDataColumnModel> DataColumnModels { get; }
        public IColumnModel EmptyColumnModel { get; }
        public IColumnModel DetailExpanderColumnModel { get; }
        public void RegisterColumn(IColumnModel columntModel);//nur registrierbare Spalten
    }
}
