
namespace Samovar.Grid
{
    public class ConstantService
        : IConstantService, IAsyncDisposable
    {
        public string OuterGridId { get; } = $"outergrid{Guid.NewGuid().ToString().Replace("-", "")}";
        public string InnerGridId { get; } = $"innergrid{Guid.NewGuid().ToString().Replace("-", "")}";

        public string DataGridId { get; } = $"samovargrid{Guid.NewGuid().ToString().Replace("-", "")}";

        public string GridHeaderContainerId { get; } = $"gridheadercontainer{Guid.NewGuid().ToString().Replace("-", "")}";

        public string GridFilterContainerId { get; } = $"gridfiltercontainer{Guid.NewGuid().ToString().Replace("-", "")}";

        public string InnerGridBodyTableId { get; } = $"innergridbodytable{Guid.NewGuid().ToString().Replace("-", "")}";

        public string InnerGridBodyId { get; } = $"innergridbody{Guid.NewGuid().ToString().Replace("-", "")}";

        public string GridBodyId { get; } = $"gridbody{Guid.NewGuid().ToString().Replace("-", "")}";

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
