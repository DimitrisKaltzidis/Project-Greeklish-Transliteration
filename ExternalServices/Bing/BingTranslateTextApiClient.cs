namespace ExternalServices.Bing
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;

    using Models;

    public class BingTranslateTextApiClient : ITranslateLanguageClient
    {
        private readonly string key;

        public BingTranslateTextApiClient(string key)
        {
            this.key = key;
        }

        public async Task<string> TranslateTextAsync(string text, string sourceLanguage, string destinationLanguage)
        {
            return await this.DoTheCall(text, sourceLanguage, destinationLanguage);
        }

        public string TranslateText(string text, string sourceLanguage, string destinationLanguage)
        {
            return this.DoTheCall(text, sourceLanguage, destinationLanguage).Result;
        }

        private async Task<string> DoTheCall(string text, string sourceLanguage, string destinationLanguage)
        {
            if (this.key == null)
            {
                throw new Exception("LOL!!! The Bing API client key is null. Init the translator with one.");
            }

            if (string.IsNullOrEmpty(sourceLanguage) || string.IsNullOrEmpty(destinationLanguage))
            {
                throw new Exception("Please request valid languages in the ISO 639-1 format.");
            }

            var client = new HttpClient();

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", $"{this.key}");

            var encodedText = HttpUtility.UrlEncode(text);

            var uri =
                $"https://api.microsofttranslator.com/V2/Http.svc/Translate?text={encodedText}&from={sourceLanguage}&to={destinationLanguage}";

            var response = await client.GetAsync(uri);

            var result = await response.Content.ReadAsStringAsync();

            result = result.StripHtml();

            return result;
        }
    }
}