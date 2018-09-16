namespace Models.ExternalServices.GoogleTranslateModels
{
    using Newtonsoft.Json;

    /// <summary>
    /// The translation.
    /// </summary>
    public class Translation
    {
        /// <summary>
        /// Gets or sets the translated text.
        /// </summary>
        [JsonProperty(PropertyName = "translatedText")]
        public string TranslatedText { get; set; }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        [JsonProperty(PropertyName = "model")]
        public string Model { get; set; }
    }
}
