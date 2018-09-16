namespace LinguisticTools.TextCat
{
    using System;
    using System.Linq;

    using LinguisticTools.TextCat.Classify;

    public class DistributionDistanceCalculator : IDistanceCalculator<IDistribution<string>>
    {
        public double CalculateDistance(IDistribution<string> languageModel1, IDistribution<string> languageModel2)
        {

            double result =
                1.0 - 
                languageModel1.DistinctRepresentedEvents.Intersect(languageModel2.DistinctRepresentedEvents)
                    .Select(ngram => Math.Min(languageModel1.GetEventFrequency(ngram), languageModel2.GetEventFrequency(ngram)))
                    .Sum();
            return result;
        }
    }
}
