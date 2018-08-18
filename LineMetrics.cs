using System.Collections.Generic;

namespace PrickleParser{
    public class LineMetrics{
        private float ascent;
        private float baseline;
        private float descent;
        private float lineSpacingAbove;
        private float lineSpacingBelow;
        private List<WordMetrics> words;

        public LineMetrics(float ascent, float baseline, float descent){
            this.words = new List<WordMetrics>();
            this.ascent = ascent;
            this.baseline = baseline;
            this.descent = descent;
        }

        public void AddWords(List<WordMetrics> words){
            this.words.AddRange(words);
        }

        public void AddWord(WordMetrics word){
            this.words.Add(word);
        }

        public List<WordMetrics> Words{
            get{ return words; }
        }

        public float Ascent{
            get{ return ascent; }
            set{ ascent = value; }
        }

        public float Baseline{
            get{ return baseline; }
            set{ baseline = value; }
        }

        public float Descent{
            get{ return descent; }
            set{ descent = value; }
        }

        public float LineSpacingAbove{
            get{ return lineSpacingAbove; }
            set{ lineSpacingAbove = value; }
        }

        public float LineSpacingBelow{
            get{ return lineSpacingBelow; }
            set{ lineSpacingBelow = value; }
        }

        public override bool Equals(object obj){
            if (obj == null){
                return false;
            }
            
            var isCorrectType = obj is LineMetrics;
            if (!isCorrectType){
                return false;
            }

            LineMetrics toCompare = (LineMetrics) obj;
            return this.baseline == toCompare.baseline;
        }

        public override int GetHashCode(){
            int hash = 13;
            hash = (hash * 7) + baseline.GetHashCode();
            return hash;
        }
    }
}