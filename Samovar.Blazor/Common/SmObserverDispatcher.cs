using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public class SmObserverDispatcher<T>
        : ObserverDispatcherBase<T>
    {
        Action<T> _action;
        public SmObserverDispatcher(Action<T> action)
        {
            _action = action;
        }
        public override void Init()
        {
            throw new NotImplementedException();
        }

        public override void Next(T newValue)
        {
            //if (newValue == null)
            //    return;

            //if (_value is null || !_value.Equals(newValue))
            //{
            //    _value = newValue;
            //    _disp.Next(newValue);
            //}

            _action(newValue);
        }

        public override void Reset()
        {
            throw new NotImplementedException();
        }
    }

    public class SmObserverDispatcherFunc<T>
        : ObserverDispatcherBase<T>
    {
        Func<T, Task> _action;

        public SmObserverDispatcherFunc(Func<T, Task> action)
        {
            _action = action;
        }
        public override void Init()
        {
            throw new NotImplementedException();
        }

        public override void Next(T newData)
        {
            _action.Invoke(newData);
        }

        public override void Reset()
        {
            throw new NotImplementedException();
        }
    }

    //public class SmObserverDispatcherFunc2<T>
    //: ObserverDispatcherBase<T>
    //{
    //    Func<T, EventCallback<T>, Task> _action;

    //    public SmObserverDispatcherFunc2(Func<T, EventCallback<T>, Task> action)
    //    {
    //        _action = action;
    //    }
    //    public override void Init()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override void Next(T newData, EventCallback<T> eventCallback)
    //    {
    //        _action.Invoke(newData, eventCallback);
    //    }

    //    public override void Reset()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
