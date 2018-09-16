namespace Models
{
    using System.Threading.Tasks;

    public interface IDetectLanguage
    {
        LanguageDetectionResult GetLanguage(string text);

        Task<LanguageDetectionResult> GetLanguageAsync(string text);
    }
}