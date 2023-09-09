using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Samovar.DataGrid
{
    internal static class DataItemExtensions
    {
        internal static List<GridRowCellModel<T>> ToCellCollection<T>(this T dataItem, Dictionary<Guid, ColumnMetadata> columnMetadata, Dictionary<string, PropertyInfo> propDict)
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

        internal static IEnumerable<T> ToCurrentViewCollection<T>(this IEnumerable<T> data, int pageNumber, int pageSize )
        {
            int skip = (pageNumber - 1) * pageSize;
            int take = pageSize;

            return data.Skip(skip).Take(take);
        }
    }
}
