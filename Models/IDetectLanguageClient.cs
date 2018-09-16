namespace Models
{
    using System.Threading.Tasks;

    public interface IDetectLanguageClient
    {
        LanguageDetectionResult GetLanguage(string text);

        Task<LanguageDetectionResult> GetLanguageAsync(string text);
    }
}