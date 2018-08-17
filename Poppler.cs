using System;
using System.Diagnostics;
using System.IO;

namespace PrickleParser{
    public class Poppler{
        
        private static string GS_PATH = Path.Combine(
            Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "Lib", "pdftocairo");
        private static string OUTPUT_DIR = Path.GetTempPath();

        
        public static void PdfToSvg(string input, string pageNum){
            if (File.Exists(input)){
                var p = new Process();
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.FileName = GS_PATH;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;

                p.StartInfo.Arguments = "-f " + pageNum + " -l " + pageNum + " -svg " + input + " " + Path.Combine(
                                            OUTPUT_DIR, Path.GetFileNameWithoutExtension(input) + ".svg");

                p.Start();

                Console.Write(p.StandardError.ReadToEnd()); // todo log error
                Console.Write(p.StandardOutput.ReadToEnd()); // todo log info

                p.WaitForExit(); 
            }
            else{
                Console.WriteLine("no file"); // todo log error
            }
            
        }
    }
}