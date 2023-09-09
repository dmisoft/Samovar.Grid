using System;
using System.Collections.Generic;

namespace Samovar.DataGrid
{
    public class ParameterReporter<T>
        : IObserver<IEnumerable<T>>
    {
        private IDisposable unsubscriber;
        private string instName;

        public ParameterReporter(string name)
        {
            instName = name;
        }

        public string Name
        { get { return instName; } }

        public virtual void Subscribe(IObservable<IEnumerable<T>> provider)
        {
            if (provider != null)
                unsubscriber = provider.Subscribe(this);
        }

        public virtual void OnCompleted()
        {
            Console.WriteLine("The Location Tracker has completed transmitting data to {0}.", this.Name);
            Unsubscribe();
        }

        public virtual void OnError(Exception e)
        {
            //Console.WriteLine("{0}: The location cannot be determined.", this.Name);
        }

        public virtual void OnNext(IEnumerable<T> value)
        {
            //Console.WriteLine("{2}: The current location is {0}, {1}", value.Latitude, value.Longitude, this.Name);
        }

        public virtual void Unsubscribe()
        {
            unsubscriber.Dispose();
        }
    }

    public class ParameterTracker<T> 
        : IObservable<IEnumerable<T>>
    {
        public ParameterTracker()
        {
            observers = new List<IObserver<IEnumerable<T>>>();
        }

        private List<IObserver<IEnumerable<T>>> observers;

        public IDisposable Subscribe(IObserver<IEnumerable<T>> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<IEnumerable<T>>> _observers;
            private IObserver<IEnumerable<T>> _observer;

            public Unsubscriber(List<IObserver<IEnumerable<T>>> observers, IObserver<IEnumerable<T>> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }

        public void TrackLocation(IEnumerable<T> param)
        {
            foreach (var observer in observers)
            {
                if (param == null)
                    observer.OnError(new LocationUnknownException());
                else
                    observer.OnNext(param);
            }
        }

        public void EndTransmission()
        {
            foreach (var observer in observers.ToArray())
                if (observers.Contains(observer))
                    observer.OnCompleted();

            observers.Clear();
        }
    }
    public class LocationUnknownException : Exception
    {
        internal LocationUnknownException()
        { }
    }
}
