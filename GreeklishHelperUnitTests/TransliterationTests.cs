namespace GreeklishHelperUnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TransliterationTests
    {
        [TestMethod]
        public void GuessGreeklishFromGreek()
        {
            var originalText = "Το αερόστρωμνό μου είναι γεμάτο χέλια";
            var expectedText = "to aerostrwmno mou einai gemato xelia";
            var transliterateLanguage = new GreeklishHelper.TransliterateLanguage();

            var resultText = transliterateLanguage.GenerateGreeklishFromGreek(originalText);

            Assert.AreEqual(resultText, expectedText);
        }

        [TestMethod]
        public void GuessGreekFromGreeklish()
        {
            var originalText = "geia sou, ths giagias sou.";
            var expectedText = "γεια σου, της γιαγιάς σου.";
            var transliterateLanguage = new GreeklishHelper.TransliterateLanguage();

            var resultText = transliterateLanguage.GuessGreekFromGreeklish(originalText);

            Assert.AreEqual(resultText, expectedText);
        }
    }
}