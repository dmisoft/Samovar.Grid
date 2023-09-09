using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

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
        //public static Subscription Subscribe<T>(this IObservable<T> observable, Action<T> action)
        //{
        //    observable.
        //    return new SubjectBase(observable, action);
        //}
        public static void Subscribe1<T>(this IObservable<T> observable, Action<T> action)
        {
            
            //IObserver1<int, Action> o = new MyObserver();
            
            //var subject = new ActionSubject<T>(observable, action);
            //subject.Subscribe(o);
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

        //public static Task<IDisposable> Subscribe<T>(this IObservable<T> observable, Func<T, EventCallback<T>, Task> action)
        //{
        //    IDisposable unsubscriber = observable.Subscribe(new SmObserverFunc2<T>(new SmObserverDispatcherFunc2<T>(action)));
        //    return Task.FromResult(unsubscriber);
        //}

        //public static Task OnNext<T>(this IObserver<T> observer, T nextValue, EventCallback<T> callback)
        //{
        //    observer.OnNext(nextValue, callback);
        //    //callback.InvokeAsync(nextValue);
        //    return Task.CompletedTask;
        //}

        //public static IObservable<TMap> Map<T1, T2, TMap>(this IObservable<T1, T2> obs, Func<T1, T2, TMap> callback)
        //{
        //    return new ObservableMap2<T1, T2, TMap>(obs, callback);
        //}
        //public static IObservable<T1, T2> Observe<T1, T2>(IObservable<T1> o1, IObservable<T2> o2)
        //{
        //    return new Observable2<T1, T2>(o1, o2);
        //}
        //private class ObservableMap2<T1, T2, TOut> : IObservable<TOut>
        //{
        //    private readonly Func<T1, T2, TOut> _map;

        //    private readonly IObservable<T1, T2> _source;

        //    public ObservableMap2(IObservable<T1, T2> source, Func<T1, T2, TOut> map)
        //    {
        //        _source = source;
        //        _map = map;
        //    }

        //    IDisposable IObservable<TOut>.Subscribe(IObserver<TOut> observer)
        //    {
        //        return _source.Subscribe(new ObserverMap2<T1, T2, TOut>(observer, _map));
        //    }
        //}

        //private class ObserverMap2<T1, T2, TOut> : IObserver<T1, T2>
        //{
        //    private readonly IObserver<TOut> _target;

        //    private readonly Func<T1, T2, TOut> _map;

        //    public ObserverMap2(IObserver<TOut> target, Func<T1, T2, TOut> map)
        //    {
        //        _target = target;
        //        _map = map;
        //    }

        //    public void OnNext(T1 v1, T2 v2)
        //    {
        //        _target.OnNext(_map(v1, v2));
        //    }
        //}

    }

}
