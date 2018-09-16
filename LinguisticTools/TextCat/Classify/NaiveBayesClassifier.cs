namespace LinguisticTools.TextCat.Classify
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class NaiveBayesClassifier<TItem, TFeature, TCategory> : ICategorizedClassifier<TItem, TCategory>
        where TItem: IEnumerable<TFeature>
    {
        private Dictionary<TCategory, DistributionAndMinEventCount<TFeature>> _categoryAndFeatures;
        private double? _kSmoothFactor;
        private long _globalTotalEventCount;
        private long _globalDistinctEventCount;

        public NaiveBayesClassifier(IDictionary<TCategory, IDistribution<TFeature>> knownInstances, double? kSmoothFactor)
        {
            this._kSmoothFactor = kSmoothFactor;
            if (knownInstances == null) throw new ArgumentNullException("knownInstances");
            //throw new NotImplementedException("bug: SOUNDS LIKE A TOTAL REPEATING EVENT COUNT!!!");
            this._globalTotalEventCount = knownInstances.Sum(d => d.Value.TotalEventCountWithNoise);
            //not exactly true because some features might be in multiple categories and should count only once
            this._globalDistinctEventCount = knownInstances.Sum(d => d.Value.DistinctEventsCountWithNoise); 
            this._categoryAndFeatures =
                knownInstances.ToDictionary(
                _ => _.Key,
                distr =>
                new DistributionAndMinEventCount<TFeature>
                    {
                        Distribution = distr.Value,
                        // APPROXIMATION! Noise feature occurs on average twice less than the least frequent represented feature.
                        // It might increase significance of noise in terms of frequencies (because noise distributes according to Zipf's law rather than linear distribution.
                        // Therefore denominator of > 2.0 might be better
                        AverageNoiseFeatureFrequencyLog = Math.Log(distr.Value.DistinctRepresentedEvents.Select(distr.Value.GetEventCount).DefaultIfEmpty(0).Min() / 2.0 / distr.Value.TotalNoiseEventsCount),
                        
                        TotalEventCountWithNoiseLog = Math.Log(distr.Value.TotalEventCountWithNoise),
                        TotalEventCountWithKSmoothingLog = 
                            this._kSmoothFactor.HasValue
                            ? Math.Log(distr.Value.TotalEventCountWithNoise + this._globalDistinctEventCount * this._kSmoothFactor.Value) 
                            : (double?)null,
                        CategoryFrequencyLog = Math.Log(distr.Value.TotalEventCountWithNoise) - Math.Log(this._globalTotalEventCount),
                    });
        }

        public IEnumerable<Tuple<TCategory, double>> Classify2(TItem item)
        {
            var features = item.ToList();
            var result = new List<Tuple<TCategory, double>>();
            foreach (var categoryAndFeature in this._categoryAndFeatures)
            {
                var category = categoryAndFeature.Key;
                var categoryProperties = categoryAndFeature.Value;
                double featureFreqSum = categoryProperties.CategoryFrequencyLog;
                foreach (var feature in features)
                {
                    var eventCount = categoryProperties.Distribution.GetEventCount(feature);
                    var featureFreqLog =
                        this._kSmoothFactor.HasValue
                            ? Math.Log(eventCount + this._kSmoothFactor.Value) - categoryProperties.TotalEventCountWithKSmoothingLog.Value
                            : eventCount > 0
                                  ? Math.Log(eventCount) - categoryProperties.TotalEventCountWithNoiseLog
                                  : categoryProperties.AverageNoiseFeatureFrequencyLog;
                    featureFreqSum += featureFreqLog;
                }
                result.Add(Tuple.Create(category, featureFreqSum));
            }
            return result.OrderByDescending(t => t.Item2);
        }

        public IEnumerable<Tuple<TCategory, double>> Classify(TItem item)
        {
            var features = item.ToList();
            var result = new List<Tuple<TCategory, double>>();
            foreach (var categoryAndFeature in this._categoryAndFeatures)
            {
                var category = categoryAndFeature.Key;
                var categoryProperties = categoryAndFeature.Value;
                double featureFreqSum = categoryProperties.CategoryFrequencyLog;
                foreach (var feature in features)
                {
                    var eventCount = categoryProperties.Distribution.GetEventCount(feature);
                    var featureFreqLog =
                        this._kSmoothFactor.HasValue
                            ? Math.Log(eventCount + this._kSmoothFactor.Value) - categoryProperties.TotalEventCountWithKSmoothingLog.Value
                            : eventCount > 0
                                  ? Math.Log(eventCount) - categoryProperties.TotalEventCountWithNoiseLog
                                  : categoryProperties.AverageNoiseFeatureFrequencyLog;
                    featureFreqSum += featureFreqLog;
                }
                result.Add(Tuple.Create(category, featureFreqSum));
            }
            return result.OrderByDescending(t => t.Item2);
        }

        private class DistributionAndMinEventCount<TFeature>
        {
            public IDistribution<TFeature> Distribution { get; set; }
            public double AverageNoiseFeatureFrequencyLog { get; set; }
            public double? TotalEventCountWithKSmoothingLog { get; set; }
            public double TotalEventCountWithNoiseLog { get; set; }
            public double CategoryFrequencyLog { get; set; }
        }
    }
}
