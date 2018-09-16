namespace LinguisticTools.TextCat.Classify
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class GaussianBag : IBag<ulong>
    {
        private const int ShortcutsRange = 256*256;
        private Dictionary<ulong, DictionaryCount> _store = new Dictionary<ulong, DictionaryCount>();
        private long[] _speedCounts = new long[ShortcutsRange];
        private long _totalCopiesCount;

        public long GetNumberOfCopies(ulong item)
        {
            if (item < ShortcutsRange)
            {
                return this._speedCounts[item];
            }

            DictionaryCount count;
            if (this._store.TryGetValue(item, out count))
                return count._count;
            return 0;
        }

        public IEnumerable<ulong> DistinctItems
        {
            get
            {
                // todo: check if this yielding doesn't introduce a bottleneck
                var length = (ulong) this._speedCounts.Length;
                for (ulong i = 0; i < length; i++)
                {
                    var count = this._speedCounts[i];
                    if (count > 0)
                        yield return i;

                }
                foreach (var key in this._store.Keys)
                {
                    yield return key;
                }
            }
        }

        public long DistinctItemsCount
        {
            get { return this._store.Keys.Count; }
        }

        public bool Add(ulong item, long count)
        {
            if (item < ShortcutsRange)
            {
                this._speedCounts[item] += count;
                this._totalCopiesCount += count;
                return true;
            }

            DictionaryCount oldCount;
            long newCount = (this._store.TryGetValue(item, out oldCount) ? oldCount._count : 0) + count;
            if (newCount < 0)
                return false;
            //_store[item] = newCount;
            if (oldCount == null)
                this._store.Add(item, new DictionaryCount(newCount));
            else 
                oldCount._count = newCount;
            this._totalCopiesCount += count;
            return true;
        }

        public bool RemoveCopies(ulong item, long count)
        {
            return this.Add(item, -count);
        }

        public void RemoveAllCopies(ulong item)
        {
            if (item < ShortcutsRange)
            {
                this._totalCopiesCount -= this._speedCounts[item]; 
                this._speedCounts[item] = 0;
                return;
            }

            DictionaryCount oldCount;
            if (this._store.TryGetValue(item, out oldCount))
            {
                this._totalCopiesCount -= oldCount._count;
                this._store.Remove(item);
            }
        }

        public long TotalCopiesCount
        {
            get { return this._totalCopiesCount; }
        }

        public IEnumerator<KeyValuePair<ulong, long>> GetEnumerator()
        {
            List<KeyValuePair<ulong, long>> list = new List<KeyValuePair<ulong, long>>();
            for (uint i = 0; i < this._speedCounts.Length; i++)
            {
                long speedCount = this._speedCounts[i];
                if (speedCount > 0)
                    list.Add(new KeyValuePair<ulong, long>(i, speedCount));
            }
            return list.Concat(this._store.Select(kvp => new KeyValuePair<ulong, long>(kvp.Key, kvp.Value._count))).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private class DictionaryCount
        {
            public DictionaryCount(long count)
            {
                this._count = count;
            }

            public long _count;
        }
    }
}
