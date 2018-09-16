namespace LinguisticTools.TextCat
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using LinguisticTools.TextCat.Classify;

    public class Tokenizer : IFeatureExtractor<TextReader, string>, IFeatureExtractor<char[], string>, IFeatureExtractor<string, string> 
    {
        public int MaxLinesToRead { get; private set; }
        public Func<char, bool> IsSeparatorPredicate { get; private set; }

        public Tokenizer(int maxLinesToRead = int.MaxValue)
            :this(maxLinesToRead, IsSeparator)
        {
        }

        public Tokenizer(int maxLinesToRead, Func<char, bool> isSeparatorPredicate)
        {
            this.MaxLinesToRead = maxLinesToRead;
            this.IsSeparatorPredicate = isSeparatorPredicate;
        }

        #region Implementation of IFeatureExtractor<TextReader,string>

        IEnumerable<string> IFeatureExtractor<TextReader, string>.GetFeatures(TextReader obj, bool toLower = false)
        {
            return this.GetTokens(obj, toLower);
        }

        #endregion

        #region Implementation of IFeatureExtractor<char[],string>

        IEnumerable<string> IFeatureExtractor<char[], string>.GetFeatures(char[] obj, bool toLower = false)
        {
            return this.GetTokens(obj, toLower);
        }

        #endregion

        #region Implementation of IFeatureExtractor<string,string>

        IEnumerable<string> IFeatureExtractor<string, string>.GetFeatures(string obj, bool toLower = false)
        {
            return this.GetTokens(obj,toLower);
        }

        #endregion

        public IEnumerable<string> GetTokens(string text, bool toLower = false)
        {
            return this.GetTokens(new StringReader(text),toLower);
        }

        public IEnumerable<string> GetTokens(char[] text, bool toLower = false)
        {
            return this.GetTokens(new string(text),toLower);
        }

        public IEnumerable<string> GetTokens(TextReader text, bool toLower = false)
        {
            long numberOfLinesRead = 0;
            bool insideWord = false;
            var buffer = new char[4096];
            int charsRead;
            char previousByte = (char)0;
            var sb = new StringBuilder();
            while ((charsRead = text.Read(buffer, 0, buffer.Length)) > 0)
            {
                for (int i = 0; i < charsRead; i++)
                {
                    // here we have explicitly implemented transforming "abcdefg" into "_abcdefg_" and getting <1.._maxNGramLength>grams
                    char currentByte = buffer[i];
                    if (toLower) currentByte = System.Char.ToLower(currentByte);
                    if (currentByte == 0xD || currentByte == 0xA && previousByte != 0xD)
                        numberOfLinesRead++;
                    if (numberOfLinesRead >= this.MaxLinesToRead)
                        break;

                    if (insideWord)
                    {
                        if (this.IsSeparatorPredicate(currentByte))
                        {
                            insideWord = false;
                            yield return sb.ToString();
                            sb.Clear();
                        }
                        else
                        {
                            sb.Append(currentByte);
                        }

                    }
                    else
                    {
                        if (this.IsSeparatorPredicate(currentByte))
                        {
                            // skip it;
                        }
                        else
                        {
                            insideWord = true;
                            sb.Append(currentByte);
                        }
                    }

                    previousByte = currentByte;
                }
            }
            if (insideWord)
            {
                yield return sb.ToString();
            }
        }

        private static bool IsSeparator(char b)
        {
            return Char.IsLetter(b) == false;
        }
    }
}
