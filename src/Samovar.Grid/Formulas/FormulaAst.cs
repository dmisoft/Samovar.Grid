namespace Samovar.Grid.Formulas;

public abstract record FormulaNode
{
    public abstract decimal? Evaluate(Func<string, decimal?> resolve);
}

public sealed record NumberLiteral(decimal Value) : FormulaNode
{
    public override decimal? Evaluate(Func<string, decimal?> resolve) => Value;
}

public sealed record FieldRef(string Name) : FormulaNode
{
    public override decimal? Evaluate(Func<string, decimal?> resolve) => resolve(Name);
}

public sealed record BinaryOp(char Op, FormulaNode Left, FormulaNode Right) : FormulaNode
{
    public override decimal? Evaluate(Func<string, decimal?> resolve)
    {
        var l = Left.Evaluate(resolve);
        if (l is null) return null;
        var r = Right.Evaluate(resolve);
        if (r is null) return null;

        return Op switch
        {
            '+' => l + r,
            '-' => l - r,
            '*' => l * r,
            '/' => r == 0m ? null : l / r,
            _ => null
        };
    }
}
