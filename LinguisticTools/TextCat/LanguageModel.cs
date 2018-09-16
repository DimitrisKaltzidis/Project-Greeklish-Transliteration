namespace LinguisticTools.TextCat
{
    using System.Collections.Generic;

    using LinguisticTools.TextCat.Classify;

    public class LanguageModel<T>
    {
        public LanguageInfo Language { get; private set; }

        public IDictionary<string, string> Metadata { get; private set; }
        
        public IDistribution<T> Features { get; private set; }

        public LanguageModel(IDistribution<T> features, LanguageInfo language)
        {
            this.Language = language;
            this.Features = features;
            this.Metadata = new Dictionary<string, string>();
        }

        public LanguageModel(IDistribution<T> features, LanguageInfo language, IDictionary<string, string> metadata)
        {
            this.Language = language;
            this.Metadata = metadata;
            this.Features = features;
        }
    }
}
