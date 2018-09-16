namespace LinguisticTools.Spell
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Custom Hyphenation class that parses the hypen dictionary to create in memory dictionary
    /// </summary>
    public class MyThes
    {
        private readonly object dictionaryLock = new object();
        private readonly Dictionary<string, ThesMeaning[]> synonyms = new Dictionary<string, ThesMeaning[]>();

        public MyThes()
        {
        }

        public MyThes(byte[] datBytes)
        {
            this.Load(datBytes);
        }

        public MyThes(string datFile)
        {
            this.Load(datFile);
        }

        public static Encoding GetEncoding(string encoding)
        {
            encoding = encoding.Trim().ToLower();
            switch (encoding)
            {
                case "utf-8":
                case "utf8":
                    return Encoding.GetEncoding(65001);
                case "iso8859-1":
                case "iso-8859-1":
                    return Encoding.GetEncoding(28591);
                case "iso8859-2":
                case "iso-8859-2":
                    return Encoding.GetEncoding(28592);
                case "iso8859-3":
                case "iso-8859-3":
                    return Encoding.GetEncoding(28593);
                case "iso8859-4":
                case "iso-8859-4":
                    return Encoding.GetEncoding(28594);
                case "iso8859-5":
                case "iso-8859-5":
                    return Encoding.GetEncoding(28595);
                case "iso8859-6":
                case "iso-8859-6":
                    return Encoding.GetEncoding(28596);
                case "iso8859-7":
                case "iso-8859-7":
                    return Encoding.GetEncoding(28597);
                case "iso8859-8":
                case "iso-8859-8":
                    return Encoding.GetEncoding(28598);
                case "iso8859-9":
                case "iso-8859-9":
                    return Encoding.GetEncoding(28599);
                case "iso8859-13":
                case "iso-8859-13":
                    return Encoding.GetEncoding(28603);
                case "iso8859-15":
                case "iso-8859-15":
                    return Encoding.GetEncoding(28605);
                case "windows-1250":
                case "microsoft-cp1250":
                    return Encoding.GetEncoding(1250);
                case "windows-1251":
                case "microsoft-cp1251":
                    return Encoding.GetEncoding(1251);
                case "windows-1252":
                case "microsoft-cp1252":
                    return Encoding.GetEncoding(1252);
                case "windows-1253":
                case "microsoft-cp1253":
                    return Encoding.GetEncoding(1253);
                case "windows-1254":
                case "microsoft-cp1254":
                    return Encoding.GetEncoding(1254);
                case "windows-1255":
                case "microsoft-cp1255":
                    return Encoding.GetEncoding(1255);
                case "windows-1256":
                case "microsoft-cp1256":
                    return Encoding.GetEncoding(1256);
                case "windows-1257":
                case "microsoft-cp1257":
                    return Encoding.GetEncoding(1257);
                case "windows-1258":
                case "microsoft-cp1258":
                    return Encoding.GetEncoding(1258);
                case "windows-1259":
                case "microsoft-cp1259":
                    return Encoding.GetEncoding(1259);
                case "koi8-r":
                case "koi8-u":
                    return Encoding.GetEncoding(20866);
                default:
                    throw new NotSupportedException("Encoding: " + encoding + " is not supported");
            }
        }

        public void Load(byte[] dictionaryBytes)
        {
            if (this.synonyms.Count > 0)
                throw new InvalidOperationException("Thesaurus already loaded");
            //Detect encoding from header row which contains the encoding
            int headerRow = 0;
            int lineLength = this.GetLineLength(dictionaryBytes, headerRow);
            Encoding encoding = MyThes.GetEncoding(Encoding.ASCII.GetString(dictionaryBytes, headerRow, lineLength));
            //skip header
            int newLinePosition = headerRow + lineLength;
            string currentWord = string.Empty;
            List<ThesMeaning> list = new List<ThesMeaning>();
            int startOfLine;
            for (; newLinePosition < dictionaryBytes.Length; newLinePosition = startOfLine + lineLength)
            {
                startOfLine = newLinePosition + this.GetCrLfLength(dictionaryBytes, newLinePosition);
                lineLength = this.GetLineLength(dictionaryBytes, startOfLine);
                string currentDictionaryEntry = encoding.GetString(dictionaryBytes, startOfLine, lineLength).Trim();
                if (currentDictionaryEntry != null && currentDictionaryEntry.Length > 0)
                {
                    string[] strArray = currentDictionaryEntry.Split('|');
                    if (strArray.Length > 0)
                    {
                        bool flag = true;
                        if (string.IsNullOrEmpty(strArray[0]))
                            flag = false;
                        else if (strArray[0].StartsWith("-"))
                            flag = false;
                        else if (strArray[0].StartsWith("(") && strArray[0].EndsWith(")"))
                            flag = false;
                        if (flag)
                        {
                            lock (this.dictionaryLock)
                            {
                                if (currentWord.Length > 0)
                                {
                                    if (!this.synonyms.ContainsKey(currentWord.ToLowerInvariant()))
                                        this.synonyms.Add(currentWord.ToLowerInvariant(), list.ToArray());
                                }
                            }
                            list = new List<ThesMeaning>();
                            currentWord = strArray[0];
                        }
                        else
                        {
                            List<string> synonyms = new List<string>();
                            string description = (string)null;
                            for (int index = 1; index < strArray.Length; ++index)
                            {
                                synonyms.Add(strArray[index]);
                                if (index == 1)
                                    description = strArray[index];
                            }
                            ThesMeaning thesMeaning = new ThesMeaning(description, synonyms);
                            list.Add(thesMeaning);
                        }
                    }
                }
            }
            lock (this.dictionaryLock)
            {
                if (currentWord.Length <= 0 || this.synonyms.ContainsKey(currentWord.ToLowerInvariant()))
                    return;
                this.synonyms.Add(currentWord.ToLowerInvariant(), list.ToArray());
            }
        }

        public void Load(string dictionaryFile)
        {
            dictionaryFile = Path.GetFullPath(dictionaryFile);
            if (!File.Exists(dictionaryFile))
                throw new FileNotFoundException("DAT File not found: " + dictionaryFile);
            byte[] dictionaryBytes;
            using (FileStream fileStream = File.OpenRead(dictionaryFile))
            {
                using (BinaryReader binaryReader = new BinaryReader((Stream)fileStream))
                    dictionaryBytes = binaryReader.ReadBytes((int)fileStream.Length);
            }
            this.Load(dictionaryBytes);
        }

        public ThesResult Lookup(string word)
        {
            if (this.synonyms.Count == 0)
                throw new InvalidOperationException("Thesaurus not loaded");
            word = word.ToLowerInvariant();
            ThesMeaning[] thesMeaningArray;
            lock (this.dictionaryLock)
            {
                if (!this.synonyms.TryGetValue(word, out thesMeaningArray))
                    return (ThesResult)null;
            }
            return new ThesResult(new List<ThesMeaning>((IEnumerable<ThesMeaning>)thesMeaningArray), false);
        }

        public ThesResult Lookup(string word, Hunspell stemming)
        {
            if (this.synonyms.Count == 0)
                throw new InvalidOperationException("Thesaurus not loaded");
            ThesResult thesResult1 = this.Lookup(word);
            if (thesResult1 != null)
                return thesResult1;
            List<string> list = stemming.Stem(word);
            if (list == null || list.Count == 0)
                return (ThesResult)null;
            List<ThesMeaning> meanings = new List<ThesMeaning>();
            foreach (string word1 in list)
            {
                ThesResult thesResult2 = this.Lookup(word1);
                if (thesResult2 != null)
                {
                    foreach (ThesMeaning thesMeaning in thesResult2.Meanings)
                    {
                        List<string> synonyms = new List<string>();
                        foreach (string word2 in thesMeaning.Synonyms)
                        {
                            foreach (string str in stemming.Generate(word2, word))
                                synonyms.Add(str);
                        }
                        if (synonyms.Count > 0)
                            meanings.Add(new ThesMeaning(thesMeaning.Description, synonyms));
                    }
                }
            }
            if (meanings.Count > 0)
                return new ThesResult(meanings, true);
            return (ThesResult)null;
        }

        private int GetCrLfLength(byte[] buffer, int pos)
        {
            if (buffer[pos] == 10)
                return buffer.Length > pos + 1 && (int)buffer[pos] == 13 ? 2 : 1;
            if (buffer[pos] != 13)
                throw new ArgumentException("buffer[pos] dosen't point to CR or LF");
            return buffer.Length > pos + 1 && buffer[pos] == 10 ? 2 : 1;
        }

        /// <summary>
        /// Return distance to first new line
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        private int GetLineLength(byte[] buffer, int start)
        {
            for (int index = start; index < buffer.Length; ++index)
            {
                if (buffer[index] == 10 || buffer[index] == 13)
                    return index - start;
            }
            return buffer.Length - start;
        }
    }
}
