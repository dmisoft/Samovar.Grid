using Microsoft.AspNetCore.Components;
using System;
using System.Reflection;

namespace Samovar.Blazor
{
    public interface IDataColumnModel
        : IColumnModel
    {
        Type ColumnDataItemType { get; }

        PropertyInfo ColumnDataItemPropertyInfo { get; }//TODO?

        public ISubject<RenderFragment<object>> CellShowTemplate { get; }

        public ISubject<RenderFragment<object>> CellEditTemplate { get; }
        
        public ISubject<string> Field { get; }

        public ISubject<string> Title { get; }

        public bool? SortingAscending { get; set; }
    }
}
