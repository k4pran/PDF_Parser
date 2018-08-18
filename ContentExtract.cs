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

            int nbPages = pdfDoc.GetNumberOfPages();
            
            for(int i = 0; i < nbPages; i++){
                Rectangle size = pdfDoc.GetPage(i + 1).GetPageSize();
                
                PageMetrics page = new PageMetrics(i + 1);
                page.Width = size.GetWidth();
                page.Height = size.GetHeight();
                page.Rotation = pdfDoc.GetPage(i + 1).GetRotation(); 

                DeepExtractionStrategy strategy = new DeepExtractionStrategy(ref page);
                Console.WriteLine("Processing page {0}", i + 1);
                page.Text = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i + 1), strategy);
                page.BuildLines();
                page.DetermineLineSpacing();
                book.AddPage(page);
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