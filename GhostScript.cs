using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;

namespace PrickleParser{
    public class GhostScript{
        
        private static string GS_PATH = Path.Combine(
            Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "Lib", "gs");
        private static string OUTPUT_DIR = Path.GetTempPath();

        public static void RemoveText(string input){
            
            if (File.Exists(input)){
                var p = new Process();
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.FileName = GS_PATH;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;

                p.StartInfo.Arguments = "-o " + Path.Combine(OUTPUT_DIR, "no_text_" + Path.GetFileName(input)) + 
                                        " -sDEVICE=pdfwrite -dFILTERTEXT " + input;

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