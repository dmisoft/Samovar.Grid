using System.ComponentModel;

namespace Samovar.Blazor
{
    public class DataGridColumnOrderInfo
    {
        public static readonly DataGridColumnOrderInfo Empty = new DataGridColumnOrderInfo { Field = "", Asc = true };

        public string Field { get; set; }
        public bool Asc { get; set; }
    }
}
