# SamovarGrid. DataGrid for Blazor.
## Functions
- Sorting
- Filter
- Paging
- Editing
- Cell templating
- Edit templating
- Row single/multiple selection
- Keyboard navigation

## Prerequisites 
- Bootstrap
- Open Iconic
- JQuery

## Javascript and CSS files
```html
Bind Samovar Javascript and CSS files in _Host.cshtml
...
<link href="_content/SamovarGrid/samovar.grid.css" rel="stylesheet" />
...

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>SamovarGridProServerTest</title>
    <base href="~/" />
    <link rel="stylesheet" href="css/bootstrap/bootstrap.min.css" />
    <link href="css/site.css" rel="stylesheet" />
    
    <span style="background-color: #FFFF00">
    <link href="_content/SamovarGrid/samovar.grid.css" rel="stylesheet" />

    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.7/umd/popper.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.min.js"></script>
    </span>
    <script src="app.js"></script>
</head>
<body>
    <app>
        @(await Html.RenderComponentAsync<App>(RenderMode.ServerPrerendered))
    </app>

    <script src="_framework/blazor.server.js"></script>
</body>
</html>
```


## TODO for Samples: Modify standard VS template code for data service

```csharp
public class WeatherForecast
{
    public int Position { get; set; }
    public DateTime Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    private string summary;
    public string Summary {
        get { return summary; }
        set {
            this.summary = value;
        } 
    }
    public string Summary2 { get; set; }

    public double SummaryDbl { get; set; }
    public bool YesNo { get; set; }
    public sbyte SByteValue { get; set; }
    public byte ByteValue { get; set; }
    public char CharValue { get; set; }

    public override string ToString()
    {
        return Position.ToString() + " " + Summary;
    }
}

public Task<WeatherForecast[]> GetForecastAsync(DateTime startDate, int dataCnt)
{
    var rng = new Random();
    int pos = 0;
    return Task.FromResult(Enumerable.Range(1, dataCnt).Select(index => new WeatherForecast
    {
        Position = ++pos,
        Date = startDate.AddDays(index),
        TemperatureC = rng.Next(-20, 55),
        SummaryDbl = rng.Next(-20, 55) + 0.125,
        Summary = Summaries[rng.Next(Summaries.Length)],
        YesNo = true,
        SByteValue = (sbyte)rng.Next(-128, 127),
        ByteValue = (byte)rng.Next(0, 255),
        CharValue = 'c'
    }).ToArray());
}
```

### Column width mode:

##### Relative (default) '*'-annotation
```html
    <GridColumn Title="Date" Field="@nameof(WeatherForecast.Date)" /><!--default '1*'-->
    <GridColumn Width="2*" Title="TemperatureC" Field="@nameof(WeatherForecast.TemperatureC)" />
    <GridColumn Width="3*" Title="TemperatureF" Field="@nameof(WeatherForecast.TemperatureF)" />
```
##### Absolute width

```html
    <GridColumn Width="100px" Title="TemperatureC" Field="@nameof(WeatherForecast.TemperatureC)" />
    <GridColumn Width="150px" Title="TemperatureF" Field="@nameof(WeatherForecast.TemperatureF)" />
```

#### Styling

```html
    <GridColumn TableTagClass="table table-dark table-sm table-striped table-hover" ... />
    <GridColumn TheadTagClass="thead-light" ... />
    <GridColumn SelectedRowClass="my-selected-row-color" ... />
```
```css
.my-selected-row-color {
    background: #fed7d7 !important;
    font-weight: 700;
}
```

## Data binding
```html
@inject WeatherForecastService ForecastService
@implements IDisposable

<h1>Data binding</h1>
<SamovarGrid Data="@forecasts">
    <GridColumns>
        <GridCommandColumn />
        <GridColumn Title="Date" Field="@nameof(WeatherForecast.Date)" />
        <GridColumn Title="TemperatureC" Field="@nameof(WeatherForecast.TemperatureC)" />
        <GridColumn Title="TemperatureF" Field="@nameof(WeatherForecast.TemperatureF)" />
        <GridColumn Title="Summary" Field="@nameof(WeatherForecast.Summary)" />
    </GridColumns>
</SamovarGrid>
```
```csharp
@code{
    IEnumerable<WeatherForecast> forecasts;

    protected override async Task OnInitializedAsync()
    {
        forecasts = await ForecastService.GetMockData();
    }

    public void Dispose()
    {
        forecasts = null;
    }
}
```


## Data bindig. Observable collection

```html
@using System.Linq
@using System.Collections.ObjectModel

@inject WeatherForecastService ForecastService

<h4>Observable collection</h4>
<button class="btn btn-primary" @onclick="AddNewRow">add new row</button>
<button class="btn btn-primary" @onclick="RemoveFirstRow">remove first row</button>
<button class="btn btn-primary" @onclick="ClearDataSet">clear data set</button>

<SamovarGrid @ref="grid"
             Data="@forecasts"
             Height="600px"
             EditMode="GridEditMode.Form"
             RowInserting="((newRow) => OnRowInserting(newRow))"
             RowDeleting="OnRowDeleting">
    <GridColumns>
        <GridColumn Title="Pos" Field="@nameof(WeatherForecast.Position)" />
        <GridColumn Title="Date" Field="@nameof(WeatherForecast.Date)" />
        <GridColumn Title="TemperatureC" Field="@nameof(WeatherForecast.TemperatureC)">
            <CellEditTemplate>
                @{
                    WeatherForecast data = context as WeatherForecast;
                    <select @bind="data.TemperatureC" class="form-control">
                        <option value="10">10</option>
                        <option value="20">20</option>
                        <option value="30">30</option>
                    </select>
                }
            </CellEditTemplate>
        </GridColumn>
        <GridColumn Title="TemperatureF" Field="@nameof(WeatherForecast.TemperatureF)" />
        <GridColumn Title="Summary" Field="@nameof(WeatherForecast.Summary)" />
        <GridCommandColumn Width="110px" />
    </GridColumns>
</SamovarGrid>
```
```csharp
@code{
    ObservableCollection<WeatherForecast> forecasts;
    SamovarGrid<WeatherForecast> grid;

    protected override async Task OnInitializedAsync()
    {
        forecasts = new ObservableCollection<WeatherForecast>(await ForecastService.GetForecastAsync(DateTime.Now, 10));
    }

    void AddNewRow()
    {
        forecasts.Insert(0, new WeatherForecast
        {
            Position = -1,
            Date = new DateTime(2019, 1, 1),
            Summary = "Summary (Pos -1)",
            Summary2 = "Summary2",
            TemperatureC = 33
        });
    }

    void RemoveFirstRow()
    {
        if (forecasts.Count() > 0)
            forecasts.RemoveAt(0);
    }

    void ClearDataSet()
    {
        forecasts.Clear();
    }

    void OnRowInserting(WeatherForecast data)
    {
        forecasts.Add(data);
    }

    void OnRowDeleting(WeatherForecast dataItem)
    {
        forecasts.Remove(dataItem);
    }
}
```
## Columns

```html
@using System.Linq

@inject WeatherForecastService ForecastService

<h1>Relative column width.</h1>

<SamovarGrid Data="@forecasts"
             OrderFieldByDefault="@nameof(WeatherForecast.TemperatureC)"
             OrderDesc="true"
             PageSize="20"
             PagerSize="10"
             Height="400px">
    <GridColumns>
        <GridColumn Title="Date (1*)" Field="@nameof(WeatherForecast.Date)" Width="1*" />
        <GridColumn Title="TemperatureC (1*)" Field="@nameof(WeatherForecast.TemperatureC)" Width="1*" />
        <GridColumn Title="TemperatureF (1*)" Field="@nameof(WeatherForecast.TemperatureF)" Width="1*" />
        <GridColumn Title="Summary (1*)" Field="@nameof(WeatherForecast.Summary)" Width="1*" />
    </GridColumns>
</SamovarGrid>

<h1 class="mt-4">Absolute column width.</h1>

<SamovarGrid Data="@forecasts"
             OrderFieldByDefault="@nameof(WeatherForecast.TemperatureC)"
             OrderDesc="true"
             PageSize="20"
             PagerSize="10"
             Height="270px" FilterMode="@GridFilterMode.FilterRow">
    <GridColumns>
        <GridColumn Title="Date (200px)" Field="@nameof(WeatherForecast.Date)" Width="200px" />
        <GridColumn Title="TemperatureC (100px)" Field="@nameof(WeatherForecast.TemperatureC)" Width="100px" />
        <GridColumn Title="TemperatureF (150px)" Field="@nameof(WeatherForecast.TemperatureF)" Width="150px" />
        <GridColumn Title="Summary (100px)" Field="@nameof(WeatherForecast.Summary)" Width="100px" />
    </GridColumns>
</SamovarGrid>

<h1>Column width. Mixed: relative and absolute</h1>

<SamovarGrid Data="@forecasts"
             OrderFieldByDefault="@nameof(WeatherForecast.TemperatureC)"
             OrderDesc="true"
             PageSize="20"
             PagerSize="10"
             Height="400px">
    <GridColumns>
        <GridColumn Title="Date (200px)" Field="@nameof(WeatherForecast.Date)" Width="200px" />
        <GridColumn Title="TemperatureC (3*)" Field="@nameof(WeatherForecast.TemperatureC)" Width="3*" />
        <GridColumn Title="TemperatureF (2*)" Field="@nameof(WeatherForecast.TemperatureF)" Width="2*" />
        <GridColumn Title="Summary (100px)" Field="@nameof(WeatherForecast.Summary)" Width="100px" />
    </GridColumns>
</SamovarGrid>

<h1>Headless grid</h1>
<SamovarGrid Data="@forecasts" Headless="true">
    <GridColumns>
        <GridColumn Title="Date" Field="@nameof(WeatherForecast.Date)" />
        <GridColumn Title="TemperatureC" Field="@nameof(WeatherForecast.TemperatureC)" />
        <GridColumn Title="TemperatureF" Field="@nameof(WeatherForecast.TemperatureF)" />
        <GridColumn Title="Summary" Field="@nameof(WeatherForecast.Summary)" />
    </GridColumns>
</SamovarGrid>
```
```csharp
@code{
    IEnumerable<WeatherForecast> forecasts;

    protected override async Task OnInitializedAsync()
    {
        forecasts = await ForecastService.GetForecastAsync(DateTime.Now, 1000);
    }
}
```

## Sample 4. Commands and event handlers

```html
@page "/GridCommands"

@using System.Linq

@inject WeatherForecastService ForecastService

<h1>Commands and event handlers</h1>
<button class="btn btn-primary" @onclick="AddNewRow">add new row</button>
<button class="btn btn-primary" @onclick="CreateNewDataSet">create new data set</button>
<button class="btn btn-primary" @onclick="ClearDataSet">clear data set</button>
<SamovarGrid Data="@forecasts"
             OrderFieldByDefault="@nameof(WeatherForecast.TemperatureC)"
             OrderDesc="true"
             Pageable="true"
             PageSize="20"
             PagerSize="10"
             Height="@gridHeight"
             Width="@gridWidth"
             OnRowSelected="RowSelectedHandler"
             
             
             
             OnRowEditBegin="RowEditBeginHandler"
             OnRowEditCommit="RowEditCommitHandler"
             OnRowEditCancel="RowEditCancelHandler"
             OnAfterFilter="AfterFilter"
             OnAfterSort="AfterSort"
             OnAfterPageChange="AfterPageChange"
             TableTagClass="table table-striped table-hover"
             Headless="false">
    <GridColumns>
        <GridColumn Title="Date" Field="@nameof(WeatherForecast.Date)">
            <CellShowTemplate>
                @{
                    WeatherForecast data = context as WeatherForecast;
                    <div>@data.Date.ToShortDateString()</div>
                }
            </CellShowTemplate>
        </GridColumn>
        <GridColumn Title="TemperatureC" Field="@nameof(WeatherForecast.TemperatureC)" />
        <GridColumn Title="TemperatureF" Field="@nameof(WeatherForecast.TemperatureF)" />
        <GridColumn Title="Summary" Field="@nameof(WeatherForecast.Summary)">
            <CellEditTemplate>
                @{
                    WeatherForecast data = context as WeatherForecast;
                    <input type="text" @bind="@data.Summary" />
                }
            </CellEditTemplate>
            <CellShowTemplate>
                @{
                    WeatherForecast data = context as WeatherForecast;
                    <div style="word-break:break-all;">@data.Summary</div>
                }
            </CellShowTemplate>
        </GridColumn>
        <GridCommandColumn>
            
            
        </GridCommandColumn>
    </GridColumns>
</SamovarGrid>

<h4>Grid items</h4>
<div>@forecasts.Count().ToString()</div>

<h4>Action</h4>
<div>@selectedAction</div>

<h4>Action row item</h4>
<div>@actionItem</div>
```
```csharp
@code{
    string gridHeight = "600px";
    string gridWidth = "100%";

    string selectedAction { set; get; }
    string actionItem { set; get; }

    List<WeatherForecast> forecasts = new List<WeatherForecast>();

    protected override async Task OnInitializedAsync()
    {
        forecasts = (await ForecastService.GetForecastAsync(DateTime.Now)).ToList();
    }

    void RowSelectedHandler(GridRowEventArgs args)
    {
        actionItem = ((WeatherForecast)args.RowData).Summary;
        selectedAction = "RowSelected";
    }

    void RowDeleteBeginHandler(GridRowEventArgs args)
    {
        actionItem = ((WeatherForecast)args.RowData).Summary;
        selectedAction = "RowDeleteBegin";
    }

    void RowDeleteCancelHandler(GridRowEventArgs args)
    {
        actionItem = ((WeatherForecast)args.RowData).Summary;
        selectedAction = "RowDeleteCancel";
    }

    void RowDeleteCommitHandler(GridRowEventArgs args)
    {
        actionItem = ((WeatherForecast)args.RowData).Summary;
        this.forecasts.Remove((WeatherForecast)args.RowData);
        selectedAction = "RowDeleteCommit";
    }

    void RowEditCommitHandler(GridRowEditEventArgs<WeatherForecast> args)
    {
        WeatherForecast item = args.NewData;
    }

    void RowEditBeginHandler(GridRowEventArgs args)
    {
        actionItem = ((WeatherForecast)args.RowData).Summary;
        selectedAction = "RowEditBegin";
    }

    void RowEditCancelHandler(GridRowEventArgs args)
    {
        actionItem = ((WeatherForecast)args.RowData).Summary;
        selectedAction = "RowEditCancel";
    }
    
    void AfterFilter(Samovar.Grid.Data.GridFilterEventArgs args)
    {
    }

    void AfterSort(Samovar.Grid.Data.GridSortEventArgs args)
    {
    }

    void AfterPageChange(Samovar.Grid.Data.GridPagingEventArgs args)
    {
    }

    void AddNewRow()
    {
        forecasts.Add(new WeatherForecast
        {
            Date = new DateTime(2019, 1, 1),
            Summary = "Summary",
            Summary2 = "Summary2",
            TemperatureC = 33
        });
    }

    async Task CreateNewDataSet()
    {
        forecasts = (await ForecastService.GetForecastAsync(DateTime.Now)).ToList();
    }

    void ClearDataSet()
    {
        forecasts.Clear();
    }   
}
```

## Sample 5. Grid cell templating 

```html
@page "/GridCellTemplating"

@using System.Linq

@inject WeatherForecastService ForecastService

<h1>Cell templating</h1>

<SamovarGrid Data="@forecasts"
             OrderFieldByDefault="@nameof(WeatherForecast.TemperatureC)"
             OrderDesc="true"
             Pageable="false"
             PageSize="20"
             PagerSize="10"
             Height="@gridHeight"
             OnRowSelected="RowSelectedHandler"
             
             
             
             OnRowEditBegin="RowEditBeginHandler"
             OnRowEditCommit="RowEditCommitHandler"
             OnRowEditCancel="RowEditCancelHandler"
             Headless="false"
             SelectedRowClass="bg-warning">
    <GridColumns>
        <GridColumn Title="Date" Width="100px">
            <CellShowTemplate>
                @{
                    WeatherForecast data = context as WeatherForecast;
                    <div>@data.Date.ToShortDateString()</div>
                }
            </CellShowTemplate>
        </GridColumn>
        <GridColumn Title="Date2" Field="@nameof(WeatherForecast.Date)" Width="200px" />
        <GridColumn Title="TemperatureC" Field="@nameof(WeatherForecast.TemperatureC)" Width="200px" />
        <GridColumn Title="TemperatureF" Field="@nameof(WeatherForecast.TemperatureF)" Width="150px" />
        <GridColumn Title="Summary" Field="@nameof(WeatherForecast.Summary)" Width="150px">
            <CellEditTemplate>
                @{
                    WeatherForecast data = context as WeatherForecast;
                    <input style="width:100%;" type="text" @bind="@data.Summary" @oninput="onInput" />

                    void onInput(ChangeEventArgs args)
                    {
                    }
                }
            </CellEditTemplate>
            <CellShowTemplate>
                @{
                    WeatherForecast data = context as WeatherForecast;
                    <div style="word-break:break-all;">@data.Summary</div>
                }
            </CellShowTemplate>
        </GridColumn>
        <GridCommandColumn Width="120px">
            
            
        </GridCommandColumn>
    </GridColumns>
</SamovarGrid>

<div class="form-group">
    <label class="mr-2">Grid height</label>
    <input @bind-value="@gridHeight" @bind-value:event="onchange" />
</div>

<h4>Datasize</h4>
<div>@forecasts.Count()</div>

<h4>Action</h4>
<div>@selectedAction</div>

<h4>Action row item</h4>
<div>@actionItem</div>
```
```csharp
@code{
    string gridHeight = "500px";

    string selectedAction { set; get; }
    string actionItem { set; get; }

    List<WeatherForecast> forecasts = new List<WeatherForecast>();

    protected override async Task OnInitializedAsync()
    {
        forecasts = (await ForecastService.GetForecastAsync(DateTime.Now, 1000)).ToList();
    }

    void RowSelectedHandler(GridRowEventArgs args)
    {
        actionItem = ((WeatherForecast)args.RowData).Summary;
        selectedAction = "RowSelected";
    }

    void RowDeleteBeginHandler(GridRowEventArgs args)
    {
        actionItem = ((WeatherForecast)args.RowData).Summary;
        selectedAction = "RowDeleteBegin";
    }

    void RowDeleteCancelHandler(GridRowEventArgs args)
    {
        actionItem = ((WeatherForecast)args.RowData).Summary;
        selectedAction = "RowDeleteCancel";
    }

    void RowDeleteCommitHandler(GridRowEventArgs args)
    {
        actionItem = ((WeatherForecast)args.RowData).Summary;
        this.forecasts.Remove((WeatherForecast)args.RowData);
        selectedAction = "RowDeleteCommit";
    }

    void RowEditCommitHandler(GridRowEditEventArgs<WeatherForecast> args)
    {
        WeatherForecast item = args.NewData;
    }

    void RowEditBeginHandler(GridRowEventArgs args)
    {
        actionItem = ((WeatherForecast)args.RowData).Summary;
        selectedAction = "RowEditBegin";
    }

    void RowEditCancelHandler(GridRowEventArgs args)
    {
        actionItem = ((WeatherForecast)args.RowData).Summary;
        selectedAction = "RowEditCancel";
    }    
}
```

## Sample 6. Grid width and height

```html
@page "/GridWidthHeight"

@using System.Linq

@inject WeatherForecastService ForecastService

<h1>Grid width and height</h1>

<SamovarGrid Data="@forecasts1"
             OrderFieldByDefault="@nameof(WeatherForecast.TemperatureC)"
             OrderDesc="true"
             Pageable="true"
             PageSize="20"
             PagerSize="10"
             Height="calc(100vh - 200px)"
             Width="100%">
    <GridColumns>
        <GridColumn Title="Date" Field="@nameof(WeatherForecast.Date)" />
        <GridColumn Title="TemperatureC" Field="@nameof(WeatherForecast.TemperatureC)" />
        <GridColumn Title="TemperatureF" Field="@nameof(WeatherForecast.TemperatureF)" />
        <GridColumn Title="Summary" Field="@nameof(WeatherForecast.Summary)" />
        <GridCommandColumn>
            
            
        </GridCommandColumn>
    </GridColumns>
</SamovarGrid>
```
```csharp
@code{
    List<WeatherForecast> forecasts1 = new List<WeatherForecast>();

    protected override async Task OnInitializedAsync()
    {
        forecasts1 = (await ForecastService.GetForecastAsync(DateTime.Now, 1000)).ToList();
    }    
}
```

## Sample 7. Filter
```html
        <SamovarGrid FilterMode="@GridFilterMode.FilterRow" OnAfterFilter="AfterFilter">...</SamovarGrid>
```
Filter applying by onchange event (focus lost or press "enter" key).
```html
@page "/"
@using System.Linq

@inject WeatherForecastService ForecastService

<h1>Filter</h1>

<SamovarGrid Data="@forecasts"
             OrderFieldByDefault="@nameof(WeatherForecast.TemperatureC)"
             OrderDesc="true"
             Pageable="true"
             PageSize="25"
             SelectedRowClass="bg-warning" 
             FilterMode="@GridFilterMode.FilterRow"
             OnAfterFilter="AfterFilter">
    <GridColumns>
        <GridColumn Title="Pos" Field="@nameof(WeatherForecast.Position)" />
        <GridColumn Title="Date" Field="@nameof(WeatherForecast.Date)" />
        <GridColumn Title="TemperatureC" Field="@nameof(WeatherForecast.TemperatureC)" />
        <GridColumn Title="TemperatureF" Field="@nameof(WeatherForecast.TemperatureF)" />
        <GridCommandColumn Width="110px">
            
            
        </GridCommandColumn>
    </GridColumns>
</SamovarGrid>
```
```csharp
@code{
    List<WeatherForecast> forecasts = new List<WeatherForecast>();

    protected override async Task OnInitializedAsync()
    {
        forecasts = (await ForecastService.GetForecastAsync(DateTime.Now, 1000)).ToList();
    }

    protected void AfterFilter(Samovar.Grid.Data.GridFilterEventArgs args) {
        //your code
    }
}
```

## Sample 8. Autogenerate columns
```html
<SamovarGrid Data="@forecasts" AutoGenerateColumns="true" />
```

```html
@page "/_008_AutoGenerateColumns"


@inject WeatherForecastService ForecastService

<h1>Autogenerate columns</h1>

<h3>Standard column width 150px</h3>
<SamovarGrid Data="@forecasts" AutoGenerateColumns="true" />

<h3 class="my-2">User defined column width</h3>
<SamovarGrid Data="@forecasts" AutoGenerateColumns="true" AutoGenerateColumnsWidth="250px" />
```
```csharp
@code{
    List<WeatherForecast> forecasts = new List<WeatherForecast>();

    protected override async Task OnInitializedAsync()
    {
        forecasts = (await ForecastService.GetForecastAsync(DateTime.Now, 100)).ToList();
    }
}
```