using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf.parser;

namespace PrickleParser{
    public class PdfPageMetrics{

        private List<LineMetrics> lines;
        private List<WordMetric> words;
        private string text;
        private float avgLineSpacing;

        public PdfPageMetrics(){
            this.lines = new List<LineMetrics>();
            this.words = new List<WordMetric>();
            this.text = "";
        }

        public void AddWord(WordMetric word){
            words.Add(word);
        }

        public string Text{
            get => text;
            set => text = value;
        }

        public void BuildLines(){
            IDictionary<LineMetrics, List<WordMetric>> lineMappings = new Dictionary<LineMetrics, List<WordMetric>>();
            foreach(WordMetric wordMetric in words){
                LineMetrics line = new LineMetrics(wordMetric.Ascent, wordMetric.Baseline, wordMetric.Descent);
                if (lineMappings.ContainsKey(line)){
                    lineMappings[line].Add(wordMetric);
                    
                }
                else{
                    lineMappings.Add(line, new List<WordMetric>{wordMetric});
                }
            }

            foreach(KeyValuePair<LineMetrics, List<WordMetric>> entry in lineMappings){
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
                lines[1].LineSpacingBelow = -1;
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
                foreach(WordMetric wordMetric in line.Words){
                    sb.Append(wordMetric.Word);
                    sb.Append(" ");
                }
                Console.WriteLine(sb.ToString());
            }
        }
    }
}