namespace Samovar.DataGrid
{
    public class GridRowCommandType 
        : Enumeration
    {
        public static GridRowCommandType Edit = new GridRowCommandType(1, "Edit", "oi-pencil");
        public static GridRowCommandType Delete = new GridRowCommandType(2, "Delete", "oi-circle-x");

        public string CssClass { get; set; }
        public GridRowCommandType(int id, string name, string cssClass)
            : base(id, name)
        {
            CssClass = $"btn btn-sm btn-outline-secondary oi mx-1 {cssClass}";
        }
    }
}
