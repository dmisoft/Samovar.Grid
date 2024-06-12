using Microsoft.AspNetCore.Components;
using Samovar.Grid.Server.Test.Data;
using System.ComponentModel.DataAnnotations;

namespace Samovar.Grid.Server.Test.Pages;
public partial class Index
    : ComponentBase
{
    private List<WeatherForecast>? forecasts;
    protected FormEditContext? EditContext;
    SmGrid<WeatherForecast>? grid;

    WeatherForecast? selectedRow = null;
    uint pageSize = 10;
    string gridWidth = "1200px";
    string gridHeight = "600px";
    string columnWidth = "150px";
    string? cssClass;

    RowSelectionMode selectionMode = RowSelectionMode.Single;
    GridEditMode gridEditMode = GridEditMode.Form;
    GridColumnResizeMode columnResizeMode = GridColumnResizeMode.Sliding;
    IEnumerable<WeatherForecast> selectedRows = [];
    bool showDetailRow = true;

    async Task loadData()
    {
        forecasts = (await ForecastService.GetForecastAsync(DateTime.Now)).ToList();
    }

    Task changeStyle()
    {
        cssClass = "blueTable";
        return Task.CompletedTask;
    }

    Task RowEditBegin(WeatherForecast item)
    {
        return Task.CompletedTask;
    }

    Task RowInsertingHandler(WeatherForecast item)
    {
        forecasts ??= new List<WeatherForecast>();
        forecasts = ((new WeatherForecast[] { item }).Concat(forecasts)).ToList();
        return Task.CompletedTask;
    }

    Task RowRemovingHandler(WeatherForecast item)
    {
        forecasts = forecasts?.Except([item]).ToList();
        return Task.CompletedTask;
    }

    Task deselectSingleSelectedItem()
    {
        selectedRow = null;
        return Task.CompletedTask;
    }

    protected override Task OnInitializedAsync()
    {
        return base.OnInitializedAsync();
    }

    async Task collapseAllDetailRows() => await grid.CollapseAllDetailRows();

    protected class FormEditContext
    {
        public FormEditContext(WeatherForecast dataItem)
        {
            DataItem = dataItem;
        }

        public WeatherForecast DataItem { get; set; }

        [Required]
        [StringLength(maximumLength: 32, MinimumLength = 4, ErrorMessage = "The description should be 4 to 32 characters.")]
        public string Summary { get; set; } = string.Empty;
    }
}
