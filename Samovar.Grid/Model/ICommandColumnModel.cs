using System.Reactive.Subjects;

namespace Samovar.Grid;
public interface ICommandColumnModel
    : IDeclarativeColumnModel
{
    public BehaviorSubject<bool> NewButtonVisible { get; }
    public BehaviorSubject<bool> EditButtonVisible { get; }
    public BehaviorSubject<bool> DeleteButtonVisible { get; }
}
