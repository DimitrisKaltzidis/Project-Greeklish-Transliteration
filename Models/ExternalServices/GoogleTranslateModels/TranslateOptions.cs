namespace Models.ExternalServices.GoogleTranslateModels
{
    /// <summary>
    /// The translate options.
    /// </summary>
    public class TranslateOptions
    {
        /// <summary>
        /// The translation model. Can be either base to use the Phrase-Based Machine Translation (PBMT) model, or nmt to use the Neural Machine Translation (NMT) model. If omitted, then nmt is used.
        /// If the model is nmt, and the requested language translation pair is not supported for the NMT model, then the request is translated using the base model.
        /// </summary>
        public enum TranslateEngine
        {
            /// <summary>
            /// The neural network translation 
            /// </summary>
            Nmt = 1,

            /// <summary>
            /// Normal Dictionary translation
            /// </summary>
            Base,
        }

        /// <summary>
        /// The format of the source text, in either HTML (default) or plain-text. A value of html indicates HTML and a value of text indicates plain-text.
        /// </summary>
        public enum TranslateFormat
        {
            /// <summary>
            /// For web page translation
            /// </summary>
            Html = 1,

            /// <summary>
            /// For plain text translation
            /// </summary>
            Text
        }
    }
}