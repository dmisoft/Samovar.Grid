## SamovarGrid

A full-featured data grid component for Blazor (.NET 10).

[![NuGet](https://img.shields.io/nuget/v/SamovarGrid)](https://www.nuget.org/packages/SamovarGrid)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)

### Features

- Paging and virtual scrolling
- Sorting
- Filtering — filter row, filter menu, or custom programmatic filter
- Inline and popup editing with built-in and custom edit templates
- Row insertion and deletion
- Single and multiple row selection
- Column resizing (block and sliding modes)
- Cell display templates
- Detail (expandable) rows
- Auto-generated columns from model properties
- Command bar with bulk selection, delete, and data export (Excel / CSV)
- Cell content alignment (left, center, right)
- Grid size modes (default, small, large)
- CSS customization via Bootstrap classes and CSS custom properties
- Works with Interactive Server, WebAssembly, and Auto render modes

---

### Getting Started

#### 1. Install the NuGet package

```
dotnet add package SamovarGrid
```

#### 2. Register services

```csharp
// Program.cs
builder.Services.AddSamovarGrid();
```

#### 3. Add the stylesheet

**Option A — Automatic CSS injection (recommended)**

```csharp
// Program.cs
builder.Services.AddSamovarGrid(options => options.InjectCss = true);
```

```razor
@* App.razor *@
<head>
    ...
    <SamovarGridStyles />
</head>
```

**Option B — Manual link**

```html
<link href="_content/SamovarGrid/samovar.grid.css" rel="stylesheet" />
```

#### 4. Add the using directive

```razor
@* _Imports.razor *@
@using Samovar.Grid
```

---

### Basic Usage

```razor
@page "/"
@rendermode InteractiveServer
@inject EmployeeService EmployeeService

<SmGrid Data=employees Height="600px">
    <Columns>
        <Column Field="@nameof(Employee.FirstName)" />
        <Column Field="@nameof(Employee.LastName)" />
        <Column Field="@nameof(Employee.Department)" />
        <Column Field="@nameof(Employee.HireDate)" />
        <Column Field="@nameof(Employee.Salary)" TextAlign="GridTextAlign.Right" />
    </Columns>
</SmGrid>

@code {
    private List<Employee>? employees;

    protected override async Task OnInitializedAsync()
    {
        employees = (await EmployeeService.GetEmployeesAsync()).ToList();
    }
}
```

---

### Auto-Generated Columns

Set `AutoGenerateColumns=true` to let the grid reflect over the data model and generate columns for all public properties with simple types (`string`, `int`, `DateTime`, `bool`, `decimal`, etc.):

```razor
<SmGrid Data=employees AutoGenerateColumns=true />
```

Use `[Browsable(false)]` to exclude a property. Use `[DisplayName]` or `[Display(Name=...)]` to customize column headers:

```csharp
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class Employee
{
    [DisplayName("Full Name")]
    public string Name { get; set; }

    [Display(Name = "Annual Salary")]
    public decimal Salary { get; set; }

    [Browsable(false)]
    public string InternalId { get; set; }
}
```

---

### Filtering

The grid supports four filter modes:

```razor
<SmGrid Data=employees FilterMode=GridFilterMode.FilterRow Height="600px">
    <Columns>
        <Column Field="@nameof(Employee.FirstName)" />
        <Column Field="@nameof(Employee.LastName)" />
        <Column Field="@nameof(Employee.IsActive)" />
    </Columns>
</SmGrid>
```

| Mode | Description |
|------|-------------|
| `GridFilterMode.None` | No filtering |
| `GridFilterMode.FilterRow` | Per-column filter inputs in a dedicated row |
| `GridFilterMode.FilterMenu` | Dropdown filter menus with value selection per column |
| `GridFilterMode.Custom` | Programmatic filter via `ApplyCustomFilter()` / `ResetCustomFilter()` |

**Custom filter example:**

```csharp
async Task ApplyCustomFilter()
{
    Func<Employee, bool> filter = e => e.IsActive;
    await grid!.ApplyCustomFilter(filter);
}

async Task ResetCustomFilter()
{
    await grid!.ResetCustomFilter();
}
```

---

### Editing

Enable inline or popup editing with the `EditMode` parameter. Add a `CommandColumn` to display edit/insert/delete buttons:

```razor
<SmGrid Data=employees
        EditMode=GridEditMode.Popup
        RowEditBegin="@((Employee e) => { })"
        RowInserting="@((Employee e) => { employees.Insert(0, e); })"
        RowsRemoving="@((List<Employee> items) => { employees = employees.Except(items).ToList(); })"
        Height="600px">
    <Columns>
        <Column Field="@nameof(Employee.FirstName)" />
        <Column Field="@nameof(Employee.LastName)" />
        <Column Field="@nameof(Employee.Salary)" />
        <CommandColumn EditButtonVisible=true DeleteButtonVisible=true NewButtonVisible=true Width="150px" />
    </Columns>
</SmGrid>
```

| Mode | Description |
|------|-------------|
| `GridEditMode.None` | No editing |
| `GridEditMode.Form` | Edit form renders below the row |
| `GridEditMode.Popup` | Edit form in a modal popup |

#### Custom Edit / Insert Templates

For full control over the edit form, use `EditFormTemplate` and `InsertFormTemplate`:

```razor
<SmGrid Data=employees EditMode=GridEditMode.Form Height="600px">
    <Columns>
        <CommandColumn EditButtonVisible=true NewButtonVisible=true Width="150px" />
        <Column Field="@nameof(Employee.FirstName)" />
        <Column Field="@nameof(Employee.Salary)" />
    </Columns>
    <EditFormTemplate>
        <EditForm Model="@editContext" OnValidSubmit="HandleEditSubmit">
            <DataAnnotationsValidator />
            <InputText @bind-Value="editContext.FirstName" />
            <button type="submit">Save</button>
            <button type="button" @onclick="() => grid.CancelCustomRowEdit(editContext)">Cancel</button>
        </EditForm>
    </EditFormTemplate>
</SmGrid>
```

---

### Row Selection

```razor
<SmGrid Data=employees
        SelectionMode=RowSelectionMode.Multiple
        @bind-MultipleSelectedDataRows=selectedRows
        Height="600px">
    <Columns>
        <Column Field="@nameof(Employee.FirstName)" />
        <Column Field="@nameof(Employee.LastName)" />
    </Columns>
</SmGrid>

@code {
    IEnumerable<Employee> selectedRows = [];
}
```

| Mode | Description |
|------|-------------|
| `RowSelectionMode.None` | No selection |
| `RowSelectionMode.Single` | One row at a time (bind with `@bind-SingleSelectedDataRow`) |
| `RowSelectionMode.Multiple` | Multiple rows with checkboxes (bind with `@bind-MultipleSelectedDataRows`) |

---

### Command Bar

Enable the command bar for bulk actions and data export:

```razor
<SmGrid Data=employees
        ShowCommandBar=true
        SelectionMode=RowSelectionMode.Multiple
        Height="600px">
    ...
</SmGrid>
```

The command bar provides:
- **Select current page** / **Select all filtered rows** / **Deselect all** buttons
- **Delete selected** rows
- **Export** dropdown: Excel (all / selected), CSV (all / selected)

---

### Cell Templates

Customize how individual cells render:

```razor
<Column Field="@nameof(Employee.Salary)">
    <CellShowTemplate>
        @{
            var salary = (context as Employee)!.Salary;
            <span class="@(salary > 100000 ? "text-success fw-bold" : "")">
                @salary.ToString("C")
            </span>
        }
    </CellShowTemplate>
</Column>
```

---

### Detail Rows

Expandable detail rows below each data row:

```razor
<SmGrid Data=employees ShowDetailRow=true Height="600px">
    <Columns>
        <Column Field="@nameof(Employee.FirstName)" />
        <Column Field="@nameof(Employee.LastName)" />
    </Columns>
    <DetailRowTemplate Context="item">
        @{
            var employee = item as Employee;
            <div>Email: @employee!.Email</div>
            <div>Phone: @employee.Phone</div>
        }
    </DetailRowTemplate>
</SmGrid>
```

Collapse all detail rows programmatically: `await grid.CollapseAllDetailRows();`

---

### Virtual Scrolling

For large datasets, use virtual scrolling instead of paging:

```razor
<SmGrid Data=employees DataNavigationMode=NavigationMode.Virtual>
    <Columns>
        <Column Field="@nameof(Employee.FirstName)" />
        <Column Field="@nameof(Employee.LastName)" />
    </Columns>
</SmGrid>
```

---

### Column Resizing

```razor
<SmGrid Data=employees ColumnResizeMode=GridColumnResizeMode.Sliding Height="600px">
    ...
</SmGrid>
```

| Mode | Description |
|------|-------------|
| `GridColumnResizeMode.None` | No resizing |
| `GridColumnResizeMode.Sliding` | Resize a column without affecting others |
| `GridColumnResizeMode.Block` | Resize a column, adjacent columns adjust |

---

### Styling

#### Bootstrap CSS classes

Apply any Bootstrap table classes via `CssClass` and `PaginationCssClass`:

```razor
<SmGrid Data=employees
        CssClass="table-bordered table-sm table-striped"
        PaginationCssClass="pagination-sm"
        Height="600px">
    ...
</SmGrid>
```

#### Grid size modes

```razor
<SmGrid Data=employees SizeMode=GridSizeMode.Small Height="600px">
    ...
</SmGrid>
```

| Mode | Description |
|------|-------------|
| `GridSizeMode.Default` | Standard spacing |
| `GridSizeMode.Small` | Compact spacing |
| `GridSizeMode.Large` | Generous spacing |

#### CSS custom properties

Override these CSS variables to customize the grid appearance:

```css
:root {
    --sm-row-selected-bg: rgba(69, 46, 222, 0.5);
    --sm-no-data-color: #999;
    --sm-detail-icon-size: 16px;
    --sm-scrollbar-size: 8px;
    --sm-scrollbar-thumb-color: #888;
    --sm-scrollbar-thumb-hover-color: #555;
    --sm-scrollbar-thumb-active-color: #333;
    --sm-scrollbar-track-color: #f1f1f1;
    --sm-scrollbar-thumb-radius: 4px;
    --sm-detail-row-expanded-icon: url('data:image/svg+xml;utf8,...');
    --sm-detail-row-collapsed-icon: url('data:image/svg+xml;utf8,...');
}
```

---

### Cell Content Alignment

```razor
<Column Field="@nameof(Employee.Salary)" TextAlign="GridTextAlign.Right" />
<Column Field="@nameof(Employee.FirstName)" TextAlign="GridTextAlign.Center" />
```

---

### SmGrid Parameters Reference

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Data` | `IEnumerable<T>` | — | Data source |
| `Height` | `string` | — | Grid height (CSS value) |
| `Width` | `string` | — | Grid width (CSS value) |
| `PageSize` | `uint` | `50` | Rows per page |
| `PagerSize` | `uint` | `10` | Pagination button count |
| `DataNavigationMode` | `NavigationMode` | `Paging` | Paging or Virtual |
| `FilterMode` | `GridFilterMode` | `None` | Filter mode |
| `EditMode` | `GridEditMode` | `None` | Edit mode |
| `SelectionMode` | `RowSelectionMode` | `None` | Row selection mode |
| `ColumnResizeMode` | `GridColumnResizeMode` | `None` | Column resize behavior |
| `SizeMode` | `GridSizeMode` | `Default` | Grid density |
| `CssClass` | `string` | — | Bootstrap table classes |
| `PaginationCssClass` | `string` | — | Bootstrap pagination classes |
| `ShowColumnHeader` | `bool?` | `true` | Show/hide column headers |
| `ShowDetailRow` | `bool?` | `false` | Enable detail rows |
| `ShowCommandBar` | `bool` | `false` | Show command bar |
| `AutoGenerateColumns` | `bool` | `false` | Auto-generate columns from model |
| `SingleSelectedDataRow` | `T?` | — | Selected row (single mode) |
| `MultipleSelectedDataRows` | `IEnumerable<T>?` | — | Selected rows (multiple mode) |

### Column Parameters Reference

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Field` | `string` | — | Property name from data model |
| `Title` | `string?` | Field name | Column header text |
| `Width` | `string?` | — | Column width (px, %, or flex like `"1*"`) |
| `TextAlign` | `GridTextAlign` | `Left` | Cell content alignment |
| `Resizable` | `bool` | `true` | Allow user resizing |
| `MinWidth` | `double` | `50` | Minimum column width in px |

### CommandColumn Parameters Reference

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Width` | `string` | — | Column width |
| `EditButtonVisible` | `bool?` | `true` | Show edit button |
| `NewButtonVisible` | `bool?` | `true` | Show insert button |
| `DeleteButtonVisible` | `bool?` | `true` | Show delete button |

---

### License

MIT
