using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Samovar.Grid.Edit;

public partial class GridEditCellBoolean
    : DesignComponentBase
{
    [SmInject]
    public required ILayoutService LayoutService { get; set; }

    [Parameter]
    public required object Data { get; set; }

    [Parameter]
    public required PropertyInfo PropInfo { get; set; }

    protected string _formSelectSizeClass = "";

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

    protected override Task OnInitializedAsync()
    {
        InnerValue = (bool?)PropInfo.GetValue(Data) ?? false;
        LayoutService.SizeMode.Subscribe(mode =>
        {
            _formSelectSizeClass = mode switch { GridSizeMode.Small => "form-select-sm", GridSizeMode.Large => "form-select-lg", _ => "" };
            _ = InvokeAsync(StateHasChanged);
        });
        if (InnerValue)
        {
            internalValue = TrueValue;
        }
        else
        {
            internalValue = FalseValue;
        }
        return base.OnInitializedAsync();
    }

    public void InnerValueOnChange(ChangeEventArgs args)
    {
        PropInfo.SetValue(Data, innerValue);
    }
}
