namespace PrickleParser{
    public class Color{
        private int MAX_VALUE = 255;
        private ColorFormat colorFormat;

        private int graylevel;
        
        private int red;
        private int green;
        private int blue;
        private int alpha;
        
        private enum ColorFormat{
            grayscale = 0,
            rgb       = 1
        }

        public Color(int graylevel){
            this.graylevel = graylevel;
            this.colorFormat = ColorFormat.grayscale;
        }

        public Color(int red, int green, int blue, int alpha){
            this.red = red;
            this.green = green;
            this.blue = blue;
            this.alpha = alpha;
            this.colorFormat = ColorFormat.rgb;
        }
        
        public Color(int red, int green, int blue){
            this.red = red;
            this.green = green;
            this.blue = blue;
            this.alpha = MAX_VALUE;
        }
    }
}