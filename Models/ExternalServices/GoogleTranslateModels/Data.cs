namespace Models.ExternalServices.GoogleTranslateModels
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// The data.
    /// </summary>
    public class Data
    {
        /// <summary>
        /// Gets or sets the translations.
        /// </summary>
        [JsonProperty(PropertyName = "translations")]
        public List<Translation> Translations { get; set; }
    }
}
