namespace Models.ExternalServices.GoogleTranslateModels
{
    using Newtonsoft.Json;

    /// <summary>
    /// The translate result.
    /// </summary>
    public class TranslateResult
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public Data Data { get; set; }
    }
}