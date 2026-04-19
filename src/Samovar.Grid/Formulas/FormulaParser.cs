using System.Globalization;
using System.Text;

namespace Samovar.Grid.Formulas;

public static class FormulaParser
{
    public static FormulaNode Parse(string formula)
    {
        if (string.IsNullOrWhiteSpace(formula))
            throw new FormatException("Formula is empty.");

        var parser = new Parser(formula);
        var node = parser.ParseExpr();
        parser.ExpectEnd();
        return node;
    }

    private sealed class Parser(string text)
    {
        private int _pos;

        public FormulaNode ParseExpr()
        {
            var left = ParseTerm();
            while (true)
            {
                SkipWs();
                if (_pos >= text.Length) break;
                var c = text[_pos];
                if (c != '+' && c != '-') break;
                _pos++;
                var right = ParseTerm();
                left = new BinaryOp(c, left, right);
            }
            return left;
        }

        private FormulaNode ParseTerm()
        {
            var left = ParseFactor();
            while (true)
            {
                SkipWs();
                if (_pos >= text.Length) break;
                var c = text[_pos];
                if (c != '*' && c != '/') break;
                _pos++;
                var right = ParseFactor();
                left = new BinaryOp(c, left, right);
            }
            return left;
        }

        private FormulaNode ParseFactor()
        {
            SkipWs();
            if (_pos >= text.Length)
                throw new FormatException("Unexpected end of formula.");

            var c = text[_pos];
            if (c == '(')
            {
                _pos++;
                var inner = ParseExpr();
                SkipWs();
                if (_pos >= text.Length || text[_pos] != ')')
                    throw new FormatException("Missing ')'.");
                _pos++;
                return inner;
            }

            if (c == '[')
            {
                _pos++;
                var sb = new StringBuilder();
                while (_pos < text.Length && text[_pos] != ']')
                {
                    sb.Append(text[_pos]);
                    _pos++;
                }
                if (_pos >= text.Length)
                    throw new FormatException("Missing ']'.");
                _pos++;
                var name = sb.ToString().Trim();
                if (name.Length == 0)
                    throw new FormatException("Empty field reference.");
                return new FieldRef(name);
            }

            if (char.IsDigit(c) || c == '.')
            {
                var start = _pos;
                while (_pos < text.Length && (char.IsDigit(text[_pos]) || text[_pos] == '.'))
                    _pos++;
                var numText = text.Substring(start, _pos - start);
                if (!decimal.TryParse(numText, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                    throw new FormatException($"Invalid number '{numText}'.");
                return new NumberLiteral(value);
            }

            throw new FormatException($"Unexpected character '{c}' at position {_pos}.");
        }

        public void ExpectEnd()
        {
            SkipWs();
            if (_pos < text.Length)
                throw new FormatException($"Unexpected trailing content at position {_pos}.");
        }

        private void SkipWs()
        {
            while (_pos < text.Length && char.IsWhiteSpace(text[_pos]))
                _pos++;
        }
    }
}
