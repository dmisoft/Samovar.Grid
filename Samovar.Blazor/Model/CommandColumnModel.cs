using System.Text.RegularExpressions;

namespace Samovar.Blazor
{
    public class CommandColumnModel
        : ColumnModelBase, ICommandColumnModel
    {
        public override DataGridColumnType ColumnType { get; } = DataGridColumnType.Command;

        public ISubject<bool> NewButtonVisible { get; } = new ParameterSubject<bool>(true);

        public ISubject<bool> EditButtonVisible { get; } = new ParameterSubject<bool>(true);

        public ISubject<bool> DeleteButtonVisible { get; } = new ParameterSubject<bool>(true);
    }
}
