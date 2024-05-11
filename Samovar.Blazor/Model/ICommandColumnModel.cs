using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public interface ICommandColumnModel
        : IColumnModel
    {
        public BehaviorSubject<bool> NewButtonVisible { get; }

        public BehaviorSubject<bool> EditButtonVisible { get; }

        public BehaviorSubject<bool> DeleteButtonVisible { get; }
        public BehaviorSubject<string> Title { get; }
    }
}
