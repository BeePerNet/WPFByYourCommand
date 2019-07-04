using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace WPFByYourCommand
{
    public class NotifyCollection<T> : Collection<T>, INotifyCollectionChanged
    {
        public void OnCollectionChanged()
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
