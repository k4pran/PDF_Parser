using System;
using System.Text;
using iTextSharp.text;

namespace PrickleParser{
    public class WordMetric : IComparable<WordMetric> {
        private StringBuilder word;
        private StringBuilder styledAndDecoratedWord;
        private StringBuilder decoratedWord;
        private StringBuilder styledWord;
        private bool bold;
        private bool italic;
        private string fontFamily;
        private Vector3D topLeft;
        private Vector3D topRight;
        private Vector3D bottomLeft;
        private Vector3D bottomRight;
        private float ascent;
        private float descent;
        private float baseline;
        private Rect rect;

        private int pageNb;
        private int lineNb;

        public WordMetric(){
            this.word = new StringBuilder();
            this.bold = false;
            this.italic = false;

            this.ascent = -1;
            this.descent = -1;
            this.topRight = null;
            this.topLeft = null;
            this.bottomLeft = null;
            this.bottomRight = null;
            this.baseline = -1; // todo check for unset variables during get

            this.rect = null;
        }

        public void Append(string s){
            this.word.Append(s);
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
                return this.styledAndDecoratedWord;
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
            return new Rect(bottomLeft, bottomRight, bottomLeft, bottomRight);
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
        
        public int CompareTo(WordMetric other){
            if (other == null || other.topLeft == null || other.bottomRight == null ||
                this.topLeft == null || this.bottomRight == null){
                throw new Exception("Invalid comparison"); // todo
            }

            if (this.baseline > other.baseline){
                return -1;
            }
            else if (this.baseline < other.baseline){
                return 1;
            }
            else{
                if (this.BottomLeft.X < other.bottomLeft.X){
                    return -1;
                }
                else if (this.bottomLeft.X > other.bottomLeft.X){
                    return 1;
                }
            }
            return 1;
        }
    }
}