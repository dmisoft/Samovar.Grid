using System.Collections.Generic;

namespace Samovar.DataGrid
{
    internal class PageableViewCollectionInfo<T>
    {
        public IEnumerable<T> PageData { get; set; }
        public int PageDataCount { get; set; }
        public int FilteredDataCount { get; set; }
    }
}
