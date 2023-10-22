using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public class CommandColumnModel
        : ColumnModelBase, ICommandColumnModel
    {
        public override DataGridColumnType ColumnType { get; } = DataGridColumnType.Command;

        public ISubject<bool> NewButtonVisible { get; } = new BehaviorSubject<bool>(true);

        public ISubject<bool> EditButtonVisible { get; } = new BehaviorSubject<bool>(true);

        public ISubject<bool> DeleteButtonVisible { get; } = new BehaviorSubject<bool>(true);
    }
}
