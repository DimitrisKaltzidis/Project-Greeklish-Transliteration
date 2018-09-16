namespace LinguisticTools.TextCat.Classify
{
    using System.Collections;
    using System.Collections.Generic;

    public class Bag<T> : IBag<T>
    {
        private Dictionary<T, long> _store = new Dictionary<T, long>();
        private long _totalCopiesCount;

        public long GetNumberOfCopies(T item)
        {
            long count;
            if (this._store.TryGetValue(item, out count))
                return count;
            return 0;
        }

        public IEnumerable<T> DistinctItems
        {
            get { return this._store.Keys; }
        }

        public long DistinctItemsCount
        {
            get { return this._store.Keys.Count; }
        }

        public bool Add(T item, long copiesCount)
        {
            long oldCount;
            long newCount = (this._store.TryGetValue(item, out oldCount) ? oldCount : 0) + copiesCount;
            if (newCount < 0)
                return false;
            this._store[item] = newCount;
            this._totalCopiesCount += copiesCount;
            return true;
        }

        public bool RemoveCopies(T item, long count)
        {
            return this.Add(item, -count);
        }

        public void RemoveAllCopies(T item)
        {
            long oldCount;
            if (this._store.TryGetValue(item, out oldCount))
            {
                this._totalCopiesCount -= oldCount;
                this._store.Remove(item);
            }
        }

        public long TotalCopiesCount
        {
            get { return this._totalCopiesCount; }
        }

        public IEnumerator<KeyValuePair<T, long>> GetEnumerator()
        {
            return this._store.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
