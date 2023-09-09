using Samovar.Grid.Prototypes.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Samovar.Grid.ZPrototypes.Data
{
    public interface IJob
    {
    }

    public class RxQueuePubSub
    {
        Subject<IJob> _jobs = new Subject<IJob>();
        private IConnectableObservable<IJob> _connectableObservable;

        public RxQueuePubSub()
        {
            _connectableObservable = _jobs.ObserveOn(Scheduler.Default).Publish();
            _connectableObservable.Connect();
        }

        public void Enqueue(IJob job)
        {
            _jobs.OnNext(job);
        }

        public void RegisterHandler<T>(Action<T> handleAction) where T : IJob
        {
            _connectableObservable.OfType<T>().Subscribe(handleAction);
        }
    }


    //Usage
    internal class JobA<T> 
        : IJob
    {
        public IEnumerable<T> Data { get; set; }
        public DataGridTest<T> Grid { get; set; }
    }

    public class Rx<T> {
        public RxQueuePubSub rxQueuePubSub = new RxQueuePubSub();
        public ViewModelService<T> ViewModelSrc { get; } = new ViewModelService<T>();

        public Rx()
        {
            rxQueuePubSub.RegisterHandler<JobA<T>>(j =>
            {
                var temp = new List<T>();
                foreach (var item in j.Data)
                {
                    temp.Add(item);
                }
                ViewModelSrc.ViewModel = temp;
                j.Grid.Refresh();
            });
        }
    }

    public class ViewModelService<T> {
        public List<T> ViewModel { get; set; } = new List<T>();
    }
}
