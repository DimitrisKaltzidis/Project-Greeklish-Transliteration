namespace LinguisticTools.TextCat.Classify
{
    public interface IClassifier<T>
    {
        double Classify(T obj);
    }
}
