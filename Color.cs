using System.Text;
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
            GRAYSCALE = 0,
            RGB       = 1
        }

        public string AsHex(){
            StringBuilder sb = new StringBuilder();
            sb.Append("#");
            switch(colorFormat){
                case ColorFormat.RGB:
                    sb.Append(red.ToString("X"));
                    sb.Append(green.ToString("X"));
                    sb.Append(blue.ToString("X"));
                    sb.Append(alpha.ToString("X"));
                    return sb.ToString();

                case ColorFormat.GRAYSCALE:

                    string hexVal = graylevel.ToString("X");
                    sb.AppendFormat("{0}{1}{2}{3}", hexVal, hexVal, hexVal, alpha);
                    return sb.ToString();
                
                default:
                    return "#00000000";
            }
        }

        public Color(float graylevel){
            this.graylevel = graylevel;
            this.colorFormat = ColorFormat.GRAYSCALE;
        }

        public Color(float red, float green, float blue, float alpha){
            this.red = red;
            this.green = green;
            this.blue = blue;
            this.alpha = alpha;
            this.colorFormat = ColorFormat.RGB;
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