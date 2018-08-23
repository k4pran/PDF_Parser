using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrickleParser{
    public class PageMetrics{

        private int pageNumber;
        private List<LineMetrics> lineMetrices;
        private List<ChunkMetrics> chunkMetrices;
        private List<WordMetrics> wordMetrices;
        private List<CharMetrics> charMetrices;
        private string text;
        private float width;
        private float height;
        private float rotation;
        private float avgLineSpacing;

        public PageMetrics(int pageNumber){
            this.pageNumber = pageNumber;
            lineMetrices = new List<LineMetrics>();
            chunkMetrices = new List<ChunkMetrics>();
            wordMetrices = new List<WordMetrics>();
            charMetrices = new List<CharMetrics>();
            
            text = "";
        }

        public void AddWord(ChunkMetrics chunk){
            chunkMetrices.Add(chunk);
        }

        public void BuildLines(){
            IDictionary<LineMetrics, List<ChunkMetrics>> lineMappings = new Dictionary<LineMetrics, List<ChunkMetrics>>();
            
            foreach(ChunkMetrics wordMetric in chunkMetrices){
                LineMetrics line = new LineMetrics(wordMetric.Ascent, wordMetric.Baseline, wordMetric.Descent);
                if (lineMappings.ContainsKey(line)){
                    lineMappings[line].Add(wordMetric);
                }
                else{
                    lineMappings.Add(line, new List<ChunkMetrics>{wordMetric});
                }
            }
            
            foreach(KeyValuePair<LineMetrics, List<ChunkMetrics>> entry in lineMappings){
                entry.Key.AddWords(entry.Value);
                entry.Key.LeftMostPos = entry.Value.ElementAt(0).BottomLeft.X;
                entry.Key.RightMostPos = entry.Value.ElementAt(entry.Value.Count - 1).BottomRight.X;
                lineMetrices.Add(entry.Key);
            }
        }

        public void DetermineLineSpacing(){
            if (lineMetrices.Count == 0){
                avgLineSpacing = -1;
                return;
            }

            if (lineMetrices.Count == 1){
                avgLineSpacing = -1;
                lineMetrices[0].LineSpacingAbove = -1;
                lineMetrices[0].LineSpacingBelow = -1;
                return;
            }
            
            float[] spacings = {};

            float lastDescent = lineMetrices[0].Descent;
            lineMetrices[0].LineSpacingAbove = -1;
            for(int i = 1; i < lineMetrices.Count; i++){
                float spacing = lastDescent - lineMetrices[i].Ascent;
                lineMetrices[i].LineSpacingAbove = spacing;
                lineMetrices[i - 1].LineSpacingBelow = spacing;
                lastDescent = lineMetrices[i].Descent;
                spacings.Append(spacing);
            }
            lineMetrices[lineMetrices.Count - 1].LineSpacingBelow = -1;

            float sum = 0f;
            for(int i = 0; i < spacings.Length; i++){
                sum += spacings[i];
            }
            avgLineSpacing = sum / spacings.Length;
        }

        public void PrintLines(){
            foreach(LineMetrics line in lineMetrices){
                StringBuilder sb = new StringBuilder();
                foreach(ChunkMetrics wordMetric in line.WordMetrices){
                    sb.Append(wordMetric.WordStr);
                    sb.Append(" ");
                }
                Console.WriteLine(sb.ToString());
            }
        }

        public int PageNumber{
            get => pageNumber;
            set => pageNumber = value;
        }
        
        public string Text{
            get => text;
            set => text = value;
        }

        public List<LineMetrics> LineMetrices{
            get => lineMetrices;
            set => lineMetrices = value;
        }

        public List<ChunkMetrics> ChunkMetrices{
            get => chunkMetrices;
            set => chunkMetrices = value;
        }

        public List<WordMetrics> WordMetrices{
            get => wordMetrices;
            set => wordMetrices = value;
        }

        public List<CharMetrics> CharMetrices{
            get => charMetrices;
            set => charMetrices = value;
        }

        public float Width{
            get => width;
            set => width = value;
        }

        public float Height{
            get => height;
            set => height = value;
        }

        public float Rotation{
            get => rotation;
            set => rotation = value;
        }
    }
}