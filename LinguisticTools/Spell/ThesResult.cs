namespace LinguisticTools.Spell
{
    using System.Collections.Generic;

    public class ThesResult
    {
        private readonly bool generatedByStem;
        private readonly List<ThesMeaning> meanings;

        public bool IsGenerated
        {
            get
            {
                return this.generatedByStem;
            }
        }

        public List<ThesMeaning> Meanings
        {
            get
            {
                return this.meanings;
            }
        }

        public ThesResult(List<ThesMeaning> meanings, bool generatedByStem)
        {
            this.meanings = meanings;
            this.generatedByStem = generatedByStem;
        }

        public Dictionary<string, List<ThesMeaning>> GetSynonyms()
        {
            Dictionary<string, List<ThesMeaning>> dictionary = new Dictionary<string, List<ThesMeaning>>();
            foreach (ThesMeaning thesMeaning in this.Meanings)
            {
                foreach (string key in thesMeaning.Synonyms)
                {
                    List<ThesMeaning> list;
                    if (!dictionary.TryGetValue(key, out list))
                    {
                        list = new List<ThesMeaning>(1);
                        dictionary.Add(key, list);
                    }
                    list.Add(thesMeaning);
                }
            }
            return dictionary;
        }
    }
}
