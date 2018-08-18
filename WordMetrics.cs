using System;
using System.Text;

namespace PrickleParser{
    public class WordMetrics : IComparable<WordMetrics> {
        private StringBuilder word;
        private StringBuilder styledAndDecoratedWord;
        private StringBuilder decoratedWord;
        private StringBuilder styledWord;
        
        private bool bold;
        private bool italic;
        private Color fillColor;
        private Color strokeColor;
        private string fontFamily;
        
        private Vector3D topLeft;
        private Vector3D topRight;
        private Vector3D bottomLeft;
        private Vector3D bottomRight;
        private float ascent;
        private float descent;
        private float baseline;
        private Rect rect;
        private float singleWhiteSpaceWidth;

        private int pageNb;
        private int lineNb;
        
        public WordMetrics(){
            word = new StringBuilder();
            bold = false;
            italic = false;

            ascent = -1;
            descent = -1;
            topRight = null;
            topLeft = null;
            bottomLeft = null;
            bottomRight = null;
            baseline = -1; // todo check for unset variables during get

            rect = null;
        }

        public void Append(string s){
            word.Append(s);
        }

        public StringBuilder Word{
            get => word;
            set => word = value;
        }

        public StringBuilder DecorateWord(){

            if (decoratedWord != null){
                return decoratedWord;
            }
            
            decoratedWord = new StringBuilder();

            if (Bold){
                decoratedWord.Append("<b>");
            }

            if (Italic){
                decoratedWord.Append("<i>");
            }

            decoratedWord.Append(word);
            
            if (Italic){
                decoratedWord.Append("</i>");
            }
            
            if (Bold){
                decoratedWord.Append("</b>");
            }

            return decoratedWord;
        }

        public StringBuilder StyleWord(){
            if (styledWord != null){
                return styledWord;
            }
            
            styledWord = new StringBuilder();
            styledWord.AppendFormat("<span style=\"font-family:{0};font-size:{1}\">", fontFamily, GetFontSize());
            styledWord.Append(word);
            styledWord.Append("</span>");
            return styledWord;
        }

        public StringBuilder StyleAndDecorateWord(){
            if (styledAndDecoratedWord != null){
                return styledAndDecoratedWord;
            }
            styledAndDecoratedWord = new StringBuilder();
            styledWord.AppendFormat("<span style=\"font-family:{0};font-size:{1}\">", fontFamily, GetFontSize());
            styledWord.Append(decoratedWord);
            styledWord.Append("</span>");
            return styledAndDecoratedWord;
        }

        public void ResetWordAlterations(){
            styledAndDecoratedWord = null;
            styledWord = null;
            decoratedWord = null;
        }

        public Rect GetRect(){
            return new Rect(bottomLeft, bottomRight, topLeft, topRight);
        }

        public float GetFontSize(){
            return ascent - descent;
        }

        public bool Bold{
            get => bold;
            set => bold = value;
        }

        public bool Italic{
            get => italic;
            set => italic = value;
        }

        public Color FillColor{
            get => fillColor;
            set => fillColor = value;
        }

        public Color StrokeColor{
            get => strokeColor;
            set => strokeColor = value;
        }

        public string FontFamily{
            get => fontFamily;
            set => fontFamily = value;
        }

        public Vector3D TopLeft{
            get => topLeft;
            set => topLeft = value;
        }

        public Vector3D TopRight{
            get => topRight;
            set => topRight = value;
        }

        public Vector3D BottomLeft{
            get => bottomLeft;
            set => bottomLeft = value;
        }

        public Vector3D BottomRight{
            get => bottomRight;
            set => bottomRight = value;
        }

        public float Ascent{
            get => ascent;
            set => ascent = value;
        }

        public float Descent{
            get => descent;
            set => descent = value;
        }

        public float Baseline{
            get => baseline;
            set => baseline = value;
        }

        public float SingleWhiteSpaceWidth{
            get => singleWhiteSpaceWidth;
            set => singleWhiteSpaceWidth = value;
        }

        public int CompareTo(WordMetrics other){
            if (other == null || other.topLeft == null || other.bottomRight == null ||
                topLeft == null || bottomRight == null){
                throw new Exception("Invalid comparison"); // todo
            }

            if (baseline > other.baseline){
                return -1;
            }
            else if (baseline < other.baseline){
                return 1;
            }
            else{
                if (BottomLeft.X < other.bottomLeft.X){
                    return -1;
                }
                else if (bottomLeft.X > other.bottomLeft.X){
                    return 1;
                }
            }
            return 1;
        }
    }
}