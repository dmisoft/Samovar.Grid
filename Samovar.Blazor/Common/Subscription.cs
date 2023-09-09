using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samovar.Blazor
{
    public class Subscription1<T1, TOut>
    {
        private ISubject<T1> _subject1;
        private IDisposable _unsubscriber1;
        private ISubject<TOut> _mapSubject;
        private Func<T1, TOut> _callback;

        public Subscription1(ISubject<T1> subject1, Func<T1, TOut> callback)
        {
            _callback = callback;
            _subject1 = subject1;
        }
        public ISubject<TOut> CreateMap()
        {
            Attach();
            _mapSubject = new ParameterSubject<TOut>();
            return _mapSubject;
        }
        private void callback1(T1 obj)
        {
            _mapSubject.OnNextParameterValue(_callback(_subject1.SubjectValue));
        }

        public void Attach()
        {
            _unsubscriber1 = _subject1.Subscribe(new SmObserver<T1>(new SmObserverDispatcher<T1>(callback1)));
        }

        public void Detach()
        {
            _unsubscriber1.Dispose();
        }
    }
    public class Subscription2<T1, T2, TOut>
    {
        private ISubject<T1> _subject1;
        private ISubject<T2> _subject2;
        
        private IDisposable _unsubscriber1;
        private IDisposable _unsubscriber2;

        private ISubject<TOut> _mapSubject;
        private Func<T1, T2, TOut> _callback;
        public Subscription2(ISubject<T1> subject1, ISubject<T2> subject2, Func<T1, T2, TOut> callback)
        {
            _callback = callback;
            _subject1 = subject1;
            _subject2 = subject2;
        }
        public ISubject<TOut> CreateMap(TOut defaultValue = default(TOut))
        {
            Attach();
            _mapSubject = new ParameterSubject<TOut>(defaultValue);
            return _mapSubject;
        }
        public void Attach()
        {
            _unsubscriber1 = _subject1.Subscribe(new SmObserver<T1>(new SmObserverDispatcher<T1>(callback1)));
            _unsubscriber2 = _subject2.Subscribe(new SmObserver<T2>(new SmObserverDispatcher<T2>(callback2)));
        }

        public void Detach()
        {
            _unsubscriber1.Dispose();
            _unsubscriber2.Dispose();
        }
        private void callback1(T1 obj)
        {
            _mapSubject.OnNextParameterValue(_callback(_subject1.SubjectValue, _subject2.SubjectValue));
        }
        private void callback2(T2 obj)
        {
            _mapSubject.OnNextParameterValue(_callback(_subject1.SubjectValue, _subject2.SubjectValue));
        }
    }

    public class Subscription3<T1, T2, T3, TOut>
    {
        private ISubject<T1> _subject1;
        private ISubject<T2> _subject2;
        private ISubject<T3> _subject3;
        private ISubject<TOut> _mapSubject;
        private Func<T1, T2, T3, TOut> _callback;
        public Subscription3(ISubject<T1> subject1, ISubject<T2> subject2, ISubject<T3> subject3, Func<T1,T2,T3,TOut> callback)
        {
            _callback = callback;
            _subject1 = subject1;
            _subject2 = subject2;
            _subject3 = subject3;
            _subject1.Subscribe(new SmObserver<T1>(new SmObserverDispatcher<T1>(callback1)));
            _subject2.Subscribe(new SmObserver<T2>(new SmObserverDispatcher<T2>(callback2)));
            _subject3.Subscribe(new SmObserver<T3>(new SmObserverDispatcher<T3>(callback3)));
        }
        public ISubject<TOut> CreateMap()
        {
            _mapSubject = new ParameterSubject<TOut>();
            return _mapSubject;
        }
        private void callback1(T1 obj)
        {
            _mapSubject?.OnNextParameterValue(_callback(_subject1.SubjectValue, _subject2.SubjectValue, _subject3.SubjectValue));
        }
        private void callback2(T2 obj)
        {
            _mapSubject?.OnNextParameterValue(_callback(_subject1.SubjectValue, _subject2.SubjectValue, _subject3.SubjectValue));
        }

        private void callback3(T3 obj)
        {
            _mapSubject?.OnNextParameterValue(_callback(_subject1.SubjectValue, _subject2.SubjectValue, _subject3.SubjectValue));
        }

    }

    public class Subscription4<T1, T2, T3, T4, TOut>
    {
        private ISubject<T1> _subject1;
        private ISubject<T2> _subject2;
        private ISubject<T3> _subject3;
        private ISubject<T4> _subject4;
        private ISubject<TOut> _mapSubject;
        private Func<T1, T2, T3, T4, TOut> _callback;
        public Subscription4(ISubject<T1> subject1, ISubject<T2> subject2, ISubject<T3> subject3, ISubject<T4> subject4, Func<T1, T2, T3, T4, TOut> callback)
        {
            _callback = callback;
            _subject1 = subject1;
            _subject2 = subject2;
            _subject3 = subject3;
            _subject4 = subject4;
            _subject1.Subscribe(new SmObserver<T1>(new SmObserverDispatcher<T1>(callback1)));
            _subject2.Subscribe(new SmObserver<T2>(new SmObserverDispatcher<T2>(callback2)));
            _subject3.Subscribe(new SmObserver<T3>(new SmObserverDispatcher<T3>(callback3)));
            _subject4.Subscribe(new SmObserver<T4>(new SmObserverDispatcher<T4>(callback4)));
        }
        public ISubject<TOut> CreateMap()
        {
            _mapSubject = new ParameterSubject<TOut>();
            return _mapSubject;
        }
        private void callback1(T1 obj)
        {
            _mapSubject?.OnNextParameterValue(_callback(_subject1.SubjectValue, _subject2.SubjectValue, _subject3.SubjectValue, _subject4.SubjectValue));
        }
        private void callback2(T2 obj)
        {
            _mapSubject?.OnNextParameterValue(_callback(_subject1.SubjectValue, _subject2.SubjectValue, _subject3.SubjectValue, _subject4.SubjectValue));
        }

        private void callback3(T3 obj)
        {
            _mapSubject?.OnNextParameterValue(_callback(_subject1.SubjectValue, _subject2.SubjectValue, _subject3.SubjectValue, _subject4.SubjectValue));
        }

        private void callback4(T4 obj)
        {
            _mapSubject?.OnNextParameterValue(_callback(_subject1.SubjectValue, _subject2.SubjectValue, _subject3.SubjectValue, _subject4.SubjectValue));
        }
    }

    public class Subscription1Task<T1, TOut>
    {
        private ISubject<T1> _subject1;
        private ISubject<Task<TOut>> _mapSubject;
        private Func<T1, Task<TOut>> _callback;
        public Subscription1Task(ISubject<T1> subject1, Func<T1, Task<TOut>> callback)
        {
            _callback = callback;
            _subject1 = subject1;
            _subject1.Subscribe(new SmObserver<T1>(new SmObserverDispatcher<T1>(callback1)));
        }
        public ISubject<Task<TOut>> CreateMap()
        {
            _mapSubject = new ParameterSubject<Task<TOut>>();
            return _mapSubject;
        }
        private void callback1(T1 obj)
        {
            _mapSubject?.OnNextParameterValue(_callback(_subject1.SubjectValue));
        }
    }

    public class Subscription2Task<T1, T2, TOut>
    {
        private ISubject<T1> _subject1;
        private ISubject<T2> _subject2;
        private ISubject<Task<TOut>> _mapSubject;
        private Func<T1, T2, Task<TOut>> _callback;
        public Subscription2Task(ISubject<T1> subject1, ISubject<T2> subject2, Func<T1, T2, Task<TOut>> callback)
        {
            _callback = callback;
            _subject1 = subject1;
            _subject2 = subject2;
            _subject1.Subscribe(new SmObserver<T1>(new SmObserverDispatcher<T1>(callback1)));
            _subject2.Subscribe(new SmObserver<T2>(new SmObserverDispatcher<T2>(callback2)));
        }
        public ISubject<Task<TOut>> CreateMap()
        {
            _mapSubject = new ParameterSubject<Task<TOut>>();
            return _mapSubject;
        }
        private void callback1(T1 obj)
        {
            _mapSubject?.OnNextParameterValue(_callback(_subject1.SubjectValue, _subject2.SubjectValue));
        }
        private void callback2(T2 obj)
        {
            _mapSubject?.OnNextParameterValue(_callback(_subject1.SubjectValue, _subject2.SubjectValue));
        }
    }

    public class Subscription1TaskVoid<T1>
    {
        private ISubject<T1> _subject1;
        private ISubject<Task> _mapSubject;
        private Func<T1, Task> _callback;
        public Subscription1TaskVoid(ISubject<T1> subject1, Func<T1, Task> callback)
        {
            _callback = callback;
            _subject1 = subject1;
            _subject1.Subscribe(new SmObserver<T1>(new SmObserverDispatcher<T1>(callback1)));
        }
        public ISubject<Task> CreateMap()
        {
            _mapSubject = new ParameterSubject<Task>();
            return _mapSubject;
        }
        private void callback1(T1 obj)
        {
            _mapSubject?.OnNextParameterValue(_callback(_subject1.SubjectValue));
        }
    }

    public class Subscription2TaskVoid<T1, T2>
    {
        private ISubject<T1> _subject1;
        private ISubject<T2> _subject2;
        private ISubject<Task> _mapSubject;
        private Func<T1, T2, Task> _callback;
        public Subscription2TaskVoid(ISubject<T1> subject1, ISubject<T2> subject2, Func<T1, T2, Task> callback)
        {
            _callback = callback;
            _subject1 = subject1;
            _subject2 = subject2;
            _subject1.Subscribe(new SmObserver<T1>(new SmObserverDispatcher<T1>(callback1)));
            _subject2.Subscribe(new SmObserver<T2>(new SmObserverDispatcher<T2>(callback2)));
        }
        public ISubject<Task> CreateMap()
        {
            _mapSubject = new ParameterSubject<Task>();
            return _mapSubject;
        }
        private void callback1(T1 obj)
        {
            _mapSubject?.OnNextParameterValue(_callback(_subject1.SubjectValue, _subject2.SubjectValue));
        }
        private void callback2(T2 obj)
        {
            _mapSubject?.OnNextParameterValue(_callback(_subject1.SubjectValue, _subject2.SubjectValue));
        }
    }

    public class Subscription3TaskVoid<T1, T2, T3>
    {
        private ISubject<T1> _subject1;
        private ISubject<T2> _subject2;
        private ISubject<T3> _subject3;

        private IDisposable _unsubscriber1;
        private IDisposable _unsubscriber2;
        private IDisposable _unsubscriber3;

        private ISubject<Task> _mapSubject;
        private Func<T1, T2, T3, Task> _callback;
        public Subscription3TaskVoid(ISubject<T1> subject1, ISubject<T2> subject2, ISubject<T3> subject3, Func<T1, T2, T3, Task> callback)
        {
            _callback = callback;

            _subject1 = subject1;
            _subject2 = subject2;
            _subject3 = subject3;

            //_subject1.Subscribe(new SmObserver<T1>(new SmObserverDispatcher<T1>(callback1)));
            //_subject2.Subscribe(new SmObserver<T2>(new SmObserverDispatcher<T2>(callback2)));
            //_subject3.Subscribe(new SmObserver<T3>(new SmObserverDispatcher<T3>(callback3)));
        }
        public ISubject<Task> CreateMap()
        {
            Attach();

            _mapSubject = new ParameterSubject<Task>();
            return _mapSubject;
        }
        private void callback1(T1 obj)
        {
            _mapSubject?.OnNextParameterValue(_callback(_subject1.SubjectValue, _subject2.SubjectValue, _subject3.SubjectValue));
        }
        private void callback2(T2 obj)
        {
            _mapSubject?.OnNextParameterValue(_callback(_subject1.SubjectValue, _subject2.SubjectValue, _subject3.SubjectValue));
        }
        private void callback3(T3 obj)
        {
            _mapSubject?.OnNextParameterValue(_callback(_subject1.SubjectValue, _subject2.SubjectValue, _subject3.SubjectValue));
        }

        public void Attach()
        {
            _unsubscriber1 = _subject1.Subscribe(new SmObserver<T1>(new SmObserverDispatcher<T1>(callback1)));
            _unsubscriber2 = _subject2.Subscribe(new SmObserver<T2>(new SmObserverDispatcher<T2>(callback2)));
            _unsubscriber3 = _subject3.Subscribe(new SmObserver<T3>(new SmObserverDispatcher<T3>(callback3)));
        }

        public void Detach()
        {
            _unsubscriber1.Dispose();
            _unsubscriber2.Dispose();
            _unsubscriber3.Dispose();
        }
    }

    public class Subscription4TaskVoid<T1, T2, T3, T4>
    {
        private ISubject<T1> _subject1;
        private ISubject<T2> _subject2;
        private ISubject<T3> _subject3;
        private ISubject<T4> _subject4;

        private IDisposable _unsubscriber1;
        private IDisposable _unsubscriber2;
        private IDisposable _unsubscriber3;
        private IDisposable _unsubscriber4;

        private ISubject<Task> _mapSubject;
        private Func<T1, T2, T3, T4, Task> _callback;
        public Subscription4TaskVoid(ISubject<T1> subject1, ISubject<T2> subject2, ISubject<T3> subject3, ISubject<T4> subject4, Func<T1, T2, T3, T4, Task> callback)
        {
            _callback = callback;

            _subject1 = subject1;
            _subject2 = subject2;
            _subject3 = subject3;
            _subject4 = subject4;
        }

        public ISubject<Task> CreateMap()
        {
            Attach();

            _mapSubject = new ParameterSubject<Task>();
            return _mapSubject;
        }
        private void callback1(T1 obj)
        {
            _mapSubject?.OnNextParameterValue(_callback(_subject1.SubjectValue, _subject2.SubjectValue, _subject3.SubjectValue, _subject4.SubjectValue));
        }
        private void callback2(T2 obj)
        {
            _mapSubject?.OnNextParameterValue(_callback(_subject1.SubjectValue, _subject2.SubjectValue, _subject3.SubjectValue, _subject4.SubjectValue));

        }
        private void callback3(T3 obj)
        {
            _mapSubject?.OnNextParameterValue(_callback(_subject1.SubjectValue, _subject2.SubjectValue, _subject3.SubjectValue, _subject4.SubjectValue));
        }

        private void callback4(T4 obj)
        {
            _mapSubject?.OnNextParameterValue(_callback(_subject1.SubjectValue, _subject2.SubjectValue, _subject3.SubjectValue, _subject4.SubjectValue));
        }

        public void Attach()
        {
            _unsubscriber1 = _subject1.Subscribe(new SmObserver<T1>(new SmObserverDispatcher<T1>(callback1)));
            _unsubscriber2 = _subject2.Subscribe(new SmObserver<T2>(new SmObserverDispatcher<T2>(callback2)));
            _unsubscriber3 = _subject3.Subscribe(new SmObserver<T3>(new SmObserverDispatcher<T3>(callback3)));
            _unsubscriber4 = _subject4.Subscribe(new SmObserver<T4>(new SmObserverDispatcher<T4>(callback4)));
        }

        public void Detach()
        {
            _unsubscriber1.Dispose();
            _unsubscriber2.Dispose();
            _unsubscriber3.Dispose();
            _unsubscriber4.Dispose();
        }
    }

    public class Subscription3Task<T1, T2, T3, TOut>
    {
        private ISubject<T1> _subject1;
        private ISubject<T2> _subject2;
        private ISubject<T3> _subject3;
        private ISubject<Task<TOut>> _mapSubject;
        private Func<T1, T2, T3, Task<TOut>> _callback;
        public Subscription3Task(ISubject<T1> subject1, ISubject<T2> subject2, ISubject<T3> subject3, Func<T1, T2, T3, Task<TOut>> callback)
        {
            _callback = callback;
            _subject1 = subject1;
            _subject2 = subject2;
            _subject3 = subject3;
            _subject1.Subscribe(new SmObserver<T1>(new SmObserverDispatcher<T1>(callback1)));
            _subject2.Subscribe(new SmObserver<T2>(new SmObserverDispatcher<T2>(callback2)));
            _subject3.Subscribe(new SmObserver<T3>(new SmObserverDispatcher<T3>(callback3)));
        }
        public ISubject<Task<TOut>> CreateMap()
        {
            _mapSubject = new ParameterSubject<Task<TOut>>();
            return _mapSubject;
        }
        private void callback1(T1 obj)
        {
            _mapSubject?.OnNextParameterValue(_callback(_subject1.SubjectValue, _subject2.SubjectValue, _subject3.SubjectValue));
        }
        private void callback2(T2 obj)
        {
            _mapSubject?.OnNextParameterValue(_callback(_subject1.SubjectValue, _subject2.SubjectValue, _subject3.SubjectValue));
        }

        private void callback3(T3 obj)
        {
            _mapSubject?.OnNextParameterValue(_callback(_subject1.SubjectValue, _subject2.SubjectValue, _subject3.SubjectValue));
        }
    }

    public class Subscription4Task<T1, T2, T3, T4, TOut>
    {
        private ISubject<T1> _subject1;
        private ISubject<T2> _subject2;
        private ISubject<T3> _subject3;
        private ISubject<T4> _subject4;

        private IDisposable _unsubscriber1;
        private IDisposable _unsubscriber2;
        private IDisposable _unsubscriber3;
        private IDisposable _unsubscriber4;

        private ISubject<Task<TOut>> _mapSubject;
        private Func<T1, T2, T3, T4, Task<TOut>> _callback;
        public Subscription4Task(ISubject<T1> subject1, ISubject<T2> subject2, ISubject<T3> subject3, ISubject<T4> subject4, Func<T1, T2, T3, T4, Task<TOut>> map)
        {
            _callback = map;
        
            _subject1 = subject1;
            _subject2 = subject2;
            _subject3 = subject3;
            _subject4 = subject4;
        }

        public ISubject<Task<TOut>> CreateMap()
        {
            Attach();

            _mapSubject = new ParameterSubject<Task<TOut>>();
            return _mapSubject;
        }

        public void Attach() {
            _unsubscriber1 = _subject1.Subscribe(new SmObserver<T1>(new SmObserverDispatcher<T1>(callback1)));
            _unsubscriber2 = _subject2.Subscribe(new SmObserver<T2>(new SmObserverDispatcher<T2>(callback2)));
            _unsubscriber3 = _subject3.Subscribe(new SmObserver<T3>(new SmObserverDispatcher<T3>(callback3)));
            _unsubscriber4 = _subject4.Subscribe(new SmObserver<T4>(new SmObserverDispatcher<T4>(callback4)));
        }

        public void Detach() {
            _unsubscriber1.Dispose();
            _unsubscriber2.Dispose();
            _unsubscriber3.Dispose();
            _unsubscriber4.Dispose();
        }

        private void callback1(T1 obj)
        {
            _mapSubject?.OnNextParameterValue(_callback(_subject1.SubjectValue, _subject2.SubjectValue, _subject3.SubjectValue, _subject4.SubjectValue));
        }
        private void callback2(T2 obj)
        {
            _mapSubject?.OnNextParameterValue(_callback(_subject1.SubjectValue, _subject2.SubjectValue, _subject3.SubjectValue, _subject4.SubjectValue));
        }

        private void callback3(T3 obj)
        {
            _mapSubject?.OnNextParameterValue(_callback(_subject1.SubjectValue, _subject2.SubjectValue, _subject3.SubjectValue, _subject4.SubjectValue));
        }

        private void callback4(T4 obj)
        {
            _mapSubject?.OnNextParameterValue(_callback(_subject1.SubjectValue, _subject2.SubjectValue, _subject3.SubjectValue, _subject4.SubjectValue));
        }
    }

}
