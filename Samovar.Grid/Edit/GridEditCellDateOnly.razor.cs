using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Samovar.Grid.Edit;

public partial class GridEditCellDateOnly
{
    [Parameter]
    public required object Data { get; set; }

    [Parameter]
    public required PropertyInfo PropInfo { get; set; }

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

    protected override void OnInitialized()
    {
        base.OnInitialized();
        innerValue = (DateOnly?)PropInfo.GetValue(Data);
    }

    public void InnerValueOnChange(ChangeEventArgs args)
    {
        PropInfo.SetValue(Data, innerValue);
    }
}
