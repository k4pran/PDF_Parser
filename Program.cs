using Path = System.IO.Path;

namespace PrickleParser{
    internal class Program{
        public static void Main(string[] args){
                                                
            // Extract pdf content
            ContentExtract.ExtractPdf(
                "/Users/ryan/RiderProjects/Sharpen Pdf Parser/Sharpen Pdf Parser/Resources/affordances.pdf",
                Path.Combine(Path.GetTempPath(), "test.pdf"));
            BookMetrics book = ContentExtract.Book;
            book.ToJson("/Users/ryan/RiderProjects/Sharpen Pdf Parser/test.json");
            
            BookMetrics bookTest = BookMetrics.FromJson("/Users/ryan/RiderProjects/Sharpen Pdf Parser/test.json");
            // Remove text from pdf
            GhostScript.RemoveText("/Users/ryan/Documents/Books/affordances.pdf");
            
            // Convert page to svg
            Poppler.PdfToSvg("/Users/ryan/Documents/Books/affordances.pdf", "1");
            
            // Preparing SVG for unity - temporary until they fix issues
            Svg.RenderEmptyPathsExplicit("/Users/ryan/RiderProjects/Sharpen Pdf Parser/Sharpen Pdf Parser/Resources/test.svg");
            Svg.RenderRgbAsHex("/Users/ryan/RiderProjects/Sharpen Pdf Parser/Sharpen Pdf Parser/Resources/test.svg");
            Svg.WriteSvg("/Users/ryan/Documents/Books/parsed2.svg");
        }
    }
}