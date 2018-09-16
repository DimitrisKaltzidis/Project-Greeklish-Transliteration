namespace GreeklishHelper
{
    using System;
    using System.Threading.Tasks;

    using ExternalServices.Bing;
    using ExternalServices.Google;

    using LinguisticTools;

    using Models;

    public class TranslateLanguage : ITranslateLanguage
    {
        private ITranslateLanguageClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslateLanguage"/> class. 
        /// Currently the library supports Google only
        /// Please provide the keys for their APIs.
        /// </summary>
        /// <param name="provider">
        /// The provider responsible for performing the language translation.
        /// Google(supports all languages but no Greeklish),
        /// </param>
        /// <param name="key">
        /// The Google API key.
        /// </param>
        public TranslateLanguage(
            Enums.TranslateLanguageProviders provider = Enums.TranslateLanguageProviders.Google,
            string key = null)
        {
            switch (provider)
            {
                case Enums.TranslateLanguageProviders.Google:
                    this.client = new GoogleTranslateTextApiClient(key);
                    break;
                /*case Enums.TranslateLanguageProviders.Bing:
                    this.client = new BingTranslateTextApiClient(key);
                    break;*/
            }
        }

        public async Task<string> TranslateTextAsync(string text, string sourceLanguage, string destinationLanguage)
        {
            if (sourceLanguage == "gren" || destinationLanguage == "gren")
            {
                throw new Exception(
                    "Greeklish are not supported in translation. "
                    + "You need to transliterate then to Greek first and then translate them. "
                    + "Use TransliterateLanguage class to do this and then translate Greek." 
                    + "Act in the same manner if you want to perform the opposite procedure.");
            }

            return await this.client.TranslateTextAsync(text, sourceLanguage, destinationLanguage);
        }

        public string TranslateText(string text, string sourceLanguage, string destinationLanguage)
        {
            if (sourceLanguage == "gren" || destinationLanguage == "gren")
            {
                throw new Exception(
                    "Greeklish are not supported in translation. "
                    + "You need to transliterate then to Greek first and then translate them. "
                    + "Use TransliterateLanguage class to do this and then translate Greek." 
                    + "Act in the same manner if you want to perform the opposite procedure.");
            }

            return this.client.TranslateText(text, sourceLanguage, destinationLanguage);
        }
    }
}