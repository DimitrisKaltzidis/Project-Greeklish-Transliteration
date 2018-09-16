namespace Models
{
    using System.Threading.Tasks;

    public interface ITranslateLanguageClient
    {
        Task<string> TranslateTextAsync(
            string text,
            string sourceLanguage,
            string destinationLanguage);

        string TranslateText(
            string text,
            string sourceLanguage,
            string destinationLanguage);
    }
}
