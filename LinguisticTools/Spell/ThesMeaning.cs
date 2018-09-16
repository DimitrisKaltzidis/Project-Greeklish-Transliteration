namespace LinguisticTools.Spell
{
    using System.Collections.Generic;

    public class ThesMeaning
    {
        private readonly string description;
        private readonly List<string> synonyms;

        public string Description
        {
            get
            {
                return this.description;
            }
        }

        public List<string> Synonyms
        {
            get
            {
                return this.synonyms;
            }
        }

        public ThesMeaning(string description, List<string> synonyms)
        {
            this.description = description;
            this.synonyms = synonyms;
        }
    }
}
