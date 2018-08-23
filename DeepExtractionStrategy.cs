using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
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
        private ChunkMetrics currChunk;

        private Vector bottomLeft;
        private Vector topLeft;
        private Vector bottomRight;
        private Vector topRight;
        
        private float ascent;
        private float baseline;
        private float descent;

        private bool startOfChunk;

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
            this.charMetrices = new List<CharMetrics>();
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
    
        /// Used to actually append text to the text results.  Subclasses can use this to insert
        ///     text that wouldn't normally be included in text parsing (e.g. result of OCR performed against
        ///     image content)
        ///     @param text the text to append to the text results accumulated so far
        protected void AppendTextChunk(string text) {
            result.Append(text);
        }
    
        /// Captures text using a simplified algorithm for inserting hard returns and spaces
        ///     @param   renderInfo  render info
        public virtual void RenderText(TextRenderInfo renderInfo){
            bool flagNoText = result.Length == 0;
            bool flagNewLine = false;
            
            CharMetrics charMetrics;
            if (startOfChunk && pageMetrics.ChunkMetrices.Count > 0){
                charMetrics = new CharMetrics(' ');
                charMetrics.BottomLeft = new Vector3D(bottomLeft);
                charMetrics.TopLeft = new Vector3D(topLeft);
                charMetrics.BottomRight = new Vector3D(renderInfo.GetBaseline().GetStartPoint());
                charMetrics.TopRight = new Vector3D(renderInfo.GetAscentLine().GetStartPoint());
                charMetrices.Add(charMetrics);
            }
            
            foreach(TextRenderInfo charInfo in renderInfo.GetCharacterRenderInfos()){

                if (charInfo.GetText().Length == 0){
                    continue;
                }
                charMetrics = new CharMetrics(charInfo.GetText()[0]);
                charMetrics.FontSize = charInfo.GetFontSize();
                charMetrics.BottomLeft = new Vector3D(charInfo.GetBaseline().GetStartPoint());
                charMetrics.TopLeft = new Vector3D(charInfo.GetAscentLine().GetStartPoint());
                charMetrics.BottomRight = new Vector3D(charInfo.GetBaseline().GetEndPoint());
                charMetrics.TopRight = new Vector3D(charInfo.GetAscentLine().GetEndPoint());
                charMetrices.Add(charMetrics);
            }

            if (startOfChunk){
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
            
            if (flagNoText && renderInfo.GetText().Length > 0){
                StartNewWord();
            }

            string currFont = renderInfo.GetFont() != null ? renderInfo.GetFont().ToString() : "";
            if (currChunk.FontFamily == null && currFont.Length > 0){
                currChunk.FontFamily = currFont;
            }
            
            //Check if faux bold is used
            if ((renderInfo.GetTextRenderMode() == (int)TextRenderMode.FillThenStrokeText) || // todo include render mode for outsets etc?
                    renderInfo.GetFont().GetFontProgram().GetFontNames().IsBold()){
                currChunk.Bold = true;
            }

            if (renderInfo.GetFont().GetFontProgram().GetFontMetrics().GetItalicAngle() != 0 ||
                    renderInfo.GetFont().GetFontProgram().GetFontNames().IsItalic()){
                currChunk.Italic = true;
            }
            
            if (currChunk.FillColor == null && renderInfo.GetFillColor() != null){
                currChunk.FillColor = ProcessColorInfo(renderInfo.GetFillColor());
            }     
                        
            if (currChunk.StrokeColor == null && renderInfo.GetStrokeColor() != null){
                currChunk.StrokeColor = ProcessColorInfo(renderInfo.GetStrokeColor());
            }    
            
            AppendTextChunk(renderInfo.GetText());
            currChunk.Append(renderInfo.GetText());
        }

        public Color ProcessColorInfo(iText.Kernel.Colors.Color color){
            if (color.GetColorSpace() is PdfDeviceCs.Cmyk){
                color = iText.Kernel.Colors.Color.ConvertCmykToRgb((DeviceCmyk) color);
            }
            
            if (color.GetNumberOfComponents() == 1){
                return new Color((int) color.GetColorValue()[0]); // todo - cast to ints ?
            }
                
            if (color.GetNumberOfComponents() == 3){
                return new Color(color.GetColorValue()[0], 
                                 color.GetColorValue()[1], 
                                 color.GetColorValue()[2]);
            }
            return null; // todo handle
        }

        /// no-op method - this renderer isn't interested in image events
        ///             @see com.itextpdf.text.pdf.parser.RenderListener#renderImage(com.itextpdf.text.pdf.parser.ImageRenderInfo)
        ///             @since 5.0.1
        public virtual void RenderImage(ImageRenderInfo renderInfo){
                        
            string path = "/Users/ryan/RiderProjects/IText Play/IText Play/Assets";
            
            try {
                String filename;
                FileStream os;
                PdfImageXObject imageObj;
                imageObj = renderInfo.GetImage();
                
                if (imageObj == null || !imageObj.GetPdfObject().ContainsKey(PdfName.BitsPerComponent)) return;

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
            } catch (iText.IO.IOException e) {
                Console.WriteLine(e.GetBaseException());
            }
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
                pageMetrics.AddWord(currChunk);
            }
            currChunk = new ChunkMetrics();
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