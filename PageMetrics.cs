using System;
using System.Collections.Generic;
using System.Text;

namespace PrickleParser{
    public class PageMetrics{

        private int pageNumber;
        private List<LineMetrics> lines;
        private List<WordMetrics> words;
        private string text;
        private float width;
        private float height;
        private float rotation;
        private float avgLineSpacing;

        public PageMetrics(int pageNumber){
            this.pageNumber = pageNumber;
            this.lines = new List<LineMetrics>();
            this.words = new List<WordMetrics>();
            this.text = "";
        }

        public void AddWord(WordMetrics word){
            words.Add(word);
        }

        public void BuildLines(){
            IDictionary<LineMetrics, List<WordMetrics>> lineMappings = new Dictionary<LineMetrics, List<WordMetrics>>();
            foreach(WordMetrics wordMetric in words){
                LineMetrics line = new LineMetrics(wordMetric.Ascent, wordMetric.Baseline, wordMetric.Descent);
                if (lineMappings.ContainsKey(line)){
                    lineMappings[line].Add(wordMetric);
                    
                }
                else{
                    lineMappings.Add(line, new List<WordMetrics>{wordMetric});
                }
            }

            foreach(KeyValuePair<LineMetrics, List<WordMetrics>> entry in lineMappings){
                entry.Key.AddWords(entry.Value);
                lines.Add(entry.Key);
            }
        }

        public void DetermineLineSpacing(){
            if (lines.Count == 0){
                avgLineSpacing = -1;
                return;
            }

            if (lines.Count == 1){
                avgLineSpacing = -1;
                lines[0].LineSpacingAbove = -1;
                lines[0].LineSpacingBelow = -1;
                return;
            }
            
            List<float> spacings = new List<float>();

            float lastDescent = lines[0].Descent;
            lines[0].LineSpacingAbove = -1;
            for(int i = 1; i < lines.Count; i++){
                float spacing = lastDescent - lines[i].Ascent;
                lines[i].LineSpacingAbove = spacing;
                lines[i - 1].LineSpacingBelow = spacing;
                lastDescent = lines[i].Descent;
                spacings.Add(spacing); // todo avg vs max spacing?
            }
            lines[lines.Count - 1].LineSpacingBelow = -1;

            float sum = 0f;
            for(int i = 0; i < spacings.Count; i++){
                sum += spacings[i];
            }
            avgLineSpacing = sum / spacings.Count;
        }

        public void PrintLines(){
            foreach(LineMetrics line in lines){
                StringBuilder sb = new StringBuilder();
                foreach(WordMetrics wordMetric in line.Words){
                    sb.Append(wordMetric.Word);
                    sb.Append(" ");
                }
                Console.WriteLine(sb.ToString());
            }
        }

        public int PageNumber{
            get{ return pageNumber; }
        }

        public string Text{
            get{ return text; }
            set{ text = value; }
        }

        public float Width{
            get{ return width; }
            set{ width = value; }
        }

        public float Height{
            get{ return height; }
            set{ height = value; }
        }

        public float Rotation{
            get{ return rotation; }
            set{ rotation = value; }
        }

        public float AvgLineSpacing{
            get{ return avgLineSpacing; }
            set{ avgLineSpacing = value; }
        }
    }
}