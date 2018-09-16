namespace GreeklishHelperUnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Models;

    [TestClass]
    public class DetectLanguageTests
    {
        [TestMethod]
        public void DetectLanguageLocal()
        {
            var text = "kalimera";
            var detector = new GreeklishHelper.DetectLanguage();
            var expectedLanguage = "gren";

            var detectedLanguage = detector.GetLanguage(text);

            Assert.AreEqual(detectedLanguage.Language, expectedLanguage);
        }

        [TestMethod]
        public void DetectLanguageGoogle()
        {
            var text = "Καλημέρα";
            var detector = new GreeklishHelper.DetectLanguage(
                Enums.DetectLanguageProvider.Google,
                "YOUR_KEY_HERE");
            var expectedLanguage = "el";

            var detectedLanguage = detector.GetLanguage(text);

            Assert.AreEqual(detectedLanguage.Language, expectedLanguage);

        }

        /*[TestMethod]
        public void DetectLanguageBing()
        {
            var text = "good morning";
            var detector = new GreeklishHelper.DetectLanguage(
                Enums.DetectLanguageProvider.Bing,
                "");
            var expectedLanguage = "en";

            var detectedLanguage = detector.GetLanguage(text);

            Assert.AreEqual(detectedLanguage.Language, expectedLanguage);

        }*/
    }
}