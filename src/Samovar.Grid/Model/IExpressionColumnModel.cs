using Samovar.Grid.Formulas;
using System.Reactive.Subjects;

namespace Samovar.Grid;

public interface IExpressionColumnModel
    : IDataColumnModel
{
    BehaviorSubject<string> Formula { get; }

    FormulaNode? Ast { get; }

    Delegate? CompiledGetter { get; set; }
}
