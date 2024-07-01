## Basic sample

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
                ShowDetailRow=@showDetailRow
                PageSize=@pageSize
                EditMode=@gridEditMode
                SelectionMode=@selectionMode
                ColumnResizeMode=@columnResizeMode
                CssClass=@cssClass
                @bind-SingleSelectedDataRow=@selectedRow
                @bind-MultipleSelectedDataRows=@selectedRows
                RowEditBegin="@((WeatherForecast data) => RowEditBeginHandler(data))"
                RowInserting="@((WeatherForecast data) => RowInsertingHandler(data))"
                RowRemoving="@((WeatherForecast data) => RowRemovingHandler(data))">
            <Columns>
                <Column Title="Date" Field="@nameof(WeatherForecast.Date)" Width="100px" />
                <Column Title="TemperatureC" Field="@nameof(WeatherForecast.TemperatureC)" Width="1*" />
                <Column Title="TemperatureF" Field="@nameof(WeatherForecast.TemperatureF)" Width=@columnWidth />
                <Column Field="@nameof(WeatherForecast.Summary)" Width="1*" />
                <CommandColumn EditButtonVisible=true DeleteButtonVisible=true NewButtonVisible=true Width="150px" />
            </Columns>
            <EditPopupTitleTemplate Context="context">
                @{
                    WeatherForecast data = context as WeatherForecast;
                    <div>@($"Temperature on {data!.Date:yyyy-MM-dd}")</div>
                }
            </EditPopupTitleTemplate>
            <DetailRowTemplate Context="mydata">
                @{
                    WeatherForecast data = mydata as WeatherForecast;
                    <div class="form-group row mb-2">
                        <label class="col-2 col-form-label">Date</label>
                        <div class="col-10">
                            @($"{data.Date:yyyy-MM-dd}")
                        </div>
                    </div>
                    <div class="form-group row mb-2">
                        <label class="col-2 col-form-label">Temp.C</label>
                        <div class="col-10">
                            @data.TemperatureC
                        </div>
                    </div>
                    <div class="form-group row mb-2">
                        <label class="col-2 col-form-label">Temp.F</label>
                        <div class="col-10">
                            @data.TemperatureF
                        </div>
                    </div>
                    <div class="form-group row mb-2">
                        <label class="col-2 col-form-label">Summary</label>
                        <div class="col-10">
                            @data.Summary
                        </div>
                    </div>
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

@code {
    private List<WeatherForecast>? forecasts;
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

    Task RowEditBeginHandler(WeatherForecast item)
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

    async Task collapseAllDetailRows() => await grid!.CollapseAllDetailRows();
}
```

### EditTemplate.razor
#### add @rendermode InteractiveServer to your page
```csharp 
@page "/edit-template"
@rendermode InteractiveServer

@using Samovar.Grid.Server.Test.Data
@using System.ComponentModel.DataAnnotations
@using System

@inject WeatherForecastService ForecastService

<h1>Insert / Edit Templates</h1>

<div class="form-group row align-items-center mb-3">
    <label for="gridEditMode" class="form-label col-1">Grid edit mode</label>
    <div class="col-1">
        <select id="gridEditMode" class="col-10 form-select form-select-lg" @bind=gridEditMode>
            <option value=@GridEditMode.None>None</option>
            <option value=@GridEditMode.Form>Form</option>
            <option value=@GridEditMode.Popup>Popup</option>
        </select>
    </div>
</div>

<SmGrid @ref=grid
        Data="forecasts"
        FilterMode=GridFilterMode.FilterRow
        Height="600px"
        RowEditBegin="@((WeatherForecast data) => RowEditBeginHandler(data))"
        RowInsertBegin=@RowInsertBeginHandler
        EditMode=@gridEditMode
        DataNavigationMode=NavigationMode.Paging
        SelectionMode=RowSelectionMode.Multiple>
    <Columns>
        <CommandColumn EditButtonVisible=true NewButtonVisible=true DeleteButtonVisible=false Width="150px" />
        <Column Title="Summary" Field="@nameof(WeatherForecast.Summary)">
            <CellShowTemplate>
                @{
                    WeatherForecast data = context as WeatherForecast;
                    <div style="white-space:break-spaces">@($"Summary: {data!.Summary}")</div>
                }
            </CellShowTemplate>
        </Column>
        <Column Title="Date" Field="@nameof(WeatherForecast.Date)" />
        <Column Title="TemperatureC" Field="@nameof(WeatherForecast.TemperatureC)" />
        <Column Title="TemperatureF" Field="@nameof(WeatherForecast.TemperatureF)" />
    </Columns>
    <EditFormTemplate>
        <EditForm Model="@EditContext" Context="EditFormContext" OnValidSubmit=HandleValidCustomEditSubmit>
            <DataAnnotationsValidator />
            <div class="form-group row mb-2">
                <label class="col-2 col-form-label" for="temperatureC">Temp.C</label>
                <div class="col-10">
                    <InputNumber class="form-control" id="temperatureC" TValue="int?" @bind-Value="EditContext.TemperatureC" />
                </div>
            </div>
            <div class="form-group row mb-2">
                <label class="col-2 col-form-label" for="summary">Summary</label>
                <div class="col-10">
                    <InputText class="form-control" id="summary" @bind-Value="EditContext.Summary" />
                </div>
            </div>
            <div class="row mb-2">
                <ValidationSummary />
            </div>
            <div class="row">
                <div class="col s-flex justify-content-end">
                    <button type="submit" class="btn btn-outline-primary mx-1">update</button>
                    <button type="button" class="btn btn-outline-secondary mx-1" @onclick="HandleCancelCustomEdit">cancel</button>
                </div>
            </div>
        </EditForm>
    </EditFormTemplate>
    <InsertFormTemplate>
        <EditForm Model="@InsertContext" Context="EditFormContext" OnValidSubmit=HandleValidInsertingSubmit>

            <DataAnnotationsValidator />
            <div class="form-group row mb-2">
                <label class="col-2 col-form-label" for="temperatureC">Temp.C</label>
                <div class="col-10">
                    <InputNumber class="form-control" id="temperatureC" TValue="int?" @bind-Value="InsertContext.TemperatureC" />
                </div>
            </div>
            <div class="form-group row mb-2">
                <label class="col-2 col-form-label" for="summary">Summary</label>
                <div class="col-10">
                    <InputText class="form-control" id="summary" @bind-Value="InsertContext.Summary" />
                </div>
            </div>
            <div class="form-row">
                <ValidationSummary />
            </div>
            <div class="row">
                <div class="col s-flex justify-content-end">
                    <button type="submit" class="btn btn-outline-primary mx-1">add</button>
                    <button type="button" class="btn btn-outline-secondary mx-1" @onclick="OnCancelInsertingButtonClick">cancel</button>
                </div>
            </div>
        </EditForm>
    </InsertFormTemplate>
</SmGrid>


@code {
    private List<WeatherForecast> forecasts;
    FormEditContext EditContext = null;
    FormInsertContext InsertContext = null;
    SmGrid<WeatherForecast> grid;
    GridEditMode gridEditMode = GridEditMode.Form;

    protected override async Task OnInitializedAsync()
    {
        forecasts = (await ForecastService.GetForecastAsync(DateTime.Now)).ToList();
    }

    void RowEditBeginHandler(WeatherForecast item)
    {
        EditContext = new FormEditContext(item);
        // var context = new Microsoft.AspNetCore.Components.Forms.EditContext(item);
        // context.
    }

    void RowInsertBeginHandler()
    {
        InsertContext = new FormInsertContext();
    }

    async Task HandleValidCustomEditSubmit()
    {
        EditContext.DataItem.TemperatureC = EditContext.TemperatureC;
        EditContext.DataItem.Summary = EditContext.Summary;
        await grid.CommitCustomRowEdit(EditContext.DataItem);
    }

    async Task HandleValidInsertingSubmit()
    {
        var newItem = new WeatherForecast
            {
                Summary = InsertContext.Summary,
                Date = DateTime.Now,
                TemperatureC = InsertContext.TemperatureC
            };

        forecasts = ((new WeatherForecast[] { newItem }).Concat(forecasts)).ToList();
        await grid.CancelRowInsert();
        StateHasChanged();
    }

    async Task HandleCancelCustomEdit()
    {
        await grid.CancelCustomRowEdit(EditContext.DataItem);
        EditContext = null;
    }

    async Task OnCancelInsertingButtonClick()
    {
        await grid.CancelRowInsert();
        EditContext = null;
    }

    class FormEditContext
    {
        public FormEditContext(WeatherForecast dataItem)
        {
            DataItem = dataItem;
            TemperatureC = dataItem.TemperatureC;
            Summary = dataItem.Summary;
        }

        public WeatherForecast DataItem { get; set; }

        [Required]
        [StringLength(maximumLength: 32, MinimumLength = 4, ErrorMessage = "The description should be 4 to 32 characters.")]
        public string Summary { get; set; }

        public int? TemperatureC { get; set; }
    }

    internal class FormInsertContext
    {
        internal FormInsertContext()
        {
            Summary = "[init summary value}";
        }

        [Required]
        public int? TemperatureC { get; set; }

        [Required]
        [StringLength(maximumLength: 32, MinimumLength = 4, ErrorMessage = "The description should be 4 to 32 characters.")]
        public string Summary { get; set; }
    }
}

```

### CellTemplate.razor
#### add @rendermode InteractiveServer to your page
```csharp 
@page "/cell-template"
@rendermode InteractiveServer

@using Samovar.Grid.Server.Test.Data
@using System.ComponentModel.DataAnnotations
@using System

@inject WeatherForecastService ForecastService

<h1>Cell Templates</h1>

<style>
    .above-degree-style {
        color: red;
        font-weight: bold;
    }

    .below-degree-style {
        color: blue;
        font-weight: bold;
    }
</style>

<SmGrid @ref=grid
        Data="forecasts"
        FilterMode=GridFilterMode.FilterRow
        Height="600px"
        DataNavigationMode=NavigationMode.Paging
        SelectionMode=RowSelectionMode.Multiple>
    <Columns>
        <Column Field="@nameof(WeatherForecast.Summary)">
            <CellShowTemplate>
                @{
                    <div>@($"Summary: {(context as WeatherForecast)!.Summary}")</div>
                }
            </CellShowTemplate>
        </Column>
        <Column Field="@nameof(WeatherForecast.Date)">
            <CellShowTemplate>
                @{
                    <div>@($"{(context as WeatherForecast)!.Date:yyyy-MM-dd}")</div>
                }
            </CellShowTemplate>
        </Column>
        <Column Field="@nameof(WeatherForecast.TemperatureC)">
            <CellShowTemplate>
                @{
                    var temperature = (context as WeatherForecast)!.TemperatureC;
                    if (temperature > 0)
                    {
                        <div class="above-degree-style">@($"{temperature} C")</div>
                    }
                    else
                    {
                        <div class="below-degree-style">@($"{temperature} C")</div>
                    }
                }
            </CellShowTemplate>
        </Column>
    </Columns>
</SmGrid>


@code {
    private List<WeatherForecast> forecasts;
    SmGrid<WeatherForecast> grid;
    GridEditMode gridEditMode = GridEditMode.Form;

    protected override async Task OnInitializedAsync()
    {
        forecasts = (await ForecastService.GetForecastAsync(DateTime.Now)).ToList();
    }
}

```

### Filter.razor
#### add @rendermode InteractiveServer to your page
```csharp 
@page "/filter"
@rendermode InteractiveServer

@using Samovar.Grid.Server.Test.Data
@using System.ComponentModel.DataAnnotations
@using System

@inject WeatherForecastService ForecastService

<h1>Filter</h1>

<div class="form-group row">
    <label for="filterMode" class="col-1 col-form-label m-2">Filter mode</label>
    <div class="col-1 m-2">
        <select id="filterMode" class="form-select form-select-lg" @bind=filterMode>
            <option value=@GridFilterMode.None>None</option>
            <option value=@GridFilterMode.Custom>Custom</option>
            <option value=@GridFilterMode.FilterRow>FilterRow</option>
        </select>
    </div>
    <button class="col-2 btn btn-primary m-3" @onclick="ApplyCustomFilter">Apply custom filter</button>
    <button class="col-2 btn btn-primary m-3" @onclick="ResetCustomFilter">reset custom filter</button>
</div>

<SmGrid @ref=grid
        Data=forecasts
        FilterMode=filterMode
        Height="600px">
    <Columns>
        <Column Field="@nameof(WeatherForecast.Summary)" />
        <Column Field="@nameof(WeatherForecast.Date)" />
        <Column Field="@nameof(WeatherForecast.TemperatureC)" />
    </Columns>
</SmGrid>


@code {
    private List<WeatherForecast> forecasts;
    SmGrid<WeatherForecast> grid;
    GridFilterMode filterMode = GridFilterMode.Custom;

    protected override async Task OnInitializedAsync()
    {
        forecasts = (await ForecastService.GetForecastAsync(DateTime.Now)).ToList();
    }

    async Task ApplyCustomFilter()
    {
        Func<WeatherForecast, bool> filter = w => w.Summary.ToLower() == "cool";
        await grid.ApplyCustomFilter(filter);
    }

    async Task ResetCustomFilter()
    {
        await grid.ResetCustomFilter();
    }
}
```

### Virtual.razor
#### add @rendermode InteractiveServer to your page
```csharp 
@page "/virtual"
@rendermode InteractiveServer

@using Samovar.Grid.Server.Test.Data
@using System.ComponentModel.DataAnnotations
@using System

@inject WeatherForecastService ForecastService

<h1>Virtual</h1>

<SmGrid Data=forecasts DataNavigationMode=NavigationMode.Virtual>
    <Columns>
        <Column Field="@nameof(WeatherForecast.Date)" />
        <Column Field="@nameof(WeatherForecast.Summary)" />
        <Column Field="@nameof(WeatherForecast.TemperatureC)" />
    </Columns>
</SmGrid>

@code {
    private List<WeatherForecast> forecasts;

    protected override async Task OnInitializedAsync()
    {
        forecasts = (await ForecastService.GetForecastAsync(DateTime.Now)).ToList();
    }
}

```