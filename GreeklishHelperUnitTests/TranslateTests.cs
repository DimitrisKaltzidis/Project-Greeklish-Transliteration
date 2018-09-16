namespace GreeklishHelperUnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Models;

    [TestClass]
    public class TranslateTests
    {
        [TestMethod]
        public void TranslateGreekToEnglish()
        {
            var text = "Τι είναι η μετάφραση;";
            var expected = "What is translation?";
            var translator = new GreeklishHelper.TranslateLanguage(Enums.TranslateLanguageProviders.Google,"YOUR_KEY_HERE");

            var result = translator.TranslateText(text, "el", "en");

            Assert.AreEqual(result, expected);
        }
    }
}
