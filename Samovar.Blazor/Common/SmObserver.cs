using System;

namespace Samovar.Blazor
{
    public class SmObserver<T>
        : IObserver<T>
    {
        SmObserverDispatcher<T> _disp;

        T _value;

        public SmObserver(SmObserverDispatcher<T> disp)
        {
            _disp = disp;
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(T newValue)
        {
            if (newValue == null)
                return;

            if (_value == null || !_value.Equals(newValue)) {
                _value = newValue;
                _disp.Next(newValue);
            }
        }
    }

    public class SmObserverFunc<T>
        : IObserver<T>
    {
        SmObserverDispatcherFunc<T> _disp;

        T _value;

        public SmObserverFunc(SmObserverDispatcherFunc<T> disp)
        {
            _disp = disp;
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(T newValue)
        {
            if (_value is null || !_value.Equals(newValue))
            {
                _value = newValue;
                _disp.Next(newValue);
            }
        }
    }
}
