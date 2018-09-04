using System;
using System.Text;

namespace PrickleParser{
    public class ChunkMetrics : IComparable<ChunkMetrics>{
        private String chunkStr;
        private StringBuilder chunkSb;
        
        private bool bold;
        private bool italic;
        private bool strikethrough;
        private bool underline;
        private bool subscript;
        private bool superscript;
        private bool additionalSpacing;
        
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
        
        public ChunkMetrics(){
            chunkSb = new StringBuilder();
            chunkStr = "";
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
            chunkSb.Append(s);
            chunkStr = chunkSb.ToString();
        }

        public Rect GetRect(){
            return new Rect(bottomLeft, bottomRight, topLeft, topRight);
        }

        public float GetFontSize(){
            return ascent - descent;
        }

        public string ChunkStr{
            get => chunkStr;
            set => chunkStr = value;
        }

        public bool Bold{
            get => bold;
            set => bold = value;
        }

        public bool Italic{
            get => italic;
            set => italic = value;
        }

        public bool Strikethrough{
            get => strikethrough;
            set => strikethrough = value;
        }

        public bool Underline{
            get => underline;
            set => underline = value;
        }

        public bool Subscript{
            get => subscript;
            set => subscript = value;
        }

        public bool Superscript{
            get => superscript;
            set => superscript = value;
        }

        public bool AdditionalSpacing{
            get => additionalSpacing;
            set => additionalSpacing = value;
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
        
        public override string ToString(){
            return chunkStr;
        }

        public int CompareTo(ChunkMetrics other){
            if (other == null || other.TopLeft == null || other.BottomRight == null ||
                topLeft == null || bottomRight == null){
                throw new Exception("Invalid comparison"); // todo
            }

            if (baseline > other.baseline){
                return -1;
            }
            if (baseline < other.baseline){
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