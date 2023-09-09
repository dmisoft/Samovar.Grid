using System.Collections.Generic;
using System.Linq;

namespace Samovar.Blazor
{
    public class ColumnService
        : IColumnService
    {
        public IColumnModel EmptyColumnModel { get; } = new EmptyColumnModel();

        public IColumnModel DetailExpanderColumnModel { get; } = new DetailExpanderColumnModel();

        public List<IColumnModel> AllColumnModels { get; } = new List<IColumnModel>();

        public IEnumerable<IDataColumnModel> DataColumnModels => AllColumnModels.OfType<IDataColumnModel>();

        //public IEnumerable<ICommandColumnModel> CommandColumnModels => AllColumnModels.OfType<ICommandColumnModel>();
        
        public ColumnService()
        {
        }

        public void RegisterColumn(IColumnModel columntModel)
        {
            int _columnOrder = AllColumnModels.Count + 1;
            columntModel.ColumnOrder = _columnOrder;
            AllColumnModels.Add(columntModel);
        }
    }
}
