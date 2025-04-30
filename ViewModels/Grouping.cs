using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace SmartHomeApp.ViewModels
{
    /// <summary>
    /// Simple grouping class for CollectionView.IsGrouped.
    /// </summary>
    public class Grouping<K, T> : ObservableCollection<T>
    {
        public K Key { get; }

        public Grouping(K key, IEnumerable<T> items) : base(items)
        {
            Key = key;
        }
    }
}
