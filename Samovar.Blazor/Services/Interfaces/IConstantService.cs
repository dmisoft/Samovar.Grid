namespace Samovar.Blazor
{
    public interface IConstantService
    {
        string InnerGridId { get; }

        string DataGridId { get; }

        string GridHeaderContainerId { get; }

        string GridFilterContainerId { get; }

        string InnerGridBodyTableId { get; }

        string InnerGridBodyId { get; }

        string GridBodyId { get; }

    }
}
