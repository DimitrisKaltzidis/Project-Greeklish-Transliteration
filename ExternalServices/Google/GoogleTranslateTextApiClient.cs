namespace ExternalServices.Google
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Models;
    using Models.ExternalServices.GoogleTranslateModels;

    public class GoogleTranslateTextApiClient : ITranslateLanguageClient
    {
        private readonly string key;

        public GoogleTranslateTextApiClient(string key)
        {
            this.key = key;
        }

      public async Task<string> TranslateTextAsync(
            string text,
            string sourceLanguage,
            string destinationLanguage)
        {
            return await this.DoTheCall(text, sourceLanguage, destinationLanguage);
        }

        public string TranslateText(
            string text,
            string sourceLanguage,
            string destinationLanguage)
        {
            return this.DoTheCall(text, sourceLanguage, destinationLanguage).Result;
        }

        private async Task<string> DoTheCall(
            string text,
            string sourceLanguage,
            string destinationLanguage)
        {
            if (this.key == null)
            {
                throw new Exception("LOL!!! The Google API client key is null. Init the translator with one.");
            }

            if (string.IsNullOrEmpty(sourceLanguage) || string.IsNullOrEmpty(destinationLanguage))
            {
                throw new Exception("Please request valid source and destination languages in the ISO 639-1 format. See https://cloud.google.com/translate/docs/languages for more info.");
            }

            var client = new HttpClient();
            var uri =
                $"https://translation.googleapis.com/language/translate/v2?q={text}&target={destinationLanguage}&format=text&source={sourceLanguage}&model=nmt&key={key}";

            var response = await client.GetAsync(uri);

            try
            {
                var result = await response.Content.ReadAsAsync<TranslateResult>();

                return result.Data.Translations.Count > 0 ? result.Data.Translations[0].TranslatedText : null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
