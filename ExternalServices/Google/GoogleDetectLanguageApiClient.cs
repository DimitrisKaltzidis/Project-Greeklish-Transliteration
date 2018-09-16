namespace ExternalServices.Google
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using Models;
    using Models.ExternalServices.GoogleDetectLanguageModels;

    using Newtonsoft.Json;

    public class GoogleDetectLanguageApiClient : IDetectLanguageClient
    {
        private readonly string key;

        public GoogleDetectLanguageApiClient(string key)
        {
            this.key = key;
        }

        public async Task<LanguageDetectionResult> GetLanguageAsync(string text)
        {
            var result = await this.DoTheCall(text);

            return result;
        }

        public LanguageDetectionResult GetLanguage(string text)
        {
            var result = this.DoTheCall(text).Result;

            return result;
        }

        public async Task<LanguageDetectionResult> DoTheCall(string text)
        {
            if (this.key == null)
            {
                throw new Exception("LOL!!! The Google API client key is null. Init the detector with one.");
            }

            var client = new HttpClient();

            var request = new DetectLanguageRequest() { q = text };

            var uri = $"https://translation.googleapis.com/language/translate/v2/detect?key={this.key}";

            var jsonForm = JsonConvert.SerializeObject(request);

            using (var content = new StringContent(jsonForm))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await client.PostAsync(uri, content);

                try
                {
                    var result = await response.Content.ReadAsAsync<TopTranslationResult>();

                    if (result.data.detections.Count > 0)
                    {
                        // TODO CHECK IF NULL FIRST - ANYBODY GOT TIME FOR DAT!!! YOLO
                        return new LanguageDetectionResult()
                        {
                            Language = result.data.detections[0][0].language,
                            Confidence = result.data.detections[0][0].confidence
                        };
                    }
                }
                catch (Exception)
                {
                    return new LanguageDetectionResult() { Language = "unknown", Confidence = -1 };
                }
            }

            return new LanguageDetectionResult() { Language = "unknown", Confidence = -1 };
        }
    }
}