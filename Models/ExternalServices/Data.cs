namespace Models.ExternalServices.GoogleDetectLanguageModels
{
    using System.Collections.Generic;

    using Models.ExternalServices.GoogleTranslateModels;

    public class Data
    {
        public List<List<DetectionResult>> detections { get; set; }
    }
}