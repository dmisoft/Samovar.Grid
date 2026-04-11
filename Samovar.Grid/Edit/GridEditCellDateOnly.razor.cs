using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Samovar.Grid.Edit;

public partial class GridEditCellDateOnly
    : DesignComponentBase
{
    [SmInject]
    public required ILayoutService LayoutService { get; set; }

    [Parameter]
    public required object Data { get; set; }

    [Parameter]
    public required PropertyInfo PropInfo { get; set; }

    protected string _formControlSizeClass = "";

    private DateOnly? innerValue = DateOnly.MinValue;
    protected DateOnly? InnerValue
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
        innerValue = (DateOnly?)PropInfo.GetValue(Data);
        LayoutService.SizeMode.Subscribe(mode =>
        {
            _formControlSizeClass = mode switch { GridSizeMode.Small => "form-control-sm", GridSizeMode.Large => "form-control-lg", _ => "" };
            StateHasChanged();
        });
        return base.OnInitializedAsync();
    }

    public void InnerValueOnChange(ChangeEventArgs args)
    {
        PropInfo.SetValue(Data, innerValue);
    }
}
