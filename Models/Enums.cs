namespace Models
{
    public class Enums
    {
        public enum Language
        {
            /// <summary>
            /// The unknown.
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// The greek.
            /// </summary>
            El = 1,

            /// <summary>
            /// The english.
            /// </summary>
            En,

            /// <summary>
            /// The greeklish.
            /// </summary>
            Gren
        }

        public enum TranslateLanguageProviders
        {
            /// <summary>
            /// Google supports every language but does not support Greeklish.
            /// </summary>
            Google = 1,

            /// <summary>
            /// Bing supports every language but does not support Greeklish.
            /// </summary>
          //  Bing
        }

        public enum DetectLanguageProvider
        {
            /// <summary>
            /// Local is the library itself that supports only Greek, English, Greeklish.
            /// </summary>
            Local = 0,

            /// <summary>
            /// Google supports every language but does not support Greeklish.
            /// </summary>
            Google = 1,

            /// <summary>
            /// Bing supports every language but does not support Greeklish.
            /// </summary>
           // Bing
        }
    }
}