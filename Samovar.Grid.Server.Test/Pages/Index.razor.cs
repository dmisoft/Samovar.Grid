﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Samovar.Grid.Server.Test.Data;
using System.ComponentModel.DataAnnotations;

namespace Samovar.Grid.Server.Test.Pages
{
    public partial class Index
        : ComponentBase
    {
        private List<WeatherForecast> forecasts;
        FormEditContext EditContext = null;
        SmGrid<WeatherForecast> grid;

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
            cssClass = "minimalistBlack";
            return Task.CompletedTask;
        }

        Task RowEditBegin(WeatherForecast item)
        {
            return Task.CompletedTask;
        }

        void RowEditBeginHandler(WeatherForecast item)
        {
            EditContext = new FormEditContext(item);
        }

        Task<string> EditingFormTitle(WeatherForecast item)
        {
            return Task.FromResult(item.TemperatureC.ToString());
        }

        Task RowInsertingHandler(WeatherForecast item)
        {
            forecasts ??= new List<WeatherForecast>();
            forecasts = ((new WeatherForecast[] { item }).Concat(forecasts)).ToList();
            return Task.CompletedTask;
        }

        Task RowRemovingHandler(WeatherForecast item)
        {
            forecasts = forecasts.Except([item]).ToList();
            return Task.CompletedTask;
        }

        Task deselectSingleSelectedItem()
        {
            selectedRow = null;
            return Task.CompletedTask;
        }

        Task selectSingleSelectedItem()
        {
            selectedRow = forecasts.Skip(1).First();
            return Task.CompletedTask;
        }

        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }
        async Task collapseAllDetailRows()
        {
            await grid.CollapseAllDetailRows();
            //return Task.CompletedTask;
        }

        class FormEditContext
        {
            public FormEditContext(WeatherForecast dataItem)
            {
                DataItem = dataItem;
                //Summary = DataItem.Summary;
                //Region = DataItem.Region;
                //City = DataItem.City;
            }

            public WeatherForecast DataItem { get; set; }

            [Required]
            [StringLength(maximumLength: 32, MinimumLength = 4, ErrorMessage = "The description should be 4 to 32 characters.")]
            public string Summary { get; set; }

            //public Action StateHasChanged { get; set; }
        }

    }
}
