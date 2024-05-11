namespace Samovar.Blazor.Filter
{
    public partial class DataGridFilterRow<TItem>
        : SmDesignComponentBase, IAsyncDisposable
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [SmInject]
        public IColumnService ColumnService { get; set; }

        [SmInject]
        public ILayoutService LayoutService { get; set; }

        [SmInject]
        public IFilterService FilterService { get; set; }

        [SmInject]
        public IRepositoryService<TItem> RepositoryService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected readonly List<Type> Numeric_Types_For_Constant_Expression = new List<Type>

                {
                    typeof(byte),
                    typeof(byte?),
                    typeof(sbyte),
                    typeof(sbyte?),
                    typeof(short),
                    typeof(short?),
                    typeof(ushort),
                    typeof(ushort?),
                    typeof(int),
                    typeof(int?),
                    typeof(uint),
                    typeof(uint?),
                    typeof(long),
                    typeof(long?),
                    typeof(ulong),
                    typeof(ulong?),
                    typeof(float),
                    typeof(float?),
                    typeof(double),
                    typeof(double?),
                    typeof(decimal),
                    typeof(decimal?),
            };

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
