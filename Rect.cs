using iText.Kernel.Geom;

namespace PrickleParser{
    public class Rect{
        private Vector3D topLeft;
        private Vector3D topRight;
        private Vector3D bottomLeft;
        private Vector3D bottomRight;
        private float width;
        private float height;

        public Rect(Vector3D bottomLeft, Vector3D bottomRight, Vector3D topLeft, Vector3D topRight){
            this.bottomLeft = bottomLeft;
            this.bottomRight = bottomRight;
            this.topLeft = topLeft;
            this.topRight = topRight;
        }

        public Rectangle ToITextRectangle(){
            return new Rectangle(bottomLeft.X, bottomLeft.Y, topRight.X, topRight.Y);
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
            this.x = v.Get(Vector.I1);
            this.y = v.Get(Vector.I2);
            this.z = v.Get(Vector.I3);
        }

        public float X => x;

        public float Y => y;

        public float Z => z;
    }
}