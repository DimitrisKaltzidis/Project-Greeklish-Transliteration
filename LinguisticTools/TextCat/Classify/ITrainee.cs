namespace LinguisticTools.TextCat.Classify
{
    public interface ITrainee<T>
    {
        void LearnMatch(T obj);
        void LearnMismatch(T obj);
    }
}
