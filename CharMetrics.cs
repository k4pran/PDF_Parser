namespace PrickleParser{
    public class CharMetrics{
        private char c;

        private float fontSize;

        private Vector3D topLeft;
        private Vector3D bottomLeft;
        private Vector3D topRight;
        private Vector3D bottomRight;

        public CharMetrics(char c){
            this.c = c;
        }

        public char C => c;

        public float FontSize{
            get => fontSize;
            set => fontSize = value;
        }

        public Vector3D TopLeft{
            get => topLeft;
            set => topLeft = value;
        }

        public Vector3D BottomLeft{
            get => bottomLeft;
            set => bottomLeft = value;
        }

        public Vector3D TopRight{
            get => topRight;
            set => topRight = value;
        }

        public Vector3D BottomRight{
            get => bottomRight;
            set => bottomRight = value;
        }
    }
}