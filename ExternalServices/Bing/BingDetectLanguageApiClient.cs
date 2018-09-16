namespace ExternalServices.Bing
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Models;

    public class BingDetectLanguageApiClient : IDetectLanguageClient
    {
        private readonly string key;

        public BingDetectLanguageApiClient(string key)
        {
            this.key = key;
        }

        public async Task<LanguageDetectionResult> GetLanguageAsync(string text)
        {
            var result = await this.DoTheCall(text);

            return new LanguageDetectionResult() { Language = result };
        }

        public LanguageDetectionResult GetLanguage(string text)
        {
            var result = this.DoTheCall(text).Result;

            return new LanguageDetectionResult() { Language = result };
        }

        private async Task<string> DoTheCall(string text)
        {
            if (this.key == null)
            {
                throw new Exception("LOL!!! The Microsoft Translator API client key is null. Please init the detector with one.");
            }

            var client = new HttpClient();

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", $"{this.key}");
            var uri = $" https://api.microsofttranslator.com/V2/Http.svc/Detect?text={text}";

            var response = await client.GetAsync(uri);

            var result = await response.Content.ReadAsStringAsync();

            result = result.StripHtml();

            return result;
        }
    }
}