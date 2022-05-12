using System.Numerics;

namespace OpenGLTest
{
    public class Camera
    {
        public Camera(CameraAttribute attribute, Vector3 position, Vector3 front, Vector3 up)
        {
            Attribute = attribute;
            Position = position;
            Front = front;
            Up = up;
        }

        public Camera(CameraAttribute attribute) : this(attribute, new Vector3(0, 0, 0), new Vector3(0, 0, -1), new Vector3(0, 1, 0)) { }
        public CameraAttribute Attribute { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Front { get; set; }
        public Vector3 Up { get; set; }
        public Matrix4x4 GetViewMatrix() => Matrix4x4.CreateLookAt(Position, Position + Front, Up);
        public Matrix4x4 GetProjectionMatrix(float width, float height) => Attribute.GetProjectionMatrix(width, height);
    }

    public class CameraAttribute
    {
        private float _fov = 45;
        private float _near;
        private float _far;
        private float _height;
        private CameraType _type = CameraType.PERSPECTIVE;

        public event EventHandler? Change;
        public float Fov
        {
            get => _fov;
            set
            {
                if (value <= 0 || value > 180)
                {
                    throw new ArgumentOutOfRangeException("Perspective field of view has to be a positive number between 0 and 180 degrees");
                }
                this._fov = value;
                if (Type == CameraType.PERSPECTIVE) OnChange();
            }
        }

        public float Near { get => _near; set { _near = value; OnChange(); } }

        public float Far { get => _far; set { _far = value; OnChange(); } }

        public float Height
        {
            get => _height; set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("Orthographic view height has to be a positive number");
                }
                this._height = value;
                if (Type == CameraType.ORTHOGONAL) OnChange();
            }
        }

        public CameraType Type { get => _type; set { _type = value; OnChange(); } }

        public Matrix4x4 GetProjectionMatrix(float width, float height)
        {
            var aspect = width / height;
            switch (_type)
            {
                case CameraType.PERSPECTIVE:
                    return Matrix4x4.CreatePerspectiveFieldOfView(this.Fov * MathF.PI / 180.0f, aspect, _near, _far);
                case CameraType.ORTHOGONAL:
                    var h = height;
                    var w = h * aspect;
                    return Matrix4x4.CreateOrthographicOffCenter(w / -2, w / 2, h / -2, h / 2, _near, _far);
                default:
                    throw new ArgumentOutOfRangeException("Undefined camera type");
            }
        }

        public void OnChange()
        {
            Change?.Invoke(this, EventArgs.Empty);
        }
    }

    public enum CameraType
    {
        PERSPECTIVE = 0,
        ORTHOGONAL = 1
    }
}