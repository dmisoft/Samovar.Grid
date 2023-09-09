using System;
using System.Collections.Generic;
using System.Reflection;

namespace Samovar.DataGrid
{
    internal static class RowModelHelper<T>
    {
        public static List<GridRowModel<T>> CreateRowModelList(IEnumerable<T> gridData, Dictionary<Guid, ColumnMetadata> columnMetadata, Dictionary<string, PropertyInfo> propInfo, int startDataItemPosition)
        {
            var retVal = new List<GridRowModel<T>>();
            int rowPosition = startDataItemPosition;
            foreach (var pair in gridData)
            {
                retVal.Add(new GridRowModel<T>(pair, columnMetadata, rowPosition, propInfo));
                rowPosition++;
            }

            return retVal;
        }

        public static T CloneRowModelData(T sourceData, T targetData)
        {
            foreach (PropertyInfo pi in sourceData.GetType().GetProperties())
            {
                if (pi.CanWrite)
                    pi.SetValue(targetData, pi.GetValue(sourceData));
            }
            return targetData;
        }

        public static T CloneRowModelData(T sourceData)
        {
            T retVal = (T)Activator.CreateInstance(typeof(T));

            foreach (PropertyInfo pi in sourceData.GetType().GetProperties())
            {
                if (pi.CanWrite)
                    pi.SetValue(retVal, pi.GetValue(sourceData));
            }
            return retVal;
        }

        //public static List<GridRowDummyModel<T>> CreateRowDummyModelList(int dummyRows, Dictionary<string, PropertyInfo> propInfo, Dictionary<Guid, ColumnMetadata> columnMetadata)
        //{
        //    var retVal = new List<GridRowDummyModel<T>>();
        //    int rowPosition = 0;
        //    for (int i = 0; i < dummyRows; i++)
        //    {
        //        rowPosition++;
        //        retVal.Add(new GridRowDummyModel<T>(rowPosition, columnMetadata, propInfo));
        //    }

        //    return retVal;
        //}
    }
}
