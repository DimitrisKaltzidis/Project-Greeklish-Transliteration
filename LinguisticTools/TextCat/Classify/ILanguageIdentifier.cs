namespace LinguisticTools.TextCat.Classify
{
    using System;
    using System.Collections.Generic;

    public interface ILanguageIdentifier
    {
        IEnumerable<Tuple<LanguageInfo, double>> Identify(string text);
    }
}
