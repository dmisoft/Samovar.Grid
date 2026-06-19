# Data Provider Roadmap

This note records a design decision and a future direction for how Samovar.Grid sources its data.
**It is documentation only — none of the `IGridDataProvider` code below is implemented yet.**

## Background

Today the grid is purely **in-memory**. A consumer hands the grid an `IEnumerable<T>` it already
holds in memory (`DataSourceService.Data`), and all Select / Sort / Filter / Page operations run
client-side over that collection using LINQ, compiled property accessors, and dynamically-built
filter expression trees.

The in-memory engine has been optimized (compiled sort keys, hoisted filter `MethodInfo` lookups,
single materialization of the filtered+sorted set) so it performs well for typical client-side grid
sizes. It is the correct default and the only mode that works in **Blazor WebAssembly**.

## Why not an embedded database

Embedding a relational database (Postgres, or even an "embedded" engine such as SQLite / DuckDB)
into the grid was considered and **rejected**:

1. **Blazor WASM is a hard blocker.** Samovar.Grid ships as a single Razor Class Library consumed by
   both Server and WASM hosts. A database engine needs a native server process or native binaries
   that cannot run in the browser sandbox — it would break WASM entirely.
2. **It is a UI grid, not an application.** Consumers already hold their data in memory. Copying
   POCOs into a DB (schema-per-`T`, serialize in, run SQL, deserialize back) is *more* overhead and
   adds a large dependency to a deliberately lean library.
3. **Object identity would break.** Selection, editing, detail-row expansion, and drag&drop rely on
   the real `T` instances. A SQL round-trip returns *copies*, destroying reference equality.

## The future seam: `IGridDataProvider<T>`

For datasets that are genuinely large or unbounded (≫1M rows), the data realistically lives in a
real database **on the consumer's side**. The right answer is to let the consumer plug in a provider
that pushes filter + sort + page down to *their* server, rather than the grid loading everything into
memory and owning a database engine itself.

```csharp
public interface IGridDataProvider<T>
{
    Task<GridPage<T>> GetPageAsync(
        FilterSpec filter,
        SortSpec sort,
        int skip,
        int take,
        CancellationToken ct);
}

public sealed record GridPage<T>(IReadOnlyList<T> Items, int TotalCount);
```

- **Default provider** = the current optimized in-memory engine. Existing consumers and WASM apps are
  unaffected; no behaviour or API change.
- **Optional provider** = a consumer-supplied implementation (EF Core, a Web API client, etc.) that
  translates `FilterSpec` / `SortSpec` into a server-side query and returns only the requested page.

## Where the seam sits

The current pipeline already separates the two concerns the provider would sit behind:

- `DataSourceService<T>` owns **filter + sort** (produces the filtered/sorted `IQueryable<T>`).
- `RepositoryService<T>` owns **paging + row-model mapping** (`Skip`/`Take`, builds `GridRowModel<T>`).

A future `IGridDataProvider<T>` would replace the *source* of the filtered/sorted/paged rows behind
these two services, so the in-memory path stays the default and Blazor WASM keeps working with no
native dependencies.

`FilterSpec` / `SortSpec` would be neutral descriptions of the current filter cells
(`GridFilterCellInfo`) and `ColumnOrderInfo`, decoupled from the in-memory expression-tree builder so
a remote provider can translate them into its own query language.
