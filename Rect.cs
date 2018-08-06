using iTextSharp.text.pdf.parser;

namespace PrickleParser{
    public class Rect{
        private Vector3D topLeft;
        private Vector3D topRight;
        private Vector3D bottomLeft;
        private Vector3D bottomRight;
        private float width;
        private float height;

        public Rect(Vector3D topLeft, Vector3D topRight, Vector3D bottomLeft, Vector3D bottomRight){
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomLeft = bottomLeft;
            this.bottomRight = bottomRight;
        }
        
        public float Width{
            get => (topRight.X - topLeft.X);
        }

        public float Height{
            get => (topLeft.Y - bottomLeft.Y);
        }
    }

    public class Vector3D{
        private float x;
        private float y;
        private float z;

        public Vector3D(float x, float y, float z){
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3D(Vector v){
            this.x = v[Vector.I1];
            this.y = v[Vector.I2];
            this.z = v[Vector.I3];
        }

        public float X => x;

        public float Y => y;

        public float Z => z;
    }
}