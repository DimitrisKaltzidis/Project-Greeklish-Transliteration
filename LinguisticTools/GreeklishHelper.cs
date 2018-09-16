namespace LinguisticTools
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using System.Threading.Tasks;

    using LinguisticTools.Spell;

    using Models;

    public class GreeklishHelper
    {
        /**
	 * Constant variables that represent the character that substitutes a
	 * digraph. The difference is that they are capital letters
	 */
        private const char AI = 'Α';
        private const char EI = 'Ε';
        private const char OI = 'Ο';
        private const char OY = 'Υ';
        private const char EY = 'Φ';
        private const char AY = 'Β';
        private const char MP = 'Μ';
        private const char GG = 'Γ';
        private const char GK = 'Κ';
        private const char NT = 'Ν';

        /**
         * The possible digraph cases.
         */
        private static Dictionary<string, string> digraphCases = new Dictionary<string, string>
        {
            {"αι", AI.ToString()},
            {"ει", EI.ToString()},
            {"οι", OI.ToString()},
            {"ου", OY.ToString()},
            {"ευ", EY.ToString()},
            {"αυ", AY.ToString()},
            {"μπ", MP.ToString()},
            {"γγ", GG.ToString()},
            {"γκ", GK.ToString()},
            {"ντ", NT.ToString()}
        };

        /**
         * The possible string conversions for each case.
         * This hash has keys all the possible conversions that can be applied and
         * values the strings that can replace the corresponding Greek character.
         */
        private static Dictionary<char, string[]> convertStrings = new Dictionary<char, string[]>
        {
            {AI, new string[] {"ai", "e"}},
            {EI, new string[] {"ei", "i"}},
            {OI, new string[] {"oi", "i"}},
            {OY, new string[] {"ou", "oy", "u"}},
            {EY, new string[] {"eu", "ef", "ev", "ey"}},
            {AY, new string[] {"au", "af", "av", "ay"}},
            {MP, new string[] {"mp", "b"}},
            {GG, new string[] {"gg", "g"}},
            {GK, new string[] {"gk", "g"}},
            {NT, new string[] {"nt", "d"}},
            {'α', new string[] {"a"}},
            {'β', new string[] {"b", "v"}},
            {'γ', new string[] {"g"}},
            {'δ', new string[] {"d", "nt"}},
            {'ε', new string[] {"e"}},
            {'ζ', new string[] {"z"}},
            {'η', new string[] {"h", "i"}},
            {'θ', new string[] {"th", "8"}},
            {'ι', new string[] {"i"}},
            {'κ', new string[] {"k", "c"}},
            {'λ', new string[] {"l"}},
            {'μ', new string[] {"m"}},
            {'ν', new string[] {"n"}},
            {'ξ', new string[] {"ks", "3"}},
            {'ο', new string[] {"o"}},
            {'π', new string[] {"p"}},
            {'ρ', new string[] {"r"}},
            {'σ', new string[] {"s"}},
            {'ς', new string[] {"s"}},
            {'τ', new string[] {"t"}},
            {'υ', new string[] {"y", "u", "i"}},
            {'φ', new string[] {"f", "ph"}},
            {'χ', new string[] {"x", "h", "ch"}},
            {'ψ', new string[] {"ps"}},
            {'ω', new string[] {"w", "o", "v"}}
        };

        public class Replacement
        {
            public Replacement(char r, char? nextChar = null)
            {
                this.Character = r;
                this.WhenNextCharacter = nextChar;
            }

            public char Character;
            public char? WhenNextCharacter;

            public bool CanReplace(char? nextChar)
            {
                if (this.WhenNextCharacter.HasValue)
                {
                    if (!nextChar.HasValue) return false;
                    return this.WhenNextCharacter.Value == nextChar.Value;
                }

                return true;
            }

            public override string ToString()
            {
                var output = "new Replacement('" + this.Character + "'";
                if (this.WhenNextCharacter.HasValue)
                {
                    output += ",'" + this.WhenNextCharacter.Value + "'";
                }

                return output + ")";
            }
        }

        /// <summary>
        /// To generager the reverse lookup fn call in the immediate window
        /// LinguisticTools.GreeklishHelper.GenerateReverseLookupFn()
        /// </summary>
        /// <returns></returns>
        public static string GenerateReverseLookupFn()
        {
            Dictionary<char, List<Replacement>> output = new Dictionary<char, List<Replacement>>();
            foreach (var k in convertStrings)
            {
                // Don't process digraph and final s
                if (Char.IsLower(k.Key) && k.Key != 'ς')
                {
                    foreach (var text in k.Value)
                    {
                        var c = text[0];
                        if (!output.ContainsKey(c))
                        {
                            output.Add(c, new List<Replacement>());
                        }

                        if (text.Length == 1)
                        {
                            output[c].Add(new Replacement(k.Key));
                        }
                        else if (text.Length == 2)
                        {
                            output[c].Add(new Replacement(k.Key, text[1]));
                        }
                        else
                        {
                            throw new Exception("Not implemented for more than 2 :(");
                        }
                    }
                }
            }

            var o = new StringBuilder();
            o.AppendLine(
                "private static Dictionary<char, Replacement[]> revertStrings = new Dictionary<char, Replacement[]>{");
            foreach (var item in output)
            {
                o.Append("\t\t\t{ '").Append(item.Key).Append("', new Replacement[]{");
                foreach (var rep in item.Value)
                {
                    o.Append(rep.ToString());
                    o.Append(",");
                }

                o.AppendLine("} },");
            }

            o.AppendLine("\t\t};");
            return o.ToString();
        }

        // The following is code generated from the above function in order to keep the model trained
        private static Dictionary<char, Replacement[]> revertStrings = new Dictionary<char, Replacement[]>
        {
            {'a', new Replacement[] {new Replacement('α'),}},
            {'b', new Replacement[] {new Replacement('β'),}},
            {'v', new Replacement[] {new Replacement('β'), new Replacement('ω'),}},
            {'g', new Replacement[] {new Replacement('γ'),}},
            {'d', new Replacement[] {new Replacement('δ'),}},
            {'n', new Replacement[] {new Replacement('δ', 't'), new Replacement('ν'),}},
            {'e', new Replacement[] {new Replacement('ε'),}},
            {'z', new Replacement[] {new Replacement('ζ'),}},
            {'h', new Replacement[] {new Replacement('η'), new Replacement('χ'),}},
            {'i', new Replacement[] {new Replacement('η'), new Replacement('ι'), new Replacement('υ'),}},
            {'t', new Replacement[] {new Replacement('θ', 'h'), new Replacement('τ'),}},
            {'8', new Replacement[] {new Replacement('θ'),}},
            {'k', new Replacement[] {new Replacement('κ'), new Replacement('ξ', 's'),}},
            {'c', new Replacement[] {new Replacement('κ'), new Replacement('χ', 'h'),}},
            {'l', new Replacement[] {new Replacement('λ'),}},
            {'m', new Replacement[] {new Replacement('μ'),}},
            {'3', new Replacement[] {new Replacement('ξ'),}},
            {'o', new Replacement[] {new Replacement('ο'), new Replacement('ω'),}},
            {'p', new Replacement[] {new Replacement('π'), new Replacement('φ', 'h'), new Replacement('ψ', 's'),}},
            {'r', new Replacement[] {new Replacement('ρ'),}},
            {'s', new Replacement[] {new Replacement('σ'),}},
            {'y', new Replacement[] {new Replacement('υ'),}},
            {'u', new Replacement[] {new Replacement('υ'),}},
            {'f', new Replacement[] {new Replacement('φ'),}},
            {'x', new Replacement[] {new Replacement('χ'),}},
            {'w', new Replacement[] {new Replacement('ω'),}},
        };


        /// <summary>
        /// Special tokenizer that allows all characters
        /// </summary>
        public static TextCat.Tokenizer GreeklishTokenizer = new TextCat.Tokenizer(int.MaxValue, (c) =>
            (Char.IsLetter(c) || revertStrings.ContainsKey(c)) == false
        );

        public static string GetPlainGreekLetters(string s)
        {
            return s;
            //return RemoveDiacritics(s).Replace("ς", "σ");
        }

        public static String RemoveDiacritics(String s)
        {
            String normalizedString = s.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < normalizedString.Length; i++)
            {
                Char c = normalizedString[i];
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }

        public static Dictionary<string, string> greeklishDictionary = null;

        private static Hunspell OpenGreekHunspell()
        {
            Hunspell _hunspell = null;
            using (Stream affStream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("LinguisticTools.Resources.GreekHunspell.el_GR.aff"))
            {
                using (Stream dicStream = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("LinguisticTools.Resources.GreekHunspell.el_GR.dic"))
                {
                    _hunspell = new Hunspell(affStream, dicStream);
                }
            }

            return _hunspell;
        }

        public static Dictionary<string, string> OpenGreeklishDictionary()
        {
            using (Stream stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("LinguisticTools.Resources.GreeklishDictiorany.bin"))
            {
                return OpenGreeklishDictionary(stream);
            }
        }

        public static Dictionary<string, string> OpenGreeklishDictionary(byte[] dictionary)
        {
            using (var ms = new MemoryStream(dictionary))
            {
                return OpenGreeklishDictionary(ms);
            }
        }

        public static Dictionary<string, string> OpenGreeklishDictionary(Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return (Dictionary<string, string>) formatter.Deserialize(stream);
        }

        /// <summary>
        /// Guess a greek sentence based on the greeklish one. It uses hunspell for checking
        /// </summary>
        /// <param name="output">A greeklish sentense</param>
        /// <returns></returns>
        public static string GuessSentense(string greeklishSentence)
        {
            var output = greeklishSentence.ToLower();
            if (greeklishDictionary == null)
            {
                greeklishDictionary = OpenGreeklishDictionary();
            }

            var tokens = GreeklishTokenizer.GetTokens(output);
            var tokensDic = new Dictionary<string, string>();
            Hunspell hunspell = null;
            foreach (var token in tokens)
            {
                if (!tokensDic.ContainsKey(token))
                {
                    int n;
                    if (int.TryParse(token, out n))
                    {
                        // ean exei poiasei to 3 i to 8 kai den exei kati allo
                        tokensDic.Add(token, token);
                    }
                    else if (greeklishDictionary.ContainsKey(token))
                    {
                        tokensDic.Add(token, greeklishDictionary[token]);
                    }
                    else
                    {
                        if (hunspell == null)
                        {
                            hunspell = OpenGreekHunspell();
                        }

                        var bestWord = UseGreekHunspell(token, hunspell);

                        tokensDic.Add(token, bestWord);
                    }
                }
            }

            if (hunspell != null)
            {
                hunspell.Dispose();
            }

            // Replace longest tokens first to avoid bad replacements (και and καιρός)
            foreach (var key in tokensDic.OrderByDescending(i => i.Key.Length))
            {
                output = output.Replace(key.Key, key.Value);
            }

            return output;
        }

        public static string UseGreekHunspell(string token, Hunspell hunspell)
        {
            if (hunspell == null)
            {
                hunspell = OpenGreekHunspell();
            }

            var guessList = GuessWords(token);
            var bestWord = GetBestGreekWord(hunspell, guessList);
            return bestWord;
        }

        /// <summary>
        /// From a list of guessed greek words retrieve the one that appears most times in the dictionary suggestions
        /// </summary>
        /// <param name="hunspell">The spell check engine to use</param>
        /// <param name="guesses">The greek guesses from a greeklish word</param>
        /// <returns></returns>
        private static string GetBestGreekWord(Hunspell hunspell, List<string> guesses)
        {
            var suggestions = new Dictionary<string, int>();
            foreach (var guess in guesses)
            {
                List<string> hunSuggests = hunspell.Suggest(guess);
                var scoreWeight = hunSuggests.Count == 1 ? 10 : 1;
                foreach (var hunSuggestion in hunSuggests)
                {
                    var hunSug = GetPlainGreekLetters(hunSuggestion);
                    if (suggestions.ContainsKey(hunSug))
                    {
                        suggestions[hunSug] += scoreWeight;
                    }
                    else
                    {
                        suggestions.Add(hunSug, scoreWeight);
                    }
                }
            }

            if (suggestions.Count > 0)
            {
                return suggestions.OrderByDescending(i => i.Value).First().Key;
            }
            else
            {
                return guesses.First();
            }
        }


        /// <summary>
        /// Return a list of possible greek words based on the greeklish word
        /// </summary>
        /// <param name="greeklishWord">Not validated list of greek letter words</param>
        /// <returns></returns>
        public static List<string> GuessWords(string greeklishWord)
        {
            var guessWord = greeklishWord.ToLower();
            //  Convert it back to array of characters. The iterations of each
            //  character will take place through this array.
            var inputToken = guessWord.ToCharArray();
            //  Iterate through the characters of the token and generate
            //  greeklish
            //  words
            var bufferSize = guessWord.Length * 2;
            var currentGrekList = new List<StringBuilder>();
            for (var i = 0; i < inputToken.Length; i++)
            {
                char cCur = inputToken[i];
                char? nCur = null;
                if (inputToken.Length > i + 1)
                {
                    nCur = inputToken[i + 1];
                }

                addGreekCharacter(currentGrekList, cCur, nCur);
            }


            return currentGrekList.Select(i => i.ToString()).ToList();
        }

        private static void addGreekCharacter(List<StringBuilder> currentGrekList, char cCur, char? nCur)
        {
            var charsToAppend = new List<char>();
            if (revertStrings.ContainsKey(cCur))
            {
                foreach (var rule in revertStrings[cCur])
                {
                    if (rule.CanReplace(nCur))
                    {
                        charsToAppend.Add(rule.Character);
                    }
                }
            }
            else
            {
                charsToAppend.Add(cCur);
            }

            if (charsToAppend.Count == 0)
            {
                // Just add the character as is
                foreach (StringBuilder atoken in currentGrekList)
                {
                    atoken.Append(cCur);
                }

                return;
            }

            if (currentGrekList.Count == 0)
            {
                //  If the token list is empty, create a new StringBuilder and add the
                //  greek chars
                foreach (char charToAppend in charsToAppend)
                {
                    StringBuilder greekWord = new StringBuilder();
                    greekWord.Append(charToAppend);
                    currentGrekList.Add(greekWord);
                }
            }
            else
            {
                //  Add the greek characters to each saved greek token, and
                //  generate new ones when the combinations are more than one.
                List<StringBuilder> additionalBuilders = new List<StringBuilder>();
                foreach (StringBuilder atoken in currentGrekList)
                {
                    if (charsToAppend.Count > 1)
                    {
                        foreach (char charToAppend in charsToAppend.Skip(1))
                        {
                            StringBuilder newToken = new StringBuilder(atoken.ToString());
                            newToken.Append(charToAppend);
                            additionalBuilders.Add(newToken);
                        }
                    }

                    atoken.Append(charsToAppend[0]);
                }

                currentGrekList.AddRange(additionalBuilders);
            }
        }


        private static Dictionary<string, string> normalAccents = new Dictionary<string, string>
        {
            {"ά", "α"},
            {"έ", "ε"},
            {"ί", "ι"},
            {"ή", "η"},
            {"ό", "ο"},
            {"ύ", "υ"},
            {"ώ", "ω"},
        };

        private static Dictionary<string, string> diaeresisAccents = new Dictionary<string, string>
        {
            {"ϋ", "υ"},
            {"ΰ", "υ"},
            {"ΐ", "ι"},
            {"ϊ", "ι"},
        };


        public static IEnumerable<string> GenerateGreeklishWords(IEnumerable<String> greekWords)
        {
            var output = new List<string>();
            var alreadyAdded = new HashSet<string>();
            foreach (String iWord in greekWords)
            {
                var currentGreklishList = GenerateGreeklishWords(iWord).ToList().Where(i => !alreadyAdded.Contains(i));
                output.AddRange(currentGreklishList);
                foreach (var i in currentGreklishList)
                {
                    alreadyAdded.Add(i);
                }
            }

            return output;
        }

        public static IEnumerable<string> GenerateGreeklishWords(string iWord)
        {
            var greekWord = iWord.ToLower();
            // Remove normal accents
            foreach (var key in normalAccents.Keys)
            {
                greekWord = greekWord.Replace(key, normalAccents[key]);
            }

            // Replace digraph with uppercase characters
            foreach (String key in digraphCases.Keys)
            {
                greekWord = greekWord.Replace(key, digraphCases[key]);
            }

            // remove special accents that should not be treated as diagraphs
            foreach (var key in diaeresisAccents.Keys)
            {
                greekWord = greekWord.Replace(key, diaeresisAccents[key]);
            }

            //  Convert it back to array of characters. The iterations of each
            //  character will take place through this array.
            var inputToken = greekWord.ToCharArray();
            //  Iterate through the characters of the token and generate
            //  greeklish
            //  words
            var bufferSize = greekWord.Length * 2;
            var currentGreklishList = new List<StringBuilder>();
            foreach (char greekChar in inputToken)
            {
                addGreeklishCharacter(currentGreklishList, convertStrings[greekChar], bufferSize);
            }

            return currentGreklishList.Select(i => i.ToString());
        }

        private static void addGreeklishCharacter(List<StringBuilder> currentGreklishList, String[] charsToAppend,
            int bufferSize)
        {
            if (currentGreklishList.Count == 0)
            {
                //  If the token list is empty, create a new StringBuilder and add the
                //  latin characters
                foreach (String charToAppend in charsToAppend)
                {
                    StringBuilder greeklishWord = new StringBuilder(bufferSize);
                    greeklishWord.Append(charToAppend);
                    currentGreklishList.Add(greeklishWord);
                }
            }
            else
            {
                //  Add the latin characters to each saved greeklish token, and
                //  generate new ones
                //  when the combinations are more than one.
                List<StringBuilder> additionalBuilders = new List<StringBuilder>();
                foreach (StringBuilder atoken in currentGreklishList)
                {
                    if (charsToAppend.Length > 1)
                    {
                        foreach (String charToAppend in charsToAppend.Skip(1))
                        {
                            StringBuilder newToken = new StringBuilder(atoken.ToString());
                            newToken.Append(charToAppend);
                            additionalBuilders.Add(newToken);
                        }
                    }

                    atoken.Append(charsToAppend[0]);
                }

                currentGreklishList.AddRange(additionalBuilders);
            }
        }

        public static string ToGreeklish(string iWord)
        {
            var greekWord = iWord.ToLower();
            // Remove normal accents
            foreach (var key in normalAccents.Keys)
            {
                greekWord = greekWord.Replace(key, normalAccents[key]);
            }

            // Replace digraph with uppercase characters
            foreach (String key in digraphCases.Keys)
            {
                greekWord = greekWord.Replace(key, digraphCases[key]);
            }

            // remove special accents that should not be treated as diagraphs
            foreach (var key in diaeresisAccents.Keys)
            {
                greekWord = greekWord.Replace(key, diaeresisAccents[key]);
            }

            //  Convert it back to array of characters. The iterations of each
            //  character will take place through this array.
            var inputToken = greekWord.ToCharArray();
            //  Iterate through the characters of the token and generate
            //  greeklish
            //  words
            var bufferSize = greekWord.Length * 2;
            var output = new StringBuilder(bufferSize);
            foreach (char greekChar in inputToken)
            {
                if (convertStrings.ContainsKey(greekChar))
                {
                    output.Append(convertStrings[greekChar][0]);
                }
                else
                {
                    output.Append(greekChar);
                }
            }

            return output.ToString();
        }
    }
}