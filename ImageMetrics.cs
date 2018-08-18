namespace PrickleParser{
    public class ImageMetrics{
        private string imageRef;
        private byte[] imageBytes;
        private string imageType;

        private Vector3D topRight;
        private Vector3D topLeft;
        private Vector3D bottomLeft;
        private Vector3D bottomRight;
        private float width;
        private float height;

        public ImageMetrics(byte[] imageBytes, float width, float height){
            this.imageBytes = imageBytes;
            this.width = width;
            this.height = height;
        }

        public byte[] ImageBytes{
            get{ return imageBytes; }
        }

        public string ImageRef{
            get{ return imageRef; }
            set{ imageRef = value; }
        }

        public string ImageType{
            get{ return imageType; }
            set{ imageType = value; }
        }

        public Vector3D TopRight{
            get{ return topRight; }
            set{ topRight = value; }
        }

        public Vector3D TopLeft{
            get{ return topLeft; }
            set{ topLeft = value; }
        }

        public Vector3D BottomLeft{
            get{ return bottomLeft; }
            set{ bottomLeft = value; }
        }

        public Vector3D BottomRight{
            get{ return bottomRight; }
            set{ bottomRight = value; }
        }

        public float Width{
            get{ return width; }
            set{ width = value; }
        }

        public float Height{
            get{ return height; }
            set{ height = value; }
        }
    }
}