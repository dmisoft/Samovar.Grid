using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public class CommandColumnModel
        : ColumnModelBase, ICommandColumnModel
    {
        public override DataGridColumnType ColumnType { get; } = DataGridColumnType.Command;

        public BehaviorSubject<bool> NewButtonVisible { get; } = new BehaviorSubject<bool>(true);

        public BehaviorSubject<bool> EditButtonVisible { get; } = new BehaviorSubject<bool>(true);

        public BehaviorSubject<bool> DeleteButtonVisible { get; } = new BehaviorSubject<bool>(true);
    }
}
