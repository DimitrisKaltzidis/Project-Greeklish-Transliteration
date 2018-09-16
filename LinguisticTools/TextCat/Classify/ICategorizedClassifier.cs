namespace LinguisticTools.TextCat.Classify
{
    using System;
    using System.Collections.Generic;

    public interface ICategorizedClassifier<TItem, TCategory>
    {
        IEnumerable<Tuple<TCategory, double>> Classify(TItem item);
    }
}
