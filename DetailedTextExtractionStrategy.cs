﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Xobject;

namespace PrickleParser{
    public class DeepExtractionStrategy : ITextExtractionStrategy {
        /// used to store the resulting String.

        private PageMetrics pdfPageMetrics;
        
        private StringBuilder result = new StringBuilder();
        private WordMetrics currWord;
        private bool flagNewWord = true;

        private Vector lastBottomLeft;
        private Vector lastTopRight;
        private Vector lastBottomRight;
    
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

        public DeepExtractionStrategy(ref PageMetrics pdfPageMetrics){
            this.pdfPageMetrics = pdfPageMetrics;
        }

        public virtual void BeginTextBlock() {
        }
    
        public virtual void EndTextBlock() {
            if (currWord != null){
                currWord.BottomRight = new Vector3D(lastBottomRight);
                currWord.TopRight = new Vector3D(lastTopRight);
                pdfPageMetrics.AddWord(currWord);
            }
            result.Append("</span>");
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
    
        protected void AppendTextChunk(char text) {
            result.Append(text);
        }
    
        /// Captures text using a simplified algorithm for inserting hard returns and spaces
        ///     @param   renderInfo  render info
        public virtual void RenderText(TextRenderInfo renderInfo) {
            bool flagNoText = result.Length == 0;
            bool flagNewLine = false;
            if (flagNoText && renderInfo.GetText().Length > 0){
                StartNewWord(renderInfo);
            }
           
            LineSegment baseline = renderInfo.GetBaseline();
            Vector bottomLeft = baseline.GetStartPoint();
            Vector bottomRight = baseline.GetEndPoint();
            Vector topRight = renderInfo.GetAscentLine().GetEndPoint();


            string currFont = renderInfo.GetFont() != null ? renderInfo.GetFont().ToString() : "";
            if (currWord.FontFamily == null && currFont.Length > 0){
                currWord.FontFamily = currFont;
            }
            
            //Check if faux bold is used
            if ((renderInfo.GetTextRenderMode() == (int)TextRenderMode.FillThenStrokeText) || // todo include render mode for outsets etc?
                    renderInfo.GetFont().GetFontProgram().GetFontNames().IsBold()){
                currWord.Bold = true;
            }
                        
            
            Rectangle rect = new Rectangle(bottomLeft.Get(Vector.I1), bottomLeft.Get(Vector.I2), topRight.Get(Vector.I1), topRight.Get(Vector.I2));
            Single currFontSize = rect.GetHeight();
            

            if (renderInfo.GetFont().GetFontProgram().GetFontMetrics().GetItalicAngle() != 0 ||
                    renderInfo.GetFont().GetFontProgram().GetFontNames().IsItalic()){
                currWord.Italic = true;
            }
            
            if (currWord.FillColor == null && renderInfo.GetFillColor() != null){
                currWord.FillColor = ProcessColorInfo(renderInfo.GetFillColor());
            }     
                        
            if (currWord.StrokeColor == null && renderInfo.GetStrokeColor() != null){
                currWord.StrokeColor = ProcessColorInfo(renderInfo.GetStrokeColor());
            }    
            
            if (!flagNoText){
                Vector v = bottomLeft;
                if ((lastBottomRight.Subtract(lastBottomLeft).Cross(lastBottomLeft.Subtract(v)).LengthSquared()) / (lastBottomRight.Subtract(lastBottomLeft).LengthSquared()) > 1.0){
                    flagNewLine = true;
                }
            }


            if (flagNewLine){
                AppendTextChunk('\n');
                flagNewWord = true;
                StartNewWord(renderInfo);
            }
            else if (!flagNoText && result[result.Length - 1] == ' ' &&
                     (renderInfo.GetText().Length > 0 && renderInfo.GetText()[0] != ' ') &&
                     lastBottomRight.Subtract(bottomLeft).Length() > renderInfo.GetSingleSpaceWidth() / 2.0){
                AppendTextChunk(' ');
                StartNewWord(renderInfo);
            }
            
            AppendTextChunk(renderInfo.GetText());
            currWord.Append(renderInfo.GetText());
            
            lastBottomLeft = bottomLeft;
            lastFontSize = currFontSize;
            lastFont = currFont;
            lastTopRight = topRight;
            lastBottomRight = bottomRight;
        }

        public Color ProcessColorInfo(iText.Kernel.Colors.Color color){
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
            
            // todo handle seperation color spaces? -- used for printing
            
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
        private void StartNewWord(TextRenderInfo renderInfo){
            if (currWord != null){
                currWord.BottomRight = new Vector3D(lastBottomRight);
                currWord.TopRight = new Vector3D(lastTopRight);
                pdfPageMetrics.AddWord(currWord);
            }

            currWord = new WordMetrics();
            currWord.BottomLeft = new Vector3D(renderInfo.GetBaseline().GetStartPoint());
            currWord.TopLeft = new Vector3D(renderInfo.GetAscentLine().GetStartPoint());
            currWord.Baseline = renderInfo.GetBaseline().GetStartPoint().Get(Vector.I2);
            currWord.Descent = renderInfo.GetDescentLine().GetStartPoint().Get(Vector.I2);
            currWord.Ascent = renderInfo.GetAscentLine().GetStartPoint().Get(Vector.I2);
            currWord.SingleWhiteSpaceWidth = renderInfo.GetSingleSpaceWidth();
            flagNewWord = false;
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
    }
}