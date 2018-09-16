namespace Models
{
    public interface ITransliterateLanguage
    {
        string GuessGreekFromGreeklish(string text);

        string GenerateGreeklishFromGreek(string text);
    }
}
