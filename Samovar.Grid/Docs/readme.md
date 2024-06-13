﻿## Basic sample

### Add the following code to your _Layout.cshtml file
```csharp 
<head>
...
<link href="_content/SamovarGrid/samovar.grid.css" rel="stylesheet" />
...
</head>
```

### _Imports.razor
```csharp 
@using static Microsoft.AspNetCore.Components.Web.RenderMode
```

### Index.razor
#### add @rendermode InteractiveServer to your page
```csharp 
@page "/"
@rendermode InteractiveServer

@using Samovar.Grid.Server.Test.Data
@using System.ComponentModel.DataAnnotations
@using System
@using System.Diagnostics;

@inject WeatherForecastService ForecastService

<h1>Common tests</h1>

<div class="mb-3">
    <button class="btn btn-primary" @onclick="loadData">load data</button>
</div>

<div class="row">
    <div class="col-10">
        <SmGrid @ref=@grid
                Data=@forecasts
                Height=@gridHeight
                Width=@gridWidth
                FilterMode=GridFilterMode.FilterRow
                ShowDetailRow=@showDetailRow
                PageSize=@pageSize
                EditMode=@gridEditMode
                EditingFormTitleDelegate="@((WeatherForecast data) => Task.FromResult(data.TemperatureC.ToString()))"
                SelectionMode=@selectionMode
                ColumnResizeMode=@columnResizeMode
                CssClass=@cssClass
                @bind-SingleSelectedDataRow=@selectedRow
                @bind-MultipleSelectedDataRows=@selectedRows
                RowEditBegin="@((WeatherForecast data) => RowEditBegin(data))"
                RowInserting="@((WeatherForecast data) => RowInsertingHandler(data))"
                RowRemoving="@((WeatherForecast data) => RowRemovingHandler(data))">
            <Columns>
                <Column Title="Date1" Field="@nameof(WeatherForecast.Date)" Width="100px">
                    <CellShowTemplate>
                        @{
                            WeatherForecast data = context as WeatherForecast;
                            <div style="white-space:break-spaces">@data.GetHashCode()</div>
                        }
                    </CellShowTemplate>
                </Column>
                <Column Title="TemperatureC" Field="@nameof(WeatherForecast.TemperatureC)" Width="1*" />
                <Column Title="TemperatureF" Field="@nameof(WeatherForecast.TemperatureF)" Width=@columnWidth />
                <Column Field="@nameof(WeatherForecast.Summary)" Width="1*" />
                <CommandColumn EditButtonVisible=true NewButtonVisible=true Width="150px" />
            </Columns>
            <DetailRowTemplate Context="mydata">
                @{
                    WeatherForecast data = mydata as WeatherForecast;
                    <div>@data.Date</div>
                    <div>@data.TemperatureC</div>
                    <div>@data.TemperatureF</div>
                }
            </DetailRowTemplate>
        </SmGrid>
    </div>
    <div class="col-2">
        <div class="mb-3">
            <label for="pageSize" class="form-label">Page size</label>
            <input type="text" class="form-control" id="pageSize" @bind=pageSize />
        </div>
        <div class="mb-3">
            <label for="columnWidth">Column width</label>
            <input type="text" class="form-control" id="columnWidth" @bind=columnWidth />
        </div>
        <div class="mb-3">
            <label for="gridHeight">Grid height</label>
            <input type="text" class="form-control" id="gridHeight" @bind=gridHeight />
        </div>
        <div class="mb-3">
            <label for="gridWidth">Grid width</label>
            <input type="text" class="form-control" id="gridWidth" @bind=gridWidth />
        </div>
        <div class="mb-3">
            <label for="gridEditMode" class="form-label">Grid edit mode</label>
            <select id="gridEditMode" class="form-select form-select-lg" @bind=gridEditMode>
                <option value=@GridEditMode.None>None</option>
                <option value=@GridEditMode.Form>Form</option>
                <option value=@GridEditMode.Popup>Popup</option>
            </select>
        </div>
        <div class="mb-3">
            <label for="columnResizeMode" class="form-label">Column resize mode</label>
            <select id="columnResizeMode" class="form-select form-select-lg" @bind=columnResizeMode>
                <option value=@GridColumnResizeMode.None>None</option>
                <option value=@GridColumnResizeMode.Sliding>Sliding</option>
                <option value=@GridColumnResizeMode.Block>Block</option>
            </select>
        </div>
        <div class="mb-3">
            <div class="form-check">
                <input class="form-check-input" type="checkbox" @bind=showDetailRow id="showDetailRow">
                <label class="form-check-label" for="showDetailRow">
                    Show detail row
                </label>
            </div>
        </div>
        
        <div class="mb-3">
            <button class="btn btn-primary" @onclick="collapseAllDetailRows">Collapse all detail rows</button>
        </div>

        <div class="mb-3">
            <button class="btn btn-primary" @onclick="changeStyle">Change table style</button>
        </div>
        
        <div class="mb-3">
            <label for="rowSelectionMode" class="form-label">Row selection mode</label>
            <select id="rowSelectionMode" class="form-select form-select-lg" @bind=selectionMode>
                <option value=@RowSelectionMode.None>None</option>
                <option value=@RowSelectionMode.Single>Single</option>
                <option value=@RowSelectionMode.Multiple>Multiple</option>
            </select>
        </div>

        @if (selectedRow is not null)
        {
            <div class="mb-3 row">
                <div class="col-8">@($"{selectedRow.Summary} Temp. {selectedRow.TemperatureC} C")</div>
                <div class="col-4">
                    <button class="btn btn-danger mb-3" @onclick="deselectSingleSelectedItem">deselect</button>
                </div>
            </div>
        }

        @if (selectedRows != null)
        {
            @foreach (var item in selectedRows)
            {
                <div class="mb-3 row">
                    <div class="col-8">@($"{item.Summary} Temp. {item.TemperatureC} C")</div>

                    <div class="col-4">
                        <button class="btn btn-danger mb-3" @onclick=@(()=>{selectedRows = selectedRows.Except([item]);})>deselect</button>
                    </div>
                </div>
            }
        }
    </div>
</div>
```

### Index.razor.cs
```csharp
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


```