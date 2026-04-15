using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Samovar.Grid.Edit;

public partial class GridEditCellString
    : DesignComponentBase
{
    [SmInject]
    public required ILayoutService LayoutService { get; set; }

    [Parameter]
    public required object Data { get; set; }

    [Parameter]
    public required PropertyInfo PropInfo { get; set; }

    protected string _formControlSizeClass = "";

    private string? innerValue;
    protected string? InnerValue
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
        InnerValue = PropInfo.GetValue(Data)?.ToString();
        LayoutService.SizeMode.Subscribe(mode =>
        {
            _formControlSizeClass = mode switch { GridSizeMode.Small => "form-control-sm", GridSizeMode.Large => "form-control-lg", _ => "" };
            _ = InvokeAsync(StateHasChanged);
        });
        return base.OnInitializedAsync();
    }

    public void InnerValueOnChange(ChangeEventArgs args)
    {
        PropInfo.SetValue(Data, innerValue);
    }
}
