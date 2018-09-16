namespace LinguisticTools.Spell
{
    using System;
    using System.IO;

    public class LanguageConfig
    {
        private string hunspellAffFile;
        private string hunspellDictFile;
        private string hyphenDictFile;
        private string languageCode;
        private string myThesDatFile;
        private string myThesIdxFile;
        private int processors;

        public string HunspellAffFile
        {
            get
            {
                return this.hunspellAffFile;
            }
            set
            {
                string fullPath = Path.GetFullPath(value);
                if (!File.Exists(fullPath))
                    throw new FileNotFoundException("Hunspell Aff file not found: " + fullPath);
                this.hunspellAffFile = fullPath;
            }
        }

        public string HunspellDictFile
        {
            get
            {
                return this.hunspellDictFile;
            }
            set
            {
                string fullPath = Path.GetFullPath(value);
                if (!File.Exists(fullPath))
                    throw new FileNotFoundException("Hunspell Dict file not found: " + fullPath);
                this.hunspellDictFile = fullPath;
            }
        }

        public string HunspellKey { get; set; }

        public string HyphenDictFile
        {
            get
            {
                return this.hyphenDictFile;
            }
            set
            {
                string fullPath = Path.GetFullPath(value);
                if (!File.Exists(fullPath))
                    throw new FileNotFoundException("Hyphen Dict file not found: " + fullPath);
                this.hyphenDictFile = fullPath;
            }
        }

        public string LanguageCode
        {
            get
            {
                return this.languageCode;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("LanguageCode cannot be null");
                if (value == string.Empty)
                    throw new ArgumentException("LanguageCode cannot be empty");
                this.languageCode = value.ToLower();
            }
        }

        public string MyThesDatFile
        {
            get
            {
                return this.myThesDatFile;
            }
            set
            {
                string fullPath = Path.GetFullPath(value);
                if (!File.Exists(fullPath))
                    throw new FileNotFoundException("MyThes Dat file not found: " + fullPath);
                this.myThesDatFile = fullPath;
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
    }
}
