namespace LinguisticTools.TextCat
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LinguisticTools.TextCat.Classify;

    public class NaiveBayesLanguageIdentifier: ILanguageIdentifier
    {
        public int MaxNGramLength { get; private set; }
        public int OnlyReadFirstNLines { get; set; }
        private NaiveBayesClassifier<IEnumerable<string>, string, LanguageInfo> _classifier;

        public NaiveBayesLanguageIdentifier(IEnumerable<LanguageModel<string>> languageModels,  int maxNGramLength, int onlyReadFirstNLines)
        {
            this.MaxNGramLength = maxNGramLength;
            this.OnlyReadFirstNLines = onlyReadFirstNLines;
            this._classifier = new NaiveBayesClassifier<IEnumerable<string>, string, LanguageInfo>(
                languageModels.ToDictionary(lm => lm.Language, lm => lm.Features), 1);
        }

        public IEnumerable<Tuple<LanguageInfo, double>> Identify(string text)
        {
            var extractor = new CharacterNGramExtractor(this.MaxNGramLength, this.OnlyReadFirstNLines);
            var tokens = extractor.GetFeatures(text);
            var likelyLanguages = this._classifier.Classify(tokens);
            return likelyLanguages;
        }
    }
}
