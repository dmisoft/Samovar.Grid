using System.Globalization;
using System.Reflection;

namespace Samovar.Grid.Formulas;

public static class FormulaCompiler
{
    public static Func<T, decimal?> Compile<T>(FormulaNode root, IReadOnlyDictionary<string, PropertyInfo> propInfo)
    {
        var refs = new Dictionary<string, PropertyInfo>(StringComparer.Ordinal);
        if (!CollectFields(root, propInfo, refs))
            return _ => null;

        return row =>
        {
            if (row is null) return null;
            try
            {
                return root.Evaluate(name =>
                {
                    if (!refs.TryGetValue(name, out var pi)) return null;
                    var raw = pi.GetValue(row);
                    if (raw is null) return null;
                    try
                    {
                        return Convert.ToDecimal(raw, CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        return null;
                    }
                });
            }
            catch
            {
                return null;
            }
        };
    }

    private static bool CollectFields(FormulaNode node, IReadOnlyDictionary<string, PropertyInfo> propInfo, Dictionary<string, PropertyInfo> sink)
    {
        switch (node)
        {
            case FieldRef f:
                if (!propInfo.TryGetValue(f.Name, out var pi))
                    return false;
                sink[f.Name] = pi;
                return true;
            case BinaryOp b:
                return CollectFields(b.Left, propInfo, sink) && CollectFields(b.Right, propInfo, sink);
            case NumberLiteral:
                return true;
            default:
                return false;
        }
    }
}
