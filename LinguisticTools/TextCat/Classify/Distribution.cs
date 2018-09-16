namespace LinguisticTools.TextCat.Classify
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// Implementation of <see cref="IDistribution{T}"/> is not strict as <see cref="Bag{T}"/> cannot contain more than int.MaxValue numbers.
    /// </remarks>
    public class Distribution<T> : IModifiableDistribution<T>
    {
        private IBag<T> _store;
        private bool _containsUnrepresentedNoiseEvents;
        private long _distinctEventsCountWithNoise;
        private long _totalEventCountWithNoise;

        public Distribution(IBag<T> store)
        {
            this._store = store;
        }

        #region Implementation of IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Implementation of IEnumerable<out KeyValuePair<T,double>>

        public IEnumerator<KeyValuePair<T, double>> GetEnumerator()
        {
            throw new NotImplementedException("Maybe a bug, should be divided by _totalEventCountWithNoise");
            long count = this._store.TotalCopiesCount;
            return this._store.Select(kvp => new KeyValuePair<T, double>(kvp.Key, kvp.Value / (double)count)).GetEnumerator();
        }

        #endregion

        #region Implementation of IEnumerable<out KeyValuePair<T,long>>

        IEnumerator<KeyValuePair<T, long>> IEnumerable<KeyValuePair<T, long>>.GetEnumerator()
        {
            return this._store.GetEnumerator();
        }

        #endregion

        #region Implementation of IDistribution<T>

        public double this[T obj]
        {
            get { return this.GetEventFrequency(obj); }
        }

        public IEnumerable<T> DistinctRepresentedEvents
        {
            get { return this._store.DistinctItems; }
        }

        public long DistinctRepresentedEventsCount
        {
            get { return this._store.DistinctItemsCount; } 
        }

        public long DistinctEventsCountWithNoise
        {
            get { return this._distinctEventsCountWithNoise; }
        }

        public long DistinctNoiseEventsCount
        {
            get { return this._distinctEventsCountWithNoise - this._store.DistinctItemsCount; }
        }

        public double GetEventFrequency(T obj)
        {
            return this._store.GetNumberOfCopies(obj) / (double)this._store.TotalCopiesCount;
        }

        public long GetEventCount(T obj)
        {
            return this._store.GetNumberOfCopies(obj);
        }

        public long TotalRepresentedEventCount
        {
            get { return this._store.TotalCopiesCount; }
        }

        public long TotalEventCountWithNoise
        {
            get { return this._totalEventCountWithNoise; }
        }

        public long TotalNoiseEventsCount
        {
            get { return this.TotalEventCountWithNoise - this.TotalRepresentedEventCount; }
        }

        #endregion

        #region Implementation of IModifiableDistribution<T>

        public void AddEvent(T obj)
        {
            this.AddEvent(obj, 1);
        }

        public void AddEvent(T obj, long count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("Cannot add negative number of items");
            this._store.Add(obj, count);
            // impossible because the internal bag should contain all the events (including those that will be considered as noise after pruning)
            // otherwise we just cannot reliably keep track _distinctEventsCountWithNoise 
            // (because we cannot distinguish between if the feature has been seen as noise or hasn't been seen at all, 
            // hence do not know if we should add +1 to _distinctEventsCountWithNoise).
            if (this._containsUnrepresentedNoiseEvents)
                throw new InvalidOperationException("Cannot add new items to the distribution after it has been pruned.");
            this._distinctEventsCountWithNoise = this._store.DistinctItemsCount;
            this._totalEventCountWithNoise += count;
        }

        public void AddNoise(long totalCount, long distinctCount)
        {
            if (totalCount < 0)
                throw new ArgumentOutOfRangeException("Cannot add negative number of items");
            this._containsUnrepresentedNoiseEvents = true;
            this._distinctEventsCountWithNoise += distinctCount;
            this._totalEventCountWithNoise += totalCount;
        }

        public void AddEventRange(IEnumerable<T> collection)
        {
            foreach (var item in collection)
                this.AddEvent(item);
        }

        public void PruneByRank(long maxRankAllowed)
        {
            IEnumerable<T> removedItems = 
                this._store
                .OrderBy(kvp => kvp.Value)
                .Select(kvp => kvp.Key)
                .Take((int) Math.Max(0, this.DistinctRepresentedEvents.LongCount() - maxRankAllowed));
            foreach (var item in removedItems)
            {
                this._store.RemoveAllCopies(item);
            }
            this._containsUnrepresentedNoiseEvents = true;
        }

        public void PruneByCount(long minCountAllowed)
        {
            if (minCountAllowed < 0)
                throw new ArgumentOutOfRangeException("minCountAllowed", "Only non-negative values allowed");
            IEnumerable<T> removedItems =
                this._store
                .Where(kvp => kvp.Value < minCountAllowed)
                .Select(kvp => kvp.Key)
                .ToList();
            foreach (var item in removedItems)
            {
                this._store.RemoveAllCopies(item);
            }
            this._containsUnrepresentedNoiseEvents = true;
        }

        #endregion
    }
}
