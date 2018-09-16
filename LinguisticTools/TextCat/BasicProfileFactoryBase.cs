namespace LinguisticTools.TextCat
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using LinguisticTools.TextCat.Classify;

    public abstract class BasicProfileFactoryBase<T>
    {

        public int MaxNGramLength { get; private set; }
        public int MaximumSizeOfDistribution { get; private set; }
        public int OccuranceNumberThreshold { get; private set; }
        public int OnlyReadFirstNLines { get; private set; }
        /// <summary>   
        /// true if it is allowed to use more than one thread for training
        /// </summary>
        public bool AllowUsingMultipleThreadsForTraining { get; private set; }

        public BasicProfileFactoryBase()
            : this(5, 4000, 0, int.MaxValue)
        {
        }

        public BasicProfileFactoryBase(int maxNGramLength, int maximumSizeOfDistribution, int occuranceNumberThreshold, int onlyReadFirstNLines, bool allowUsingMultipleThreadsForTraining = true)
        {
            this.MaxNGramLength = maxNGramLength;
            this.MaximumSizeOfDistribution = maximumSizeOfDistribution;
            this.OccuranceNumberThreshold = occuranceNumberThreshold;
            this.OnlyReadFirstNLines = onlyReadFirstNLines;
            this.AllowUsingMultipleThreadsForTraining = allowUsingMultipleThreadsForTraining;
        }

        public T Create(IEnumerable<LanguageModel<string>> languageModels)
        {
            return this.Create(languageModels, this.MaxNGramLength, this.MaximumSizeOfDistribution, this.OccuranceNumberThreshold, this.OnlyReadFirstNLines);
        }
        public abstract T Create(IEnumerable<LanguageModel<string>> languageModels, int maxNGramLength, int maximumSizeOfDistribution, int occuranceNumberThreshold, int onlyReadFirstNLines);

        public T Train(IEnumerable<Tuple<LanguageInfo, TextReader>> input)
        {
            var languageModels = this.TrainModels(input).ToList();
            var identifier = this.Create(languageModels);
            return identifier;
        }

        /// <summary>
        /// Disposes TextReader instances!
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public IEnumerable<LanguageModel<string>> TrainModels(IEnumerable<Tuple<LanguageInfo, TextReader>> input, bool toLower = false)
        {
            if (this.AllowUsingMultipleThreadsForTraining)
            {
                return input.AsParallel().AsOrdered()
                    .Select(
                        languageAndText =>
                            {
                                using (languageAndText.Item2)
                                {
                                    return this.TrainModel(languageAndText.Item1, languageAndText.Item2,toLower);
                                }
                            });
            }
            return input.Select(
                languageAndText =>
                    {
                        using (languageAndText.Item2)
                        {
                            return this.TrainModel(languageAndText.Item1, languageAndText.Item2,toLower);
                        }
                    });
        }

        private LanguageModel<string> TrainModel(LanguageInfo languageInfo, TextReader text, bool toLower = false)
        {
            IEnumerable<string> tokens = new CharacterNGramExtractor(this.MaxNGramLength, this.OnlyReadFirstNLines).GetFeatures(text, toLower);
            IDistribution<string> distribution = LanguageModelCreator.CreateLangaugeModel(tokens, this.OccuranceNumberThreshold, this.MaximumSizeOfDistribution);
            var languageModel = new LanguageModel<string>(distribution, languageInfo);
            return languageModel;
        }

        public void SaveProfile(IEnumerable<LanguageModel<string>> languageModels, string outputFilePath)
        {
            using (var file = File.OpenWrite(outputFilePath))
            {
                this.SaveProfile(languageModels, file);
            }
        }

        public void SaveProfile(IEnumerable<LanguageModel<string>> languageModels, Stream outputStream)
        {
            XmlProfilePersister.Save(languageModels, this.MaximumSizeOfDistribution, this.MaxNGramLength, outputStream);
        }

        public T TrainAndSave(IEnumerable<Tuple<LanguageInfo, TextReader>> input, string outputFilePath, bool toLower = false)
        {
            using (var file = File.OpenWrite(outputFilePath))
            {
                return this.TrainAndSave(input, file, toLower);
            }
        }

        public T TrainAndSave(IEnumerable<Tuple<LanguageInfo, TextReader>> input, Stream outputStream, bool toLower = false)
        {
            var languageModels = this.TrainModels(input,toLower).ToList();
            this.SaveProfile(languageModels, outputStream);
            return this.Create(languageModels, this.MaxNGramLength, this.MaximumSizeOfDistribution, this.OccuranceNumberThreshold, this.OnlyReadFirstNLines);
        }

        public T Load(Func<LanguageModel<string>, bool> filterPredicate = null)
        {
            using (Stream file = Assembly.GetExecutingAssembly().GetManifestResourceStream("LinguisticTools.Resources.LanguageDefinition.xml"))
            {
                return this.Load(file, filterPredicate);
            }
        }


        public T Load(string inputFilePath, Func<LanguageModel<string>, bool> filterPredicate = null)
        {
            using (var file = File.OpenRead(inputFilePath))
            {
                return this.Load(file, filterPredicate);
            }
        }

        public T Load(Stream inputStream, Func<LanguageModel<string>, bool> filterPredicate = null)
        {
            filterPredicate = filterPredicate ?? (_ => true);
            int maxNGramLength;
            int maximumSizeOfDistribution;
            var languageModelList =
                XmlProfilePersister.Load<string>(inputStream, out maximumSizeOfDistribution, out maxNGramLength)
                    .Where(filterPredicate);

            return this.Create(languageModelList, maxNGramLength, maximumSizeOfDistribution, this.OccuranceNumberThreshold, this.OnlyReadFirstNLines);
        }
    }
}
