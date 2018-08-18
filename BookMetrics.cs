using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using Newtonsoft.Json;

namespace PrickleParser{
    
    public class BookMetrics{

        private List<PageMetrics> pages;
        private int nbPages;
        private string author;
        private string title;
        private string date;
        private string publisher;

        public BookMetrics(){
            pages = new List<PageMetrics>();
            nbPages = 0;
            Author = "";
            title = "";
            date = "";
            publisher = "";
        }

        public void AddPage(PageMetrics page){
            pages.Add(page);
            nbPages++;
        }

        public void ToJson(string output){
            try{
                File.WriteAllText(output, JsonConvert.SerializeObject(this, Formatting.Indented));
            }
            catch(Exception e){
                if (e is UnauthorizedAccessException || e is SecurityException){
                    e.Data.Add("WriteJsonError", "Failed to write json to path: " + output + 
                                                 " as you do not have the required permissions");
                    throw;
                }
                if (e is NotSupportedException){
                    e.Data.Add("WriteJsonError", "Failed to write json to path: " + output +
                                                 " as path is not in a valid format");
                    throw;
                }
                e.Data.Add("WriteJsonError", "Failed to write json to path: " + output);
                throw;
            }
        }

        public static BookMetrics FromJson(string input){
            if (File.Exists(input)){
                try{
                    return JsonConvert.DeserializeObject<BookMetrics>(File.ReadAllText(input));
                }
                catch(Exception e){
                    if (e is UnauthorizedAccessException || e is SecurityException){
                        e.Data.Add("ReadJsonError", "Failed to read json to path: " + input + 
                                                     " as you do not have the required permissions");
                        throw;
                    }
                    if (e is NotSupportedException){
                        e.Data.Add("ReadJsonError", "Failed to read json to path: " + input +
                                                     " as path is not in a valid format");
                        throw;
                    }
                    e.Data.Add("ReadJsonError", "Failed to read json data from path: " + input);
                    throw;                    
                }
            }
            throw new Exception("Input file not found at path: " + input);
        }

        public List<PageMetrics> Pages => pages;

        public int NbPages => nbPages;

        public string Author{
            get => author;
            set => author = value;
        }

        public string Title{
            get => title;
            set => title = value;
        }

        public string Date{
            get => date;
            set => date = value;
        }

        public string Publisher{
            get => publisher;
            set => publisher = value;
        }
    }
}