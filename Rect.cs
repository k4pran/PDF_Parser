using Newtonsoft.Json;

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

        public float Width{
            get{ return width; }
        }

        public float Height{
            get{ return height; }
        }
    }

    public class Vector3D{
        private float x;
        private float y;
        private float z;

        [JsonConstructor]
        public Vector3D(float x, float y, float z){
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public float X{
            get{ return x; }
        }

        public float Y{
            get{ return y; }
        }

        public float Z{
            get{ return z; }
        }
    }
}