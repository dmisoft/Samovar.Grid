namespace Samovar.Grid.Filter;

public partial class GridFilterCellBoolean
    : GridFilterCellBase<bool?>
{
    protected string NotDefinedValue = "null";
    protected string TrueValue = true.ToString();
    protected string FalseValue = false.ToString();

    protected override async Task FilterService_FilterCleared()
    {
        await base.FilterService_FilterCleared();
        InnerValue = null;
        internalValue = NotDefinedValue;
    }

    private string internalValue = "null";

    protected string InternalValue
    {
        get { return internalValue; }
        set
        {
            internalValue = value;
            if (value == TrueValue)
            {
                InnerValue = true;
            }
            else if (value == FalseValue)
            {
                InnerValue = false;
            }
            else
            {
                InnerValue = null;
            }
        }
    }
}
