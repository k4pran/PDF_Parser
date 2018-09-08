using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Xobject;

namespace PrickleParser{
    public class DeepExtractionStrategy : ITextExtractionStrategy {
        /// used to store the resulting String.

        private PageMetrics pageMetrics;
        private List<CharMetrics> charMetrices;
        
        private StringBuilder result = new StringBuilder();
        private LineMetrics currLine;
        private ChunkMetrics currChunk;

        private Vector bottomLeft;
        private Vector topLeft;
        private Vector bottomRight;
        private Vector topRight;
        
        private float ascent;
        private float baseline;
        private float descent;

        private bool startOfChunk;
        private bool startOfNewline;

        private float widthSpaceFromLastChunk;

        private float spaceWidth;
       
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

        public DeepExtractionStrategy(ref PageMetrics pageMetrics){            
            this.pageMetrics = pageMetrics;
            charMetrices = new List<CharMetrics>();
            startOfNewline = true;
            descent = pageMetrics.Height;
        }

        public virtual void BeginTextBlock(){
            if (currChunk == null){
                StartNewWord();
            }
            startOfChunk = true;
        }
    
        public virtual void EndTextBlock() {
            StartNewWord();
        }
    
        /// Returns the result so far.
        ///     @return  a String with the resulting text.
        public virtual string GetResultantText() {
          return result.ToString();
        }
    
        /// Captures text using a simplified algorithm for inserting hard returns and spaces
        ///     @param   renderInfo  render info
        public virtual void RenderText(TextRenderInfo renderInfo){
            
            // todo handle adding last line

            string text = renderInfo.GetText().Replace('\t', ' ');
            
            if (!Utils.FloatsNearlyEqual(renderInfo.GetBaseline().GetStartPoint().Get(Vector.I2), baseline, 0.01f) &&
                !Utils.FloatsNearlyEqual(renderInfo.GetAscentLine().GetStartPoint().Get(Vector.I2), ascent, 0.01f)){
                startOfNewline = true;
            }

            if (startOfNewline){
                if (currLine != null){
                    if (currChunk.ChunkStr.Length > 0){
                        StartNewWord();
                        startOfChunk = true;
                    }
                    currLine.RightMostPos = bottomRight.Get(Vector.I1);
                    currLine.LineSpacingBelow =
                        currLine.Descent - renderInfo.GetAscentLine().GetStartPoint().Get(Vector.I2);
                }

                currLine = new LineMetrics();
                pageMetrics.AddLine(currLine);
                currLine.Ascent = renderInfo.GetAscentLine().GetStartPoint().Get(Vector.I2);
                currLine.Descent = renderInfo.GetDescentLine().GetStartPoint().Get(Vector.I2);
                currLine.Baseline = renderInfo.GetBaseline().GetStartPoint().Get(Vector.I2);
                currLine.LeftMostPos = renderInfo.GetBaseline().GetStartPoint().Get(Vector.I1);
                currLine.LineSpacingAbove = descent - currLine.Ascent;
                
                startOfNewline = false;
            }
            
            CharMetrics charMetrics;
            // Check if there is space char required between chunks
            if (GetResultantText().Length > 0 &&
                IsSpaceRequired(bottomRight.Get(Vector.I1), renderInfo.GetBaseline().GetStartPoint().Get(Vector.I1),
                renderInfo.GetBaseline().GetStartPoint().Get(Vector.I2), 
                renderInfo.GetSingleSpaceWidth())){ // todo switch order

                charMetrics = new CharMetrics(' ');
                charMetrics.BottomLeft = new Vector3D(bottomLeft);
                charMetrics.TopLeft = new Vector3D(topLeft);
                charMetrics.BottomRight = new Vector3D(renderInfo.GetBaseline().GetStartPoint());
                charMetrics.TopRight = new Vector3D(renderInfo.GetAscentLine().GetStartPoint());
                this.charMetrices.Add(charMetrics);
                text = " " + text; // todo extra spacing? e.g. handle if it is 4x singlespace width
            }

            TextRenderInfo lastChar = null;
            int charInd = 0;
            foreach(TextRenderInfo charInfo in renderInfo.GetCharacterRenderInfos()){

                if (charInfo.GetText().Length == 0){
                    continue;
                }

                if (lastChar != null && IsSpaceRequired(lastChar.GetBaseline().GetEndPoint().Get(Vector.I1), 
                                                        charInfo.GetBaseline().GetStartPoint().Get(Vector.I1),
                                                        renderInfo.GetSingleSpaceWidth())){
                    charMetrics = new CharMetrics(' ');
                    charMetrics.FontSize = charInfo.GetFontSize();
                    charMetrics.BottomLeft = new Vector3D(lastChar.GetBaseline().GetEndPoint());
                    charMetrics.TopLeft = new Vector3D(lastChar.GetAscentLine().GetEndPoint());
                    charMetrics.BottomRight = new Vector3D(charInfo.GetBaseline().GetStartPoint());
                    charMetrics.TopRight = new Vector3D(charInfo.GetAscentLine().GetStartPoint());
                    charMetrices.Add(charMetrics);
                    text = text.Insert(charInd + 1, " ");
                }

                char c = charInfo.GetText()[0] == '\t' ? ' ' : charInfo.GetText()[0];
                charMetrics = new CharMetrics(c);
                charMetrics.FontSize = charInfo.GetFontSize();
                charMetrics.BottomLeft = new Vector3D(charInfo.GetBaseline().GetStartPoint());
                charMetrics.TopLeft = new Vector3D(charInfo.GetAscentLine().GetStartPoint());
                charMetrics.BottomRight = new Vector3D(charInfo.GetBaseline().GetEndPoint());
                charMetrics.TopRight = new Vector3D(charInfo.GetAscentLine().GetEndPoint());
                charMetrices.Add(charMetrics);

                lastChar = charInfo;
                charInd++;
            }

            if (startOfChunk){

                if (Utils.FloatsNearlyEqual(currLine.Baseline, baseline, 0.001f)){
                    float horSpacing = Math.Abs(
                        bottomRight.Get(Vector.I1) - renderInfo.GetBaseline().GetStartPoint().Get(Vector.I1));
                    if (horSpacing > spaceWidth){
                        Console.Write("");
                    }
                }
                
                bottomLeft = renderInfo.GetBaseline().GetStartPoint();
                topLeft = renderInfo.GetAscentLine().GetStartPoint();
                currChunk.BottomLeft = new Vector3D(bottomLeft);
                currChunk.TopLeft = new Vector3D(topLeft);
                baseline = renderInfo.GetBaseline().GetStartPoint().Get(Vector.I2);
                ascent = renderInfo.GetAscentLine().GetStartPoint().Get(Vector.I2);
                descent = renderInfo.GetDescentLine().GetStartPoint().Get(Vector.I2);
                spaceWidth = renderInfo.GetSingleSpaceWidth();
                                                
                startOfChunk = false;
            }
            
            bottomRight = renderInfo.GetBaseline().GetEndPoint();
            topRight = renderInfo.GetAscentLine().GetEndPoint();
            
            string currFont = renderInfo.GetFont() != null ? renderInfo.GetFont().ToString() : "";
            if (currChunk.FontFamily == null && currFont.Length > 0){
                currChunk.FontFamily = currFont;
            }
            
            //Check if faux bold is used
            if ((renderInfo.GetTextRenderMode() == (int)TextRenderMode.FillThenStrokeText) || // todo include render mode for outsets etc?
                    renderInfo.GetFont().GetFontProgram().GetFontNames().IsBold()){
                currChunk.Bold = true;
            }

            if (!(Utils.FloatsNearlyEqual(renderInfo.GetFont().GetFontProgram().GetFontMetrics().GetItalicAngle(), 0, 0.001f)) ||
                    renderInfo.GetFont().GetFontProgram().GetFontNames().IsItalic()){
                currChunk.Italic = true;
            }
            
            if (currChunk.FillColor == null && renderInfo.GetFillColor() != null){
                currChunk.FillColor = ProcessColorInfo(renderInfo.GetFillColor());
            }     
                        
            if (currChunk.StrokeColor == null && renderInfo.GetStrokeColor() != null){
                currChunk.StrokeColor = ProcessColorInfo(renderInfo.GetStrokeColor());
            }    
            
            currChunk.Append(text);
            result.Append(text); // todo include newlines
        }

        public Color ProcessColorInfo(iText.Kernel.Colors.Color color){
            if (color.GetColorSpace() is PdfDeviceCs.Cmyk){
                color = iText.Kernel.Colors.Color.ConvertCmykToRgb((DeviceCmyk) color);
            }
            
            if (color.GetNumberOfComponents() == 1){
                return new Color((int) color.GetColorValue()[0]);
            }
                
            if (color.GetNumberOfComponents() == 3){
                return new Color(color.GetColorValue()[0], 
                                 color.GetColorValue()[1], 
                                 color.GetColorValue()[2]);
            }
            return null; // todo handle
        }

        // todo better option for determining space as spaceWidth is not always accurate
        public bool IsSpaceRequired(float left, float right, float spaceWidth){
            return Utils.FloatsNearlyEqual(right - left, spaceWidth / 2, 0.001f) || 
                    right - left > spaceWidth / 3;
        }

        public bool IsSpaceRequired(float left, float right, float currChunkBaseline, float spaceWidth){
            if (Utils.FloatsNearlyEqual(left - right, spaceWidth / 2, 0.001f) ||
            right - left > spaceWidth / 3){
                if (Utils.FloatsNearlyEqual(currChunkBaseline, baseline, 0.001f)){
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// Processes and stores a new word with metrics then starts the next word.
        /// </summary>
        /// <param name="renderInfo">Text rendering info</param>
        private void StartNewWord(){
            if (currChunk != null){
                currChunk.BottomRight = new Vector3D(bottomRight);
                currChunk.TopRight = new Vector3D(topRight);
                currChunk.Ascent = ascent;
                currChunk.Baseline = baseline;
                currChunk.Descent = descent;
                currChunk.SingleWhiteSpaceWidth = spaceWidth;
                currLine.AddChunk(currChunk);
                pageMetrics.AddChunk(currChunk);
            }
            currChunk = new ChunkMetrics();
        }

        /// no-op method - this renderer isn't interested in image events
        ///             @see com.itextpdf.text.pdf.parser.RenderListener#renderImage(com.itextpdf.text.pdf.parser.ImageRenderInfo)
        ///             @since 5.0.1
        public virtual void RenderImage(ImageRenderInfo renderInfo){
                        
            string path = "/Users/ryan/RiderProjects/Sharpen Pdf Parser/Sharpen Pdf Parser/image_dump";
            
            try {
                String filename;
                FileStream os;
                PdfImageXObject imageObj;
                imageObj = renderInfo.GetImage();
                
                if (imageObj == null) return;

                float width = imageObj.GetWidth();
                float height = imageObj.GetHeight();
                
                ImageMetrics imageMetrics = new ImageMetrics(imageObj.GetImageBytes(), width, height);
                float x = renderInfo.GetImageCtm().Get(Matrix.I31);
                float y = renderInfo.GetImageCtm().Get(Matrix.I32);

                imageMetrics.BottomLeft = new Vector3D(x, y, 1);
                imageMetrics.TopLeft = new Vector3D(x, y + height, 1);
                imageMetrics.BottomRight = new Vector3D(x + width, y, 1);
                imageMetrics.TopRight = new Vector3D(x + width, y + height, 1);
                
                filename = path + renderInfo.GetImageResourceName() + "." + imageObj.IdentifyImageFileExtension();
                os = new FileStream(filename, FileMode.Create);
                os.Write(imageObj.GetImageBytes(), 0 , imageObj.GetImageBytes().Length);
                os.Flush();
                os.Close();
            } catch (Exception e) {
                Console.WriteLine(e.GetBaseException());
                pageMetrics.ErrorLoadingImages = true;
            }
        }

        public void EventOccurred(IEventData data, EventType type){
            if (!GetSupportedEvents().Contains(type)){
                return;
            }
            
            switch(type){
                case EventType.BEGIN_TEXT:
                    BeginTextBlock();
                    break;
                
                case EventType.RENDER_TEXT:
                    RenderText((TextRenderInfo) data);
                    break;
                    
                case EventType.END_TEXT:
                    EndTextBlock();
                    break;
                    
                case EventType.RENDER_IMAGE:
                    RenderImage((ImageRenderInfo) data);
                    break;
            }
        }

        public ICollection<EventType> GetSupportedEvents(){
            return new HashSet<EventType>(){
                EventType.RENDER_TEXT, 
                EventType.BEGIN_TEXT, 
                EventType.END_TEXT,
                EventType.RENDER_IMAGE
            };
        }

        public List<CharMetrics> CharMetrices => charMetrices;
    }
}