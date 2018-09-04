using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrickleParser{
    public class WordMetrics : IComparable<WordMetrics>{

        private String wordStr;
        
        private Vector3D topLeft;
        private Vector3D bottomLeft;
        private Vector3D topRight;
        private Vector3D bottomRight;

        public static List<WordMetrics> FromChars(List<CharMetrics> chars){
            
            chars.Sort();
            
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

        public string WordStr{
            get => wordStr;
            set => wordStr = value;
        }

        public Vector3D TopLeft{
            get => topLeft;
            set => topLeft = value;
        }

        public Vector3D BottomLeft{
            get => bottomLeft;
            set => bottomLeft = value;
        }

        public Vector3D TopRight{
            get => topRight;
            set => topRight = value;
        }

        public Vector3D BottomRight{
            get => bottomRight;
            set => bottomRight = value;
        }

        public int CompareTo(WordMetrics other){
            if (other == null || other.TopLeft == null || other.BottomRight == null ||
                topLeft == null || bottomRight == null){
                throw new Exception("Invalid comparison"); // todo
            }

            if (bottomLeft.Y > other.BottomLeft.Y){
                return -1;
            }
            if (bottomLeft.Y < other.BottomLeft.Y){
                return 1;
            }
            if (bottomLeft.X < other.BottomLeft.X){
                return -1;
            }
            if (bottomLeft.X > other.BottomLeft.X){
                return 1;
            }
            return 1;
        }
    }
}