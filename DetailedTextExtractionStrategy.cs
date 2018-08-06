using System;
using System.Text;
using iTextSharp.text.pdf.parser;

namespace PrickleParser{
    public class DetailedTextExtractionStrategy : ITextExtractionStrategy, IRenderListener{
        /// used to store the resulting String.

        private PdfPageMetrics pdfPageMetrics;
        
        private StringBuilder result = new StringBuilder();
        private WordMetric currWord;
        private bool flagNewWord = true;

        private Vector lastStart;
        private Vector lastEnd;
        private Vector lastTopRight;
    
        /// store font info.
        private string lastFont;
        private float lastFontSize;
                
        //http://api.itextpdf.com/itext/com/itextpdf/text/pdf/parser/TextRenderInfo.html
        private enum TextRenderMode{
            FillText = 0,
            StrokeText = 1,
            FillThenStrokeText = 2,
            Invisible = 3,
            FillTextAndAddToPathForClipping = 4,
            StrokeTextAndAddToPathForClipping = 5,
            FillThenStrokeTextAndAddToPathForClipping = 6,
            AddTextToPaddForClipping = 7
        }

        public DetailedTextExtractionStrategy(ref PdfPageMetrics pdfPageMetrics){
            this.pdfPageMetrics = pdfPageMetrics;
        }

        /// @since 5.0.1
        public virtual void BeginTextBlock() {
        }
    
        /// @since 5.0.1
        public virtual void EndTextBlock() {
            if (currWord != null){
                currWord.BottomRight = new Vector3D(lastEnd);
                currWord.TopRight = new Vector3D(lastTopRight);
                pdfPageMetrics.AddWord(currWord);
            }
            result.Append("</span>");
        }
    
        /// Returns the result so far.
        ///             @return  a String with the resulting text.
        public virtual string GetResultantText() {
          return result.ToString();
        }
    
        /// Used to actually append text to the text results.  Subclasses can use this to insert
        ///             text that wouldn't normally be included in text parsing (e.g. result of OCR performed against
        ///             image content)
        ///             @param text the text to append to the text results accumulated so far
        protected void AppendTextChunk(string text) {
            result.Append(text);
        }
    
        protected void AppendTextChunk(char text) {
            result.Append(text);
        }
    
        /// Captures text using a simplified algorithm for inserting hard returns and spaces
        ///             @param   renderInfo  render info
        public virtual void RenderText(TextRenderInfo renderInfo) {
            bool flagNoText = result.Length == 0;
            bool flagNewLine = false;

            if (flagNoText && renderInfo.GetText().Length > 0){
                StartNewWord(renderInfo);
            }
           
            LineSegment baseline = renderInfo.GetBaseline();
            Vector baseLineStartPoint = baseline.GetStartPoint();
            Vector baselineEndPoint = baseline.GetEndPoint();
            Vector ascentEndPoint = renderInfo.GetAscentLine().GetEndPoint();
            
            string currFont = renderInfo.GetFont().PostscriptFontName;
            //Check if faux bold is used
            if ((renderInfo.GetTextRenderMode() == (int)TextRenderMode.FillThenStrokeText) || // todo include render mode for outsets etc?
                    currFont.ToLower().Contains("bold")){
                currWord.Bold = true;
            }
                        
            iTextSharp.text.Rectangle rect = new iTextSharp.text.Rectangle(baseLineStartPoint[Vector.I1], baseLineStartPoint[Vector.I2], ascentEndPoint[Vector.I1], ascentEndPoint[Vector.I2]);
            Single currFontSize = rect.Height;

            if (renderInfo.GetFont().GetFontDescriptor(4, currFontSize) != 0 ||
                    currFont.ToLower().Contains("italic")){
                currWord.Italic = true;
            }


            if (currFontSize != lastFontSize || currFont != lastFont){
                //Create an HTML tag with appropriate styles
            }
            

            if (!flagNoText){
                Vector v = baseLineStartPoint;

                if ((lastEnd.Subtract(lastStart).Cross(lastStart.Subtract(v)).LengthSquared /
                     lastEnd.Subtract(lastStart).LengthSquared) > 1.0){
                    flagNewLine = true;
                }
            }


            if (flagNewLine){
                this.AppendTextChunk('\n');
                flagNewWord = true;
                StartNewWord(renderInfo);
            }
            else if (!flagNoText && this.result[this.result.Length - 1] != ' ' &&
                     (renderInfo.GetText().Length > 0 && renderInfo.GetText()[0] != ' ') &&
                     lastEnd.Subtract(baseLineStartPoint).Length > renderInfo.GetSingleSpaceWidth() / 2.0){
                this.AppendTextChunk(' ');
                StartNewWord(renderInfo);
            }
            
            AppendTextChunk(renderInfo.GetText());
            currWord.Append(renderInfo.GetText());
            
            lastStart = baseLineStartPoint;
            lastEnd = baselineEndPoint;
            lastFontSize = currFontSize;
            lastFont = currFont;
            lastTopRight = ascentEndPoint;
        }
    
        /// no-op method - this renderer isn't interested in image events
        ///             @see com.itextpdf.text.pdf.parser.RenderListener#renderImage(com.itextpdf.text.pdf.parser.ImageRenderInfo)
        ///             @since 5.0.1
        public virtual void RenderImage(ImageRenderInfo renderInfo) {
        }

        private void StartNewWord(TextRenderInfo renderInfo){
            if (currWord != null){
                currWord.BottomRight = new Vector3D(renderInfo.GetBaseline().GetEndPoint());
                currWord.TopRight = new Vector3D(renderInfo.GetAscentLine().GetEndPoint());
                pdfPageMetrics.AddWord(currWord);
            }

            currWord = new WordMetric();
            currWord.BottomLeft = new Vector3D(renderInfo.GetBaseline().GetStartPoint());
            currWord.TopLeft = new Vector3D(renderInfo.GetAscentLine().GetStartPoint());
            currWord.Baseline = renderInfo.GetBaseline().GetStartPoint()[Vector.I2];
            currWord.Descent = renderInfo.GetDescentLine().GetStartPoint()[Vector.I2];
            currWord.Ascent = renderInfo.GetAscentLine().GetStartPoint()[Vector.I2];
            currWord.FontFamily = renderInfo.GetFont().PostscriptFontName;
            flagNewWord = false;
        }
    }
}