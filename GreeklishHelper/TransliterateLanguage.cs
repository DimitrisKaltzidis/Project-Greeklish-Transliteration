namespace GreeklishHelper
{
    using System.Collections.Generic;
    using System.Linq;

    using Models;

    // TODO I should souloupwsw this a bit
    public class TransliterateLanguage : ITransliterateLanguage
    {
        public string GuessGreekFromGreeklish(string input)
        {
            return LinguisticTools.GreeklishHelper.GuessSentense(input);
        }

        public string GenerateGreeklishFromGreek(string text)
        {
            return LinguisticTools.GreeklishHelper.ToGreeklish(text);
        }
    }
}