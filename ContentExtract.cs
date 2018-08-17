using System;
using System.Text;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

namespace PrickleParser{
    public class ContentExtract{

        private static BookMetrics book;
        
        public static void ExtractPdf(string input, string output){
            
            PdfDocument pdfDoc = new PdfDocument(
                new PdfReader(input), new PdfWriter(output));
            
            EncodingProvider codePages = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(codePages);
            
            pdfDoc.GetNumberOfPages();
            
            book = new BookMetrics();
            book.Author = pdfDoc.GetDocumentInfo().GetAuthor();
            book.Title = pdfDoc.GetDocumentInfo().GetTitle();
            book.Publisher = pdfDoc.GetDocumentInfo().GetProducer();

            book.NbPages = pdfDoc.GetNumberOfPages();
            
            for(int i = 0; i < book.NbPages; i++){
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

        public static void WritePdf(){
            if (book == null){
                throw new PdfNotExtractedException("Must call 'ExtractPdf()' method before writing pdf");
            }
        }

        public static BookMetrics Book => book;
    }

    class PdfNotExtractedException : Exception{
        public PdfNotExtractedException(string message) : base(message){
        }
    }
}