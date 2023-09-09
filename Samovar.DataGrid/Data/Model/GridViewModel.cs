//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Samovar.DataGrid
//{
//    internal class GridViewModel<T>
//        : IObserver<IEnumerable<T>>
//    {
//        private IDisposable unsubscriber;

//        internal IEnumerable<GridRowModel<T>> ViewCollection { get; set; }

//        public void OnCompleted()
//        {
//            throw new NotImplementedException();
//        }

//        public void OnError(Exception error)
//        {
//            throw new NotImplementedException();
//        }

//        public void OnNext(IEnumerable<T> value)
//        {
//            ViewCollection = null;
//        }

//        public virtual void Subscribe(IObservable<IEnumerable<T>> provider)
//        {
//            if (provider != null)
//                unsubscriber = provider.Subscribe(this);
//        }

//        public virtual void Unsubscribe()
//        {
//            unsubscriber.Dispose();
//        }
//    }
//}
