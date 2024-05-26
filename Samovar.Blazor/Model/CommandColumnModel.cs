using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public class CommandColumnModel
        : ColumnModelBase, ICommandColumnModel
    {
        public override ColumnType ColumnType { get; } = ColumnType.Command;

        public BehaviorSubject<bool> NewButtonVisible { get; } = new BehaviorSubject<bool>(true);

        public BehaviorSubject<bool> EditButtonVisible { get; } = new BehaviorSubject<bool>(true);

        public BehaviorSubject<bool> DeleteButtonVisible { get; } = new BehaviorSubject<bool>(true);
    }
}
