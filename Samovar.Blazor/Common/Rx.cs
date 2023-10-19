using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
//commit test

namespace Samovar.Blazor
{
    public interface IObserver<T1, T2, Action>
    {
        void OnNext(T1 v1, T2 v2);
    }
    public interface IObserver1<T1, T2>
    {
        void OnNext(T1 v, T2 v2);
    }
    public interface IObservable<T1, T2>
    {
        IDisposable Subscribe(IObserver<T1, T2, Action> observer);
    }

    public class Observable2<T1, T2> : IObservable<T1, T2>
    {
        private IObservable<T1> _o1;

        private IObservable<T2> _o2;

        public Observable2(IObservable<T1> o1, IObservable<T2> o2)
        {
            _o1 = o1 ?? throw new ArgumentException();
            _o2 = o2 ?? throw new ArgumentException();
        }

        //Return an unsubscriber
        IDisposable IObservable<T1, T2>.Subscribe(IObserver<T1, T2, Action> observer)
        {

            return null;
            //return Join(_o1, _o2, observer.OnNext);
        }
    }
    
    public abstract class ObserverDispatcherBase<T>
    {
        public SubjectBase<T> ControlSubject { get; set; }

        public abstract void Init();
        public abstract void Reset();
        public abstract void Next(T newData);
    }

    public static class ObservableExtension
    {
        private static IDisposable Join<T1, T2>(IObservable<T1> o1, IObservable<T2> o2, Action<T1, T2> update, T1 v1 = default(T1), T2 v2 = default(T2), int remainInitCount = 2, int updateLock = 0)
        {
            return null;
            //return AsSingleDisposable<IDisposable>(o1.SubscribeAggregation(subscription1, beginUpdate, endUpdate), o2.SubscribeAggregation(subscription2, beginUpdate, endUpdate));
            //void beginUpdate()
            //{
            //    updateLock++;
            //}
            //void callUpdate()
            //{
            //    if (remainInitCount == 0)
            //    {
            //        if (updateLock == 0)
            //        {
            //            update(v1, v2);
            //        }
            //    }
            //    else
            //    {
            //        remainInitCount--;
            //    }
            //}
            //void endUpdate()
            //{
            //    updateLock--;
            //}
            //void subscription1(T1 v)
            //{
            //    v1 = v;
            //    callUpdate();
            //}
            //void subscription2(T2 v)
            //{
            //    v2 = v;
            //    callUpdate();
            //}

        }

        public static void Subscribe1<T>(this IObservable<T> observable, Action<T> action)
        {
        }
        public static Task<IDisposable> Subscribe<T>(this IObservable<T> observable, Func<T, Task> action)
        {
            IDisposable unsubscriber = observable.Subscribe(new SmObserverFunc<T>(new SmObserverDispatcherFunc<T>(action)));
            return Task.FromResult(unsubscriber);
        }

        public static Task<IDisposable> Subscribe<T>(this IObservable<T> observable, Action<T> action)
        {
            IDisposable unsubscriber = observable.Subscribe(new SmObserver<T>(new SmObserverDispatcher<T>(action)));
            return Task.FromResult(unsubscriber);
        }
    }
}
