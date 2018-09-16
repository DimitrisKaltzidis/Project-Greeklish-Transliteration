namespace Models.ExternalServices.GoogleTranslateModels
{
    public class DetectionResult
    {
        public double confidence { get; set; }

        public bool isReliable { get; set; }

        public string language { get; set; }
    }
}