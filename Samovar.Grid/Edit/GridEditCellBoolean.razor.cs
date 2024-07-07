using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Samovar.Grid.Edit;

public partial class GridEditCellBoolean
{
    [Parameter]
    public required object Data { get; set; }

    [Parameter]
    public required PropertyInfo PropInfo { get; set; }

    protected string NotDefinedValue = "null";
    protected string TrueValue = true.ToString();
    protected string FalseValue = false.ToString();

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
        }
    }

    private bool innerValue;
    protected bool InnerValue
    {
        set
        {
            innerValue = value;
            PropInfo.SetValue(Data, innerValue);
        }
        get
        {
            return innerValue;
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        InnerValue = (bool?)PropInfo.GetValue(Data) ?? false;
        if (InnerValue)
        {
            internalValue = TrueValue;
        }
        else
        {
            internalValue = FalseValue;
        }
    }

    public void InnerValueOnChange(ChangeEventArgs args)
    {
        PropInfo.SetValue(Data, innerValue);
    }
}
