namespace LinguisticTools.Spell
{
    public class HyphenResult
    {
        private readonly int[] cut;
        private readonly byte[] points;
        private readonly int[] pos;
        private readonly string[] rep;
        private readonly string word;

        public string HyphenatedWord
        {
            get
            {
                return this.word;
            }
        }

        public int[] HyphenationCuts
        {
            get
            {
                return this.cut;
            }
        }

        public byte[] HyphenationPoints
        {
            get
            {
                return this.points;
            }
        }

        public int[] HyphenationPositions
        {
            get
            {
                return this.pos;
            }
        }

        public string[] HyphenationReplacements
        {
            get
            {
                return this.rep;
            }
        }

        public HyphenResult(string hyphenatedWord, byte[] hyphenationPoints, string[] hyphenationRep, int[] hyphenationPos, int[] hyphenationCut)
        {
            this.word = hyphenatedWord;
            this.points = hyphenationPoints;
            this.rep = hyphenationRep;
            this.pos = hyphenationPos;
            this.cut = hyphenationCut;
        }
    }
}
