using System;
using System.Text;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

namespace PrickleParser{
    internal class Program{
        public static void Main(string[] args){
            PdfDocument pdfDoc = new PdfDocument(
                new PdfReader("/Users/ryan/RiderProjects/IText Play/IText Play/Assets/Artificial Intelligence A Modern Approach.pdf"), 
                new PdfWriter("/Users/ryan/RiderProjects/IText Play/IText Play/Assets/cleaned.pdf"));


            EncodingProvider codePages = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(codePages);
            
            pdfDoc.GetNumberOfPages();
            
            BookMetrics bookMetrics = new BookMetrics();
            bookMetrics.Author = pdfDoc.GetDocumentInfo().GetAuthor();
            bookMetrics.Title = pdfDoc.GetDocumentInfo().GetTitle();
            bookMetrics.Publisher = pdfDoc.GetDocumentInfo().GetProducer();

            bookMetrics.NbPages = pdfDoc.GetNumberOfPages();
            
            for(int i = 0; i < bookMetrics.NbPages; i++){
                Rectangle size = pdfDoc.GetPage(i + 1).GetPageSize();
                
                PageMetrics pdfPageMetrics = new PageMetrics(i + 1);
                pdfPageMetrics.Width = size.GetWidth();
                pdfPageMetrics.Height = size.GetHeight();
                pdfPageMetrics.Rotation = pdfDoc.GetPage(i + 1).GetRotation(); 

                DeepExtractionStrategy strategy = new DeepExtractionStrategy(ref pdfPageMetrics);
                Console.WriteLine("Processing page {0}", i + 1);
                pdfPageMetrics.Text = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i + 1), strategy);
                pdfPageMetrics.BuildLines();
                pdfPageMetrics.DetermineLineSpacing();
            }    
            
            pdfDoc.Close();
        }
    }
}