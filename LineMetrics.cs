using System;
using System.Collections.Generic;
using System.Linq;

namespace PrickleParser{
    public class LineMetrics : IComparable<LineMetrics>{
        private float ascent;
        private float baseline;
        private float descent;
        private float lineLineSpacingAbove;
        private float lineLineSpacingBelow;
        private float leftMostPos;
        private float rightMostPos;
        private List<ChunkMetrics> chunkMetrices;

        public LineMetrics(){
            chunkMetrices = new List<ChunkMetrics>();
        }

        public LineMetrics(float ascent, float baseline, float descent){
            chunkMetrices = new List<ChunkMetrics>();
            this.ascent = ascent;
            this.baseline = baseline;
            this.descent = descent;
        }
        
        public void AddChunks(List<ChunkMetrics> chunks){
            chunkMetrices.AddRange(chunks);
        }

        public void AddChunk(ChunkMetrics chunk){
            chunkMetrices.Add(chunk);
        }

        public List<ChunkMetrics> ChunkMetrices => chunkMetrices;

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
            return Math.Abs(rightMostPos - leftMostPos);
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
            return baseline == toCompare.baseline;
        }

        public override string ToString(){
            return String.Join("", ChunkMetrices.Select(x => x.ToString()).ToArray());
        }
        
        public override int GetHashCode(){
            int hash = 13;
            hash = (hash * 7) + baseline.GetHashCode();
            return hash;
        }
        
        public int CompareTo(LineMetrics other){
            if (other == null) {
                throw new Exception("Invalid comparison"); // todo
            }

            if (baseline > other.baseline){
                return -1;
            }
            if (baseline < other.baseline){
                return 1;
            }
            return 1;
        }
    }
}