﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using LinguisticTools.TextCat.Common;
using Models;
using Newtonsoft.Json;

namespace LinguisticTools
{
    using System.Threading.Tasks;

    public class LanguageDetector : IDetectLanguageClient
    {
        private static readonly Regex UrlRegex = new Regex("https?://[-_.?&~;+=/#0-9A-Za-z]{1,2076}", RegexOptions.Compiled);
        private static readonly Regex EmailRegex = new Regex("[-_.0-9A-Za-z]{1,64}@[-_0-9A-Za-z]{1,255}[-_.0-9A-Za-z]{1,255}", RegexOptions.Compiled);
        private const string ResourceNamePrefix = "LinguisticTools.Profiles.";
        private const double AlphaDefault = 0.5;
        private const double AlphaWidth = 0.05;
        private const int MaxIterations = 1000;
        private const double ProbabilityThreshold = 0.1;
        private const double ConvergenceThreshold = 0.99999;
        private const int BaseFrequency = 10000;

        private List<LanguageProfile> languages;
        private Dictionary<string, Dictionary<LanguageProfile, double>> wordLanguageProbabilities;

        public LanguageDetector(params string[] languages)
        {
            this.Alpha = AlphaDefault;
            this.RandomSeed = null;
            this.Trials = 7;
            this.NGramLength = 3;
            this.MaxTextLength = 10000;

            this.languages = new List<LanguageProfile>();
            this.wordLanguageProbabilities = new Dictionary<string, Dictionary<LanguageProfile, double>>();
            this.AddLanguages(languages);
        }

        public double Alpha { get; set; }
        public int? RandomSeed { get; set; }
        public int Trials { get; set; }
        public int NGramLength { get; set; }
        public int MaxTextLength { get; set; }

        public void AddAllLanguages()
        {
            var names = this.GetType().Assembly.GetManifestResourceNames();
            var selectedNames = names.Where(name => name.StartsWith(ResourceNamePrefix))
                .Select(name => name.Substring(ResourceNamePrefix.Length)).ToArray();
            this.AddLanguages(selectedNames);
        }

        public void AddLanguages(params string[] languages)
        {
            Assembly assembly = this.GetType().Assembly;

            foreach (string language in languages)
            {
                using (Stream stream = assembly.GetManifestResourceStream("LinguisticTools.Profiles." + language))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    this.AddLanguageProfile(JsonConvert.DeserializeObject<LanguageProfile>(json));
                }
            }
        }

        private void AddLanguageProfile(LanguageProfile profile)
        {
            this.languages.Add(profile);

            foreach (string word in profile.Frequencies.Keys)
            {
                if (!this.wordLanguageProbabilities.ContainsKey(word))
                    this.wordLanguageProbabilities[word] = new Dictionary<LanguageProfile, double>();

                if (word.Length >= 1 && word.Length <= this.NGramLength)
                {
                    double prob = (double)profile.Frequencies[word] / profile.WordCount[word.Length - 1];
                    this.wordLanguageProbabilities[word][profile] = prob;
                }
            }
        }

        public LanguageDetectionResult GetLanguage(string text)
        {
            return this.Detect(text);
        }

        public async Task<LanguageDetectionResult> GetLanguageAsync(string text)
        {
            return this.Detect(text);
        }

        public LanguageDetectionResult Detect(string text)
        {
            if (text.StringIsDouble())
            {
                return new LanguageDetectionResult()
                {
                    Language = "unknown",
                    Confidence = 1
                };
            }

            DetectedLanguage language = this.DetectAll(text).FirstOrDefault();
            var languageCode = language.Language;
            var probability = language.Probability;
            var languageEnum = languageCode.GetEnum(Enums.Language.Unknown);
            return new LanguageDetectionResult()
            {
                Language = Enum.GetName(typeof(Enums.Language), languageEnum).ToLower(),
                Confidence = probability
            };
        }

        public IEnumerable<DetectedLanguage> DetectAll(string text)
        {
            List<string> ngrams = this.ExtractNGrams(this.NormalizeText(text));
            if (ngrams.Count == 0)
                return new DetectedLanguage[0];

            double[] languageProbabilities = new double[this.languages.Count];

            Random random = this.RandomSeed != null ? new Random(this.RandomSeed.Value) : new Random();

            for (int t = 0; t < this.Trials; t++)
            {
                double[] probs = this.InitializeProbabilities();
                double alpha = this.Alpha + random.NextDouble() * AlphaWidth;

                for (int i = 0; ; i++)
                {
                    int r = random.Next(ngrams.Count);
                    this.UpdateProbabilities(probs, ngrams[r], alpha);

                    if (i % 5 == 0)
                    {
                        if (NormalizeProbabilities(probs) > ConvergenceThreshold || i >= MaxIterations)
                            break;
                    }
                }

                for (int j = 0; j < languageProbabilities.Length; j++)
                    languageProbabilities[j] += probs[j] / this.Trials;
            }

            return this.SortProbabilities(languageProbabilities);
        }

        private List<string> ExtractNGrams(string text)
        {
            List<string> list = new List<string>();

            NGram ngram = new NGram();

            foreach (char c in text)
            {
                ngram.Add(c);

                for (int n = 1; n <= NGram.N_GRAM; n++)
                {
                    string w = ngram.Get(n);

                    if (w != null && this.wordLanguageProbabilities.ContainsKey(w))
                        list.Add(w);
                }
            }

            return list;
        }

        #region Normalize text
        private string NormalizeText(string text)
        {
            if (text.Length > this.MaxTextLength)
                text = text.Substring(0, this.MaxTextLength);

            text = RemoveAddresses(text);
            text = NormalizeAlphabet(text);
            text = NormalizeVietnamese(text);
            text = NormalizeWhitespace(text);

            return text;
        }

        private static string NormalizeAlphabet(string text)
        {
            int latinCount = 0;
            int nonLatinCount = 0;

            for (int i = 0; i < text.Length; ++i)
            {
                char c = text[i];

                if (c <= 'z' && c >= 'A')
                {
                    ++latinCount;
                }
                else if (c >= '\u0300' && !(c >= 0x1e00 && c <= 0x1eff))
                {
                    ++nonLatinCount;
                }
            }

            if (latinCount * 2 < nonLatinCount)
            {
                StringBuilder textWithoutLatin = new StringBuilder();
                for (int i = 0; i < text.Length; ++i)
                {
                    char c = text[i];
                    if (c > 'z' || c < 'A')
                        textWithoutLatin.Append(c);
                }
                text = textWithoutLatin.ToString();
            }

            return text;
        }

        private static string NormalizeVietnamese(string text)
        {
            // todo
            return text;
        }

        private static string NormalizeWhitespace(string text)
        {
            StringBuilder sb = new StringBuilder(text.Length);

            char? prev = null;

            foreach (char c in text)
            {
                if (c != ' ' || prev != ' ')
                    sb.Append(c);
                prev = c;
            }

            return sb.ToString();
        }

        private static string RemoveAddresses(string text)
        {
            text = UrlRegex.Replace(text, " ");
            text = EmailRegex.Replace(text, " ");
            return text;
        }
        #endregion

        #region Probabilities
        private double[] InitializeProbabilities()
        {
            double[] prob = new double[this.languages.Count];
            for (int i = 0; i < prob.Length; i++)
                prob[i] = 1.0 / this.languages.Count;
            return prob;
        }

        private void UpdateProbabilities(double[] prob, string word, double alpha)
        {
            if (word == null || !this.wordLanguageProbabilities.ContainsKey(word))
                return;

            var languageProbabilities = this.wordLanguageProbabilities[word];
            double weight = alpha / BaseFrequency;

            for (int i = 0; i < prob.Length; i++)
            {
                LanguageProfile profile = this.languages[i];
                prob[i] *= weight + (languageProbabilities.ContainsKey(profile) ? languageProbabilities[profile] : 0);
            }
        }

        private static double NormalizeProbabilities(double[] probs)
        {
            double maxp = 0, sump = 0;

            for (int i = 0; i < probs.Length; ++i)
                sump += probs[i];

            for (int i = 0; i < probs.Length; ++i)
            {
                double p = probs[i] / sump;
                if (maxp < p) maxp = p;
                probs[i] = p;
            }

            return maxp;
        }

        private IEnumerable<DetectedLanguage> SortProbabilities(double[] probs)
        {
            List<DetectedLanguage> list = new List<DetectedLanguage>();

            for (int j = 0; j < probs.Length; j++)
            {
                double p = probs[j];

                if (p > ProbabilityThreshold)
                {
                    for (int i = 0; i <= list.Count; i++)
                    {
                        if (i == list.Count || list[i].Probability < p)
                        {
                            list.Insert(i, new DetectedLanguage { Language = this.languages[j].Code, Probability = p });
                            break;
                        }
                    }
                }
            }

            return list;
        }
        #endregion

        public class DetectedLanguage
        {
            public string Language { get; set; }
            public double Probability { get; set; }
        }

        private class NGram
        {
            public const int N_GRAM = 3;

            private StringBuilder buffer = new StringBuilder(" ", N_GRAM);
            private bool capital = false;

            public void Add(char c)
            {
                char lastChar = this.buffer[this.buffer.Length - 1];

                if (lastChar == ' ')
                {
                    this.buffer = new StringBuilder(" ");
                    this.capital = false;
                    if (c == ' ') return;
                }
                else if (this.buffer.Length >= N_GRAM)
                {
                    this.buffer.Remove(0, 1);
                }

                this.buffer.Append(c);

                if (char.IsUpper(c))
                {
                    if (char.IsUpper(lastChar))
                        this.capital = true;
                }
                else
                {
                    this.capital = false;
                }
            }

            public string Get(int n)
            {
                if (this.capital)
                    return null;

                if (n < 1 || n > N_GRAM || this.buffer.Length < n)
                    return null;

                if (n == 1)
                {
                    char c = this.buffer[this.buffer.Length - 1];
                    if (c == ' ') return null;
                    return c.ToString();
                }
                else
                {
                    return this.buffer.ToString(this.buffer.Length - n, this.buffer.Length - n);
                }
            }
        }

        private class LanguageProfile
        {
            [JsonProperty(PropertyName = "name")]
            public string Code { get; set; }
            [JsonProperty(PropertyName = "freq")]
            public Dictionary<string, int> Frequencies { get; set; }
            [JsonProperty(PropertyName = "n_words")]
            public int[] WordCount { get; set; }
        }
    }
}
