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

        public byte[] ImageBytes => imageBytes;

        public string ImageRef{
            get => imageRef;
            set => imageRef = value;
        }

        public string ImageType{
            get => imageType;
            set => imageType = value;
        }

        public Vector3D TopRight{
            get => topRight;
            set => topRight = value;
        }

        public Vector3D TopLeft{
            get => topLeft;
            set => topLeft = value;
        }

        public Vector3D BottomLeft{
            get => bottomLeft;
            set => bottomLeft = value;
        }

        public Vector3D BottomRight{
            get => bottomRight;
            set => bottomRight = value;
        }

        public float Width{
            get => width;
            set => width = value;
        }

        public float Height{
            get => height;
            set => height = value;
        }
    }
}