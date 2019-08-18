using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;

namespace WPFByYourCommand.Observables
{
    /// <summary>
    /// FROM https://www.codeproject.com/Articles/64936/Threadsafe-ObservableImmutable-Collection
    /// By AnthonyPaulO
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1030:Use events where appropriate", Justification = "<En attente>")]
    public abstract class ObservableCollectionObject : INotifyCollectionChanged, INotifyPropertyChanged
    {
        #region Private

        private bool _lockObjWasTaken;
        private readonly object _lockObj;
        private int _lock; // 0=unlocked		1=locked

        #endregion Private

        #region Public Properties

        private readonly ObservableLockType _lockType;
        public ObservableLockType LockType => _lockType;

        #endregion Public Properties

        #region Constructor

        protected ObservableCollectionObject(ObservableLockType lockType)
        {
            _lockType = lockType;
            _lockObj = new object();
        }

        #endregion Constructor

        #region SpinWait/PumpWait Methods

        // note : find time to put all these methods into a helper class instead of in a base class

        // returns a valid dispatcher if this is a UI thread (can be more than one UI thread so different dispatchers are possible); null if not a UI thread
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static Dispatcher GetDispatcher()
        {
            return Dispatcher.FromThread(Thread.CurrentThread);
        }

        protected void WaitForCondition(Func<bool> condition)
        {
            Dispatcher dispatcher = GetDispatcher();

            if (dispatcher == null)
            {
                switch (LockType)
                {
                    case ObservableLockType.SpinWait:
                        SpinWait.SpinUntil(condition); // spin baby... 
                        break;
                    case ObservableLockType.Lock:
                        bool isLockTaken = false;
                        Monitor.Enter(_lockObj, ref isLockTaken);
                        _lockObjWasTaken = isLockTaken;
                        break;
                }
                return;
            }

            _lockObjWasTaken = true;
            PumpWaitPumpUntil(dispatcher, condition);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<En attente>")]
        protected static void PumpWaitPumpUntil(Dispatcher dispatcher, Func<bool> condition)
        {
            DispatcherFrame frame = new DispatcherFrame();
            BeginInvokePump(dispatcher, frame, condition);
            Dispatcher.PushFrame(frame);
        }

        private static void BeginInvokePump(Dispatcher dispatcher, DispatcherFrame frame, Func<bool> condition)
        {
            dispatcher.BeginInvoke
                (
                DispatcherPriority.DataBind,
                (Action)
                    (
                    () =>
                    {
                        frame.Continue = !condition();

                        if (frame.Continue)
                        {
                            BeginInvokePump(dispatcher, frame, condition);
                        }
                    }
                    )
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DoEvents()
        {
            Dispatcher dispatcher = GetDispatcher();
            if (dispatcher == null)
            {
                return;
            }

            DispatcherFrame frame = new DispatcherFrame();
            dispatcher.BeginInvoke(DispatcherPriority.DataBind, new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object ExitFrame(object frame)
        {
            ((DispatcherFrame)frame).Continue = false;
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool TryLock()
        {
            switch (LockType)
            {
                case ObservableLockType.SpinWait:
                    return Interlocked.CompareExchange(ref _lock, 1, 0) == 0;
                case ObservableLockType.Lock:
                    return Monitor.TryEnter(_lockObj);
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Lock()
        {
            switch (LockType)
            {
                case ObservableLockType.SpinWait:
                    WaitForCondition(() => Interlocked.CompareExchange(ref _lock, 1, 0) == 0);
                    break;
                case ObservableLockType.Lock:
                    WaitForCondition(() => Monitor.TryEnter(_lockObj));
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Unlock()
        {
            switch (LockType)
            {
                case ObservableLockType.SpinWait:
                    _lock = 0;
                    break;
                case ObservableLockType.Lock:
                    if (_lockObjWasTaken)
                    {
                        Monitor.Exit(_lockObj);
                    }

                    _lockObjWasTaken = false;
                    break;
            }
        }

        #endregion SpinWait/PumpWait Methods

        #region INotifyCollectionChanged

        public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            NotifyCollectionChangedEventHandler notifyCollectionChangedEventHandler = CollectionChanged;

            if (notifyCollectionChangedEventHandler == null)
            {
                return;
            }

            foreach (NotifyCollectionChangedEventHandler handler in notifyCollectionChangedEventHandler.GetInvocationList())
            {
                if (handler.Target is DispatcherObject dispatcherObject && !dispatcherObject.CheckAccess())
                {
                    dispatcherObject.Dispatcher.Invoke(DispatcherPriority.DataBind, handler, this, args);
                }
                else
                {
                    handler(this, args);
                }
            }
        }

        protected virtual void RaiseNotifyCollectionChanged()
        {
            RaiseNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected virtual void RaiseNotifyCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            RaisePropertyChanged("Count");
            RaisePropertyChanged("Item[]");
            OnCollectionChanged(args);
        }

        #endregion INotifyCollectionChanged

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged

        #region Nested Types

        public enum ObservableLockType
        {
            SpinWait,
            Lock
        }

        #endregion Nested Types
    }
}
