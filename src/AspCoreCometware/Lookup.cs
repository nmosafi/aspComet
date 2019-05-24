using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AspComet
{
    internal class Lookup<TKey, TValue> : ILookup<TKey, TValue>
    {
        private readonly Dictionary<TKey, List<TValue>> dictionary = new Dictionary<TKey, List<TValue>>();

        public void Add(TKey key, TValue value)
        {
            // TODO: lock?
            if (this.dictionary.ContainsKey(key))
            {
                this.dictionary[key].Add(value);
            }
            else
            {
                this.dictionary[key] = new List<TValue> { value };
            }
        }

        public IEnumerator<IGrouping<TKey, TValue>> GetEnumerator()
        {
            List<KeyValuePair<TKey, TValue>> flattenedList = new List<KeyValuePair<TKey, TValue>>();
            foreach (var pair in this.dictionary)
            {
                foreach (var value in pair.Value)
                {
                    flattenedList.Add(new KeyValuePair<TKey, TValue>(pair.Key, value));
                }
            }

            return flattenedList.GroupBy(item => item.Key, pair => pair.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public bool Contains(TKey key)
        {
            return this.dictionary.ContainsKey(key);
        }

        public int Count
        {
            get { return this.dictionary.Values.Count; }
        }

        public IEnumerable<TValue> this[TKey key]
        {
            get { return this.dictionary[key]; }
        }

        public Dictionary<TKey, List<TValue>>.KeyCollection Keys
        {
            get { return this.dictionary.Keys; }
        }
    }
}