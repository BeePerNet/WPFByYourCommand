using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Threading;

namespace WPFByYourCommand.Observables
{
    [DebuggerDisplay("{GetType()}, Childrens={Count}")]
    public class DispatchObservableCollection<T> : ObservableCollection<T>
    {
        // Override the event so this class can access it
        public override event NotifyCollectionChangedEventHandler CollectionChanged;

        public DispatchObservableCollection(IEnumerable<T> collection) : base(collection) { }
        public DispatchObservableCollection(List<T> collection) : base(collection) { }
        public DispatchObservableCollection() : base() { }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            // Be nice - use BlockReentrancy like MSDN said
            using (BlockReentrancy())
            {
                var eventHandler = CollectionChanged;
                if (eventHandler != null)
                {
                    Delegate[] delegates = eventHandler.GetInvocationList();
                    // Walk thru invocation list
                    foreach (NotifyCollectionChangedEventHandler handler in delegates)
                    {
                        DispatcherObject dispatcherObject = handler.Target as DispatcherObject;
                        // If the subscriber is a DispatcherObject and different thread
                        if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                            // Invoke handler in the target dispatcher's thread
                            dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                        else // Execute handler as is
                            handler(this, e);
                    }
                }
            }
        }
    }
}
