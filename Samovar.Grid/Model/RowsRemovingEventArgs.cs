namespace Samovar.Grid;

public class RowsRemovingEventArgs<T>
{
    public RowsRemovingEventArgs(IEnumerable<T> items)
    {
        Items = new List<T>(items);
    }

    /// <summary>Consumer may remove items to narrow the deletion set.</summary>
    public List<T> Items { get; }

    /// <summary>Set to true to abort deletion entirely. Selection is NOT cleared.</summary>
    public bool Cancel { get; set; }
}
