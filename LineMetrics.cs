using System;
using System.Collections.Generic;
using System.Linq;

namespace PrickleParser{
    public class LineMetrics{
        private float ascent;
        private float baseline;
        private float descent;
        private float lineLineSpacingAbove;
        private float lineLineSpacingBelow;
        private float leftMostPos;
        private float rightMostPos;
        private List<ChunkMetrics> wordMetrices;

        public LineMetrics(float ascent, float baseline, float descent){
            wordMetrices = new List<ChunkMetrics>();
            this.ascent = ascent;
            this.baseline = baseline;
            this.descent = descent;
        }

        public void AddWords(List<ChunkMetrics> words){
            this.wordMetrices.AddRange(words);
        }

        public void AddWord(ChunkMetrics chunk){
            this.wordMetrices.Add(chunk);
        }

        public List<ChunkMetrics> WordMetrices => wordMetrices;

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

        public float LeftMostPos{
            get => leftMostPos;
            set => leftMostPos = value;
        }

        public float RightMostPos{
            get => rightMostPos;
            set => rightMostPos = value;
        }

        public float ContentHeight(){
            return Math.Abs(ascent - descent);
        }
        
        public float ContentWidth(){
            return Math.Abs(RightMostPos - LeftMostPos);
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

        public override string ToString(){
            return String.Join(" ", WordMetrices.Select(x => x.ToString()).ToArray());
        }
        
        public override int GetHashCode(){
            int hash = 13;
            hash = (hash * 7) + baseline.GetHashCode();
            return hash;
        }
    }
}