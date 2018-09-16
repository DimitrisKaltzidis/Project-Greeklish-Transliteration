namespace GreeklishHelper
{
    using System.Globalization;
    using System.Threading.Tasks;

    using ExternalServices.Bing;
    using ExternalServices.Google;

    using LinguisticTools;

    using Models;
    using Models.ExternalServices;

    public class DetectLanguage : IDetectLanguage
    {
        private IDetectLanguageClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="DetectLanguage"/> class. 
        /// If you intent to use third party detectors for languages other than Greek, English and Greeklish
        /// Then please provide the keys for their APIs. Currently the library supports
        /// Google only.
        /// </summary>
        /// <param name="provider">
        /// The provider responsible for performing the language detection.
        /// Local(supports only Greek{el}, English{en} an Greeklish{gren}) no need for an API key,
        /// Google(supports all languages but no Greeklish),
        /// </param>
        /// <param name="key">
        /// The Google API key.
        /// </param>
        public DetectLanguage(Enums.DetectLanguageProvider provider = Enums.DetectLanguageProvider.Local, string key = null)
        {
            switch (provider)
            {
                    case Enums.DetectLanguageProvider.Local:
                        this.client = new LanguageDetector("el", "en", "gren");
                        break;
                    case Enums.DetectLanguageProvider.Google:
                        this.client = new GoogleDetectLanguageApiClient(key);
                        break;
                    /*case Enums.DetectLanguageProvider.Bing:
                        this.client = new BingDetectLanguageApiClient(key);
                        break;*/
            }
        }

        /// <summary>
        /// Detects the language of a given string. If you choce Local as the provider it supports only greek, english and Greeklish.
        /// Otherwise it suports every language but not Greeklish.
        /// </summary>
        /// <param name="text">
        /// Text for identification
        /// </param>
        /// <returns>
        /// Results contains the language and the confidence of the detection
        /// </returns>
        public LanguageDetectionResult GetLanguage(string text)
        {
            return this.client.GetLanguage(text);
        }

        /// <summary>
        /// Detects the language of a given string. If you choce Local as the provider it supports only greek, english and Greeklish.
        /// Otherwise it suports every language but not Greeklish.
        /// </summary>
        /// <param name="text">
        /// Text for identification
        /// </param>
        /// <returns>
        /// Results contains the language and the confidence of the detection
        /// </returns>
        public async Task<LanguageDetectionResult> GetLanguageAsync(string text)
        {
            return await this.client.GetLanguageAsync(text);
        }
    }
}