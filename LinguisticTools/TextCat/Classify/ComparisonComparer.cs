namespace LinguisticTools.TextCat.Classify
{
    using System;
    using System.Collections.Generic;

    public class ComparisonComparer<T> : IComparer<T>
    {
        private readonly Comparison<T> _comparison;

        public ComparisonComparer(Comparison<T> comparison)
        {
            this._comparison = comparison;
        }

        public int Compare(T x, T y)
        {
            return this._comparison(x, y);
        }
    }
}
