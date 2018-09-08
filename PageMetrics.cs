using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iText.Kernel.Geom;

namespace PrickleParser{
    public class PageMetrics{

        private int pageNumber;
        private List<LineMetrics> lineMetrices;
        private List<ChunkMetrics> chunkMetrices;
        private List<WordMetrics> wordMetrices;
        private List<CharMetrics> charMetrices;
        
        private List<ImageMetrics> imageMetrices;
        private bool errorLoadingImages = false;         
        
        private string text;
        private float width;
        private float height;
        private float rotation;
        private float avgLineSpacing;

        public PageMetrics(int pageNumber){
            this.pageNumber = pageNumber;
            lineMetrices = new List<LineMetrics>();
            chunkMetrices = new List<ChunkMetrics>();
            wordMetrices = new List<WordMetrics>();
            charMetrices = new List<CharMetrics>();
            imageMetrices = new List<ImageMetrics>();
            
            text = "";
        }

        public void SortLines(){
            lineMetrices.Sort();
        }

        public void SortChunks(){
            chunkMetrices.Sort();
        }

        public void SortWords(){
            wordMetrices.Sort();
        }

        public void SortChars(){
            charMetrices.Sort();
        }

        public void AddLine(LineMetrics line){
            lineMetrices.Add(line);
        }

        public void AddChunk(ChunkMetrics chunk){
            chunkMetrices.Add(chunk);
        }

        public void PrintLines(){
            foreach(LineMetrics line in lineMetrices){
                StringBuilder sb = new StringBuilder();
                foreach(ChunkMetrics chunkMetric in line.ChunkMetrices){
                    sb.Append(chunkMetric.ChunkStr);
                    sb.Append(" ");
                }
                Console.WriteLine(sb.ToString());
            }
        }

        public int PageNumber{
            get => pageNumber;
            set => pageNumber = value;
        }
        
        public string Text{
            get => text;
            set => text = value;
        }

        public List<LineMetrics> LineMetrices{
            get => lineMetrices;
            set => lineMetrices = value;
        }

        public List<ChunkMetrics> ChunkMetrices{
            get => chunkMetrices;
            set => chunkMetrices = value;
        }

        public List<WordMetrics> WordMetrices{
            get => wordMetrices;
            set => wordMetrices = value;
        }

        public List<CharMetrics> CharMetrices{
            get => charMetrices;
            set => charMetrices = value;
        }

        public List<ImageMetrics> ImageMetrices{
            get => imageMetrices;
            set => imageMetrices = value;
        }

        public bool ErrorLoadingImages{
            get => errorLoadingImages;
            set => errorLoadingImages = value;
        }

        public float Width{
            get => width;
            set => width = value;
        }

        public float Height{
            get => height;
            set => height = value;
        }

        public float Rotation{
            get => rotation;
            set => rotation = value;
        }
    }
}