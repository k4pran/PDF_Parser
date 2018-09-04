using System.Text;
using Newtonsoft.Json;

namespace PrickleParser{
    public class Color{
        private int MAX_VALUE = 1;
        private ColorFormat format;

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
            switch(format){
                case ColorFormat.RGB:
                    sb.Append(PercentTo8Bit(red).ToString("X2"));
                    sb.Append(PercentTo8Bit(green).ToString("X2"));
                    sb.Append(PercentTo8Bit(blue).ToString("X2"));
                    sb.Append(PercentTo8Bit(alpha).ToString("X2"));
                    return sb.ToString();

                case ColorFormat.GRAYSCALE:

                    int hexVal = PercentTo8Bit(graylevel);
                    sb.AppendFormat("{0:X2}{1:X2}{2:X2}{3:X2}", hexVal, hexVal, hexVal, PercentTo8Bit(alpha));
                    return sb.ToString();
                
                default:
                    return "#000000FF";
            }
        }

        public static int PercentTo8Bit(float value){
            return (int)(255f / 100f * (int)(value * 100));
        }

        public Color(float graylevel){
            this.graylevel = graylevel;
            format = ColorFormat.GRAYSCALE;
            alpha = MAX_VALUE;
        }

        public Color(float red, float green, float blue, float alpha){
            this.red = red;
            this.green = green;
            this.blue = blue;
            this.alpha = alpha;
            format = ColorFormat.RGB;
        }
        
        public Color(float red, float green, float blue){
            this.red = red;
            this.green = green;
            this.blue = blue;
            alpha = MAX_VALUE;
            format = ColorFormat.RGB;
        }
       
        [JsonConstructor]
        public Color(ColorFormat format, float graylevel, float red, float green, float blue, float alpha){
            this.format = format;
            this.graylevel = graylevel;
            this.red = red;
            this.green = green;
            this.blue = blue;
            this.alpha = alpha;
        }

        public ColorFormat Format => format;

        public float Graylevel => graylevel;

        public float Red => red;

        public float Green => green;

        public float Blue => blue;

        public float Alpha => alpha;
    }
}