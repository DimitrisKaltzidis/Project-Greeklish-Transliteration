namespace LinguisticTools.TextCat
{
    using System;
    using System.Collections.Generic;

    using LinguisticTools.TextCat.Classify;

    /// <summary>
    /// Identifies the language of a given text.
    /// Please use <see cref="RankedLanguageIdentifierFactory"/> to load an instance of this class from a file
    /// </summary>
    public class RankedLanguageIdentifier: ILanguageIdentifier
    {
        //private readonly List<LanguageModel<string>> _languageModels;
        private RankedClassifier<string> _classifier;

        public int MaxNGramLength { get; private set; }
        public int MaximumSizeOfDistribution { get; private set; }
        public int OccuranceNumberThreshold { get; set; }
        public int OnlyReadFirstNLines { get; set; }

        public RankedLanguageIdentifier(IEnumerable<LanguageModel<string>> languageModels, int maxNGramLength, int maximumSizeOfDistribution, int occuranceNumberThreshold, int onlyReadFirstNLines)
        {
            this.MaxNGramLength = maxNGramLength;
            this.MaximumSizeOfDistribution = maximumSizeOfDistribution;
            this.OccuranceNumberThreshold = occuranceNumberThreshold;
            this.OnlyReadFirstNLines = onlyReadFirstNLines;

            //_languageModels = languageModels.ToList();

            //_classifier =
            //    new KnnMonoCategorizedClassifier<IDistribution<string>, LanguageInfo>(
            //        (IDistanceCalculator<IDistribution<string>>) new RankingDistanceCalculator<IDistribution<string>>(MaximumSizeOfDistribution),
            //        _languageModels.ToDictionary(lm => lm.Features, lm => lm.Language));

            this._classifier = new RankedClassifier<string>(this.MaximumSizeOfDistribution);
            foreach (var languageModel in languageModels)
            {
                this._classifier.AddEtalonLanguageModel(languageModel);
            }
        }


        public IEnumerable<Tuple<LanguageInfo, double>> Identify(string text)
        {
            var extractor = new CharacterNGramExtractor(this.MaxNGramLength, this.OnlyReadFirstNLines);
            var tokens = extractor.GetFeatures(text);
            var model = LanguageModelCreator.CreateLangaugeModel(tokens, this.OccuranceNumberThreshold, this.MaximumSizeOfDistribution);
            var likelyLanguages = this._classifier.Classify(model);
            return likelyLanguages;
        }
    }
}
