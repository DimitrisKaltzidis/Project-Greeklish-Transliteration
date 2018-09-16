namespace LinguisticTools.TextCat.Classify
{
    using System.Collections.Generic;

    public interface IFeatureExtractor<TSource, TFeature>
    {
        IEnumerable<TFeature> GetFeatures(TSource obj, bool toLower = false);
    }
}
