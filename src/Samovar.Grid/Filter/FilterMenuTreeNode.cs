namespace Samovar.Grid.Filter;

public class FilterMenuTreeNode
{
    public string Label { get; set; } = "";
    public object? RawValue { get; set; }
    public List<FilterMenuTreeNode> Children { get; set; } = [];
    public bool IsExpanded { get; set; } = true;
    public bool IsChecked { get; set; } = false;

    public bool IsIndeterminate => Children.Count > 0
        && Children.Any(c => c.IsChecked || c.IsIndeterminate)
        && !Children.All(c => c.IsChecked && !c.IsIndeterminate);

    public bool IsLeaf => Children.Count == 0;

    public void SetCheckedRecursive(bool value)
    {
        IsChecked = value;
        foreach (var child in Children)
            child.SetCheckedRecursive(value);
    }

    public void UpdateCheckedFromChildren()
    {
        if (IsLeaf) return;
        foreach (var child in Children)
            child.UpdateCheckedFromChildren();
        IsChecked = Children.All(c => c.IsChecked);
    }
}
