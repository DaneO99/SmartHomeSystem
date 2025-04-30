// Namespace imports for observable collections and enumerable support
using System.Collections.ObjectModel;     // Provides ObservableCollection<T>
using System.Collections.Generic;         // Provides IEnumerable<T>

namespace SmartHomeApp.ViewModels
{
    /// <summary>
    /// Provides a grouping container for use with CollectionView.IsGrouped,
    /// allowing elements to be organized under a common key.
    /// </summary>
    /// <typeparam name="K">Type of the grouping key.</typeparam>
    /// <typeparam name="T">Type of elements in the group.</typeparam>
    public class Grouping<K, T> : ObservableCollection<T>
    {
        /// <summary>
        /// Gets the key identifying this group.
        /// </summary>
        public K Key { get; }

        /// <summary>
        /// Initializes a new instance with a specified key and collection of items.
        /// </summary>
        /// <param name="key">Value used to label the group (e.g., room name).</param>
        /// <param name="items">Sequence of items belonging to the group.</param>
        public Grouping(K key, IEnumerable<T> items)
            : base(items)
        {
            Key = key;  // Store grouping key
        }
    }
}
