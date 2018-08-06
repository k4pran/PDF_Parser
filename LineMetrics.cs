using System.Collections.Generic;
using iTextSharp.text;

namespace PrickleParser{
    public class LineMetrics{
        private float ascent;
        private float baseline;
        private float descent;
        private float lineLineSpacingAbove;
        private float lineLineSpacingBelow;
        private List<WordMetric> words;

        public LineMetrics(float ascent, float baseline, float descent){
            this.words = new List<WordMetric>();
            this.ascent = ascent;
            this.baseline = baseline;
            this.descent = descent;
        }

        public void AddWords(List<WordMetric> words){
            this.words.AddRange(words);
        }

        public void AddWord(WordMetric word){
            this.words.Add(word);
        }

        public List<WordMetric> Words => words;

        public float Ascent{
            get => ascent;
            set => ascent = value;
        }

        public float Baseline{
            get => baseline;
            set => baseline = value;
        }

        public float Descent{
            get => descent;
            set => descent = value;
        }

        public float LineSpacingAbove{
            get => lineLineSpacingAbove;
            set => lineLineSpacingAbove = value;
        }

        public float LineSpacingBelow{
            get => lineLineSpacingBelow;
            set => lineLineSpacingBelow = value;
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