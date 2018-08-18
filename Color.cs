using Newtonsoft.Json;

namespace PrickleParser{
    public class Color{
        private int MAX_VALUE = 255;
        private ColorFormat colorFormat;

        private float graylevel;
        
        private float red;
        private float green;
        private float blue;
        private float alpha;
        
        public enum ColorFormat{
            grayscale = 0,
            rgb       = 1
        }

        public Color(float graylevel){
            this.graylevel = graylevel;
            this.colorFormat = ColorFormat.grayscale;
        }

        public Color(float red, float green, float blue, float alpha){
            this.red = red;
            this.green = green;
            this.blue = blue;
            this.alpha = alpha;
            this.colorFormat = ColorFormat.rgb;
        }
        
        public Color(float red, float green, float blue){
            this.red = red;
            this.green = green;
            this.blue = blue;
            this.alpha = MAX_VALUE;
        }

        [JsonConstructor]
        public Color(ColorFormat colorFormat, float graylevel, float red, float green, float blue, float alpha){
            this.colorFormat = colorFormat;
            this.graylevel = graylevel;
            this.red = red;
            this.green = green;
            this.blue = blue;
            this.alpha = alpha;
        }
    }
}