using System;
using System.Collections.Generic;
using System.Linq;
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
            
            float[] spacings = {};

            float lastDescent = lines[0].Descent;
            lines[0].LineSpacingAbove = -1;
            for(int i = 1; i < lines.Count; i++){
                float spacing = lastDescent - lines[i].Ascent;
                lines[i].LineSpacingAbove = spacing;
                lines[i - 1].LineSpacingBelow = spacing;
                lastDescent = lines[i].Descent;
                spacings.Append(spacing);
            }
            lines[lines.Count - 1].LineSpacingBelow = -1;

            float sum = 0f;
            for(int i = 0; i < spacings.Length; i++){
                sum += spacings[i];
            }
            avgLineSpacing = sum / spacings.Length;
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
            get => pageNumber;
            set => pageNumber = value;
        }
        
        public string Text{
            get => text;
            set => text = value;
        }

        public List<LineMetrics> Lines{
            get => lines;
            set => lines = value;
        }

        public List<WordMetrics> Words{
            get => words;
            set => words = value;
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