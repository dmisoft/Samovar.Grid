using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Samovar.Grid.ZLabor
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
    class JobA : IJob
    {
    }

    class JobB : IJob
    {
    }

    public static class Global
    {
        public static int Counter = 0;
    }

    public class RXTest {
        public void Start()
        {
            var q = new RxQueuePubSub();

            q.RegisterHandler<JobA>(j => Console.WriteLine(Global.Counter));
            q.RegisterHandler<JobB>(j => Global.Counter++);
            
            q.Enqueue(new JobA());//print
            q.Enqueue(new JobB());//add
            q.Enqueue(new JobA());//print
            q.Enqueue(new JobB());//add
            q.Enqueue(new JobB());//add
            q.Enqueue(new JobA());//print
        }
    }
    
}
