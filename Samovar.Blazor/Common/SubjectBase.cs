using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Samovar.Blazor
{

    public class ParameterActionObserver<T, TDelegate>
        : IObserver<T>
    {
        public ParameterActionObserver(TDelegate callback)
        {
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(T value)
        {
            throw new NotImplementedException();
        }
    }
    
    public class ParameterActionSubject<T>
        : SubjectBase<T>
    {
        EventCallback<T> _callback;
        public List<ParameterActionObserver<T, EventCallback<T>>> observers { get; set; } = new List<ParameterActionObserver<T, EventCallback<T>>>();

        public ParameterActionSubject(T defaultValue, EventCallback<T> callback)
            : base(defaultValue)
        {
            _callback = callback;
            //observers = new List<ParameterActionObserver<T, EventCallback<T>>>();
        }

        public override void OnNextParameterValue(T nextParameterValue, EventCallback<T> callback = default)
        {
            foreach (var observer in observers)
            {
                //observer.OnNext(nextParameterValue, callback);
            }
        }
    }


    public abstract class SubjectBase<T>
        : ISubject<T>
    {
        //public virtual List<IObserver<T>> observers { get; set; } = new List<IObserver<T>>();
        public virtual ConcurrentBag<IObserver<T>> observersCB { get; set; } = new ConcurrentBag<IObserver<T>>();

        public T SubjectValue { get; set; }

        public SubjectBase(T defaultValue)
        {
            SubjectValue = defaultValue;
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(T value)
        {
            throw new NotImplementedException();
        }

        public abstract void OnNextParameterValue(T nextParameterValue, EventCallback<T> callback = default);

        public IDisposable Subscribe(IObserver<T> observer)
        {
            //if (!observers.Contains(observer))
            //{
            //    observers.Add(observer);
            //}
            observersCB.Add(observer);
            return new Unsubscriber<T>(observersCB, observer);
        }
    }

    public class ParameterSubject<T>
        : SubjectBase<T>
    {
        public ParameterSubject()
            : base(default(T))
        {
        }

        public ParameterSubject(T defaultValue)
            : base(defaultValue)
        {
        }

        public override void OnNextParameterValue(T nextParameterValue, EventCallback<T> callback = default)
        {
            SubjectValue = nextParameterValue;

            foreach (var observer in observersCB)
            {
                observer.OnNext(nextParameterValue);
            }
        }
    }

    internal class Unsubscriber<T> : IDisposable
    {
        private ConcurrentBag<IObserver<T>> _observers;
        private IObserver<T> _observer;

        public Unsubscriber(ConcurrentBag<IObserver<T>> observers, IObserver<T> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            while (!_observers.IsEmpty)
            {
                IObserver<T> item;
                _observers.TryTake(out item);
            }

            //if (_observer != null && _observers.Contains(_observer))
            //    _observers.Remove(_observer);
        }
    }
}
