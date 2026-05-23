using Microsoft.AspNetCore.Components;
using Samovar.Grid.Formulas;
using System.Reactive.Subjects;

namespace Samovar.Grid;

public partial class ExpressionColumnModel
    : DeclarativeColumnModel, IExpressionColumnModel
{
    public override ColumnType ColumnType { get; } = ColumnType.Expression;

    public BehaviorSubject<RenderFragment<object>?> CellShowTemplate { get; } = new(null);

    public BehaviorSubject<string> Field { get; } = new($"__expr_{Guid.NewGuid():N}");

    public BehaviorSubject<GridTextAlign> TextAlign { get; } = new(GridTextAlign.Right);

    public BehaviorSubject<string?> Format { get; } = new(null);

    public BehaviorSubject<int?> GroupIndex { get; } = new(null);

    public BehaviorSubject<string> Formula { get; } = new(string.Empty);

    public FormulaNode? Ast { get; private set; }

    public Delegate? CompiledGetter { get; set; }

    public ExpressionColumnModel()
    {
        Formula.Subscribe(OnFormulaChanged);
    }

    private void OnFormulaChanged(string formula)
    {
        CompiledGetter = null;
        if (string.IsNullOrWhiteSpace(formula))
        {
            Ast = null;
            return;
        }
        try
        {
            Ast = FormulaParser.Parse(formula);
        }
        catch (FormatException)
        {
            Ast = null;
        }
    }
}
