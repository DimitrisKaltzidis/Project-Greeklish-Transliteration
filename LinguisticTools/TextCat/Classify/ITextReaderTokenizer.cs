namespace LinguisticTools.TextCat.Classify
{
    using System.IO;

    public interface ITextReaderTokenizer : IFeatureExtractor<TextReader, string>
    {
    }
}
