namespace LinguisticTools.Spell
{
    using System;
    using System.Collections.Generic;

    public class SpellEngine : IDisposable
    {
        private readonly object dictionaryLock;
        private Dictionary<string, SpellFactory> languages;
        private int processors;

        public bool IsDisposed
        {
            get
            {
                lock (this.dictionaryLock)
                    return this.languages == null;
            }
        }

        public int Processors
        {
            get
            {
                return this.processors;
            }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("Processors", "Processors must be greater than 0");
                this.processors = value;
            }
        }

        public SpellFactory this[string language]
        {
            get
            {
                lock (this.dictionaryLock)
                    return this.languages[language.ToLower()];
            }
        }

        public SpellEngine()
        {
            this.processors = Environment.ProcessorCount;
            this.languages = new Dictionary<string, SpellFactory>();
            this.dictionaryLock = new object();
        }

        public void AddLanguage(LanguageConfig config)
        {
            string key = config.LanguageCode.ToLower();
            if (config.Processors < 1)
                config.Processors = this.Processors;
            SpellFactory spellFactory = new SpellFactory(config);
            lock (this.dictionaryLock)
                this.languages.Add(key, spellFactory);
        }

        public void Dispose()
        {
            if (this.IsDisposed)
                return;
            lock (this.dictionaryLock)
            {
                foreach (SpellFactory item_0 in this.languages.Values)
                    item_0.Dispose();
                this.languages = (Dictionary<string, SpellFactory>)null;
            }
        }
    }
}
