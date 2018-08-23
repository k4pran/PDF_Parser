using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrickleParser{
    public class WordMetrics{

        private String wordStr;
        
        private Vector3D topLeft;
        private Vector3D bottomLeft;
        private Vector3D topRight;
        private Vector3D bottomRight;

        public static List<WordMetrics> FromOrderedChars(List<CharMetrics> chars){
            
            List<WordMetrics> wordMetrices = new List<WordMetrics>();
            if (chars.Count == 0){
                return wordMetrices;
            }
            
            WordMetrics wordMetrics = new WordMetrics();
            StringBuilder sb = new StringBuilder();
            wordMetrics.bottomLeft = chars.ElementAt(0).BottomLeft;
            wordMetrics.topLeft = chars.ElementAt(0).TopLeft;
            foreach(CharMetrics c in chars){
                if (c.C == ' '){
                    wordMetrics.bottomRight = c.BottomRight;
                    wordMetrics.topRight = c.TopRight;
                    wordMetrics.wordStr = sb.ToString();
                    wordMetrices.Add(wordMetrics);

                    sb = new StringBuilder();
                    wordMetrics = new WordMetrics();
                    wordMetrics.bottomLeft = c.BottomLeft;
                    wordMetrics.topLeft = c.TopLeft;
                    continue;
                }
                sb.Append(c.C);
            }
            return wordMetrices;
        }
    }
}